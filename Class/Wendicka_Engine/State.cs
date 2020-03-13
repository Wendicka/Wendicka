// Lic:
// Class/Wendicka_Engine/State.cs
// Wendicka - State
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
using System;
using TrickyUnits;

namespace Wendicka_Engine {
    class WenState {
        #region variable declarations
        static int cnt = 0;
        readonly public string StateName;
        readonly TMap<string, Variable> Globals = new TMap<string, Variable>();
        ChunkMap Chunks;
        #endregion

        #region debug
        void Chat(params string[] m) {
#if QCLI && DEBUG
            foreach (string M in m) Console.WriteLine($"QCLI.DEBUG.CHAT: {M}");
#endif
        }
        #endregion

        #region Init
        void SetUp(byte[] data) {
            var WBIN = "WBIN";
            int startoffs=0;
            byte[] offbytes;
            cnt++;
            Chunks = new ChunkMap(this);
            if (data.Length < 12) throw new ExWendickaBinTooShort();
            for(int i = 0; i < 4; i++) {
                if ((char)data[(data.Length - 12) + i] != WBIN[i]) throw new ExWendickaNotRecognized();
                if ((char)data[(data.Length - 4) + i] != WBIN[i]) throw new ExWendickaNotRecognized();
            }
            offbytes = new byte[4] { data[(data.Length - 8)], data[(data.Length - 7)], data[(data.Length - 6)], data[(data.Length - 5)] };
            if (!BitConverter.IsLittleEndian) Array.Reverse(offbytes);
            startoffs = (data.Length-BitConverter.ToInt32(offbytes,0))-4;
#if QCLI && DEBUG
            foreach (byte b in offbytes) Console.Write($"{b.ToString("X")}.{b}.{(char)b};\t");
            Console.WriteLine($"{BitConverter.ToInt32(offbytes, 0)} / {data.Length} / {startoffs}");
#endif
            if (startoffs < 0 || startoffs > data.Length - 12) throw new ExWendickaFalseBineryOffset(startoffs);
            var bt = QuickStream.StreamFromBytes(data);
            do { } while (bt.ReadByte() != 26);
            //var bitMain = bt.ReadByte();
            //var bitInst = bt.ReadByte();
            //if (bitMain != 1 || bitInst != 1) throw new ExWendickaNotYetSupported($"Tagging other than 8 bit ({bitMain}/{bitInst})");
            Chunk CChunk;
            string ChunkName="";
            long ChunkStart;
            long ChunkPos() => bt.Position - ChunkStart;
            long ChunkLength;
            long ChunkEnd;
            void ChunkCheck(Chunk.Instruction.Param.DataKind i) { if (CChunk == null || ChunkName == "") throw new ExWendickaChunkless($"{i}", bt.Position); }
            Chunk.Instruction.Param.DataKind GetTag() => (Chunk.Instruction.Param.DataKind)bt.ReadByte();
            do {
                var tag = GetTag();
                Chat($"MainTag: {tag} / {(byte)tag}");
                switch (tag) {
                    case Chunk.Instruction.Param.DataKind.Einde:
                        goto escape;
                    case Chunk.Instruction.Param.DataKind.StartChunk: {
                            byte ChuTag = bt.ReadByte();
                            while (ChuTag != 0) {
                                Chat($"NEWCHUNK.TAG: {ChuTag}");
                                switch (ChuTag) {
                                    case 0:
                                        break; // Should never happen!
                                    case 1:
                                        ChunkName = bt.ReadString().ToUpper();
                                        Chat($"NEWCHUNK: {ChunkName}");
                                        CChunk = Chunks[ChunkName];
                                        break;
                                    case 2:
                                        ChunkStart = bt.Position;
                                        ChunkLength = (long)bt.ReadInt();
                                        ChunkEnd = ChunkStart + ChunkLength;                                        
                                        if (ChunkStart < 0 || ChunkEnd > bt.Size) throw new ExWendickaIllegalChunkSize(ChunkName, ChunkStart, ChunkLength, ChunkEnd);
                                        break;
                                }
                                ChuTag = bt.ReadByte();
                            }
                        }
                        var bitMain = bt.ReadByte();
                        var bitInst = bt.ReadByte();
                        if (bitMain != 8 || bitInst != 8) throw new ExWendickaNotYetSupported($"Tagging other than 8 bit ({bitMain}/{bitInst})");
                        break;
                    default:
                        throw new ExWendickaNotYetSupported($"Main tag {tag}({(byte)tag})");
                }
            } while (true);
            escape:
            bt.Close();
        }

        public WenState(string nameme,byte[] data) {
            StateName = nameme;
            SetUp(data);
        }
        public WenState(byte[] data) {
            StateName = $"WENDICKA.STATE:{qstr.md5($"{DateTime.Now}.cnt")}{cnt}";
            SetUp(data);
        }
        public WenState(string nameme,string file) {
            StateName = nameme;
            SetUp(QuickStream.GetFile(file));
        }
        public WenState(string file) {
            StateName = $"WENDICKA.STATE['{file}']:{qstr.md5($"{file}.{DateTime.Now}.cnt")}{cnt}";
            SetUp(QuickStream.GetFile(file));
        }
        #endregion


    }
}