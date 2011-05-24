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
using Encog.ML.Data.Buffer.CODEC;
using Encog.Util;
using Encog.Util.CSV;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.ML.Data.Buffer
{
    [TestClass]
    public class TestBinaryData
    {
        [TestMethod]
        public void TestArrayCODEC()
        {
            var codec = new ArrayDataCODEC(XOR.XORInput, XOR.XORIdeal);
            var loader = new BinaryDataLoader(codec);
            loader.External2Binary("encog.bin");

            var codec2 = new ArrayDataCODEC();
            var loader2 = new BinaryDataLoader(codec2);
            loader2.Binary2External("encog.bin");

            double[][] input = codec2.Input;
            double[][] ideal = codec2.Ideal;

            for (int i = 0; i < XOR.XORInput.Length; i++)
            {
                for (int j = 0; j < XOR.XORInput[i].Length; j++)
                {
                    Assert.AreEqual(input[i][j], XOR.XORInput[i][j], 0.01);
                }

                for (int j = 0; j < XOR.XORIdeal[i].Length; j++)
                {
                    Assert.AreEqual(ideal[i][j], XOR.XORIdeal[i][j], 0.01);
                }
            }
        }

        [TestMethod]
        public void TestCSV()
        {
            var codec = new ArrayDataCODEC(XOR.XORInput, XOR.XORIdeal);
            var loader = new BinaryDataLoader(codec);
            loader.External2Binary("encog.bin");

            var codec2 = new CSVDataCODEC("encog.csv", CSVFormat.English);
            var loader2 = new BinaryDataLoader(codec2);
            loader2.Binary2External("encog.bin");

            var codec3 = new CSVDataCODEC("encog.csv", CSVFormat.English, false, 2, 1);
            var loader3 = new BinaryDataLoader(codec3);
            loader3.External2Binary("encog.bin");

            var codec4 = new ArrayDataCODEC();
            var loader4 = new BinaryDataLoader(codec4);
            loader4.Binary2External("encog.bin");

            double[][] input = codec4.Input;
            double[][] ideal = codec4.Ideal;

            for (int i = 0; i < XOR.XORInput.Length; i++)
            {
                for (int j = 0; j < XOR.XORInput[i].Length; j++)
                {
                    Assert.AreEqual(input[i][j], XOR.XORInput[i][j], 0.01);
                }

                for (int j = 0; j < XOR.XORIdeal[i].Length; j++)
                {
                    Assert.AreEqual(ideal[i][j], XOR.XORIdeal[i][j], 0.01);
                }
            }
        }
    }
}
