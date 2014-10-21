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
using System;
using System.Collections.Generic;
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
        public const int Port = 5128;

        /// <summary>
        /// The instruments that we are downloading (i.e. ticker symbols, and their data)
        /// </summary>
        private readonly IDictionary<string, InstrumentHolder> _data = new Dictionary<string, InstrumentHolder>();

        /// <summary>
        /// The target CSV file.
        /// </summary>
        private readonly FileInfo _targetFile;

        /// <summary>
        /// The number of rows downloaded.
        /// </summary>
        private int _rowsDownloaded;

        /// <summary>
        /// Construct the download indicator.
        /// </summary>
        /// <param name="theFile">The local CSV file.</param>
        public DownloadIndicator(FileInfo theFile)
            : base(false)
        {
            _targetFile = theFile;
        }

        /// <summary>
        /// The number of rows downloaded.
        /// </summary>
        public int RowsDownloaded
        {
            get { return _rowsDownloaded; }
        }

        /// <summary>
        /// Close.
        /// </summary>
        public void Close()
        {
            Link.Close();
            _data.Clear();
        }

        /// <inheritdoc/>
        public override void NotifyPacket(IndicatorPacket packet)
        {
            if (string.Compare(packet.Command, IndicatorLink.PacketBar, true) == 0)
            {
                String security = packet.Args[1];
                long when = long.Parse(packet.Args[0]);
                String key = security.ToLower();
                InstrumentHolder holder;

                if (_data.ContainsKey(key))
                {
                    holder = _data[key];
                }
                else
                {
                    holder = new InstrumentHolder();
                    _data[key] = holder;
                }

                if (holder.Record(when, 2, packet.Args))
                {
                    _rowsDownloaded++;
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
                if (_data.Count == 0)
                {
                    return;
                }


                using (var outfile =
                    new StreamWriter(_targetFile.ToString()))
                {
                    // output header
                    outfile.Write("\"INSTRUMENT\",\"WHEN\"");
                    int index = 0;
                    foreach (String str in DataRequested)
                    {
                        String str2;
                        int ix = str.IndexOf('[');
                        str2 = ix != -1 ? str.Substring(0, ix).Trim() : str;

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
                    foreach (string ins in _data.Keys)
                    {
                        InstrumentHolder holder = _data[ins];
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
    }
}
