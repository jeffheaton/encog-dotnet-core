// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Normalize.Input;
using Encog.Normalize;
using Encog.Normalize.Output.Nominal;
using Encog.Normalize.Target;
using Encog.Normalize.Output;
using Encog.Normalize.Segregate.Index;
using Encog.Normalize.Segregate;
using Encog.Engine;
using Encog.App.Quant.Segregate;
using Encog.Util.CSV;
using Encog.App.Quant.Balance;
using Encog.App.Quant.Shuffle;
using Encog.App.Quant.Classify;
using Encog.App.Quant.Normalize;
using Encog.Util.Simple;
using Encog.App.Quant.Filter;
using Encog.Engine.Util;

namespace Encog.Examples.Forest
{
    public class GenerateData : IStatusReportable
    {
        private IExampleInterface app;

        public GenerateData(IExampleInterface app)
        {
            this.app = app;
        }

        public void Step1()
        {
            app.WriteLine("Step 1: Filter to single wilderness area");
            FilterCSV filter = new FilterCSV();
            filter.Analyze(Constant.COVER_TYPE_FILE, false, CSVFormat.ENGLISH);
            //filter.Exclude(10, "1");
            filter.Exclude(11, "1");
            filter.Exclude(12, "1");
            filter.Exclude(13, "1");
            filter.Process(Constant.FILTERED_FILE);
            app.WriteLine("Origional row count: " + Format.FormatInteger(filter.RecordCount) + ", Filtered row count:" + Format.FormatInteger(filter.FilteredRowCount));
        }

        public void Step2()
        {
            app.WriteLine("Step 2: Shuffle source file");
            ShuffleCSV shuffle = new ShuffleCSV();
            shuffle.Analyze(Constant.FILTERED_FILE, false, CSVFormat.ENGLISH);
            shuffle.Process(Constant.RANDOM_FILE);
        }

        public void Step3()
        {
            SegregateCSV seg = new SegregateCSV();
            seg.Targets.Add(new SegregateTargetPercent(Constant.TRAINING_FILE, 75));
            seg.Targets.Add(new SegregateTargetPercent(Constant.EVALUATE_FILE, 25));
            app.WriteLine("Step 3: Generate training and evaluation files");
            seg.Analyze(Constant.RANDOM_FILE, false, CSVFormat.ENGLISH);            
            seg.Process();            
        }

        public void Step4()
        {
            app.WriteLine("Step 4: Balance training to have the same number of each tree");
            BalanceCSV balance = new BalanceCSV();
            balance.Analyze(Constant.TRAINING_FILE, false, CSVFormat.ENGLISH);
            balance.Process(Constant.BALANCE_FILE, 54, 500);
            app.WriteLine("Count per Tree:");
            app.WriteLine(balance.DumpCounts());
        }

        public int Step5(ClassifyMethod method)
        {
            app.WriteLine("Step 5: Classify training data");
            ClassifyCSV cls = new ClassifyCSV();
            cls.Analyze(Constant.BALANCE_FILE, false, CSVFormat.ENGLISH, 54);
            cls.Process(Constant.CLASSIFY_FILE, method, -1, false);
            cls.Stats.WriteStatsFile(Constant.CLASSIFY_STATS_FILE);
            return cls.Stats.ColumnsNeeded;
        }

        public void Step6(int outputColumns)
        {
            app.WriteLine("Step 6: Normalize training data");
            NormalizeCSV norm = new NormalizeCSV();
            norm.Analyze(Constant.CLASSIFY_FILE, false, CSVFormat.ENGLISH);

            int index = 0;
            norm.Stats.Data[index++].Name = "elevation";
            norm.Stats.Data[index++].Name = "aspect";
            norm.Stats.Data[index++].Name = "slope";
            norm.Stats.Data[index++].Name = "hydro_h";
            norm.Stats.Data[index++].Name = "hydro_v";
            norm.Stats.Data[index++].Name = "roadway";
            norm.Stats.Data[index++].Name = "shade_9";
            norm.Stats.Data[index++].Name = "shade_12";
            norm.Stats.Data[index++].Name = "shade_3";
            norm.Stats.Data[index++].Name = "fire";

            norm.Stats.Data[index].Name = "area1";
            norm.Stats.Data[index++].Action = NormalizationDesired.Ignore;
            norm.Stats.Data[index].Name = "area2";
            norm.Stats.Data[index++].Action = NormalizationDesired.Ignore;
            norm.Stats.Data[index].Name = "area3";
            norm.Stats.Data[index++].Action = NormalizationDesired.Ignore;
            norm.Stats.Data[index].Name = "area4";
            norm.Stats.Data[index++].Action = NormalizationDesired.Ignore;

            for (int i = 0; i < 40; i++)
            {
                norm.Stats.Data[index].Name = "soil" + (i + 1);
                norm.Stats.Data[index].ActualHigh = 1;
                norm.Stats.Data[index++].ActualLow = 0;
            }

            for (int i = 0; i < outputColumns; i++)
            {
                norm.Stats.Data[index].Name = "covertype" + (i - 54);
                norm.Stats.Data[index++].MakePassThrough();
            }

            norm.Normalize(Constant.NORMALIZED_FILE);
            norm.WriteStatsFile(Constant.NORMALIZED_STATS_FILE);
        }

        public void Step7(int outputCount)
        {
            app.WriteLine("Step 7: Converting training file to binary");
            int inputCount = Constant.INPUT_COUNT;
            EncogUtility.ConvertCSV2Binary(Constant.NORMALIZED_FILE, Constant.BINARY_FILE, inputCount, outputCount, true);

        }

        public void Report(int total, int current, String message)
        {
            app.WriteLine(current + "/" + total + " " + message);

        }
    }
}
