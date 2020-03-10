using System;
using TrickyUnits;

namespace Wendicka_Engine {
    class WenState {
        static int cnt = 0;
        readonly public string StateName;
        void SetUp(byte[] data) {
            cnt++;
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
    }
}
