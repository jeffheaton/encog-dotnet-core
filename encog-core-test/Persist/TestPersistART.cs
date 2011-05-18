using System.IO;
using Encog.Neural.ART;
using Encog.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Persist
{
    [TestClass]
    public class TestPersistART
    {
        public static readonly TempDir TEMP_DIR = new TempDir();
        public readonly FileInfo EG_FILENAME = TEMP_DIR.CreateFile("encogtest.eg");
        public readonly FileInfo SERIAL_FILENAME = TEMP_DIR.CreateFile("encogtest.ser");

        private ART1 Create()
        {
            var network = new ART1(6, 3);
            network.WeightsF1toF2[1, 1] = 2.0;
            network.WeightsF2toF1[2, 2] = 3.0;
            return network;
        }


        private void Validate(ART1 network)
        {
            Assert.AreEqual(6, network.F1Count);
            Assert.AreEqual(3, network.F2Count);
            Assert.AreEqual(18, network.WeightsF1toF2.Size);
            Assert.AreEqual(18, network.WeightsF2toF1.Size);
            Assert.AreEqual(2.0, network.WeightsF1toF2[1, 1]);
            Assert.AreEqual(3.0, network.WeightsF2toF1[2, 2]);
            Assert.AreEqual(1.0, network.A1);
            Assert.AreEqual(1.5, network.B1);
            Assert.AreEqual(5.0, network.C1);
            Assert.AreEqual(0.9, network.D1);
            Assert.AreEqual(0.9, network.Vigilance);
        }

        [TestMethod]
        public void TestPersistEG()
        {
            ART1 network = Create();

            EncogDirectoryPersistence.SaveObject(EG_FILENAME, network);
            var network2 = (ART1) EncogDirectoryPersistence.LoadObject(EG_FILENAME);

            Validate(network2);
        }

        [TestMethod]
        public void TestPersistSerial()
        {
            ART1 network = Create();

            SerializeObject.Save(SERIAL_FILENAME.ToString(), network);
            var network2 = (ART1) SerializeObject.Load(SERIAL_FILENAME.ToString());

            Validate(network2);
        }
    }
}