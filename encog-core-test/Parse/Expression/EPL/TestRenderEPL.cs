using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.ML.Prg;

namespace Encog.Parse.Expression.EPL
{
    [TestClass]
    public class TestRenderEPL
    {
        [TestMethod]
        public void TestRenderBasic()
        {
            EncogProgram expression = new EncogProgram("(2+6)");
            RenderEPL render = new RenderEPL();
            String result = render.Render(expression);
            Assert.AreEqual("[#const:0:2][#const:0:6][+:2]", result);
        }

        [TestMethod]
        public void TestRenderComplex()
        {
            EncogProgram expression = new EncogProgram("((a+25)^3/25)-((a*3)^4/250)");
            RenderEPL render = new RenderEPL();
            String result = render.Render(expression);
            Assert.AreEqual("[#var:0:0][#const:0:25][+:2][#const:0:3][^:2][#const:0:25][/:2][#var:0:0][#const:0:3][*:2][#const:0:4][^:2][#const:0:250][/:2][-:2]", result);
        }

        [TestMethod]
        public void TestRenderFunction()
        {
            EncogProgram expression = new EncogProgram("(sin(x)+cos(x))/2");
            RenderEPL render = new RenderEPL();
            String result = render.Render(expression);
            Assert.AreEqual("[#var:0:0][sin:1][#var:0:0][cos:1][+:2][#const:0:2][/:2]", result);
        }
    }
}
