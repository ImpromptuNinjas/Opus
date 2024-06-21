namespace ImpromptuNinjas.Opus;

public sealed partial class OpusMappedFile {

  /// <summary>
  /// An enumeration mechanism of <see cref="OpusPacketSpan"/>s that make up an <see cref="OpusMappedFile"/>.
  /// </summary>
  /// <inheritdoc/>
  public sealed class Enumerator : IEnumerator<OpusPacketSpan> {

    private OpusMappedFile? _file;

    private int _packetIndex = -1;

    internal Enumerator(OpusMappedFile file)
      => _file = file;

    /// <inheritdoc/>
    public bool MoveNext() {
        if (_file == null)
          throw new ObjectDisposedException(nameof(Enumerator));

        Current = _file.GetPacket(++_packetIndex);
        return Current.Pointer != default;
      }

    /// <inheritdoc/>
    public void Reset() {
        if (_file == null)
          throw new ObjectDisposedException(nameof(Enumerator));
        if (_file._disposed)
          throw new ObjectDisposedException(nameof(OpusMappedFile));

        _packetIndex = -1;
      }

    /// <inheritdoc/>
    public OpusPacketSpan Current { get; private set; }

    object? IEnumerator.Current => Current;

    /// <inheritdoc/>
    public void Dispose()
      => _file = null;

  }

}
