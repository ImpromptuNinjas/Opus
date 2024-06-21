using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Silk.NET.OpenAL;
using Silk.NET.OpenAL.Extensions.EXT;

namespace ImpromptuNinjas.Opus.OpenAlDemo;

public static class Program {

  private const int SleepIntervalMs = 5;

  private const int MaxOpenAlBuffersQueued = 8;

  public static unsafe void Main(string[] args) {
    var al = AL.GetApi();
    var alc = ALContext.GetApi();
    //var floatExt = al.GetExtension<FloatFormat>();
    if (!al.TryGetExtension<FloatFormat>(out var floatExt))
      ThrowIfError(al);
    //ThrowIfError(al);
    var dev = alc.OpenDevice(null);
    ThrowIfError(alc, dev);

    var ctx = alc.CreateContext(dev, null);
    alc.MakeContextCurrent(ctx);
    ThrowIfError(alc, dev);

    var src = al.GenSource();
    al.SourceQueueBuffers(src, 0, null);
    ThrowIfError(al);

    var bufPool = new Queue<uint>();

#if USE_FILE_STREAMS
      var opusFiles = new Lazy<OpusStreamedFile>?[args.Length];
      for (var i = 0; i < args.Length; ++i) {
        var filePath = args[i];
        opusFiles[i] = new Lazy<OpusStreamedFile>(() => new OpusStreamedFile(filePath));
      }
#else
    var opusFiles = new Lazy<OpusMappedFile>?[args.Length];
    for (var i = 0; i < args.Length; ++i) {
      var filePath = args[i];
      opusFiles[i] = new Lazy<OpusMappedFile>(() => new OpusMappedFile(filePath));
    }
#endif

    OpusDecoder* decoder = null;

    Console.WriteLine($"Streaming {args.Length} files.");

    for (var f = 0; f < opusFiles.Length; f++) {
      using var opusFile = opusFiles[f]!.Value;
      //Console.Write("                                    \r");
      Console.WriteLine(args[f]);
      var channelCount = opusFile.ChannelCount;
      if (decoder == null)
        decoder = OpusDecoder.Create(OpusFrequency._48kHz, channelCount);
      else
        decoder->Init(OpusFrequency._48kHz, channelCount);
      var pool = ArrayPool<byte>.Shared;
      // ReSharper disable once ForCanBeConvertedToForeach
      foreach (var packet in opusFile) {
        var segment = (ReadOnlySpan<byte>) packet;
        //var frames = OpusDecoder.GetFrames(segment);
        //var sampleCount = decoder->GetSamples(bytePtr, length);
        var sampleCount = OpusDecoder.GetSamples(segment, OpusFrequency._48kHz);
        if (sampleCount == 0)
          throw new NotImplementedException();

        if (!bufPool.TryDequeue(out var buf)) {
          buf = al.GenBuffer();
          //Console.WriteLine($"Created buffer {buf}");
        }
        else {
          //Console.WriteLine($"Recycling buffer {buf}");
        }

        ThrowIfError(al);
        byte[] buffer;
        if (floatExt != null) {
          var minBufSize = channelCount * sampleCount * sizeof(float);
          buffer = pool.Rent(minBufSize);
          fixed (byte* pBuffer = buffer) {
            var bufferSpan = MemoryMarshal.Cast<byte, float>(new Span<byte>(pBuffer, minBufSize));
            var decodedSampleCount = decoder->Decode(segment, bufferSpan, sampleCount, false);
            var decodedBufSize = channelCount * decodedSampleCount * sizeof(float);
            floatExt.BufferData(buf, FloatBufferFormat.Stereo, pBuffer, decodedBufSize, 48000);
            ThrowIfError(al);
          }
        }
        else {
          var minBufSize = channelCount * sampleCount * sizeof(short);
          buffer = pool.Rent(minBufSize);
          fixed (byte* pBuffer = buffer) {
            var bufferSpan = MemoryMarshal.Cast<byte, short>(new Span<byte>(pBuffer, minBufSize));
            var decodedSampleCount = decoder->Decode(segment, bufferSpan, sampleCount, false);
            var decodedBufSize = channelCount * decodedSampleCount * sizeof(short);
            al.BufferData(buf, BufferFormat.Stereo16, pBuffer, decodedBufSize, 48000);
            ThrowIfError(al);
          }
        }

        pool.Return(buffer);
        al.SourceQueueBuffers(src, 1, &buf);
        ThrowIfError(al);

        al.GetSourceProperty(src, GetSourceInteger.BuffersQueued, out var bufsQueued);

        //Console.Write($"Buffer Queue: {bufsQueued} (Pool: {bufPool.Count})            \r");

        if (bufsQueued >= 1) {
          al.GetSourceProperty(src, GetSourceInteger.SourceState, out var srcState);
          ThrowIfError(al);
          if (srcState != (int) SourceState.Playing) {
            al.SourcePlay(src);
            ThrowIfError(al);
          }
        }
        else {
          al.SourcePause(src);
          ThrowIfError(al);
        }

        while (bufsQueued >= MaxOpenAlBuffersQueued) {
          //alc.ProcessContext(ctx);
          ThrowIfError(alc, dev);
          ProcessBufferPool(al, bufPool, src);
          //Thread.Yield();
          Thread.Sleep(SleepIntervalMs);
          al.GetSourceProperty(src, GetSourceInteger.BuffersQueued, out bufsQueued);
          ThrowIfError(alc, dev);
          //Console.Write($"Buffer Queue: {bufsQueued} (Pool: {bufPool.Count})            \r");
        }

        ProcessBufferPool(al, bufPool, src);
      }

      opusFiles[0] = null;
    }

    for (;;) {
      al.GetSourceProperty(src, GetSourceInteger.SourceState, out var srcState);
      ThrowIfError(al);
      if (srcState != (int) SourceState.Playing) {
        break;
      }

      //alc.ProcessContext(ctx);
      ThrowIfError(alc, dev);
      ProcessBufferPool(al, bufPool, src);
      //Thread.Yield();
      Thread.Sleep(SleepIntervalMs);

      //al.GetSourceProperty(src, GetSourceInteger.BuffersQueued, out var bufsQueued);
      //ThrowIfError(alc, dev);
      //Console.Write($"Buffer Queue: {bufsQueued} (Pool: {bufPool.Count})            \r");
    }

    for (;;) {
      ProcessBufferPool(al, bufPool, src);
      al.GetSourceProperty(src, GetSourceInteger.BuffersQueued, out var bufsQueued);
      //Console.Write($"Buffer Queue: {bufsQueued} (Pool: {bufPool.Count})            \r");
      if (bufsQueued == 0)
        break;
    }

    Console.WriteLine();
    Console.WriteLine("Done playing.");

    al.SourceStop(src);
    //alc.ProcessContext(ctx);
    ProcessBufferPool(al, bufPool, src);

    ThrowIfError(al);
    al.DeleteSource(src);
    ThrowIfError(al);
    foreach (var buf in bufPool) {
      al.DeleteBuffer(buf);
      ThrowIfError(al);
    }

    alc.MakeContextCurrent(null);
    ThrowIfError(alc, dev);
    alc.DestroyContext(ctx);
    ThrowIfError(alc, dev);
    alc.CloseDevice(dev);
    ThrowIfError(alc, null);
  }

  private static unsafe void ProcessBufferPool(AL al, Queue<uint> bufPool, uint src) {
    var dequeue = 0;
    al.GetSourceProperty(src, GetSourceInteger.BuffersProcessed, &dequeue);
    var bufToDequeue = stackalloc uint[dequeue];
    al.SourceUnqueueBuffers(src, dequeue, bufToDequeue);
    for (var i = 0; i < dequeue; ++i) {
      var buf = bufToDequeue[i];
      bufPool.Enqueue(buf);
      //Console.WriteLine($"Pooled buffer {buf}");
    }
  }

  private static void ThrowIfError(AL al) {
    var err = al.GetError();
    if (err != AudioError.NoError)
      throw new Exception(err.ToString());
  }

  private static unsafe void ThrowIfError(ALContext alc, Device* dev) {
    var err = alc.GetError(dev);
    if (err != ContextError.NoError)
      throw new Exception(err.ToString());
  }

}
