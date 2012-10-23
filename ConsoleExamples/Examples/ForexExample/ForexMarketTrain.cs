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
using ConsoleExamples.Examples;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Temporal;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks.Training.Lma;
using Encog.Neural.Pattern;
using Encog.Util;
using Encog.Util.Arrayutil;

namespace Encog.Examples.ForexExample
{
    public class ForexMarketTrain : IExample
    {
        public const int StartingYear = 1700;
        public const int WindowSize = 30;
        public const int TrainStart = WindowSize;
        public const int TrainEnd = 259;
        public const int EvaluateStart = 2000;

        public const double MaxError = 0.01;

        public static double[] ForexPair = {0};
        public static void GetForexPairData()
        {
          ForexPair=  Encog.Util.NetworkUtil.QuickCSVUtils.QuickParseCSV("DB!EURUSD.Bar.Time.600.csv", "Close", 2500).ToArray();
            Console.WriteLine("Retrieved :{0} Closing values", ForexPair.Length);
            Console.WriteLine("Press a key to continue");
            Console.Read();
        }
        public static int EvaluateEnd = ForexPair.Length - 1;

        private double[] _closedLoopForexPair;
        private double[] _normalizedForexPair;

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(ForexMarketTrain),
                    "Forex",
                    "Predict Forex rates via CSV.",
                    "Use a Simple Recurrent Neural Network (Elman) to predict forex pair prices.");
                return info;
            }
        }

        #region IExample Members

        public void Execute(IExampleInterface app)
        {
            GetForexPairData();
            EvaluateEnd = EvaluateStart + 100;
            NormalizeForexPair(-1, 1);
            var network = (BasicNetwork)CreateElmanNetwork(WindowSize);
            IMLDataSet training = GenerateTraining();
            Train(network, training);
            Predict(network);
        }

        #endregion

        private NormalizeArray array;

        public void NormalizeForexPair(double lo, double hi)
        {
            array= new NormalizeArray { NormalizedHigh = hi, NormalizedLow = lo };
            // create arrays to hold the normalized forex pair data
            _normalizedForexPair = array.Process(ForexPair);
            _closedLoopForexPair = EngineArray.ArrayCopy(_normalizedForexPair);
        }


        public IMLDataSet GenerateTraining()
        {
            var result = new TemporalMLDataSet(WindowSize, 1);
            var desc = new TemporalDataDescription(TemporalDataDescription.Type.Raw, true, true);
            result.AddDescription(desc);

            for (var year = TrainStart; year < TrainEnd; year++)
            {
                var point = new TemporalPoint(1) { Sequence = year };
                point.Data[0] = _normalizedForexPair[year];
                result.Points.Add(point);
            }

            result.Generate();
            return result;
        }


        private IMLMethod CreateElmanNetwork(int input)
        {
            // construct an Elman type network
            var pattern = new ElmanPattern
            {
                InputNeurons = input
            };
            pattern.AddHiddenLayer(2);
            pattern.OutputNeurons = 1;
            return pattern.Generate();
        }

        public void Train(BasicNetwork network, IMLDataSet training)
        {
            IMLTrain trainMain = new LevenbergMarquardtTraining(network, training);
            // train the neural network
            var stop = new StopTrainingStrategy();
            var score = new TrainingSetScore(trainMain.Training);
            var trainAlt = new NeuralSimulatedAnnealing(network, score, 10, 2, 100);
            trainMain.AddStrategy(new HybridStrategy(trainAlt));
            trainMain.AddStrategy(stop);

            var epoch = 0;
            while (!stop.ShouldStop() && trainMain.IterationNumber < 1500)
            {
                trainMain.Iteration();
                Console.WriteLine("Training " +  ", Epoch #" + epoch + " Error:" + trainMain.Error);
                epoch++;
            }
        }

        public void Predict(BasicNetwork network)
        {
            Console.WriteLine(@"Year    Actual    Predict     Closed Loop     Predict    Denormalized Value   Real Value");

            for (var year = EvaluateStart; year < EvaluateEnd; year++)
            {
                // calculate based on actual data
                var input = new BasicMLData(WindowSize);
                for (var i = 0; i < input.Count; i++)
                {
                    input[i] = _normalizedForexPair[(year - WindowSize) + i];
                }
                IMLData output = network.Compute(input);
                var prediction = output[0];
                _closedLoopForexPair[year] = prediction;

                // calculate "closed loop", based on predicted data
                for (var i = 0; i < input.Count; i++)
                {
                    input[i] = _closedLoopForexPair[(year - WindowSize) + i];
                }
                output = network.Compute(input);
                var closedLoopPrediction = output[0];

                // display
                Console.WriteLine("{0}  {1}  {2}  {3} Accuracy:{4} Denormalized:{5} Real value:{6}",
                    (StartingYear + year),
                    Format.FormatDouble(_normalizedForexPair[year], 5),
                    Format.FormatDouble(prediction, 5),
                    Format.FormatDouble(closedLoopPrediction, 5),
                    Format.FormatDouble(_normalizedForexPair[year] - prediction, 5),
                    array.Stats.DeNormalize(prediction),
                    ForexPair[year]);
            }
        }
    }
}
