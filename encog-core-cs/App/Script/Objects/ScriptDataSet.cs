using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data.Basic;

namespace Encog.App.Script.Objects
{
    public class ScriptDataSet: IScriptedObject
    {
        private BasicNeuralDataSet dataset;

        public BasicNeuralDataSet Dataset
        {
            get
            {
                return this.dataset;
            }
        }

        public ScriptDataSet(BasicNeuralDataSet dataset)
        {
            this.dataset = dataset;
        }

        public ScriptDataSet():
            this(new BasicNeuralDataSet())
        {            
        }

        public void PerformFinalize(EncogQuantScript script)
        {
        }

        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[DataSet: input=");
            result.Append(this.dataset.InputSize);
            result.Append(", ideal=");
            result.Append(this.dataset.IdealSize);
            result.Append(", rows=");
            result.Append(this.dataset.Count);
            result.Append("]");
            return result.ToString();
        }

        public bool IsFinal()
        {
            return true;
        }


        public Persist.IEncogPersistedObject EncogObject
        {
            get 
            {
                return this.dataset;
            }
        }
    }
}
