#if NETFRAMEWORK || NETSTANDARD

namespace ImpromptuNinjas.Opus {

  internal delegate IntPtr DllImportResolver(
    string libraryName,
    Assembly assembly,
    DllImportSearchPath? searchPath
  );

  internal abstract class NativeLibrary {

    private static readonly INativeLibraryLoader Loader
      = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        ? Kernel32.Instance
        : LibDl.Instance;

#if !NETSTANDARD1_1
    private static readonly ConditionalWeakTable<Assembly, LinkedList<DllImportResolver>> Resolvers = new();
#else
    private static readonly Dictionary<Assembly, LinkedList<DllImportResolver>> Resolvers = new();
#endif

    public static void SetDllImportResolver(Assembly assembly, DllImportResolver resolver) {
      lock (Resolvers)
        Resolvers.GetOrCreateValue(assembly).AddLast(resolver);
    }

    public static IntPtr GetExport(IntPtr handle, string name) {
      if (handle == default)
        throw new ArgumentNullException(nameof(handle));
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof(name));

      var export = Loader.GetExport(handle, name);
      if (export == default)
#if !NETSTANDARD1_4 && !NETSTANDARD1_1
        throw new EntryPointNotFoundException(name);
#else
        throw new TypeLoadException($"Entry point not found: {name}");
#endif

      return export;
    }

    public static IntPtr Load(string libraryName, Assembly assembly, DllImportSearchPath? searchPath = null) {
      if (string.IsNullOrEmpty(libraryName))
        throw new ArgumentNullException(nameof(libraryName));
      if (assembly == null)
        throw new ArgumentNullException(nameof(assembly));

      lock (Resolvers) {
        if (!Resolvers.TryGetValue(assembly, out var resolvers))
          return Load(libraryName);

        foreach (var resolver in resolvers) {
          var result = resolver(libraryName, assembly, searchPath);
          if (result != default)
            return result;
        }

        var loaded = Load(libraryName);
        if (loaded == default)
#if NETSTANDARD1_1
          throw new Exception(libraryName);
#else
          throw new InvalidProgramException(libraryName);
#endif

        return loaded;
      }
    }

    public static IntPtr Load(string libraryPath) {
      if (string.IsNullOrEmpty(libraryPath))
        throw new ArgumentNullException(nameof(libraryPath));

      var loaded = Loader.Load(libraryPath);
      if (loaded == default)
#if NETSTANDARD1_1
          throw new InvalidOperationException(libraryPath);
#else
        throw new InvalidProgramException(libraryPath);
#endif

      return loaded;
    }

    private interface INativeLibraryLoader {

      void Init();

      IntPtr Load(string libraryPath);

      IntPtr GetExport(IntPtr handle, string name);

    }

    private static class LibDl {

      static LibDl() {
        //Trace.TraceInformation($"LibDl initializing.");
        INativeLibraryLoader loader;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
          // delayed linkage means we can't check for dl2 safely
          loader = new LibDl1();
          loader.Init();
        }
        else {
          // not delayed linkage
          try {
            loader = new LibDl2();
            loader.Init();
          }
          catch {
            loader = new LibDl1();
            loader.Init();
          }
        }

        Instance = loader;
      }

      internal static readonly INativeLibraryLoader Instance;

    }

    private sealed class LibDl1 : INativeLibraryLoader {

      // ReSharper disable once MemberHidesStaticFromOuterClass
      private const string LibName = "dl"; // can be libdl.so or libdl.dylib

      [DllImport(LibName, EntryPoint = "dlopen")]
      // ReSharper disable once MemberHidesStaticFromOuterClass
      internal static extern IntPtr Load(string fileName, int flags);

      [DllImport(LibName, EntryPoint = "dlsym")]
      // ReSharper disable once MemberHidesStaticFromOuterClass
      private static extern IntPtr GetExport(IntPtr handle, string symbol);

      IntPtr INativeLibraryLoader.Load(string libraryPath)
        => Load(libraryPath, 0x0002 /*RTLD_NOW*/);

      IntPtr INativeLibraryLoader.GetExport(IntPtr handle, string name)
        => GetExport(handle, name);

      public void Init() {
      }

    }

    private sealed class LibDl2 : INativeLibraryLoader {

      // ReSharper disable once MemberHidesStaticFromOuterClass
      private const string LibName = "dl.so.2";

      [DllImport(LibName, EntryPoint = "dlopen")]
      // ReSharper disable once MemberHidesStaticFromOuterClass
      internal static extern IntPtr Load(string fileName, int flags);

      [DllImport(LibName, EntryPoint = "dlsym")]
      // ReSharper disable once MemberHidesStaticFromOuterClass
      private static extern IntPtr GetExport(IntPtr handle, string symbol);

      IntPtr INativeLibraryLoader.Load(string libraryPath)
        => Load(libraryPath, 0x0002 /*RTLD_NOW*/);

      IntPtr INativeLibraryLoader.GetExport(IntPtr handle, string name)
        => GetExport(handle, name);

      public void Init() {
      }

    }

    private sealed class Kernel32 : INativeLibraryLoader {

      // ReSharper disable once MemberHidesStaticFromOuterClass
      private const string LibName = "kernel32";

      private Kernel32() {
      }

      internal static readonly INativeLibraryLoader Instance = new Kernel32();

      [DllImport(LibName, EntryPoint = "LoadLibrary", SetLastError = true)]
      // ReSharper disable once MemberHidesStaticFromOuterClass
      private static extern IntPtr Load(string lpFileName);

      [DllImport(LibName, EntryPoint = "GetProcAddress")]
      // ReSharper disable once MemberHidesStaticFromOuterClass
      private static extern IntPtr GetExport(IntPtr handle, string procedureName);

      IntPtr INativeLibraryLoader.Load(string libraryPath)
        => Load(libraryPath);

      IntPtr INativeLibraryLoader.GetExport(IntPtr handle, string name)
        => GetExport(handle, name);

      public void Init() {
      }

    }

  }

}

#endif
