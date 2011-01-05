using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Script.Objects;
using Encog.Engine.Network.Activation;

namespace Encog.App.Script.Commands
{
    public class CmdNetworkLayer : IQuantCommand
    {
        public string CommandName
        {
            get { return "networklayer"; }
        }

        public void Execute(EncogQuantScript script, ParseLine line)
        {
            String name = line.GetParameterString("name", true);
            String activation = line.GetParameterString("activation", false);
            int count = line.GetParameterInt("count", true);
            bool bias = line.GetParameterBoolean("bias", false, true);

            IActivationFunction af;

            if (activation == null || activation.Length == 0)
            {
                af = new ActivationTANH();
            }
            else if (string.Compare(activation, "TANH", true) == 0)
            {
                af = new ActivationTANH();
            }
            else if (string.Compare(activation, "SIGMOID", true) == 0)
            {
                af = new ActivationSigmoid();
            }
            else if (string.Compare(activation, "LINEAR", true) == 0)
            {
                af = new ActivationLinear();
            }
            else
            {
                throw new ScriptError("Unknonwn activation function: " + activation);
            }

            ScriptNetwork network = (ScriptNetwork)script.RequireObject(name, typeof(ScriptNetwork),false ); 
            ScriptLayer layer = new ScriptLayer();
            layer.NeuronCount = count;
            layer.HasBiasLayer = true;
            layer.Activation = af;
            network.Layers.Add(layer);
        }
    }
}
