using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.Parse.Expression.Common;
using Encog.ML.Prg;

namespace Encog.ML.Prg.Train.Crossover
{
    [TestClass]
    public class TestSubtreeCrossover
    {
        [TestMethod]
        public void TestCrossoverOperation()
        {
            RenderCommonExpression render = new RenderCommonExpression();
            EncogProgram prg = new EncogProgram("1+2");
            EncogProgram prg2 = new EncogProgram("4+5");
            ProgramNode node = prg.FindNode(2);
            prg.ReplaceNode(node, prg2.RootNode);
            Assert.AreEqual("(1+(4+5))", render.Render(prg));
        }
    }
}
