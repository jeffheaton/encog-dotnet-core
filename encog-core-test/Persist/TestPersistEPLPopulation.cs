using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.Util;
using System.IO;
using Encog.ML.Prg.Train;
using Encog.ML.Prg;
using Encog.ML.Prg.Ext;
using Encog.ML.EA.Species;
using Encog.ML.EA.Genome;
using Encog.Parse.Expression.Common;

namespace Encog.Persist
{
    [TestClass]
    public class TestPersistEPLPopulation
    {
        public static readonly TempDir TEMP_DIR = new TempDir();
        public readonly FileInfo EG_FILENAME = TEMP_DIR.CreateFile("encogtest.eg");
        public readonly FileInfo SERIAL_FILENAME = TEMP_DIR.CreateFile("encogtest.ser");

        private PrgPopulation Create()
        {
            EncogProgramContext context = new EncogProgramContext();
            context.DefineVariable("x");
            StandardExtensions.CreateAll(context);
            PrgPopulation pop = new PrgPopulation(context, 10);
            EncogProgram prg1 = new EncogProgram(context);
            EncogProgram prg2 = new EncogProgram(context);
            prg1.CompileExpression("x+1");
            prg2.CompileExpression("(x+5)/2");

            ISpecies defaultSpecies = pop.CreateSpecies();
            defaultSpecies.Add(prg1);
            defaultSpecies.Add(prg2);
            return pop;
        }

        [TestMethod]
        public void TestPersistEG()
        {
            PrgPopulation pop = Create();
            EncogDirectoryPersistence.SaveObject(EG_FILENAME, pop);
            PrgPopulation pop2 = (PrgPopulation)EncogDirectoryPersistence.LoadObject(EG_FILENAME);
            Validate(pop2);
        }

        [TestMethod]
        public void testPersistSerial()
        {
            PrgPopulation pop = Create();
            Validate(pop);
            SerializeObject.Save(SERIAL_FILENAME.ToString(), pop);
            PrgPopulation pop2 = (PrgPopulation)SerializeObject.Load(SERIAL_FILENAME.ToString());
            Validate(pop2);
        }

        private void Validate(PrgPopulation pop)
        {
            IList<IGenome> list = pop.Flatten();
            Assert.AreEqual(2, list.Count);

            EncogProgram prg1 = (EncogProgram)list[0];
            EncogProgram prg2 = (EncogProgram)list[1];

            RenderCommonExpression render = new RenderCommonExpression();
            Assert.AreEqual("(x+1)", render.Render(prg1));
            Assert.AreEqual("((x+5)/2)", render.Render(prg2));
        }
    }
}
