using System;
using ConsoleExamples.Examples;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Temporal;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks.Training.Lma;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Pattern;
using Encog.Util;
using Encog.Util.Arrayutil;

namespace Encog.Examples.ForexExample
{
    public class ForexExample : IExample
    {
        public const int StartingYear = 1700;
        public const int WindowSize = 30;
        public const int TrainStart = WindowSize;
        public const int TrainEnd = 259;
        public const int EvaluateStart = 2000;

        public const double MaxError = 0.01;

        public static double[] Sunspots = {0};
        public static void GetSunSpots()
        {

          Sunspots=  Encog.Util.NetworkUtil.QuickCSVUtils.QuickParseCSV("DB!EURUSD.Bar.Time.600.csv", "Close", 2500).ToArray();
            Console.WriteLine("Retrieved :" + Sunspots.Length + " Closing values");
            Console.WriteLine("Press a key to continue");
            Console.Read();
        }
        public static int EvaluateEnd = Sunspots.Length - 1;

        private double[] _closedLoopSunspots;
        private double[] _normalizedSunspots;

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(ForexExample),
                    "Forex",
                    "Predict Forex rates via CSV.",
                    "Use a feedforward neural network to predict sunspots.");
                return info;
            }
        }

        #region IExample Members

        public void Execute(IExampleInterface app)
        {
            GetSunSpots();
            EvaluateEnd = EvaluateStart + 100;
            NormalizeSunspots(-1, 1);
            BasicNetwork network = (BasicNetwork)CreateElmanNetwork(WindowSize);
            IMLDataSet training = GenerateTraining();
            Train(network, training);
            Predict(network);
        }

        #endregion

        private NormalizeArray array;

        public void NormalizeSunspots(double lo, double hi)
        {
            array= new NormalizeArray { NormalizedHigh = hi, NormalizedLow = lo };

            // create arrays to hold the normalized sunspots
            _normalizedSunspots = array.Process(Sunspots);
            _closedLoopSunspots = EngineArray.ArrayCopy(_normalizedSunspots);
        }


        public IMLDataSet GenerateTraining()
        {
            var result = new TemporalMLDataSet(WindowSize, 1);

            var desc = new TemporalDataDescription(TemporalDataDescription.Type.Raw, true, true);
            result.AddDescription(desc);

            for (ulong year = TrainStart; year < TrainEnd; year++)
            {
                var point = new TemporalPoint(1) { Sequence = year };
                point.Data[0] = _normalizedSunspots[year];
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

            ICalculateScore score = new TrainingSetScore(trainMain.Training);
            IMLTrain trainAlt = new NeuralSimulatedAnnealing(network, score, 10, 2, 100);
            trainMain.AddStrategy(new HybridStrategy(trainAlt));
            trainMain.AddStrategy(stop);

            int epoch = 0;
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

            for (int year = EvaluateStart; year < EvaluateEnd; year++)
            {
                // calculate based on actual data
                IMLData input = new BasicMLData(WindowSize);
                for (var i = 0; i < input.Count; i++)
                {
                    input.Data[i] = _normalizedSunspots[(year - WindowSize) + i];
                }
                IMLData output = network.Compute(input);
                double prediction = output.Data[0];
                _closedLoopSunspots[year] = prediction;

                // calculate "closed loop", based on predicted data
                for (var i = 0; i < input.Count; i++)
                {
                    input.Data[i] = _closedLoopSunspots[(year - WindowSize) + i];
                }
                output = network.Compute(input);
                double closedLoopPrediction = output.Data[0];
               
                // display
                Console.WriteLine((StartingYear + year)
                                  + @"  " + Format.FormatDouble(_normalizedSunspots[year], 5)
                                  + @"  " + Format.FormatDouble(prediction, 5)
                                  + @"  " + Format.FormatDouble(closedLoopPrediction, 5)
                                  + @" Accuracy:" +
                                  Format.FormatDouble(_normalizedSunspots[year] - prediction, 5)
                                  + " Denormalized:" + array.Stats.DeNormalize(prediction)
                                  + " Real value:" + Sunspots[year]);



            }
        }
    }
}