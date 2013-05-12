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
