using System.Diagnostics;
using System.Threading;

namespace ImpromptuNinjas.Opus;

internal static partial class Native {

  private const string LibName = "libopus";

  internal static string? LibPath;

  private static readonly unsafe Lazy<IntPtr> LazyLoadedLib = new(() => {
    var asm = typeof(Native).GetAssembly();
    var baseDir = asm.GetLocalCodeBaseDirectory();

    var ptrBits = sizeof(void*) * 8;

    if (ptrBits == 32)
      throw new NotImplementedException("Support for 32-bit platforms is not implemented.");

    // ReSharper disable once RedundantAssignment
    IntPtr lib = default;

#if NETFRAMEWORK
      LibPath = Path.Combine(baseDir, "libopus.dll");
#else
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
      LibPath = Path.Combine(baseDir, "libopus.dll");
      if (!TryLoad(LibPath, out lib))
        LibPath = Path.Combine(baseDir, "runtimes", "win-x64", "native", "libopus.dll");
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
      LibPath = Path.Combine(baseDir, "libopus.dylib");
      if (!TryLoad(LibPath, out lib))
        LibPath = Path.Combine(baseDir, "runtimes", "osx-x64", "native", "libopus.dylib");
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
      LibPath = Path.Combine(baseDir, "libopus.so");
      if (!TryLoad(LibPath, out lib))
        LibPath = Path.Combine(baseDir, "runtimes", $"{(IsMusl() ? "linux-musl-" : "linux-")}{GetProcArchString()}", "native", "libopus.so");
    }
    else throw new PlatformNotSupportedException();
#endif

    // ReSharper disable once InvertIf
    if (lib == default && !TryLoad(LibPath, out lib)) {
#if !NETSTANDARD1_1
      if (File.Exists(LibPath))
        throw new UnauthorizedAccessException(LibPath);
#endif
#if !NETFRAMEWORK
      throw new DllNotFoundException(LibPath);
#else
        throw new FileNotFoundException(LibPath + "\n" +
          $"You may need to specify <RuntimeIdentifier>win-x64<RuntimeIdentifier> or <RuntimeIdentifier>win<RuntimeIdentifier> in your project file.",
          LibPath);
#endif
    }

    return lib;
  }, LazyThreadSafetyMode.ExecutionAndPublication);

  private static bool TryLoad(string libPath, out IntPtr lib) {
      try {
        lib = NativeLibrary.Load(libPath);
      }
      catch {
        lib = default;
        return false;
      }

      return true;
    }

  public static IntPtr Lib {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => LazyLoadedLib.Value;
  }

  static Native()
    => NativeLibrary.SetDllImportResolver(typeof(Native).GetAssembly(),
      (name, _, _)
        => {
        if (name != LibName)
          return default;

        Debug.Assert(Lib != default);
        return Lib;
      });

  internal static void Init()
    => Debug.Assert(Lib != default);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static unsafe string? GetString(sbyte* pStr) {
#if !NETSTANDARD1_1 && !NETSTANDARD1_4
    return pStr is null ? null : new string(pStr);
#else
      return Marshal.PtrToStringAnsi((IntPtr) pStr);
#endif
  }

}
