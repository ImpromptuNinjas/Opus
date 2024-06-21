global using System;
global using System.Buffers;
global using System.Diagnostics;
global using System.IO;
global using System.Text;
global using System.Collections;
global using System.Collections.Generic;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Threading;
global using JetBrains.Annotations;

#if !NETSTANDARD1_1
global using System.IO.MemoryMappedFiles;
#endif

#if !NETSTANDARD2_0_OR_GREATER && !NET
global using InlineIL;
global using static InlineIL.IL;
global using static InlineIL.IL.Emit;
#endif
