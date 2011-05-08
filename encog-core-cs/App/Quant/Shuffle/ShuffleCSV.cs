using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;
using System.Collections;
using System.IO;
using Encog.App.Quant.Basic;
using Encog.MathUtil.Randomize;

namespace Encog.App.Quant.Shuffle
{
    /// <summary>
    /// Randomly shuffle the lines of a CSV file.
    /// </summary>
    public class ShuffleCSV : BasicFile
    {
        /// <summary>
        /// The buffer size.
        /// </summary>
        private int bufferSize;

        /// <summary>
        /// The buffer.
        /// </summary>
        private LoadedRow[] buffer;

        /// <summary>
        /// Remaining in the buffer.
        /// </summary>
        private int remaining;

        /// <summary>
        /// The buffer size.  This is how many rows of data are loaded(and randomized),
        /// at a time. The default is 5,000.
        /// </summary>
        private int BufferSize
        {
            get
            {
                return this.bufferSize;
            }
            set
            {
                this.bufferSize = value;
                this.buffer = new LoadedRow[this.bufferSize];
            }
        }

        /// <summary>
        /// Construct the object
        /// </summary>
        public ShuffleCSV()
        {
            this.BufferSize = 5000;
        }


        /// <summary>
        /// Analyze the neural network.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <param name="headers">True, if there are headers.</param>
        /// <param name="format">The format of the CSV file.</param>
        public void Analyze(String inputFile, bool headers, CSVFormat format)
        {
            this.InputFilename = inputFile;
            this.ExpectInputHeaders = headers;
            this.InputFormat = format;

            this.Analyzed = true;

            PerformBasicCounts();
        }

        /// <summary>
        /// Load the buffer from the underlying file.
        /// </summary>
        /// <param name="csv">The CSV file to load from.</param>
        private void LoadBuffer(ReadCSV csv)
        {
            for (int i = 0; i < this.buffer.Length; i++)
                this.buffer[i] = null;

            int index = 0;
            while (csv.Next() && (index < this.bufferSize))
            {
                LoadedRow row = new LoadedRow(csv);
                buffer[index++] = row;
            }

            this.remaining = index;
        }

        /// <summary>
        /// Get the next row from the underlying CSV file.
        /// </summary>
        /// <param name="csv">The underlying CSV file.</param>
        /// <returns>The loaded row.</returns>
        private LoadedRow GetNextRow(ReadCSV csv)
        {
            if (remaining == 0)
            {
                LoadBuffer(csv);
            }

            while (remaining > 0)
            {
                int index = RangeRandomizer.RandomInt(0, this.bufferSize - 1);
                if (this.buffer[index] != null)
                {
                    LoadedRow result = this.buffer[index];
                    this.buffer[index] = null;
                    this.remaining--;
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        /// Process, and generate the output file.
        /// </summary>
        /// <param name="outputFile">The output file.</param>
        public void Process(String outputFile)
        {
            ValidateAnalyzed();

            ReadCSV csv = new ReadCSV(this.InputFilename, this.ExpectInputHeaders, this.InputFormat);
            LoadedRow row;

            TextWriter tw = this.PrepareOutputFile(outputFile);

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
