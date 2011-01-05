using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Script.Objects;

namespace Encog.App.Script.Commands
{
    public class CmdFileRemove : IQuantCommand
    {
        public string CommandName
        {
            get
            {
                return "fileremove";
            }
        }

        public void Execute(EncogQuantScript script, ParseLine line)
        {
            String name = line.GetParameterString("name", true);
            String resource = line.GetParameterString("resource", true);

            // see if we need to create a new collection
            ScriptEGFile encog = ((ScriptEGFile)script.RequireObject(name, typeof(ScriptEGFile), true));

            // delete the object
            encog.Collection.Delete(name);
        }
    }
}
