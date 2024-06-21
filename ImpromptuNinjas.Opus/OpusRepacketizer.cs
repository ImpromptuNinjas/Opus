namespace ImpromptuNinjas.Opus;

/// <summary>
/// The repacketizer can be used to merge multiple Opus packets into a single packet or alternatively to split Opus
/// packets that have previously been merged.
/// </summary>
/// <remarks>
/// Splitting valid Opus packets is always guaranteed to succeed, whereas
/// merging valid packets only succeeds if all frames have the same mode, bandwidth, and frame size, and when the
/// total duration of the merged packet is no more than 120 ms. The 120 ms limit comes from the specification and
/// limits decoder memory requirements at a point where framing overhead becomes negligible.
/// The repacketizer currently only operates on elementary Opus streams. It will not manipulate multi-stream packets
/// successfully, except in the degenerate case where they consist of data from a single stream.
/// </remarks>
[PublicAPI]
public readonly struct OpusRepacketizer {

  static OpusRepacketizer() => Native.Init();

  #region Dynamic Library Import Table

  // ReSharper disable IdentifierTypo
  // ReSharper disable StringLiteralTypo
  // ReSharper disable InconsistentNaming

  // int opus_repacketizer_get_size(void);
#if !NETSTANDARD2_0_OR_GREATER && !NET
  private static readonly IntPtr opus_repacketizer_get_size =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<int> opus_repacketizer_get_size
    = (delegate* unmanaged[Cdecl]<int>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_repacketizer_get_size));

  // OpusRepacketizer * opus_repacketizer_create(void)
#if !NETSTANDARD2_0_OR_GREATER && !NET
  private static readonly IntPtr opus_repacketizer_create =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusRepacketizer*> opus_repacketizer_create
    = (delegate* unmanaged[Cdecl]<OpusRepacketizer*>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_repacketizer_create));

  // int opus_packet_pad(unsigned char *data, opus_int32 len, opus_int32 new_len)
#if !NETSTANDARD2_0_OR_GREATER && !NET
  private static readonly IntPtr opus_packet_pad =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<byte*, int, int, OpusStatusCode> opus_packet_pad
    = (delegate* unmanaged[Cdecl]<byte*, int, int, OpusStatusCode>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_packet_pad));

  // int opus_packet_unpad(unsigned char *data, opus_int32 len)
#if !NETSTANDARD2_0_OR_GREATER && !NET
  private static readonly IntPtr opus_packet_unpad =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<byte*, int, int> opus_packet_unpad
    = (delegate* unmanaged[Cdecl]<byte*, int, int>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_packet_unpad));

  // int opus_multistream_packet_pad(unsigned char *data, opus_int32 len, opus_int32 new_len, int nb_streams)
#if !NETSTANDARD2_0_OR_GREATER && !NET
  private static readonly IntPtr opus_multistream_packet_pad =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<byte*, int, int, int, OpusStatusCode>
    opus_multistream_packet_pad = (delegate* unmanaged[Cdecl]<byte*, int, int, int, OpusStatusCode>)
#endif
      NativeLibrary.GetExport(Native.Lib, nameof(opus_multistream_packet_pad));

  // int opus_multistream_packet_unpad(unsigned char *data, opus_int32 len, int nb_streams)
#if !NETSTANDARD2_0_OR_GREATER && !NET
  private static readonly IntPtr opus_multistream_packet_unpad =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<byte*, int, int, int> opus_multistream_packet_unpad
    = (delegate* unmanaged[Cdecl]<byte*, int, int, int>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_multistream_packet_unpad));

  // ReSharper restore InconsistentNaming
  // ReSharper restore StringLiteralTypo
  // ReSharper restore IdentifierTypo

  #endregion

  /// <summary>
  /// Gets the size of an OpusRepacketizer structure.
  /// </summary>
  /// <returns>The size in bytes.</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static unsafe int GetSize() {
#if !NETSTANDARD2_0_OR_GREATER && !NET
    Push(opus_repacketizer_get_size);
    Tail();
    Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(int),
      typeof(int)));
    return Return<int>();
#else
    return opus_repacketizer_get_size();
#endif
  }

  private static Lazy<int> _lazySize = new(GetSize);

  public static int Size {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => _lazySize.Value;
  }

  /// <summary>
  /// Allocates memory and initializes the new repacketizer with <see cref="OpusRepacketizerExtensions.Init"/>.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusRepacketizer* Create() {
#if !NETSTANDARD2_0_OR_GREATER && !NET
    Push(opus_repacketizer_create);
    Tail();
    Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusRepacketizer*)));
    return ReturnPointer<OpusRepacketizer>();
#else
    return opus_repacketizer_create();
#endif
  }

  /// <summary>
  /// Pads a given Opus packet to a larger size (possibly changing the TOC sequence).
  /// </summary>
  /// <param name="data">The buffer containing the packet to pad.</param>
  /// <param name="len">The size of the packet. This must be at least 1.</param>
  /// <param name="newLen">
  /// The desired size of the packet after padding. This must be at least as large as len.
  /// </param>
  /// <returns>
  /// <list type="table">
  /// <listheader>
  /// <term><see cref="OpusStatusCode"/></term>
  /// <description>Explanation</description>
  /// </listheader>
  /// <item>
  /// <term><see cref="OpusStatusCode.Ok"/></term>
  /// <description>Success.</description>
  /// </item>
  /// <item>
  /// <term><see cref="OpusStatusCode.BadArg"/></term>
  /// <description><paramref name="len"/> was less than 1 or <paramref name="newLen"/> was less than
  /// <paramref name="len"/>.</description>
  /// </item>
  /// <item>
  /// <term><see cref="OpusStatusCode.InvalidPacket"/></term>
  /// <description><paramref name="data"/> did not contain a valid Opus packet.</description>
  /// </item>
  /// </list>
  /// </returns>
  /// <seealso cref="PadPacket(Span{byte},int)"/>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusStatusCode PadPacket(byte* data, int len, int newLen) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
    Push(data);
    Push(len);
    Push(newLen);
    Push(opus_packet_pad);
    Tail();
    Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusStatusCode),
      typeof(byte*), typeof(int), typeof(int)));
    return Return<OpusStatusCode>();
#else
    return opus_packet_pad(data, len, newLen);
#endif
  }

  /// <inheritdoc cref="PadPacket(byte*,int,int)"/>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusStatusCode PadPacket(Span<byte> data, int newLen) {
    fixed (byte* pData = data)
      return PadPacket(pData, data.Length, newLen);
  }

  /// <summary>
  /// Remove all padding from a given Opus packet and rewrite the TOC sequence to minimize space usage.
  /// </summary>
  /// <param name="data">The buffer containing the packet to strip.</param>
  /// <param name="len">The size of the packet. This must be at least 1.</param>
  /// <returns>
  /// The new size of the output packet on success, or a <see cref="OpusStatusCode"/> on failure.
  /// <list type="table">
  /// <listheader>
  /// <term><see cref="OpusStatusCode"/></term>
  /// <description>Explanation</description>
  /// </listheader>
  /// <item>
  /// <term><see cref="OpusStatusCode.BadArg"/></term>
  /// <description><paramref name="len"/> was less than 1.</description>
  /// </item>
  /// <item>
  /// <term><see cref="OpusStatusCode.InvalidPacket"/></term>
  /// <description><paramref name="data"/> did not contain a valid Opus packet.</description>
  /// </item>
  /// </list>
  /// </returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int UnpadPacket(byte* data, int len) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
    Push(data);
    Push(len);
    Push(opus_packet_pad);
    Tail();
    Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(int),
      typeof(byte*), typeof(int)));
    return Return<int>();
#else
    return opus_packet_unpad(data, len);
#endif
  }

  /// <inheritdoc cref="PadPacket(byte*,int,int)"/>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int UnpadPacket(Span<byte> data) {
    fixed (byte* pData = data)
      return UnpadPacket(pData, data.Length);
  }

  /// <summary>
  /// Pads a given Opus multi-stream packet to a larger size (possibly changing the TOC sequence).
  /// </summary>
  /// <param name="data">The buffer containing the packet to pad.</param>
  /// <param name="len">The size of the packet. This must be at least 1.</param>
  /// <param name="newLen">The desired size of the packet after padding. This must be at least 1.</param>
  /// <param name="nbStreams">
  /// The number of streams (not channels) in the packet.
  /// This must be at least as large as <paramref name="len"/>.
  /// </param>
  /// <returns>
  /// <list type="table">
  /// <listheader>
  /// <term><see cref="OpusStatusCode"/></term>
  /// <description>Explanation</description>
  /// </listheader>
  /// <item>
  /// <term><see cref="OpusStatusCode.Ok"/></term>
  /// <description>Success.</description>
  /// </item>
  /// <item>
  /// <term><see cref="OpusStatusCode.BadArg"/></term>
  /// <description><paramref name="len"/> was less than 1.</description>
  /// </item>
  /// <item>
  /// <term><see cref="OpusStatusCode.InvalidPacket"/></term>
  /// <description><paramref name="data"/> did not contain a valid Opus packet.</description>
  /// </item>
  /// </list>
  /// </returns>
  /// <seealso cref="PadMultistreamPacket(Span{byte},int,int)"/>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusStatusCode PadMultistreamPacket(byte* data, int len, int newLen, int nbStreams) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
    Push(data);
    Push(len);
    Push(newLen);
    Push(nbStreams);
    Push(opus_multistream_packet_pad);
    Tail();
    Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusStatusCode),
      typeof(byte*), typeof(int), typeof(int), typeof(int)));
    return Return<OpusStatusCode>();
#else
    return opus_multistream_packet_pad(data, len, newLen, nbStreams);
#endif
  }

  /// <inheritdoc cref="PadMultistreamPacket(byte*,int,int,int)"/>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusStatusCode PadMultistreamPacket(Span<byte> data, int newLen, int nbStreams) {
    fixed (byte* pData = data)
      return PadMultistreamPacket(pData, data.Length, newLen, nbStreams);
  }

  /// <summary>
  /// Remove all padding from a given Opus multi-stream packet and rewrite the TOC sequence to minimize space usage.
  /// </summary>
  /// <param name="data">The buffer containing the packet to strip.</param>
  /// <param name="len">The size of the packet. This must be at least 1.</param>
  /// <param name="nbStreams"> The number of streams (not channels) in the packet. This must be at least 1. </param>
  /// <returns>
  /// The new size of the output packet on success, or a <see cref="OpusStatusCode"/> on failure.
  /// <list type="table">
  /// <listheader>
  /// <term><see cref="OpusStatusCode"/></term>
  /// <description>Explanation</description>
  /// </listheader>
  /// <item>
  /// <term><see cref="OpusStatusCode.BadArg"/></term>
  /// <description><paramref name="len"/> was less than 1.</description>
  /// </item>
  /// <item>
  /// <term><see cref="OpusStatusCode.InvalidPacket"/></term>
  /// <description><paramref name="data"/> did not contain a valid Opus packet.</description>
  /// </item>
  /// </list>
  /// </returns>
  /// <seealso cref="UnpadMultistreamPacket(Span{byte},int)"/>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int UnpadMultistreamPacket(byte* data, int len, int nbStreams) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
    Push(data);
    Push(len);
    Push(nbStreams);
    Push(opus_multistream_packet_unpad);
    Tail();
    Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(int),
      typeof(byte*), typeof(int), typeof(int)));
    return Return<int>();
#else
    return opus_multistream_packet_unpad(data, len, nbStreams);
#endif
  }

  /// <inheritdoc cref="UnpadMultistreamPacket(byte*,int,int)"/>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int UnpadMultistreamPacket(Span<byte> data, int nbStreams) {
    fixed (byte* pData = data)
      return UnpadMultistreamPacket(pData, data.Length, nbStreams);
  }

}

