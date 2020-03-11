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
        ChunkMap Chunks;
        #endregion

        #region Init
        void SetUp(byte[] data) {
            cnt++;
            Chunks = new ChunkMap(this);
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