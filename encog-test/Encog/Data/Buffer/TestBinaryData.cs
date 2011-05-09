using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data.Buffer;
using Encog.ML.Data.Buffer.CODEC;
using encog_test.Neural.Networks;
using NUnit.Framework;
using Encog.Util.CSV;

namespace encog_test.Encog.Data.Buffer
{
    [TestFixture]
    public class TestBinaryData
    {
        [Test]
        public void TestArrayCODEC()
        {
            ArrayDataCODEC codec = new ArrayDataCODEC(XOR.XOR_INPUT, XOR.XOR_IDEAL);
            BinaryDataLoader loader = new BinaryDataLoader(codec);
            loader.External2Binary("encog.bin");

            ArrayDataCODEC codec2 = new ArrayDataCODEC();
            BinaryDataLoader loader2 = new BinaryDataLoader(codec2);
            loader2.Binary2External("encog.bin");

            double[][] input = codec2.Input;
            double[][] ideal = codec2.Ideal;

            for (int i = 0; i < XOR.XOR_INPUT.Length; i++)
            {
                for (int j = 0; j < XOR.XOR_INPUT[i].Length; j++)
                {
                    Assert.AreEqual(input[i][j], XOR.XOR_INPUT[i][j], 0.01);
                }

                for (int j = 0; j < XOR.XOR_IDEAL[i].Length; j++)
                {
                    Assert.AreEqual(ideal[i][j], XOR.XOR_IDEAL[i][j], 0.01);
                }
            }

        }

        [Test]
        public void TestCSV()
        {
            ArrayDataCODEC codec = new ArrayDataCODEC(XOR.XOR_INPUT, XOR.XOR_IDEAL);
            BinaryDataLoader loader = new BinaryDataLoader(codec);
            loader.External2Binary("encog.bin");

            CSVDataCODEC codec2 = new CSVDataCODEC("encog.csv", CSVFormat.ENGLISH);
            BinaryDataLoader loader2 = new BinaryDataLoader(codec2);
            loader2.Binary2External("encog.bin");

            CSVDataCODEC codec3 = new CSVDataCODEC("encog.csv", CSVFormat.ENGLISH, false, 2, 1);
            BinaryDataLoader loader3 = new BinaryDataLoader(codec3);
            loader3.External2Binary("encog.bin");

            ArrayDataCODEC codec4 = new ArrayDataCODEC();
            BinaryDataLoader loader4 = new BinaryDataLoader(codec4);
            loader4.Binary2External("encog.bin");

            double[][] input = codec4.Input;
            double[][] ideal = codec4.Ideal;

            for (int i = 0; i < XOR.XOR_INPUT.Length; i++)
            {
                for (int j = 0; j < XOR.XOR_INPUT[i].Length; j++)
                {
                    Assert.AreEqual(input[i][j], XOR.XOR_INPUT[i][j], 0.01);
                }

                for (int j = 0; j < XOR.XOR_IDEAL[i].Length; j++)
                {
                    Assert.AreEqual(ideal[i][j], XOR.XOR_IDEAL[i][j], 0.01);
                }
            }
        }
    }
}
