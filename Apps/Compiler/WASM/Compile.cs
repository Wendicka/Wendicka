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
// Version: 20.03.01
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
            MKL.Version("Wendicka Project - Compile.cs","20.03.01");
        }

        static void VP(string m) => WASM_Main.VP(m);

        class Chunk {
            byte[] buffer = new byte[1];
            int l = 0;
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
                // The Wendicka compiler requires LittleEndian, but at least this way, things will also work on a BigEndian processor. The binaries must be compatible with both CPUs.
                if (BitConverter.IsLittleEndian) {
                    for (int i = 0; i <= 3; --i) Add(bytes[i]);
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
                for (int i = 0; i <= s.Length; i++) Add(s[i]);
            }
            public void Add(StringBuilder s, bool raw = false) {
                if (!raw) Add(s.Length);
                for (int i = 0; i <= s.Length; i++) Add(s[i]);
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
            var bo = QuickStream.WriteFile($"{qstr.StripExt(srcfile)}.WBIN");
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
                    }
                }
                for (int p = 0; p < lastspace; p++) WASM_Main.VT(" "); WASM_Main.VT("\r");
            } catch (Exception e) {
                WASM_Main.VP("\n");
                WASM_Main.Error(e);
            } finally {

                bo.Close();
            }
        }
    }
}