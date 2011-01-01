using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data.Basic;

namespace Encog.App.Script.Commands
{
    public class CmdDatasetCreate: IQuantCommand
    {

        public string CommandName
        {
            get { return "datasetcreate"; }
        }

        public void Execute(EncogQuantScript script, ParseLine line)
        {
            String name = line.GetParameterString("name", true);

            BasicNeuralDataSet set = new BasicNeuralDataSet();
            script.Memory[name] = set;
        }
    }
}
