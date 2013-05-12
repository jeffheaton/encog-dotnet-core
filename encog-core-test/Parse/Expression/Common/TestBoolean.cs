using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.ML.Prg;

namespace Encog.Parse.Expression.Common
{
    [TestClass]
    public class TestBoolean
    {
        [TestMethod]
        public void TestBooleanConst()
        {
            Assert.AreEqual(true, EncogProgram.ParseBoolean("true"));
            Assert.AreEqual(false, EncogProgram.ParseBoolean("false"));
        }

        [TestMethod]
        public void TestCompare()
        {
            Assert.AreEqual(false, EncogProgram.ParseBoolean("3>5"));
            Assert.AreEqual(true, EncogProgram.ParseBoolean("3<5"));
            Assert.AreEqual(false, EncogProgram.ParseBoolean("3=5"));
            Assert.AreEqual(true, EncogProgram.ParseBoolean("5=5"));
            Assert.AreEqual(true, EncogProgram.ParseBoolean("3<=5"));
            Assert.AreEqual(true, EncogProgram.ParseBoolean("5<=5"));
            Assert.AreEqual(false, EncogProgram.ParseBoolean("3>=5"));
            Assert.AreEqual(true, EncogProgram.ParseBoolean("5>=5"));
        }

        [TestMethod]
        public void TestLogic()
        {
            Assert.AreEqual(true, EncogProgram.ParseBoolean("true&true"));
            Assert.AreEqual(false, EncogProgram.ParseBoolean("true&false"));
            Assert.AreEqual(true, EncogProgram.ParseBoolean("true|true"));
            Assert.AreEqual(true, EncogProgram.ParseBoolean("true|false"));
            Assert.AreEqual(false, EncogProgram.ParseBoolean("false|false"));
        }
    }
}
