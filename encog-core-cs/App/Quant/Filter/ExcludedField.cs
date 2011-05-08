using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;

namespace Encog.App.Quant.Filter
{
    public class ExcludedField
    {
        public int FieldNumber { get; set; }
        public String FieldValue { get; set; }

        public ExcludedField(int fieldNumber, String fieldValue)
        {
            this.FieldNumber = fieldNumber;
            this.FieldValue = fieldValue;
        }
    }
}
