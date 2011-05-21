//
// Encog(tm) Unit Tests v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Util;
using Encog.ML.Train;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Networks.Training.Lma;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training.Propagation.Manhattan;
using Encog.Neural.Networks.Training.Propagation.SCG;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks.Training.Genetic;
using Encog.MathUtil.Randomize;
using Encog.Neural.PNN;
using Encog.Neural.Networks.Training.PNN;

namespace Encog.Neural.Networks.Training
{
    [TestClass]
    public class TestTraining
    {
        [TestMethod]
        public void TestRPROP()
        {
            IMLDataSet trainingData = new BasicMLDataSet(XOR.XORInput, XOR.XORIdeal);

            BasicNetwork network = NetworkUtil.CreateXORNetworkUntrained();
            MLTrain rprop = new ResilientPropagation(network, trainingData);
            NetworkUtil.TestTraining(rprop, 0.03);
        }

        [TestMethod]
        public void TestLMA()
        {
            IMLDataSet trainingData = new BasicMLDataSet(XOR.XORInput, XOR.XORIdeal);

            BasicNetwork network = NetworkUtil.CreateXORNetworkUntrained();
            MLTrain rprop = new LevenbergMarquardtTraining(network, trainingData);
            NetworkUtil.TestTraining(rprop, 0.03);
        }

        [TestMethod]
        public void TestBPROP()
        {
            IMLDataSet trainingData = new BasicMLDataSet(XOR.XORInput, XOR.XORIdeal);

            BasicNetwork network = NetworkUtil.CreateXORNetworkUntrained();

            MLTrain bprop = new Backpropagation(network, trainingData, 0.7, 0.9);
            NetworkUtil.TestTraining(bprop, 0.01);
        }

        [TestMethod]
        public void TestManhattan()
        {
            IMLDataSet trainingData = new BasicMLDataSet(XOR.XORInput, XOR.XORIdeal);

            BasicNetwork network = NetworkUtil.CreateXORNetworkUntrained();
            MLTrain bprop = new ManhattanPropagation(network, trainingData, 0.01);
            NetworkUtil.TestTraining(bprop, 0.01);
        }

        [TestMethod]
        public void TestSCG()
        {
            IMLDataSet trainingData = new BasicMLDataSet(XOR.XORInput, XOR.XORIdeal);

            BasicNetwork network = NetworkUtil.CreateXORNetworkUntrained();
            MLTrain bprop = new ScaledConjugateGradient(network, trainingData);
            NetworkUtil.TestTraining(bprop, 0.04);
        }

        [TestMethod]
        public void TestAnneal()
        {
            IMLDataSet trainingData = new BasicMLDataSet(XOR.XORInput, XOR.XORIdeal);
            BasicNetwork network = NetworkUtil.CreateXORNetworkUntrained();
            ICalculateScore score = new TrainingSetScore(trainingData);
            NeuralSimulatedAnnealing anneal = new NeuralSimulatedAnnealing(network, score, 10, 2, 100);
            NetworkUtil.TestTraining(anneal, 0.01);
        }

        [TestMethod]
        public void TestGenetic()
        {
            IMLDataSet trainingData = new BasicMLDataSet(XOR.XORInput, XOR.XORIdeal);
            BasicNetwork network = NetworkUtil.CreateXORNetworkUntrained();
            ICalculateScore score = new TrainingSetScore(trainingData);
            NeuralGeneticAlgorithm genetic = new NeuralGeneticAlgorithm(network, new RangeRandomizer(-1, 1), score, 500, 0.1, 0.25);
            NetworkUtil.TestTraining(genetic, 0.00001);
        }

        [TestMethod]
        public void TestRegPNN()
        {

            PNNOutputMode mode = PNNOutputMode.Regression;
            BasicPNN network = new BasicPNN(PNNKernelType.Gaussian, mode, 2, 1);

            IMLDataSet trainingData = new BasicMLDataSet(XOR.XORInput, XOR.XORIdeal);

            TrainBasicPNN train = new TrainBasicPNN(network, trainingData);
            train.Iteration();

            XOR.VerifyXOR(network, 0.01);
        }

        [TestMethod]
        public void TestClassifyPNN()
        {

            PNNOutputMode mode = PNNOutputMode.Classification;
            BasicPNN network = new BasicPNN(PNNKernelType.Gaussian, mode, 2, 2);

            IMLDataSet trainingData = new BasicMLDataSet(XOR.XORInput, XOR.XORIdeal);

            TrainBasicPNN train = new TrainBasicPNN(network, trainingData);
            train.Iteration();

            XOR.VerifyXOR(network, 0.01);
        }
    }
}
