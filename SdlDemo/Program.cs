#define SDL_AUDIO_USE_CALLBACK

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.IO;
using static SDL2.SDL;

namespace ImpromptuNinjas.Opus.SdlDemo;

public static class Program {

  [DllImport("SDL2", CallingConvention = (CallingConvention) 2)]
  private static extern int SDL_AudioStreamFlush(nint stream);

  private const int SleepIntervalMs = 5;

  public static unsafe void Main(string[] args) {
    var r = SDL_Init(SDL_INIT_AUDIO);

    if (r != 0)
      throw new Exception(SDL_GetError());

#if SDL_AUDIO_USE_CALLBACK
    var ctx = new AudioContext();
    var spec = new SDL_AudioSpec {
      freq = 48000,
      format = AUDIO_S16,
      channels = 2,
      samples = 4096,
      callback = AudioContext.AudioCallback,
      userdata = GCHandle.ToIntPtr(GCHandle.Alloc(ctx))
    };
#else
    var spec = new SDL_AudioSpec {
      freq = 48000,
      format = AUDIO_S16,
      channels = 2,
      samples = 4096,
      callback = null,
      userdata = default
    };
#endif

    //SDL.SDL_ClearError();
    var dev = SDL_OpenAudioDevice(null, 0,
      ref spec, out var obtained, 0);

#if !SDL_AUDIO_USE_CALLBACK
    var ctx = new AudioContext(dev);
#endif

    var err = SDL_GetError();
    if (!string.IsNullOrEmpty(err))
      throw new Exception(err);

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

    // start invoking the callback before data is available, this makes it actually 'streaming'
    SDL_PauseAudioDevice(dev, 0);

    Console.WriteLine($"Streaming {args.Length} files.");

    for (var f = 0; f < opusFiles.Length; f++) {
      using var opusFile = opusFiles[f]!.Value;

      var sdlStream = SDL_NewAudioStream(
        AUDIO_S16, (byte) opusFile.ChannelCount, 48000,
        obtained.format, obtained.channels, obtained.freq);

      //Console.Write("                                    \r");
      Console.WriteLine(args[f]);
      var channelCount = opusFile.ChannelCount;
      if (decoder == null)
        decoder = OpusDecoder.Create(OpusFrequency._48kHz, channelCount);
      else
        decoder->Init(OpusFrequency._48kHz, channelCount);
      var pool = ArrayPool<byte>.Shared;
      // ReSharper disable once ForCanBeConvertedToForeach

      // queue after first packet
      var enqueued = false;

      foreach (var packet in opusFile) {
        var segment = (ReadOnlySpan<byte>) packet;
        //var frames = OpusDecoder.GetFrames(segment);
        //var sampleCount = decoder->GetSamples(bytePtr, length);
        var sampleCount = OpusDecoder.GetSamples(segment, OpusFrequency._48kHz);
        if (sampleCount == 0)
          throw new NotImplementedException();

        byte[] buffer;

        {
          var streamSize = channelCount * sampleCount * sizeof(short);
          buffer = pool.Rent(streamSize);
          fixed (byte* pBuffer = buffer) {
            var bufferSpan = MemoryMarshal.Cast<byte, short>(new Span<byte>(pBuffer, streamSize));
            var decodedSampleCount = decoder->Decode(segment, bufferSpan, sampleCount, false);
            var decodedBufSize = channelCount * decodedSampleCount * sizeof(short);
            var result = SDL_AudioStreamPut(sdlStream, (nint) pBuffer, decodedBufSize);
            if (result == -1)
              throw new Exception(SDL_GetError());
          }
        }

        pool.Return(buffer);
        // read from stream while available until complete and stick into memStream

        // queue after first packet
        if (!enqueued)
          ctx.QueuedAudioStreams.Enqueue(sdlStream);
      }

      var flushResult = SDL_AudioStreamFlush(sdlStream);

      if (flushResult == -1)
        throw new Exception(SDL_GetError());

      opusFiles[0] = null;
    }

    // could invoke it here, most of the data would probably already be loaded
    //SDL_PauseAudioDevice(dev, 0);

    while (ctx.HasQueuedAudio)
      // wait for streams to finish
      Thread.Sleep(SleepIntervalMs);

    Console.WriteLine();
    Console.WriteLine("Done playing.");
  }

}
