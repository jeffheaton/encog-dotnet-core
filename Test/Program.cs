using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Engine.Network.Activation;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Util;
using Encog.Util.Simple;

namespace QuickConsoles
{
    class Program
    {
        static void Main(string[] args)
        {

            //make inputs.
            double[] firstinput = MakeInputs();
            double[] SecondInput = MakeInputs();
            double[] ThirdInputs = MakeInputs();
            double[] FourthInputs = MakeInputs();
            //Make ideal.
            double[] ideal = MakeInputs();

            //put them in jagged arrays.
            double[][] FinalInputs = CreateIdealOrInput(500, firstinput, SecondInput, ThirdInputs, FourthInputs);
            double[][] Ideal = CreateIdealOrInput(500, ideal);

            //make a training.
            var Training = new BasicMLDataSet(FinalInputs, Ideal);

            //make a network.
            var network = new BasicNetwork();
            network.AddLayer(new BasicLayer(new ActivationTANH(), true, 4));
            network.AddLayer(new BasicLayer(new ActivationTANH(), true, 3));
            network.AddLayer(new BasicLayer(new ActivationLinear(), false, 4));
            network.Structure.FinalizeStructure();
            network.Reset();

            //train it
            EncogUtility.TrainConsole(network, Training, 1);


        }
        public static double[] MakeInputs()
        {
            Random rdn = new Random();

            double[] x = new double[500];

            for (int i = 0; i < 500; i++)
            {
                x[i] = rdn.NextDouble();
            }

            return x;
        }

        public static double[][] CreateIdealOrInput(int nbofSeCondDimendsion, params object[] inputs)
        {
            double[][] result = EngineArray.AllocateDouble2D(inputs.Length, nbofSeCondDimendsion);
            int i = 0, k = 0;
            foreach (double[] doubles in inputs)
            {
                foreach (double d in doubles)
                {
                    result[i][k] = d;
                    k++;
                }
                if (i < inputs.Length - 1)
                {
                    i++;
                    k = 0;
                }
            }
            return result;
        }
    }
}
