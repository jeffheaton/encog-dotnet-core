using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist;
using Encog.Neural.Data.Basic;
using encog_test.Neural.Networks;
using System.IO;
using Encog.Neural.Networks;
using NUnit.Framework;

namespace encog_test.Neural.Persist
{

    public class TestSort
    {
        public const String NAME_DATA1 = "data1";
        public const String NAME_DATA2 = "data2";
        public const String NAME_DATA3 = "data3";
        public const String NAME_NETWORK1 = "network1";
        public const String NAME_NETWORK2 = "network2";
        public const String NAME_NETWORK3 = "network3";


        public void testSort()
        {
            EncogPersistedCollection encog =
                new EncogPersistedCollection("encogtest3.eg", FileMode.OpenOrCreate);
            encog.Create();

            BasicNeuralDataSet data1 =
                new BasicNeuralDataSet(XOR.XOR_INPUT, XOR.XOR_IDEAL);
            BasicNeuralDataSet data2 =
                new BasicNeuralDataSet(XOR.XOR_INPUT, XOR.XOR_IDEAL);
            BasicNeuralDataSet data3 =
                new BasicNeuralDataSet(XOR.XOR_INPUT, XOR.XOR_IDEAL);

            BasicNetwork network1 = CreateNetwork.createXORNetworkUntrained();
            BasicNetwork network2 = CreateNetwork.createXORNetworkUntrained();
            BasicNetwork network3 = CreateNetwork.createXORNetworkUntrained();

            encog.Add(NAME_NETWORK3, network3);
            encog.Add(NAME_NETWORK1, network1);
            encog.Add(NAME_DATA3, data3);
            encog.Add(NAME_DATA1, data1);
            encog.Add(NAME_DATA2, data2);
            encog.Add(NAME_NETWORK2, network2);

            encog.BuildDirectory();
            IList<DirectoryEntry> dir = encog.Directory;
            Object[] dir2 = dir.ToArray();
            Assert.AreEqual(NAME_DATA1,
                    ((DirectoryEntry)dir2[0]).Name);
            Assert.AreEqual(NAME_DATA2,
                    ((DirectoryEntry)dir2[1]).Name);
            Assert.AreEqual(NAME_DATA3,
                    ((DirectoryEntry)dir2[2]).Name);

            Assert.AreEqual(NAME_NETWORK1,
                    ((DirectoryEntry)dir2[3]).Name);
            Assert.AreEqual(NAME_NETWORK2,
                    ((DirectoryEntry)dir2[4]).Name);
            Assert.AreEqual(NAME_NETWORK3,
                    ((DirectoryEntry)dir2[5]).Name);


        }
    }
}
