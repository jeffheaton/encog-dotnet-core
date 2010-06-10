/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 11/22/2008
 * Time: 8:17 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NUnit.Framework;
using System.IO;
using System.Text;
using Encog.Neural.NeuralData.CSV;
using encog_test.Neural.Networks;

namespace encog_test.Data.CSV
{
    /// <summary>
    /// Description of TestCSVNeuralData.
    /// </summary>
    [TestFixture]
    public class TestCSVNeuralData
    {
        public const String FILENAME = "xor.csv";

        private void GenerateCSV()
        {
            TextWriter fp = new StreamWriter(TestCSVNeuralData.FILENAME);

            for (int count = 0; count < XOR.XOR_INPUT.Length; count++)
            {
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < XOR.XOR_INPUT[0].Length; i++)
                {
                    if (builder.Length > 0)
                        builder.Append(',');
                    builder.Append(XOR.XOR_INPUT[count][i]);
                }

                for (int i = 0; i < XOR.XOR_IDEAL[0].Length; i++)
                {
                    if (builder.Length > 0)
                        builder.Append(',');
                    builder.Append(XOR.XOR_IDEAL[count][i]);
                }
                fp.WriteLine(builder.ToString());
            }
            fp.Close();
        }

        [Test]
        public void CSVData()
        {
            GenerateCSV();

            CSVNeuralDataSet set = new CSVNeuralDataSet("xor.csv", 2, 1, false);

            XOR.TestXORDataSet(set);

            set.Close();
            File.Delete(TestCSVNeuralData.FILENAME);

        }
    }
}
