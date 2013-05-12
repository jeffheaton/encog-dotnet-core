using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.ML.Prg;
using Encog.ML.Prg.ExpValue;
using Encog.ML.EA.Exceptions;

namespace Encog.Parse.Expression.Common
{
    [TestClass]
    public class TestExpression
    {
        [TestMethod]
        public void TestConst()
        {
            Assert.AreEqual(1, EncogProgram.ParseFloat("1"), EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(-1, EncogProgram.ParseFloat("-1"), EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(1, EncogProgram.ParseFloat("--1"), EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(-1, EncogProgram.ParseFloat("---1"), EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(1, EncogProgram.ParseFloat("----1"), EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(100, EncogProgram.ParseFloat("100"), EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(100, EncogProgram.ParseFloat("+100"), EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(-100, EncogProgram.ParseFloat("-100"), EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(1000, EncogProgram.ParseFloat("1e3"), EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(0.001, EncogProgram.ParseFloat("1e-3"), EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(1.5, EncogProgram.ParseFloat("1.5"), EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(-1.5, EncogProgram.ParseFloat("-1.5"), EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(1500, EncogProgram.ParseFloat("1.5e3"), EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(-0.0015, EncogProgram.ParseFloat("-1.5e-3"), EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(1.2345678, EncogProgram.ParseFloat("1.2345678"), EncogFramework.DefaultDoubleEqual);
        }

        [TestMethod]
        public void TestTypes()
        {
            ExpressionValue exp = EncogProgram.ParseExpression("cint(1.2345678)");
            Assert.IsTrue(exp.IsInt);
            Assert.AreEqual(1, exp.ToIntValue());

            exp = EncogProgram.ParseExpression("cstr(1.2345678)");
            Assert.IsTrue(exp.IsString);
            Assert.AreEqual("1.2345678", exp.ToStringValue());

            exp = EncogProgram.ParseExpression("cfloat(\"1.2345678\")");
            Assert.IsTrue(exp.IsFloat);
            Assert.AreEqual("1.2345678", exp.ToStringValue());

        }

        [TestMethod]
        public void TestPrecedence()
        {
            Assert.AreEqual(-2.5, EncogProgram.ParseFloat("1.0+2.0*3.0/4.0-5.0"), EncogFramework.DefaultDoubleEqual);
        }

        [TestMethod]
        public void TestAdd()
        {
            Assert.AreEqual(5, EncogProgram.ParseFloat("2+3"), EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(3, EncogProgram.ParseFloat("5+-2"), EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(6, EncogProgram.ParseFloat("1+2+3"), EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(10, EncogProgram.ParseFloat("1+2+3+4"), EncogFramework.DefaultDoubleEqual);
        }

        [TestMethod]
        public void TestSub()
        {
            Assert.AreEqual(-1, EncogProgram.ParseFloat("2-3"), EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(7, EncogProgram.ParseFloat("5--2"), EncogFramework.DefaultDoubleEqual);
        }

        [TestMethod]
        public void TestMul()
        {
            Assert.AreEqual(-6, EncogProgram.ParseFloat("-2*3"), EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(6, EncogProgram.ParseFloat("2*3"), EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(6, EncogProgram.ParseFloat("-2*-3"), EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(24, EncogProgram.ParseFloat("2*3*4"), EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(120, EncogProgram.ParseFloat("2*3*4*5"), EncogFramework.DefaultDoubleEqual);
        }

        [TestMethod]
        public void TestPower()
        {
            Assert.AreEqual(8, EncogProgram.ParseFloat("2^3"), EncogFramework.DefaultDoubleEqual);
        }

        [TestMethod]
        public void TestParen1()
        {
            Assert.AreEqual(14, EncogProgram.ParseFloat("2*(3+4)"), EncogFramework.DefaultDoubleEqual);
        }

        [TestMethod]
        public void TestParen2()
        {
            Assert.AreEqual(10, EncogProgram.ParseFloat("(2*3)+4"), EncogFramework.DefaultDoubleEqual);
        }

        [TestMethod]
        public void TestParen3()
        {
            Assert.AreEqual(100, EncogProgram.ParseFloat("(2*3)^2+(4*2)^2"), EncogFramework.DefaultDoubleEqual);
        }

        [TestMethod]
        public void TestParen4()
        {
            Assert.AreEqual(4, EncogProgram.ParseFloat("2^(1+1)"), EncogFramework.DefaultDoubleEqual);
        }

        [TestMethod]
        public void TestBad()
        {
            try
            {
                Assert.AreEqual(0, EncogProgram.ParseFloat("2*(3+4"), EncogFramework.DefaultDoubleEqual);
                Assert.IsTrue(false);
            }
            catch (EAError ex)
            {
                // good, we want an exception.
            }

            try
            {
                Assert.AreEqual(0, EncogProgram.ParseFloat("5+"), EncogFramework.DefaultDoubleEqual);
                Assert.IsTrue(false);
            }
            catch (EAError ex)
            {
                // good, we want an exception.
            }
        }
    }
}
