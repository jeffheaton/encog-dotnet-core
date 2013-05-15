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
using Encog.Util.Normalize.Input;
using Encog.Util.Normalize.Output.Multiplicative;
using Encog.Util.Normalize.Target;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Util.Normalize
{
    [TestClass]
    public class TestMultiplicative
    {
        private readonly double[][] SAMPLE1 = {new[] {-10.0, 5.0, 15.0}, new[] {-2.0, 1.0, 3.0}};

        public DataNormalization Create(double[][] arrayOutput)
        {
            IInputField a;
            IInputField b;
            IInputField c;

            var target = new NormalizationStorageArray2D(arrayOutput);
            var group = new MultiplicativeGroup();
            var norm = new DataNormalization();
            norm.Report = new NullStatusReportable();
            norm.Storage = target;
            norm.AddInputField(a = new InputFieldArray2D(false, SAMPLE1, 0));
            norm.AddInputField(b = new InputFieldArray2D(false, SAMPLE1, 1));
            norm.AddInputField(c = new InputFieldArray2D(false, SAMPLE1, 2));
            norm.AddOutputField(new OutputFieldMultiplicative(group, a));
            norm.AddOutputField(new OutputFieldMultiplicative(group, b));
            norm.AddOutputField(new OutputFieldMultiplicative(group, c));
            return norm;
        }

        [TestMethod]
        public void TestAbsolute()
        {
            double[][] arrayOutput = EngineArray.AllocateDouble2D(2, 3);
            DataNormalization norm = Create(arrayOutput);
            norm.Process();
            for (int i = 0; i < arrayOutput[0].Length; i++)
            {
                Assert.AreEqual(arrayOutput[0][i], arrayOutput[1][i], 0.01);
            }
        }

        [TestMethod]
        public void TestAbsoluteSerial()
        {
            double[][] arrayOutput = EngineArray.AllocateDouble2D(2, 3);
            DataNormalization norm = Create(arrayOutput);
            norm = (DataNormalization) SerializeRoundTrip.RoundTrip(norm);
            arrayOutput = ((NormalizationStorageArray2D) norm.Storage).GetArray();
            norm.Process();
            for (int i = 0; i < arrayOutput[0].Length; i++)
            {
                Assert.AreEqual(arrayOutput[0][i], arrayOutput[1][i], 0.01);
            }
        }
    }
}
