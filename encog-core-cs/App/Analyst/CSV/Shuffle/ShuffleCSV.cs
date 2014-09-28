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
using System.IO;
using Encog.App.Analyst.CSV.Basic;
using Encog.MathUtil.Randomize;
using Encog.Util.CSV;

namespace Encog.App.Analyst.CSV.Shuffle
{
    /// <summary>
    ///     Randomly shuffle the lines of a CSV file.
    /// </summary>
    public class ShuffleCSV : BasicFile
    {
        /// <summary>
        ///     The default buffer size.
        /// </summary>
        public const int DefaultBufferSize = 5000;

        /// <summary>
        ///     The buffer.
        /// </summary>
        private LoadedRow[] _buffer;

        /// <summary>
        ///     The buffer size.
        /// </summary>
        private int _bufferSize;

        /// <summary>
        ///     Remaining in the buffer.
        /// </summary>
        private int _remaining;

        /// <summary>
        ///     Construct the object.
        /// </summary>
        public ShuffleCSV()
        {
            BufferSize = DefaultBufferSize;
        }

        /// <summary>
        ///     The buffer size. This is how many rows of data are loaded(and
        ///     randomized), at a time. The default is 5,000.
        /// </summary>
        public int BufferSize
        {
            get { return _bufferSize; }
            set
            {
                _bufferSize = value;
                _buffer = new LoadedRow[_bufferSize];
            }
        }

        /// <summary>
        ///     Analyze the neural network.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <param name="headers">True, if there are headers.</param>
        /// <param name="format">The format of the CSV file.</param>
        public void Analyze(FileInfo inputFile, bool headers,
                            CSVFormat format)
        {
            InputFilename = inputFile;
            ExpectInputHeaders = headers;
            Format = format;

            Analyzed = true;

            PerformBasicCounts();
        }


        /// <summary>
        ///     Get the next row from the underlying CSV file.
        /// </summary>
        /// <param name="csv">The underlying CSV file.</param>
        /// <returns>The loaded row.</returns>
        private LoadedRow GetNextRow(ReadCSV csv)
        {
            if (_remaining == 0)
            {
                LoadBuffer(csv);
            }

            while (_remaining > 0)
            {
                int index = RangeRandomizer.RandomInt(0, _bufferSize - 1);
                if (_buffer[index] != null)
                {
                    LoadedRow result = _buffer[index];
                    _buffer[index] = null;
                    _remaining--;
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        ///     Load the buffer from the underlying file.
        /// </summary>
        /// <param name="csv">The CSV file to load from.</param>
        private void LoadBuffer(ReadCSV csv)
        {
            for (int i = 0; i < _buffer.Length; i++)
            {
                _buffer[i] = null;
            }

            int index = 0;
            while (csv.Next() && (index < _bufferSize) && !ShouldStop())
            {
                var row = new LoadedRow(csv);
                _buffer[index++] = row;
            }

            _remaining = index;
        }

        /// <summary>
        ///     Process, and generate the output file.
        /// </summary>
        /// <param name="outputFile">The output file.</param>
        public void Process(FileInfo outputFile)
        {
            ValidateAnalyzed();

            var csv = new ReadCSV(InputFilename.ToString(),
                                  ExpectInputHeaders, Format);
            LoadedRow row;

            StreamWriter tw = PrepareOutputFile(outputFile);

            ResetStatus();
            while ((row = GetNextRow(csv)) != null)
            {
                WriteRow(tw, row);
                UpdateStatus(false);
            }
            ReportDone(false);
            tw.Close();
            csv.Close();
        }
    }
}
