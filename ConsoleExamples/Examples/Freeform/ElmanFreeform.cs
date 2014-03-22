using System;
using ConsoleExamples.Examples;
using Encog.Engine.Network.Activation;
using Encog.Examples.Util;
using Encog.ML.Data;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Freeform;
using Encog.Neural.Freeform.Training;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Util.Simple;

namespace Encog.Examples.Freeform
{
    public class ElmanFreeform: IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(ElmanFreeform),
                    "freeform-elman",
                    "Freeform Network: Elman SRN",
                    "Create a an Elman SRN network using freeform");
                return info;
            }
        }


       public static double TrainNetwork(String what,
            FreeformNetwork network, IMLDataSet trainingSet) {
    	ICalculateScore score = new TrainingSetScore(trainingSet);
    	
		IMLTrain trainAlt = new NeuralSimulatedAnnealing(
				network, score, 10, 2, 100);

		IMLTrain trainMain = new FreeformBackPropagation(network, trainingSet,0.00001, 0.0);

		StopTrainingStrategy stop = new StopTrainingStrategy();
		trainMain.AddStrategy(new Greedy());
		trainMain.AddStrategy(new HybridStrategy(trainAlt));
		trainMain.AddStrategy(stop);
		
		EncogUtility.TrainToError(trainMain, 0.01);
    	
        return trainMain.Error;
    }


        public void Execute(IExampleInterface app)
        {
            TemporalXOR temp = new TemporalXOR();
            IMLDataSet trainingSet = temp.Generate(120);

            FreeformNetwork elmanNetwork = FreeformNetwork.CreateElman(1, 6, 1, new ActivationSigmoid());
            FreeformNetwork feedforwardNetwork = FreeformNetwork.CreateFeedforward(1, 6, 0, 1, new ActivationSigmoid());

            double feedforwardError = TrainNetwork("feedforward",feedforwardNetwork,trainingSet);
            double elmanError = TrainNetwork("elman",elmanNetwork,trainingSet);

            Console.WriteLine(@"Best error rate with Elman Network: " + elmanError);
            Console.WriteLine(@"Best error rate with Feedforward Network: "
                + feedforwardError);
            Console.WriteLine(@"Elman should be able to get into the 10% range,\nfeedforward should not go below 25%.\nThe recurrent Elment net can learn better in this case.");
            Console.WriteLine(@"If your results are not as good, try rerunning, or perhaps training longer.");

            EncogFramework.Instance.Shutdown();
        }
    }
}
