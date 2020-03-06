// Lic:
// Wendicka
// Main Class
// 
// 
// 
// (c) Jeroen P. Broks, 
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
// Please note that some references to data like pictures or audio, do not automatically
// fall under this licenses. Mostly this is noted in the respective files.
// 
// Version: 20.03.01
// EndLic
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrickyUnits;

namespace WASM {
    class WASM_Main {

        static bool Silence = false;
        static int Warnings = 0;
        static int Errors = 0;
        public static void WARN(string w) {
            Console.Beep();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("WARNING! ");
            Console.ResetColor();
            Console.WriteLine(w);
            Warnings++;
        }

        public static void Error(string w) {
            Console.Beep();
            Console.Beep();
            Console.Beep();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("ERROR! ");
            Console.ResetColor();
            Console.WriteLine(w);
            Errors++;
        }
        public static void Error(Exception w) {
#if DEBUG
            Error($"{w.Message}\n\n{w.StackTrace}");
#else
            Error($"{w.Message}");
#endif
        }

        public static void CRASH(string w) {
            Console.Beep();
            Console.Beep();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("FATAL ");
            Error(w);
            TrickyDebug.AttachWait();
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

        public static void VT(string m) {
            if (!Silence) Console.Write(m);
        }

        static void Main(string[] args) {
            Source.Hello();
            Compile.Hello();
            Dirry.InitAltDrives();
            Instruction.Init();
            MKL.Version("Wendicka Project - WASM_Main.cs","20.03.01");
            MKL.Lic    ("Wendicka Project - WASM_Main.cs","GNU General Public License 3");
            Console.WriteLine($"Wendicka Assembler {MKL.Newest} - (c) {MKL.CYear(2020)} Jeroen P. Broks\n");
            if (args.Length == 0) {
                Console.WriteLine("Usage: WASM [options] <File>");
                TrickyDebug.AttachWait();
                return;
            }
            var parse = new FlagParse(args);
            parse.CrBool("s", false);
            parse.Parse();
            Silence = parse.GetBool("s");
            foreach (string f in parse.Args) Compile.Go(Dirry.AD(f).Replace("\\","/"));
            Console.WriteLine($"\tErrors: {Errors}; Warnings: {Warnings}");
            TrickyDebug.AttachWait();
        }
    }
}