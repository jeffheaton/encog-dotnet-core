//
// Encog(tm) Core v3.1 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2012 Heaton Research, Inc.
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
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Util;
using Encog.Util.Simple;

namespace Encog.Examples.Persist
{
    public class PersistSerial : IExample
    {
        /// <summary>
        /// Input for the XOR function.
        /// </summary>
        public static double[][] XOR_INPUT = {
                                                 new double[2] {0.0, 0.0},
                                                 new double[2] {1.0, 0.0},
                                                 new double[2] {0.0, 1.0},
                                                 new double[2] {1.0, 1.0}
                                             };

        /// <summary>
        /// Ideal output for the XOR function.
        /// </summary>
        public static double[][] XOR_IDEAL = {
                                                 new double[1] {0.0},
                                                 new double[1] {1.0},
                                                 new double[1] {1.0},
                                                 new double[1] {0.0}
                                             };

        private IExampleInterface app;

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof (PersistSerial),
                    "persist-serial",
                    "Persist using .Net Serialization",
                    "Create and persist a neural network using .Net serialization.");
                return info;
            }
        }

        #region IExample Members

        public void Execute(IExampleInterface app)
        {
            this.app = app;
            this.app = app;
            IMLDataSet trainingSet = new BasicMLDataSet(XOR_INPUT, XOR_IDEAL);
            BasicNetwork network = EncogUtility.SimpleFeedForward(2, 6, 0, 1, false);
            EncogUtility.TrainToError(network, trainingSet, 0.01);
            double error = network.CalculateError(trainingSet);
            SerializeObject.Save("encog.ser", network);
            network = (BasicNetwork) SerializeObject.Load("encog.ser");
            double error2 = network.CalculateError(trainingSet);
            app.WriteLine("Error before save to ser: " + Format.FormatPercent(error));
            app.WriteLine("Error before after to ser: " + Format.FormatPercent(error2));
        }

        #endregion
    }
}
