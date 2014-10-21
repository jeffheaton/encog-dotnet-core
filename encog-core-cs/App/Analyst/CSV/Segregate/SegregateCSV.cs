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
using System.Collections.Generic;
using System.IO;
using Encog.App.Analyst.CSV.Basic;
using Encog.App.Quant;
using Encog.Util.CSV;

namespace Encog.App.Analyst.CSV.Segregate
{
    /// <summary>
    ///     This class is used to segregate a CSV file into several sub-files. This can
    ///     be used to create training and evaluation datasets.
    /// </summary>
    public class SegregateCSV : BasicFile
    {
        /// <summary>
        ///     TOtal percents should add to this.
        /// </summary>
        public const int TotalPct = 100;

        /// <summary>
        ///     The segregation targets.
        /// </summary>
        private readonly IList<SegregateTargetPercent> _targets;

        /// <summary>
        ///     Construct the object.
        /// </summary>
        public SegregateCSV()
        {
            _targets = new List<SegregateTargetPercent>();
        }

        /// <value>The segregation targets.</value>
        public IList<SegregateTargetPercent> Targets
        {
            get { return _targets; }
        }

        /// <summary>
        ///     Analyze the input file.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="format">The format of the input file.</param>
        public void Analyze(FileInfo inputFile, bool headers,
                            CSVFormat format)
        {
            InputFilename = inputFile;
            ExpectInputHeaders = headers;
            Format = format;

            Analyzed = true;

            PerformBasicCounts();

            BalanceTargets();
        }

        /// <summary>
        ///     Balance the targets.
        /// </summary>
        private void BalanceTargets()
        {
            SegregateTargetPercent smallestItem = null;
            int numberAssigned = 0;


            // first try to assign as many as can be assigned
            foreach (SegregateTargetPercent p  in  _targets)
            {
                SegregateTargetPercent stp = p;

                // assign a number of records to this
                double percent = stp.Percent/100.0;
                var c = (int) (RecordCount*percent);
                stp.NumberRemaining = c;

                // track the smallest group
                if ((smallestItem == null)
                    || (smallestItem.Percent > stp.Percent))
                {
                    smallestItem = stp;
                }

                numberAssigned += c;
            }

            // see if there are any remaining items
            int remain = RecordCount - numberAssigned;

            // if there are extras, just add them to the largest group
            if (remain > 0)
            {
                if (smallestItem != null)
                    smallestItem.NumberRemaining = smallestItem.NumberRemaining
                                                   + remain;
            }
        }


        /// <summary>
        ///     Process the input file and segregate into the output files.
        /// </summary>
        public void Process()
        {
            Validate();

            var csv = new ReadCSV(InputFilename.ToString(),
                                  ExpectInputHeaders, Format);
            ResetStatus();

            foreach (SegregateTargetPercent target  in  _targets)
            {
                StreamWriter tw = PrepareOutputFile(target.Filename);

                while ((target.NumberRemaining > 0) && csv.Next()
                       && !ShouldStop())
                {
                    UpdateStatus(false);
                    var row = new LoadedRow(csv);
                    WriteRow(tw, row);
                    target.NumberRemaining = target.NumberRemaining - 1;
                }

                tw.Close();
            }
            ReportDone(false);
            csv.Close();
        }

        /// <summary>
        ///     Validate that the data is correct.
        /// </summary>
        private void Validate()
        {
            ValidateAnalyzed();

            if (_targets.Count < 1)
            {
                throw new QuantError("There are no segregation targets.");
            }

            if (_targets.Count < 2)
            {
                throw new QuantError(
                    "There must be at least two segregation targets.");
            }

            int total = 0;

            foreach (SegregateTargetPercent p  in  _targets)
            {
                total += p.Percent;
            }

            if (total != TotalPct)
            {
                throw new QuantError("Target percents must equal 100.");
            }
        }
    }
}
