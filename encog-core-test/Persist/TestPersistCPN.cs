using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.Util;
using System.IO;
using Encog.Neural.CPN;

namespace Encog.Persist
{
    [TestClass]
    public class TestPersistCPN
    {
        public static readonly TempDir TEMP_DIR = new TempDir();
        public readonly FileInfo EG_FILENAME = TEMP_DIR.CreateFile("encogtest.eg");
        public readonly FileInfo SERIAL_FILENAME = TEMP_DIR.CreateFile("encogtest.ser");

        private CPNNetwork Create()
        {
            CPNNetwork result = new CPNNetwork(5, 4, 3, 2);
            return result;
        }

        [TestMethod]
        public void TestPersistEG()
        {
            CPNNetwork network = Create();

            EncogDirectoryPersistence.SaveObject(EG_FILENAME, network);
            CPNNetwork network2 = (CPNNetwork)EncogDirectoryPersistence.LoadObject(EG_FILENAME);

            Validate(network2);
        }

        [TestMethod]
        public void TestPersistSerial()
        {
            CPNNetwork network = Create();

            SerializeObject.Save(SERIAL_FILENAME.ToString(), network);
            CPNNetwork network2 = (CPNNetwork)SerializeObject.Load(SERIAL_FILENAME.ToString());

            Validate(network2);
        }

        private void Validate(CPNNetwork cpn)
        {
            Assert.AreEqual(5, cpn.InputCount);
            Assert.AreEqual(4, cpn.InstarCount);
            Assert.AreEqual(3, cpn.OutputCount);
            Assert.AreEqual(3, cpn.OutstarCount);
            Assert.AreEqual(2, cpn.WinnerCount);
            Assert.AreEqual(5, cpn.WeightsInputToInstar.Rows);
            Assert.AreEqual(4, cpn.WeightsInputToInstar.Cols);
            Assert.AreEqual(4, cpn.WeightsInstarToOutstar.Rows);
            Assert.AreEqual(3, cpn.WeightsInstarToOutstar.Cols);
        }
    }
}
