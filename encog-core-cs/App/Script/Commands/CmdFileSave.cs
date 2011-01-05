using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Script.Objects;

namespace Encog.App.Script.Commands
{
    public class CmdFileSave : IQuantCommand
    {
        public string CommandName
        {
            get
            {
                return "filesave";
            }
        }

        public void Execute(EncogQuantScript script, ParseLine line)
        {
            String name = line.GetParameterString("name", true);
            String filename = line.GetParameterString("filename", true);

            ScriptEGFile encog = ((ScriptEGFile)script.RequireObject(name, typeof(ScriptEGFile), true));
            encog.Collection.Save(filename);
        }
    }
}
