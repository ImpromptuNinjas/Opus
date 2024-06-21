#define SDL_AUDIO_USE_CALLBACK

using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.IO;
using SDL2;
using static SDL2.SDL;

namespace ImpromptuNinjas.Opus.SdlDemo;

public class AudioContext {

  public uint Device { get; set; }

  // queue of SDL audio streams; they just do transcoding
  // either calls SDL_QueueAudio on the output after transcoding
  // or puts the transcoded data into Buffers for the callback to read
  public ConcurrentQueue<nint> QueuedAudioStreams { get; } = new();

  public Thread SdlAudioStreamWorkerThread { get; }

  public AudioContext(uint dev = 0) {
    Device = dev;

    (SdlAudioStreamWorkerThread
      = new Thread(SdlAudioStreamWorker) {
        Name = nameof(SdlAudioStreamWorker),
        IsBackground = true
      }).Start();
  }

  public unsafe void SdlAudioStreamWorker() {
    var pDoneBuffer = stackalloc byte[8];
    //var doneBufferSpan = new Span<byte>(pDoneBuffer, 8);
    var pool = ArrayPool<byte>.Shared;
    for (;;) {
      if (!QueuedAudioStreams.TryPeek(out var sdlStream))
        continue;

      var avail = SDL_AudioStreamAvailable(sdlStream);

      if (avail > 0) {
        var buffer = pool.Rent(64 * 1024);
        try {
#if !SDL_AUDIO_USE_CALLBACK
            var bufferSpan = buffer.AsSpan(0, avail);
#endif
          fixed (byte* pBuffer = buffer) {
            var read = SDL_AudioStreamGet(sdlStream, (nint) pBuffer, avail);
            if (read == 0) {
              // maybe stream is done?
              QueuedAudioStreams.TryDequeue(out _);
              //SDL_FreeAudioStream(sdlStream);
              continue;
            }

            if (read < 0)
              throw new Exception(SDL_GetError());

            //bufferSpan = bufferSpan[..read];
#if SDL_AUDIO_USE_CALLBACK
            Buffers.Enqueue(new Buffer {
              Data = new ArraySegment<byte>(buffer, 0, read)
            });
#else
              var result = SDL_QueueAudio(this.Device, (nint) pBuffer, (uint) read);
              if (result != 0)
                throw new Exception(SDL_GetError());
#endif
          }
        }
        finally {
#if SDL_AUDIO_USE_CALLBACK
          // do not return buffer
#else
            pool.Return(buffer);
#endif
        }
      }
      else {
        // try to read 0 into a 8 byte buffer to see if the stream is done
        var read = SDL_AudioStreamGet(sdlStream, (nint) pDoneBuffer, 0);
        if (read != 0)
          // stream is buffering?
          continue;

        // stream is done?
        QueuedAudioStreams.TryDequeue(out _);
      }
    }
  }

#if SDL_AUDIO_USE_CALLBACK
  public static unsafe void AudioCallback(nint userdata, nint stream, int len) {
    var ctx = (AudioContext) GCHandle.FromIntPtr(userdata).Target!;
    var span = new Span<byte>((void*) stream, len);
    try {
      ctx.SdlAudioCallbackWorker(span);
    }
    catch (Exception ex) {
      Debugger.Launch();
      Console.WriteLine(ex);
    }
  }

  public class Buffer {

    public ArraySegment<byte> Data { get; set; }

  }

  public ConcurrentQueue<Buffer> Buffers { get; } = new();

  public bool HasQueuedAudio {
    get {
      return !QueuedAudioStreams.IsEmpty ||
#if SDL_AUDIO_USE_CALLBACK
        !Buffers.IsEmpty;
#else
          SDL_GetQueuedAudioSize(Device) > 0;
#endif
    }
  }

  //
  private void SdlAudioCallbackWorker(Span<byte> span) {
    for (;;) {
      // write Buffers to span
      if (!Buffers.TryPeek(out var buffer))
        // no more buffers
        break;

      if (buffer.Data.Count > span.Length) {
        buffer.Data.AsSpan(..span.Length).CopyTo(span);
        buffer.Data = new ArraySegment<byte>(
          buffer.Data.Array!,
          buffer.Data.Offset + span.Length,
          buffer.Data.Count - span.Length);
        // incomplete buffer read
        break;
      }

      buffer.Data.AsSpan().CopyTo(span);
      // complete buffer read
      Buffers.TryDequeue(out _);

      // return buffer
      ArrayPool<byte>.Shared.Return(buffer.Data.Array!);

      span = span[buffer.Data.Count..];

      if (span.Length == 0)
        // span is full
        break;

      // more space in span, next buffer
    }
  }
#endif

}
