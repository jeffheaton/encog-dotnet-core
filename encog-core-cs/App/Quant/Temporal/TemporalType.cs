using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Temporal
{
    public enum TemporalType
    {
        Input,
        Predict,
        InputAndPredict,
        Ignore,
        PassThrough
    }
}
