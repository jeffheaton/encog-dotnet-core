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
using System.Diagnostics;
using System.IO;
using System.Text;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Buffer;
using Encog.Util.File;

namespace Encog.Util.Banchmark
{
    /// <summary>
    /// Benchmark Encog with several network types.
    /// </summary>
    public class EncogBenchmark
    {
        /// <summary>
        /// Number of steps in all.
        /// </summary>
        private const int Steps = 3;

        /// <summary>
        /// The first step.
        /// </summary>
        private const int Step1 = 1;

        /// <summary>
        /// The third step.
        /// </summary>
        private const int Step2 = 2;

        /// <summary>
        /// The fourth step.
        /// </summary>
        private const int Step3 = 3;

        /// <summary>
        /// Report progress.
        /// </summary>
        private readonly IStatusReportable _report;

        /// <summary>
        /// The binary score.
        /// </summary>
        private int _binaryScore;

        /// <summary>
        /// The CPU score.
        /// </summary>
        private int _cpuScore;

        /// <summary>
        /// The memory score.
        /// </summary>
        private int _memoryScore;

        /// <summary>
        /// Construct a benchmark object.
        /// </summary>
        /// <param name="report">The object to report progress to.</param>
        public EncogBenchmark(IStatusReportable report)
        {
            _report = report;
        }

        /// <summary>
        /// The CPU score.
        /// </summary>
        public int CpuScore
        {
            get { return _cpuScore; }
        }

        /// <summary>
        /// The memory score.
        /// </summary>
        public int MemoryScore
        {
            get { return _memoryScore; }
        }

        /// <summary>
        /// The binary score.
        /// </summary>
        public int BinaryScore
        {
            get { return _binaryScore; }
        }

        /// <summary>
        /// Perform the benchmark. Returns the total amount of time for all of the
        /// benchmarks. Returns the final score. The lower the better for a score.
        /// </summary>
        /// <returns>The total time, which is the final Encog benchmark score.</returns>
        public String Process()
        {
            _report.Report(Steps, 0, "Beginning benchmark");

            EvalCpu();
            EvalMemory();
            EvalBinary();

            var result = new StringBuilder();

            result.Append("Encog Benchmark: CPU:");
            result.Append(Format.FormatInteger(_cpuScore));

            result.Append(", Memory:");
            result.Append(Format.FormatInteger(_memoryScore));
            result.Append(", Disk:");
            result.Append(Format.FormatInteger(_binaryScore));
            _report.Report(Steps, Steps, result
                                            .ToString());

            return result.ToString();
        }


        /// <summary>
        /// Evaluate the CPU.
        /// </summary>
        private void EvalCpu()
        {
            int small = Evaluate.EvaluateTrain(2, 4, 0, 1);
            _report.Report(Steps, Step1,
                          "Evaluate CPU, tiny= " + Format.FormatInteger(small/100));

            int medium = Evaluate.EvaluateTrain(10, 20, 0, 1);
            _report.Report(Steps, Step1,
                          "Evaluate CPU, small= " + Format.FormatInteger(medium/30));

            int large = Evaluate.EvaluateTrain(100, 200, 40, 5);
            _report.Report(Steps, Step1,
                          "Evaluate CPU, large= " + Format.FormatInteger(large));

            int huge = Evaluate.EvaluateTrain(200, 300, 200, 50);
            _report.Report(Steps, Step1,
                          "Evaluate CPU, huge= " + Format.FormatInteger(huge));

            int result = (small/100) + (medium/30) + large + huge;

            _report.Report(Steps, Step1,
                          "CPU result: " + result);
            _cpuScore = result;
        }


        /// <summary>
        /// Evaluate memory.
        /// </summary>
        private void EvalMemory()
        {
            BasicMLDataSet training = RandomTrainingFactory.Generate(
                1000, 10000, 10, 10, -1, 1);

            const long stop = (10*Evaluate.Milis);
            int record = 0;

            IMLDataPair pair;

            int iterations = 0;
            var watch = new Stopwatch();
            watch.Start();
			while(true)
			{
				iterations++;
				pair = training[record++];
				if(record >= training.Count)
					record = 0;

				if((iterations & 0xff) == 0 && watch.ElapsedMilliseconds >= stop)
					break;
			} 
			iterations /= 100000;

            _report.Report(Steps, Step2,
                          "Memory dataset, result: " + Format.FormatInteger(iterations));

            _memoryScore = iterations;
        }

        /// <summary>
        /// Evaluate disk.
        /// </summary>
        private void EvalBinary()
        {
            FileInfo file = FileUtil.CombinePath( new FileInfo(Path.GetTempPath()), "temp.egb" );

            BasicMLDataSet training = RandomTrainingFactory.Generate(
                1000, 10000, 10, 10, -1, 1);

            // create the binary file

            if (file.Exists)
            {
                file.Delete();
            }

            var training2 = new BufferedMLDataSet(file.ToString());
            training2.Load(training);

            const long stop = (10*Evaluate.Milis);
            int record = 0;

            IMLDataPair pair;

            var watch = new Stopwatch();
            watch.Start();

            int iterations = 0;
			while(true)
			{
				iterations++;
				pair = training[record++];
				if(record >= training.Count)
					record = 0;

				if((iterations & 0xff) == 0 && watch.ElapsedMilliseconds >= stop)
					break;
			}

            training2.Close();

            iterations /= 100000;

            _report.Report(Steps, Step3,
                          "Disk(binary) dataset, result: "
                          + Format.FormatInteger(iterations));

            if (file.Exists)
            {
                file.Delete();
            }
            _binaryScore = iterations;
        }
    }
}
