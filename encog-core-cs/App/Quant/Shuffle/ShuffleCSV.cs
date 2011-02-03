using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;
using System.Collections;
using System.IO;
using Encog.App.Quant.Basic;
using Encog.MathUtil.Randomize;

namespace Encog.App.Quant.Shuffle
{
    public class ShuffleCSV : BasicFile
    {
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

        public ShuffleCSV()
        {
            this.BufferSize = 500;
        }



        public void Analyze(String inputFile, bool headers, CSVFormat format)
        {
            this.InputFilename = inputFile;
            this.ExpectInputHeaders = headers;
            this.InputFormat = format;

            this.Analyzed = true;

            PerformBasicCounts();
        }

        private void LoadBuffer(ReadCSV csv)
        {
            for (int i = 0; i < this.buffer.Length; i++)
                this.buffer[i] = null;

            int index = 0;
            while (csv.Next() && (index < this.bufferSize))
            {
                LoadedRow row = new LoadedRow(csv);
                buffer[index++] = row;
            }

            this.remaining = index;
        }

        private LoadedRow GetNextRow(ReadCSV csv)
        {
            if (remaining == 0)
            {
                LoadBuffer(csv);
            }

            while (remaining > 0)
            {
                int index = RangeRandomizer.RandomInt(0, this.bufferSize - 1);
                if (this.buffer[index] != null)
                {
                    LoadedRow result = this.buffer[index];
                    this.buffer[index] = null;
                    this.remaining--;
                    return result;
                }
            }
            return null;
        }


        public void Process(String outputFile)
        {
            ValidateAnalyzed();

            ReadCSV csv = new ReadCSV(this.InputFilename, this.ExpectInputHeaders, this.InputFormat);
            LoadedRow row;

            TextWriter tw = this.PrepareOutputFile(outputFile);

            while ((row = GetNextRow(csv)) != null)
            {
                WriteRow(tw, row);
            }

            tw.Close();
            csv.Close();
        }
    }
}
