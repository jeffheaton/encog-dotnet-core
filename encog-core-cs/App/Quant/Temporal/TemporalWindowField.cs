using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Temporal
{
    public class TemporalWindowField
    {
        public bool Ignore 
        {
            get
            {
                return !Input && !Predict;
            }
        }

        public bool Input { get; set; }
        public bool Predict { get; set; }
    }
}
