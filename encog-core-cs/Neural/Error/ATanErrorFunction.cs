//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using Encog.ML.Data;
using Encog.Engine.Network.Activation;

namespace Encog.Neural.Error
{
    /// <summary>
    /// An ATan based error function.  This is often used either with QuickProp
    /// or alone.  This can improve the training time of a propagation
    /// trained neural network.
    /// </summary>
    public class ATanErrorFunction : IErrorFunction
    {
        public void CalculateError(IActivationFunction af, double[] b, double[] a,
            IMLData ideal, double[] actual, double[] error, double derivShift,
            double significance)
        {

            for (int i = 0; i < actual.Length; i++)
            {
                double deriv = af.DerivativeFunction(b[i], a[i]);
                error[i] = (Math.Atan(ideal[i] - actual[i]) * significance) * deriv;
            }
        }
    }
}
