using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.ML.Prg;
using Encog.ML.Prg.Ext;
using Encog.ML.Prg.Generator;
using Encog.Parse.Expression.Common;
using Encog.MathUtil.Randomize;

namespace Encog.ML.KMeans.Train
{
    [TestClass]
    public class TestGenerate
    {
        [TestMethod]
        public void TestDepth()
        {
            EncogProgramContext context = new EncogProgramContext();
            context.DefineVariable("x");

            StandardExtensions.CreateAll(context);

            PrgGrowGenerator rnd = new PrgGrowGenerator(context, 2);
            EncogProgram prg = (EncogProgram)rnd.Generate(new EncogRandom());
            RenderCommonExpression render = new RenderCommonExpression();
        }
    }
}
   
