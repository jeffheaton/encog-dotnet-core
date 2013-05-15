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
