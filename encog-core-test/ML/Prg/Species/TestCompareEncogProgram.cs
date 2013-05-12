using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.ML.Prg.Species
{
    [TestClass]
    public class TestCompareEncogProgram
    {
        public double Eval(String prg1, String prg2)
        {
            EncogProgram expression1 = new EncogProgram(prg1);
            EncogProgram expression2 = new EncogProgram(prg2);
            CompareEncogProgram comp = new CompareEncogProgram();
            return comp.Compare(expression1, expression2);
        }

        [TestMethod]
        public void TestSingle()
        {
            Assert.AreEqual(2.0, Eval("1+x", "x+1"), EncogFramework.DefaultDoubleEqual);
        }
    }
}
