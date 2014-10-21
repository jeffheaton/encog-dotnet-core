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
using Encog.ML.Train;
using Encog.Neural.SOM;
using Encog.Neural.Som.Training.Clustercopy;

namespace Encog.ML.Factory.Train
{
    /// <summary>
    /// Create a trainer that uses the SOM cluster training method.
    /// </summary>
    ///
    public class ClusterSOMFactory
    {
        /// <summary>
        /// Create a cluster SOM trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            if (!(method is SOMNetwork))
            {
                throw new EncogError(
                    "Cluster SOM training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            return new SOMClusterCopyTraining((SOMNetwork) method, training);
        }
    }
}
