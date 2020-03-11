// Lic:
// QCLI
// QCLI Wendicka Test
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
// Version: 20.03.10
// EndLic
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TrickyUnits;
using Wendicka_Engine;




namespace QCLI {
    class Program {

        static string MyExe => System.Reflection.Assembly.GetEntryAssembly().Location;

        static Program() {
            MKL.Lic    ("Wendicka Project - Program.cs","GNU General Public License 3");
            MKL.Version("Wendicka Project - Program.cs","20.03.10");
            WRT.Hello();
            qstr.Hello();
            QuickStream.Hello();
            GINI.Hello();
            FileList.Hello();
            GINIE.Hello();
            MKL.AllWidth = 60;
        }

        static void Head() {
            Console.WriteLine($"Wendicka Runtime QCLI, version {MKL.Newest}");
            Console.WriteLine("Coded by Jeroen P. Broks!");
            Console.WriteLine($"(c) {MKL.CYear(2020)} Jeroen P. Broks\n");
        }

        static void NoStuff() {
            Head();
            Console.WriteLine(MKL.All());
            Console.WriteLine($"Usage: {qstr.StripAll(MyExe)} <WendickaAppFile> [<parameters>]");
        }

        static void Run(string[] args) {
            var Targs = new string[args.Length - 1];
            var Exe = Dirry.AD(args[0]);
            for (int i = 1; i < args.Length; ++i) Targs[i - 1] = args[i];
            Debug.WriteLine($"Loading: {Exe}");
            var State = new WenState($"QCLI: {Exe}", Exe);
        }

        static void Main(string[] args) {
            Dirry.InitAltDrives();
            if (args.Length == 0)
                NoStuff();
            else
                Run(args);
            TrickyDebug.AttachWait();
        }
    }
}