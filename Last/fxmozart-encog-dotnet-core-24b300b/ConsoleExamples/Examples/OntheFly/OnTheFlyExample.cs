using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ConsoleExamples.Examples;
using Encog.Engine.Network.Activation;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Util;
using Encog.Util.NetworkUtil;

namespace Encog.Examples.OntheFly
{
    class OnTheFlyExample :IExample
    {

        private IExampleInterface app;

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(OnTheFlyExample),
                    "onthefly",
                    "Gets many inputs and starts a network analysis",
                    "Shows how to get many different inputs and train a network.");
                return info;
            }
        }

        public void Execute(IExampleInterface app)
        {
            BuildNetwork();

        }

        private static void BuildNetwork()
        {
            const string fileName = "c:\\EURUSD_Hourly_Bid_1999.03.04_2011.03.14.csv";
            List<double> Opens = NetworkUtility.QuickParseCSV(fileName, "Open", 1200);
            List<double> High = NetworkUtility.QuickParseCSV(fileName, "High", 1200);
            List<double> Low = NetworkUtility.QuickParseCSV(fileName, "Low", 1200);
            List<double> Close = NetworkUtility.QuickParseCSV(fileName, "Close", 1200);



        }

        private static void BuildNetwork(BasicMLDataSet trains)
        {
             //make a network.
            var network = new BasicNetwork();
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true,
                                            trains.InputSize));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true,
                                            20));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false,
                                            trains.IdealSize));
            network.Structure.FinalizeStructure();
            network.Reset();
            //train it
            // train the neural network
            IMLTrain train = new Backpropagation(network, trains, 0.7, 0.7);
            var sw = new Stopwatch();
            sw.Start();
            // run epoch of learning procedure
            for (int i = 0; i < 3000; i++)
            {
                train.Iteration();
                Console.WriteLine(@"Iteration #:"+ train.IterationNumber+ @" Current Error Rate:"   +train.Error );
            }
            sw.Stop();

            Console.WriteLine(@"Elapsed : " + sw.ElapsedMilliseconds);
            
        }

        private static double[][] GenerateNews(int columns, params double[] inputs)
        {
            double[][] result = EngineArray.AllocateDouble2D(inputs.Length, columns);

            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    result[i][j] = inputs[j];
                }
            }
            return result;
        }
    }
}
