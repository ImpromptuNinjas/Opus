namespace ImpromptuNinjas.Opus;

[Flags]
public enum OggPageType : byte {

  /// <summary>
  /// This packet is not a continuation of a previous packet.
  /// </summary>
  Fresh = 0,

  /// <summary>
  /// The first packet on this page is a continuation of the previous packet in the logical bitstream.
  /// </summary>
  Continuation = 1 << 0,

  /// <summary>
  /// This page is the first page in the logical bitstream.
  /// This flag must be set on the first page of every logical bitstream, and must not be set on any other page.
  /// </summary>
  BeginningOfStream = 1 << 1,

  /// <summary>
  /// This page is the last page in the logical bitstream.
  /// This flag must be set on the final page of every logical bitstream, and must not be set on any other page.
  /// </summary>
  EndOfStream = 1 << 2

}