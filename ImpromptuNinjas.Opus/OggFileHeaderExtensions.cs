namespace ImpromptuNinjas.Opus;

public static class OggFileHeaderExtensions {

  /// <summary>
  /// References a segment table entry in the header.
  /// </summary>
  /// <remarks>
  /// The segment table is a vector of 8-bit values, each indicating the length of the corresponding segment within
  /// the page body. The number of segments is determined from the preceding Page Segments field.
  /// Each segment is between 0 and 255 bytes in length.
  ///
  /// The segments provide a way to group segments into packets, which are meaningful units of data for the decoder.
  /// When the segment's length is indicated to be 255, this indicates that the following segment is to be
  /// concatenated to this one and is part of the same packet. When the segment's length is 0–254, this indicates that
  /// this segment is the final segment in this packet. Where a packet's length is a multiple of 255, the final
  /// segment is length 0.
  ///
  /// Where the final packet continues on the next page, the final segment value is 255, and the continuation flag is
  /// set on the following page to indicate that the start of the new page is a continuation of last page.
  /// </remarks>
  /// <returns>A segment table entry.</returns>
  public static ref readonly byte ReadSegmentTable(in this OggFileHeader header, byte index) {
      if (index >= header.PageSegments) throw new ArgumentOutOfRangeException(nameof(index));

      return ref Unsafe.AddByteOffset(ref Unsafe.AsRef(header.PageSegments), (IntPtr) (1 + index));
    }

  /// <summary>
  /// References a segment table entry in the header.
  /// </summary>
  /// <remarks>
  /// The segment table is a vector of 8-bit values, each indicating the length of the corresponding segment within
  /// the page body. The number of segments is determined from the preceding Page Segments field.
  /// Each segment is between 0 and 255 bytes in length.
  ///
  /// The segments provide a way to group segments into packets, which are meaningful units of data for the decoder.
  /// When the segment's length is indicated to be 255, this indicates that the following segment is to be
  /// concatenated to this one and is part of the same packet. When the segment's length is 0–254, this indicates that
  /// this segment is the final segment in this packet. Where a packet's length is a multiple of 255, the final
  /// segment is length 0.
  ///
  /// Where the final packet continues on the next page, the final segment value is 255, and the continuation flag is
  /// set on the following page to indicate that the start of the new page is a continuation of last page.
  /// </remarks>
  /// <returns>A segment table entry.</returns>
  public static ref byte SegmentTable(ref this OggFileHeader header, byte index) {
      if (index >= header.PageSegments) throw new ArgumentOutOfRangeException(nameof(index));

      return ref Unsafe.AddByteOffset(ref header.PageSegments, (IntPtr) (1 + index));
    }

  /// <summary>
  /// Validates the capture pattern is the expected 'OggS' magic value.
  /// </summary>
  public static bool ValidateCapturePattern(in this OggFileHeader header)
    => header.CapturePattern == ('O' | ('g' << 8) | ('g' << 16) | ('S' << 24));

  public static unsafe void VisitSegments(ref this OggFileHeader header, [InstantHandle] SpanAction<byte> segmentHandler) {
      var segmentBytes = header.PageSegments;
      ref readonly var firstDataByte = ref Unsafe.AddByteOffset(ref header.PageSegments, (IntPtr) (1 + segmentBytes));

      var dataPointer = Unsafe.AsPointer(ref Unsafe.AsRef(in firstDataByte));

      var segmentSize = 0;
      for (byte i = 0; i < segmentBytes; ++i) {
        var value = header.ReadSegmentTable(i);
        segmentSize += value;
        if (value == 255)
          continue;

        segmentHandler(new Span<byte>(dataPointer, segmentSize));
        segmentSize = 0;
      }
    }

  /// <summary>
  /// Read each segment as described by the header.
  /// </summary>
  /// <param name="header">The Ogg header structure in memory. Must be inline with data.</param>
  /// <param name="visitor">A callback that will visit the segment.</param>
  /// <returns>The total size of all segments in bytes.</returns>
  public static unsafe ulong ReadSegments(in this OggFileHeader header, [InstantHandle] ReadOnlySpanAction<byte> visitor) {
      var segmentBytes = header.PageSegments;
      ref readonly var firstDataByte = ref Unsafe.AddByteOffset(ref Unsafe.AsRef(header.PageSegments), (IntPtr) (1 + segmentBytes));

      var initialDataPointer = Unsafe.AsPointer(ref Unsafe.AsRef(in firstDataByte));

      var dataPointer = initialDataPointer;

      var segmentSize = 0;
      for (byte i = 0; i < segmentBytes; ++i) {
        var value = header.ReadSegmentTable(i);
        segmentSize += value;
        if (value == 255)
          continue;

        visitor(new ReadOnlySpan<byte>(dataPointer, segmentSize));
        dataPointer = Unsafe.Add<byte>(dataPointer, segmentSize);
        segmentSize = 0;
      }

      return ((UIntPtr) dataPointer).ToUInt64() - ((UIntPtr) initialDataPointer).ToUInt64();
    }

}

public delegate void SpanAction<T>(Span<T> span);

public delegate void ReadOnlySpanAction<T>(ReadOnlySpan<T> span);