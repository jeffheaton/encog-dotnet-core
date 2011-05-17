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

        public static bool VerifyXOR(BasicNetwork network, double tolerance)
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
    }
}