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
using Encog.MathUtil.LIBSVM;
using Encog.ML.Data;

namespace Encog.ML.SVM.Training
{
    /// <summary>
    /// Encode an Encog dataset as a SVM problem.
    /// </summary>
    ///
    public static class EncodeSVMProblem
    {
        /// <summary>
        /// Encode the Encog dataset.
        /// </summary>
        ///
        /// <param name="training">The training data.</param>
        /// <param name="outputIndex"></param>
        /// <returns>The SVM problem.</returns>
        public static svm_problem Encode(IMLDataSet training,
                                         int outputIndex)
        {
            try
            {
                var result = new svm_problem {l = (int) training.Count};

                result.y = new double[result.l];
                result.x = new svm_node[result.l][];
                for (int i = 0; i < result.l; i++)
                {
                    result.x[i] = new svm_node[training.InputSize];
                }

                int elementIndex = 0;


                foreach (IMLDataPair pair  in  training)
                {
                    IMLData input = pair.Input;
                    IMLData output = pair.Ideal;
                    result.x[elementIndex] = new svm_node[input.Count];

                    for (int i = 0; i < input.Count; i++)
                    {
                        result.x[elementIndex][i] = new svm_node {index = i + 1, value_Renamed = input[i]};
                    }

                    result.y[elementIndex] = output[outputIndex];

                    elementIndex++;
                }

                return result;
            }
            catch (OutOfMemoryException )
            {
                throw new EncogError("SVM Model - Out of Memory");
            }
        }
    }
}
