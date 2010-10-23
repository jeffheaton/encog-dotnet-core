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
using Encog.Neural.Networks;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Synapse;
using Encog.Engine;
using Encog.Engine.Opencl;
using Encog.Engine.Util;
using Encog.Neural.Data.Basic;
using Encog.Neural.Data.Buffer;
using System.Diagnostics;
using System.IO;

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
        private const int STEPS = 4;

        /// <summary>
        /// The first step.
        /// </summary>
        private const int STEP1 = 1;

        /// <summary>
        /// The second step.
        /// </summary>
        private const int STEP2 = 2;

        /// <summary>
        /// The third step.
        /// </summary>
        private const int STEP3 = 3;

        /// <summary>
        /// The fourth step.
        /// </summary>
        private const int STEP4 = 4;

        /// <summary>
        /// Report progress.
        /// </summary>
        private IStatusReportable report;

        /// <summary>
        /// The CPU score.
        /// </summary>
        private int cpuScore;

        /// <summary>
        /// The OpenCL score.
        /// </summary>
        private int clScore;

        /// <summary>
        /// The memory score.
        /// </summary>
        private int memoryScore;

        /// <summary>
        /// The binary score.
        /// </summary>
        private int binaryScore;

        /// <summary>
        /// The OpenCL device to use.
        /// </summary>
        private EncogCLDevice device;

        /// <summary>
        /// Construct a benchmark object.
        /// </summary>
        /// <param name="report">The object to report progress to.</param>
        public EncogBenchmark(IStatusReportable report)
        {
            this.report = report;
        }

        /// <summary>
        /// Perform the benchmark. Returns the total amount of time for all of the
        /// benchmarks. Returns the final score. The lower the better for a score.
        /// </summary>
        /// <returns>The total time, which is the final Encog benchmark score.</returns>
        public String Process()
        {
            this.report.Report(EncogBenchmark.STEPS, 0, "Beginning benchmark");

            EvalCPU();
            EvalOpenCL();
            EvalMemory();
            EvalBinary();

            StringBuilder result = new StringBuilder();

            result.Append("Encog Benchmark: CPU:");
            result.Append(Format.FormatInteger(this.cpuScore));
            result.Append(", OpenCL");
            if (this.device == null)
                result.Append("(none)");
            else if (this.device.CPU)
                result.Append("(cpu)");
            else
                result.Append("(gpu)");

            result.Append(":");
            result.Append(Format.FormatInteger(this.clScore));
            result.Append(", Memory:");
            result.Append(Format.FormatInteger(this.memoryScore));
            result.Append(", Disk:");
            result.Append(Format.FormatInteger(this.binaryScore));
            this.report.Report(EncogBenchmark.STEPS, EncogBenchmark.STEPS, result
                    .ToString());

            return result.ToString();
        }

 
        /// <summary>
        /// Evaluate the CPU.
        /// </summary>
        private void EvalCPU()
        {

            int small = Evaluate.EvaluateTrain(2, 4, 0, 1);
            this.report.Report(EncogBenchmark.STEPS, EncogBenchmark.STEP1,
                    "Evaluate CPU, tiny= " + Format.FormatInteger(small / 100));

            int medium = Evaluate.EvaluateTrain(10, 20, 0, 1);
            this.report.Report(EncogBenchmark.STEPS, EncogBenchmark.STEP1,
                    "Evaluate CPU, small= " + Format.FormatInteger(medium / 30));

            int large = Evaluate.EvaluateTrain(100, 200, 40, 5);
            this.report.Report(EncogBenchmark.STEPS, EncogBenchmark.STEP1,
                    "Evaluate CPU, large= " + Format.FormatInteger(large));

            int huge = Evaluate.EvaluateTrain(200, 300, 200, 50);
            this.report.Report(EncogBenchmark.STEPS, EncogBenchmark.STEP1,
                    "Evaluate CPU, huge= " + Format.FormatInteger(huge));

            int result = (small / 100) + (medium / 30) + large + huge;

            this.report.Report(EncogBenchmark.STEPS, EncogBenchmark.STEP1,
                    "CPU result: " + result);
            this.cpuScore = result;
        }

        /// <summary>
        /// Evaluate the OpenCL device.
        /// </summary>
        private void EvalOpenCL()
        {

            try
            {
                // did the caller assign a device? If not, use the first GPU,
                // failing that,
                // use the first CPU. Failing that, as well, don't test OpenCL.
                if (this.device == null)
                {

                    if (EncogFramework.Instance.CL == null)
                        EncogFramework.Instance.InitCL();

                    this.device = EncogFramework.Instance.CL.ChooseDevice();

                }
            }
            catch (Exception t)
            {
                this.report.Report(EncogBenchmark.STEPS, EncogBenchmark.STEP2,
                        "No OpenCL devices, result: 0");
                this.clScore = 0;
            }

            int small = 0, medium = 0, large = 0, huge = 0;

            try
            {
                small = Evaluate.EvaluateTrain(device, 2, 4, 0, 1);
                this.report.Report(EncogBenchmark.STEPS, EncogBenchmark.STEP2,
                        "Evaluate OpenCL, tiny= "
                                + Format.FormatInteger(small / 100));
            }
            catch (Exception t)
            {
                this.report.Report(EncogBenchmark.STEPS, EncogBenchmark.STEP2,
                        "Evaluate OpenCL, tiny FAILED");
            }

            try
            {
                medium = Evaluate.EvaluateTrain(device, 10, 20, 0, 1);
                this.report.Report(EncogBenchmark.STEPS, EncogBenchmark.STEP2,
                        "Evaluate OpenCL, small= "
                                + Format.FormatInteger(medium / 30));
            }
            catch (Exception t)
            {
                this.report.Report(EncogBenchmark.STEPS, EncogBenchmark.STEP2,
                        "Evaluate OpenCL, small FAILED");
            }

            try
            {
                large = Evaluate.EvaluateTrain(device, 100, 200, 40, 5);
                this.report.Report(EncogBenchmark.STEPS, EncogBenchmark.STEP2,
                        "Evaluate OpenCL, large= " + Format.FormatInteger(large));
            }
            catch (Exception t)
            {
                this.report.Report(EncogBenchmark.STEPS, EncogBenchmark.STEP2,
                        "Evaluate OpenCL, large FAILED");
            }

            try
            {
                huge = Evaluate.EvaluateTrain(device, 200, 300, 200, 50);
                this.report.Report(EncogBenchmark.STEPS, EncogBenchmark.STEP2,
                        "Evaluate OpenCL, huge= " + Format.FormatInteger(huge));
            }
            catch (Exception t)
            {
                this.report.Report(EncogBenchmark.STEPS, EncogBenchmark.STEP2,
                        "Evaluate OpenCL, huge FAILED");
            }

            int result = (small / 100) + (medium / 30) + large + huge;

            this.report.Report(EncogBenchmark.STEPS, EncogBenchmark.STEP2,
                    "OpenCL result: " + result);
            this.clScore = result;

        }

        /// <summary>
        /// Evaluate memory.
        /// </summary>
        private void EvalMemory()
        {
            BasicNeuralDataSet training = RandomTrainingFactory.Generate(
                    1000, 10000, 10, 10, -1, 1);

            long stop = (10 * Evaluate.MILIS);
            int record = 0;

            INeuralDataPair pair = BasicNeuralDataPair.CreatePair(10, 10);

            int iterations = 0;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            while (watch.ElapsedMilliseconds < stop)
            {
                iterations++;
                training.GetRecord(record++, pair);
                if (record >= training.Count)
                    record = 0;
            }

            iterations /= 100000;

            this.report.Report(EncogBenchmark.STEPS, EncogBenchmark.STEP3,
                    "Memory dataset, result: " + Format.FormatInteger(iterations));

            this.memoryScore = iterations;
        }

        /// <summary>
        /// Evaluate disk.
        /// </summary>
        private void EvalBinary()
        {
            String file = "temp.egb";

            BasicNeuralDataSet training = RandomTrainingFactory.Generate(
                    1000, 10000, 10, 10, -1, 1);

            // create the binary file

            File.Delete(file);

            BufferedNeuralDataSet training2 = new BufferedNeuralDataSet(file);
            training2.Load(training);

            long stop = (10 * Evaluate.MILIS);
            int record = 0;

            INeuralDataPair pair = BasicNeuralDataPair.CreatePair(10, 10);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            int iterations = 0;
            while (watch.ElapsedMilliseconds < stop)
            {
                iterations++;
                training2.GetRecord(record++, pair);
                if (record >= training2.Count)
                    record = 0;
            }

            training2.Close();

            iterations /= 100000;

            this.report.Report(EncogBenchmark.STEPS, EncogBenchmark.STEP4,
                    "Disk(binary) dataset, result: "
                            + Format.FormatInteger(iterations));

            File.Delete(file);
            this.binaryScore = iterations;
        }

        /// <summary>
        /// The CPU score.
        /// </summary>
        public int CpuScore
        {
            get
            {
                return cpuScore;
            }
        }

        /// <summary>
        /// The OpenCL score.
        /// </summary>
        public int CLScore
        {
            get
            {
                return clScore;
            }
        }

        /// <summary>
        /// The memory score.
        /// </summary>
        public int MemoryScore
        {
            get
            {
                return memoryScore;
            }
        }

        /// <summary>
        /// The binary score.
        /// </summary>
        public int BinaryScore
        {
            get
            {
                return binaryScore;
            }
        }

        /// <summary>
        /// The device.
        /// </summary>
        public EncogCLDevice Device
        {
            get
            {
                return device;
            }
            set
            {
                this.device = value;
            }
        }
    }
}
