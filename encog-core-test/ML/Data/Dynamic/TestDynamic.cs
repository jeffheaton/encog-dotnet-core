//
// Encog(tm) Core v3.1 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2012 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.Collections.Generic;
using Encog.Engine.Network.Activation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.Neural.Flat;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using System.Diagnostics;
using Encog.Util;

namespace Encog.ML.Data.Dynamic
{
    [TestClass]
    public class TestDynamic
    {
		[TestMethod]
		public void SimpleSlidingParameters()
		{
			var list = new List<double>(20);
			for(int i = 0; i < 20; i++) list.Add(i);

			var input = new SlidingWindowMLDataProvider(list, 10, -5, 3);

			Assert.AreEqual(6, input.Count);
			Assert.AreEqual(0.0, input[0, 0]);
			Assert.AreEqual(0.0, input[0, 5]);
			Assert.AreEqual(4.0, input[0, 9]);
			Assert.AreEqual(17.0, input[6, 4]);
			Assert.AreEqual(0.0, input[6, 9]);

			input.PadWithNearest = true;
			Assert.AreEqual(19.0, input[6, 9]);
			Assert.AreEqual(19.0, input[6, 90]); // no range error presently

			input.DefaultPadValue = -1.0;
			input.PadWithNearest = false;
			Assert.AreEqual(-1.0, input[0, 0]);
			Assert.AreEqual(-1.0, input[6, 9]);
		}

		[TestMethod]
		public void SimpleSlidingGap()
		{
			var list = new List<double>(20);
			for(int i = 0; i < 20; i++) list.Add(i);

			var input = new SlidingWindowMLDataProvider(list, 12, 0, 1, 4);

			Assert.AreEqual(20, input.Count);
			Assert.AreEqual(3, input.Size);
			Assert.AreEqual(0.0, input[0, 0]);
			Assert.AreEqual(4.0, input[0, 1]);
			Assert.AreEqual(8.0, input[0, 2]);
			Assert.AreEqual(1.0, input[1, 0]);

			Assert.AreEqual(19.0, input[19, 0]);
			Assert.AreEqual(0.0, input[19, 1]);
		}

		[TestMethod]
        public void BasicSlidingSineSignal()
        {
			var listSize = 30 * 200;
			var inputList = new List<double>(listSize);
			var idealList = new List<double>(listSize);
			var rand = new Random(23);
			for(int i = 0; i < listSize; i++)
			{
				idealList.Add(Math.Sin(Math.PI * 2.0 * i / 30));
				inputList.Add(idealList[idealList.Count - 1] + (rand.NextDouble() - 0.5) * 0.1);
			}

			var input = new SlidingWindowMLDataProvider(inputList, 10, 0, 1);
			var ideal = new SlidingWindowMLDataProvider(idealList, 2, 11, 1); // predecit the eleventh, twelth item from the ten previous to it
			var ds = new DynamicMLDataSet(input, ideal);

			Assert.AreEqual(10, input.WindowSize);
			Assert.AreEqual(10, ds.InputSize);
			Assert.AreEqual(2, ds.IdealSize);
			Assert.AreEqual(listSize, ds.Count);

			var network = new BasicNetwork();
			network.AddLayer(new BasicLayer(ds.InputSize));
			network.AddLayer(new BasicLayer(ds.InputSize + 3));
			network.AddLayer(new BasicLayer(ds.IdealSize));
			network.Structure.FinalizeStructure();
			network.Reset(42);

			var trainer = new Encog.Neural.Networks.Training.Propagation.Resilient.ResilientPropagation(network, ds);

			int maxIteration = 300;
			int iteration = 0;
			do
			{
				trainer.Iteration();
				Debug.WriteLine(++iteration + ": Error = " + trainer.Error);
			} while(trainer.Error > 0.001 && maxIteration > iteration);

			Assert.IsTrue(iteration < maxIteration);
		}

		[TestMethod]
		public void TestDynamicXOR()
		{

			Func<int, int, double> inputFunc = OnInputFunc;
			Func<int, int, double> idealFunc = delegate(int chunk, int index) { return XOR.XORIdeal[chunk][index]; };
			var input = new FuncMLDataProvider(inputFunc, XOR.XORInput.Length, XOR.XORInput[0].Length);
			var ideal = new FuncMLDataProvider(idealFunc, XOR.XORIdeal.Length, XOR.XORIdeal[0].Length);

			var ds = new DynamicMLDataSet(input, ideal);

			var network = new BasicNetwork();
			network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, ds.InputSize));
			network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, ds.InputSize + 5));
			network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, ds.IdealSize));
			network.Structure.FinalizeStructure();
			network.Reset(42);

			var trainer = new Encog.Neural.Networks.Training.Propagation.Resilient.ResilientPropagation(network, ds);

			int maxIteration = 300;
			int iteration = 0;
			do
			{
				trainer.Iteration();
				Debug.WriteLine(++iteration + ": Error = " + trainer.Error);
			} while(trainer.Error > 0.0001 && maxIteration > iteration);

			Assert.IsTrue(iteration < maxIteration);

		}

        private double OnInputFunc(int chunk, int index)
        {
            return XOR.XORInput[chunk][index];
        }
    }
}
