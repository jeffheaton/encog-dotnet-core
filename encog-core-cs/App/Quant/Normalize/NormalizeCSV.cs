using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;
using System.IO;
using Encog.App.Quant.Basic;

namespace Encog.App.Quant.Normalize
{
    /// <summary>
    /// Normalize, or denormalize, a CSV file.
    /// </summary>
    public class NormalizeCSV: BasicFile
    {
        /// <summary>
        /// The stats for the fields that were normalized.
        /// </summary>
        public NormalizationStats Stats { get; set; }
     
        /// <summary>
        /// The file to 
        /// </summary>
        private String targetFile;

        /// <summary>
        /// Set the source file.  This is useful if you want to use pre-existing stats 
        /// to normalize something and skip the analyze step.
        /// </summary>
        /// <param name="file">The file to use.</param>
        /// <param name="headers">True, if headers are to be expected.</param>
        /// <param name="format">The format of the CSV file.</param>
        public void SetSourceFile(String file, bool headers, CSVFormat format)
        {
            this.InputFilename = file;
            this.ExpectInputHeaders = headers;
            this.InputFormat = format;
        }

        /// <summary>
        /// Analyze the file.
        /// </summary>
        /// <param name="file">The file to analyze.</param>
        /// <param name="headers">True, if the file has headers.</param>
        /// <param name="format">The format of the CSV file.</param>
        public void Analyze(String file, bool headers, CSVFormat format )
        {
            ReadCSV csv = null;

            try
            {
                ResetStatus();
                csv = new ReadCSV(file, headers, format);

                if (!csv.Next())
                {
                    throw new EncogError("File is empty");
                }

                this.InputFilename = file;
                this.ExpectInputHeaders = headers;
                this.InputFormat = format;

                // analyze first row
                int fieldCount = csv.GetColumnCount();
                this.Stats = new NormalizationStats(fieldCount);

                for (int i = 0; i < fieldCount; i++)
                {
                    Stats[i] = new NormalizedFieldStats();
                    if (headers)
                        Stats[i].Name = csv.ColumnNames[i];
                    else
                        Stats[i].Name = "field-" + i;
                }

                // Read entire file to analyze
                do
                {
                    for (int i = 0; i < fieldCount; i++)
                    {
                        if (Stats[i].Action == NormalizationDesired.Normalize)
                        {
                            String str = csv.Get(i);
                            double d;
                            if (Double.TryParse(str, out d))
                            {
                                Stats[i].Analyze(d);
                            }
                            else
                            {
                                Stats[i].MakePassThrough();
                            }
                        }
                    }
                    UpdateStatus(true);
                } while (csv.Next());
            }
            finally
            {
                ReportDone(true);
                // Close the CSV file
                if( csv!=null )
                    csv.Close();
            }

        }

        /// <summary>
        /// Denormalize the input file.
        /// </summary>
        /// <param name="targetFile"></param>
        public void DeNormalize(String targetFile)
        {
            if (this.Stats.Count == 0)
            {
                throw new EncogError("Can't denormalize, there are no stats loaded.");
            }

            ReadCSV csv = new ReadCSV(this.InputFilename, this.ExpectInputHeaders, this.InputFormat);
            TextWriter tw = new StreamWriter(targetFile);

            if( !csv.Next() )
            {
                throw new EncogError("The source file " + this.InputFilename + " is empty.");
            }

            if (csv.GetColumnCount() != this.Stats.Count)
            {
                throw new EncogError("The number of columns in the input file("+csv.GetColumnCount()+") and stats file("+this.Stats.Count+") must match.");
            }

            // write headers, if needed
            if (this.ExpectInputHeaders)
            {
                WriteHeaders(tw);
            }

            ResetStatus();

            do
            {
                StringBuilder line = new StringBuilder();
                UpdateStatus(false);

                int index = 0;
                foreach (NormalizedFieldStats stat in this.Stats.Data)
                {
                    String str = csv.Get(index++);
                    if (line.Length > 0 && stat.Action != NormalizationDesired.Ignore)
                        line.Append(this.InputFormat.Separator);
                    switch (stat.Action)
                    {
                        case NormalizationDesired.PassThrough:
                            line.Append("\"");
                            line.Append(str);
                            line.Append("\"");
                            break;
                        case NormalizationDesired.Normalize:
                            double d;
                            if (Double.TryParse(str, out d))
                            {
                                d = stat.DeNormalize(d);
                                line.Append(this.InputFormat.Format(d, 10));
                            }
                            break;
                    }
                }
                tw.WriteLine(line.ToString());
            } while (csv.Next());

            ReportDone(false);
            tw.Close();
            csv.Close();

        }

        /// <summary>
        /// Write the headers.
        /// </summary>
        /// <param name="tw">The output stream.</param>
        private void WriteHeaders(TextWriter tw)
        {
            StringBuilder line = new StringBuilder();
            foreach (NormalizedFieldStats stat in this.Stats.Data)
            {
                if (line.Length > 0 && stat.Action != NormalizationDesired.Ignore)
                    line.Append(this.InputFormat.Separator);

                if (stat.Action != NormalizationDesired.Ignore)
                {
                    line.Append("\"");
                    line.Append(stat.Name);
                    line.Append("\"");
                }
            }
            tw.WriteLine(line.ToString());
        }

        /// <summary>
        /// Normalize the input file.  Write to the specified file.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        public void Normalize(String file)
        {
            if (this.Stats.Count<1 )
                throw new EncogError("Can't normalize yet, file has not been analyzed.");

            Stats.FixSingleValue();

            ReadCSV csv = null;
            TextWriter tw = null;

            try
            {
                csv = new ReadCSV(this.InputFilename, this.ExpectInputHeaders, this.InputFormat);

                tw = new StreamWriter(file);

                // write headers, if needed
                if (this.ExpectInputHeaders)
                {
                    WriteHeaders(tw);
                }

                ResetStatus();
                // write file contents
                while (csv.Next())
                {
                    StringBuilder line = new StringBuilder();
                    UpdateStatus(false);
                    int index = 0;
                    foreach (NormalizedFieldStats stat in this.Stats.Data)
                    {
                        String str = csv.Get(index++);
                        if (line.Length > 0 && stat.Action!=NormalizationDesired.Ignore)
                            line.Append(this.InputFormat.Separator);
                        switch (stat.Action)
                        {
                            case NormalizationDesired.PassThrough:
                                line.Append("\"");
                                line.Append(str);
                                line.Append("\"");
                                break;
                            case NormalizationDesired.Normalize:
                                double d;
                                if (Double.TryParse(str, out d))
                                {
                                    d = stat.Normalize(d);
                                    line.Append(this.InputFormat.Format(d, 10));
                                }
                                break;
                        }
                    }
                    tw.WriteLine(line);
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
        /// Read the stats file.
        /// </summary>
        /// <param name="filename">The file to read from.</param>
        public void ReadStatsFile(String filename)
        {
            IList<NormalizedFieldStats> list = new List<NormalizedFieldStats>();

            ReadCSV csv = null;

            try
            {
                csv = new ReadCSV(filename, true, CSVFormat.EG_FORMAT);
                while (csv.Next())
                {
                    String type = csv.Get(0);
                    if (type.Equals("Normalize"))
                    {
                        String name = csv.Get(1);
                        double ahigh = csv.GetDouble(2);
                        double alow = csv.GetDouble(3);
                        double nhigh = csv.GetDouble(4);
                        double nlow = csv.GetDouble(5);
                        list.Add(new NormalizedFieldStats(NormalizationDesired.Normalize, name, ahigh, alow, nhigh, nlow));
                    }
                    else if (type.Equals("PassThrough"))
                    {
                        String name = csv.Get(1);
                        list.Add(new NormalizedFieldStats(NormalizationDesired.PassThrough,name));
                    }
                    else if (type.Equals("Ignore"))
                    {
                        String name = csv.Get(1);
                        list.Add(new NormalizedFieldStats(NormalizationDesired.Ignore,name));
                    }
                }
                csv.Close();

                this.Stats = new NormalizationStats(list.Count);
                this.Stats.Data = list.ToArray<NormalizedFieldStats>();
            }
            finally
            {
                if (csv != null)
                    csv.Close();
            }
        }

        /// <summary>
        /// Write the stats file.
        /// </summary>
        /// <param name="filename">The file to write to.</param>
        public void WriteStatsFile(String filename)
        {
            TextWriter tw = null;

            try
            {
                tw = new StreamWriter(filename);

                tw.WriteLine("type,name,ahigh,alow,nhigh,nlow");

                foreach (NormalizedFieldStats stat in this.Stats.Data)
                {
                    StringBuilder line = new StringBuilder();
                    switch (stat.Action)
                    {
                        case NormalizationDesired.Ignore:
                            line.Append("Ignore,\"");
                            line.Append(stat.Name);
                            line.Append("\",0,0,0,0");
                            break;
                        case NormalizationDesired.Normalize:
                            line.Append("Normalize,");
                            line.Append("\"");
                            line.Append(stat.Name);
                            line.Append("\",");
                            double[] d = new double[4] { stat.ActualHigh, stat.ActualLow, stat.NormalizedHigh, stat.NormalizedLow };
                            StringBuilder temp = new StringBuilder();
                            NumberList.ToList(CSVFormat.EG_FORMAT, temp, d);
                            line.Append(temp);
                            break;
                        case NormalizationDesired.PassThrough:
                            line.Append("PassThrough,\"");
                            line.Append(stat.Name);
                            line.Append("\",0,0,0,0");
                            break;

                    }

                    tw.WriteLine(line.ToString());
                }
            }
            finally
            {
                // close the stream
                if (tw != null)
                    tw.Close();
            }
        }
    }
}
