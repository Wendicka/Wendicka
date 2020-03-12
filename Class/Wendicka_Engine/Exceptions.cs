using System;
namespace Wendicka_Engine {

    class ExWendickaBinTooShort : Exception { public ExWendickaBinTooShort() : base("Buffer too short to be a Wendicka binary") { Console.Beep(); } }
    class ExWendickaNotRecognized : Exception { public ExWendickaNotRecognized() : base("Binary not recognized as Wendicka binary"){} }
    class ExWendickaFalseBineryOffset : Exception { public ExWendickaFalseBineryOffset(int o) : base($"False bineray offset ({o})!"){} }
}