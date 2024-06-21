namespace ImpromptuNinjas.Opus;

/// <summary>
/// This contains the complete state of an Opus encoder.
/// </summary>
/// <remarks>
/// It is position independent and can be freely copied if the size is known.
/// Because this is a reference type, a Clone method should be used instead of copy assignment.
/// </remarks>
[PublicAPI]
public readonly struct OpusEncoder {

  static OpusEncoder() => Native.Init();

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

  // OpusEncoder *opus_encoder_create(opus_int32 Fs, int channels, int application, int *error);
#if !NETSTANDARD2_0_OR_GREATER && !NET
    private static readonly IntPtr opus_encoder_create =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusFrequency, int, int, OpusStatusCode*, OpusEncoder*> opus_encoder_create
    = (delegate* unmanaged[Cdecl]<OpusFrequency, int, int, OpusStatusCode*, OpusEncoder*>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_encoder_create));

  // ReSharper restore InconsistentNaming
  // ReSharper restore StringLiteralTypo
  // ReSharper restore IdentifierTypo

  #endregion

  /// <summary>
  /// Allocates and initializes an encoder state.
  /// </summary>
  /// <remarks>
  /// There are three coding modes:
  /// <list type="number">
  /// <item>
  /// <term><see cref="OpusApplication.Voip"/></term> gives best quality at a given bitrate for voice signals.
  /// It enhances the input signal by high-pass filtering and emphasizing formants and harmonics.
  /// Optionally it includes in-band forward error correction to protect against packet loss.
  /// Use this mode for typical VoIP applications. Because of the enhancement, even at high bitrates the output may
  /// sound different from the input.
  /// </item>
  /// <item>
  /// <term><see cref="OpusApplication.Audio"/></term> gives best quality at a given bitrate for most non-voice
  /// signals like music. Use this mode for music and mixed (music/voice) content, broadcast, and applications
  /// requiring less than 15 ms of coding delay.
  /// </item>
  /// <item>
  /// <term><see cref="OpusApplication.RestrictedLowDelay"/></term> configures low-delay mode that disables the
  /// speech-optimized mode in exchange for slightly reduced delay. This mode can only be set on an newly
  /// initialized or freshly reset encoder because it changes the codec delay.
  /// </item>
  /// </list>
  /// </remarks>
  /// <param name="fs">Sampling rate of input signal (Hz) This must be one of 8000, 12000, 16000, 24000, or 48000.</param>
  /// <param name="channels">Number of channels (1 or 2) in input signal.</param>
  /// <param name="application">Coding mode (See <see cref="OpusApplication"/>)</param>
  /// <param name="error">Error codes (See <see cref="OpusStatusCode"/>)</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusEncoder* Create(OpusFrequency fs, int channels, int application, OpusStatusCode* error) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Push(fs);
      Push(channels);
      Push(application);
      Push(error);
      Push(opus_encoder_create);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusEncoder*),
        typeof(OpusFrequency), typeof(int), typeof(int), typeof(int*)));
      Ret();
      throw Unreachable();
#else
    return opus_encoder_create(fs, channels, application, error);
#endif
  }

  /// <inheritdoc cref="Create(OpusFrequency,int,int,OpusStatusCode*)"/>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusEncoder* Create(OpusFrequency fs, int channels, int application) {
    OpusStatusCode error;
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Push(fs);
      Push(channels);
      Push(application);
      Push(&error);
      Push(opus_encoder_create);
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusEncoder*),
        typeof(OpusFrequency), typeof(int), typeof(int), typeof(int*)));
      Pop(out var pEnc);
#else
    var pEnc = opus_encoder_create(fs, channels, application, &error);
#endif
    // ReSharper disable once InvertIf
    if (error != OpusStatusCode.Ok) {
      LibOpus.ThrowIfError(error);
    }

    // ReSharper disable once RedundantCast
    return (OpusEncoder*) pEnc;
  }

  /// <summary>
  /// Gets the size of an <see cref="OpusEncoder"/> structure.
  /// </summary>
  /// <param name="channels">Number of channels. This must be 1 or 2.</param>
  /// <returns>The size in bytes.</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static unsafe int GetSize(int channels) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
      Push(channels);
      Push(opus_encoder_get_size);
      Tail();
      Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(int),
        typeof(int)));
      return Return<int>();
#else
    return opus_encoder_get_size(channels);
#endif
  }

  private static Lazy<int> _lazySize1 = new(() => GetSize(1));

  private static Lazy<int> _lazySize2 = new(() => GetSize(2));

  /// <summary>
  /// The size of an <see cref="OpusEncoder"/> structure for one channel in bytes.
  /// </summary>
  public static int SizeForOneChannel {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => _lazySize1.Value;
  }

  /// <summary>
  /// The size of an <see cref="OpusEncoder"/> structure for two channels in bytes.
  /// </summary>
  public static int SizeForTwoChannels {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => _lazySize2.Value;
  }

  /// <summary>
  /// The final state of the codec's entropy coder.
  /// </summary>
  public unsafe uint FinalRange {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
        uint x;
        LibOpus.ThrowIfError(this.GetControl(OpusGetControlRequest.GetFinalRange, &x));
        return x;
      }
  }

  /// <summary>
  /// The encoder's configured bandpass or the decoder's last bandpass.
  /// </summary>
  /// <remarks>
  /// This prevents the encoder from automatically selecting the bandpass based on the available bitrate.
  /// If an application knows the bandpass of the input audio it is providing, it should normally use
  /// <see cref="MaxBandwidth"/> instead, which still gives the encoder the freedom to reduce the bandpass when the
  /// bitrate becomes too low, for better overall quality.
  /// </remarks>
  public unsafe OpusBandwidth Bandwidth {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
        int x;
        LibOpus.ThrowIfError(this.GetControl(OpusGetControlRequest.GetBandwidth, &x));
        return (OpusBandwidth) x;
      }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    set => LibOpus.ThrowIfError(this.SetControl(OpusSetControlRequest.SetBandwidth, (int) value));
  }

  /// <summary>
  /// The discontinuous transmission state of the encoder.
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
  /// The encoder's configured phase inversion status
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
  /// The sampling rate the encoder was initialized with.
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
  /// The encoder's configured application.
  /// </summary>
  public unsafe OpusApplication Application {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
        int x;
        LibOpus.ThrowIfError(this.GetControl(OpusGetControlRequest.GetApplication, &x));
        return (OpusApplication) x;
      }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    set => LibOpus.ThrowIfError(this.SetControl(OpusSetControlRequest.SetApplication, (int) value));
  }

  /// <summary>
  /// The encoder's complexity configuration.
  /// </summary>
  public unsafe int Bitrate {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
        int x;
        LibOpus.ThrowIfError(this.GetControl(OpusGetControlRequest.GetBitrate, &x));
        return x;
      }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    set => LibOpus.ThrowIfError(this.SetControl(OpusSetControlRequest.SetBitrate, value));
  }

  /// <summary>
  /// The encoder's complexity configuration.
  /// </summary>
  public unsafe int Complexity {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
        int x;
        LibOpus.ThrowIfError(this.GetControl(OpusGetControlRequest.GetComplexity, &x));
        return x;
      }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    set => LibOpus.ThrowIfError(this.SetControl(OpusSetControlRequest.SetComplexity, value));
  }

  /// <summary>
  /// The encoder's use of discontinuous transmission.
  /// </summary>
  public unsafe bool Dtx {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
        int x;
        LibOpus.ThrowIfError(this.GetControl(OpusGetControlRequest.GetDtx, &x));
        return x != 0;
      }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    set => LibOpus.ThrowIfError(this.SetControl(OpusSetControlRequest.SetDtx, value ? 1 : 0));
  }

  /// <summary>
  /// The encoder's use of variable duration frames.
  /// </summary>
  public unsafe OpusFrameSize ExpertFrameDuration {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
        int x;
        LibOpus.ThrowIfError(this.GetControl(OpusGetControlRequest.GetExpertFrameDuration, &x));
        return (OpusFrameSize) x;
      }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    set => LibOpus.ThrowIfError(this.SetControl(OpusSetControlRequest.SetExpertFrameDuration, (int) value));
  }

  /// <summary>
  /// The encoder's forced channel configuration.
  /// </summary>
  public unsafe OpusChannels ForceChannels {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
        int x;
        LibOpus.ThrowIfError(this.GetControl(OpusGetControlRequest.GetForceChannels, &x));
        return (OpusChannels) x;
      }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    set => LibOpus.ThrowIfError(this.SetControl(OpusSetControlRequest.SetForceChannels, (int) value));
  }

  /// <summary>
  /// The encoder's use of inband forward error correction.
  /// </summary>
  public unsafe bool InbandFec {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
        int x;
        LibOpus.ThrowIfError(this.GetControl(OpusGetControlRequest.GetInbandFec, &x));
        return x != 0;
      }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    set => LibOpus.ThrowIfError(this.SetControl(OpusSetControlRequest.SetInbandFec, value ? 1 : 0));
  }

  /// <summary>
  /// The total samples of delay added by the entire codec.
  /// </summary>
  /// <remarks>
  /// This can be queried by the encoder and then the provided number of samples can be skipped on from the start of
  /// the decoder's output to provide time aligned input and output. From the perspective of a decoding application
  /// the real data begins this many samples late.
  /// The decoder contribution to this delay is identical for all decoders, but the encoder portion of the delay may
  /// vary from implementation to implementation, version to version, or even depend on the encoder's initial
  /// configuration. Applications needing delay compensation should call this CTL rather than hard-coding a value.
  /// </remarks>
  public unsafe int Lookahead {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
        int x;
        LibOpus.ThrowIfError(this.GetControl(OpusGetControlRequest.GetLookahead, &x));
        return x;
      }
  }

  /// <summary>
  /// The encoder's signal depth.
  /// </summary>
  /// <remarks>
  /// Input precision in bits, between 8 and 24
  /// </remarks>
  public unsafe int LsbDepth {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
        int x;
        LibOpus.ThrowIfError(this.GetControl(OpusGetControlRequest.GetLsbDepth, &x));
        return x;
      }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    set => LibOpus.ThrowIfError(this.SetControl(OpusSetControlRequest.SetLsbDepth, value));
  }

  /// <summary>
  /// The encoder's maximum allowed bandpass.
  /// </summary>
  /// <remarks>
  /// Applications should normally use this instead of <see cref="Bandwidth"/> (leaving that set to the default,
  /// <see cref="OpusBandwidth.Auto"/>). This allows the application to set an upper bound based on the type of
  /// input it is providing, but still gives the encoder the freedom to reduce the bandpass when the bitrate becomes
  /// too low, for better overall quality.
  /// </remarks>
  public unsafe OpusBandwidth MaxBandwidth {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
        int x;
        LibOpus.ThrowIfError(this.GetControl(OpusGetControlRequest.GetMaxBandwidth, &x));
        return (OpusBandwidth) x;
      }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    set => LibOpus.ThrowIfError(this.SetControl(OpusSetControlRequest.SetMaxBandwidth, (int) value));
  }

  /// <summary>
  /// The encoder's packet loss percentage.
  /// </summary>
  /// <remarks>
  /// In the range 0-100, inclusive.
  /// </remarks>
  public unsafe OpusBandwidth PacketLossPercentage {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
        int x;
        LibOpus.ThrowIfError(this.GetControl(OpusGetControlRequest.GetPacketLossPerc, &x));
        return (OpusBandwidth) x;
      }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    set => LibOpus.ThrowIfError(this.SetControl(OpusSetControlRequest.SetPacketLossPerc, (int) value));
  }

  /// <summary>
  /// The encoder's prediction status.
  /// </summary>
  public unsafe bool PredictionDisabled {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
        int x;
        LibOpus.ThrowIfError(this.GetControl(OpusGetControlRequest.GetPredictionDisabled, &x));
        return x != 0;
      }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    set => LibOpus.ThrowIfError(this.SetControl(OpusSetControlRequest.SetPredictionDisabled, value ? 1 : 0));
  }

  /// <summary>
  /// The encoder's signal type.
  /// </summary>
  public unsafe OpusSignal Signal {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
        int x;
        LibOpus.ThrowIfError(this.GetControl(OpusGetControlRequest.GetSignal, &x));
        return (OpusSignal) x;
      }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    set => LibOpus.ThrowIfError(this.SetControl(OpusSetControlRequest.SetSignal, (int) value));
  }

  /// <summary>
  /// Determines if variable bitrate (VBR) is enabled in the encoder.
  /// </summary>
  /// <remarks>
  /// <see langword="false"/> means hard CBR, <see langword="true"/> means VBR.
  /// </remarks>
  /// <seealso cref="VbrConstraint"/>
  public unsafe bool Vbr {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
        int x;
        LibOpus.ThrowIfError(this.GetControl(OpusGetControlRequest.GetVbr, &x));
        return x != 0;
      }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    set => LibOpus.ThrowIfError(this.SetControl(OpusSetControlRequest.SetVbr, value ? 1 : 0));
  }

  /// <summary>
  /// Determine if constrained variable bitrate (VBR) is enabled in the encoder.
  /// </summary>
  /// <remarks>
  /// <see langword="false"/> means unconstrained VBR, <see langword="true"/> means constrained VBR.
  /// </remarks>
  /// <seealso cref="Vbr"/>
  public unsafe bool VbrConstraint {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get {
        int x;
        LibOpus.ThrowIfError(this.GetControl(OpusGetControlRequest.GetVbr, &x));
        return x != 0;
      }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    set => LibOpus.ThrowIfError(this.SetControl(OpusSetControlRequest.SetVbr, value ? 1 : 0));
  }

}
