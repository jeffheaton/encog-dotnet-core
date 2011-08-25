using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Util.Simple;
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
        public static double RandomTrainerMethod(int inputs, int predictWindow)
        {
            double[] firstinput = MakeInputs(inputs);
            double[] SecondInput = MakeInputs(inputs);
            double[] ThirdInputs = MakeInputs(inputs);
            double[] FourthInputs = MakeInputs(inputs);
            double[] inp5 = MakeInputs(inputs);
            double[] inp6 = MakeInputs(inputs);

            var pair = SuperUtils.ProcessPair(firstinput, firstinput, inputs, predictWindow);
            var pair2 = SuperUtils.ProcessPair(SecondInput, firstinput, inputs, predictWindow);
            var pair3 = SuperUtils.ProcessPair(ThirdInputs, firstinput, inputs, predictWindow);
            var pair4 = SuperUtils.ProcessPair(FourthInputs, firstinput, inputs, predictWindow);
            var pair5 = SuperUtils.ProcessPair(inp5, firstinput, inputs, predictWindow);
            var pair6 = SuperUtils.ProcessPair(inp6, firstinput, inputs, predictWindow);
            BasicMLDataSet SuperSet = new BasicMLDataSet();
            SuperSet.Add(pair);
            SuperSet.Add(pair2);

            SuperSet.Add(pair3);
            SuperSet.Add(pair4);
            var network = new BasicNetwork();
            network.AddLayer(new BasicLayer(new ActivationTANH(), true, SuperSet.InputSize));
            network.AddLayer(new BasicLayer(new ActivationTANH(), false, 20));
            network.AddLayer(new BasicLayer(new ActivationTANH(), true, 0));
            network.AddLayer(new BasicLayer(new ActivationLinear(), true, predictWindow));

            //var layer = new BasicLayer(new ActivationTANH(), true, SuperSet.InputSize);
            //layer.Network = network;


            network.Structure.FinalizeStructure();
            network.Reset();
           

           // var network = (BasicNetwork)CreateEval.CreateElmanNetwork(SuperSet.InputSize, SuperSet.IdealSize);
            return  CreateEval.TrainNetworks(network, SuperSet);
            //Lets create an evaluation.
            //Console.WriteLine(@"Last error rate on random trainer:" + error);
            
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
