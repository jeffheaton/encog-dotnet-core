using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Script.Commands
{
    public class CmdWriteLine: IQuantCommand
    {

        public string CommandName
        {
            get { return "writeline"; }
        }

        public void Execute(EncogQuantScript script, ParseLine line)
        {
            String str = script.ResolveVariables(line.PrimaryParameter);
            script.WriteLine(str);
        }
    }
}
