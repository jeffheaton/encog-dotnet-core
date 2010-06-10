using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Encog.Neural.Networks;
using Encog.Neural.NeuralData;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Data;

namespace encog_test.Neural.Networks
{
    public class XOR
    {
        public static double[][] XOR_INPUT = {
                                             new double[2]{ 0.0, 0.0 }, new double[2]{ 1.0, 0.0 }, new double[2]{ 0.0, 1.0 },
				new double[2]{ 1.0, 1.0 } };

        public static double[][] XOR_IDEAL = { new double[1] { 0.0 }, new double[1] { 1.0 }, new double[1] { 1.0 }, new double[1] { 0.0 } };

        public static bool VerifyXOR(BasicNetwork network, double tolerance)
        {
            for (int trainingSet = 0; trainingSet < XOR.XOR_IDEAL.Length; trainingSet++)
            {
                INeuralData actual = network.Compute(new BasicNeuralData(XOR.XOR_INPUT[trainingSet]));

                for (int i = 0; i < XOR.XOR_IDEAL[0].Length; i++)
                {
                    double diff = Math.Abs(actual[i] - XOR.XOR_IDEAL[trainingSet][i]);
                    if (diff > tolerance)
                        return false;
                }

            }

            return true;
        }

        public static void TestXORDataSet(INeuralDataSet set)
        {
            int row = 0;
            foreach (INeuralDataPair item in set)
            {
                for (int i = 0; i < XOR.XOR_INPUT[0].Length; i++)
                {
                    Assert.AreEqual(item.Input[i],
                            XOR.XOR_INPUT[row][i]);
                }

                for (int i = 0; i < XOR.XOR_IDEAL[0].Length; i++)
                {
                    Assert.AreEqual(item.Ideal[i],
                            XOR.XOR_IDEAL[row][i]);
                }

                row++;
            }
        }

        public static BasicNetwork CreateThreeLayerNet()
        {
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(2));
            network.AddLayer(new BasicLayer(3));
            network.AddLayer(new BasicLayer(1));
            network.Structure.FinalizeStructure();
            network.Reset();
            return network;
        }
    }
}
