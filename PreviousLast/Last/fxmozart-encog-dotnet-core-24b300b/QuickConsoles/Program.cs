using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
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
            double [] firstinput = MakeInputs();
            double[] SecondInput = MakeInputs();
            double[] ThirdInputs = MakeInputs();
            double[] FourthInputs = MakeInputs();
            //Make ideal.
            double[] ideal = MakeInputs();
            //put them in jagged arrays.
            double[][] FinalInputs = CreateIdealOrInput(500, firstinput, SecondInput, ThirdInputs, FourthInputs);
            double[][] Ideal = CreateIdealOrInput(500, ideal,ideal,ideal,ideal);
            //make a training.
            var Training = new BasicMLDataSet();

            IMLData data1 = new BasicMLData(firstinput);
            IMLData data2 = new BasicMLData(SecondInput);
            IMLData data3 = new BasicMLData(ThirdInputs);
            IMLData data4 = new BasicMLData(FourthInputs);

            IMLData IdealData = new BasicMLData(ideal);
            IMLDataPair pairs = new BasicMLDataPair(data1, IdealData);
            IMLDataPair pairs2 = new BasicMLDataPair(data2, IdealData);
            IMLDataPair pairs3 = new BasicMLDataPair(data3, IdealData);
            IMLDataPair pairs4 = new BasicMLDataPair(data4, IdealData);


            IMLDataPair pairsa =ProcessDoubleSerieIntoIMLDataset(firstinput, 500, 1);
            IMLDataPair pairsb = ProcessDoubleSerieIntoIMLDataset(SecondInput, 500, 1);
            IMLDataPair pairsc = ProcessDoubleSerieIntoIMLDataset(ThirdInputs, 500, 1);

            List<IMLDataPair> listData = new List<IMLDataPair>();

            listData.Add(pairsa);
            listData.Add(pairsb);
            listData.Add(pairsc);
            //listData.Add(pairs4);


            var trains = new BasicMLDataSet(listData);
            
            //make a network.
            var network = new BasicNetwork();
            network.AddLayer(new BasicLayer(new ActivationTANH(), true, 3));
            network.AddLayer(new BasicLayer(new ActivationTANH(), true, 3));
            network.AddLayer(new BasicLayer(new ActivationLinear(), false, 1));
            network.Structure.FinalizeStructure();
            network.Reset();
            //train it
            EncogUtility.TrainConsole(network, trains, 1);
        }
        public static double[] MakeInputs()
        {
            Random rdn = new Random();

            double [] x = new double[500];
            for (int i = 0; i < 500; i++)
            {
                x[i] = rdn.NextDouble();
            }
            return x;
        }



        /// <summary>
        /// Processes the specified double serie into an IMLDataset.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="_inputWindow">The _input window.</param>
        /// <param name="_predictWindow">The _predict window.</param>
        /// <returns></returns>
        public static IMLDataPair ProcessDoubleSerieIntoIMLDataset(double[] data, int _inputWindow, int _predictWindow)
        {
           
            int totalWindowSize = _inputWindow + _predictWindow;
            int stopPoint = data.Length - totalWindowSize;
            for (int i = 0; i < stopPoint; i++)
            {
                IMLData inputData = new BasicMLData(_inputWindow);
                IMLData idealData = new BasicMLData(_predictWindow);
                int index = i;
                // handle input window
                for (int j = 0; j < _inputWindow; j++)
                {
                    inputData[j] = data[index++];
                }
                // handle predict window
                for (int j = 0; j < _predictWindow; j++)
                {
                    idealData[j] = data[index++];
                }
                var pair = new BasicMLDataPair(inputData, idealData);
                return pair;
            }
            return null;
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
