using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;
using System.IO;
using Encog.App.Quant.Util;
using Encog.App.Quant.Basic;

namespace Encog.App.Quant.Temporal
{
    /// <summary>
    /// This class is used to break a CSV file into temporal windows.  This is used for 
    /// predictive neural networks.
    /// </summary>
    public class TemporalWindowCSV: BasicFile
    {
        /// <summary>
        /// The size of the input window.
        /// </summary>
        public int InputWindow { get; set; }

        /// <summary>
        /// The size of the prediction window.
        /// </summary>
        public int PredictWindow { get; set; }

        /// <summary>
        /// The fields that are to be processed.
        /// </summary>
        public TemporalWindowField[] Fields
        {
            get
            {
                return this.fields;
            }
        }

        /// <summary>
        /// The fields that are to be processed.
        /// </summary>
        private TemporalWindowField[] fields;

        /// <summary>
        /// A buffer to hold the data.
        /// </summary>
        private BarBuffer buffer;


        /// <summary>
        /// Construct the object and set the defaults.
        /// </summary>
        public TemporalWindowCSV()
            :base()
        {
            InputWindow = 10;
            PredictWindow = 1;
        }

        /// <summary>
        /// Format the headings to a string.
        /// </summary>
        /// <returns>The a string holding the headers, ready to be written.</returns>
        private String WriteHeaders()
        {
            StringBuilder line = new StringBuilder();

            // write any passthrough fields
            foreach (TemporalWindowField field in this.fields)
            {
                if (field.Action == TemporalType.PassThrough)
                {
                    if (line.Length > 0)
                    {
                        line.Append(",");
                    }

                    line.Append(field.Name);
                }
            }

            // write any input fields
            for (int i = 0; i < this.InputWindow; i++)
            {
                foreach (TemporalWindowField field in this.fields)
                {
                    if (field.Input)
                    {
                        if (line.Length > 0)
                        {
                            line.Append(",");
                        }

                        line.Append("Input:");
                        line.Append(field.Name);

                        if (i > 0)
                        {
                            line.Append("(t-");
                            line.Append(i);
                            line.Append(")");
                        }
                        else
                        {
                            line.Append("(t)");
                        }
                    }
                }
            }

            // write any output fields
            for (int i = 0; i < this.PredictWindow; i++)
            {
                foreach (TemporalWindowField field in this.fields)
                {
                    if (field.Predict)
                    {
                        if (line.Length > 0)
                        {
                            line.Append(",");
                        }

                        line.Append("Predict:");
                        line.Append(field.Name);

                        line.Append("(t+");
                        line.Append(i + 1);
                        line.Append(")");
                    }

                }
            }

            return line.ToString();
        }

        /// <summary>
        /// Process the input file, and write to the output file.
        /// </summary>
        /// <param name="outputFile">The output file.</param>
        public void Process(String outputFile)
        {
            if (InputWindow < 1)
            {
                throw new EncogError("Input window must be greater than one.");
            }

            if (PredictWindow < 1)
            {
                throw new EncogError("Predict window must be greater than one.");
            }

            int inputFieldCount = CountInputFields();
            int predictFieldCount = CountPredictFields();

            if (inputFieldCount < 1)
            {
                throw new EncogError("There must be at least 1 input field.");
            }

            if (predictFieldCount < 1)
            {
                throw new EncogError("There must be at least 1 input field.");
            }

            int barSize = inputFieldCount + predictFieldCount;

            buffer = new BarBuffer(InputWindow + PredictWindow);

            ReadCSV csv = null;
            TextWriter tw = null;

            try
            {
                csv = new ReadCSV(this.InputFilename, this.ExpectInputHeaders, this.InputFormat);

                tw = new StreamWriter(outputFile);

                // write headers, if needed
                if (this.ExpectInputHeaders)
                {
                    tw.WriteLine(WriteHeaders());
                }

                ResetStatus();
                while (csv.Next())
                {
                    UpdateStatus(false);
                    // begin to populate the bar
                    double[] bar = new double[barSize];

                    int fieldIndex = 0;
                    int barIndex = 0;
                    foreach (TemporalWindowField field in this.fields)
                    {
                        String str = csv.Get(fieldIndex++);

                        if (field.Action != TemporalType.Ignore && field.Action!=TemporalType.PassThrough)
                        {
                            bar[barIndex++] = this.InputFormat.Parse(str);
                        }
                        field.LastValue = str;
                    }
                    buffer.Add(bar);

                    // if the buffer is full, begin writing out temporal windows
                    if (buffer.Full)
                    {
                        StringBuilder line = new StringBuilder();
                        
                        // write passthroughs
                        foreach (TemporalWindowField field in this.fields)
                        {
                            if (field.Action == TemporalType.PassThrough)
                            {
                                if (line.Length > 0)
                                {
                                    line.Append(",");
                                }

                                line.Append("\"");
                                line.Append(field.LastValue);
                                line.Append("\"");
                            }
                        }

                        // write input
                        for (int i = 0; i < this.InputWindow; i++)
                        {
                            bar = buffer.Data[buffer.Data.Count - 1 - i];

                            int index = 0;
                            foreach (TemporalWindowField field in this.fields)
                            {
                                if (field.Input)
                                {
                                    if (line.Length > 0)
                                        line.Append(',');
                                    line.Append(this.InputFormat.Format(bar[index], Precision));
                                    index++;
                                }
                            }
                        }

                        // write prediction
                        for (int i = 0; i < this.PredictWindow; i++)
                        {
                            bar = buffer.Data[PredictWindow - 1 - i];

                            int index = 0;
                            foreach (TemporalWindowField field in this.fields)
                            {
                                if (field.Predict)
                                {
                                    if (line.Length > 0)
                                        line.Append(',');
                                    line.Append(this.InputFormat.Format(bar[index], Precision));
                                    index++;
                                }
                            }
                        }

                        // write the line
                        tw.WriteLine(line.ToString());
                    }
                }
            }
            finally
            {
                ReportDone(false);
                if (csv != null)
                {
                    try
                    {
                        csv.Close();
                    }
                    catch (Exception)
                    {
                    }
                }

                if (tw != null)
                {
                    try
                    {
                        tw.Close();
                    }
                    catch (Exception)
                    {
                    }
                }
            }


        }

        /// <summary>
        /// Count the number of fields that are that are in the prediction.
        /// </summary>
        /// <returns>The number of fields predicted.</returns>
        public int CountPredictFields()
        {
            int result = 0;

            foreach (TemporalWindowField field in this.fields)
            {
                if (field.Predict)
                    result++;
            }

            return result;
        }

        /// <summary>
        /// Count the number of input fields, or fields used to predict.
        /// </summary>
        /// <returns>The number of input fields.</returns>
        public int CountInputFields()
        {
            int result = 0;

            foreach (TemporalWindowField field in this.fields)
            {
                if (field.Input)
                    result++;
            }

            return result;
        }

        /// <summary>
        /// Analyze the input file, prior to processing.
        /// </summary>
        /// <param name="filename">The filename to process.</param>
        /// <param name="headers">True, if the input file has headers.</param>
        /// <param name="format">The format of the input file.</param>
        public void Analyze(String filename, bool headers, CSVFormat format)
        {
            ReadCSV csv = null;

            try
            {
                csv = new ReadCSV(filename, headers, format);
                if (!csv.Next())
                {
                    throw new EncogError("Empty file");
                }

                this.InputFilename = filename;
                this.ExpectInputHeaders = headers;
                this.InputFormat = format;

                this.fields = new TemporalWindowField[csv.GetColumnCount()];
                double d;

                for (int i = 0; i < csv.GetColumnCount(); i++)
                {
                    String str = csv.Get(i);
                    String fieldname;

                    if (this.ExpectInputHeaders)
                    {
                        fieldname = csv.ColumnNames[i];
                    }
                    else
                    {
                        fieldname = "Column-" + i;
                    }

                    this.fields[i] = new TemporalWindowField(fieldname);
                    if (!Double.TryParse(str, out d))
                    {
                        this.fields[i].Action = TemporalType.Input;
                    }
                }
            }
            finally
            {
                if (csv != null)
                {
                    csv.Close();
                }
            }
        }
    }
}
