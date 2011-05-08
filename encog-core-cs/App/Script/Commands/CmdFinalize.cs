using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Script.Objects;

namespace Encog.App.Script.Commands
{
    public class CmdFinalize : IQuantCommand
    {
        public string CommandName
        {
            get { return "finalize"; }
        }

        public void Execute(EncogQuantScript script, ParseLine line)
        {
            String name = line.GetParameterString("name", true);
            IScriptedObject obj = script.RequireObject(name, null, false);
            if (obj.IsFinal())
            {
                throw new ScriptError("Can't finalize " + name + ", it has already been finalized.");
            }

            obj.PerformFinalize(script);
        }
    }
}
