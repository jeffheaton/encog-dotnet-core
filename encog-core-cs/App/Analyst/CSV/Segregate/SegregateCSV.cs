using System.Collections.Generic;
using System.IO;
using Encog.App.Analyst.CSV.Basic;
using Encog.App.Quant;
using Encog.Util;
using Encog.Util.CSV;

namespace Encog.App.Analyst.CSV.Segregate
{
    /// <summary>
    /// This class is used to segregate a CSV file into several sub-files. This can
    /// be used to create training and evaluation datasets.
    /// </summary>
    ///
    public class SegregateCSV : BasicFile
    {
        /// <summary>
        /// TOtal percents should add to this.
        /// </summary>
        ///
        public const int TOTAL_PCT = 100;

        /// <summary>
        /// The segregation targets.
        /// </summary>
        ///
        private readonly IList<SegregateTargetPercent> targets;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public SegregateCSV()
        {
            targets = new List<SegregateTargetPercent>();
        }

        /// <value>The segregation targets.</value>
        public IList<SegregateTargetPercent> Targets
        {
            get { return targets; }
        }

        /// <summary>
        /// Analyze the input file.
        /// </summary>
        ///
        /// <param name="inputFile">The input file.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="format">The format of the input file.</param>
        public void Analyze(FileInfo inputFile, bool headers,
                            CSVFormat format)
        {
            InputFilename = inputFile;
            ExpectInputHeaders = headers;
            InputFormat = format;

            Analyzed = true;

            PerformBasicCounts();

            BalanceTargets();
        }

        /// <summary>
        /// Balance the targets.
        /// </summary>
        ///
        private void BalanceTargets()
        {
            SegregateTargetPercent smallestItem = null;
            int numberAssigned = 0;


            // first try to assign as many as can be assigned
            foreach (SegregateTargetPercent p  in  targets)
            {
                SegregateTargetPercent stp = p;

                // assign a number of records to this
                double percent = stp.Percent/Format.HUNDRED_PERCENT;
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
                smallestItem.NumberRemaining = smallestItem.NumberRemaining
                                               + remain;
            }
        }


        /// <summary>
        /// Process the input file and segregate into the output files.
        /// </summary>
        ///
        public void Process()
        {
            Validate();

            var csv = new ReadCSV(InputFilename.ToString(),
                                  ExpectInputHeaders, InputFormat);
            ResetStatus();

            foreach (SegregateTargetPercent target  in  targets)
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
        /// Validate that the data is correct.
        /// </summary>
        ///
        private void Validate()
        {
            ValidateAnalyzed();

            if (targets.Count < 1)
            {
                throw new QuantError("There are no segregation targets.");
            }

            if (targets.Count < 2)
            {
                throw new QuantError(
                    "There must be at least two segregation targets.");
            }

            int total = 0;

            foreach (SegregateTargetPercent p  in  targets)
            {
                total += p.Percent;
            }

            if (total != TOTAL_PCT)
            {
                throw new QuantError("Target percents must equal 100.");
            }
        }
    }
}