using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Script.Objects;

namespace Encog.App.Script.Commands
{
    public class CmdFileAdd : IQuantCommand
    {
        public string CommandName
        {
            get 
            { 
                return "fileadd"; 
            }
        }

        public void Execute(EncogQuantScript script, ParseLine line)
        {
            String name = line.GetParameterString("name", true);
            String resource = line.GetParameterString("resource", true);
            String data = line.GetParameterString("data", true);

            // see if we need to create a new collection
            ScriptEGFile encog;

            if (script.IsDefined(name))
            {
                encog = ((ScriptEGFile)script.RequireObject(name, typeof(ScriptEGFile), true));
            }
            else
            {
                encog = new ScriptEGFile();
                script.SetVariable(name, encog);
            }

            // add the object
            encog.Collection.Add(resource, script.RequireObject(data,null,true).EncogObject);
        }
    }
}
