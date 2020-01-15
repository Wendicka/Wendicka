// Lic:
// Wendicka
// Compiler
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
// Version: 20.01.06
// EndLic

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrickyUnits;

namespace WASM {
    internal class Compile {

        static internal void Hello() {
            MKL.Lic    ("Wendicka Project - Compile.cs","GNU General Public License 3");
            MKL.Version("Wendicka Project - Compile.cs","20.01.06");
        }

        static void VP(string m) => WASM_Main.VP(m);

        static internal void Go(string srcfile) {
            VP($"Assembling: {srcfile}");
            var src = new Source(srcfile);
            var bo = QuickStream.WriteFile($"{qstr.StripExt(srcfile)}.WBIN");
            try {

            } catch (Exception e){
                WASM_Main.Error(e);
            } finally {
                bo.Close();
            }
        }
    }
}