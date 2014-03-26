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
namespace Encog.Examples.LiveSimul
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using ConsoleExamples.Examples;

    using Encog.ML.Data;
    using Encog.ML.Data.Basic;
    using Encog.Util.NetworkUtil;
    using Encog.Util.Simple;

    /// <summary>
    /// A quick example that simulates neuronal activty in live situation and shows how to process data and compute into the network in 2 commands.
    /// </summary>
    public class LiveSimul : IExample
    {

        //Each input is taken and placed after the other until we have input x 10 -Slided -windowed inputs.
        private const int WindowSize = 10;


        #region Implementation of IExample
        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(LiveSimul),
                    "livesimul",
                    "A quick example that simulates neuronal activty in live situation and shows how to process data and compute into the network in 2 commands..",
                    "Use a feedforward neural network to predict random data and normalizes.");
                return info;
            }
        }

        public void Execute(IExampleInterface app)
        {
            double[] firstinput = MakeInputs(150);
            double[] SecondInput = MakeInputs(150);
            double[] ThirdInputs = MakeInputs(150);
            double[] ideals = MakeInputs(150);
            //our set holds both the normilization and the imldataset, we can put as many inputs as needed.
            var set = EasyData.Load(ideals, WindowSize, firstinput, SecondInput, ThirdInputs, ideals);

            var network = EncogUtility.SimpleFeedForward(4, 100, 1, 1, false);

            EncogUtility.TrainConsole(network,set.Item1,22.1);

            //Simulate live data ..

            double[] live1 = MakeInputs(150);
            double[] Live2 = MakeInputs(150);
            double[] live3 = MakeInputs(150);

            var computes= EasyData.GetReadiedComputePair(WindowSize, live1, Live2, live3);

            Console.WriteLine("Network computed denormalized : " + computes.Item2.Stats.DeNormalize(network.Compute(new BasicMLData(computes.Item1.ToArray()))[0]));

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
        #endregion
    }
}
