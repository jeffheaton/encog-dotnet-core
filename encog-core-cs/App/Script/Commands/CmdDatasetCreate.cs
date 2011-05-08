using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data.Basic;
using Encog.App.Script.Objects;

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

            script.Memory[name] = new ScriptDataSet();
        }
    }
}
