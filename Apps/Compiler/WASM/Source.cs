using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TrickyUnits;

namespace WASM {
    class Source {

        static List<string> IncPath = new List<string>();

        internal class Line {
            StringBuilder Code;
            internal string From_File;
            internal int Line_Number;

            override public string ToString() => Code.ToString();
            internal Line(string s, string f, int i) {
                Code = new StringBuilder(s.Trim());
                From_File = f;
                Line_Number = i;
            }


        }
        Line[] Lines;
        internal Source(string file) {
            if (!File.Exists(file)) WASM_Main.CRASH($"File '{file}' has not been found!");
            var fpath = qstr.ExtractDir(file).Replace("\\","/");
            try {
                var src = new List<Line>();
                var rawsrc = QuickStream.LoadString(file);
                if (rawsrc.IndexOf('\n') >= 0) { WASM_Main.WARN("<CR> characters found in source! Please deliver source in <LF> only format!"); rawsrc = rawsrc.Replace("\n", ""); }
                var srcl = rawsrc.Split('\n');
                for (int i = 0;  i < srcl.Length; i++) {
                    srcl[i] = srcl[i].Trim();
                    if (qstr.Left(srcl[i].ToUpper(),8)=="INCLUDE ") {
                            var f = srcl[i].Substring(8).Trim().Replace('\\','/');
                        string ult="";
                        if (IncPath.Count==0) {
                            if (fpath != "") IncPath.Add(fpath); else IncPath.Add(".");                            
                        }

                        if (f[0] == '/' || f[1] == ':')
                            ult = f;
                        else {
                            foreach(string p in IncPath) {
                                var test = $"{p}/{f}";
                                if (File.Exists(test)) { ult = test; break; }
                            }
                            if (ult == "") WASM_Main.CRASH($"No path provided a way to find requested include file {f}");
                        }
                        WASM_Main.VP($"Including: {ult}");
                        var Sub = new Source(ult);
                        foreach (Line L in Sub.Lines) src.Add(L);
                    } else if (qstr.Prefixed(srcl[i],";")) {
                        System.Diagnostics.Debug.WriteLine($"Skipping comment: {srcl[i]}");
                    } else {
                        src.Add(new Line(srcl[i], file, i + 1);
                    }
                }
                Lines = src.ToArray();
            } catch (Exception e) {
                WASM_Main.CRASH(e);
            }
        }

    }
}
