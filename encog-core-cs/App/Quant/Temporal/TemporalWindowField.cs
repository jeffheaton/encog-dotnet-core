using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Temporal
{
    public class TemporalWindowField
    {
        public TemporalWindowField(String name)
        {
            this.Name = name;
        }

        public bool Input
        {
            get
            {
                return( Action==TemporalType.Input || Action==TemporalType.InputAndPredict );
            }
        }

        public bool Predict
        {
            get
            {
                return (Action == TemporalType.Predict || Action == TemporalType.InputAndPredict);
            }
        }


        public TemporalType Action { get; set; }
        public String Name { get; set; }
        public String LastValue { get; set; }
    }
}
