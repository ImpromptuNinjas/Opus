namespace ImpromptuNinjas.Opus;

[PublicAPI]
public static class LibOpus {

  #region Dynamic Library Import Table

  // ReSharper disable IdentifierTypo
  // ReSharper disable StringLiteralTypo
  // ReSharper disable InconsistentNaming

  // const char * opus_get_version_string(void);
#if !NETSTANDARD2_0_OR_GREATER && !NET
  private static readonly nint opus_get_version_string =
#else
    private static readonly unsafe delegate * unmanaged[Cdecl]<sbyte*> opus_get_version_string
      = (delegate * unmanaged[Cdecl]<sbyte*>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_get_version_string));

  // const char * opus_strerror(int error);
#if !NETSTANDARD2_0_OR_GREATER && !NET
  private static readonly nint opus_strerror =
#else
  private static readonly unsafe delegate * unmanaged[Cdecl]<OpusStatusCode, sbyte*> opus_strerror
    = (delegate * unmanaged[Cdecl]<OpusStatusCode, sbyte*>)
#endif
    NativeLibrary.GetExport(Native.Lib, nameof(opus_strerror));

  // ReSharper restore InconsistentNaming
  // ReSharper restore StringLiteralTypo
  // ReSharper restore IdentifierTypo

  #endregion

  /// <summary>
  /// Gets the libopus version string.
  /// </summary>
  /// <remarks>
  /// Applications may look for the substring "-fixed" in the version string to determine whether they have a fixed-point or floating-point build at runtime.
  /// </remarks>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static unsafe sbyte* GetVersionString() {
#if !NETSTANDARD2_0_OR_GREATER && !NET
    Push(opus_get_version_string);
    Tail();
    Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(sbyte*)));
    return ReturnPointer<sbyte>();
#else
    return opus_get_version_string();
#endif
  }

  private static unsafe Lazy<string?> _lazyVersionString = new(() => Native.GetString(GetVersionString()));

  public static string? VersionString => _lazyVersionString.Value;

  /// <summary>
  /// Converts an opus error code into a human readable string.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static unsafe sbyte* GetErrorString(OpusStatusCode error) {
#if !NETSTANDARD2_0_OR_GREATER && !NET
    Push(error);
    Push(opus_strerror);
    Tail();
    Calli(StandAloneMethodSig.UnmanagedMethod(CallingConvention.Cdecl, typeof(sbyte*),
      typeof(OpusStatusCode)));
    return ReturnPointer<sbyte>();
#else
    return opus_strerror(error);
#endif
  }

  private static Dictionary<OpusStatusCode, string> _statusDescriptions = new();

  public static unsafe string GetDescription(OpusStatusCode status) {
    if (status > 0) return "Not an error.";

    lock (_statusDescriptions) {
      if (_statusDescriptions.TryGetValue(status, out var str))
        return str;

      var pStr = GetErrorString(status);
      if (pStr == default)
        return "Unknown.";

      str = Native.GetString(pStr);
      if (str is null)
        return "Unknown.";

      _statusDescriptions.Add(status, str);
      return str;
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Exception? GetError(OpusStatusCode error)
    => error switch {
      OpusStatusCode.BadArg => new ArgumentException(GetDescription(error)),
      OpusStatusCode.BufferTooSmall => new ArgumentException(GetDescription(error)),
      OpusStatusCode.InternalError => new Exception(GetDescription(error)),
      OpusStatusCode.InvalidPacket => new ArgumentException(GetDescription(error)),
      OpusStatusCode.Unimplemented => new NotImplementedException(GetDescription(error)),
      OpusStatusCode.InvalidState => new InvalidOperationException(GetDescription(error)),
      OpusStatusCode.AllocFail => new OutOfMemoryException(GetDescription(error)),
      _ => error < 0 ? new NotImplementedException($"Unknown Error {(int) error}: {GetDescription(error)}") : null
    };

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void ThrowIfError(OpusStatusCode error) {
    var exc = GetError(error);
    if (exc != null) throw exc;
  }

}
