using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.ML.Prg;

namespace Encog.Parse.Expression.Common
{
    [TestClass]
    public class TestString
    {
        [TestMethod]
        public void TestSimple()
        {
            Assert.AreEqual("test", EncogProgram.ParseString("\"test\""));
            Assert.AreEqual("", EncogProgram.ParseString("\"\""));
        }

        [TestMethod]
        public void TestConcat()
        {
            Assert.AreEqual("test:123", EncogProgram.ParseString("\"test:\"+123.0"));
            Assert.AreEqual("helloworld", EncogProgram.ParseString("\"hello\"+\"world\""));
            Assert.AreEqual(4, (int)EncogProgram.ParseFloat("length(\"test\")"));
            Assert.AreEqual("5.22", EncogProgram.ParseString("format(5.2222,2)"));
        }
    }
}
