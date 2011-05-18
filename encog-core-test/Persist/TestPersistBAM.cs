using System.IO;
using Encog.Neural.BAM;
using Encog.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Persist
{
    [TestClass]
    public class TestPersistBAM
    {
        public static readonly TempDir TEMP_DIR = new TempDir();
        public readonly FileInfo EG_FILENAME = TEMP_DIR.CreateFile("encogtest.eg");
        public readonly FileInfo SERIAL_FILENAME = TEMP_DIR.CreateFile("encogtest.ser");

        private BAMNetwork Create()
        {
            var network = new BAMNetwork(6, 3);
            network.WeightsF1toF2[1, 1] = 2.0;
            network.WeightsF2toF1[2, 2] = 3.0;
            return network;
        }

        [TestMethod]
        public void TestPersistEG()
        {
            BAMNetwork network = Create();

            EncogDirectoryPersistence.SaveObject(EG_FILENAME, network);
            var network2 = (BAMNetwork) EncogDirectoryPersistence.LoadObject(EG_FILENAME);

            ValidateBAM(network2);
        }

        [TestMethod]
        public void TestPersistSerial()
        {
            BAMNetwork network = Create();

            SerializeObject.Save(SERIAL_FILENAME.ToString(), network);
            var network2 = (BAMNetwork) SerializeObject.Load(SERIAL_FILENAME.ToString());

            ValidateBAM(network2);
        }

        private void ValidateBAM(BAMNetwork network)
        {
            Assert.AreEqual(6, network.F1Count);
            Assert.AreEqual(3, network.F2Count);
            Assert.AreEqual(18, network.WeightsF1toF2.Size);
            Assert.AreEqual(18, network.WeightsF2toF1.Size);
            Assert.AreEqual(2.0, network.WeightsF1toF2[1, 1]);
            Assert.AreEqual(3.0, network.WeightsF2toF1[2, 2]);
        }
    }
}