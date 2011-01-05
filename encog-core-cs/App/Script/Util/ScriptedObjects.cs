using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist;
using Encog.Neural.Networks;
using Encog.Neural.Data.Basic;
using Encog.App.Script.Objects;

namespace Encog.App.Script.Util
{
    public class ScriptedObjects
    {
        public static IScriptedObject MakeScripted(IEncogPersistedObject obj)
        {
            if (obj is BasicNetwork)
            {
                return new ScriptNetwork((BasicNetwork)obj);
            }
            else if (obj is BasicNeuralDataSet)
            {
                return new ScriptDataSet((BasicNeuralDataSet)obj);
            }
            else
                return null;
        }

        public static String FormatName(String name)
        {
            String result;

            int idx = name.LastIndexOf('.');
            if (idx != null)
            {
                result = name.Substring(idx + 1);
            }
            else
            {
                result = name;
            }

            if (result.StartsWith("Script"))
            {
                result = result.Substring(6);
            }

            return result;
        }
    }
}
