using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Engine.Util;
using Encog.Neural.Networks.Training.Strategy;

namespace Encog.App.Quant.Script.Commands
{
    public class CmdTrain: IQuantCommand
    {
        private BasicNetwork network;
        private BasicNeuralDataSet training;
        private EncogQuantScript script;
        private int maxIterations = 0;       

        public string CommandName
        {
            get { return "train"; }
        }

        public void Execute(EncogQuantScript script, ParseLine line)
        {
            this.script = script;

            String networkName = line.GetParameterString("network", true);
            this.network = (BasicNetwork)script.Memory[networkName];

            String name = line.GetParameterString("training", true);
            this.training = (BasicNeuralDataSet)script.Memory[name];

            this.maxIterations = line.GetParameterInt("maxiterations", false);
            if (this.maxIterations == 0)
                this.maxIterations = int.MaxValue;

            Process();
        }

        private void Process()
        {
               ResilientPropagation rprop = new ResilientPropagation(network,training);               
               rprop.AddStrategy(new RequiredImprovementStrategy(5));
               do
               {
                   rprop.Iteration();
                   this.script.WriteLine("Training... Error:" + Format.FormatPercent(rprop.Error));
               } while (rprop.Error > 0.01 && rprop.CurrentIteration<this.maxIterations);

               this.script.WriteLine("Training Complete after " + Format.FormatInteger(rprop.CurrentIteration)  + " iterations, Error:" + Format.FormatPercent(rprop.Error));
        }
    }
}
