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
using System.IO;
using System.Text;
using Encog.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.ML.Data.Specific
{
    /// <summary>
    /// Description of TestCSVNeuralData.
    /// </summary>
    [TestClass]
    public class TestCSVNeuralData
    {
        public const string Filename = "xor.csv";

        private static void GenerateCSV()
        {
            TextWriter fp = new StreamWriter(Filename);

            for (int count = 0; count < XOR.XORInput.Length; count++)
            {
                var builder = new StringBuilder();

                for (int i = 0; i < XOR.XORInput[0].Length; i++)
                {
                    if (builder.Length > 0)
                        builder.Append(',');
                    builder.Append(XOR.XORInput[count][i]);
                }

                for (int i = 0; i < XOR.XORIdeal[0].Length; i++)
                {
                    if (builder.Length > 0)
                        builder.Append(',');
                    builder.Append(XOR.XORIdeal[count][i]);
                }
                fp.WriteLine(builder.ToString());
            }
            fp.Close();
        }

        [TestMethod]
        public void CSVData()
        {
            GenerateCSV();

            var set = new CSVMLDataSet("xor.csv", 2, 1, false);

            XOR.TestXORDataSet(set);

            set.Close();
            File.Delete(Filename);
        }
    }
}
