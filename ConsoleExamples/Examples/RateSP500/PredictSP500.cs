using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleExamples.Examples;
using Encog.Engine.Network.Activation;
using Encog.Examples.RateSP500;
using Encog.MathUtil.Error;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Pattern;
using Encog.Util;
using Encog.Util.CSV;
using Encog.Neural;
using Encog.Util.Simple;
using SuperUtils = Encog.Util.NetworkUtil.NetworkUtility;
namespace Encog.Examples.RateSP500
{
    public class PredictSP500 :IExample
    {

        public const int TRAINING_SIZE = 500;
        public const int INPUT_SIZE = 10;
        public const int OUTPUT_SIZE = 1;
        public const int NEURONS_HIDDEN_1 = 20;
        public const int NEURONS_HIDDEN_2 = 0;
        public const double MAX_ERROR = 0.04;
        public DateTime PREDICT_FROM = ReadCSV.ParseDate("2007-01-01", "yyyy-MM-dd");
        public DateTime LEARN_FROM = ReadCSV.ParseDate("1980-01-01", "yyyy-MM-dd");

        //public PredictSP500
        //{
        //    PredictSP500 predict = new PredictSP500();
        //    //if (args.Length > 0 && args[0].Equals("full", StringComparison.CurrentCultureIgnoreCase))
        //    //    predict.run(true);
        //    //else
        //    //    predict.run(false);


        //    Console.ReadKey();
        //}

        private double[][] input;

        private double[][] ideal;
        private BasicNetwork network;

        private SP500Actual actual;

        public BasicNetwork createNetwork()
        {
            BasicNetwork net = (BasicNetwork)CreateFeedforwardNetwork(INPUT_SIZE, OUTPUT_SIZE, NEURONS_HIDDEN_1);

            return net;
        }


        private static IMLMethod CreateFeedforwardNetwork(int inputs, int outputs, int hidden)
        {
            // construct a feedforward type network
            var pattern = new FeedForwardPattern();
            pattern.ActivationFunction = new ActivationSigmoid();
            pattern.InputNeurons = inputs;
            pattern.AddHiddenLayer(outputs);
            pattern.OutputNeurons = hidden;
            return pattern.Generate();
        }



        public void display()
        {


            double[] present = new double[INPUT_SIZE * 2];
            double[] predict = new double[OUTPUT_SIZE];
            double[] actualOutput = new double[OUTPUT_SIZE];

            int index = 0;
            foreach (FinancialSample sample in this.actual.getSamples())
            {
                if (sample.getDate().CompareTo(this.PREDICT_FROM) > 0)
                {
                    StringBuilder str = new StringBuilder();
                    str.Append(ReadCSV.ParseDate(sample.getDate().ToShortDateString(), "yyyy-MM-dd"));
                    str.Append(":Start=");
                    str.Append(sample.getAmount());

                    this.actual.getInputData(index - INPUT_SIZE, present);
                    this.actual.getOutputData(index - INPUT_SIZE, actualOutput);
                    IMLData data = new BasicMLData(present);

                    IMLData Output = network.Compute(data);
                    predict = Output.Data;

                    str.Append(",Actual % Change=");
                    str.Append(actualOutput[0].ToString("N2"));
                    str.Append(",Predicted % Change= ");
                    str.Append(predict[0].ToString("N2"));

                    str.Append(":Difference=");

                    ErrorCalculation error = new ErrorCalculation();
                    error.UpdateError(Output.Data, actualOutput, 1);
                    str.Append(error.CalculateRMS().ToString("N2"));

                    // 

                    Console.WriteLine(str.ToString());


                }

                index++;
            }
        }

        private void generateTrainingSets()
        {
            this.input = new double[TRAINING_SIZE][];//[INPUT_SIZE * 2];
            this.ideal = new double[TRAINING_SIZE][];//[OUTPUT_SIZE];

            // find where we are starting from
            int startIndex = this.actual.getSamples().TakeWhile(sample => sample.getDate().CompareTo(this.LEARN_FROM) <= 0).Count();

            // create a sample factor across the training area
            int eligibleSamples = TRAINING_SIZE - startIndex;
            if (eligibleSamples == 0)
            {
                Console.WriteLine("Need an earlier date for LEARN_FROM or a smaller number for TRAINING_SIZE.");
                return;
            }
            int factor = eligibleSamples / TRAINING_SIZE;

            // grab the actual training data from that point
            for (int i = 0; i < TRAINING_SIZE; i++)
            {
                this.input[i] = new double[INPUT_SIZE * 2];
                this.ideal[i] = new double[OUTPUT_SIZE];
                this.actual.getInputData(startIndex + (i * factor), this.input[i]);
                this.actual.getOutputData(startIndex + (i * factor), this.ideal[i]);
            }
        }

        public void loadNeuralNetwork()
        {
            this.network = (BasicNetwork)SerializeObject.Load("sp500.net");
        }

        public void run(bool full)
        {
            //try
            //{
            this.actual = new SP500Actual(INPUT_SIZE, OUTPUT_SIZE);
            this.actual.load("sp500.csv", "prime.csv");

            Console.WriteLine("Samples read: " + this.actual.size());

            if (full)
            {
                createNetwork();
                generateTrainingSets();

                trainNetworkBackprop();

                saveNeuralNetwork();
            }
            else
            {
                loadNeuralNetwork();
            }

            display();

            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //    Console.WriteLine(e.StackTrace);
            // }
        }

        public void saveNeuralNetwork()
        {
            SerializeObject.Save("sp500.net", this.network);
        }

        private void trainNetworkBackprop()
        {
            // IMLTrain train = new Backpropagation(this.network, this.input,this.ideal, 0.000001, 0.1);

            IMLDataSet aset = new BasicMLDataSet(input, ideal);
            IMLTrain train = new Backpropagation(network, aset);

            double lastError = Double.MaxValue;
            int epoch = 1;
            int lastAnneal = 0;
            // train the neural network
            ICalculateScore score = new TrainingSetScore(aset);
            IMLTrain trainAlt = new NeuralSimulatedAnnealing(network, score, 10, 2, 100);


            IMLTrain trainMain = new Backpropagation(network, aset, 0.001, 0.0);

            StopTrainingStrategy stop = new StopTrainingStrategy();
            trainMain.AddStrategy(new Greedy());
            trainMain.AddStrategy(new HybridStrategy(trainAlt));
            trainMain.AddStrategy(stop);


            while (!stop.ShouldStop())
            {
                trainMain.Iteration();
                Console.WriteLine(@"Training " + @"Epoch #" + epoch + @" Error:" + trainMain.Error);
                epoch++;
            }



        }

        public void Execute(IExampleInterface app)
        {
            PredictSP500 predict = new PredictSP500();
            if (app.Args.Length > 0 && app.Args[0].Equals("full", StringComparison.CurrentCultureIgnoreCase))
                predict.run(true);
            else
                predict.run(false);


            Console.ReadKey();
        }


        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(PredictSP500),
                    "SP500",
                    "Predicts the SNP500",
                    "Reads CSV and predicts the SNP 500.");
                return info;
            }
        }
    }
}