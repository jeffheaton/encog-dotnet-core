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
using System.Text;
using Encog.Engine.Network.Activation;
using Encog.ML.Data.Versatile;
using Encog.ML.Data.Versatile.Normalizers.Strategy;
using Encog.ML.Factory;
using Encog.Neural.Networks;

namespace Encog.ML.Model.Config
{
    /// <summary>
    ///     Config class for EncogModel to use a feedforward neural network.
    /// </summary>
    public class FeedforwardConfig : IMethodConfig
    {
        /// <inheritdoc />
        public String MethodName
        {
            get { return MLMethodFactory.TypeFeedforward; }
        }

        /// <inheritdoc />
        public String SuggestModelArchitecture(VersatileMLDataSet dataset)
        {
            int inputColumns = dataset.NormHelper.InputColumns.Count;
            int outputColumns = dataset.NormHelper.OutputColumns.Count;
            var hiddenCount = (int) ((inputColumns + outputColumns)*1.5);
            var result = new StringBuilder();
            result.Append("?:B->TANH->");
            result.Append(hiddenCount);
            result.Append(":B->TANH->?");
            return result.ToString();
        }

        /// <inheritdoc />
        public INormalizationStrategy SuggestNormalizationStrategy(VersatileMLDataSet dataset, String architecture)
        {
            double inputLow = -1;
            double inputHigh = 1;
            double outputLow = -1;
            double outputHigh = 1;

            // Create a basic neural network, just to examine activation functions.
            var methodFactory = new MLMethodFactory();
            var network = (BasicNetwork) methodFactory.Create(MethodName, architecture, 1, 1);

            if (network.LayerCount < 1)
            {
                throw new EncogError("Neural network does not have an output layer.");
            }

            IActivationFunction outputFunction = network.GetActivation(network.LayerCount - 1);

            double[] d = {-1000, -100, -50};
            outputFunction.ActivationFunction(d, 0, d.Length);

            if (d[0] > 0 && d[1] > 0 && d[2] > 0)
            {
                inputLow = 0;
            }

            INormalizationStrategy result = new BasicNormalizationStrategy(
                inputLow,
                inputHigh,
                outputLow,
                outputHigh);
            return result;
        }


        /// <inheritdoc />
        public String SuggestTrainingType()
        {
            return "rprop";
        }


        /// <inheritdoc />
        public String SuggestTrainingArgs(string trainingType)
        {
            return "";
        }

        /// <inheritdoc />
        public int DetermineOutputCount(VersatileMLDataSet dataset)
        {
            return dataset.NormHelper.CalculateNormalizedOutputCount();
        }
    }
}
