using System;
using System.IO;
using System.Text;
using Encog.Engine.Util;
using Encog.Util.CSV;

namespace Encog.ML.Data.Buffer.CODEC
{
    /// <summary>
    /// A CODEC used to read/write data from/to a CSV data file. There are two
    /// constructors provided, one is for reading, the other for writing. Make sure
    /// you use the correct one for your intended purpose.
    /// 
    /// This CODEC is typically used with the BinaryDataLoader, to load external data
    /// into the Encog binary training format.
    /// </summary>
    public class CSVDataCODEC : IDataSetCODEC
    {
        /// <summary>
        /// The external CSV file.
        /// </summary>
        private String file;

        /// <summary>
        /// The CSV format to use.
        /// </summary>
        private CSVFormat format;

        /// <summary>
        /// The size of the input data.
        /// </summary>
        private int inputCount;

        /// <summary>
        /// The size of the ideal data.
        /// </summary>
        private int idealCount;

        /// <summary>
        /// True, if headers are present in the CSV file.
        /// </summary>
        private bool headers;

        /// <summary>
        /// The utility to assist in reading the CSV file.
        /// </summary>
        private ReadCSV readCSV;

        /// <summary>
        /// A file used to output the CSV file.
        /// </summary>
        private TextWriter output;

        /// <summary>
        /// Create a CODEC to load data from CSV to binary. 
        /// </summary>
        /// <param name="file">The CSV file to load.</param>
        /// <param name="format">The format that the CSV file is in.</param>
        /// <param name="headers">True, if there are headers.</param>
        /// <param name="inputCount">The number of input columns.</param>
        /// <param name="idealCount">The number of ideal columns.</param>
        public CSVDataCODEC(
                 String file,
                 CSVFormat format,
                 bool headers,
                 int inputCount, int idealCount)
        {
            if (this.inputCount != 0)
            {
                throw new BufferedDataError(
                        "To export CSV, you must use the CSVDataCODEC constructor that does not specify input or ideal sizes.");
            }
            this.file = file;
            this.format = format;
            this.inputCount = inputCount;
            this.idealCount = idealCount;
            this.headers = headers;
        }

        /// <summary>
        /// Constructor to create CSV from binary.
        /// </summary>
        /// <param name="file">The CSV file to create.</param>
        /// <param name="format">The format for that CSV file.</param>
        public CSVDataCODEC(String file, CSVFormat format)
        {
            this.file = file;
            this.format = format;
        }


        /// <summary>
        /// Read one record of data from a CSV file. 
        /// </summary>
        /// <param name="input">The input data array.</param>
        /// <param name="ideal">The ideal data array.</param>
        /// <returns>True, if there is more data to be read.</returns>
        public bool Read(double[] input, double[] ideal)
        {
            if (this.readCSV.Next())
            {
                int index = 0;
                for (int i = 0; i < input.Length; i++)
                {
                    input[i] = readCSV.GetDouble(index++);
                }

                for (int i = 0; i < ideal.Length; i++)
                {
                    ideal[i] = readCSV.GetDouble(index++);
                }
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Write one record of data to a CSV file. 
        /// </summary>
        /// <param name="input">The input data array.</param>
        /// <param name="ideal">The ideal data array.</param>
        public void Write(double[] input, double[] ideal)
        {
            double[] record = new double[input.Length + ideal.Length];
            EngineArray.ArrayCopy(input, record);
            EngineArray.ArrayCopy(ideal, 0, record, input.Length, ideal.Length);
            StringBuilder result = new StringBuilder();
            NumberList.ToList(this.format, result, record);
            this.output.WriteLine(result.ToString());
        }
        
        /// <summary>
        /// Prepare to write to a CSV file. 
        /// </summary>
        /// <param name="recordCount">The total record count, that will be written.</param>
        /// <param name="inputSize">The input size.</param>
        /// <param name="idealSize">The ideal size.</param>
        public void PrepareWrite(
                    int recordCount,
                    int inputSize,
                    int idealSize)
        {
            try
            {
                this.inputCount = inputSize;
                this.idealCount = idealSize;
                this.output = new StreamWriter(new FileStream(file, FileMode.Create));
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Prepare to read from the CSV file.
        /// </summary>
        public void PrepareRead()
        {
            if (this.inputCount == 0)
            {
                throw new BufferedDataError(
                        "To import CSV, you must use the CSVDataCODEC constructor that specifies input and ideal sizes.");
            }
            this.readCSV = new ReadCSV(this.file, this.headers,
                    this.format);
        }

        /// <inheritDoc/>
        public int InputSize
        {
            get
            {
                return this.inputCount;
            }
        }

        /// <inheritDoc/>
        public int IdealSize
        {
            get
            {
                return this.idealCount;
            }
        }

        /// <inheritDoc/>
        public void Close()
        {
            if (this.readCSV != null)
            {
                this.readCSV.Close();
                this.readCSV = null;
            }

            if (this.output != null)
            {
                this.output.Close();
                this.output = null;
            }

        }
    }
}
