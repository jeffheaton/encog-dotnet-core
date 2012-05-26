using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Encog.Util.Arrayutil;
using Encog.ML.Data;
using Encog.ML.Data.Buffer;
using Encog.ML.Data.Basic;
using Encog.Util.CSV;
using Encog.Util.File;

namespace Encog.Examples.Indicator.Ninja.Avg
{
    /// <summary>
    /// This class is used to generate training for the moving averages example.
    /// </summary>
    public class GenerateTraining
    {
        /// <summary>
        /// The path that the data files will be stored at.
        /// </summary>
        private readonly string path;

        /// <summary>
        /// The path to the training file.
        /// </summary>
        private FileInfo trainingFile;

        /// <summary>
        /// A moving window used to track future gains.
        /// </summary>
        private readonly WindowDouble window = new WindowDouble(Config.PREDICT_WINDOW);

        /// <summary>
        /// Used to normalize the difference between the two fields.
        /// </summary>
        private readonly NormalizedField fieldDifference;

        /// <summary>
        /// Used to normalize the outcome (gain or loss).
        /// </summary>
        private readonly NormalizedField fieldOutcome;

        /// <summary>
        /// The maximum difference.
        /// </summary>
        private double maxDifference;

        /// <summary>
        /// The minimum difference.
        /// </summary>
        private double minDifference;

        /// <summary>
        /// The max pip gain/loss.
        /// </summary>
        private int maxPIPs;

        /// <summary>
        /// The min pip gain/loss.
        /// </summary>
        private int minPIPs;

        /// <summary>
        /// Construct the training generator. 
        /// </summary>
        /// <param name="thePath">The path to use.</param>
        public GenerateTraining(string thePath)
        {
            this.path = thePath;
            this.trainingFile = FileUtil.CombinePath(new FileInfo(this.path), Config.FILENAME_TRAIN);

            this.fieldDifference = new NormalizedField(NormalizationAction.Normalize, "diff", Config.DIFF_RANGE, -Config.DIFF_RANGE, 1, -1);
            this.fieldOutcome = new NormalizedField(NormalizationAction.Normalize, "out", Config.PIP_RANGE, -Config.PIP_RANGE, 1, -1);
        }

        /// <summary>
        /// Process the individual training file. 
        /// </summary>
        /// <param name="file">The training file to process.</param>
        /// <param name="output">The data set to output to.</param>
        protected void ProcessFile(string file, BufferedMLDataSet output)
        {

            BasicMLData inputData = new BasicMLData(output.InputSize);
            BasicMLData idealData = new BasicMLData(output.IdealSize);

            ReadCSV csv = new ReadCSV(file.ToString(), true, CSVFormat.English);
            while (csv.Next())
            {
                double[] a = new double[Config.INPUT_WINDOW + 1];
                double close = csv.GetDouble(1);

                int fastIndex = 2;
                int slowIndex = fastIndex + Config.INPUT_WINDOW;

                a[0] = close;
                for (int i = 0; i < 3; i++)
                {
                    double fast = csv.GetDouble(fastIndex + i);
                    double slow = csv.GetDouble(slowIndex + i);
                    double diff = this.fieldDifference.Normalize((fast - slow) / Config.PIP_SIZE);
                    a[i + 1] = diff;
                }
                window.Add(a);

                if (window.IsFull())
                {
                    double max = (this.window.CalculateMax(0, Config.INPUT_WINDOW) - close) / Config.PIP_SIZE;
                    double min = (this.window.CalculateMin(0, Config.INPUT_WINDOW) - close) / Config.PIP_SIZE;
                    double o;

                    if (Math.Abs(max) > Math.Abs(min))
                    {
                        o = max;
                    }
                    else
                    {
                        o = min;
                    }

                    a = window.GetLast();
                    for (int i = 0; i < 3; i++)
                    {
                        inputData[i] = a[i + 1];
                    }

                    o = this.fieldOutcome.Normalize(o);
                    idealData[0] = o;

                    output.Add(inputData, idealData);
                }
            }
        }

        /// <summary>
        /// Used to calibrate the training file. 
        /// </summary>
        /// <param name="file">The file to consider.</param>
        protected void CalibrateFile(string file)
        {

            ReadCSV csv = new ReadCSV(file.ToString(), true, CSVFormat.English);
            while (csv.Next())
            {
                double[] a = new double[1];
                double close = csv.GetDouble(1);

                int fastIndex = 2;
                int slowIndex = fastIndex + Config.INPUT_WINDOW;
                a[0] = close;
                for (int i = 0; i < Config.INPUT_WINDOW; i++)
                {
                    double fast = csv.GetDouble(fastIndex + i);
                    double slow = csv.GetDouble(slowIndex + i);

                    if (!double.IsNaN(fast) && !double.IsNaN(slow))
                    {
                        double diff = (fast - slow) / Config.PIP_SIZE;
                        this.minDifference = Math.Min(this.minDifference, diff);
                        this.maxDifference = Math.Max(this.maxDifference, diff);
                    }
                }
                window.Add(a);

                if (window.IsFull())
                {
                    double max = (this.window.CalculateMax(0, Config.INPUT_WINDOW) - close) / Config.PIP_SIZE;
                    double min = (this.window.CalculateMin(0, Config.INPUT_WINDOW) - close) / Config.PIP_SIZE;
                    double o;

                    if (Math.Abs(max) > Math.Abs(min))
                    {
                        o = max;
                    }
                    else
                    {
                        o = min;
                    }

                    this.maxPIPs = Math.Max(this.maxPIPs, (int)o);
                    this.minPIPs = Math.Min(this.minPIPs, (int)o);
                }
            }
        }

        /// <summary>
        /// Called to generate the training file.
        /// </summary>
        public void Generate()
        {
            string[] list = System.IO.Directory.GetFiles(path);

            this.trainingFile.Delete();
            BufferedMLDataSet output = new BufferedMLDataSet(this.trainingFile.ToString());
            output.BeginLoad(Config.INPUT_WINDOW, 1);

            foreach (string file in list)
            {
                String fn = FileUtil.GetFileName(new FileInfo(file));
                if (fn.StartsWith("collected") && fn.EndsWith(".csv"))
                {
                    ProcessFile(file, output);
                }
            }

            output.EndLoad();
            output.Close();
        }

        /// <summary>
        /// Called to calibrate the data.  Does not actually do anything, other
        /// than display a range report.
        /// </summary>
        public void Calibrate()
        {
            string[] list = System.IO.Directory.GetFiles(path);

            this.maxDifference = double.NegativeInfinity;
            this.minDifference = double.PositiveInfinity;
            this.maxPIPs = int.MinValue;
            this.minPIPs = int.MaxValue;

            foreach (string file in list)
            {
                string fn = FileUtil.GetFileName(new FileInfo(file));
                if (fn.StartsWith("collected") && fn.EndsWith(".csv"))
                {
                    CalibrateFile(file);
                }
            }

            Console.WriteLine("Max difference: " + this.maxDifference);
            Console.WriteLine("Min difference: " + this.minDifference);
            Console.WriteLine("Max PIPs: " + this.maxPIPs);
            Console.WriteLine("Min PIPs: " + this.minPIPs);
            Console.WriteLine("\nSuggested calibration: ");
            Console.WriteLine("DIFF_RANGE = " + (int)(Math.Max(this.maxDifference, Math.Abs(this.minDifference)) * 1.2));
            Console.WriteLine("PIP_RANGE = " + (int)(Math.Max(this.maxPIPs, Math.Abs(this.minPIPs)) * 1.2));

        }
    }
}
