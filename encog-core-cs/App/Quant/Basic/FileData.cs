using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Basic
{
    public class FileData : BaseCachedColumn
    {
        public const String DATE = "date";
        public const String TIME = "time";
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
