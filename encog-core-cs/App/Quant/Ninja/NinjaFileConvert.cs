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
using System.IO;
using System.Text;
using Encog.App.Analyst.CSV.Basic;
using Encog.Util.CSV;

namespace Encog.App.Quant.Ninja
{
    /// <summary>
    ///     A simple class to convert financial data files into the format used by NinjaTrader for
    ///     input.
    /// </summary>
    public class NinjaFileConvert : BasicCachedFile
    {
        /// <summary>
        ///     Process the file and output to the target file.
        /// </summary>
        /// <param name="target">The target file to write to.</param>
        public void Process(string target)
        {
            var csv = new ReadCSV(InputFilename.ToString(), ExpectInputHeaders, Format);
            TextWriter tw = new StreamWriter(target);

            ResetStatus();
            while (csv.Next())
            {
                var line = new StringBuilder();
                UpdateStatus(false);
                line.Append(GetColumnData(FileData.Date, csv));
                line.Append(" ");
                line.Append(GetColumnData(FileData.Time, csv));
                line.Append(";");
                line.Append(Format.Format(double.Parse(GetColumnData(FileData.Open, csv)), Precision));
                line.Append(";");
                line.Append(Format.Format(double.Parse(GetColumnData(FileData.High, csv)), Precision));
                line.Append(";");
                line.Append(Format.Format(double.Parse(GetColumnData(FileData.Low, csv)), Precision));
                line.Append(";");
                line.Append(Format.Format(double.Parse(GetColumnData(FileData.Close, csv)), Precision));
                line.Append(";");
                line.Append(Format.Format(double.Parse(GetColumnData(FileData.Volume, csv)), Precision));

                tw.WriteLine(line.ToString());
            }
            ReportDone(false);
            csv.Close();
            tw.Close();
        }

        /// <summary>
        ///     Analyze the input file.
        /// </summary>
        /// <param name="input">The name of the input file.</param>
        /// <param name="headers">True, if headers are present.</param>
        /// <param name="format">The format of the input file.</param>
        public override void Analyze(FileInfo input, bool headers, CSVFormat format)
        {
            base.Analyze(input, headers, format);
        }
    }
}
