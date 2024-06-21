namespace ImpromptuNinjas.Opus;

[PublicAPI]
public enum OpusStatusCode {

  AllocFail = -7,

  InvalidState = -6,

  Unimplemented = -5,

  InvalidPacket = -4,

  InternalError = -3,

  BufferTooSmall = -2,

  BadArg = -1,

  Ok = 0,

}
