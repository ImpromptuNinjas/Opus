namespace ImpromptuNinjas.Opus;

public sealed partial class OpusStreamedFile {

  /// <summary>
  /// An enumeration mechanism of <see cref="ArraySegment{T}"/>s of <see cref="Byte"/>s that make up an <see cref="OpusStreamedFile"/>.
  /// </summary>
  /// <inheritdoc/>
  public sealed class Enumerator : IEnumerator<ArraySegment<byte>> {

    private OpusStreamedFile? _file;

    private int _packetIndex = -1;

    internal Enumerator(OpusStreamedFile file)
      => _file = file;

    /// <inheritdoc/>
    public bool MoveNext() {
        if (_file == null)
          throw new ObjectDisposedException(nameof(Enumerator));

        Current = _file.GetPacket(++_packetIndex);
        return Current.Array != null;
      }

    /// <inheritdoc/>
    public void Reset() {
        if (_file == null)
          throw new ObjectDisposedException(nameof(Enumerator));
        if (_file._disposed)
          throw new ObjectDisposedException(nameof(OpusStreamedFile));

        _packetIndex = -1;
      }

    /// <inheritdoc/>
    public ArraySegment<byte> Current { get; private set; }

    object? IEnumerator.Current => Current;

    /// <inheritdoc/>
    public void Dispose()
      => _file = null;

  }

}
