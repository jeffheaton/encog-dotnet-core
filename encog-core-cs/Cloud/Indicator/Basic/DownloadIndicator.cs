using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Encog.Cloud.Indicator.Server;

namespace Encog.Cloud.Indicator.Basic
{
    /// <summary>
    /// This indicator is used to download data from an external source.  For example
    /// you may want to export financial data from Ninja Trader, including Ninja Trader
    /// native indicators.  This class will download that data and write it to a CSV file.
    /// </summary>
    public class DownloadIndicator : BasicIndicator
    {
        /// <summary>
        /// The default port.
        /// </summary>
        public const int PORT = 5128;

        /// <summary>
        /// The number of rows downloaded.
        /// </summary>
        private int rowsDownloaded;

        /// <summary>
        /// The instruments that we are downloading (i.e. ticker symbols, and their data)
        /// </summary>
        private IDictionary<string, InstrumentHolder> data = new Dictionary<string, InstrumentHolder>();

        /// <summary>
        /// The target CSV file.
        /// </summary>
        private FileInfo targetFile;

        /// <summary>
        /// Construct the download indicator.
        /// </summary>
        /// <param name="theFile">The local CSV file.</param>
        public DownloadIndicator(FileInfo theFile)
            : base(false)
        {

            this.targetFile = theFile;
        }

        /// <summary>
        /// Close.
        /// </summary>
        public void Close()
        {
            this.Link.Close();
            this.data.Clear();
        }

        /// <inheritdoc/>
        public override void NotifyPacket(IndicatorPacket packet)
        {
            if (string.Compare(packet.Command, IndicatorLink.PACKET_BAR, true) == 0)
            {
                String security = packet.Args[1];
                long when = long.Parse(packet.Args[0]);
                String key = security.ToLower();
                InstrumentHolder holder = null;

                if (this.data.ContainsKey(key))
                {
                    holder = this.data[key];
                }
                else
                {
                    holder = new InstrumentHolder();
                    this.data[key] = holder;
                }

                if (holder.Record(when, 2, packet.Args))
                {
                    this.rowsDownloaded++;
                }
            }
        }

        /// <inheritdoc/>
        public override void NotifyTermination()
        {
            Save();
        }

        /// <summary>
        /// Save the file.
        /// </summary>
        public void Save()
        {
            try
            {
                if (this.data.Count == 0)
                {
                    return;
                }


                using (StreamWriter outfile =
                    new StreamWriter(targetFile.ToString()))
                {
                    // output header
                    outfile.Write("\"INSTRUMENT\",\"WHEN\"");
                    int index = 0;
                    foreach (String str in this.DataRequested)
                    {
                        String str2;
                        int ix = str.IndexOf('[');
                        if (ix != -1)
                        {
                            str2 = str.Substring(0, ix).Trim();
                        }
                        else
                        {
                            str2 = str;
                        }

                        int c = DataCount[index++];
                        if (c <= 1)
                        {
                            outfile.Write(",\"" + str2 + "\"");
                        }
                        else
                        {
                            for (int i = 0; i < c; i++)
                            {                                
                                outfile.Write(",\"" + str2 + "-b" + i + "\"");
                            }
                        }
                    }
                    outfile.WriteLine();

                    // output data
                    foreach (string ins in this.data.Keys)
                    {
                        InstrumentHolder holder = this.data[ins];
                        foreach (long key in holder.Sorted)
                        {
                            String str = holder.Data[key];
                            outfile.WriteLine("\"" + ins + "\"," + key + "," + str);
                        }
                    }

                    outfile.Close();
                }



            }
            catch (IOException ex)
            {
                throw new EncogError(ex);
            }
        }

        /// <summary>
        /// The number of rows downloaded.
        /// </summary>
        public int RowsDownloaded
        {
            get
            {
                return this.rowsDownloaded;
            }
        }
    }
}
