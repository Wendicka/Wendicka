// Lic:
// Instructions for Wendicka
// A quick way for the compiler to find all its instructions
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
// Version: 20.03.09
// EndLic

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TrickyUnits;

namespace WASM {
    class Instruction {

        readonly static Dictionary<string, Instruction> Instructions = new Dictionary<string, Instruction>();
        internal static Instruction Get(string i) {
            i = i.ToUpper();
            if (!Instructions.ContainsKey(i)) return null;
            return Instructions[i];
        }
        private static void Set(string i, Instruction d) { Instructions[i.ToUpper()] = d; Debug.WriteLine($"Registered instruction {d.insnum} as {i}"); }
        internal delegate bool CheckerFunction(Source.Line l);
        internal CheckerFunction Check { get; private set; } = AlwaysRight;
        readonly internal int insnum;
        static bool AlwaysRight(Source.Line L) => true; // Only used when nothing's set

        private Instruction(int i, CheckerFunction chk = null) {
            if (chk != null) Check = chk;
            insnum = i;
            Debug.WriteLine($"Created instruction #{i}");
        }

        internal static void Init() {
            MKL.Lic    ("Wendicka Project - Instruction.cs","GNU General Public License 3");
            MKL.Version("Wendicka Project - Instruction.cs","20.03.09");
            Set("end", new Instruction(0, delegate (Source.Line l) { return l.Parameters.Length == 0; }));
            Set("call", new Instruction(1, delegate (Source.Line l) {
                //var ret = true;
                var p = l.Parameters;
                if (p.Length==0) { WASM_Main.VP("\nNothing to call");return false; }
                System.Diagnostics.Debug.WriteLine($"Checking call! Total parameters {p.Length}");
                if (p[0].Kind != Source.DataKind.API && p[0].Kind != Source.DataKind.Chunk && p[0].Kind != Source.DataKind.GlobalVar && p[0].Kind != Source.DataKind.LocalVar) { WASM_Main.Error($"Uncallable call: {p[0].Kind}"); return false; }
                return true;
            }));
            Set("invoke", new Instruction(2, Get("call").Check));
            Set("defer", new Instruction(3, Get("call").Check));
            Set("resume", new Instruction(4, delegate { throw new Exception("resume after yield not yet implemented"); }));
            Set("return", new Instruction(5));
            Set("yield", new Instruction(6));
        }

    }
}