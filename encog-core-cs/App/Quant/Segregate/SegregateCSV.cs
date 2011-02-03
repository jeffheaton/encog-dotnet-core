using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;
using System.Collections;
using System.IO;
using Encog.App.Quant.Basic;
using Encog.MathUtil.Randomize;

namespace Encog.App.Quant.Segregate
{
    public class SegregateCSV : BasicFile
    {
        public IList<SegregateTargetPercent> Targets { get { return this.targets; } }
        public bool RandomOrder { get; set; }
        private IList<SegregateTargetPercent> targets = new List<SegregateTargetPercent>();
        private int bufferSize;
        private LoadedRow[] buffer;
        private int remaining;

        private int BufferSize
        {
            get
            {
                return this.bufferSize;
            }
            set
            {
                this.bufferSize = value;
                this.buffer = new LoadedRow[this.bufferSize];
            }
        }

        public SegregateCSV()
        {
            this.BufferSize = 500;
            this.RandomOrder = true;
        }


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

        public void Analyze(String inputFile, bool headers, CSVFormat format)
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

            this.ColumnCount = csv.GetColumnCount();

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
            BalanceTargets();
        }

        private void LoadBuffer(ReadCSV csv)
        {
            for (int i = 0; i < this.buffer.Length; i++)
                this.buffer[i] = null;

            int index = 0;
            while ( csv.Next() && (index<this.bufferSize) )
            {
                LoadedRow row = new LoadedRow(csv);
                buffer[index++] = row;
            }

            this.remaining = index;
        }

        private LoadedRow GetNextRow(ReadCSV csv)
        {
            if( remaining==0 )
            {
                LoadBuffer(csv);
            }

            while (remaining > 0)
            {
                int index = RangeRandomizer.RandomInt(0, this.bufferSize-1);
                if (this.buffer[index]!=null)
                {
                    LoadedRow result = this.buffer[index];
                    this.buffer[index] = null;
                    this.remaining--;
                    return result;
                }
            }
            return null;            
        }

        
        public void Process()
        {            
            Validate();

            IList<LoadedRow> result = new List<LoadedRow>();

            ReadCSV csv = new ReadCSV(this.InputFilename, this.ExpectInputHeaders, this.InputFormat);
            
            foreach(SegregateTargetPercent target in this.targets) {
                TextWriter tw = this.PrepareOutputFile(target.Filename);
                LoadedRow row;
                while (target.NumberRemaining > 0 && (row = GetNextRow(csv)) != null)
                {
                    WriteRow(tw, row);
                    target.NumberRemaining--;
                }

                tw.Close();
            }
         
            csv.Close();
        }
    }
}
