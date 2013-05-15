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
using Encog.ML.Prg.Ext;
using Encog.ML.Prg.Train;
using Encog.Neural.Networks.Training;
using Encog.ML.EA.Train;
using Encog.Parse.Expression.Common;
using Encog.ML.Prg.Train.Rewrite;
using Encog.ML.EA.Score.Adjust;
using Encog.ML.Prg.Opp;
using Encog.ML.Prg.ExpValue;

namespace Encog.ML.Prg.Rewrite
{
    [TestClass]
    public class TestRewriteAlgebraic
    {
        public void Eval(String start, String expect)
        {
            EncogProgramContext context = new EncogProgramContext();
            StandardExtensions.CreateNumericOperators(context);
            PrgPopulation pop = new PrgPopulation(context, 1);
            ICalculateScore score = new ZeroEvalScoreFunction();

            TrainEA genetic = new TrainEA(pop, score);
            genetic.ValidationMode = true;
            genetic.CODEC = new PrgCODEC();
            genetic.AddOperation(0.95, new SubtreeCrossover());
            genetic.AddOperation(0.05, new SubtreeMutation(context, 4));
            genetic.AddScoreAdjuster(new ComplexityAdjustedScore());
            genetic.Rules.AddRewriteRule(new RewriteConstants());
            genetic.Rules.AddRewriteRule(new RewriteAlgebraic());

            EncogProgram expression = new EncogProgram(context);
            expression.CompileExpression(start);
            RenderCommonExpression render = new RenderCommonExpression();
            genetic.Rules.Rewrite(expression);
            Assert.AreEqual(expect, render.Render(expression));
        }

        [TestMethod]
        public void TestMinusZero()
        {
            Eval("x-0", "x");
            Eval("0-0", "0");
            Eval("10-0", "10");
        }

        [TestMethod]
        public void TestZeroMul()
        {
            Eval("0*0", "0");
            Eval("1*0", "0");
            Eval("0*1", "0");
        }

        [TestMethod]
        public void TestZeroDiv()
        {
            try
            {
                Eval("0/0", "(0/0)");
                Assert.IsFalse(true);
            }
            catch (DivisionByZeroError ex)
            {
                // expected
            }
            Eval("0/5", "0");
            Eval("0/x", "0");
        }

        [TestMethod]
        public void TestZeroPlus()
        {
            Eval("0+0", "0");
            Eval("1+0", "1");
            Eval("0+1", "1");
            Eval("x+0", "x");
        }

        [TestMethod]
        public void TestPowerZero()
        {
            Eval("0^x", "0");
            Eval("0^0", "1");
            Eval("x^0", "1");
            Eval("1^0", "1");
            Eval("-1^0", "1");
            Eval("(x+y)^0", "1");
            Eval("x+(x+y)^0", "(x+1)");
        }

        [TestMethod]
        public void TestOnePower()
        {
            Eval("1^500", "1");
            Eval("1^x", "1");
            Eval("1^1", "1");
        }

        [TestMethod]
        public void TestDoubleNegative()
        {
            Eval("--x", "x");
            //eval("-x","-(x)");
        }

        [TestMethod]
        public void TestMinusMinus()
        {
            Eval("x--3", "(x+3)");
        }

        [TestMethod]
        public void TestPlusNeg()
        {
            Eval("x+-y", "(x-y)");
            Eval("x+-1", "(x-1)");
        }

        [TestMethod]
        public void TestVarOpVar()
        {
            Eval("x-x", "0");
            Eval("x+x", "(2*x)");
            Eval("x*x", "(x^2)");
            Eval("x/x", "1");
        }

        [TestMethod]
        public void TestMultiple()
        {
            Eval("((x+-((0-(x+x))))*x)", "((x-(0-(2*x)))*x)");
        }
    }
}
