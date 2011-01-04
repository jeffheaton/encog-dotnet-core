using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Quant.Ninja;
using Encog.Util.CSV;

namespace Encog.App.Script.Commands
{
    public class CmdNinjaConvert : IQuantCommand
    {
        public string CommandName
        {
            get 
            {
                return "ninjaconvert";
            }
        }

        public void Execute(EncogQuantScript script, ParseLine line)
        {
            NinjaFileConvert convert = new NinjaFileConvert();
            String source = line.GetParameterString("source", true);
            String target = line.GetParameterString("target", true);
            convert.Analyze(source,true,CSVFormat.ENGLISH);
            convert.Process(target);
        }
    }
}
