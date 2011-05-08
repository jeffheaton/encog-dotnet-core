using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Script.Objects;
using Encog.Persist;
using Encog.App.Script.Util;

namespace Encog.App.Script.Commands
{
    public class CmdFileFind : IQuantCommand
    {
        public string CommandName
        {
            get
            {
                return "filefind";
            }
        }

        public void Execute(EncogQuantScript script, ParseLine line)
        {
            String name = line.GetParameterString("name", true);
            String resource = line.GetParameterString("resource", true);
            String data = line.GetParameterString("data", true);

            // see if we need to create a new collection
            ScriptEGFile encog = ((ScriptEGFile)script.RequireObject(name, typeof(ScriptEGFile), true));

            // add the object
            IEncogPersistedObject obj = encog.Collection.Find(resource);
            IScriptedObject obj2 = ScriptedObjects.MakeScripted(obj);
            script.SetVariable(resource, obj2);
        }
    }
}
