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
// Version: 20.03.09
// EndLic

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TrickyUnits;

namespace WASM {
    internal class Compile {

        static internal void Hello() {
            MKL.Lic    ("Wendicka Project - Compile.cs","GNU General Public License 3");
            MKL.Version("Wendicka Project - Compile.cs","20.03.09");
        }

        static void VP(string m) => WASM_Main.VP(m);

        class Chunk {
            internal byte[] buffer { get; private set; } = new byte[1];
            int l = 0;
            internal SortedDictionary<string, int> Labels = new SortedDictionary<string, int>();
            internal SortedDictionary<int, string> RequestedLabels = new SortedDictionary<int, string>();
            internal int Length => l;
            public void Add(byte b) {
                if (l >= buffer.Length) {
                    if (buffer.Length >= 2000000000) throw new Exception("Chunk buffer overflow!");
                    byte[] nb;
                    if (buffer.Length >= 1000000000) nb = new byte[2000000000];
                    nb = new byte[buffer.Length * 2];
                    for (int i=0; i < l; i++) nb[i] = buffer[i];
                    buffer = nb;
                }
                buffer[l] = b;
                l++;
            }
            public void Add(bool b) {
                if (b) Add((byte)1); else Add((byte)0);
            }
            public void Add(int b) {
                byte[] bytes = BitConverter.GetBytes(b);
                if (bytes.Length!=4) { Console.Beep(1200,20); Debug.WriteLine($"int is not 4 bytes long it's {bytes.Length} in stead! How come?"); }
                // The Wendicka compiler requires LittleEndian, but at least this way, things will also work on a BigEndian processor. The binaries must be compatible with both CPUs.
                if (BitConverter.IsLittleEndian) {
                    for                         (int i = 0; i <= 3; ++i) Add(bytes[i]);
                } else {
                    for (int i = 3; i >= 0; --i) Add(bytes[i]);
                }
            }
            public void Add(long b) {
                byte[] bytes = BitConverter.GetBytes(b);
                // The Wendicka compiler requires LittleEndian, but at least this way, things will also work on a BigEndian processor. The binaries must be compatible with both CPUs.
                if (BitConverter.IsLittleEndian) {
                    for (int i = 0; i <= 7; --i) Add(bytes[i]);
                } else {
                    for (int i = 7; i >= 0; --i) Add(bytes[i]);
                }
            }
            public void Add(string s, bool raw = false) {
                if (!raw) Add(s.Length);
                for (int i = 0; i <= s.Length; i++) Add((byte)s[i]);
            }
            public void Add(StringBuilder s, bool raw = false) {
                if (!raw) Add(s.Length);
                for (int i = 0; i < s.Length; i++) Add((byte)s[i]);
            }

            public Chunk() {
                Add((byte)8); // I must be 100% sure the 'byte' variant was used and not the 'int' variant!
                Add((byte)8);
                // First byte indicates the hard-tags, used in this setup has 8 bit
                // Second byte indicates the actual instruction/data is prefixed with an 8 bit tag.
                // Doing this per chunk has been done to ensure backward compatibility if higher numbers are needed for this in the future (which I do not expect).
            }
        }
        
        static bool ValidID(string ID) {
            var ret = true;
            ID = ID.ToUpper();
            if (ID == "") return false;
            for(int i = 0; i < ID.Length; i++) {
                var c = ID[i];
                ret = ret && ((c >= 48 && c <= 57 && i != 0) || c == 95 || (c >= (byte)'A' && c <= (byte)'Z'));
            }
            return ret;
        }

        static internal void Go(string srcfile) {
            VP($"Assembling: {srcfile}");
            var src = new Source(srcfile);
            var ofile = $"{qstr.StripExt(srcfile)}.WBIN";
            var bo = QuickStream.WriteFile(ofile); bo.WriteString($"Wendicka Binary {(char)26}",true);
            var Chunks = new SortedDictionary<string, Chunk>();
            Chunk CurrentChunk = null;
            try {
                var lastspace = 0;
                foreach (Source.Line L in src.Lines) {
                    for (int p = 0; p < lastspace; p++) WASM_Main.VT(" "); WASM_Main.VT("\r");
                    var s = $"{L.From_File}:{L.Line_Number}";
                    lastspace = s.Length;
                    WASM_Main.VT($"{s}\r");
                    Debug.WriteLine(s);
                    var instr = Instruction.Get(L.Instruction);
                    if (L.Instruction == "CHUNK") {
                        if (!ValidID(L.ParamString)) throw new Exception($"Illegal chunk name: {L.ParamString}");
                        var CName = L.ParamString.ToUpper();
                        if (Chunks.ContainsKey(CName)) {
                            WASM_Main.VP("");
                            WASM_Main.WARN($"Duplicate chunk name {CName}! Code will be appended to existing chunk! Is that what you wanted?");
                            CurrentChunk = Chunks[CName];
                            Debug.WriteLine($"Appending to Chunk: {CName}");
                        } else {
                            Debug.WriteLine($"Creating Chunk: {CName}");
                            CurrentChunk = new Chunk();
                            Chunks[CName] = CurrentChunk;
                        }
                    } else if (L.Instruction.Length>0 && (L.Instruction[0]==':' || L.Instruction[L.Instruction.Length-1]==':')) {
                        var Lab = L.Instruction.Replace(":", "").ToUpper();
                        if (CurrentChunk == null) throw new Exception("Label requires chunk");
                        CurrentChunk.Labels[Lab] = CurrentChunk.Length;
                    } else if (instr == null) {
                        throw new Exception($"Unknown instruction! \"{L.Instruction}\"!");
                    } else if (!instr.Check(L)) {
                        throw new Exception($"Instruction {L.Instruction} does not provide the parameters wanted, or has otherwise bad input!");
                    } else if (CurrentChunk == null) {
                        throw new Exception("No chunk!");
                    } else {
                        // If all is good, let's do it!
                        Debug.WriteLine($"Writing instruction {L.Instruction}/{Instruction.Get(L.Instruction).insnum}");
                        CurrentChunk.Add((byte)1);
                        CurrentChunk.Add((byte)Instruction.Get(L.Instruction).insnum);
                        foreach(Source.Parameter p in L.Parameters) {
                            switch (p.Kind) {
                                case Source.DataKind.StartChunk:
                                case Source.DataKind.Einde:
                                case Source.DataKind.Instruction:
                                    throw new Exception($"Internal error! Illegal instruction parameter! {p.Kind}");
                                case Source.DataKind.Index:
                                    CurrentChunk.Add((byte)Source.DataKind.Index);
                                    break;
                                case Source.DataKind.Chunk:
                                case Source.DataKind.String:
                                case Source.DataKind.LocalVar:
                                case Source.DataKind.GlobalVar:
                                case Source.DataKind.API:
                                    CurrentChunk.Add((byte)p.Kind);
                                    CurrentChunk.Add(p.StrValue);
                                    break;
                                case Source.DataKind.Label:
                                    CurrentChunk.Add((byte)Source.DataKind.Label);
                                    CurrentChunk.RequestedLabels[CurrentChunk.Length] = p.StrValue.ToString();
                                    Debug.WriteLine($"Label {p.StrValue} requested at offset {CurrentChunk.Length}");
                                    CurrentChunk.Add((int)0);
                                    break;
                                case Source.DataKind.Reference:
                                    throw new Exception("No support for referrence (yet)");
                                case Source.DataKind.IntValue:
                                    CurrentChunk.Add((byte)Source.DataKind.IntValue);
                                    CurrentChunk.Add(p.intvalue);
                                    break;
                                case Source.DataKind.FloatValue:
                                    throw new Exception("No support for float yet");
                                case Source.DataKind.Boolean: {
                                        var c = p.StrValue.ToString().ToUpper() == "TRUE" || p.StrValue.ToString().ToUpper() == "YES";
                                        CurrentChunk.Add((byte)Source.DataKind.Boolean);
                                        CurrentChunk.Add((byte)p.intvalue);
                                        break;
                                    }
                                case Source.DataKind.Null:
                                    CurrentChunk.Add((byte)12);
                                    break;
                                default:
                                    throw new Exception($"Unknown datakind {p.Kind}! Source version conflict? Internal error?");
                            }
                        }
                        CurrentChunk.Add((byte)0);
                        Debug.WriteLine($"Instruction {L.Instruction} ended");
                    }
                }
                for (int p = 0; p < lastspace; p++) WASM_Main.VT(" "); WASM_Main.VT("\r");
                VP($"    Saving: {ofile}");
                foreach(string k in Chunks.Keys) {
                    var Chnk = Chunks[k];
                    Debug.WriteLine($"Chunk: {k}");
                    Debug.WriteLine("= Configuring Labels");
                    foreach(int ofs in Chnk.RequestedLabels.Keys ) {
                        var Lab = Chnk.RequestedLabels[ofs];
                        var o = BitConverter.GetBytes(Chnk.Labels[Lab]);
                        Debug.WriteLine($"  = {ofs} => {Lab}");
                        if (!BitConverter.IsLittleEndian) Array.Reverse(o);
                        for (byte i = 0; i < 4; ++i) Chnk.buffer[ofs + i] = o[i];
                    }
                    Debug.WriteLine("= Writing Chunk Buffer");
                    bo.WriteByte((byte)Source.DataKind.StartChunk);
                    bo.WriteByte(1); bo.WriteString(k);
                    bo.WriteByte(2); bo.WriteInt(Chnk.buffer.Length);
                    bo.WriteByte(0); bo.WriteBytes(Chnk.buffer); bo.WriteByte(0);
                }
                bo.WriteString("WBIN", true);
                bo.WriteInt((int)bo.Size + 4);
                bo.WriteString("WBIN", true);
            } catch (Exception e) {
                WASM_Main.VP("\n");
                WASM_Main.Error(e);
            } finally {

                bo.Close();
            }
        }
    }
}