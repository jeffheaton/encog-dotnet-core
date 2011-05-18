using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.Util;
using System.IO;
using Encog.Neural.Networks;

namespace Encog.Persist
{
    [TestClass]
    public class TestPersistBasicNetwork
    {
        public static readonly TempDir TEMP_DIR = new TempDir();
        public readonly FileInfo EG_FILENAME = TEMP_DIR.CreateFile("encogtest.eg");
        public readonly FileInfo SERIAL_FILENAME = TEMP_DIR.CreateFile("encogtest.ser");

        public BasicNetwork Create()
        {
            BasicNetwork network = XOR.CreateTrainedXOR();
            XOR.VerifyXOR(network, 0.1);

            network.SetProperty("test", "test2");


            return network;
        }

        public void Validate(BasicNetwork network)
        {
            network.ClearContext();
            XOR.VerifyXOR(network, 0.1);
        }
        [TestMethod]
        public void TestPersistEG()
        {
            BasicNetwork network = Create();

            EncogDirectoryPersistence.SaveObject(EG_FILENAME, network);
            BasicNetwork network2 = (BasicNetwork)EncogDirectoryPersistence.LoadObject(EG_FILENAME);

            Validate(network2);
        }

        [TestMethod]
        public void TestPersistSerial()
        {
            BasicNetwork network = Create();

            SerializeObject.Save(SERIAL_FILENAME.ToString(), network);
            BasicNetwork network2 = (BasicNetwork)SerializeObject.Load(SERIAL_FILENAME.ToString());

            Validate(network2);
        }


    }
}
