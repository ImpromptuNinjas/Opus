namespace ImpromptuNinjas.Opus;

[PublicAPI]
public sealed partial class OpusStreamedFile : IEnumerable<ArraySegment<byte>>, IDisposable {

  private readonly Stream _stream;

  private bool _owned;

  private readonly Dictionary<string, string?> _tags = new();

  private readonly LinkedList<string> _comments = [];

  private readonly List<ArraySegment<byte>> _packets = [];

  private int _channelCount;

  private bool _headRead;

  private bool _tagsRead;

  private bool _headerComplete;

  private int _parsedPageNum;

  private bool _readAllPackets;

  private bool _disposed;

#if !NETSTANDARD1_1
  /// <summary>
  /// Loads an Opus file and parses it on demand.
  /// </summary>
  /// <remarks>
  /// Supports tags and comments.
  /// Pictures not yet implemented.
  /// Non-standard (custom) encodings not supported
  /// </remarks>
  /// <param name="filePath">Path to the Opus file.</param>
  public OpusStreamedFile(string filePath) {
      _stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, 65536, FileOptions.SequentialScan);
      _owned = true;
    }
#endif

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
  public OpusStreamedFile(Stream stream, bool ownedStream) {
      _stream = stream;
      _owned = ownedStream;
    }

  public int ChannelCount {
    get {
        if (!_headerComplete)
          ReadHeader();
        return _channelCount;
      }
  }

  /// <summary>
  /// Provides a mechanism to read tags.
  /// </summary>
  public IReadOnlyDictionary<string, string?> Tags {
    get {
        if (!_headerComplete)
          ReadHeader();
        return _tags;
      }
  }

  /// <summary>
  /// Provides a mechanism to read comments.
  /// </summary>
  public IEnumerable<string> Comments {
    get {
        if (!_headerComplete)
          ReadHeader();
        return _comments;
      }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private ref readonly T2 CastAsRefReadOnly<T1, T2>(ReadOnlySpan<T1> span)
    => ref Unsafe.As<T1, T2>(ref Unsafe.AsRef(span.GetPinnableReference()));

  /// <summary>
  /// Read each segment as described by the header.
  /// </summary>
  /// <param name="segmentSizingInfo">Pointer to the header page's segment sizing info, the part that should immediately follow the header.</param>
  /// <param name="visitor">A callback that will visit the segment.</param>
  /// <returns>The total size of all segments in bytes.</returns>
  public static ulong ReadSegmentSizes(ReadOnlySpan<byte> segmentSizingInfo, [InstantHandle] Action<ulong> visitor) {
      var totalSize = 0uL;
      var segmentSize = 0uL;
      for (byte i = 0; i < segmentSizingInfo.Length; ++i) {
        var value = segmentSizingInfo[i];
        segmentSize += value;
        if (value == 255)
          continue;

        visitor(segmentSize);

        totalSize += segmentSize;

        segmentSize = 0;
      }

      return totalSize;
    }

  private unsafe void ReadHeader() {
    if (_disposed) throw new ObjectDisposedException(nameof(OpusStreamedFile));
    if (_headerComplete) throw new InvalidOperationException("Header already read.");
    // read header first

    var headerSize = sizeof(OggFileHeader);
    var headerBuf = ArrayPool<byte>.Shared.Rent(headerSize);
    var headerSpan = new Span<byte>(headerBuf, 0, headerSize);
    do {
      {
        var span = headerSpan;
        var read = _stream.Read(span);
        while (read != span.Length)
          read = _stream.Read(span = span.Slice(read));
      }

      ref readonly var header = ref CastAsRefReadOnly<byte, OggFileHeader>(headerSpan);

      if (!header.ValidateCapturePattern())
        throw new InvalidDataException("Not a continuation of an OggS container.");

      if (header.PageSequenceNumber != _parsedPageNum)
        throw new InvalidDataException($"Expected page sequence number {_parsedPageNum}.");

      if (_parsedPageNum == 0 && header.HeaderType != OggPageType.BeginningOfStream)
        throw new InvalidDataException("Expected beginning of stream.");

      ++_parsedPageNum;

      var pageSegmentSizesSize = header.PageSegments;

      // read segment sizing info

      {
        var buf = ArrayPool<byte>.Shared.Rent(pageSegmentSizesSize);
        var span = new Span<byte>(buf, 0, pageSegmentSizesSize);

        var read = _stream.Read(span);

        while (read != span.Length)
          read = _stream.Read(span = span.Slice(read));

        span = new Span<byte>(buf, 0, pageSegmentSizesSize);

        ReadSegmentSizes(span, segmentSize => {
          var size = (int) segmentSize;
          var seg = ArrayPool<byte>.Shared.Rent(size);
          var segSpan = new Span<byte>(seg, 0, size);
          read = _stream.Read(segSpan);
          while (read != segSpan.Length)
            read = _stream.Read(segSpan = segSpan.Slice(read));

          segSpan = new Span<byte>(seg, 0, size);
          var readFirstPacket = false;
          {
            var tag = MemoryMarshal.Read<int>(segSpan);
            if (tag == OpusFile.OpusTag) {
              segSpan = segSpan.Slice(4);
              tag = MemoryMarshal.Read<int>(segSpan);
              switch (tag) {
                case OpusFile.HeadTag when _headRead:
                  throw new NotImplementedException("Duplicate head tag in Ogg container.");
                case OpusFile.HeadTag: {
                  segSpan = segSpan.Slice(4);
                  ref readonly var head = ref CastAsRefReadOnly<byte, OpusHead>(segSpan);
                  segSpan = segSpan.Slice(sizeof(OpusHead));
                  if (head.MappingFamily != OpusChannelMappingFamily.SingleStream) {
                    //ref readonly var headMs = ref MemoryMarshal.AsRef<OpusHeadMultistream>(segment);
                    //segment = segment.Slice(sizeof(OpusHeadMultistream));
                    // mappings ...
                    throw new NotImplementedException("Multistream support not implemented.");
                  }

                  if (head.Version != 1)
                    throw new NotSupportedException("Only Opus file version 1 is supported at this time.");

                  if (segSpan.Length > 0)
                    throw new NotImplementedException("Extraneous data exists after OpusHead segment.");

                  //Console.WriteLine($"Channels: {head.ChannelCount}");
                  //Console.WriteLine($"Sample Rate: {head.InputSampleRate / 1000.0}kHz");

                  //headCopy = head;
                  _channelCount = head.ChannelCount;

                  _headRead = true;
                  break;
                }
                case OpusFile.TagsTag when !_headRead:
                  throw new NotImplementedException("Tags tag came before head.");
                case OpusFile.TagsTag when _tagsRead:
                  throw new NotImplementedException("Duplicate tags tag in Ogg container.");
                case OpusFile.TagsTag: {
                  segSpan = segSpan.Slice(4);
                  var strLen = MemoryMarshal.Read<int>(segSpan);
                  if (strLen < 0)
                    throw new InvalidDataException("OpusTags library version string way too large or corrupt.");

                  segSpan = segSpan.Slice(4);
                  var str = OpusFile.ReadString(segSpan, strLen);
                  if (!str.StartsWith("libopus 1."))
                    throw new NotImplementedException($"Support for {str} not implemented.");

                  segSpan = segSpan.Slice(strLen);
                  if (segSpan.Length == 0)
                    throw new NotImplementedException("Missing tag count?");

                  var tagCount = MemoryMarshal.Read<int>(segSpan);
                  segSpan = segSpan.Slice(4);
                  for (var tagIndex = 0; tagIndex < tagCount; ++tagIndex) {
                    strLen = MemoryMarshal.Read<int>(segSpan);
                    if (strLen < 0)
                      throw new InvalidDataException($"OpusTags tag {tagIndex} way too large or corrupt.");
                    if (strLen == 0)
                      throw new InvalidDataException("Null tag in OpusTags data.");

                    segSpan = segSpan.Slice(4);
                    str = OpusFile.ReadString(segSpan, strLen);
#if NETSTANDARD2_1 || NET
                    var parts = str.Split('=', 2);
#else
                    var parts = str.Split(['='], 2);
#endif
                    switch (parts.Length) {
                      case 2:
                        _tags.Add(parts[0], parts[1]);
                        break;
                      case 1:
                        _tags.Add(parts[0], null);
                        break;
                      default: throw new InvalidDataException($"Unknown tag structure: {str}");
                    }

                    segSpan = segSpan.Slice(strLen);
                  }

                  if (segSpan.Length > 0) {
                    var commentsCount = MemoryMarshal.Read<int>(segSpan);
                    segSpan = segSpan.Slice(4);
                    for (var commentIndex = 0; commentIndex < commentsCount; ++commentIndex) {
                      strLen = MemoryMarshal.Read<int>(segSpan);
                      if (strLen < 0)
                        throw new InvalidDataException($"OpusTags comment {commentIndex} way too large or corrupt.");
                      if (strLen == 0)
                        throw new InvalidDataException("Null comment in OpusTags data.");

                      segSpan = segSpan.Slice(4);
                      str = OpusFile.ReadString(segSpan, strLen);
                      _comments.AddLast(str);

                      segSpan = segSpan.Slice(strLen);
                    }
                  }

                  if (segSpan.Length > 0) {
                    for (var i = 0; i < segSpan.Length; ++i) {
                      if (segSpan[i] != 0)
                        throw new NotImplementedException($"Unhandled data in OpusTag page, segment offset {i}: 0x{segSpan[i]:X2}");
                    }
                  }

                  _tagsRead = true;
                  _headerComplete = true;

                  break;
                }
                default:
                  throw new NotImplementedException("Unknown Opus header tag.");
              }
            }
            else {
              _headerComplete = true;
              _packets.Add(new ArraySegment<byte>(seg, 0, size));
              // must be start of packets
              readFirstPacket = true;
            }
          }

          if (!readFirstPacket)
            ArrayPool<byte>.Shared.Return(seg);
        });

        ArrayPool<byte>.Shared.Return(buf);
      }
    } while (!_headerComplete);

    ArrayPool<byte>.Shared.Return(headerBuf);
  }

  private unsafe bool ReadPacket() {
      if (_disposed) throw new ObjectDisposedException(nameof(OpusStreamedFile));
      if (!_headerComplete) throw new InvalidOperationException("Header must be read first.");

      var headerSize = sizeof(OggFileHeader);

      Span<byte> headerSpan = stackalloc byte[headerSize];

      {
        var span = headerSpan;
        var read = _stream.Read(span);
        while (read != span.Length)
          read = _stream.Read(span = span.Slice(read));
      }

      ref readonly var header = ref CastAsRefReadOnly<byte, OggFileHeader>(headerSpan);

      if (!header.ValidateCapturePattern())
        throw new InvalidDataException("Not a continuation of an OggS container.");

      var headerType = header.HeaderType;

      if (header.PageSequenceNumber != _parsedPageNum)
        throw new InvalidDataException($"Expected page sequence number {_parsedPageNum}.");

      ++_parsedPageNum;

      var pageSegmentSizesSize = header.PageSegments;

      // read segment sizing info

      {
        var buf = ArrayPool<byte>.Shared.Rent(pageSegmentSizesSize);
        var span = new Span<byte>(buf, 0, pageSegmentSizesSize);

        var read = _stream.Read(span);

        while (read != span.Length)
          read = _stream.Read(span = span.Slice(read));

        span = new Span<byte>(buf, 0, pageSegmentSizesSize);

        ReadSegmentSizes(span, segmentSize => {
          var size = (int) segmentSize;
          var segBuf = ArrayPool<byte>.Shared.Rent(size);
          var segSpan = new Span<byte>(segBuf, 0, size);
          read = _stream.Read(segSpan);
          _packets.Add(new ArraySegment<byte>(segBuf, 0, size));
        });

        if ((headerType & OggPageType.EndOfStream) == 0)
          return true;

        _packets.Capacity = _packets.Count;

        _readAllPackets = true;

        return false;
      }
    }

  private ArraySegment<byte> GetPacket(int i) {
      if (_disposed) throw new ObjectDisposedException(nameof(OpusStreamedFile));

      if (!_headerComplete)
        ReadHeader();

      if (i < _packets.Count)
        return _packets[i];

      if (_readAllPackets)
        return default;

      while (i >= _packets.Count) {
        if (!ReadPacket())
          return default;
      }

      return i < _packets.Count ? _packets[i] : default;
    }

  /// <summary>
  /// Pre-loads and pre-parses packets within the mapping.
  /// </summary>
  /// <param name="i">
  /// The number of packets to ensure preload up to.
  /// Values of <c>-1</c> or less indicate to preload of all packets.
  /// </param>
  public void Preload(int i = -1) {
      if (i >= 0) {
        while (i >= _packets.Count) {
          if (!ReadPacket())
            return;
        }

        return;
      }

      while (!_readAllPackets) {
        if (!ReadPacket())
          return;
      }
    }

  public void Dispose() {
      if (_disposed) return;

      _disposed = true;

      if (_owned)
        _stream.Dispose();

      var arrayPool = ArrayPool<byte>.Shared;
      foreach (var packet in _packets)
        arrayPool.Return(packet.Array!);

      _packets.Clear();
    }

  public IEnumerator<ArraySegment<byte>> GetEnumerator()
    => new Enumerator(this);

  IEnumerator IEnumerable.GetEnumerator()
    => GetEnumerator();

}
