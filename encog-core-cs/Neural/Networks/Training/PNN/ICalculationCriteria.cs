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
namespace Encog.Neural.Networks.Training.PNN
{
    /// <summary>
    /// Calculate criteria.
    /// </summary>
    ///
    public interface ICalculationCriteria
    {
        /// <summary>
        /// Calculate the error with a single sigma.
        /// </summary>
        ///
        /// <param name="sigma">The sigma.</param>
        /// <returns>The error.</returns>
        double CalcErrorWithSingleSigma(double sigma);

        /// <summary>
        /// Calculate the error with multiple sigmas.
        /// </summary>
        ///
        /// <param name="x">The data.</param>
        /// <param name="direc">The first derivative.</param>
        /// <param name="deriv2">The 2nd derivatives.</param>
        /// <param name="b">Calculate the derivative.</param>
        /// <returns>The error.</returns>
        double CalcErrorWithMultipleSigma(double[] x, double[] direc,
                                          double[] deriv2, bool b);
    }
}
