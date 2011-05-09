// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using Encog.ML.Data.Specific;
using NUnit.Framework;
using System.IO;
using System.Text;
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

            CsvMlDataSet set = new CsvMlDataSet("xor.csv", 2, 1, false);

            XOR.TestXORDataSet(set);

            set.Close();
            File.Delete(TestCSVNeuralData.FILENAME);

        }
    }
}
