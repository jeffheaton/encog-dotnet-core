using System;
using System.IO;
using Encog.Neural.Freeform;
using Encog.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Persist
{
    [TestClass]
    public class TestPersistFreeform
    {
        public static readonly TempDir TEMP_DIR = new TempDir();
        public readonly FileInfo EG_FILENAME = TEMP_DIR.CreateFile("encogtest.eg");
        public readonly FileInfo SERIAL_FILENAME = TEMP_DIR.CreateFile("encogtest.ser");

        public FreeformNetwork Create()
        {
            FreeformNetwork network = XOR.CreateTrainedFreeformXOR();
            XOR.VerifyXOR(network, 0.1);

            network.SetProperty("test", "test2");

            return network;
        }

        public void Validate(FreeformNetwork network)
        {
            network.ClearContext();
            XOR.VerifyXOR(network, 0.1);
        }

        [TestMethod]
        public void TestPersistSerial()
        {
            var network = Create();

            SerializeObject.Save(SERIAL_FILENAME.ToString(), network);
            var network2 = (FreeformNetwork)SerializeObject.Load(SERIAL_FILENAME.ToString());

            Validate(network2);
        }
    }
}
