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
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.ML.HMM;
using Encog.Util;
using System.IO;
using Encog.ML.HMM.Distributions;
using Encog.ML.HMM.Alog;

namespace Encog.Persist
{
    [TestClass]
    public class TestPersistHMM
    {
        public static readonly TempDir TEMP_DIR = new TempDir();
        public readonly FileInfo EG_FILENAME = TEMP_DIR.CreateFile("encogtest.eg");
        public readonly FileInfo SERIAL_FILENAME = TEMP_DIR.CreateFile("encogtest.ser");

        static HiddenMarkovModel BuildContHMM()
	{	
		double [] mean1 = {0.25, -0.25};
		double [][] covariance1 = { new[] {1.0, 2.0}, new[] {1.0, 4.0} };
		
		double [] mean2 = {0.5, 0.25};
		double [][] covariance2 = { new[] {4.0, 2.0}, new[] {3.0, 4.0} };
		
		HiddenMarkovModel hmm = new HiddenMarkovModel(2);
		
		hmm.Pi[0] = 0.8;
		hmm.Pi[1] = 0.2;
		
		hmm.StateDistributions[0] = new ContinousDistribution(mean1,covariance1);
		hmm.StateDistributions[1] = new ContinousDistribution(mean2,covariance2);
		
		hmm.TransitionProbability[0][1] = 0.05;
		hmm.TransitionProbability[0][0] = 0.95;
		hmm.TransitionProbability[1][0] = 0.10;
		hmm.TransitionProbability[1][1] = 0.90;
		
		return hmm;
	}

        static HiddenMarkovModel BuildDiscHMM()
        {
            HiddenMarkovModel hmm =
                new HiddenMarkovModel(2, 2);

            hmm.Pi[0] = 0.95;
            hmm.Pi[1] = 0.05;

            hmm.StateDistributions[0] = new DiscreteDistribution(new double[][] { new[] { 0.95, 0.05 } });
            hmm.StateDistributions[1] = new DiscreteDistribution(new double[][] { new[] { 0.20, 0.80 } });

            hmm.TransitionProbability[0][1] = 0.05;
            hmm.TransitionProbability[0][0] = 0.95;
            hmm.TransitionProbability[1][0] = 0.10;
            hmm.TransitionProbability[1][1] = 0.90;

            return hmm;
        }

        public void Validate(HiddenMarkovModel result, HiddenMarkovModel source)
        {
            KullbackLeiblerDistanceCalculator klc =
                    new KullbackLeiblerDistanceCalculator();

            double e = klc.Distance(result, source);
            Assert.IsTrue(e < 0.01);
        }

        [TestMethod]
        public void TestDiscPersistEG()
        {
            HiddenMarkovModel sourceHMM = BuildDiscHMM();

            EncogDirectoryPersistence.SaveObject(EG_FILENAME, sourceHMM);
            HiddenMarkovModel resultHMM = (HiddenMarkovModel)EncogDirectoryPersistence.LoadObject(EG_FILENAME);

            Validate(resultHMM, sourceHMM);
        }

        [TestMethod]
        public void TestDiscPersistSerial()
        {
            HiddenMarkovModel sourceHMM = BuildDiscHMM();

            SerializeObject.Save(SERIAL_FILENAME.ToString(), sourceHMM);
            HiddenMarkovModel resultHMM = (HiddenMarkovModel)SerializeObject.Load(SERIAL_FILENAME.ToString());

            Validate(resultHMM, sourceHMM);
        }

        [TestMethod]
        public void TestContPersistEG()
        {
            HiddenMarkovModel sourceHMM = BuildContHMM();

            EncogDirectoryPersistence.SaveObject(EG_FILENAME, sourceHMM);
            HiddenMarkovModel resultHMM = (HiddenMarkovModel)EncogDirectoryPersistence.LoadObject(EG_FILENAME);

            Validate(resultHMM, sourceHMM);
        }

        [TestMethod]
        public void TestContPersistSerial()
        {
            HiddenMarkovModel sourceHMM = BuildContHMM();

            SerializeObject.Save(SERIAL_FILENAME.ToString(), sourceHMM);
            HiddenMarkovModel resultHMM = (HiddenMarkovModel)SerializeObject.Load(SERIAL_FILENAME.ToString());

            Validate(resultHMM, sourceHMM);
        }

    }
}
