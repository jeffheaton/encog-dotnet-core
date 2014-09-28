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
using System.Collections.Generic;
using Encog.MathUtil.Randomize;
using Encog.ML.Data;
using Encog.ML.Factory.Parse;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using Encog.Util;
using Encog.ML.Genetic;

namespace Encog.ML.Factory.Train
{
    /// <summary>
    /// A factory to create genetic algorithm trainers.
    /// </summary>
    ///
    public class GeneticFactory
    {
        /// <summary>
        /// Create an annealing trainer.
        /// </summary>
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            if (!(method is BasicNetwork))
            {
                throw new TrainingError(
                    "Invalid method type, requires BasicNetwork");
            }

            ICalculateScore score = new TrainingSetScore(training);

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);
            int populationSize = holder.GetInt(
				MLTrainFactory.PropertyPopulationSize, false, 5000);
		
		IMLTrain train = new MLMethodGeneticAlgorithm( () => {
			
				IMLMethod result = (IMLMethod) ObjectCloner.DeepCopy(method);
				((IMLResettable)result).Reset();
				return result;
			}, score, populationSize);

		return train;

       
        }
    }
}
