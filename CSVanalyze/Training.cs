using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks;
using Encog.ML.Data;
using CSVanalyze.Properties;
using Encog.ML.Train;
using Encog.Neural.Networks.Training.Anneal;
using Encog.ML.Data.Market;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training;

namespace CSVanalyze
{
    public class Training
    {


        #region trainers


        /// <summary>
        /// Trains the network with the specified training set.
        /// you can specify if you want to use the greed strategy by the boolean UseGreed.
        /// </summary>
        /// <param name="what">The what.</param>
        /// <param name="network">The network.</param>
        /// <param name="trainingSet">The training set.</param>
        /// <param name="UseGreed">if set to <c>true</c> [use greed].</param>
        /// <returns></returns>
        public static BasicNetwork TrainNetwork(String what, BasicNetwork network, IMLDataSet trainingSet, bool UseGreed)
        {
            // train the neural network
            ICalculateScore score = new TrainingSetScore(trainingSet);


            IMLTrain trainAlt = new NeuralSimulatedAnnealing(network, score, 10, 2, 10);
            IMLTrain trainMain = new Backpropagation(network, trainingSet, 0.00001, 0.001);
            StopTrainingStrategy stop = new StopTrainingStrategy(0.00000001, 200);
            if (UseGreed)
            {
                trainMain.AddStrategy(new Greedy());
            }
            trainMain.AddStrategy(new HybridStrategy(trainAlt, 0.00001, 5, 10));
            trainMain.AddStrategy(stop);

            int epoch = 0;
            int geneticIterations = 0;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            sw.Start();
            while (!stop.ShouldStop() && trainMain.IterationNumber < Settings.Default.MaxEpochs && sw.Elapsed.TotalMinutes < Settings.Default.MaxTrainingTime)
            {

                if (geneticIterations < trainAlt.IterationNumber)
                    Console.WriteLine("Genetic Trained #" + (trainAlt.IterationNumber - geneticIterations));

                geneticIterations = trainAlt.IterationNumber;

                trainMain.Iteration();

                Console.WriteLine("Training " + what + ", Epoch #" + epoch + " Error:" + trainMain.Error + " Train Genetic #" + trainAlt.IterationNumber);
                epoch++;
            }
            trainMain.FinishTraining();
            sw.Stop();

            Console.WriteLine("Trained the network " + what + " for :" + trainMain.IterationNumber + " Epochs " + " Elapsed Minutes:" + sw.Elapsed.TotalMinutes);
            return network;
        }

    }
        #endregion

}
