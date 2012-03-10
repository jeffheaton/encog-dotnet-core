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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks.Training.Propagation.Back;
using SuperUtils = Encog.Util.NetworkUtil.NetworkUtility;
namespace Encog.Examples.RangeandMarket
{
    static class RandomTrainer
    {

        /// <summary>
        /// Trains a random trainer.
        /// </summary>
        /// <param name="inputs">The inputs.</param>
        /// <param name="predictWindow">The predict window.</param>
        public static void RandomTrainerMethod(int inputs, int predictWindow)
        {
            double[] firstinput = MakeInputs(inputs);
            double[] SecondInput = MakeInputs(inputs);
            double[] ThirdInputs = MakeInputs(inputs);
            double[] FourthInputs = MakeInputs(inputs);
            var pair = SuperUtils.ProcessPair(firstinput, FourthInputs, inputs, predictWindow);
            var pair2 = SuperUtils.ProcessPair(SecondInput, FourthInputs, inputs, predictWindow);
            var pair3 = SuperUtils.ProcessPair(ThirdInputs, FourthInputs, inputs, predictWindow);
            var pair4 = SuperUtils.ProcessPair(FourthInputs, FourthInputs, inputs, predictWindow);
            BasicMLDataSet SuperSet = new BasicMLDataSet();
            SuperSet.Add(pair);
            SuperSet.Add(pair2);
            SuperSet.Add(pair3);
            SuperSet.Add(pair4);
            var network = (BasicNetwork)CreateEval.CreateElmanNetwork(SuperSet.InputSize, SuperSet.IdealSize);
            double error = CreateEval.TrainNetworks(network, SuperSet);
            //Lets create an evaluation.
            Console.WriteLine(@"Last error rate on random trainer:" + error);
        }

        /// <summary>
        /// Makes the inputs by randomizing with encog randomize , the normal random from net framework library.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        public static double[] MakeInputs(int number)
        {
            Random rdn = new Random();
            Encog.MathUtil.Randomize.RangeRandomizer encogRnd = new Encog.MathUtil.Randomize.RangeRandomizer(-1, 1);
            double[] x = new double[number];
            for (int i = 0; i < number; i++)
            {
                x[i] = encogRnd.Randomize((rdn.NextDouble()));

            }
            return x;
        }

       
    }
}
