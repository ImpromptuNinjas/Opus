namespace ImpromptuNinjas.Opus;

[PublicAPI]
public readonly struct OpusPacketSpan {

  public readonly IntPtr Pointer;

  public readonly int Length;

  public OpusPacketSpan(in IntPtr pointer, in int length) {
      Pointer = pointer;
      Length = length;
    }

  public static unsafe implicit operator ReadOnlySpan<byte>(OpusPacketSpan span)
    => new((byte*) span.Pointer, span.Length);

  public void Deconstruct(out IntPtr intPtr, out int length) {
      intPtr = Pointer;
      length = Length;
    }

}
