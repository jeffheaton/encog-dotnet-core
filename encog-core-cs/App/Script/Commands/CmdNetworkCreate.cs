using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks.Pattern;
using Encog.Engine.Network.Activation;
using Encog.Neural.Networks;
using Encog.App.Script.Objects;

namespace Encog.App.Script.Commands
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

            ScriptNetwork network = new ScriptNetwork();
            script.Memory[name] = network;
        }
    }
}
