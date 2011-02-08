using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Quant.Shuffle;
using Encog.Util.CSV;
using Encog.App.Quant.Segregate;
using Encog.App.Quant.Classify;
using Encog.App.Quant.Normalize;
using Encog.Util.Simple;

namespace Encog.Examples.Iris
{
    public class IrisGenerate
    {
        private IExampleInterface app;

        public IrisGenerate(IExampleInterface app)
        {
            this.app = app;
        }

        public void Step1()
        {
            app.WriteLine("Step 1: Shuffle source file");
            ShuffleCSV shuffle = new ShuffleCSV();
            shuffle.Analyze(IrisConstant.IRIS_FILE, false, CSVFormat.ENGLISH);
            shuffle.Process(IrisConstant.RANDOM_FILE);
        }

        public void Step2()
        {
            SegregateCSV seg = new SegregateCSV();
            seg.Targets.Add(new SegregateTargetPercent(IrisConstant.TRAINING_FILE, 75));
            seg.Targets.Add(new SegregateTargetPercent(IrisConstant.EVALUATE_FILE, 25));
            app.WriteLine("Step 2: Generate training and evaluation files");
            seg.Analyze(IrisConstant.RANDOM_FILE, false, CSVFormat.ENGLISH);
            seg.Process();
        }

        public int Step3(ClassifyMethod method)
        {
            app.WriteLine("Step 3: Classify training data");
            ClassifyCSV cls = new ClassifyCSV();
            cls.Analyze(IrisConstant.RANDOM_FILE, false, CSVFormat.ENGLISH, 4);
            cls.Process(IrisConstant.CLASSIFY_FILE, method, -1, null);
            cls.Stats.WriteStatsFile(IrisConstant.CLASSIFY_STATS_FILE);
            return cls.Stats.ColumnsNeeded;
        }

        public void Step4(int outputColumns)
        {
            app.WriteLine("Step 4: Normalize training data");
            NormalizeCSV norm = new NormalizeCSV();
            norm.Analyze(IrisConstant.CLASSIFY_FILE, false, CSVFormat.ENGLISH);

            int index = 0;
            norm.Stats.Data[index++].Name = "sepal_length";
            norm.Stats.Data[index++].Name = "sepal_width";
            norm.Stats.Data[index++].Name = "petal_length";
            norm.Stats.Data[index++].Name = "petal_width";

            for (int i = 0; i < outputColumns; i++)
            {
                norm.Stats.Data[index].Name = "type" + i;
                norm.Stats.Data[index++].MakePassThrough();
            }

            norm.Normalize(IrisConstant.NORMALIZED_FILE);
            norm.WriteStatsFile(IrisConstant.NORMALIZED_STATS_FILE);
        }

        public void Step5(int outputCount)
        {
            app.WriteLine("Step 5: Converting training file to binary");
            int inputCount = IrisConstant.INPUT_COUNT;
            EncogUtility.ConvertCSV2Binary(IrisConstant.NORMALIZED_FILE, IrisConstant.BINARY_FILE, inputCount, outputCount, true);

        }

        public void Report(int total, int current, String message)
        {
            app.WriteLine(current + "/" + total + " " + message);

        }

    }
}
