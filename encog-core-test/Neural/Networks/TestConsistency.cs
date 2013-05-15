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
using Encog.MathUtil.Randomize;
using Encog.ML.Data;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Persist;
using Encog.Util.Banchmark;
using Encog.Util.Simple;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.Util.Arrayutil;

namespace Encog.Neural.Networks
{
    [TestClass]
    public class TestConsistency
    {
        public readonly double[] ExpectedWeights1 = {
                                                         0.008012107263322008, 1.3830172071769407, -0.027657273609111438,
                                                         0.3926920473512011, -0.5591917997643333, -0.03508764590487992,
                                                         -0.8339860696052167, 0.1371821074024733, 0.6804152092361858,
                                                         0.9587552253200567, -0.9363149724379914, -0.28898946379986346,
                                                         1.0572222265035895, 0.3146739685034085, -0.8752594385878787,
                                                         0.4819077576654748, 0.7108891944426319, 0.7165167879211988,
                                                         -0.49437671786974574, -0.5433328356252362, -0.563603612348345,
                                                         0.559330141185627
                                                     };

        public readonly double[] ExpectedWeights2 = {
                                                         0.040412107263322006, 1.6318492071769406, 0.058742726390888546,
                                                         0.43589204735120113, -0.5159917997643333, 0.008112354095120074,
                                                         -0.8555860696052167, 0.07497410740247332, 0.7668152092361858,
                                                         0.9911552253200567, -0.8643149724379915, -0.26738946379986345,
                                                         1.0788222265035896, 0.3470739685034085, -0.8302594385878788,
                                                         1.1248619976654748, 0.7984891944426319, 0.6841167879211988,
                                                         -0.6059767178697457, -0.6729328356252361, -0.720851612348345,
                                                         0.551830141185627
                                                     };

        [TestMethod]
        public void TestRPROPConsistency()
        {
            IMLDataSet training = EncoderTrainingFactory.generateTraining(4, false);
            var network = EncogUtility.SimpleFeedForward(4, 2, 0, 4, true);
            (new ConsistentRandomizer(-1, 1, 50)).Randomize(network);
            var rprop = new ResilientPropagation(network, training);
            for (var i = 0; i < 5; i++)
            {
                rprop.Iteration();
            }
            Assert.IsTrue(CompareArray.Compare(ExpectedWeights1, network.Flat.Weights,0.00001));

            for (var i = 0; i < 5; i++)
            {
                rprop.Iteration();
            }
            Assert.IsTrue(CompareArray.Compare(ExpectedWeights2, network.Flat.Weights, 0.00001));

            var e = network.CalculateError(training);
            Assert.AreEqual(0.0767386807494191, e, 0.00001);
        }

        [TestMethod]
        public void TestFileConsistency()
        {
            var training = EncoderTrainingFactory.generateTraining(4, false);
            var network = (BasicNetwork)EncogDirectoryPersistence.LoadResourceObject("Encog.Resources.xor-nn.eg");
            var e = network.CalculateError(training);
            Assert.AreEqual(0.046796914913558987, e, 0.00001);
        }
    }
}
