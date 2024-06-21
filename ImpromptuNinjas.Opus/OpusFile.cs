namespace ImpromptuNinjas.Opus;

[PublicAPI]
public static class OpusFile {

  public const int OpusTag = ('O' | ('p' << 8) | ('u' << 16) | ('s' << 24));

  public const int HeadTag = ('H' | ('e' << 8) | ('a' << 16) | ('d' << 24));

  public const int TagsTag = ('T' | ('a' << 8) | ('g' << 16) | ('s' << 24));

#if !NETSTANDARD1_1 // not supported
    /// <summary>
    /// Loads an Opus file and parses it on demand.
    /// </summary>
    /// <remarks>
    /// Supports tags and comments.
    /// Pictures not yet implemented.
    /// Non-standard (custom) encodings not supported
    /// </remarks>
    /// <param name="filePath">Path to the Opus file.</param>
    public static OpusMappedFile OpenRead(string filePath)
      => new(filePath);
#endif

  /// <summary>
  /// Loads an Opus file and parses it on demand.
  /// </summary>
  /// <remarks>
  /// Supports tags and comments.
  /// Pictures not yet implemented.
  /// Non-standard (custom) encodings not supported
  /// </remarks>
  /// <param name="pointer">Pointer to an Opus file mapped in memory.</param>
  /// <param name="mappingSize">The size of the mapping.</param>
  public static unsafe OpusMappedFile OpenRead(byte* pointer, ulong mappingSize)
    => new(pointer, mappingSize);

  /// <summary>
  /// Loads an Opus file and parses it on demand.
  /// </summary>
  /// <remarks>
  /// Supports tags and comments.
  /// Pictures not yet implemented.
  /// Non-standard (custom) encodings not supported
  /// </remarks>
  /// <param name="stream">A stream containing an Opus file.</param>
  /// <param name="ownedStream">Disposes of the stream when disposing if <see langword="true"/>.</param>
  public static OpusStreamedFile OpenRead(Stream stream, bool ownedStream = false)
    => new(stream, ownedStream);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static unsafe string ReadString(ReadOnlySpan<byte> span, int len) {
#if NETSTANDARD1_1 || NETFRAMEWORK
    var bufPool = ArrayPool<byte>.Shared;
    var buf = bufPool.Rent(len);
    Unsafe.CopyBlockUnaligned(ref buf[0], ref Unsafe.AsRef(span.GetPinnableReference()), (uint) len);
    var str = Encoding.UTF8.GetString(buf, 0, len);
    bufPool.Return(buf);
#elif NETSTANDARD1_4 || NETSTANDARD2_0
      var ptr = Unsafe.AsPointer(ref Unsafe.AsRef(span.GetPinnableReference()));
      var str = Encoding.UTF8.GetString((byte*) ptr, len);
#else
      var str = Encoding.UTF8.GetString(span.Slice(0, len));
#endif
    return str;
  }

}
