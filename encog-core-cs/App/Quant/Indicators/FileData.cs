using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Indicators
{
    public class FileData: BaseColumn
    {
        public const String HIGH = "high";
        public const String LOW = "low";
        public const String OPEN = "open";
        public const String CLOSE = "close";
        public const String VOLUME = "volume";

        public int Index { get; set; }

        public FileData(String name, int index, bool input, bool output)
            :base(name, input, output)
        {           
            Output = output;
            Index = index;
        }
    }
}
