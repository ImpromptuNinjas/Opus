namespace ImpromptuNinjas.Opus;

[PublicAPI]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct OggFileHeader {

  /// <summary>
  /// The capture pattern or sync code is a magic number used to ensure synchronization when parsing Ogg files.
  /// Every page starts with the four ASCII character sequence, "OggS".
  /// This assists in resynchronizing a parser in cases where data has been lost or is corrupted, and is a sanity
  /// check before commencing parsing of the page structure.
  /// </summary>
  public int CapturePattern;

  /// <summary>
  /// This field indicates the version of the Ogg bitstream format, to allow for future expansion.
  /// It is currently mandated to be 0.
  /// </summary>
  public byte Version;

  /// <summary>
  /// Indicates the type of page that follows. See <see cref="OggPageType"/>.
  /// </summary>
  public OggPageType HeaderType;

  /// <summary>
  /// A granule position is the time marker in Ogg files. It is an abstract value, whose meaning is determined by the
  /// codec. It may, for example, be a count of the number of samples, the number of frames or a more complex scheme.
  /// </summary>
  public long GranulePosition;

  /// <summary>
  /// This field is a serial number that identifies a page as belonging to a particular logical bitstream. Each
  /// logical bitstream in a file has a unique value, and this field allows implementations to deliver the pages to
  /// the appropriate decoder. In a typical Vorbis and Theora file, one stream is the audio (Vorbis), and the other is
  /// the video. (Theora)
  /// </summary>
  public int BitstreamSerialNumber;

  /// <summary>
  /// This field is a monotonically increasing field for each logical bitstream. The first page is 0, the second 1,
  /// etc. This allows implementations to detect when data has been lost.
  /// </summary>
  public int PageSequenceNumber;

  /// <summary>
  /// This field provides a CRC32 checksum of the data in the entire page (including the page header, calculated with
  /// the checksum field set to 0). This allows verification that the data has not been corrupted since it was
  /// created. Pages that fail the checksum should be discarded. The checksum is generated using a polynomial value
  /// of 0x04C11DB7.
  /// </summary>
  public int Checksum;

  /// <summary>
  /// This field indicates the number of segments that exist in this page.
  /// It also indicates how many bytes are in the segment table that follows this field.
  /// There can be a maximum of 255 segments in any one page.
  /// </summary>
  public byte PageSegments;

}

/// <summary>
/// Ogg Opus bitstream information.
/// This contains the basic playback parameters for a stream, and corresponds to the initial ID header packet of an
/// Ogg Opus stream.
/// </summary>
[PublicAPI]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct OpusHead {

  /// <summary>
  /// The Ogg Opus format version.
  /// </summary>
  /// <remarks>
  /// The top 4 bits represent a "major" version, and the bottom 4 bits represent backwards-compatible "minor"
  /// revisions.
  /// </remarks>
  public byte Version;

  /// <summary>
  /// The number of channels.
  /// </summary>
  public byte ChannelCount;

  /// <summary>
  /// The number of samples that should be discarded from the beginning of the stream.
  /// </summary>
  public ushort PreSkip;

  /// <summary>
  /// The sampling rate of the original input.
  /// </summary>
  /// <remarks>
  /// All Opus audio is coded at 48 kHz, and should also be decoded at 48 kHz for playback unless the target
  /// hardware does not support this sampling rate.
  /// However, this field may be used to resample the audio back to the original sampling rate, for example, when
  /// saving the output to a file.
  /// </remarks>
  public uint InputSampleRate;

  /// <summary>
  /// The gain to apply to the decoded output, in dB, as a Q8 value in the range [-32768, 32767].
  /// </summary>
  /// <remarks>
  /// This library will NOT automatically apply this gain to the decoded output before returning it.
  /// The scaling is expected to be <c><see cref="System.Math.Pow"/>(10,<see cref="OutputGain"/>/(20.0*256))</c>.
  /// </remarks>
  public short OutputGain;

  /// <summary>
  /// The channel mapping family.
  /// </summary>
  /// <remarks>
  /// If this is not <see cref="OpusChannelMappingFamily.SingleStream"/>, <see cref="OpusHeadMultistream"/> follows
  /// immediately after.
  /// </remarks>
  /// <seealso cref="OpusHeadMultistream"/>
  public OpusChannelMappingFamily MappingFamily;

}

/// <summary>
///
/// </summary>
[PublicAPI]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct OpusHeadMultistream {

  /// <summary>
  /// The number of Opus streams in each Ogg packet
  /// </summary>
  public byte StreamCount;

  /// <summary>
  /// The number of coupled Opus streams in each Ogg packet.
  /// </summary>
  /// <remarks>
  /// Range is [0, 127].
  /// This must satisfy <c>0 &lt;= <see cref="CoupledCount"/> &lt;= <see cref="StreamCount"/></c>
  /// and <c><see cref="CoupledCount"/> + <see cref="StreamCount"/> &lt;= 255</c>.
  /// The coupled streams appear first, before all uncoupled streams, in an Ogg Opus packet.
  /// </remarks>
  public byte CoupledCount;

  /// <summary>
  /// The mapping from coded stream channels to output channels.
  /// </summary>
  /// <remarks>
  /// Let <c>i = <see cref="Mapping(System.Span{byte},byte)"/>(data,i)</c> be the value for channel <c>k</c>.
  /// If multistream and <c>i &lt; 2*<see cref="OpusHeadMultistream.CoupledCount"/></c>, then it refers to the left
  /// channel from stream <c>i / 2</c> if even, and the right channel from stream <c>i / 2</c> if odd.
  /// Otherwise, it refers to the output of the uncoupled stream
  /// <c>i - <see cref="OpusHeadMultistream.CoupledCount"/></c>.
  /// </remarks>
  public static ref byte Mapping(Span<byte> spanAfterHead, byte i) => ref spanAfterHead[i];

  /// <inheritdoc cref="Mapping(System.Span{byte},byte)"/>
  public static ref readonly byte Mapping(ReadOnlySpan<byte> spanAfterHead, byte i) => ref spanAfterHead[i];

}

/// <summary>
///
/// </summary>
/// <remarks>
/// Channel mapping family 0 covers mono or stereo in a single stream.
///
/// Channel mapping family 1 covers 1 to 8 channels in one or more streams using the Vorbis speaker assignments.
///
/// Channel mapping family 255 covers 1 to 255 channels in one or more streams but without any defined speaker
/// assignment.
/// </remarks>
public enum OpusChannelMappingFamily : byte {

  SingleStream = 0,

  MultiStream1 = 1,

  MultiStream2,

  MultiStream3,

  MultiStream4,

  MultiStream5,

  MultiStream6,

  MultiStream7,

  MultiStream8,

  MultistreamUnassigned = 255

}