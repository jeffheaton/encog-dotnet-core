using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data.Basic;
using Encog.Util.CSV;
using Encog.Neural.Data;
using Encog.App.Script.Objects;

namespace Encog.App.Script.Commands
{
    public class CmdDatasetAdd: IQuantCommand
    {
        public string CommandName
        {
            get { return "datasetadd"; }
        }

        public void Execute(EncogQuantScript script, ParseLine line)
        {
            String name = line.GetParameterString("name", true);
            BasicNeuralDataSet set = ((ScriptDataSet)script.RequireObject(name, typeof(ScriptDataSet), false)).Dataset;

            INeuralData inputData = null;
            INeuralData idealData = null;

            String input = line.GetParameterString("input", true);
            double[] inputArray = NumberList.FromList(CSVFormat.EG_FORMAT, input);
            inputData = new BasicNeuralData(inputArray);

            String ideal = line.GetParameterString("ideal", false);
            if (ideal != null)
            {
                double[] idealArray = NumberList.FromList(CSVFormat.EG_FORMAT, ideal);
                idealData = new BasicNeuralData(idealArray);
            }

            BasicNeuralDataPair pair = new BasicNeuralDataPair(inputData, idealData);
            set.Add(pair);
        }
    }
}
