using System.IO;
using Encog.App.Analyst.CSV.Basic;
using Encog.MathUtil.Randomize;
using Encog.Util.CSV;

namespace Encog.App.Analyst.CSV.Shuffle
{
    /// <summary>
    /// Randomly shuffle the lines of a CSV file.
    /// </summary>
    ///
    public class ShuffleCSV : BasicFile
    {
        /// <summary>
        /// The default buffer size.
        /// </summary>
        ///
        public const int DEFAULT_BUFFER_SIZE = 5000;

        /// <summary>
        /// The buffer.
        /// </summary>
        ///
        private LoadedRow[] buffer;

        /// <summary>
        /// The buffer size.
        /// </summary>
        ///
        private int bufferSize;

        /// <summary>
        /// Remaining in the buffer.
        /// </summary>
        ///
        private int remaining;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        public ShuffleCSV()
        {
            BufferSize = DEFAULT_BUFFER_SIZE;
        }

        /// <summary>
        /// The buffer size. This is how many rows of data are loaded(and
        /// randomized), at a time. The default is 5,000.
        /// </summary>
        public int BufferSize
        {
            get { return bufferSize; }
            set
            {
                bufferSize = value;
                buffer = new LoadedRow[bufferSize];
            }
        }

        /// <summary>
        /// Analyze the neural network.
        /// </summary>
        ///
        /// <param name="inputFile">The input file.</param>
        /// <param name="headers">True, if there are headers.</param>
        /// <param name="format">The format of the CSV file.</param>
        public void Analyze(FileInfo inputFile, bool headers,
                            CSVFormat format)
        {
            InputFilename = inputFile;
            ExpectInputHeaders = headers;
            InputFormat = format;

            Analyzed = true;

            PerformBasicCounts();
        }


        /// <summary>
        /// Get the next row from the underlying CSV file.
        /// </summary>
        ///
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
                int index = RangeRandomizer.RandomInt(0, bufferSize - 1);
                if (buffer[index] != null)
                {
                    LoadedRow result = buffer[index];
                    buffer[index] = null;
                    remaining--;
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        /// Load the buffer from the underlying file.
        /// </summary>
        ///
        /// <param name="csv">The CSV file to load from.</param>
        private void LoadBuffer(ReadCSV csv)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = null;
            }

            int index = 0;
            while (csv.Next() && (index < bufferSize) && !ShouldStop())
            {
                var row = new LoadedRow(csv);
                buffer[index++] = row;
            }

            remaining = index;
        }

        /// <summary>
        /// Process, and generate the output file.
        /// </summary>
        ///
        /// <param name="outputFile">The output file.</param>
        public void Process(FileInfo outputFile)
        {
            ValidateAnalyzed();

            var csv = new ReadCSV(InputFilename.ToString(),
                                  ExpectInputHeaders, InputFormat);
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