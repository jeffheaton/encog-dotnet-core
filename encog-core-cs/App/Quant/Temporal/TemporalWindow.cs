using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;
using System.IO;
using Encog.App.Quant.Util;

namespace Encog.App.Quant.Temporal
{
    public class TemporalWindow
    {
        public int InputWindow { get; set; }
        public int PredictWindow { get; set; }
        public int Precision { get; set; }
        public TemporalWindowField[] Fields
        {
            get
            {
                return this.fields;
            }
        }

        private TemporalWindowField[] fields;
        private String sourceFile;
        private bool sourceHeaders;
        private CSVFormat sourceFormat;
        private BarBuffer buffer;

        public TemporalWindow()
        {
            InputWindow = 10;
            PredictWindow = 1;
            Precision = 10;
        }

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
                csv = new ReadCSV(this.sourceFile, this.sourceHeaders, this.sourceFormat);

                tw = new StreamWriter(outputFile);

                // write headers, if needed
                if (this.sourceHeaders)
                {
                    StringBuilder line = new StringBuilder();

                    for (int i = 0; i < this.InputWindow; i++)
                    {
                        int index = 0;
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

                    for (int i = 0; i < this.PredictWindow; i++)
                    {
                        int index = 0;
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

                    tw.WriteLine(line.ToString());
                }

                while (csv.Next())
                {
                    // begin to populate the bar
                    double[] bar = new double[barSize];

                    int fieldIndex = 0;
                    int barIndex = 0;
                    foreach (TemporalWindowField field in this.fields)
                    {
                        String str = csv.Get(fieldIndex++);

                        if (!field.Ignore)
                        {
                            bar[barIndex++] = this.sourceFormat.Parse(str);
                        }
                    }
                    buffer.Add(bar);

                    // if the buffer is full, begin writing out temporal windows
                    if (buffer.Full)
                    {
                        StringBuilder line = new StringBuilder();

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
                                    line.Append(this.sourceFormat.Format(bar[index], Precision));
                                }
                                index++;
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
                                    line.Append(this.sourceFormat.Format(bar[index], Precision));
                                }
                                index++;
                            }
                        }

                        // write the line
                        tw.WriteLine(line.ToString());
                    }
                }
            }
            finally
            {
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

                this.sourceFile = filename;
                this.sourceHeaders = headers;
                this.sourceFormat = format;

                this.fields = new TemporalWindowField[csv.GetColumnCount()];
                double d;

                for (int i = 0; i < csv.GetColumnCount(); i++)
                {
                    String str = csv.Get(i);
                    String fieldname;

                    if (this.sourceHeaders)
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
                        this.fields[i].Input = true;
                        this.fields[i].Predict = false;
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
