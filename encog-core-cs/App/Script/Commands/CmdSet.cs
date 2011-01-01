using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Script.Commands
{
    public class CmdSet: IQuantCommand
    {
        public string CommandName
        {
            get 
            { 
                return "set"; 
            }
        }

        public void Execute(EncogQuantScript script, ParseLine line)
        {
            foreach (String key in line.Parameters.Keys)
            {
                object value = line.Parameters[key];
                script.Memory[key.ToLower()] = value;
            }
        }
    }
}
