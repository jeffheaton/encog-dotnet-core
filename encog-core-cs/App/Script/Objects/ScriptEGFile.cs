using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist;

namespace Encog.App.Script.Objects
{
    public class ScriptEGFile : IScriptedObject
    {
        private EncogMemoryCollection encog = new EncogMemoryCollection();

        public EncogMemoryCollection Collection
        {
            get
            {
                return encog;
            }
        }

        public void PerformFinalize(EncogQuantScript script)
        {
        }

        public bool IsFinal()
        {
            return true;
        }


        public IEncogPersistedObject EncogObject
        {
            get 
            {
                return null;
            }
        }
    }
}
