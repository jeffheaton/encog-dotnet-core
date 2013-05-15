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
using Encog.Util.Normalize.Output;
using Encog.Util.Normalize.Target;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Util.Normalize
{
    [TestClass]
    public class TestNormArray
    {
        public static double[] ARRAY_1D = {1.0, 2.0, 3.0, 4.0, 5.0};

        public static double[][] ARRAY_2D = {
                                                new[] {1.0, 2.0, 3.0, 4.0, 5.0},
                                                new[] {6.0, 7.0, 8.0, 9.0}
                                            };

        private DataNormalization Create1D(double[] arrayOutput)
        {
            IInputField a;

            var target = new NormalizationStorageArray1D(arrayOutput);

            var norm = new DataNormalization();
            norm.Report = new NullStatusReportable();
            norm.Storage = target;
            norm.AddInputField(a = new InputFieldArray1D(false, ARRAY_1D));
            norm.AddOutputField(new OutputFieldRangeMapped(a, 0.1, 0.9));
            return norm;
        }

        private DataNormalization Create2D(double[][] arrayOutput)
        {
            IInputField a, b;

            var target = new NormalizationStorageArray2D(arrayOutput);

            var norm = new DataNormalization();
            norm.Report = new NullStatusReportable();
            norm.Storage = target;
            norm.AddInputField(a = new InputFieldArray2D(false, ARRAY_2D, 0));
            norm.AddInputField(b = new InputFieldArray2D(false, ARRAY_2D, 1));
            norm.AddOutputField(new OutputFieldRangeMapped(a, 0.1, 0.9));
            norm.AddOutputField(new OutputFieldRangeMapped(b, 0.1, 0.9));
            return norm;
        }

        [TestMethod]
        public void TestArray1D()
        {
            var arrayOutput = new double[5];
            DataNormalization norm = Create1D(arrayOutput);
            norm.Process();
            Check1D(arrayOutput);
        }

        [TestMethod]
        public void TestArray2D()
        {
            double[][] arrayOutput = EngineArray.AllocateDouble2D(2, 2);
            DataNormalization norm = Create2D(arrayOutput);
            norm.Process();
            Check2D(arrayOutput);
        }

        [TestMethod]
        public void TestArray1DSerial()
        {
            var arrayOutput = new double[5];
            DataNormalization norm = Create1D(arrayOutput);
            norm = (DataNormalization) SerializeRoundTrip.RoundTrip(norm);
            arrayOutput = ((NormalizationStorageArray1D) norm.Storage).GetArray();
            norm.Process();
            Check1D(arrayOutput);
        }

        [TestMethod]
        public void TestArray2DSerial()
        {
            var arrayOutput = EngineArray.AllocateDouble2D(2, 2);
            DataNormalization norm = Create2D(arrayOutput);
            norm = (DataNormalization) SerializeRoundTrip.RoundTrip(norm);
            arrayOutput = ((NormalizationStorageArray2D) norm.Storage).GetArray();
            norm.Process();
            Check2D(arrayOutput);
        }

        public void Check1D(double[] arrayOutput)
        {
            Assert.AreEqual(arrayOutput[0], 0.1, 0.1);
            Assert.AreEqual(arrayOutput[1], 0.3, 0.1);
            Assert.AreEqual(arrayOutput[2], 0.5, 0.1);
            Assert.AreEqual(arrayOutput[3], 0.7, 0.1);
            Assert.AreEqual(arrayOutput[4], 0.9, 0.1);
        }

        public void Check2D(double[][] arrayOutput)
        {
            Assert.AreEqual(arrayOutput[0][0], 0.1, 0.1);
            Assert.AreEqual(arrayOutput[1][0], 0.9, 0.1);
            Assert.AreEqual(arrayOutput[0][1], 0.1, 0.1);
            Assert.AreEqual(arrayOutput[1][1], 0.9, 0.1);
        }
    }
}
