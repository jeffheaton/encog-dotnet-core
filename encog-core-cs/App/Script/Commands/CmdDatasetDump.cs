using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data.Basic;
using Encog.Engine.Util;
using Encog.App.Script.Objects;

namespace Encog.App.Script.Commands
{
    public class CmdDatasetDump: IQuantCommand
    {
        public string CommandName
        {
            get { return "datasetdump"; }
        }

        public void Execute(EncogQuantScript script, ParseLine line)
        {
            String name = line.GetParameterString("name", true);
            BasicNeuralDataSet set = ((ScriptDataSet)script.RequireObject(name, typeof(ScriptDataSet), true)).Dataset;

            foreach (BasicNeuralDataPair pair in set)
            {
                StringBuilder str = new StringBuilder();
                for (int i = 0; i < set.InputSize; i++)
                {
                    if (i > 0)
                        str.Append(',');
                    str.Append(Format.FormatDouble(pair.Input[i],2));
                }

                if (pair.Ideal!=null )
                {
                    str.Append(" --> ");
                    for (int i = 0; i < set.IdealSize; i++)
                    {
                        if (i > 0)
                            str.Append(',');
                        str.Append(Format.FormatDouble(pair.Ideal[i], 2));
                    }
                }

                script.WriteLine(str.ToString());
            }
        }
    }
}
