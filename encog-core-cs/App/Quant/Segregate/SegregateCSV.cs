using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;
using System.Collections;
using System.IO;
using Encog.App.Quant.Basic;

namespace Encog.App.Quant.Segregate
{
    public class SortCSV : BasicFile
    {
        public IList<SegregateTargetPercent> Targets { get { return this.targets; } }
        private IList<SegregateTargetPercent> targets = new List<SegregateTargetPercent>();

        private void Validate()
        {
            ValidateAnalyzed();

            if (targets.Count < 1)
            {
                throw new QuantError("There are no segregation targets.");
            }

            if (targets.Count < 2)
            {
                throw new QuantError("There must be at least two segregation targets.");
            }

            int total = 0;
            foreach (SegregateTargetPercent p in this.targets)
            {
                total += p.Percent;
            }

            if (total != 100)
            {
                throw new QuantError("Target percents must equal 100.");
            }            
        }

        private void OpenTargetFiles()
        {
            foreach (SegregateTargetPercent p in this.targets)
            {
                p.TargetFile = this.PrepareOutputFile(p.Filename);
            }
        }

        private void CloseTargetFiles()
        {
            foreach (SegregateTargetPercent p in this.targets)
            {
                p.TargetFile.Close();
            }
        }

        private void BalanceTargets()
        {
            SegregateTargetPercent smallestItem = null;
            int numberAssigned = 0;

            // first try to assign as many as can be assigned
            foreach (SegregateTargetPercent p in this.targets)
            {
                // assign a number of records to this 
                double percent = p.Percent / 100.0;
                int c = (int)(this.RecordCount * percent);
                p.NumberRemaining = c;

                // track the smallest group
                if (smallestItem == null || smallestItem.Percent > p.Percent)
                {
                    smallestItem = p;
                }             

                numberAssigned+=c;
            }

            // see if there are any remaining items
            int remain = this.RecordCount - numberAssigned;

            // if there are extras, just add them to the largest group
            if (remain > 0)
            {
                smallestItem.NumberRemaining += remain;
            }



        }

        public void Analyze(String inputFile, String outputFile, bool headers, CSVFormat format)
        {
            this.InputFilename = inputFile;
            this.ExpectInputHeaders = headers;
            this.InputFormat = format;

            this.Analyzed = true;

            int recordCount = 0;
            ReadCSV csv = new ReadCSV(this.InputFilename, this.ExpectInputHeaders, this.InputFormat);
            while (csv.Next())
            {
                recordCount++;
            }

            if (this.ExpectInputHeaders)
            {
                this.InputHeadings = new String[csv.ColumnNames.Count];
                for (int i = 0; i < csv.ColumnNames.Count; i++)
                {
                    this.InputHeadings[i] = csv.ColumnNames[i];
                }
            }
            csv.Close();

            this.RecordCount = recordCount;

        }

        public void Process(String outputFile)
        {            
            Validate();
            OpenTargetFiles();

            ReadCSV csv = new ReadCSV(this.InputFilename, this.ExpectInputHeaders, this.InputFormat);
            while (csv.Next())
            {
                
                
            }
         
            csv.Close();

            CloseTargetFiles();
        }
    }
}
