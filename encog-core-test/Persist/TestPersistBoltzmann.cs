using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.Util;
using System.IO;
using Encog.Neural.Thermal;

namespace Encog.Persist
{
    [TestClass]
    public class TestPersistBoltzmann
    {
        public static readonly TempDir TEMP_DIR = new TempDir();
        public readonly FileInfo EG_FILENAME = TEMP_DIR.CreateFile("encogtest.eg");
        public readonly FileInfo SERIAL_FILENAME = TEMP_DIR.CreateFile("encogtest.ser");

        [TestMethod]
        public void TestPersistEG()
        {
            BoltzmannMachine network = new BoltzmannMachine(4);
            network.SetWeight(1, 1, 1);
            network.Threshold[2] = 2;

            EncogDirectoryPersistence.SaveObject(EG_FILENAME, network);
            BoltzmannMachine network2 = (BoltzmannMachine)EncogDirectoryPersistence.LoadObject(EG_FILENAME);

            ValidateHopfield(network2);
        }

        [TestMethod]
        public void TestPersistSerial()
        {
            BoltzmannMachine network = new BoltzmannMachine(4);
            network.SetWeight(1, 1, 1);
            network.Threshold[2] = 2;

            SerializeObject.Save(SERIAL_FILENAME.ToString(), network);
            BoltzmannMachine network2 = (BoltzmannMachine)SerializeObject.Load(SERIAL_FILENAME.ToString());

            ValidateHopfield(network2);
        }

        private void ValidateHopfield(BoltzmannMachine network)
        {
            network.Run();// at least see if it can run without an exception
            Assert.AreEqual(4, network.NeuronCount);
            Assert.AreEqual(4, network.CurrentState.Count);
            Assert.AreEqual(16, network.Weights.Length);
            Assert.AreEqual(1.0, network.GetWeight(1, 1));
            Assert.AreEqual(2.0, network.Threshold[2]);
        }

    }
}
