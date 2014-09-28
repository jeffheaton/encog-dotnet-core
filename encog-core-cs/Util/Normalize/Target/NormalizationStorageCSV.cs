//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.IO;
using System.Text;
using Encog.Util.CSV;

namespace Encog.Util.Normalize.Target
{
    /// <summary>
    /// Store normalized data to a CSV file.
    /// </summary>
    [Serializable]
    public class NormalizationStorageCSV : INormalizationStorage
    {
        /// <summary>
        /// The CSV format to use.
        /// </summary>
        private readonly CSVFormat _format;

        /// <summary>
        /// The output file.
        /// </summary> 
        private readonly String _outputFile;

        /// <summary>
        /// The output writer.
        /// </summary>
        private StreamWriter _output;

        /// <summary>
        /// Construct a CSV storage object from the specified file.
        /// </summary>
        /// <param name="format">The format to use.</param>
        /// <param name="file">The file to write the CSV to.</param>
        public NormalizationStorageCSV(CSVFormat format, String file)
        {
            _format = format;
            _outputFile = file;
        }

        /// <summary>
        /// Construct a CSV storage object from the specified file.
        /// </summary>
        /// <param name="file">The file to write the CSV to.</param>
        public NormalizationStorageCSV(String file)
        {
            _format = CSVFormat.English;
            _outputFile = file;
        }

        #region INormalizationStorage Members


        /// <summary>
        /// Close the CSV file.
        /// </summary>
        public void Close()
        {
            _output.Close();
        }

        /// <summary>
        /// Open the CSV file.
        /// </summary>
        public void Open()
        {
            _output = new StreamWriter(_outputFile);
        }

        /// <summary>
        /// Write an array.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="inputCount"> How much of the data is input.</param>
        public void Write(double[] data, int inputCount)
        {
            var result = new StringBuilder();
            NumberList.ToList(_format, result, data);
            _output.WriteLine(result.ToString());
        }

        #endregion
    }
}
