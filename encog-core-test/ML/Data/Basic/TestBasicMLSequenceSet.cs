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

namespace Encog.ML.Data.Basic
{
    [TestClass]
    public class TestBasicMLSequenceSet
    {
        public static IMLDataPair TEST1 = new BasicMLDataPair(new BasicMLData(new double[] { 1.0 }), null);
        public static IMLDataPair TEST2 = new BasicMLDataPair(new BasicMLData(new double[] { 2.0 }), null);
        public static IMLDataPair TEST3 = new BasicMLDataPair(new BasicMLData(new double[] { 3.0 }), null);
        public static IMLDataPair TEST4 = new BasicMLDataPair(new BasicMLData(new double[] { 4.0 }), null);
        public static IMLDataPair TEST5 = new BasicMLDataPair(new BasicMLData(new double[] { 5.0 }), null);
        public static int[] CHECK = { 1, 2, 3, 4, 1, 2, 3, 2, 1 };

        [TestMethod]
        public void TestSimple()
        {
            BasicMLSequenceSet seq = new BasicMLSequenceSet();
            seq.StartNewSequence();
            seq.Add(TEST1);
            seq.Add(TEST2);
            seq.Add(TEST3);
            seq.Add(TEST4);
            seq.StartNewSequence();
            seq.Add(TEST1);
		    seq.Add(TEST2);
		    seq.StartNewSequence();
		    seq.Add(TEST3);
		    seq.Add(TEST2);
		    seq.Add(TEST1);
		
		    Assert.AreEqual(9, seq.Count);
		    Assert.AreEqual(3, seq.SequenceCount);
		
		    int i = 0;
		    foreach(IMLDataPair pair in seq) {
                Assert.AreEqual(CHECK[i++], (int)pair.Input[0],"Equal Input with array.");
		    }
        }
    }
}
