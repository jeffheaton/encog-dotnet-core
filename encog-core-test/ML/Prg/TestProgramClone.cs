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
using Encog.Parse.Expression.Common;

namespace Encog.ML.Prg
{
    [TestClass]
    public class TestProgramClone
    {
        [TestMethod]
        public void TestSimpleClone()
        {

            EncogProgramContext context = new EncogProgramContext();
            context.LoadAllFunctions();
            RenderCommonExpression render = new RenderCommonExpression();

            EncogProgram prg1 = context.CreateProgram("1*2*3");
            EncogProgram prg2 = context.CloneProgram(prg1);

            Assert.AreEqual("((1*2)*3)", render.Render(prg1));
            Assert.AreEqual("((1*2)*3)", render.Render(prg2));
        }

        [TestMethod]
        public void TestCloneVar()
        {

            EncogProgramContext context = new EncogProgramContext();
            context.LoadAllFunctions();
            context.DefineVariable("x");
            RenderCommonExpression render = new RenderCommonExpression();

            EncogProgram prg1 = context.CreateProgram("x*2*3");
            EncogProgram prg2 = context.CloneProgram(prg1);

            Assert.AreEqual("((x*2)*3)", render.Render(prg1));
            Assert.AreEqual("((x*2)*3)", render.Render(prg2));
        }

        [TestMethod]
        public void TestCloneComplex()
        {

            EncogProgramContext context = new EncogProgramContext();
            context.LoadAllFunctions();
            context.DefineVariable("a");
            RenderCommonExpression render = new RenderCommonExpression();

            EncogProgram prg1 = context.CreateProgram("((a+25)^3/25)-((a*3)^4/250)");
            EncogProgram prg2 = context.CloneProgram(prg1);

            Assert.AreEqual("((((a+25)^3)/25)-(((a*3)^4)/250))", render.Render(prg1));
            Assert.AreEqual("((((a+25)^3)/25)-(((a*3)^4)/250))", render.Render(prg2));
        }
    }
}
