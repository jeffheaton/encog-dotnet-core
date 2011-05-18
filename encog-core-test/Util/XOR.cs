// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.Util.Simple;
using Encog.Neural.Networks.Structure;
using Encog.ML;

namespace Encog.Util
{
    public class XOR
    {
        public static double[][] XORInput = {
                                                 new[] {0.0, 0.0}, new[] {1.0, 0.0},
                                                 new[] {0.0, 1.0},
                                                 new[] {1.0, 1.0}
                                             };

        public static double[][] XORIdeal = {
                                                 new[] {0.0}, new[] {1.0}, new[] {1.0},
                                                 new[] {0.0}
                                             };

        public static bool VerifyXOR(MLRegression network, double tolerance)
        {
            for (int trainingSet = 0; trainingSet < XORIdeal.Length; trainingSet++)
            {
                var actual = network.Compute(new BasicMLData(XORInput[trainingSet]));

                for (var i = 0; i < XORIdeal[0].Length; i++)
                {
                    double diff = Math.Abs(actual[i] - XORIdeal[trainingSet][i]);
                    if (diff > tolerance)
                        return false;
                }
            }

            return true;
        }

        public static void TestXORDataSet(MLDataSet set)
        {
            int row = 0;
            foreach (MLDataPair item in set)
            {
                for (int i = 0; i < XORInput[0].Length; i++)
                {
                    Assert.AreEqual(item.Input[i],
                                    XORInput[row][i]);
                }

                for (int i = 0; i < XORIdeal[0].Length; i++)
                {
                    Assert.AreEqual(item.Ideal[i],
                                    XORIdeal[row][i]);
                }

                row++;
            }
        }

        public static BasicNetwork CreateThreeLayerNet()
        {
            var network = new BasicNetwork();
            network.AddLayer(new BasicLayer(2));
            network.AddLayer(new BasicLayer(3));
            network.AddLayer(new BasicLayer(1));
            network.Structure.FinalizeStructure();
            network.Reset();
            return network;
        }

        public static BasicNetwork CreateTrainedXOR()
        {
            double[] TRAINED_XOR_WEIGHTS = { 25.427193285452972, -26.92000502099534, 20.76598054603445, -12.921266548020219, -0.9223427050161919, -1.0588373209475093, -3.80109620509867, 3.1764938777876837, 80.98981535707951, -75.5552829139118, 37.089976176012634, 74.85166823997326, 75.20561368661059, -37.18307123471437, -21.044949631177417, 43.81815044327334, 9.648991753485689 };
            BasicNetwork network = EncogUtility.SimpleFeedForward(2, 4, 0, 1, false);
            NetworkCODEC.ArrayToNetwork(TRAINED_XOR_WEIGHTS, network);
            return network;
        }

        public static BasicNetwork CreateUnTrainedXOR()
        {
            double[] TRAINED_XOR_WEIGHTS = { -0.427193285452972, 0.92000502099534, -0.76598054603445, -0.921266548020219, -0.9223427050161919, -0.0588373209475093, -0.80109620509867, 3.1764938777876837, 0.98981535707951, -0.5552829139118, 0.089976176012634, 0.85166823997326, 0.20561368661059, 0.18307123471437, 0.044949631177417, 0.81815044327334, 0.648991753485689 };
            BasicNetwork network = EncogUtility.SimpleFeedForward(2, 4, 0, 1, false);
            NetworkCODEC.ArrayToNetwork(TRAINED_XOR_WEIGHTS, network);
            return network;
        }

        public static MLDataSet CreateXORDataSet()
        {
            return new BasicMLDataSet(XOR.XORInput, XOR.XORIdeal);
        }

    }
}