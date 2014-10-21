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
using System.Linq;
using System.Text;
using Encog.ML.Data;
using Encog.ML.Factory.Parse;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.NM;
using Encog.Util;

namespace Encog.ML.Factory.Train
{
    public class NelderMeadFactory
    {
        /// <summary>
        /// Create a Nelder Mead trainer.
        /// </summary>
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                IMLDataSet training, String argsStr)
        {

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);

            //final double learningRate = holder.getDouble(
            //		MLTrainFactory.PROPERTY_LEARNING_RATE, false, 0.1);

            return new NelderMeadTraining((BasicNetwork)method, training);
        }
    }
}
