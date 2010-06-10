using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using encog_test.Neural.Networks;
using Encog.Neural.Data.XML;

namespace encog_test.Data.XML
{
    [TestFixture]
    public class TestXMLNeuralData
    {
        public const String FILENAME = "test.xml";

        private void GenerateXML()
        {
            TextWriter ps = new StreamWriter(TestXMLNeuralData.FILENAME);
            ps.WriteLine("<DataSet>");
            ps.WriteLine("<pair><input><value>0</value><value>0</value></input><ideal><value>0</value></ideal></pair>");
            ps.WriteLine("<pair><input><value>1</value><value>0</value></input><ideal><value>1</value></ideal></pair>");
            ps.WriteLine("<pair><input><value>0</value><value>1</value></input><ideal><value>1</value></ideal></pair>");
            ps.WriteLine("<pair><input><value>1</value><value>1</value></input><ideal><value>0</value></ideal></pair>");
            ps.WriteLine("</DataSet>");
            ps.Close();
        }

        [Test]
        public void XMLNeuralData()
        {
            GenerateXML();
            XMLNeuralDataSet set = new XMLNeuralDataSet(
                    TestXMLNeuralData.FILENAME,2,1,
                    "pair",
                    "input",
                    "ideal",
                    "value");

            Assert.IsTrue(set.InputSize == 2);
            Assert.IsTrue(set.IdealSize == 1);
            XOR.TestXORDataSet(set);

        }
    }
}
