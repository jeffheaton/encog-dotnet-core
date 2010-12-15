using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks.Pattern;
using Encog.Engine.Network.Activation;
using Encog.Neural.Networks;

namespace Encog.App.Quant.Script.Commands
{
    public class CmdNetworkCreate: IQuantCommand
    {
        public string CommandName
        {
            get { return "networkcreate"; }
        }

        public void Execute(EncogQuantScript script, ParseLine line)
        {
            String name = line.GetParameterString("name", true);
            int inputCount = line.GetParameterInt("input", true);
            int outputCount = line.GetParameterInt("output", true);

            FeedForwardPattern pattern = new FeedForwardPattern();
            pattern.InputNeurons = inputCount;
            pattern.OutputNeurons = outputCount;
            pattern.AddHiddenLayer( (inputCount + outputCount)*2);
            pattern.ActivationFunction = new ActivationTANH();

            BasicNetwork network = pattern.Generate();
            script.Memory[name] = network;
        }
    }
}
