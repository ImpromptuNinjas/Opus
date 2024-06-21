#if !NETSTANDARD2_0_OR_GREATER && !NET
// ReSharper disable RedundantUnsafeContext
#endif

namespace ImpromptuNinjas.Opus;

[PublicAPI]
public static class OpusEncoderExtensions {

  #region Dynamic Library Import Table

  // ReSharper disable IdentifierTypo
  // ReSharper disable StringLiteralTypo
  // ReSharper disable InconsistentNaming

  // int opus_encoder_get_size(int channels);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_encoder_get_size =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<int, int> opus_encoder_get_size
    = (delegate* unmanaged[Cdecl]<int, int>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_encoder_get_size));

  // OpusEncoder* opus_encoder_create(int Fs, int channels, int application, int* error);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_encoder_create =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<int, int, int, int, OpusEncoder*> opus_encoder_create
    = (delegate* unmanaged[Cdecl]<int, int, int, int, OpusEncoder*>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_encoder_create));

  // int opus_encoder_init(OpusEncoder* st, opus_int32 Fs, int channels, int application);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_encoder_init =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusEncoder*, OpusFrequency, int, int, OpusStatusCode> opus_encoder_init
    = (delegate* unmanaged[Cdecl]<OpusEncoder*, OpusFrequency, int, int, OpusStatusCode>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_encoder_init));

  // int opus_encode(OpusEncoder* st, const opus_int16* pcm, int frame_size, unsigned char* data, opus_int32 max_data_bytes);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_encode =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusEncoder*, short*, OpusFrameSize, byte*, int, int> opus_encode
    = (delegate* unmanaged[Cdecl]<OpusEncoder*, short*, OpusFrameSize, byte*, int, int>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_encode));

  // int opus_encode_float(OpusEncoder* st, const float* pcm, int frame_size, unsigned char* data, opus_int32 max_data_bytes);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_encode_float =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusEncoder*, float*, OpusFrameSize, byte*, int, int> opus_encode_float
    = (delegate* unmanaged[Cdecl]<OpusEncoder*, float*, OpusFrameSize, byte*, int, int>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_encode_float));

  // void opus_encoder_destroy(OpusEncoder* st);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_encoder_destroy =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusEncoder*, void> opus_encoder_destroy
    = (delegate* unmanaged[Cdecl]<OpusEncoder*, void>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_encoder_destroy));

  // the following are from opus_bindings_compat.h included in this project
  // they are not part of the official libopus API, they provide non-varargs
  // versions of the opus_encoder_ctl sub-functions
  /**
OPUS_EXPORT int opus_encoder_ctl_request(OpusEncoder* st, int request);
OPUS_EXPORT int opus_encoder_ctl_set_int(OpusEncoder* st, int request, int x);
OPUS_EXPORT int opus_encoder_ctl_get_int(OpusEncoder* st, int request, int* x);
OPUS_EXPORT int opus_encoder_ctl_set_uint(OpusEncoder* st, int request, unsigned x);
OPUS_EXPORT int opus_encoder_ctl_get_uint(OpusEncoder* st, int request, unsigned* x);
OPUS_EXPORT int opus_encoder_ctl_set_val16(OpusEncoder* st, int request, short x);
OPUS_EXPORT int opus_encoder_ctl_get_val16(OpusEncoder* st, int request, short* x);
   */

  // int opus_encoder_ctl_request(OpusEncoder* st, int request);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_encoder_ctl_request =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusEncoder*, OpusControlRequest, OpusStatusCode> opus_encoder_ctl_request
    = (delegate* unmanaged[Cdecl]<OpusEncoder*, OpusControlRequest, OpusStatusCode>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_encoder_ctl_request));

  // int opus_encoder_ctl_get_int(OpusEncoder* st, int request, int* x);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_encoder_ctl_get_int =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusEncoder*, OpusGetControlRequest, int*, OpusStatusCode> opus_encoder_ctl_get_int
    = (delegate* unmanaged[Cdecl]<OpusEncoder*, OpusGetControlRequest, int*, OpusStatusCode>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_encoder_ctl_get_int));

  // int opus_encoder_ctl_set_int(OpusEncoder* st, int request, int x);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_encoder_ctl_set_int =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusEncoder*, OpusSetControlRequest, int, OpusStatusCode> opus_encoder_ctl_set_int
    = (delegate* unmanaged[Cdecl]<OpusEncoder*, OpusSetControlRequest, int, OpusStatusCode>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_encoder_ctl_set_int));

  // int opus_encoder_ctl_get_val16(OpusEncoder* st, int request, short* x);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_encoder_ctl_get_val16 =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusEncoder*, OpusGetControlRequest, short*, OpusStatusCode> opus_encoder_ctl_get_val16
    = (delegate* unmanaged[Cdecl]<OpusEncoder*, OpusGetControlRequest, short*, OpusStatusCode>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_encoder_ctl_get_val16));

  // int opus_encoder_ctl_set_val16(OpusEncoder* st, int request, short x);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_encoder_ctl_set_val16 =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusEncoder*, OpusSetControlRequest, short, OpusStatusCode> opus_encoder_ctl_set_val16
    = (delegate* unmanaged[Cdecl]<OpusEncoder*, OpusSetControlRequest, short, OpusStatusCode>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_encoder_ctl_set_val16));

  // int opus_encoder_ctl_get_uint(OpusEncoder* st, int request, unsigned* x);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_encoder_ctl_get_uint =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusEncoder*, OpusGetControlRequest, uint*, OpusStatusCode> opus_encoder_ctl_get_uint
    = (delegate* unmanaged[Cdecl]<OpusEncoder*, OpusGetControlRequest, uint*, OpusStatusCode>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_encoder_ctl_get_uint));

  // int opus_encoder_ctl_set_uint(OpusEncoder* st, int request, unsigned x);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_encoder_ctl_set_uint =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusEncoder*, OpusSetControlRequest, uint, OpusStatusCode> opus_encoder_ctl_set_uint
    = (delegate* unmanaged[Cdecl]<OpusEncoder*, OpusSetControlRequest, uint, OpusStatusCode>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_encoder_ctl_set_uint));

  // ReSharper restore InconsistentNaming
  // ReSharper restore StringLiteralTypo
  // ReSharper restore IdentifierTypo

  #endregion

  /// <summary>
  /// Encodes an Opus frame.
  /// </summary>
  /// <param name="_" />
  /// <param name="pcm">
  /// Input signal (interleaved if 2 channels).
  /// Length is <paramref name="frameSize"/>*<c>channels</c>*<see langword="sizeof"/>(<see cref="Int16"/>).
  /// </param>
  /// <param name="frameSize">
  /// Number of samples per channel in the input signal.
  /// This must be an Opus frame size for the encoder's sampling rate.
  /// For example, at 48 kHz the permitted values are 120, 240, 480, 960, 1920, and 2880.
  /// Passing in a duration of less than 10 ms (480 samples at 48 kHz) will prevent the encoder from using the LPC
  /// or hybrid modes.
  /// </param>
  /// <param name="data">
  /// Output payload.
  /// This must contain storage for at least <paramref name="maxDataBytes"/>.</param>
  /// <param name="maxDataBytes">
  /// Size of the allocated memory for the output payload.
  /// This may be used to impose an upper limit on the instant bitrate, but should not be used as the only bitrate
  /// control.
  /// Use <see cref="SetControl"/> with <see cref="OpusSetControlRequest.SetBitrate"/> to control the bitrate.
  /// </param>
  /// <returns>
  /// The length of the encoded packet (in bytes) on success or if negative, an error code (<see cref="OpusStatusCode"/>) on failure.
  /// </returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int Encode(in this OpusEncoder self, short* pcm, OpusFrameSize frameSize, byte* data, int maxDataBytes) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Ldarg_0();
      Push(pcm);
      Push(frameSize);
      Push(data);
      Push(opus_encode);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(int),
        typeof(OpusEncoder*), typeof(short*), typeof(OpusFrameSize), typeof(byte*), typeof(int)));
      return Return<int>();
#else
    var p = (OpusEncoder*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_encode(p, pcm, frameSize, data, maxDataBytes);
#endif
  }

  /// <summary>
  /// Encodes an Opus frame.
  /// </summary>
  /// <param name="_" />
  /// <param name="pcm">
  /// Input signal (interleaved if 2 channels).
  /// Length is <paramref name="frameSize"/>*<c>channels</c>*<see langword="sizeof"/>(<see cref="Int16"/>).
  /// </param>
  /// <param name="frameSize">
  /// Number of samples per channel in the input signal.
  /// This must be an Opus frame size for the encoder's sampling rate.
  /// For example, at 48 kHz the permitted values are 120, 240, 480, 960, 1920, and 2880.
  /// Passing in a duration of less than 10 ms (480 samples at 48 kHz) will prevent the encoder from using the LPC
  /// or hybrid modes.
  /// </param>
  /// <param name="data">
  /// Output payload.
  /// </param>
  /// <returns>
  /// The length of the encoded packet (in bytes) on success or if negative, an error code (<see cref="OpusStatusCode"/>) on failure.
  /// </returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int Encode(in this OpusEncoder self, ReadOnlySpan<short> pcm, OpusFrameSize frameSize, Span<byte> data) {
    // TODO: validate frame size vs pcm length
    fixed (short* pPcm = pcm)
    fixed (byte* pData = data)
      return Encode(self, pPcm, frameSize, pData, data.Length);
  }

  /// <summary>
  /// Encodes an Opus frame.
  /// </summary>
  /// <param name="self" />
  /// <param name="pcm">
  /// Input signal (interleaved if 2 channels).
  /// Length is <paramref name="frameSize"/>*<c>channels</c>*<see langword="sizeof"/>(<see cref="Single"/>).
  /// </param>
  /// <param name="frameSize">
  /// Number of samples per channel in the input signal.
  /// This must be an Opus frame size for the encoder's sampling rate.
  /// For example, at 48 kHz the permitted values are 120, 240, 480, 960, 1920, and 2880.
  /// Passing in a duration of less than 10 ms (480 samples at 48 kHz) will prevent the encoder from using the LPC
  /// or hybrid modes.
  /// </param>
  /// <param name="data">
  /// Output payload.
  /// This must contain storage for at least <paramref name="maxDataBytes"/>.
  /// </param>
  /// <param name="maxDataBytes">
  /// Size of the allocated memory for the output payload.
  /// This may be used to impose an upper limit on the instant bitrate, but should not be used as the only bitrate
  /// control.
  /// Use <see cref="SetControl"/> with <see cref="OpusSetControlRequest.SetBitrate"/> to control the bitrate.
  /// </param>
  /// <returns>
  /// The length of the encoded packet (in bytes) on success or if negative, an error code (<see cref="OpusStatusCode"/>) on failure.
  /// </returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int Encode(in this OpusEncoder self, float* pcm, OpusFrameSize frameSize, byte* data, int maxDataBytes) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Ldarg_0();
      Push(pcm);
      Push(frameSize);
      Push(data);
      Push(opus_encode_float);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(int),
        typeof(OpusEncoder*), typeof(float*), typeof(OpusFrameSize), typeof(byte*), typeof(int)));
      return Return<int>();
#else
    var p = (OpusEncoder*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_encode_float(p, pcm, frameSize, data, maxDataBytes);
#endif
  }

  /// <summary>
  /// Encodes an Opus frame.
  /// </summary>
  /// <param name="self" />
  /// <param name="pcm">
  /// Input signal (interleaved if 2 channels).
  /// Length is <paramref name="frameSize"/>*<c>channels</c>*<see langword="sizeof"/>(<see cref="Single"/>).
  /// </param>
  /// <param name="frameSize">
  /// Number of samples per channel in the input signal.
  /// This must be an Opus frame size for the encoder's sampling rate.
  /// For example, at 48 kHz the permitted values are 120, 240, 480, 960, 1920, and 2880.
  /// Passing in a duration of less than 10 ms (480 samples at 48 kHz) will prevent the encoder from using the LPC
  /// or hybrid modes.
  /// </param>
  /// <param name="data">
  /// Output payload.
  /// </param>
  /// <returns>
  /// The length of the encoded packet (in bytes) on success or if negative, an error code (<see cref="OpusStatusCode"/>) on failure.
  /// </returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int Encode(in this OpusEncoder self, ReadOnlySpan<float> pcm, OpusFrameSize frameSize, Span<byte> data) {
    // TODO: validate frame size vs pcm length
    fixed (float* pPcm = pcm)
    fixed (byte* pData = data)
      return Encode(self, pPcm, frameSize, pData, data.Length);
  }

  /// <summary>
  /// Perform a parameterless control function on an <see cref="OpusEncoder"/>.
  /// </summary>
  /// <param name="self" />
  /// <param name="request">The control request.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusStatusCode ControlRequest(in this OpusEncoder self, OpusControlRequest request) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Ldarg_0();
      Push(request);
      Push(opus_encoder_ctl_request);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusStatusCode),
        typeof(OpusEncoder*), typeof(OpusGetControlRequest)));
      return Return<OpusStatusCode>();
#else
    var p = (OpusEncoder*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_encoder_ctl_request(p, request);
#endif
  }

  /// <summary>
  /// Gets a control value from an <see cref="OpusEncoder"/>.
  /// </summary>
  /// <param name="self" />
  /// <param name="request">The control request.</param>
  /// <param name="x">The control value.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusStatusCode GetControl(in this OpusEncoder self, OpusGetControlRequest request, int* x) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Ldarg_0();
      Push(request);
      Push(x);
      Push(opus_encoder_ctl_get_int);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusStatusCode),
        typeof(OpusEncoder*), typeof(OpusGetControlRequest), typeof(int*)));
      return Return<OpusStatusCode>();
#else
    var p = (OpusEncoder*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_encoder_ctl_get_int(p, request, x);
#endif
  }

  /// <summary>
  /// Gets a control value from an <see cref="OpusEncoder"/>.
  /// </summary>
  /// <param name="self" />
  /// <param name="request">The control request.</param>
  /// <param name="x">The control value.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusStatusCode GetControl(in this OpusEncoder self, OpusGetControlRequest request, uint* x) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Ldarg_0();
      Push(request);
      Push(x);
      Push(opus_encoder_ctl_get_uint);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusStatusCode),
        typeof(OpusEncoder*), typeof(OpusGetControlRequest), typeof(uint*)));
      return Return<OpusStatusCode>();
#else
    var p = (OpusEncoder*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_encoder_ctl_get_uint(p, request, x);
#endif
  }

  /// <summary>
  /// Gets a control value from an <see cref="OpusEncoder"/>.
  /// </summary>
  /// <param name="self" />
  /// <param name="request">The control request.</param>
  /// <param name="x">The control value.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusStatusCode GetControl(in this OpusEncoder self, OpusGetControlRequest request, short* x) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Ldarg_0();
      Push(request);
      Push(x);
      Push(opus_encoder_ctl_get_val16);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusStatusCode),
        typeof(OpusEncoder*), typeof(OpusGetControlRequest), typeof(short*)));
      return Return<OpusStatusCode>();
#else
    var p = (OpusEncoder*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_encoder_ctl_get_val16(p, request, x);
#endif
  }

  /// <summary>
  /// Sets a control value from an <see cref="OpusEncoder"/>.
  /// </summary>
  /// <param name="self" />
  /// <param name="request">The control request.</param>
  /// <param name="x">The control value.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusStatusCode SetControl(in this OpusEncoder self, OpusSetControlRequest request, int x) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Ldarg_0();
      Push(request);
      Push(x);
      Push(opus_encoder_ctl_set_int);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusStatusCode),
        typeof(OpusEncoder*), typeof(OpusSetControlRequest), typeof(int)));
      return Return<OpusStatusCode>();
#else
    var p = (OpusEncoder*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_encoder_ctl_set_int(p, request, x);
#endif
  }

  /// <summary>
  /// Sets a control value from an <see cref="OpusEncoder"/>.
  /// </summary>
  /// <param name="self" />
  /// <param name="request">The control request.</param>
  /// <param name="x">The control value.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusStatusCode SetControl(in this OpusEncoder self, OpusSetControlRequest request, uint x) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Ldarg_0();
      Push(request);
      Push(x);
      Push(opus_encoder_ctl_set_uint);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusStatusCode),
        typeof(OpusEncoder*), typeof(OpusSetControlRequest), typeof(uint)));
      return Return<OpusStatusCode>();
#else
    var p = (OpusEncoder*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_encoder_ctl_set_uint(p, request, x);
#endif
  }

  /// <summary>
  /// Sets a control value from an <see cref="OpusEncoder"/>.
  /// </summary>
  /// <param name="self" />
  /// <param name="request">The control request.</param>
  /// <param name="x">The control value.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusStatusCode SetControl(in this OpusEncoder self, OpusSetControlRequest request, short x) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Ldarg_0();
      Push(request);
      Push(x);
      Push(opus_encoder_ctl_set_val16);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusStatusCode),
        typeof(OpusEncoder*), typeof(OpusSetControlRequest), typeof(short)));
      return Return<OpusStatusCode>();
#else
    var p = (OpusEncoder*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_encoder_ctl_set_val16(p, request, x);
#endif
  }

  /// <summary>
  /// Frees an <see cref="OpusEncoder"/> allocated by <see cref="OpusEncoder.Create(OpusFrequency,int,int,OpusStatusCode*)"/>.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe void Destroy(in this OpusEncoder self) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Ldarg_0();
      Push(opus_encoder_destroy);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(void),
        typeof(OpusEncoder*)));
#else
    var p = (OpusEncoder*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    opus_encoder_destroy(p);
#endif
  }

  /// <summary>
  /// Initializes a previously allocated encoder state.
  /// The state must be at least the size returned by <see cref="OpusEncoder.GetSize"/>.
  /// This is intended for applications which use their own allocator instead of malloc.
  /// </summary>
  /// <param name="self" />
  /// <param name="fs">Sampling rate to decode to (Hz). This must be one of 8000, 12000, 16000, 24000, or 48000.</param>
  /// <param name="channels">Number of channels (1 or 2) to decode.</param>
  /// <param name="application">Coding mode (See <see cref="OpusApplication"/>)</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusStatusCode Init(in this OpusEncoder self, OpusFrequency fs, int channels, int application) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Ldarg_0();
      Push(fs);
      Push(channels);
      Push(application);
      Push(opus_encoder_init);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusStatusCode),
        typeof(OpusEncoder*), typeof(OpusFrequency), typeof(int), typeof(int)));
      return Return<OpusStatusCode>();
#else
    var p = (OpusEncoder*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_encoder_init(p, fs, channels, application);
#endif
  }

  /// <summary>
  /// Resets the codec state to be equivalent to a freshly initialized state.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static OpusStatusCode Reset(in this OpusEncoder self)
    => self.ControlRequest(OpusControlRequest.ResetState);

}
