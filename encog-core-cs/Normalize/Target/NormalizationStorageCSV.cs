// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;
using System.IO;

namespace Encog.Normalize.Target
{
    /// <summary>
    /// Store normalized data to a CSV file.
    /// </summary>
    public class NormalizationStorageCSV : INormalizationStorage
    {
        /// <summary>
        /// The output file.
        /// </summary>
        private String outputFile;

        /// <summary>
        /// The output writer.
        /// </summary>
        private StreamWriter output;

        /// <summary>
        /// The CSV format to use.
        /// </summary>
        private CSVFormat format;

        /// <summary>
        /// Construct a CSV storage object from the specified file.
        /// </summary>
        /// <param name="format">The format to use.</param>
        /// <param name="file">The file to write the CSV to.</param>
        public NormalizationStorageCSV(CSVFormat format, String file)
        {
            this.format = format;
            this.outputFile = file;
        }

        /// <summary>
        /// Construct a CSV storage object from the specified file.
        /// </summary>
        /// <param name="file">The file to write the CSV to.</param>
        public NormalizationStorageCSV(String file)
        {
            this.format = CSVFormat.ENGLISH;
            this.outputFile = file;
        }

        /// <summary>
        /// Close the CSV file.
        /// </summary>
        public void Close()
        {
            this.output.Close();
        }

        /// <summary>
        /// Open the CSV file.
        /// </summary>
        public void Open()
        {
            this.output = new StreamWriter(this.outputFile);
        }

        /// <summary>
        /// Write an array.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="inputCount"> How much of the data is input.</param>
        public void Write(double[] data, int inputCount)
        {
            StringBuilder result = new StringBuilder();
            NumberList.ToList(this.format, result, data);
            this.output.WriteLine(result.ToString());
        }
    }
}
