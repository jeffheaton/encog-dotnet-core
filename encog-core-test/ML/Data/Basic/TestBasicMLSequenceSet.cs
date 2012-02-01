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
                Assert.AreEqual(CHECK[i++], (int)pair.InputArray[0]);
		    }
        }
    }
}
