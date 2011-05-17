using System.IO;
using Encog.ML.Data.Basic;
using Encog.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.ML.Data.Buffer
{
    [TestClass]
    public class TestBufferedNeuralDataSet
    {
        public static readonly string Filename = "xor.bin";

        [TestMethod]
        public void TestBufferData()
        {
            File.Delete(Filename);
            var set = new BufferedMLDataSet(Filename);
            set.BeginLoad(2, 1);
            for (int i = 0; i < XOR.XORInput.Length; i++)
            {
                var input = new BasicMLData(XOR.XORInput[i]);
                var ideal = new BasicMLData(XOR.XORIdeal[i]);
                set.Add(input, ideal);
            }
            set.EndLoad();

            XOR.TestXORDataSet(set);
        }
    }
}