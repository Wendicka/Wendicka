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
// Version: 20.02.29
// EndLic

using System;
using System.Collections.Generic;
using System.Linq;
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
        private static void Set(string i, Instruction d) { Instructions[i.ToUpper()] = d; }
        internal delegate bool CheckerFunction(Source.Line l);
        internal CheckerFunction Check { get; private set; } = AlwaysRight;
        readonly int insnum;
        static bool AlwaysRight(Source.Line L) => true; // Only used when nothing's set

        private Instruction(int i,CheckerFunction chk = null            ) {
            if (chk != null) Check = chk;
            insnum = i;
        }

        internal static void Init() {
            MKL.Lic    ("Wendicka Project - Instruction.cs","GNU General Public License 3");
            MKL.Version("Wendicka Project - Instruction.cs","20.02.29");
        }

    }
}