using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;

namespace Encog.App.Script.Objects
{
    public class ScriptNetwork: IScriptedObject
    {
        private BasicNetwork network;
        private IList<ScriptLayer> layers = new List<ScriptLayer>();

        public BasicNetwork Network
        {
            get
            {
                return this.network;
            }
        }

        public IList<ScriptLayer> Layers
        {
            get
            {
                return this.layers;
            }
        }

        public ScriptNetwork()
        {
        }

        public ScriptNetwork(BasicNetwork network)
        {
            this.network = network;
        }

        public void PerformFinalize(EncogQuantScript script)
        {
        }

        void IScriptedObject.PerformFinalize(EncogQuantScript script)
        {
            this.network = new BasicNetwork();
            foreach (ScriptLayer layer in layers)
            {
                this.network.AddLayer(new BasicLayer(
                    layer.Activation,layer.HasBiasLayer,layer.NeuronCount));
            }
            this.network.Structure.FinalizeStructure();
            this.network.Reset();
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[Network: ");
            if (result.Length != 0)
            {
                result.Append("input=");
                result.Append(this.network.InputCount);
                result.Append(", output=");
                result.Append(this.network.OutputCount);
            }
            else
            {
                result.Append("Not finalized");
            }

            result.Append("]");
            return result.ToString();
        }


        public bool IsFinal()
        {
            return network != null;
        }


        public Persist.IEncogPersistedObject EncogObject
        {
            get 
            {
                return this.network;
            }
        }
    }
}
