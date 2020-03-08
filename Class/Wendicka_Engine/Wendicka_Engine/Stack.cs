using System;
using System.Collections.Generic;
using System.Text;

/*
 * This is a simple stacking engine.
 * It's rather a kind of linked list in stead of a true stacker, however this will do the job, as far as Wendicka is concerned that is what matters most to me!
 */ 


namespace Wendicka_Engine {
    class Stack<StackType> {

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
