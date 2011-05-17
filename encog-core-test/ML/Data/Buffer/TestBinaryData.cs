using Encog.ML.Data.Buffer.CODEC;
using Encog.Util;
using Encog.Util.CSV;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.ML.Data.Buffer
{
    [TestClass]
    public class TestBinaryData
    {
        [TestMethod]
        public void TestArrayCODEC()
        {
            var codec = new ArrayDataCODEC(XOR.XORInput, XOR.XORIdeal);
            var loader = new BinaryDataLoader(codec);
            loader.External2Binary("encog.bin");

            var codec2 = new ArrayDataCODEC();
            var loader2 = new BinaryDataLoader(codec2);
            loader2.Binary2External("encog.bin");

            double[][] input = codec2.Input;
            double[][] ideal = codec2.Ideal;

            for (int i = 0; i < XOR.XORInput.Length; i++)
            {
                for (int j = 0; j < XOR.XORInput[i].Length; j++)
                {
                    Assert.AreEqual(input[i][j], XOR.XORInput[i][j], 0.01);
                }

                for (int j = 0; j < XOR.XORIdeal[i].Length; j++)
                {
                    Assert.AreEqual(ideal[i][j], XOR.XORIdeal[i][j], 0.01);
                }
            }
        }

        [TestMethod]
        public void TestCSV()
        {
            var codec = new ArrayDataCODEC(XOR.XORInput, XOR.XORIdeal);
            var loader = new BinaryDataLoader(codec);
            loader.External2Binary("encog.bin");

            var codec2 = new CSVDataCODEC("encog.csv", CSVFormat.ENGLISH);
            var loader2 = new BinaryDataLoader(codec2);
            loader2.Binary2External("encog.bin");

            var codec3 = new CSVDataCODEC("encog.csv", CSVFormat.ENGLISH, false, 2, 1);
            var loader3 = new BinaryDataLoader(codec3);
            loader3.External2Binary("encog.bin");

            var codec4 = new ArrayDataCODEC();
            var loader4 = new BinaryDataLoader(codec4);
            loader4.Binary2External("encog.bin");

            double[][] input = codec4.Input;
            double[][] ideal = codec4.Ideal;

            for (int i = 0; i < XOR.XORInput.Length; i++)
            {
                for (int j = 0; j < XOR.XORInput[i].Length; j++)
                {
                    Assert.AreEqual(input[i][j], XOR.XORInput[i][j], 0.01);
                }

                for (int j = 0; j < XOR.XORIdeal[i].Length; j++)
                {
                    Assert.AreEqual(ideal[i][j], XOR.XORIdeal[i][j], 0.01);
                }
            }
        }
    }
}