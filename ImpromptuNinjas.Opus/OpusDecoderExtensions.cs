namespace ImpromptuNinjas.Opus;

[PublicAPI]
public static class OpusDecoderExtensions {

  #region Dynamic Library Import Table

  // ReSharper disable IdentifierTypo
  // ReSharper disable StringLiteralTypo
  // ReSharper disable InconsistentNaming

  // int opus_decoder_init(OpusDecoder *st, opus_int32 Fs, int channels);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_decoder_init =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusDecoder*, OpusFrequency, int, OpusStatusCode> opus_decoder_init
    = (delegate* unmanaged[Cdecl]<OpusDecoder*, OpusFrequency, int, OpusStatusCode>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_decoder_init));

  // int opus_decode(OpusDecoder *st, const unsigned char *data, opus_int32 len, opus_int16 *pcm, int frame_size, int decode_fec);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_decode =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusDecoder*, byte*, int, short*, int, int, int> opus_decode
    = (delegate* unmanaged[Cdecl]<OpusDecoder*, byte*, int, short*, int, int, int>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_decode));

  // int opus_decoder_get_nb_samples(const OpusDecoder *dec, const unsigned char *packet, opus_int32 len);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_decode_float =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusDecoder*, byte*, int, float*, int, int, int> opus_decode_float
    = (delegate* unmanaged[Cdecl]<OpusDecoder*, byte*, int, float*, int, int, int>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_decode_float));

  // void opus_decoder_destroy(OpusDecoder *st);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_decoder_destroy =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusDecoder*, void> opus_decoder_destroy
    = (delegate* unmanaged[Cdecl]<OpusDecoder*, void>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_decoder_destroy));

  // int opus_decoder_get_nb_samples(const OpusDecoder *dec, const unsigned char *packet, opus_int32 len);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_decoder_get_nb_samples =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusDecoder*, byte*, int, int> opus_decoder_get_nb_samples
    = (delegate* unmanaged[Cdecl]<OpusDecoder*, byte*, int, int>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_decoder_get_nb_samples));

  // the following are from opus_bindings_compat.h included in this project
  // they are not part of the official libopus API, they provide non-varargs
  // versions of the opus_decoder_ctl sub-functions

  // int opus_decoder_ctl_request(OpusDecoder* st, int request);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_decoder_ctl_request =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusDecoder*, OpusControlRequest, OpusStatusCode> opus_decoder_ctl_request
    = (delegate* unmanaged[Cdecl]<OpusDecoder*, OpusControlRequest, OpusStatusCode>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_decoder_ctl_request));

  // int opus_decoder_ctl_get_int(OpusDecoder* st, int request, int* x);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_decoder_ctl_get_int =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusDecoder*, OpusGetControlRequest, int*, OpusStatusCode> opus_decoder_ctl_get_int
    = (delegate* unmanaged[Cdecl]<OpusDecoder*, OpusGetControlRequest, int*, OpusStatusCode>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_decoder_ctl_get_int));

  // int opus_decoder_ctl_set_int(OpusDecoder* st, int request, int x);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_decoder_ctl_set_int =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusDecoder*, OpusSetControlRequest, int, OpusStatusCode> opus_decoder_ctl_set_int
    = (delegate* unmanaged[Cdecl]<OpusDecoder*, OpusSetControlRequest, int, OpusStatusCode>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_decoder_ctl_set_int));

  // int opus_decoder_ctl_get_val16(OpusDecoder* st, int request, short* x);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_decoder_ctl_get_val16 =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusDecoder*, OpusGetControlRequest, short*, OpusStatusCode> opus_decoder_ctl_get_val16
    = (delegate* unmanaged[Cdecl]<OpusDecoder*, OpusGetControlRequest, short*, OpusStatusCode>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_decoder_ctl_get_val16));

  // int opus_decoder_ctl_set_val16(OpusDecoder* st, int request, short x);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_decoder_ctl_set_val16 =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusDecoder*, OpusSetControlRequest, short, OpusStatusCode> opus_decoder_ctl_set_val16
    = (delegate* unmanaged[Cdecl]<OpusDecoder*, OpusSetControlRequest, short, OpusStatusCode>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_decoder_ctl_set_val16));

  // int opus_decoder_ctl_get_uint(OpusDecoder* st, int request, unsigned* x);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_decoder_ctl_get_uint =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusDecoder*, OpusGetControlRequest, uint*, OpusStatusCode> opus_decoder_ctl_get_uint
    = (delegate* unmanaged[Cdecl]<OpusDecoder*, OpusGetControlRequest, uint*, OpusStatusCode>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_decoder_ctl_get_uint));

  // int opus_decoder_ctl_set_uint(OpusDecoder* st, int request, unsigned x);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_decoder_ctl_set_uint =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusDecoder*, OpusSetControlRequest, uint, OpusStatusCode> opus_decoder_ctl_set_uint
    = (delegate* unmanaged[Cdecl]<OpusDecoder*, OpusSetControlRequest, uint, OpusStatusCode>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_decoder_ctl_set_uint));

  // ReSharper restore InconsistentNaming
  // ReSharper restore StringLiteralTypo
  // ReSharper restore IdentifierTypo

  #endregion

  /// <summary>
  /// Decode an Opus packet.
  /// </summary>
  /// <param name="self" />
  /// <param name="data">Input payload. Use a null pointer to indicate packet loss.</param>
  /// <param name="len">Number of bytes in payload.</param>
  /// <param name="pcm">
  /// Output signal (interleaved if 2 channels).
  /// Length is <paramref name="frameSize"/>*<c>channels</c>*<see langword="sizeof"/>(<see cref="Int16"/>)
  /// </param>
  /// <param name="frameSize">
  /// Number of samples per channel of available space in pcm.
  /// If this is less than the maximum packet duration (120ms; 5760 for 48kHz), this function will not be capable of
  /// decoding some packets. In the case of PLC (<paramref name="data"/> is <see langword="null"/>) or FEC
  /// (<paramref name="decodeFec"/>=1), then <paramref name="frameSize"/> needs to be exactly the duration of audio
  /// that is missing, otherwise the decoder will not be in the optimal state to decode the next incoming packet.
  /// For the PLC and FEC cases, <paramref name="frameSize"/> must be a multiple of 2.5 ms.
  /// </param>
  /// <param name="decodeFec">
  /// Flag (0 or 1) to request that any in-band forward error correction data be decoded.
  /// If no such data is available, the frame is decoded as if it were lost.
  /// </param>
  /// <returns>Number of decoded samples or if negative, <see cref="OpusStatusCode"/>.</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int Decode(in this OpusDecoder self, byte* data, int len, short* pcm, int frameSize, bool decodeFec) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Ldarg_0();
      Push(data);
      Push(len);
      Push(pcm);
      Push(frameSize);
      Push(decodeFec ? 1 : 0);
      Push(opus_decode);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(int),
        typeof(OpusDecoder*), typeof(byte*), typeof(int), typeof(short*), typeof(int), typeof(int)));
      return Return<int>();
#else
    var p = (OpusDecoder*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_decode(p, data, len, pcm, frameSize, decodeFec ? 1 : 0);
#endif
  }

  /// <summary>
  /// Decode an Opus packet.
  /// </summary>
  /// <param name="self" />
  /// <param name="data">Input payload. Use a null pointer to indicate packet loss.</param>
  /// <param name="pcm">
  /// Output signal (interleaved if 2 channels).
  /// Length is <paramref name="frameSize"/>*<c>channels</c>*<see langword="sizeof"/>(<see cref="Int16"/>)
  /// </param>
  /// <param name="frameSize">
  /// Number of samples per channel of available space in pcm.
  /// If this is less than the maximum packet duration (120ms; 5760 for 48kHz), this function will not be capable of
  /// decoding some packets. In the case of PLC (<paramref name="data"/> is <see langword="null"/>) or FEC
  /// (<paramref name="decodeFec"/>=1), then <paramref name="frameSize"/> needs to be exactly the duration of audio
  /// that is missing, otherwise the decoder will not be in the optimal state to decode the next incoming packet.
  /// For the PLC and FEC cases, <paramref name="frameSize"/> must be a multiple of 2.5 ms.
  /// </param>
  /// <param name="decodeFec">
  /// Flag (0 or 1) to request that any in-band forward error correction data be decoded.
  /// If no such data is available, the frame is decoded as if it were lost.
  /// </param>
  /// <returns>Number of decoded samples or if negative, <see cref="OpusStatusCode"/>.</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int Decode(in this OpusDecoder self, ReadOnlySpan<byte> data, Span<short> pcm, int frameSize, bool decodeFec) {
    // TODO: validate frame size vs pcm length
    fixed (byte* pData = data)
    fixed (short* pPcm = pcm)
      return Decode(self, pData, data.Length, pPcm, frameSize, decodeFec);
  }

  /// <summary>
  /// Decode an Opus packet.
  /// </summary>
  /// <param name="self" />
  /// <param name="data">Input payload. Use a null pointer to indicate packet loss.</param>
  /// <param name="len">Number of bytes in payload.</param>
  /// <param name="pcm">
  /// Output signal (interleaved if 2 channels).
  /// Length is <paramref name="frameSize"/>*<c>channels</c>*<see langword="sizeof"/>(<see cref="Single"/>)
  /// </param>
  /// <param name="frameSize">
  /// Number of samples per channel of available space in pcm.
  /// If this is less than the maximum packet duration (120ms; 5760 for 48kHz), this function will not be capable of
  /// decoding some packets. In the case of PLC (<paramref name="data"/> is <see langword="null"/>) or FEC
  /// (<paramref name="decodeFec"/>=1), then <paramref name="frameSize"/> needs to be exactly the duration of audio
  /// that is missing, otherwise the decoder will not be in the optimal state to decode the next incoming packet.
  /// For the PLC and FEC cases, <paramref name="frameSize"/> must be a multiple of 2.5 ms.
  /// </param>
  /// <param name="decodeFec">
  /// Flag (0 or 1) to request that any in-band forward error correction data be decoded.
  /// If no such data is available, the frame is decoded as if it were lost.
  /// </param>
  /// <returns>Number of decoded samples or if negative, <see cref="OpusStatusCode"/>.</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int Decode(in this OpusDecoder self, byte* data, int len, float* pcm, int frameSize, bool decodeFec) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Ldarg_0();
      Push(data);
      Push(len);
      Push(pcm);
      Push(frameSize);
      Push(decodeFec ? 1 : 0);
      Push(opus_decode_float);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(int),
        typeof(OpusDecoder*), typeof(byte*), typeof(int), typeof(float*), typeof(int), typeof(int)));
      return Return<int>();
#else
    var p = (OpusDecoder*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_decode_float(p, data, len, pcm, frameSize, decodeFec ? 1 : 0);
#endif
  }

  /// <summary>
  /// Decode an Opus packet.
  /// </summary>
  /// <param name="self" />
  /// <param name="data">Input payload. Use a null pointer to indicate packet loss.</param>
  /// <param name="pcm">
  /// Output signal (interleaved if 2 channels).
  /// Length is <paramref name="frameSize"/>*<c>channels</c>*<see langword="sizeof"/>(<see cref="Single"/>)
  /// </param>
  /// <param name="frameSize">
  /// Number of samples per channel of available space in pcm.
  /// If this is less than the maximum packet duration (120ms; 5760 for 48kHz), this function will not be capable of
  /// decoding some packets. In the case of PLC (<paramref name="data"/> is <see langword="null"/>) or FEC
  /// (<paramref name="decodeFec"/>=1), then <paramref name="frameSize"/> needs to be exactly the duration of audio
  /// that is missing, otherwise the decoder will not be in the optimal state to decode the next incoming packet.
  /// For the PLC and FEC cases, <paramref name="frameSize"/> must be a multiple of 2.5 ms.
  /// </param>
  /// <param name="decodeFec">
  /// Flag (0 or 1) to request that any in-band forward error correction data be decoded.
  /// If no such data is available, the frame is decoded as if it were lost.
  /// </param>
  /// <returns>Number of decoded samples or if negative, <see cref="OpusStatusCode"/>.</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int Decode(in this OpusDecoder self, ReadOnlySpan<byte> data, Span<float> pcm, int frameSize, bool decodeFec) {
    // TODO: validate frame size vs pcm length
    fixed (byte* pData = data)
    fixed (float* pPcm = pcm)
      return Decode(self, pData, data.Length, pPcm, frameSize, decodeFec);
  }

  /// <summary>
  /// Perform a parameterless control function on an <see cref="OpusDecoder"/>.
  /// </summary>
  /// <param name="self" />
  /// <param name="request">The control request.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusStatusCode ControlRequest(in this OpusDecoder self, OpusControlRequest request) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Ldarg_0();
      Push(request);
      Push(opus_decoder_ctl_request);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusStatusCode),
        typeof(OpusDecoder*), typeof(OpusGetControlRequest)));
      return Return<OpusStatusCode>();
#else
    var p = (OpusDecoder*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_decoder_ctl_request(p, request);
#endif
  }

  /// <summary>
  /// Gets a control value from an <see cref="OpusDecoder"/>.
  /// </summary>
  /// <param name="self" />
  /// <param name="request">The control request.</param>
  /// <param name="x">The control value.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusStatusCode GetControl(in this OpusDecoder self, OpusGetControlRequest request, int* x) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Ldarg_0();
      Push(request);
      Push(x);
      Push(opus_decoder_ctl_get_int);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusStatusCode),
        typeof(OpusDecoder*), typeof(OpusGetControlRequest), typeof(int*)));
      return Return<OpusStatusCode>();
#else
    var p = (OpusDecoder*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_decoder_ctl_get_int(p, request, x);
#endif
  }

  /// <summary>
  /// Gets a control value from an <see cref="OpusDecoder"/>.
  /// </summary>
  /// <param name="self" />
  /// <param name="request">The control request.</param>
  /// <param name="x">The control value.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusStatusCode GetControl(in this OpusDecoder self, OpusGetControlRequest request, uint* x) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Ldarg_0();
      Push(request);
      Push(x);
      Push(opus_decoder_ctl_get_uint);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusStatusCode),
        typeof(OpusDecoder*), typeof(OpusGetControlRequest), typeof(uint*)));
      return Return<OpusStatusCode>();
#else
    var p = (OpusDecoder*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_decoder_ctl_get_uint(p, request, x);
#endif
  }

  /// <summary>
  /// Gets a control value from an <see cref="OpusDecoder"/>.
  /// </summary>
  /// <param name="self" />
  /// <param name="request">The control request.</param>
  /// <param name="x">The control value.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusStatusCode GetControl(in this OpusDecoder self, OpusGetControlRequest request, short* x) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Ldarg_0();
      Push(request);
      Push(x);
      Push(opus_decoder_ctl_get_val16);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusStatusCode),
        typeof(OpusDecoder*), typeof(OpusGetControlRequest), typeof(short*)));
      return Return<OpusStatusCode>();
#else
    var p = (OpusDecoder*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_decoder_ctl_get_val16(p, request, x);
#endif
  }

  /// <summary>
  /// Sets a control value from an <see cref="OpusDecoder"/>.
  /// </summary>
  /// <param name="self" />
  /// <param name="request">The control request.</param>
  /// <param name="x">The control value.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static OpusStatusCode SetControl(in this OpusDecoder self, OpusSetControlRequest request, int x) {
    unsafe {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Ldarg_0();
      Push(request);
      Push(x);
      Push(opus_decoder_ctl_set_int);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusStatusCode),
        typeof(OpusDecoder*), typeof(OpusSetControlRequest), typeof(int)));
      return Return<OpusStatusCode>();
#else
      var p = (OpusDecoder*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
      return opus_decoder_ctl_set_int(p, request, x);
#endif
    }
  }

  /// <summary>
  /// Sets a control value from an <see cref="OpusDecoder"/>.
  /// </summary>
  /// <param name="self" />
  /// <param name="request">The control request.</param>
  /// <param name="x">The control value.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusStatusCode SetControl(in this OpusDecoder self, OpusSetControlRequest request, uint x) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Ldarg_0();
      Push(request);
      Push(x);
      Push(opus_decoder_ctl_set_uint);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusStatusCode),
        typeof(OpusDecoder*), typeof(OpusSetControlRequest), typeof(uint)));
      return Return<OpusStatusCode>();
#else
    var p = (OpusDecoder*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_decoder_ctl_set_uint(p, request, x);
#endif
  }

  /// <summary>
  /// Sets a control value from an <see cref="OpusDecoder"/>.
  /// </summary>
  /// <param name="self" />
  /// <param name="request">The control request.</param>
  /// <param name="x">The control value.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusStatusCode SetControl(in this OpusDecoder self, OpusSetControlRequest request, short x) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Ldarg_0();
      Push(request);
      Push(x);
      Push(opus_decoder_ctl_set_val16);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusStatusCode),
        typeof(OpusDecoder*), typeof(OpusSetControlRequest), typeof(short)));
      return Return<OpusStatusCode>();
#else
    var p = (OpusDecoder*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_decoder_ctl_set_val16(p, request, x);
#endif
  }

  /// <summary>
  /// Frees an  <see cref="OpusDecoder"/> allocated by <see cref="OpusDecoder.Create(OpusFrequency,int,OpusStatusCode*)"/>.
  /// </summary>
  /// <param name="self" />
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe void Destroy(in this OpusDecoder self) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Ldarg_0();
      Push(opus_decoder_destroy);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(void),
        typeof(OpusDecoder*)));
#else
    var p = (OpusDecoder*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    opus_decoder_destroy(p);
#endif
  }

  /// <summary>
  /// Initializes a previously allocated decoder state.
  /// The state must be at least the size returned by <see cref="OpusDecoder.GetSize"/>.
  /// This is intended for applications which use their own allocator instead of malloc.
  /// </summary>
  /// <param name="self" />
  /// <param name="fs">Sampling rate to decode to (Hz). This must be one of 8000, 12000, 16000, 24000, or 48000.</param>
  /// <param name="channels">Number of channels (1 or 2) to decode.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusStatusCode Init(in this OpusDecoder self, OpusFrequency fs, int channels) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Ldarg_0();
      Push(fs);
      Push(channels);
      Push(opus_decoder_init);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusStatusCode),
        typeof(OpusDecoder*), typeof(OpusFrequency), typeof(int)));
      return Return<OpusStatusCode>();
#else
    var p = (OpusDecoder*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_decoder_init(p, fs, channels);
#endif
  }

  /// <summary>
  /// Gets the number of samples of an Opus packet.
  /// </summary>
  /// <param name="self" />
  /// <param name="packet">Opus packet.</param>
  /// <param name="len">Length of packet.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int GetSamples(in this OpusDecoder self, byte* packet, int len) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Ldarg_0();
      Push(packet);
      Push(len);
      Push(opus_decoder_get_nb_samples);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(int),
        typeof(OpusDecoder*), typeof(byte*), typeof(int)));
      return Return<int>();
#else
    var p = (OpusDecoder*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_decoder_get_nb_samples(p, packet, len);
#endif
  }

  /// <summary>
  /// Gets the number of samples of an Opus packet.
  /// </summary>
  /// <param name="self" />
  /// <param name="packet">Opus packet.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int GetSamples(in this OpusDecoder self, ReadOnlySpan<byte> packet) {
    fixed (byte* pData = packet)
      return GetSamples(self, pData, packet.Length);
  }

  /// <summary>
  /// Resets the codec state to be equivalent to a freshly initialized state.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static OpusStatusCode Reset(in this OpusDecoder self)
    => self.ControlRequest(OpusControlRequest.ResetState);

}
