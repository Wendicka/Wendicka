// Lic:
// Class/Wendicka_Engine/Chunks and stuff.cs
// Wendicka - Chunks 'n stuff
// version: 20.03.10
// Copyright (C)  Jeroen P. Broks
// This software is provided 'as-is', without any express or implied
// warranty.  In no event will the authors be held liable for any damages
// arising from the use of this software.
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
// 1. The origin of this software must not be misrepresented; you must not
// claim that you wrote the original software. If you use this software
// in a product, an acknowledgment in the product documentation would be
// appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be
// misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.
// EndLic

using TrickyUnits;
using System.Collections.Generic;
using System.Text;

namespace Wendicka_Engine {

    internal class Chunk {
        readonly ChunkMap Parent;
        WenState State => Parent.Parent;
        internal Chunk(ChunkMap Parent) { this.Parent = Parent; }
        internal class Instruction {
            
            internal class Param {
                internal enum DataKind { Einde, StartChunk, Instruction, Index, Chunk, Label, GlobalVar, LocalVar, Reference, String, IntValue, FloatValue, Boolean, Null, API }
                DataKind Kind;

            }
        }
    }

    internal class ChunkMap {        
        SortedDictionary<string, Chunk> M = new SortedDictionary<string, Chunk>();
        readonly internal WenState Parent;
        public ChunkMap(WenState Parent) { this.Parent = Parent; }
        public Chunk this[string k] {
            get {
                if (!M.ContainsKey(k)) M[k] = new Chunk(this);
                return M[k];
            }
        }

        static public void Hello() {
            MKL.Version("Wendicka Project - Chunks and stuff.cs","20.03.10");
            MKL.Lic    ("Wendicka Project - Chunks and stuff.cs","ZLib License");
        }
    }

}