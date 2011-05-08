using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data.Basic;
using NUnit.Framework;
using System.IO;
using Encog.Neural.Data.Buffer;
using encog_test.Neural.Networks;

namespace encog_test.Encog.Data.Buffer
{
    [TestFixture]
    public class TestBufferedNeuralDataSet
    {
        public readonly static String FILENAME = "xor.bin";

        [Test]
        public void TestBufferData()
        {
            File.Delete(FILENAME);
            BufferedMlDataSet set = new BufferedMlDataSet(FILENAME);
            set.BeginLoad(2, 1);
            for (int i = 0; i < XOR.XOR_INPUT.Length; i++)
            {
                BasicMLData input = new BasicMLData(XOR.XOR_INPUT[i]);
                BasicMLData ideal = new BasicMLData(XOR.XOR_IDEAL[i]);
                set.Add(input, ideal);
            }
            set.EndLoad();

            XOR.TestXORDataSet(set);

        }
    }
}
