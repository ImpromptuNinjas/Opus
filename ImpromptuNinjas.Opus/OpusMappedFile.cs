namespace ImpromptuNinjas.Opus;

[PublicAPI]
public sealed partial class OpusMappedFile : IEnumerable<OpusPacketSpan>, IDisposable {

#if !NETSTANDARD1_1
  private readonly MemoryMappedFile? _mappedFile;

  private readonly MemoryMappedViewAccessor? _view;
#endif

  private readonly Dictionary<string, string?> _tags = new();

  private readonly LinkedList<string> _comments = [];

  private readonly List<OpusPacketSpan> _packets = [];

  private int _channelCount;

  private readonly unsafe byte* _pView;

  private unsafe byte* _pCursor;

  private readonly ulong _mappingSize;

  private int _headerRead;

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
  public unsafe OpusMappedFile(string filePath) {
      _mappedFile = MemoryMappedFile.CreateFromFile(filePath);
      //Console.WriteLine(Path.GetFileNameWithoutExtension(filePath));

      _view = _mappedFile.CreateViewAccessor(0, 0);

      _pView = null;

      _view.SafeMemoryMappedViewHandle.AcquirePointer(ref _pView);

      _pCursor = null;

      _mappingSize = _view.SafeMemoryMappedViewHandle.ByteLength;

      _headerRead = 0;

      _readAllPackets = false;
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
  /// <param name="pointer">Pointer to an Opus file mapped in memory.</param>
  /// <param name="mappingSize">The size of the mapping.</param>
  public unsafe OpusMappedFile(byte* pointer, ulong mappingSize) {
#if !NETSTANDARD1_1
    _mappedFile = null;
    _view = null;
#endif

    _pView = pointer;

    _pCursor = null;

    _mappingSize = mappingSize;

    _headerRead = 0;

    _readAllPackets = false;
  }

  public int ChannelCount {
    get {
        if (_headerRead == 0)
          ReadHeader();
        return _channelCount;
      }
  }

  /// <summary>
  /// Provides a mechanism to read tags.
  /// </summary>
  public IReadOnlyDictionary<string, string?> Tags {
    get {
        if (_headerRead == 0)
          ReadHeader();
        return _tags;
      }
  }

  /// <summary>
  /// Provides a mechanism to read comments.
  /// </summary>
  public IEnumerable<string> Comments {
    get {
        if (_headerRead == 0)
          ReadHeader();
        return _comments;
      }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private ref readonly T2 CastAsRefReadOnly<T1, T2>(ReadOnlySpan<T1> span)
    => ref Unsafe.As<T1, T2>(ref Unsafe.AsRef(span.GetPinnableReference()));

  private unsafe void ReadHeader() {
    if (_disposed) throw new ObjectDisposedException(nameof(OpusMappedFile));

    var viewSpan = new ReadOnlySpan<byte>(_pView, (int) _mappingSize);

    ref readonly var header = ref CastAsRefReadOnly<byte, OggFileHeader>(viewSpan);

    if (!header.ValidateCapturePattern())
      throw new InvalidDataException("Not a valid OggS container.");

    if (header.HeaderType != OggPageType.BeginningOfStream)
      throw new InvalidDataException("Expected beginning of stream.");

    var segmentCount = header.PageSegments;

    _parsedPageNum = 0;
    if (header.PageSequenceNumber != _parsedPageNum)
      throw new InvalidDataException($"Expected page sequence number {_parsedPageNum}.");

    ++_parsedPageNum;

    var segmentsSize = header.ReadSegments(segment => {
      if (MemoryMarshal.Read<int>(segment) == OpusFile.OpusTag) {
        segment = segment.Slice(4);
        switch (MemoryMarshal.Read<int>(segment)) {
          case OpusFile.HeadTag: {
            segment = segment.Slice(4);
            ref readonly var head = ref CastAsRefReadOnly<byte, OpusHead>(segment);
            segment = segment.Slice(sizeof(OpusHead));
            if (head.MappingFamily != OpusChannelMappingFamily.SingleStream) {
              //ref readonly var headMs = ref MemoryMarshal.AsRef<OpusHeadMultistream>(segment);
              //segment = segment.Slice(sizeof(OpusHeadMultistream));
              // mappings ...
              throw new NotImplementedException("Multistream support not implemented.");
            }

            if (head.Version != 1)
              throw new NotSupportedException("Only Opus file version 1 is supported at this time.");

            if (segment.Length > 0)
              throw new NotImplementedException("Extraneous data exists after OpusHead segment.");

            //Console.WriteLine($"Channels: {head.ChannelCount}");
            //Console.WriteLine($"Sample Rate: {head.InputSampleRate / 1000.0}kHz");

            //headCopy = head;
            _channelCount = head.ChannelCount;

            break;
          }
          case OpusFile.TagsTag: {
            throw new NotImplementedException("Tags tag came before head.");
          }
          default: {
            throw new NotImplementedException("Unknown tag in Ogg container.");
          }
        }
      }
      else {
        throw new InvalidDataException("Unknown tag in Ogg container.");
      }
    });

    var pageSize = sizeof(OggFileHeader) + segmentCount + (int) segmentsSize;
    {
      viewSpan = viewSpan.Slice(pageSize);

      ref readonly var nextHeader = ref Unsafe.As<byte, OggFileHeader>(ref Unsafe.AsRef(viewSpan.GetPinnableReference()));

      if (!nextHeader.ValidateCapturePattern())
        throw new InvalidDataException("Not a continuation of an OggS container.");

      var headerType = nextHeader.HeaderType;

      if (nextHeader.PageSequenceNumber != _parsedPageNum)
        throw new InvalidDataException($"Expected page sequence number {_parsedPageNum}.");

      ++_parsedPageNum;

      if ((headerType & OggPageType.Continuation) != 0)
        throw new InvalidDataException($"Unknown page type. (Type {headerType})");

      segmentCount = nextHeader.PageSegments;

      segmentsSize = nextHeader.ReadSegments(segment => {
        if (MemoryMarshal.Read<int>(segment) == OpusFile.OpusTag) {
          segment = segment.Slice(4);
          switch (MemoryMarshal.Read<int>(segment)) {
            case OpusFile.HeadTag: {
              throw new NotImplementedException("Duplicate head tag in Ogg container.");
            }
            case OpusFile.TagsTag: {
              segment = segment.Slice(4);
              var strLen = MemoryMarshal.Read<int>(segment);
              if (strLen < 0)
                throw new InvalidDataException("OpusTags library version string way too large or corrupt.");

              segment = segment.Slice(4);
              var str = OpusFile.ReadString(segment, strLen);
              if (!str.StartsWith("libopus 1."))
                throw new NotImplementedException($"Support for {str} not implemented.");

              segment = segment.Slice(strLen);
              if (segment.Length == 0)
                throw new NotImplementedException("Missing tag count?");

              var tagCount = MemoryMarshal.Read<int>(segment);
              segment = segment.Slice(4);
              for (var tagIndex = 0; tagIndex < tagCount; ++tagIndex) {
                strLen = MemoryMarshal.Read<int>(segment);
                if (strLen < 0)
                  throw new InvalidDataException($"OpusTags tag {tagIndex} way too large or corrupt.");
                if (strLen == 0)
                  throw new InvalidDataException("Null tag in OpusTags data.");

                segment = segment.Slice(4);
                str = OpusFile.ReadString(segment, strLen);
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

                segment = segment.Slice(strLen);
              }

              if (segment.Length > 0) {
                var commentsCount = MemoryMarshal.Read<int>(segment);
                segment = segment.Slice(4);
                for (var commentIndex = 0; commentIndex < commentsCount; ++commentIndex) {
                  strLen = MemoryMarshal.Read<int>(segment);
                  if (strLen < 0)
                    throw new InvalidDataException($"OpusTags comment {commentIndex} way too large or corrupt.");
                  if (strLen == 0)
                    throw new InvalidDataException("Null comment in OpusTags data.");

                  segment = segment.Slice(4);
                  str = OpusFile.ReadString(segment, strLen);
                  _comments.AddLast(str);

                  segment = segment.Slice(strLen);
                }
              }

              if (segment.Length > 0) {
                for (var i = 0; i < segment.Length; ++i) {
                  if (segment[i] != 0)
                    throw new NotImplementedException($"Unhandled data in OpusTag page, segment offset {i}: 0x{segment[i]:X2}");
                }
              }

              break;
            }
            default: {
              throw new NotImplementedException("Unknown tag in Ogg container.");
            }
          }
        }
        else {
          // must be an Opus packet
          var pointer = (IntPtr) Unsafe.AsPointer(ref Unsafe.AsRef(segment.GetPinnableReference()));
          _packets.Add(new OpusPacketSpan(pointer, segment.Length));
        }
      });

      pageSize = sizeof(OggFileHeader) + segmentCount + (int) segmentsSize;

      if ((headerType & OggPageType.EndOfStream) != 0)
        throw new InvalidDataException("End of stream earlier than expected.");
    }

    _headerRead = pageSize;

    viewSpan = viewSpan.Slice(pageSize);

    _pCursor = (byte*) Unsafe.AsPointer(ref Unsafe.AsRef(viewSpan.GetPinnableReference()));
  }

  private unsafe bool ReadPacket() {
      if (_disposed) throw new ObjectDisposedException(nameof(OpusMappedFile));
      if (_headerRead == 0) throw new InvalidOperationException("Header must be read first.");

      // start reading subsequent pages
      var viewSpan = new ReadOnlySpan<byte>(_pCursor, (int) (_mappingSize - (uint) _headerRead));
      ref readonly var nextHeader = ref CastAsRefReadOnly<byte, OggFileHeader>(viewSpan);

      if (!nextHeader.ValidateCapturePattern())
        throw new InvalidDataException("Not a continuation of an OggS container.");

      var headerType = nextHeader.HeaderType;

      if (nextHeader.PageSequenceNumber != _parsedPageNum)
        throw new InvalidDataException($"Expected page sequence number {_parsedPageNum}.");

      ++_parsedPageNum;

      var segmentCount = nextHeader.PageSegments;

      var segmentsSize = nextHeader.ReadSegments(segment => {
        var pointer = (IntPtr) Unsafe.AsPointer(ref Unsafe.AsRef(segment.GetPinnableReference()));
        _packets.Add(new OpusPacketSpan(pointer, segment.Length));
      });

      var pageSize = sizeof(OggFileHeader) + segmentCount + (int) segmentsSize;

      viewSpan = viewSpan.Slice(pageSize);

      _pCursor = (byte*) Unsafe.AsPointer(ref Unsafe.AsRef(viewSpan.GetPinnableReference()));

      if ((headerType & OggPageType.EndOfStream) == 0)
        return true;

      _packets.Capacity = _packets.Count;

      _readAllPackets = true;

      return false;
    }

  private OpusPacketSpan GetPacket(int i) {
      if (_disposed) throw new ObjectDisposedException(nameof(OpusMappedFile));

      if (_headerRead == 0)
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

  /// <inheritdoc/>
  public IEnumerator<OpusPacketSpan> GetEnumerator()
    => new Enumerator(this);

  IEnumerator IEnumerable.GetEnumerator()
    => GetEnumerator();

  /// <inheritdoc/>
  public void Dispose() {
    if (_disposed) return;

    _disposed = true;
#if !NETSTANDARD1_1
    if (_view == null) return;

    _view.SafeMemoryMappedViewHandle.ReleasePointer();
    _view.Dispose();
    _mappedFile?.Dispose();
#endif
  }

}
