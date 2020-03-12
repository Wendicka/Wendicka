using System.Text;
using TrickyUnits;

namespace Wendicka_Engine {

    class Table {
        TMap<object, Variable> Tab = new TMap<object, Variable>();
        string Destructor = "";

        ~Table() {
            if (Destructor!="") {
                throw new System.Exception("Destructors not yet supported!");
            }
        }
    }

    class Variable {
        string Type = "NULL";
        Table TabPointer = null;
        StringBuilder StringValue = new StringBuilder("");
        long intvalue = 0;
        float floatvalue = 0;
    }
}