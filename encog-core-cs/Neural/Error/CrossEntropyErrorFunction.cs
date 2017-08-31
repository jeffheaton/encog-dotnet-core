using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data;
using Encog.Engine.Network.Activation;

namespace Encog.Neural.Error
{
    /// <summary>
    /// Implements a cross entropy error function.  This can be used with backpropagation to
    /// sometimes provide better performance than the standard linear error function.
    /// </summary>
    public class CrossEntropyErrorFunction : IErrorFunction
    {
        public void CalculateError(IActivationFunction af, double[] b, double[] a,
            IMLData ideal, double[] actual, double[] error, double derivShift,
            double significance)
        {

            for (int i = 0; i < actual.Length; i++)
            {
                error[i] = (ideal[i] - actual[i]) * significance;
            }
        }
    }
}
