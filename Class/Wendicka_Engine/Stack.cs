// Lic:
// Class/Wendicka_Engine/Stack.cs
// Wendicka - Stacking
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
using System.Collections.Generic;
using System.Text;
using TrickyUnits;

/*
 * This is a simple stacking engine.
 * It's rather a kind of linked list in stead of a true stacker, however this will do the job, as far as Wendicka is concerned that is what matters most to me!
 */ 


namespace Wendicka_Engine {
    class Stack<StackType> {

        public static void Hello() {
            MKL.Version("Wendicka Project - Stack.cs","20.03.10");
            MKL.Lic    ("Wendicka Project - Stack.cs","ZLib License");
        }

        class StackItem {
            internal StackItem Prev;
            internal StackItem Next;
            internal StackType Obj;
        }

        private StackItem FirstStackItem;
        public void Push(StackType Obj) {
            var NI = new StackItem();
            NI.Obj = Obj;
            NI.Prev = FirstStackItem;
            if (NI.Prev != null) NI.Prev.Next = NI;
            FirstStackItem = NI;
        }
        public StackType Pop { get {
                var PI = FirstStackItem;
                if (PI != null && PI.Prev != null) {
                    PI.Prev.Next = null;
                }
                FirstStackItem = PI.Prev;
                return PI.Obj;
            }
        }

        public int Count {
            get {
                int i = 0;
                var CI = FirstStackItem;
                while (CI != null) {
                    i++;
                    CI = CI.Prev;
                }
                return i;
            }
        }

        public StackType[] ToArray() {
            var ret = new StackType[Count];
            var CI = FirstStackItem;
            var i = 0;
            while (CI != null) {
                ret[i] = CI.Obj;
                CI = CI.Prev;
                i++;
            }
            return ret;
        }

        public override string ToString() {
            var ret = new StringBuilder();
            var CI = FirstStackItem;
            while (CI != null) {
                ret.Append($"{CI.Obj}");
            }
            return $"{ret}";
        }

        public StackType this[int i] {
            get {
                var CI = FirstStackItem;
                while (i > 0) {
                    --i; CI = CI.Prev;
                    if (CI == null) throw new Exception("Beyond stack");
                }
                return CI.Obj;
            }
        }

        public Stack(params StackType[] Objs) {
            foreach (StackType O in Objs) Push(O);
        }

    }
}