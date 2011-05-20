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
using Encog.ML.Data.Basic;
using Encog.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.ML.Data.Buffer
{
    [TestClass]
    public class TestBufferedNeuralDataSet
    {
        public static readonly string Filename = "xor.bin";

        [TestMethod]
        public void TestBufferData()
        {
            File.Delete(Filename);
            var set = new BufferedMLDataSet(Filename);
            set.BeginLoad(2, 1);
            for (int i = 0; i < XOR.XORInput.Length; i++)
            {
                var input = new BasicMLData(XOR.XORInput[i]);
                var ideal = new BasicMLData(XOR.XORIdeal[i]);
                set.Add(input, ideal);
            }
            set.EndLoad();

            XOR.TestXORDataSet(set);
        }
    }
}
