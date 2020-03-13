using System;
namespace Wendicka_Engine {

    class ExWendickaBinTooShort : Exception { public ExWendickaBinTooShort() : base("Buffer too short to be a Wendicka binary") { Console.Beep(); } }
    class ExWendickaNotRecognized : Exception { public ExWendickaNotRecognized() : base("Binary not recognized as Wendicka binary"){} }
    class ExWendickaFalseBineryOffset : Exception { public ExWendickaFalseBineryOffset(int o) : base($"False bineray offset ({o})!"){} }
    class ExWendickaNotYetSupported :Exception { public ExWendickaNotYetSupported(string s) : base($"Unsupported feature found ({s})!\nPerhaps you need a later version?") { } }
    class ExWendickaChunkless:Exception { public ExWendickaChunkless(string s, long o) : base($"Chunkless data ({s}) at {o.ToString("X")}") { Console.Beep(); Console.Beep(); } }
    class ExWendickaIllegalChunkSize:Exception { public ExWendickaIllegalChunkSize(string chnkname, long start, long size, long end) : base($"Illagal Chunk Size/Offsets!\nStart:{start.ToString("X")}\nEnd:  {end.ToString("X")}\nSize: {size.ToString("X")}") { } }
}