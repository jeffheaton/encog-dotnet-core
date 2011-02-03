using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Encog.App.Quant.Segregate
{
    public class SegregateTargetPercent
    {
        public int Percent { get; set; }
        public String Filename { get; set; }
        public TextWriter TargetFile { get; set; }
        public int NumberRemaining { get; set; }

        public SegregateTargetPercent(String outputFile, int percent)
        {
            this.Percent = percent;
            this.Filename = outputFile;
        }
    }
}
