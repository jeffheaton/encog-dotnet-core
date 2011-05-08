using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Script.Objects
{
    public class ScriptVariable: IScriptedObject
    {
        public String StringValue { get; set; }

        public ScriptVariable(String str)
        {
            StringValue = str;
        }

        public void PerformFinalize(EncogQuantScript script)
        {            
        }


        public bool IsFinal()
        {
            return true;
        }


        public Persist.IEncogPersistedObject EncogObject
        {
            get 
            {
                return null;
            }
        }
    }
}
