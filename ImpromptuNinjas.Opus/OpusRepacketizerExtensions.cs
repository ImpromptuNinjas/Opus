namespace ImpromptuNinjas.Opus;

[PublicAPI]
public static class OpusRepacketizerExtensions {

  #region Dynamic Library Import Table

  // ReSharper disable IdentifierTypo
  // ReSharper disable StringLiteralTypo
  // ReSharper disable InconsistentNaming

  // OpusRepacketizer * opus_repacketizer_init(OpusRepacketizer *rp)
#if !NETSTANDARD2_0_OR_GREATER && !NET
  private static readonly IntPtr opus_repacketizer_init =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusRepacketizer*, OpusRepacketizer*> opus_repacketizer_init
    = (delegate* unmanaged[Cdecl]<OpusRepacketizer*, OpusRepacketizer*>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_repacketizer_init));

  // void opus_repacketizer_destroy(OpusRepacketizer *rp)
#if !NETSTANDARD2_0_OR_GREATER && !NET
  private static readonly IntPtr opus_repacketizer_destroy =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusRepacketizer*, void> opus_repacketizer_destroy
    = (delegate* unmanaged[Cdecl]<OpusRepacketizer*, void>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_repacketizer_destroy));

  // int opus_repacketizer_cat(OpusRepacketizer *rp, const unsigned char *data, opus_int32 len)
#if !NETSTANDARD2_0_OR_GREATER && !NET
  private static readonly IntPtr opus_repacketizer_cat =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusRepacketizer*, byte*, int, OpusStatusCode>
    opus_repacketizer_cat = (delegate* unmanaged[Cdecl]<OpusRepacketizer*, byte*, int, OpusStatusCode>)
#endif
      NativeLibrary.GetExport(Native.Lib, nameof(opus_repacketizer_cat));

  // int opus_repacketizer_out_range(OpusRepacketizer *rp, int begin, int end, unsigned char *data, opus_int32 maxlen)
#if !NETSTANDARD2_0_OR_GREATER && !NET
  private static readonly IntPtr opus_repacketizer_out_range =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusRepacketizer*, int, int, byte*, int, int>
    opus_repacketizer_out_range = (delegate* unmanaged[Cdecl]<OpusRepacketizer*, int, int, byte*, int, int>)
#endif
      NativeLibrary.GetExport(Native.Lib, nameof(opus_repacketizer_out_range));

  // int opus_repacketizer_get_nb_frames(OpusRepacketizer *rp)
#if !NETSTANDARD2_0_OR_GREATER && !NET
  private static readonly IntPtr opus_repacketizer_get_nb_frames =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusRepacketizer*, int> opus_repacketizer_get_nb_frames
    = (delegate* unmanaged[Cdecl]<OpusRepacketizer*, int>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_repacketizer_get_nb_frames));

  // int opus_repacketizer_out(OpusRepacketizer *rp, unsigned char *data, opus_int32 maxlen)
#if !NETSTANDARD2_0_OR_GREATER && !NET
  private static readonly IntPtr opus_repacketizer_out =
#else
  private static readonly unsafe delegate* unmanaged[Cdecl]<OpusRepacketizer*, byte*, int, int>
    opus_repacketizer_out = (delegate* unmanaged[Cdecl]<OpusRepacketizer*, byte*, int, int>)
#endif
      NativeLibrary.GetExport(Native.Lib, nameof(opus_repacketizer_out));




  /// <summary>
  /// Frees an OpusRepacketizer allocated by <see cref="OpusRepacketizer.Create"/>.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe void Destroy(in this OpusRepacketizer self) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
    Ldarg_0();
    Push(opus_repacketizer_destroy);
    Tail();
    Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(void),
      typeof(OpusRepacketizer*)));
#else
    var p = (OpusRepacketizer*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    opus_repacketizer_destroy(p);
#endif
  }

  // ReSharper restore InconsistentNaming
  // ReSharper restore StringLiteralTypo
  // ReSharper restore IdentifierTypo

  #endregion


  /// <summary>
  /// (Re)initializes a previously allocated repacketizer state.
  /// </summary>
  /// <remarks>
  /// The state must be at least the size returned by <see cref="OpusRepacketizer.GetSize"/>.
  /// This can be used for applications which use their own allocator instead of <c>malloc()</c>.
  /// It must also be called to reset the queue of packets waiting to be repacketized, which is necessary if the
  /// maximum packet duration of 120 ms is reached or if you wish to submit packets with a different Opus
  /// configuration (coding mode, audio bandwidth, frame size, or channel count).
  /// Failure to do so will prevent a new packet from being added with <see cref="Concat"/>.
  /// </remarks>
  /// <returns>The same pointer that was passed in.</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusRepacketizer* Init(in this OpusRepacketizer self) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
    Ldarg_0();
    Push(opus_repacketizer_init);
    Tail();
    Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusRepacketizer*),
      typeof(OpusRepacketizer*)));
    return ReturnPointer<OpusRepacketizer>();
#else
    var p = (OpusRepacketizer*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_repacketizer_init(null);
#endif
  }

  /// <summary>
  /// Add a packet to the current repacketizer state.
  /// </summary>
  /// <remarks>
  /// This packet must match the configuration of any packets already submitted for repacketization since the last
  /// call to <see cref="Init()"/>. This means that it must have the same
  /// coding mode, audio bandwidth, frame size, and channel count.
  /// This can be checked in advance by examining the top 6 bits of the first byte of the packet, and ensuring they
  /// match the top 6 bits of the first byte of any previously submitted packet.
  /// The total duration of audio in the repacketizer state also must not exceed 120 ms, the maximum duration of a
  /// single packet, after adding this packet.
  /// The contents of the current repacketizer state can be extracted into new packets using <see cref="Out(byte*,int)"/> or
  /// <see cref="OutRange"/>.
  /// In order to add a packet with a different configuration or to add more audio beyond 120 ms, you must clear
  /// the repacketizer state by calling <see cref="Init"/>. If a packet is
  /// too large to add to the current repacketizer state, no part of it is added, even if it contains multiple
  /// frames, some of which might fit. If you wish to be able to add parts of such packets, you should first use
  /// another repacketizer to split the packet into pieces and add them individually.
  /// </remarks>
  /// <param name="self" />
  /// <param name="data">
  /// The packet data.
  /// The application must ensure this pointer remains valid until the next call to
  /// <see cref="Init"/> or
  /// <see cref="Destroy"/>.
  /// </param>
  /// <param name="len">The number of bytes in the packet data.</param>
  /// <returns>
  /// <list type="table">
  /// <listheader>
  /// <term><see cref="OpusStatusCode"/></term>
  /// <description>Explanation</description>
  /// </listheader>
  /// <item>
  /// <term><see cref="OpusStatusCode.Ok"/></term>
  /// <description>The packet's contents have been added to the repacketizer state.</description>
  /// </item>
  /// <item>
  /// <term><see cref="OpusStatusCode.InvalidPacket"/></term>
  /// <description>The packet did not have a valid TOC sequence, the packet's TOC sequence was not compatible with
  /// previously submitted packets (because the coding mode, audio bandwidth, frame size, or channel count did not
  /// match), or adding this packet would increase the total amount of audio stored in the repacketizer state to
  /// more than 120 ms.</description>
  /// </item>
  /// </list>
  /// </returns>
  /// <seealso cref="Concat"/>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusStatusCode Concat(in this OpusRepacketizer self, byte* data, int len) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
    Ldarg_0();
    Push(data);
    Push(len);
    Push(opus_repacketizer_cat);
    Tail();
    Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(OpusStatusCode),
      typeof(OpusRepacketizer*), typeof(byte*), typeof(int)));
    return Return<OpusStatusCode>();
#else
    var p = (OpusRepacketizer*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_repacketizer_cat(p, data, len);
#endif
  }

  /// <inheritdoc cref="OpusRepacketizer.Concat(byte*,int)"/>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe OpusStatusCode Concat(in this OpusRepacketizer self, Span<byte> data) {
    fixed (byte* pData = data)
      return Concat(self, pData, data.Length);
  }

  /// <summary>
  /// Construct a new packet from data previously submitted to the repacketizer state via <see cref="Concat(byte*,int)"/>.
  /// </summary>
  /// <param name="self" />
  /// <param name="begin">
  /// The index of the first frame in the current repacketizer state to include in the output.
  /// </param>
  /// <param name="end">
  /// One past the index of the last frame in the current repacketizer state to include in the output.
  /// </param>
  /// <param name="data">The buffer in which to store the output packet.</param>
  /// <param name="maxLen">
  /// The maximum number of bytes to store in the output buffer.
  /// In order to guarantee success, this should be at least <c>1276</c> for a single frame, or for multiple frames,
  /// <c>1277*(<paramref name="end"/>-<paramref name="begin"/>)</c>. However,
  /// <c>(<paramref name="end"/>-<paramref name="begin"/>)+(the size of all packet data submitted to the
  /// repacketizer since the last call to <see cref="Init()"/> or
  /// <see cref="OpusRepacketizer.Create"/>)</c> is also sufficient, and possibly much smaller.
  /// </param>
  /// <returns>The total size of the output packet on success, or <see cref="OpusStatusCode"/> on failure.</returns>
  /// <seealso cref="OutRange"/>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int OutRange(in this OpusRepacketizer self, int begin, int end, byte* data, int maxLen) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
    Ldarg_0();
    Push(begin);
    Push(end);
    Push(data);
    Push(maxLen);
    Push(opus_repacketizer_out_range);
    Tail();
    Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(int),
      typeof(OpusRepacketizer*), typeof(int), typeof(int), typeof(byte*), typeof(int)));
    return Return<int>();
#else
    var p = (OpusRepacketizer*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_repacketizer_out_range(p, begin, end, data, maxLen);
#endif
  }

  /// <inheritdoc cref="OutRange(int,int,byte*,int)"/>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int OutRange(in this OpusRepacketizer self, int begin, int end, Span<byte> data) {
    fixed (byte* pData = data)
      return OutRange(self, begin, end, pData, data.Length);
  }

  /// <summary>
  /// Get the total number of frames contained in packet data submitted to the repacketizer state so far via
  /// <see cref="Concat(byte*,int)"/> since the last call to <see cref="Init"/> or
  /// <see cref="OpusRepacketizer.Create"/>.
  /// </summary>
  /// <remarks>
  /// This defines the valid range of packets that can be extracted with <see cref="OutRange(int,int,byte*,int)"/> or
  /// <see cref="Out(byte*,int)"/>.
  /// </remarks>
  /// <returns>
  /// The total number of frames contained in the packet data submitted to the repacketizer state.
  /// </returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int GetNumberOfFrames(in this OpusRepacketizer self) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
    Ldarg_0();
    Push(opus_repacketizer_get_nb_frames);
    Tail();
    Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(int),
      typeof(OpusRepacketizer*)));
    return Return<int>();
#else
    var p = (OpusRepacketizer*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_repacketizer_get_nb_frames(p);
#endif
  }

  /// <summary>
  /// Construct a new packet from data previously submitted to the repacketizer state via <see cref="Concat(byte*,int)"/>.
  /// This is a convenience routine that returns all the data submitted so far in a single packet.
  /// </summary>
  /// <remarks>
  /// It is equivalent to the following call;
  /// <see cref="OutRange(int,int,byte*,int)"/>(<c>0</c>, <see cref="GetNumberOfFrames"/>(),
  /// <paramref name="data"/>, <paramref name="maxLen"/>).
  /// </remarks>
  /// <param name="data">The buffer in which to store the output packet.</param>
  /// <param name="maxLen">
  /// The maximum number of bytes to store in the output buffer.
  /// In order to guarantee success, this should be at least <c>1276</c> for a single frame, or for multiple frames,
  /// <c>1277*<see cref="GetNumberOfFrames"/>()</c>. However,
  /// <c><see cref="GetNumberOfFrames"/>()+(the size of all packet data submitted to the
  /// repacketizer since the last call to <see cref="Init"/> or
  /// <see cref="OpusRepacketizer.Create"/>)</c> is also sufficient, and possibly much smaller.
  /// </param>
  /// <returns>The total size of the output packet on success, or <see cref="OpusStatusCode"/> on failure.</returns>
  /// <seealso cref="Out(Span{byte})"/>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int Out(in this OpusRepacketizer self, byte* data, int maxLen) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
    Ldarg_0();
    Push(data);
    Push(maxLen);
    Push(opus_repacketizer_out_range);
    Tail();
    Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(int),
      typeof(OpusRepacketizer*), typeof(byte*), typeof(int)));
    return Return<int>();
#else
    var p = (OpusRepacketizer*) Unsafe.AsPointer(ref Unsafe.AsRef(in self));
    return opus_repacketizer_out(p, data, maxLen);
#endif
  }

  /// <inheritdoc cref="Out(byte*,int)"/>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int Out(in this OpusRepacketizer self, Span<byte> data) {
    fixed (byte* pData = data)
      return Out(self, pData, data.Length);
  }

}
