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
