using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Segregate
{
    public class SegregateFieldCompare
    {
        public bool IsAnd { get; set; }
        public int FieldNumber { get; set; }
        public String FieldValue { get; set; }

        public SegregateFieldCompare(bool isAnd, int fieldNumber, String value)
        {
            IsAnd = isAnd;
            FieldNumber = fieldNumber;
            FieldValue = value;
        }
    }
}
