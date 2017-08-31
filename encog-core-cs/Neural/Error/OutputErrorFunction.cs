using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Error
{
    public class OutputErrorFunction: IErrorFunction
    {
        public void CalculateError(IActivationFunction af, double[] b, double[] a,
            IMLData ideal, double[] actual, double[] error, double derivShift,
            double significance)
        {

            for (int i = 0; i < actual.Length; i++)
            {
                double deriv = af.DerivativeFunction(b[i], a[i]) + derivShift;
                error[i] = ((ideal[i] - actual[i]) * significance) * deriv;
            }
        }
    }
}
