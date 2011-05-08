using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Script.Objects;

namespace Encog.App.Script.Commands
{
    public class CmdFileLoad : IQuantCommand
    {
        public string CommandName
        {
            get
            {
                return "fileload";
            }
        }

        public void Execute(EncogQuantScript script, ParseLine line)
        {
            String name = line.GetParameterString("name", true);
            String filename = line.GetParameterString("filename", true);

            ScriptEGFile encog = new ScriptEGFile();
            script.SetVariable(name, encog);
            encog.Collection.Load(filename);
        }
    }
}
