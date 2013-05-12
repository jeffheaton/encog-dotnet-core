using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.ML.Prg;

namespace Encog.Parse.Expression.Common
{
    [TestClass]
    public class TestFunctions
    {
        [TestMethod]
        public void TestBasicFunctions()
        {
            Assert.AreEqual(3, EncogProgram.ParseFloat("sqrt(9)"), EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(100, EncogProgram.ParseFloat("pow(10,2)"), EncogFramework.DefaultDoubleEqual);
        }
    }
}
