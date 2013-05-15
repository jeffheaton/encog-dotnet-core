//
// Encog(tm) Core v3.2 - .Net Version (Unit Test)
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2013 Heaton Research, Inc.
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
using Encog.ML.HMM.Alog;
using Encog.ML.HMM.Distributions;
using Encog.ML.HMM.Train.BW;
using Encog.ML.HMM.Train.KMeans;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.ML.HMM
{
    [TestClass]
    public class TestHMM
    {
        private static HiddenMarkovModel BuildContHMM()
        {
            double[] mean1 = {0.25, -0.25};
            double[][] covariance1 = {
                                         new[] {1.0, 2.0},
                                         new[] {1.0, 4.0}
                                     };

            double[] mean2 = {0.5, 0.25};
            double[][] covariance2 = {
                                         new[] {4.0, 2.0},
                                         new[] {3.0, 4.0}
                                     };

            var hmm = new HiddenMarkovModel(2);

            hmm.Pi[0] = 0.8;
            hmm.Pi[1] = 0.2;

            hmm.StateDistributions[0] = new ContinousDistribution(mean1, covariance1);
            hmm.StateDistributions[1] = new ContinousDistribution(mean2, covariance2);

            hmm.TransitionProbability[0][1] = 0.05;
            hmm.TransitionProbability[0][0] = 0.95;
            hmm.TransitionProbability[1][0] = 0.10;
            hmm.TransitionProbability[1][1] = 0.90;

            return hmm;
        }

        private static HiddenMarkovModel BuildContInitHMM()
        {
            double[] mean1 = {0.20, -0.20};
            double[][] covariance1 = {new[] {1.3, 2.2}, new[] {1.3, 4.3}};

            double[] mean2 = {0.5, 0.25};
            double[][] covariance2 = {new[] {4.1, 2.1}, new[] {3.2, 4.4}};

            var hmm = new HiddenMarkovModel(2);

            hmm.Pi[0] = 0.9;
            hmm.Pi[1] = 0.1;

            hmm.StateDistributions[0] = new ContinousDistribution(mean1, covariance1);
            hmm.StateDistributions[1] = new ContinousDistribution(mean2, covariance2);

            hmm.TransitionProbability[0][1] = 0.10;
            hmm.TransitionProbability[0][0] = 0.90;
            hmm.TransitionProbability[1][0] = 0.15;
            hmm.TransitionProbability[1][1] = 0.85;

            return hmm;
        }

        private static HiddenMarkovModel BuildDiscHMM()
        {
            var hmm =
                new HiddenMarkovModel(2, 2);

            hmm.Pi[0] = 0.95;
            hmm.Pi[1] = 0.05;

            hmm.StateDistributions[0] = new DiscreteDistribution(new[] {new[] {0.95, 0.05}});
            hmm.StateDistributions[1] = new DiscreteDistribution(new[] {new[] {0.20, 0.80}});

            hmm.TransitionProbability[0][1] = 0.05;
            hmm.TransitionProbability[0][0] = 0.95;
            hmm.TransitionProbability[1][0] = 0.10;
            hmm.TransitionProbability[1][1] = 0.90;

            return hmm;
        }


        /* Initial guess for the Baum-Welch algorithm */

        private static HiddenMarkovModel BuildDiscInitHMM()
        {
            var hmm = new HiddenMarkovModel(2, 2);

            hmm.Pi[0] = 0.50;
            hmm.Pi[1] = 0.50;

            hmm.StateDistributions[0] = new DiscreteDistribution(new[] {new[] {0.8, 0.2}});
            hmm.StateDistributions[1] = new DiscreteDistribution(new[] {new[] {0.1, 0.9}});

            hmm.TransitionProbability[0][1] = 0.2;
            hmm.TransitionProbability[0][0] = 0.8;
            hmm.TransitionProbability[1][0] = 0.2;
            hmm.TransitionProbability[1][1] = 0.8;

            return hmm;
        }

        [TestMethod]
        public void TestDiscBWL()
        {
            var hmm = BuildDiscHMM();
            var learntHmm = BuildDiscInitHMM();
            var mg = new MarkovGenerator(hmm);
            var training = mg.GenerateSequences(200, 100);
            var bwl = new TrainBaumWelch(learntHmm, training);
            var klc = new KullbackLeiblerDistanceCalculator();

            bwl.Iteration(5);
            learntHmm = (HiddenMarkovModel) bwl.Method;

            var e = klc.Distance(learntHmm, hmm);
            Assert.IsTrue(e < 0.01);
        }

        [TestMethod]
        public void TestContBWL()
        {
            var hmm = BuildContHMM();
            var learntHmm = BuildContInitHMM();
            var mg = new MarkovGenerator(hmm);
            var training = mg.GenerateSequences(200, 100);
            var bwl = new TrainBaumWelch(learntHmm, training);
            var klc = new KullbackLeiblerDistanceCalculator();

            bwl.Iteration(5);
            learntHmm = (HiddenMarkovModel) bwl.Method;

            var e = klc.Distance(learntHmm, hmm);
            Assert.IsTrue(e < 0.01);
        }

        [TestMethod]
        public void TestDiscKMeans()
        {
            var hmm = BuildDiscHMM();

            var mg = new MarkovGenerator(hmm);
            var sequences = mg.GenerateSequences(200, 100);

            var trainer = new TrainKMeans(hmm, sequences);
            var klc =
                new KullbackLeiblerDistanceCalculator();

            trainer.Iteration(5);
            var learntHmm = (HiddenMarkovModel) trainer.Method;
            var e = klc.Distance(learntHmm, hmm);
            Assert.IsTrue(e < 0.05);
        }
    }
}

