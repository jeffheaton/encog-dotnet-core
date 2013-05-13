using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.Obj
{
    /// <summary>
    /// Holds objects, along with a probability.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    [Serializable]
    public struct ObjectHolder<T>
    {
        public T obj;
        public double probability;
    }
}
