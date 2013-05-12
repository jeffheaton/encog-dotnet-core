using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.ML.Prg
{
    [TestClass]
    public class TestEncogProgram
    {
        [TestMethod]
        public void TestSize()
        {
            EncogProgram expression = new EncogProgram("1");
            Assert.AreEqual(1, expression.RootNode.Count);

            expression = new EncogProgram("1+1");
            Assert.AreEqual(3, expression.RootNode.Count);

            expression = new EncogProgram("1+1+1");
            Assert.AreEqual(5, expression.RootNode.Count);

            expression = new EncogProgram("(sin(x)+cos(x))/2");
            Assert.AreEqual(7, expression.RootNode.Count);
        }
    }
}
