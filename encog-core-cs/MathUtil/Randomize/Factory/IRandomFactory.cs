using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.MathUtil.Randomize.Factory
{
    public interface IRandomFactory
    {
        Random Factor();
        IRandomFactory FactorFactory();
    }
}
