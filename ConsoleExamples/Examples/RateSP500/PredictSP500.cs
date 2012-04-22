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
#region

using System;
using System.Linq;
using System.Text;
using ConsoleExamples.Examples;
using Encog.Engine.Network.Activation;
using Encog.MathUtil.Error;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.NEAT;
using Encog.Neural.NEAT.Training;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Pattern;
using Encog.Util;
using Encog.Util.CSV;

#endregion

namespace Encog.Examples.RateSP500
{
    public class PredictSP500 : IExample
    {
        public const int TRAINING_SIZE = 500;
        public const int INPUT_SIZE = 20;
        public const int OUTPUT_SIZE = 1;
        public const int NEURONS_HIDDEN_1 = 20;
        public const int NEURONS_HIDDEN_2 = 0;
        public const double MAX_ERROR = 0.04;
        public DateTime LEARN_FROM = ReadCSV.ParseDate("1980-01-01", "yyyy-MM-dd");
        public DateTime PREDICT_FROM = ReadCSV.ParseDate("2007-01-01", "yyyy-MM-dd");
        private SP500Actual actual;

        //public PredictSP500
        //{
        //    PredictSP500 predict = new PredictSP500();
        //    //if (args.Length > 0 && args[0].Equals("full", StringComparison.CurrentCultureIgnoreCase))
        //    //    predict.run(true);
        //    //else
        //    //    predict.run(false);


        //    Console.ReadKey();
        //}

        private double[][] ideal;
        private double[][] input;
        private BasicNetwork network;

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof (PredictSP500),
                    "SP500",
                    "Predicts the SNP500",
                    "Reads CSV and predicts the SNP 500.");
                return info;
            }
        }

        #region IExample Members

        public void Execute(IExampleInterface app)
        {
            PredictSP500 predict = new PredictSP500();
            if (app.Args.Length > 0 && app.Args[0].Equals("full"))
                predict.run(true);
            else
                predict.run(false);


            Console.ReadKey();
        }

        #endregion

        public BasicNetwork createNetwork()
        {
            BasicNetwork net = (BasicNetwork) CreateFeedforwardNetwork(INPUT_SIZE*2,OUTPUT_SIZE, NEURONS_HIDDEN_1);

            return net;
        }


        private static IMLMethod CreateFeedforwardNetwork(int inputs, int outputs, int hidden)
        {
            // construct a feedforward type network
            var pattern = new FeedForwardPattern();
            pattern.ActivationFunction = new ActivationSigmoid();
            pattern.InputNeurons = inputs;
            pattern.AddHiddenLayer(hidden);
            pattern.OutputNeurons = outputs;
            return pattern.Generate();
        }


        public void display()
        {
            double[] present = new double[INPUT_SIZE*2];
            double[] actualOutput = new double[OUTPUT_SIZE];

            int index = 0;
            foreach (FinancialSample sample in actual.getSamples())
            {
                if (sample.getDate().CompareTo(PREDICT_FROM) > 0)
                {
                    StringBuilder str = new StringBuilder();
                    str.Append(sample.getDate());
                    str.Append(":Start=");
                    str.Append(sample.getAmount());

                    actual.getInputData(index - INPUT_SIZE, present);
                    actual.getOutputData(index - INPUT_SIZE, actualOutput);
                    IMLData data = new BasicMLData(present);

                    IMLData Output = network.Compute(data);

                    str.Append(",Actual % Change=");
                    str.Append(actualOutput[0].ToString("N2"));
                    str.Append(",Predicted % Change= ");
                    str.Append(Output[0].ToString("N2"));

                    str.Append(":Difference=");

                    ErrorCalculation error = new ErrorCalculation();
                    error.UpdateError(Output, actualOutput, 1);
                    str.Append(error.CalculateRMS().ToString("N2"));
                    // 
                    Console.WriteLine(str.ToString());
                }

                index++;
            }
        }

        private void generateTrainingSets()
        {
            input = new double[TRAINING_SIZE][]; //[INPUT_SIZE * 2];
            ideal = new double[TRAINING_SIZE][]; //[OUTPUT_SIZE];

            // find where we are starting from
            if (actual == null) return;
            int startIndex = actual.getSamples().TakeWhile(sample => sample.getDate().CompareTo(LEARN_FROM) <= 0).Count();

            // create a sample factor across the training area
            int eligibleSamples = TRAINING_SIZE - startIndex;
            if (eligibleSamples == 0)
            {
                Console.WriteLine(@"Need an earlier date for LEARN_FROM or a smaller number for TRAINING_SIZE.");
                return;
            }
            int factor = eligibleSamples/TRAINING_SIZE;

            // grab the actual training data from that point
            for (int i = 0; i < TRAINING_SIZE; i++)
            {
                input[i] = new double[INPUT_SIZE*2];
                ideal[i] = new double[OUTPUT_SIZE];
                actual.getInputData(startIndex + (i*factor), input[i]);
                actual.getOutputData(startIndex + (i*factor), ideal[i]);
            }
        }

        public void loadNeuralNetwork()
        {
            network = (BasicNetwork) SerializeObject.Load("sp500.net");
        }

        public void run(bool full)
        {
           try
           {
               actual = new SP500Actual(INPUT_SIZE, OUTPUT_SIZE);
            actual.load("sp500.csv", "prime.csv");

            Console.WriteLine(@"Samples read: " + actual.size());

            if (full)
            {
               network= createNetwork();
                generateTrainingSets();

                trainNetworkBackprop();

                saveNeuralNetwork();
            }
            else
            {
                loadNeuralNetwork();
            }

            display();

           }
           catch (Exception e)
           {
               Console.WriteLine(e);
               Console.WriteLine(e.StackTrace);
           }
        }

        public void saveNeuralNetwork()
        {
            SerializeObject.Save("sp500.net", network);
        }

        private void trainNetworkBackprop()
        {
            // IMLTrain train = new Backpropagation(this.network, this.input,this.ideal, 0.000001, 0.1);

            IMLDataSet aset = new BasicMLDataSet(input, ideal);
            int epoch = 1;
            // train the neural network
            ICalculateScore score = new TrainingSetScore(aset);
            IMLTrain trainAlt = new NeuralSimulatedAnnealing(network, score, 10, 2, 100);
            IMLTrain trainMain = new Backpropagation(network, aset, 0.001, 0.0);
            StopTrainingStrategy stop = new StopTrainingStrategy();
            var pop = new NEATPopulation(INPUT_SIZE, OUTPUT_SIZE, 1000);
            // train the neural network
            var step = new ActivationStep();
            step.Center = 0.5;
            pop.OutputActivationFunction = step;
            var train = new NEATTraining(score, pop);
            trainMain.AddStrategy(new Greedy());
            trainMain.AddStrategy(new HybridStrategy(trainAlt));
            trainMain.AddStrategy(stop);
            trainMain.AddStrategy(new HybridStrategy(train));


            network.ClearContext();

            while (!stop.ShouldStop())
            {
                trainMain.Iteration();
                train.Iteration();
                Console.WriteLine(@"Training " + @"Epoch #" + epoch + @" Error:" + trainMain.Error+ @" Genetic iteration:"+trainAlt.IterationNumber+ @"neat iteration:"+train.IterationNumber );
                epoch++;
            }
        }



        public void StartPrediction()
        {
            
        }

        #region Direction enum

        public enum Direction
        {
            Up,
            Down
        } ;

        #endregion

        public static Direction DetermineDirection(double d)
        {
            return d < 0 ? Direction.Down : Direction.Up;
        }

    }
}
