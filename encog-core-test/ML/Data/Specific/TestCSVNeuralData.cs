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

using System.IO;
using System.Text;
using Encog.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.ML.Data.Specific
{
    /// <summary>
    /// Description of TestCSVNeuralData.
    /// </summary>
    [TestClass]
    public class TestCSVNeuralData
    {
        public const string Filename = "xor.csv";

        private static void GenerateCSV()
        {
            TextWriter fp = new StreamWriter(Filename);

            for (int count = 0; count < XOR.XORInput.Length; count++)
            {
                var builder = new StringBuilder();

                for (int i = 0; i < XOR.XORInput[0].Length; i++)
                {
                    if (builder.Length > 0)
                        builder.Append(',');
                    builder.Append(XOR.XORInput[count][i]);
                }

                for (int i = 0; i < XOR.XORIdeal[0].Length; i++)
                {
                    if (builder.Length > 0)
                        builder.Append(',');
                    builder.Append(XOR.XORIdeal[count][i]);
                }
                fp.WriteLine(builder.ToString());
            }
            fp.Close();
        }

        [TestMethod]
        public void CSVData()
        {
            GenerateCSV();

            var set = new CSVMLDataSet("xor.csv", 2, 1, false);

            XOR.TestXORDataSet(set);

            set.Close();
            File.Delete(Filename);
        }
    }
}