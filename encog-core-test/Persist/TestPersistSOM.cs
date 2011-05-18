using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.Util;
using System.IO;
using Encog.Neural.SOM;

namespace Encog.Persist
{
    [TestClass]
    public class TestPersistSOM
    {
        public static readonly TempDir TEMP_DIR = new TempDir();
        public readonly FileInfo EG_FILENAME = TEMP_DIR.CreateFile("encogtest.eg");
        public readonly FileInfo SERIAL_FILENAME = TEMP_DIR.CreateFile("encogtest.ser");

        private SOMNetwork Create()
        {
            SOMNetwork network = new SOMNetwork(4, 2);
            return network;
        }

        [TestMethod]
        public void TestPersistEG()
        {
            SOMNetwork network = Create();

            EncogDirectoryPersistence.SaveObject(EG_FILENAME, network);
            SOMNetwork network2 = (SOMNetwork)EncogDirectoryPersistence.LoadObject(EG_FILENAME);

            Validate(network2);
        }

        [TestMethod]
        public void TestPersistSerial()
        {
            SOMNetwork network = Create();
            SerializeObject.Save(SERIAL_FILENAME.ToString(), network);
            SOMNetwork network2 = (SOMNetwork)SerializeObject.Load(SERIAL_FILENAME.ToString());

            Validate(network2);
        }

        private void Validate(SOMNetwork network)
        {
            Assert.AreEqual(4, network.InputNeuronCount);
            Assert.AreEqual(2, network.OutputNeuronCount);
            Assert.AreEqual(8, network.Weights.ToPackedArray().Length);
        }
    }
}
