//
// Encog(tm) Core v3.2 - .Net Version
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
using ConsoleExamples.Examples;
using Encog.Engine.Network.Activation;
using Encog.ML.Data.Basic;
using Encog.Neural.Freeform;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Util.Simple;

namespace Encog.Examples.Freeform
{
    public class ConvertToFreeform: IExample
    {
        /// <summary>
        ///     Input for the XOR function.
        /// </summary>
        public static double[][] XORInput =
        {
            new[] {0.0, 0.0},
            new[] {1.0, 0.0},
            new[] {0.0, 1.0},
            new[] {1.0, 1.0}
        };

        /// <summary>
        ///     Ideal output for the XOR function.
        /// </summary>
        public static double[][] XORIdeal =
        {
            new[] {0.0},
            new[] {1.0},
            new[] {1.0},
            new[] {0.0}
        };

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(ConvertToFreeform),
                    "freeform-convert",
                    "Freeform Network: convert flat network to freeform",
                    "Create a flat network and convert to freeform");
                return info;
            }
        }

        public void Execute(IExampleInterface app)
        {
            // create a neural network, without using a factory
            var network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, 2));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 3));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 1));
            network.Structure.FinalizeStructure();
            network.Reset();

            // create training data
            var trainingSet = new BasicMLDataSet(XORInput, XORIdeal);
            EncogUtility.TrainToError(network, trainingSet, 0.01);
            EncogUtility.Evaluate(network, trainingSet);

            var ff = new FreeformNetwork(network);
            EncogUtility.Evaluate(ff, trainingSet);

            EncogFramework.Instance.Shutdown();
        }
    }
}
