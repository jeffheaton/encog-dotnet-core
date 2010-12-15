using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks;
using Encog.Engine.Util;
using Encog.Neural.Data;

namespace Encog.App.Quant.Script.Commands
{
    public class CmdDatasetEvaluate: IQuantCommand
    {
        public string CommandName
        {
            get { return "datasetevaluate"; }
        }

        public void Execute(EncogQuantScript script, ParseLine line)
        {
            String networkName = line.GetParameterString("network", true);
            BasicNetwork network = (BasicNetwork)script.Memory[networkName];

            String name = line.GetParameterString("training", true);
            BasicNeuralDataSet training = (BasicNeuralDataSet)script.Memory[name];


            foreach (BasicNeuralDataPair pair in training)
            {
                StringBuilder str = new StringBuilder();

                str.Append("Input: ");
                for (int i = 0; i < training.InputSize; i++)
                {
                    if (i > 0)
                        str.Append(',');
                    str.Append(Format.FormatDouble(pair.Input[i], 2));
                }

                if (pair.Ideal != null)
                {
                    str.Append(", Ideal: ");
                    for (int i = 0; i < training.IdealSize; i++)
                    {
                        if (i > 0)
                            str.Append(',');
                        str.Append(Format.FormatDouble(pair.Ideal[i], 2));
                    }
                }

                str.Append(", Actual: ");
                INeuralData output = network.Compute(pair.Input);

                for (int i = 0; i < output.Count; i++)
                {
                    if (i > 0)
                        str.Append(',');
                    str.Append(Format.FormatDouble(output[i], 2));
                }

                script.WriteLine(str.ToString());
            }
        }

    }
}
