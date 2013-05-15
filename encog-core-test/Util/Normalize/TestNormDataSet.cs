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
using Encog.ML.Data.Basic;
using Encog.Util.Normalize.Input;
using Encog.Util.Normalize.Output;
using Encog.Util.Normalize.Target;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Util.Normalize
{
    [TestClass]
    public class TestNormDataSet
    {
        public static readonly double[][] ARRAY_2D = {
                                                         new[] {1.0, 2.0, 3.0, 4.0, 5.0},
                                                         new[] {6.0, 7.0, 8.0, 9.0, 0.0}
                                                     };


        private DataNormalization Create()
        {
            IInputField a, b;
            double[][] arrayOutput = EngineArray.AllocateDouble2D(2, 2);

            var dataset = new BasicMLDataSet(ARRAY_2D, null);

            var target = new NormalizationStorageArray2D(arrayOutput);

            var norm = new DataNormalization();
            norm.Report = new NullStatusReportable();
            norm.Storage = target;
            norm.AddInputField(a = new InputFieldMLDataSet(false, dataset, 0));
            norm.AddInputField(b = new InputFieldMLDataSet(false, dataset, 1));
            norm.AddOutputField(new OutputFieldRangeMapped(a, 0.1, 0.9));
            norm.AddOutputField(new OutputFieldRangeMapped(b, 0.1, 0.9));
            return norm;
        }

        private void Check(DataNormalization norm)
        {
            double[][] arrayOutput = ((NormalizationStorageArray2D) norm.Storage).GetArray();
            Assert.AreEqual(arrayOutput[0][0], 0.1, 0.1);
            Assert.AreEqual(arrayOutput[1][0], 0.9, 0.1);
            Assert.AreEqual(arrayOutput[0][1], 0.1, 0.1);
            Assert.AreEqual(arrayOutput[1][1], 0.9, 0.1);
        }

        [TestMethod]
        public void TestDataSet()
        {
            DataNormalization norm = Create();
            norm.Process();
            Check(norm);
        }

        [TestMethod]
        public void TestDataSetSerial()
        {
            DataNormalization norm = Create();
            norm = (DataNormalization) SerializeRoundTrip.RoundTrip(norm);
            norm.Process();
            Check(norm);
        }
    }
}
