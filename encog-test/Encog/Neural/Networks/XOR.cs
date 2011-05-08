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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using NUnit.Framework;
using Encog.Neural.Networks;
using Encog.Neural.NeuralData;
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
                MLData actual = network.Compute(new BasicMLData(XOR.XOR_INPUT[trainingSet]));

                for (int i = 0; i < XOR.XOR_IDEAL[0].Length; i++)
                {
                    double diff = Math.Abs(actual[i] - XOR.XOR_IDEAL[trainingSet][i]);
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
