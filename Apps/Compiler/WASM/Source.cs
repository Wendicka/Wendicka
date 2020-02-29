// Lic:
// Wendicka
// Source Loader
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
using System.IO;
using System.Text;
using TrickyUnits;

namespace WASM {
    class Source {

        static internal void Hello() {
            MKL.Lic    ("Wendicka Project - Source.cs","GNU General Public License 3");
            MKL.Version("Wendicka Project - Source.cs","20.02.29");
        }

        static List<string> IncPath = new List<string>();

        internal class Line {
            StringBuilder Code;
            internal string From_File;
            internal int Line_Number;

            override public string ToString() => Code.ToString();
            internal string Instruction { get {
                    var c = Code.ToString();
                    var p = c.IndexOf(' ');
                    if (p == -1) return c;
                    return c.Substring(0, p).ToUpper();
                }
            }
            internal string ParamString {
                get {
                    var c = Code.ToString();
                    var p = c.IndexOf(' ');
                    if (p == -1) return "";
                    return c.Substring(p+1);
                }
            }
            internal Line(string s, string f, int i) {
                Code = new StringBuilder(s.Trim());
                From_File = f;
                Line_Number = i;
            }


        }
        internal int CountLines => Lines.Length;
        internal Line this[int ln] => Lines[ln];

        internal Line[] Lines;
        internal Source(string file) {
            if (!File.Exists(file)) WASM_Main.CRASH($"File '{file}' has not been found!");
            var fpath = qstr.ExtractDir(file).Replace("\\", "/");
            try {
                var src = new List<Line>();
                var rawsrc = QuickStream.LoadString(file);
                if (rawsrc.IndexOf('\r') >= 0) { WASM_Main.WARN("<CR> characters found in source! Please deliver source in <LF> only format!"); rawsrc = rawsrc.Replace("\r", ""); }
                var srcl = rawsrc.Split('\n');
                for (int i = 0; i < srcl.Length; i++) {
                    srcl[i] = srcl[i].Trim();
                    if (qstr.Left(srcl[i].ToUpper(), 8) == "INCLUDE ") {
                        var f = srcl[i].Substring(8).Trim().Replace('\\', '/');
                        string ult = "";
                        if (IncPath.Count == 0) {
                            if (fpath != "") IncPath.Add(fpath); else IncPath.Add(".");
                        }

                        if (f[0] == '/' || f[1] == ':')
                            ult = f;
                        else {
                            foreach (string p in IncPath) {
                                var test = $"{p}/{f}";
                                if (File.Exists(test)) { ult = test; break; }
                            }
                            if (ult == "") WASM_Main.CRASH($"No path provided a way to find requested include file {f}");
                        }
                        WASM_Main.VP($" Including: {ult}");
                        var Sub = new Source(ult);
                        foreach (Line L in Sub.Lines) src.Add(L);
                    } else if (qstr.Prefixed(srcl[i], ";")) {
                        System.Diagnostics.Debug.WriteLine($"Skipping comment: {srcl[i]}");
                    } else if (srcl[i]=="") {
                        System.Diagnostics.Debug.WriteLine($"Skipping whiteline {i+1}");
                    } else {
                        src.Add(new Line(srcl[i], file, i + 1));
                    }
                }
                Lines = src.ToArray();
            } catch (Exception e) {
                WASM_Main.CRASH(e);
            }
        }

    }
}