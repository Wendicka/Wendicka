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
// Version: 20.03.09
// EndLic
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using TrickyUnits;

namespace WASM {
    class Source {

        static internal void Hello() {
            MKL.Lic    ("Wendicka Project - Source.cs","GNU General Public License 3");
            MKL.Version("Wendicka Project - Source.cs","20.03.09");
        }

        static List<string> IncPath = new List<string>();

        internal enum DataKind { Einde, StartChunk, Instruction, Index, Chunk, Label, GlobalVar, LocalVar, Reference, String, IntValue, FloatValue, Boolean, Null, API }

        internal class Parameter {
            internal DataKind Kind;
            internal byte KindByte => (byte)Kind;
            internal long intvalue {
                get {
                    throw new Exception("Upcoming feature! Int calculation!");
                }
            }
            internal StringBuilder StrValue = new StringBuilder(1);
        }

        internal class Line {
            StringBuilder Code;
            internal string From_File;
            internal int Line_Number;
            internal Parameter[] Parameters {
                get {
                    var ret = new List<Parameter>();
                    int pos = 0;
                    Parameter Par=null;
                    void Flush() { if (Par != null) ret.Add(Par); Par = null; }
                    var instring = false;
                    var escape = false;
                    while (pos < ParamString.Length) {
                        char cb = ParamString[pos];
                        if (Par == null) {
                            Par = new Parameter();
                            switch (cb) {
                                case '@':
                                    Par.Kind = DataKind.API;
                                    pos++;
                                    break;
                                case '"':
                                    Par.Kind = DataKind.String;
                                    instring = true;
                                    escape = false;
                                    pos++;
                                    break;
                                case '$':
                                    if (ParamString[pos + 1] == '$') {
                                        pos++;
                                        Par.Kind = DataKind.GlobalVar;
                                    } else { Par.Kind = DataKind.LocalVar; }
                                    break;
                                case '&':
                                    throw new Exception("No support YET for references");
                                case '0':
                                case '1':
                                case '2':
                                case '3':
                                case '4':
                                case '5':
                                case '6':
                                case '7':
                                case '8':
                                case '9':
                                    Par.Kind = DataKind.IntValue;
                                    pos++;
                                    break;
                                default:
                                    if (Instruction == "JMP" || Instruction == "JZ" || Instruction == "JNZ") Par.Kind = DataKind.Label; else Par.Kind = DataKind.Chunk;
                                    // NO 'pos++' this time! I'm serious!
                                    break;
                            }
                        } else if (instring) {
                            char b = '0';
                            if (escape) {
                                switch (cb) {
                                    case '\\':
                                    case '"':
                                        b = cb;
                                        pos++;
                                        break;
                                    case 'n':
                                        b = '\n';
                                        pos++;
                                        break;
                                    case 'r':
                                        b = '\r';
                                        pos++;
                                        break;
                                    case 't':
                                        b = '\t';
                                        pos++;
                                        break;
                                    case 'b':
                                        b = '\b';
                                        pos++;
                                        break;
                                    case '0':
                                    case '1':
                                    case '2':
                                    case '3':
                                    case '4':
                                    case '5':
                                    case '6':
                                    case '7':
                                    case '8':
                                    case '9':
                                        if (ParamString.Length < pos + 3) throw new Exception("Invalid number for escape");
                                        var s = $"{ParamString[pos + 0]}{ParamString[pos + 1]}{ParamString[pos + 2]}";
                                        var i = qstr.ToInt(s);
                                        if (i >= 256) throw new Exception("Too high ASCII");
                                        b = (char)(byte)i;
                                        pos += 3;
                                        break;
                                    default:
                                        throw new Exception("Invalid string escape code");
                                }
                            } else if (cb=='\\') {
                                escape = true;
                                pos++;
                                continue;
                            } else if (cb == '"') {
                                instring = false;
                                Debug.WriteLine($"End of string at {pos}");
                                pos++;
                            } else {
                                b = cb;
                                pos++;
                            }
                            if (instring && b > 0 ) Par.StrValue.Append(b);
                            escape = false;
                        } else if (cb == ',') {
                            Debug.WriteLine($"Param SPLIT! {pos}");
                            Flush();
                            pos++;
                        } else if (cb == '#') {
                            switch (Par.Kind) {
                                case DataKind.Einde:
                                case DataKind.Instruction:
                                    throw new Exception($"Invalid p-block: {Par.Kind}! (Internal error!)");
                                case DataKind.API:
                                case DataKind.Chunk:
                                    throw new Exception("Illegal index request!");
                            }
                            Flush();
                            Par = new Parameter();
                            Par.Kind = DataKind.Index;
                            Flush();
                        } else {
                            Debug.WriteLine($"Appending ot {Par.Kind} / {pos} / {ParamString.Length}");
                            switch (Par.Kind) {
                                case DataKind.API:
                                case DataKind.Chunk:
                                case DataKind.GlobalVar:
                                case DataKind.Label:
                                case DataKind.LocalVar:
                                    if (cb >= 'a' && cb <= 'z') cb = (char)((byte)cb - 32);
                                    var allow = (cb >= 'A' && cb <= 'Z') || cb == '_' || (cb >= '0' && cb <= '9' && Par.StrValue.Length > 0);
                                    if (!allow) throw new Exception($"Invalid name for {Par.Kind}");
                                    Par.StrValue.Append(cb);
                                    pos++;
                                    break;
                                case DataKind.FloatValue:
                                case DataKind.IntValue:
                                    throw new Exception("Numberic values not yet supported");
                                case DataKind.String:
                                    break;
                                default:
                                    throw new Exception($"Unknown DataKind: {Par.Kind}");
                            }
                        }
                    }
                    if (instring) throw new Exception("Unfinished string!");
                    Flush();
                    return ret.ToArray();
                }
            }

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