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
using Encog.MathUtil.Error;
using Encog.ML;
using Encog.ML.Data;

namespace Encog.Util.Error
{
    /// <summary>
    /// Calculate the error for regression based Machine Learning Methods.
    /// </summary>
    public class CalculateRegressionError
    {
        /// <summary>
        /// Calculate an error for a method that uses regression.
        /// </summary>
        /// <param name="method">The method to evaluate.</param>
        /// <param name="data">The training data to evaluate with.</param>
        /// <returns>The error.</returns>
        public static double CalculateError(IMLRegression method,
                                            IMLDataSet data)
        {
            var errorCalculation = new ErrorCalculation();

            // clear context
            if (method is IMLContext)
            {
                ((IMLContext) method).ClearContext();
            }


            // calculate error
            foreach (IMLDataPair pair  in  data)
            {
                IMLData actual = method.Compute(pair.Input);
                errorCalculation.UpdateError(actual, pair.Ideal, pair.Significance);
            }
            return errorCalculation.Calculate();
        }
    }
}
