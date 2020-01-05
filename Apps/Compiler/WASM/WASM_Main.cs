using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrickyUnits;

namespace WASM {
    class WASM_Main {

        static bool Silence = false;
        static int Warnings = 0;
        public static void WARN(string w) {
            Console.Beep();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("WARNING! ");
            Console.ResetColor();
            Console.WriteLine(w);
            Warnings++;
        }

        public static void CRASH(string w) {
            Console.Beep();
            Console.Beep();
            Console.Beep();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("FATAL ERROR! ");
            Console.ResetColor();
            Console.WriteLine(w);
            Environment.Exit(10);
        }

        public static void CRASH(Exception w) {
#if DEBUG
            CRASH($".NET ERROR: {w.Message}\n\n{w.StackTrace}");
#else
            CRASH($".NET ERROR: {w.Message}");
#endif
        }

        public static void VP(string m) {
            if (!Silence) Console.WriteLine(m);
        }

        static void Main(string[] args) {
            Console.WriteLine($"Warnings {Warnings}");
            TrickyDebug.AttachWait();
        }
    }
}
