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
using Encog.Util.Normalize.Segregate;
using Encog.Util.Normalize.Segregate.Index;
using Encog.Util.Normalize.Target;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Util.Normalize
{
    [TestClass]
    public class TestSegregate
    {
        public static readonly double[][] ARRAY_2D = {
                                                         new[] {1.0, 2.0, 3.0, 4.0, 5.0},
                                                         new[] {1.0, 2.0, 3.0, 4.0, 5.0},
                                                         new[] {1.0, 2.0, 3.0, 4.0, 5.0},
                                                         new[] {1.0, 2.0, 3.0, 4.0, 5.0},
                                                         new[] {1.0, 2.0, 3.0, 4.0, 5.0},
                                                         new[] {2.0, 2.0, 3.0, 4.0, 5.0}
                                                     };

        private DataNormalization CreateIntegerBalance()
        {
            IInputField a, b;
            double[][] arrayOutput = EngineArray.AllocateDouble2D(3, 2);


            var target = new NormalizationStorageArray2D(arrayOutput);

            var norm = new DataNormalization();
            norm.Report = new NullStatusReportable();
            norm.Storage = target;
            norm.AddInputField(a = new InputFieldArray2D(false, ARRAY_2D, 0));
            norm.AddInputField(b = new InputFieldArray2D(false, ARRAY_2D, 1));
            norm.AddOutputField(new OutputFieldRangeMapped(a, 0.1, 0.9));
            norm.AddOutputField(new OutputFieldRangeMapped(b, 0.1, 0.9));
            norm.AddSegregator(new IntegerBalanceSegregator(a, 2));
            return norm;
        }

        private void Check(DataNormalization norm, int req)
        {
            ISegregator s = norm.Segregators[0];
            double[][] arrayOutput = ((NormalizationStorageArray2D) norm.Storage).GetArray();
            Assert.AreEqual(req, arrayOutput.Length);
        }

        [TestMethod]
        public void TestIntegerBalance()
        {
            DataNormalization norm = CreateIntegerBalance();
            norm.Process();
            Check(norm, 3);
        }

        private DataNormalization CreateRangeSegregate()
        {
            IInputField a, b;
            double[][] arrayOutput = EngineArray.AllocateDouble2D(1, 2);

            RangeSegregator s;

            var target = new NormalizationStorageArray2D(arrayOutput);

            var norm = new DataNormalization();
            norm.Report = new NullStatusReportable();
            norm.Storage = target;
            norm.AddInputField(a = new InputFieldArray2D(false, ARRAY_2D, 0));
            norm.AddInputField(b = new InputFieldArray2D(false, ARRAY_2D, 1));
            norm.AddOutputField(new OutputFieldRangeMapped(a, 0.1, 0.9));
            norm.AddOutputField(new OutputFieldRangeMapped(b, 0.1, 0.9));
            norm.AddSegregator(s = new RangeSegregator(a, false));
            s.AddRange(2, 2, true);
            return norm;
        }

        [TestMethod]
        public void TestRangeSegregate()
        {
            DataNormalization norm = CreateRangeSegregate();
            norm.Process();
            Check(norm, 1);
        }

        private DataNormalization CreateSampleSegregate()
        {
            IInputField a, b;
            var arrayOutput = EngineArray.AllocateDouble2D(6, 2);

            var target = new NormalizationStorageArray2D(arrayOutput);

            var norm = new DataNormalization();
            norm.Report = new NullStatusReportable();
            norm.Storage = target;
            norm.AddInputField(a = new InputFieldArray2D(false, ARRAY_2D, 0));
            norm.AddInputField(b = new InputFieldArray2D(false, ARRAY_2D, 1));
            norm.AddOutputField(new OutputFieldRangeMapped(a, 0.1, 0.9));
            norm.AddOutputField(new OutputFieldRangeMapped(b, 0.1, 0.9));
            norm.AddSegregator(new IndexSampleSegregator(0, 3, 2));
            return norm;
        }

        [TestMethod]
        public void TestSampleSegregate()
        {
            DataNormalization norm = CreateSampleSegregate();
            norm.Process();
            Check(norm, 6);
        }

        public DataNormalization CreateIndexSegregate()
        {
            IInputField a, b;
            double[][] arrayOutput = EngineArray.AllocateDouble2D(6, 2);

            var target = new NormalizationStorageArray2D(arrayOutput);

            var norm = new DataNormalization();
            norm.Report = new NullStatusReportable();
            norm.Storage = target;
            norm.AddInputField(a = new InputFieldArray2D(false, ARRAY_2D, 0));
            norm.AddInputField(b = new InputFieldArray2D(false, ARRAY_2D, 1));
            norm.AddOutputField(new OutputFieldRangeMapped(a, 0.1, 0.9));
            norm.AddOutputField(new OutputFieldRangeMapped(b, 0.1, 0.9));
            norm.AddSegregator(new IndexRangeSegregator(0, 3));
            return norm;
        }

        [TestMethod]
        public void TestIndexSegregate()
        {
            DataNormalization norm = CreateIndexSegregate();
            norm.Process();
            Check(norm, 6);
        }
    }
}
