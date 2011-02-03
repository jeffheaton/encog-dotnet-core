using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;
using System.IO;

namespace Encog.App.Quant.Normalize
{
    public class NormalizeCSV
    {
        public int Precision { get; set; }
        public NormalizationStats Stats { get; set; }
        
        private String sourceFile;
        private String targetFile;
        private CSVFormat sourceFormat;
        private bool sourceHeaders;

        public NormalizeCSV()
        {
            Precision = 10;
        }

        public void SetSourceFile(String file, bool headers, CSVFormat format)
        {
            this.sourceFile = file;
            this.sourceHeaders = headers;
            this.sourceFormat = format;
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
                this.Stats = new NormalizationStats(fieldCount);

                for (int i = 0; i < fieldCount; i++)
                {
                    Stats[i] = new NormalizedFieldStats();
                    Stats[i].Name = csv.ColumnNames[i];
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
            if (this.Stats.Count == 0)
            {
                throw new EncogError("Can't denormalize, there are no stats loaded.");
            }

            ReadCSV csv = new ReadCSV(sourceFile, headers, format);
            TextWriter tw = new StreamWriter(targetFile);

            if( !csv.Next() )
            {
                throw new EncogError("The source file " + sourceFile + " is empty.");
            }

            if (csv.GetColumnCount() != this.Stats.Count)
            {
                throw new EncogError("The number of columns in the input file("+csv.GetColumnCount()+") and stats file("+this.Stats.Count+") must match.");
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
                foreach (NormalizedFieldStats stat in this.Stats.Data)
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
            foreach (NormalizedFieldStats stat in this.Stats.Data)
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
            if (this.Stats.Count<1 )
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
                    foreach (NormalizedFieldStats stat in this.Stats.Data)
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

                this.Stats.Data = list.ToArray<NormalizedFieldStats>();
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
