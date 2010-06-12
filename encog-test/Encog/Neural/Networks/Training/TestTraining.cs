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
using NUnit.Framework;
using Encog.Util.Logging;
using Encog.Neural.NeuralData;
using encog_test.Neural.Networks;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training.Propagation.Manhattan;
using Encog.Neural.Networks.Training.Propagation.SCG;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks.Training.Genetic;
using Encog.MathUtil.Randomize;
using Encog.Neural.Networks.Training.Propagation;

namespace encog_test.Encog.Neural.Networks.Training
{
    [TestFixture]
    public class TestTraining
    {
        	[Test]
	public void TestRPROP()
	{
		Logging.StopConsoleLogging();
		INeuralDataSet trainingData = new BasicNeuralDataSet(XOR.XOR_INPUT,XOR.XOR_IDEAL);
		
		BasicNetwork network = NetworkUtil.CreateXORNetworkUntrained();
		ITrain rprop = new ResilientPropagation(network, trainingData);
		NetworkUtil.TestTraining(rprop,0.03);
	}
	
	[Test]
	public void TestBPROP() 
	{
		Logging.StopConsoleLogging();
		INeuralDataSet trainingData = new BasicNeuralDataSet(XOR.XOR_INPUT,XOR.XOR_IDEAL);
		
		BasicNetwork network = NetworkUtil.CreateXORNetworkUntrained();
		ITrain bprop = new Backpropagation(network, trainingData, 0.7, 0.9);
		NetworkUtil.TestTraining(bprop,0.01);
	}
	
	[Test]
	public void TestManhattan()
	{
		Logging.StopConsoleLogging();
		INeuralDataSet trainingData = new BasicNeuralDataSet(XOR.XOR_INPUT,XOR.XOR_IDEAL);
		
		BasicNetwork network = NetworkUtil.CreateXORNetworkUntrained();
		ITrain bprop = new ManhattanPropagation(network, trainingData, 0.01);
		NetworkUtil.TestTraining(bprop,0.01);
	}
	
	[Test]
	public void TestSCG() 
	{
		Logging.StopConsoleLogging();
		INeuralDataSet trainingData = new BasicNeuralDataSet(XOR.XOR_INPUT,XOR.XOR_IDEAL);
		
		BasicNetwork network = NetworkUtil.CreateXORNetworkUntrained();
		ITrain bprop = new ScaledConjugateGradient(network, trainingData);
		NetworkUtil.TestTraining(bprop,0.04);
	}
	
	[Test]
	public void TestAnneal()
	{
		Logging.StopConsoleLogging();
		INeuralDataSet trainingData = new BasicNeuralDataSet(XOR.XOR_INPUT,XOR.XOR_IDEAL);		
		BasicNetwork network = NetworkUtil.CreateXORNetworkUntrained();
		ICalculateScore score = new TrainingSetScore(trainingData);
		NeuralSimulatedAnnealing anneal = new NeuralSimulatedAnnealing(network,score,10,2,100);
		NetworkUtil.TestTraining(anneal,0.01);
	}
	
	[Test]
	public void TestGenetic()
	{
		Logging.StopConsoleLogging();
		INeuralDataSet trainingData = new BasicNeuralDataSet(XOR.XOR_INPUT,XOR.XOR_IDEAL);		
		BasicNetwork network = NetworkUtil.CreateXORNetworkUntrained();
		ICalculateScore score = new TrainingSetScore(trainingData);
		NeuralGeneticAlgorithm genetic = new NeuralGeneticAlgorithm(network, new RangeRandomizer(-1,1), score, 500,0.1,0.25);
		NetworkUtil.TestTraining(genetic,0.000001);
	}
	
        [Test]
	public void TestCont()
	{
		Logging.StopConsoleLogging();
		INeuralDataSet trainingData = new BasicNeuralDataSet(XOR.XOR_INPUT,XOR.XOR_IDEAL);
		
		BasicNetwork network = NetworkUtil.CreateXORNetworkUntrained();
		Propagation prop = new Backpropagation(network, trainingData, 0.7, 0.9);
		
		Assert.IsFalse(prop.CanContinue);
		
		try
		{
			prop.Pause();
            Assert.IsFalse(true);
		}
		catch(Exception)
		{
			// we want an exception.
		}
		
		try
		{
			prop.Resume(null);
			Assert.IsFalse(true);
		}
		catch(Exception)
		{
			// we want an exception.
		}
	}

    }
}
