using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;
using System.IO;

namespace Encog.App.Quant.Normalize
{
    public class EncogNormalize
    {
        public int Precision { get; set; }

        private NormalizedFieldStats[] stats;
        private String sourceFile;
        private String targetFile;
        private CSVFormat sourceFormat;
        private bool sourceHeaders;

        public EncogNormalize()
        {
            Precision = 10;
        }

        public void Analyze(String file, bool headers, CSVFormat format )
        {
            ReadCSV csv = null;

            try
            {
                csv = new ReadCSV(file, headers, format);

                if (!csv.Next())
                {
                    throw new EncogError("File is empty");
                }

                this.sourceFile = file;
                this.sourceHeaders = headers;
                this.sourceFormat = format;

                // analyze first row
                int fieldCount = csv.GetColumnCount();
                this.stats = new NormalizedFieldStats[fieldCount];

                for (int i = 0; i < fieldCount; i++)
                {
                    stats[i] = new NormalizedFieldStats();
                    stats[i].Name = csv.ColumnNames[i];
                }

                // Read entire file to analyze
                do
                {
                    for (int i = 0; i < fieldCount; i++)
                    {
                        if (stats[i].Action == NormalizationDesired.Normalize)
                        {
                            String str = csv.Get(i);
                            double d;
                            if (Double.TryParse(str, out d))
                            {
                                stats[i].Analyze(d);
                            }
                            else
                            {
                                stats[i].MakePassThrough();
                            }
                        }
                    }
                } while (csv.Next());
            }
            finally
            {
                // Close the CSV file
                if( csv!=null )
                    csv.Close();
            }

        }

        public void DeNormalize(String sourceFile, String targetFile, bool headers, CSVFormat format)
        {
            if (this.stats.Length == 0)
            {
                throw new EncogError("Can't denormalize, there are no stats loaded.");
            }

            ReadCSV csv = new ReadCSV(sourceFile, headers, format);
            TextWriter tw = new StreamWriter(targetFile);

            if( !csv.Next() )
            {
                throw new EncogError("The source file " + sourceFile + " is empty.");
            }

            if (csv.GetColumnCount() != this.stats.Length)
            {
                throw new EncogError("The number of columns in the input file("+csv.GetColumnCount()+") and stats file("+this.stats.Length+") must match.");
            }

            // write headers, if needed
            if (headers)
            {
                WriteHeaders(tw);
            }

            do
            {
                StringBuilder line = new StringBuilder();

                int index = 0;
                foreach (NormalizedFieldStats stat in this.stats)
                {
                    String str = csv.Get(index++);
                    if (line.Length > 0)
                        line.Append(this.sourceFormat.Separator);
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
                                line.Append(this.sourceFormat.Format(d, 10));
                            }
                            break;
                    }
                }
                tw.WriteLine(line.ToString());
            } while (csv.Next());

            tw.Close();
            csv.Close();

        }

        private void WriteHeaders(TextWriter tw)
        {
            StringBuilder line = new StringBuilder();
            foreach (NormalizedFieldStats stat in this.stats)
            {
                if (line.Length > 0)
                    line.Append(this.sourceFormat.Separator);
                line.Append("\"");
                line.Append(stat.Name);
                line.Append("\"");
            }
            tw.WriteLine(line.ToString());
        }

        public void Normalize(String file)
        {
            if (this.sourceFile == null)
                throw new EncogError("Can't normalize yet, file has not been analyzed.");


            ReadCSV csv = null;
            TextWriter tw = null;

            try
            {
                csv = new ReadCSV(this.sourceFile, this.sourceHeaders, this.sourceFormat);

                tw = new StreamWriter(file);

                // write headers, if needed
                if (this.sourceHeaders)
                {
                    WriteHeaders(tw);
                }

                // write file contents
                while (csv.Next())
                {
                    StringBuilder line = new StringBuilder();
                    int index = 0;
                    foreach (NormalizedFieldStats stat in this.stats)
                    {
                        String str = csv.Get(index++);
                        if (line.Length > 0)
                            line.Append(this.sourceFormat.Separator);
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
                                    line.Append(this.sourceFormat.Format(d, 10));
                                }
                                break;
                        }
                    }
                    tw.WriteLine(line);
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
                        double ahigh = csv.GetDouble(1);
                        double alow = csv.GetDouble(2);
                        double nhigh = csv.GetDouble(3);
                        double nlow = csv.GetDouble(4);
                        list.Add(new NormalizedFieldStats(NormalizationDesired.Normalize, ahigh, alow, nhigh, nlow));
                    }
                    else if (type.Equals("PassThrough"))
                    {
                        list.Add(new NormalizedFieldStats(NormalizationDesired.PassThrough));
                    }
                    else if (type.Equals("Ignore"))
                    {
                        list.Add(new NormalizedFieldStats(NormalizationDesired.Ignore));
                    }
                }
                csv.Close();

                this.stats = list.ToArray<NormalizedFieldStats>();
            }
            finally
            {
                if (csv != null)
                    csv.Close();
            }
        }

        public void WriteStatsFile(String filename)
        {
            TextWriter tw = null;

            try
            {
                tw = new StreamWriter(filename);

                tw.WriteLine("type,ahigh,alow,nhigh,nlow");

                foreach (NormalizedFieldStats stat in this.stats)
                {
                    StringBuilder line = new StringBuilder();
                    switch (stat.Action)
                    {
                        case NormalizationDesired.Ignore:
                            line.Append("Ignore,0,0,0,0");
                            break;
                        case NormalizationDesired.Normalize:
                            line.Append("Normalize,");
                            double[] d = new double[4] { stat.ActualHigh, stat.ActualLow, stat.NormalizedHigh, stat.NormalizedLow };
                            StringBuilder temp = new StringBuilder();
                            NumberList.ToList(CSVFormat.EG_FORMAT, temp, d);
                            line.Append(temp);
                            break;
                        case NormalizationDesired.PassThrough:
                            line.Append("PassThrough,0,0,0,0");
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
