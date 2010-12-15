using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Script.Commands
{
    public class CmdWriteLine: IQuantCommand
    {

        public string CommandName
        {
            get { return "WriteLine"; }
        }

        public void Execute(EncogQuantScript script, ParseLine line)
        {
            script.WriteLine(line.PrimaryParameter);
        }
    }
}
