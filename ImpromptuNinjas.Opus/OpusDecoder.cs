namespace ImpromptuNinjas.Opus;

/// <summary>
/// This contains the complete state of an Opus decoder.
/// </summary>
/// <remarks>
/// It is position independent and can be freely copied if the size is known.
/// Because this is a reference type, a Clone method should be used instead of copy assignment.
/// </remarks>
[PublicAPI]
public readonly struct OpusDecoder {

  static OpusDecoder() => Native.Init();

  #region Dynamic Library Import Table

  // ReSharper disable IdentifierTypo
  // ReSharper disable StringLiteralTypo
  // ReSharper disable InconsistentNaming

  // int opus_decoder_get_size(int channels);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_decoder_get_size =
#else
  private static readonly unsafe delegate * unmanaged[Cdecl]<int, int> opus_decoder_get_size
    = (delegate * unmanaged[Cdecl]<int, int>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_decoder_get_size));

  // OpusDecoder * opus_decoder_create(opus_int32 Fs, int channels, int *error);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_decoder_create =
#else
  private static readonly unsafe delegate * unmanaged[Cdecl]<OpusFrequency, int, OpusStatusCode*, OpusDecoder*> opus_decoder_create
    = (delegate * unmanaged[Cdecl]<OpusFrequency, int, OpusStatusCode*, OpusDecoder*>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_decoder_create));

  // int opus_packet_parse(const unsigned char *data, opus_int32 len, unsigned char *out_toc, const unsigned char *frames[48], short size[48], int *payload_offset);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_packet_parse =
#else
  private static readonly unsafe delegate * unmanaged[Cdecl]<byte*, int, byte*, byte**, short*, int*, int> opus_packet_parse
    = (delegate * unmanaged[Cdecl]<byte*, int, byte*, byte**, short*, int*, int>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_packet_parse));

  // int opus_packet_get_bandwidth(const unsigned char *data);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_packet_get_bandwidth =
#else
  private static readonly unsafe delegate * unmanaged[Cdecl]<byte*, OpusBandwidth> opus_packet_get_bandwidth
    = (delegate * unmanaged[Cdecl]<byte*, OpusBandwidth>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_packet_get_bandwidth));

  // int opus_packet_get_samples_per_frame(const unsigned char *data, opus_int32 Fs);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_packet_get_samples_per_frame =
#else
  private static readonly unsafe delegate * unmanaged[Cdecl]<byte*, OpusFrequency, int> opus_packet_get_samples_per_frame
    = (delegate * unmanaged[Cdecl]<byte*, OpusFrequency, int>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_packet_get_samples_per_frame));

  // int opus_packet_get_nb_channels(const unsigned char *data);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_packet_get_nb_channels =
#else
  private static readonly unsafe delegate * unmanaged[Cdecl]<byte*, int> opus_packet_get_nb_channels
    = (delegate * unmanaged[Cdecl]<byte*, int>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_packet_get_nb_channels));

  // int opus_packet_get_nb_frames(const unsigned char *packet, opus_int32 len);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_packet_get_nb_frames =
#else
  private static readonly unsafe delegate * unmanaged[Cdecl]<byte*, int, int> opus_packet_get_nb_frames
    = (delegate * unmanaged[Cdecl]<byte*, int, int>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_packet_get_nb_frames));

  // int opus_packet_get_nb_samples(const unsigned char packet[], opus_int32 len, opus_int32 Fs);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_packet_get_nb_samples =
#else
  private static readonly unsafe delegate * unmanaged[Cdecl]<byte*, int, OpusFrequency, int> opus_packet_get_nb_samples
    = (delegate * unmanaged[Cdecl]<byte*, int, OpusFrequency, int>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_packet_get_nb_samples));

  // void opus_pcm_soft_clip(const opus_val16 *pcm, int frame_size, int channels, float *soft_clip_mem);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_pcm_soft_clip =
#else
  private static readonly unsafe delegate * unmanaged[Cdecl]<byte*, int, int, float*, void> opus_pcm_soft_clip
    = (delegate * unmanaged[Cdecl]<byte*, int, int, float*, void>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_pcm_soft_clip));

  // ReSharper restore InconsistentNaming
  // ReSharper restore StringLiteralTypo
  // ReSharper restore IdentifierTypo

  #endregion

  /// <summary>
  /// Allocates and initializes a decoder state.
  /// </summary>
  /// <remarks>
  /// Internally Opus stores data at 48000 Hz, so that should be the default value for Fs.
  /// However, the decoder can efficiently decode to buffers at 8, 12, 16, and 24 kHz so if for some reason the
  /// caller cannot use data at the full sample rate, or knows the compressed data doesn't use the full frequency
  /// range, it can request decoding at a reduced rate.
  /// Likewise, the decoder is capable of filling in either mono or interleaved stereo pcm buffers, at the caller's
  /// request.
  ///
  /// Returns <see langword="null"/> if channels is 0.
  /// </remarks>
  /// <param name="fs">Sample rate to decode at (Hz). This must be one of 8000, 12000, 16000, 24000, or 48000.</param>
  /// <param name="channels">Number of channels (1 or 2) to decode.</param>
  /// <param name="error">See <see cref="OpusStatusCode"/>.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusDecoder* Create(OpusFrequency fs, int channels, OpusStatusCode* error) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Push(fs);
      Push(channels);
      Push(error);
      Push(opus_decoder_create);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusDecoder*),
        typeof(OpusFrequency), typeof(int), typeof(OpusStatusCode*)));
      Ret();
      throw Unreachable();
#else
    return opus_decoder_create(fs, channels, error);
#endif
  }

  /// <summary>
  /// Allocates and initializes a decoder state.
  /// </summary>
  /// <remarks>
  /// Internally Opus stores data at 48000 Hz, so that should be the default value for Fs.
  /// However, the decoder can efficiently decode to buffers at 8, 12, 16, and 24 kHz so if for some reason the
  /// caller cannot use data at the full sample rate, or knows the compressed data doesn't use the full frequency
  /// range, it can request decoding at a reduced rate.
  /// Likewise, the decoder is capable of filling in either mono or interleaved stereo pcm buffers, at the caller's
  /// request.
  ///
  /// Returns <see langword="null"/> if channels is 0.
  /// </remarks>
  /// <param name="fs">Sample rate to decode at (Hz). This must be one of 8000, 12000, 16000, 24000, or 48000.</param>
  /// <param name="channels">Number of channels (1 or 2) to decode.</param>
  public static unsafe OpusDecoder* Create(OpusFrequency fs, int channels) {
    OpusStatusCode error;
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Push(fs);
      Push(channels);
      Push(&error);
      Push(opus_decoder_create);
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusDecoder*),
        typeof(OpusFrequency), typeof(int), typeof(OpusStatusCode*)));
      Pop(out IntPtr pDec);
#else
    var pDec = opus_decoder_create(fs, channels, &error);
#endif
    // ReSharper disable once InvertIf
    if (error != OpusStatusCode.Ok)
      LibOpus.ThrowIfError(error);

    // ReSharper disable once RedundantCast
    return (OpusDecoder*) pDec;
  }

  /// <summary>
  /// Gets the size of an <see cref="OpusDecoder"/> structure.
  /// </summary>
  /// <param name="channels">Number of channels. This must be 1 or 2.</param>
  /// <returns>The size in bytes.</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int GetSize(int channels) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Push(channels);
      Push(opus_decoder_get_size);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(int),
        typeof(int)));
      return Return<int>();
#else
    return opus_decoder_get_size(channels);
#endif
  }

  private static Lazy<int> _lazySize1 = new(() => GetSize(1));

  private static Lazy<int> _lazySize2 = new(() => GetSize(2));

  /// <summary>
  /// The size of an <see cref="OpusDecoder"/> structure for one channel in bytes.
  /// </summary>
  public static int SizeForOneChannel {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => _lazySize1.Value;
  }

  /// <summary>
  /// The size of an <see cref="OpusDecoder"/> structure for two channels in bytes.
  /// </summary>
  public static int SizeForTwoChannels {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => _lazySize2.Value;
  }

  /// <summary>
  /// Applies soft-clipping to bring a float signal within the [-1,1] range.
  /// </summary>
  /// <remarks>
  /// If the signal is already in that range, nothing is done.
  /// If there are values outside of [-1,1], then the signal is clipped as smoothly as possible to both fit in the
  /// range and avoid creating excessive distortion in the process.
  /// </remarks>
  /// <param name="pcm">Input PCM and modified PCM.</param>
  /// <param name="frameSize">Number of samples per channel to process.</param>
  /// <param name="channels">Number of channels.</param>
  /// <param name="softClipMem">State memory for the soft clipping process. One float per channel, initialized to zero.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe void PcmSoftClip(byte* pcm, int frameSize, int channels, float* softClipMem) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Push(pcm);
      Push(frameSize);
      Push(channels);
      Push(softClipMem);
      Push(opus_pcm_soft_clip);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(void),
        typeof(byte*), typeof(int), typeof(int), typeof(float*)));
#else
    opus_pcm_soft_clip(pcm, frameSize, channels, softClipMem);
#endif
  }

  /// <inheritdoc cref="PcmSoftClip(byte*,int,int,float*)"/>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe void PcmSoftClip(Span<byte> pcm, int frameSize, int channels) {
    var softClipMem = new float[channels];
    fixed (byte* pPcm = pcm)
    fixed (float* pSoftClipMem = softClipMem)
      PcmSoftClip(pPcm, frameSize, channels, pSoftClipMem);
  }

  /// <summary>
  /// Gets the bandwidth of an Opus packet.
  /// </summary>
  /// <param name="data">Opus packet.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusBandwidth GetBandwidth(byte* data) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Push(data);
      Push(opus_packet_get_bandwidth);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusBandwidth),
        typeof(byte*)));
      return Return<OpusBandwidth>();
#else
    return opus_packet_get_bandwidth(data);
#endif
  }

  /// <inheritdoc cref="GetBandwidth(byte*)"/>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusBandwidth GetBandwidth(ReadOnlySpan<byte> data) {
    fixed (byte* pData = data)
      return GetBandwidth(pData);
  }

  /// <summary>
  /// Gets the number of channels from an Opus packet.
  /// </summary>
  /// <param name="data">Opus packet.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int GetChannels(byte* data) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Push(data);
      Push(opus_packet_get_nb_channels);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(int),
        typeof(byte*)));
      return Return<int>();
#else
    return opus_packet_get_nb_channels(data);
#endif
  }

  /// <inheritdoc cref="GetChannels(byte*)"/>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int GetChannels(ReadOnlySpan<byte> data) {
    fixed (byte* pData = data)
      return GetChannels(pData);
  }

  /// <summary>
  /// Gets the number of frames in an Opus packet.
  /// </summary>
  /// <param name="packet">Opus packet.</param>
  /// <param name="len">Length of packet.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int GetFrames(byte* packet, int len) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Push(packet);
      Push(len);
      Push(opus_packet_get_nb_frames);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(int),
        typeof(byte*), typeof(int)));
      return Return<int>();
#else
    return opus_packet_get_nb_frames(packet, len);
#endif
  }

  /// <inheritdoc cref="GetFrames(byte*,int)"/>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int GetFrames(ReadOnlySpan<byte> data) {
    fixed (byte* pData = data)
      return GetFrames(pData, data.Length);
  }

  /// <summary>
  /// Gets the number of samples of an Opus packet.
  /// </summary>
  /// <param name="packet">Opus packet.</param>
  /// <param name="len">Length of packet.</param>
  /// <param name="fs">Sampling rate in Hz. This must be a multiple of 400, or inaccurate results will be returned.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int GetSamples(byte* packet, int len, OpusFrequency fs) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Push(packet);
      Push(len);
      Push(fs);
      Push(opus_packet_get_nb_samples);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(int),
        typeof(byte*), typeof(int), typeof(OpusFrequency)));
      return Return<int>();
#else
    return opus_packet_get_nb_samples(packet, len, fs);
#endif
  }

  /// <inheritdoc cref="GetSamples(byte*,int,OpusFrequency)"/>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int GetSamples(ReadOnlySpan<byte> data, OpusFrequency fs) {
    fixed (byte* pData = data)
      return GetSamples(pData, data.Length, fs);
  }

  /// <summary>
  /// Gets the number of samples per frame from an Opus packet.
  /// </summary>
  /// <param name="data">Opus packet. This must contain at least one byte of data.</param>
  /// <param name="fs"> Sampling rate in Hz. This must be a multiple of 400, or inaccurate results will be returned.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int GetSamplesPerFrame(byte* data, OpusFrequency fs) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Push(data);
      Push(fs);
      Push(opus_packet_get_samples_per_frame);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(int),
        typeof(byte*), typeof(OpusFrequency)));
      return Return<int>();
#else
    return opus_packet_get_samples_per_frame(data, fs);
#endif
  }

  /// <summary>
  /// Gets the number of samples per frame from an Opus packet.
  /// </summary>
  /// <param name="data">Opus packet. This must contain at least one byte of data.</param>
  /// <param name="fs"> Sampling rate in Hz. This must be a multiple of 400, or inaccurate results will be returned.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int GetSamplesPerFrame(ReadOnlySpan<byte> data, OpusFrequency fs) {
    fixed (byte* pData = data)
      return GetSamplesPerFrame(pData, fs);
  }

  /// <summary>
  /// Parse an Opus packet into one or more frames.
  /// </summary>
  /// <remarks>
  /// <see cref="OpusDecoderExtensions.Decode"/> will perform this operation internally so most applications do not need to use this
  /// function. This function does not copy the frames, the returned pointers are pointers into the input packet.
  /// </remarks>
  /// <param name="data">Opus packet to be parsed.</param>
  /// <param name="len">Size of <paramref name="data"/>.</param>
  /// <param name="outToc">TOC pointer.</param>
  /// <param name="frames">48 encapsulated frames.</param>
  /// <param name="sizes">48 sizes of the encapsulated frames.</param>
  /// <param name="payloadOffset">The position of the payload within the packet in bytes.</param>
  /// <returns>Number of frames.</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int ParsePacket(byte* data, int len, byte* outToc, byte** frames, short* sizes, int* payloadOffset) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Push(data);
      Push(len);
      Push(outToc);
      Push(frames);
      Push(sizes);
      Push(payloadOffset);
      Push(opus_packet_parse);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(int),
        typeof(byte*), typeof(int), typeof(byte*), typeof(byte**), typeof(short*), typeof(int*)));
      return Return<int>();
#else
    return opus_packet_parse(data, len, outToc, frames, sizes, payloadOffset);
#endif
  }

  /// <inheritdoc cref="ParsePacket(byte*,int,byte*,byte**,short*,int*)"/>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int ParsePacket(ReadOnlySpan<byte> data, Span<byte> outToc, Span<IntPtr> frames, Span<short> sizes, int* payloadOffset) {
    fixed (byte* pData = data)
    fixed (byte* pOutToc = outToc)
    fixed (IntPtr* pFrames = frames)
    fixed (short* pSizes = sizes)
      return ParsePacket(pData, data.Length, pOutToc, (byte**) pFrames, pSizes, payloadOffset);
  }

  /// <summary>
  /// The encoder's configured bandpass or the decoder's last bandpass.
  /// </summary>
  public unsafe OpusBandwidth Bandwidth {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
      int x;
      LibOpus.ThrowIfError(this.GetControl(OpusGetControlRequest.GetBandwidth, &x));
      return (OpusBandwidth) x;
    }
  }

  /// <summary>
  /// The DTX state of the encoder.
  /// </summary>
  public unsafe bool InDtx {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
      int x;
      LibOpus.ThrowIfError(this.GetControl(OpusGetControlRequest.GetBandwidth, &x));
      return x != 0;
    }
  }

  /// <summary>
  /// The decoder's configured phase inversion status
  /// </summary>
  public unsafe bool PhaseInversionDisabled {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
      int x;
      LibOpus.ThrowIfError(this.GetControl(OpusGetControlRequest.GetPhaseInversionDisabled, &x));
      return x != 0;
    }
  }

  /// <summary>
  /// The sampling rate the decoder was initialized with.
  /// </summary>
  public unsafe OpusFrequency SampleRate {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
      int x;
      LibOpus.ThrowIfError(this.GetControl(OpusGetControlRequest.GetSampleRate, &x));
      return (OpusFrequency) x;
    }
  }

  /// <summary>
  /// The decoder's gain adjustment.
  /// </summary>
  /// <remarks>
  /// Amount to scale PCM signal by in Q8 dB units.
  /// This has a maximum range of -32768 to 32767 inclusive.
  /// The default is zero indicating no adjustment.
  /// This setting survives decoder reset.
  /// </remarks>
  public unsafe int Gain {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
      int x;
      LibOpus.ThrowIfError(this.GetControl(OpusGetControlRequest.GetGain, &x));
      return x;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    set => LibOpus.ThrowIfError(this.SetControl(OpusSetControlRequest.SetGain, value));
  }

  /// <summary>
  /// The duration (in samples) of the last packet successfully decoded or concealed.
  /// </summary>
  public unsafe int LastPacketDuration {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
      int x;
      LibOpus.ThrowIfError(this.GetControl(OpusGetControlRequest.GetLastPacketDuration, &x));
      return x;
    }
  }

  /// <summary>
  /// The pitch of the last decoded frame, if available.
  /// </summary>
  /// <remarks>
  /// This can be used for any post-processing algorithm requiring the use of pitch, e.g. time stretching/shortening.
  /// If the last frame was not voiced, or if the pitch was not coded in the frame, then zero is returned.
  /// </remarks>
  /// <returns>
  /// Pitch period at 48 kHz (or 0 if not available).
  /// </returns>
  public unsafe int Pitch {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
      int x;
      LibOpus.ThrowIfError(this.GetControl(OpusGetControlRequest.GetPitch, &x));
      return x;
    }
  }

}
