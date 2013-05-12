using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.ML.Prg;

namespace Encog.Parse.Expression.Common
{
    [TestClass]
    public class TestRenderCommon
    {
        [TestMethod]
        public void TestRenderBasic()
        {
            EncogProgram expression = new EncogProgram("(2+6)");
            RenderCommonExpression render = new RenderCommonExpression();
            String result = render.Render(expression);
            Assert.AreEqual("(2+6)", result);
        }

        [TestMethod]
        public void TestRenderComplex()
        {
            EncogProgram expression = new EncogProgram("((a+25)^3/25)-((a*3)^4/250)");
            RenderCommonExpression render = new RenderCommonExpression();
            String result = render.Render(expression);
            Assert.AreEqual("((((a+25)^3)/25)-(((a*3)^4)/250))", result);
        }

        [TestMethod]
        public void TestRenderFunction()
        {
            EncogProgram expression = new EncogProgram("(sin(x)+cos(x))/2");
            RenderCommonExpression render = new RenderCommonExpression();
            String result = render.Render(expression);
            Assert.AreEqual("((sin(x)+cos(x))/2)", result);
        }

        [TestMethod]
        public void TestKnownConst()
        {
            EncogProgram expression = new EncogProgram("x*2*PI");
            RenderCommonExpression render = new RenderCommonExpression();
            String result = render.Render(expression);
            Assert.AreEqual("((x*2)*PI)", result);
        }
    }
}
