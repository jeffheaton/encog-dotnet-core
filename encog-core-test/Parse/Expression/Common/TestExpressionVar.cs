using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.ML.Prg;

namespace Encog.Parse.Expression.Common
{
    [TestClass]
    public class TestExpressionVar
    {
        [TestMethod]
        public void TestAssignment()
        {
            EncogProgram expression = new EncogProgram("a");
            expression.Variables.SetVariable("a", 5);
            Assert.AreEqual(5, expression.Evaluate().ToFloatValue(), EncogFramework.DefaultDoubleEqual);
        }

        [TestMethod]
        public void TestNegAssignment()
        {
            EncogProgram expression = new EncogProgram("-a");
            expression.Variables.SetVariable("a", 5);
            Assert.AreEqual(-5, expression.Evaluate().ToFloatValue(), EncogFramework.DefaultDoubleEqual);
        }

        [TestMethod]
        public void Test2NegAssignment()
        {
            EncogProgram expression = new EncogProgram("--a");
            expression.Variables.SetVariable("a", 5);
            Assert.AreEqual(5, expression.Evaluate().ToFloatValue(), EncogFramework.DefaultDoubleEqual);
        }

        [TestMethod]
        public void TestAssignment2()
        {
            EncogProgram expression = new EncogProgram("cccc*(aa+bbb)");
            expression.Variables.SetVariable("aa", 1);
            expression.Variables.SetVariable("bbb", 2);
            expression.Variables.SetVariable("cccc", 3);
            Assert.AreEqual(9, expression.Evaluate().ToFloatValue(), EncogFramework.DefaultDoubleEqual);
        }

        [TestMethod]
        public void TestAssignment3()
        {
            EncogProgram expression = new EncogProgram("v1+v2+v3");
            expression.Variables.SetVariable("v1", 1);
            expression.Variables.SetVariable("v2", 2);
            expression.Variables.SetVariable("v3", 3);
            Assert.AreEqual(6, expression.Evaluate().ToFloatValue(), EncogFramework.DefaultDoubleEqual);
        }

        [TestMethod]
        public void testVarComplex()
        {
            EncogProgram expression = new EncogProgram("(x^((1+((x^-8)-(4^x)))^(((-7/2)-(0--5.8))/x)))");
            expression.Variables.SetVariable("x", 10);
            Assert.IsTrue(Double.IsNaN(expression.Evaluate().ToFloatValue()));
        }
    }
}
