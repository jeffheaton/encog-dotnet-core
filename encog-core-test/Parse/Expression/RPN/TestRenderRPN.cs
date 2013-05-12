using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.ML.Prg;

namespace Encog.Parse.Expression.RPN
{
    [TestClass]
    public class TestRenderRPN
    {
        [TestMethod]
        public void TestRenderBasic()
        {
            EncogProgram expression = new EncogProgram("(2+6)");
            RenderRPN render = new RenderRPN();
            String result = render.Render(expression);
            Assert.AreEqual("2 6 [+]", result);
        }

        [TestMethod]
        public void TestRenderComplex()
        {
            EncogProgram expression = new EncogProgram("((a+25)^3/25)-((a*3)^4/250)");
            RenderRPN render = new RenderRPN();
            String result = render.Render(expression);
            Assert.AreEqual("a 25 [+] 3 [^] 25 [/] a 3 [*] 4 [^] 250 [/] [-]", result);
        }

        [TestMethod]
        public void TestRenderFunction()
        {
            EncogProgram expression = new EncogProgram("(sin(x)+cos(x))/2");
            RenderRPN render = new RenderRPN();
            String result = render.Render(expression);
            Assert.AreEqual("x [sin] x [cos] [+] 2 [/]", result);
        }

        [TestMethod]
        public void TestKnownConst()
        {
            EncogProgram expression = new EncogProgram("x*2*PI");
            RenderRPN render = new RenderRPN();
            String result = render.Render(expression);
            Assert.AreEqual("x 2 [*] PI [*]", result);
        }
    }
}
