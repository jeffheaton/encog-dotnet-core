using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.MathUtil.Randomize.Factory
{
    public interface IRandomFactory
    {
        EncogRandom Factor();
        IRandomFactory FactorFactory();
    }
}
