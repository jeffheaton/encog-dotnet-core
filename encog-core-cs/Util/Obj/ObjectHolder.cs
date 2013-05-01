using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.Obj
{
    public struct ObjectHolder<T>
    {
        public T obj;
        public double probability;
    }
}
