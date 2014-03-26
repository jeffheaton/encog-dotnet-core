//
// Encog(tm) Core v3.2 - .Net Version
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
using System.IO;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Buffer;
using Encog.Util.Arrayutil;
using Encog.Util.CSV;
using Encog.Util.File;
using Directory = System.IO.Directory;

namespace Encog.Examples.Indicator.Avg
{
    /// <summary>
    /// This class is used to generate training for the moving averages example.
    /// </summary>
    public class GenerateTraining
    {
        /// <summary>
        /// Used to normalize the difference between the two fields.
        /// </summary>
        private readonly NormalizedField _fieldDifference;

        /// <summary>
        /// Used to normalize the outcome (gain or loss).
        /// </summary>
        private readonly NormalizedField _fieldOutcome;

        /// <summary>
        /// The path that the data files will be stored at.
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// The path to the training file.
        /// </summary>
        private readonly FileInfo _trainingFile;

        /// <summary>
        /// A moving window used to track future gains.
        /// </summary>
        private readonly WindowDouble _window = new WindowDouble(Config.PredictWindow);

        /// <summary>
        /// The maximum difference.
        /// </summary>
        private double _maxDifference;

        /// <summary>
        /// The max pip gain/loss.
        /// </summary>
        private int _maxPiPs;

        /// <summary>
        /// The minimum difference.
        /// </summary>
        private double _minDifference;

        /// <summary>
        /// The min pip gain/loss.
        /// </summary>
        private int _minPiPs;

        /// <summary>
        /// Construct the training generator. 
        /// </summary>
        /// <param name="thePath">The path to use.</param>
        public GenerateTraining(string thePath)
        {
            _path = thePath;
            _trainingFile = FileUtil.CombinePath(new FileInfo(_path), Config.FilenameTrain);

            _fieldDifference = new NormalizedField(NormalizationAction.Normalize, "diff", Config.DiffRange,
                                                  -Config.DiffRange, 1, -1);
            _fieldOutcome = new NormalizedField(NormalizationAction.Normalize, "out", Config.PipRange, -Config.PipRange,
                                               1, -1);
        }

        /// <summary>
        /// Process the individual training file. 
        /// </summary>
        /// <param name="file">The training file to process.</param>
        /// <param name="output">The data set to output to.</param>
        protected void ProcessFile(string file, BufferedMLDataSet output)
        {
            var inputData = new BasicMLData(output.InputSize);
            var idealData = new BasicMLData(output.IdealSize);

            var csv = new ReadCSV(file, true, CSVFormat.English);
            while (csv.Next())
            {
                var a = new double[Config.InputWindow + 1];
                double close = csv.GetDouble(1);

                const int fastIndex = 2;
                const int slowIndex = fastIndex + Config.InputWindow;

                a[0] = close;
                for (int i = 0; i < 3; i++)
                {
                    double fast = csv.GetDouble(fastIndex + i);
                    double slow = csv.GetDouble(slowIndex + i);
                    double diff = _fieldDifference.Normalize((fast - slow)/Config.PipSize);
                    a[i + 1] = diff;
                }
                _window.Add(a);

                if (_window.IsFull())
                {
                    double max = (_window.CalculateMax(0, Config.InputWindow) - close)/Config.PipSize;
                    double min = (_window.CalculateMin(0, Config.InputWindow) - close)/Config.PipSize;

                    double o = Math.Abs(max) > Math.Abs(min) ? max : min;

                    a = _window.GetLast();
                    for (int i = 0; i < 3; i++)
                    {
                        inputData[i] = a[i + 1];
                    }

                    o = _fieldOutcome.Normalize(o);
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
            var csv = new ReadCSV(file, true, CSVFormat.English);
            while (csv.Next())
            {
                var a = new double[1];
                double close = csv.GetDouble(1);

                const int fastIndex = 2;
                const int slowIndex = fastIndex + Config.InputWindow;
                a[0] = close;
                for (int i = 0; i < Config.InputWindow; i++)
                {
                    double fast = csv.GetDouble(fastIndex + i);
                    double slow = csv.GetDouble(slowIndex + i);

                    if (!double.IsNaN(fast) && !double.IsNaN(slow))
                    {
                        double diff = (fast - slow)/Config.PipSize;
                        _minDifference = Math.Min(_minDifference, diff);
                        _maxDifference = Math.Max(_maxDifference, diff);
                    }
                }
                _window.Add(a);

                if (_window.IsFull())
                {
                    double max = (_window.CalculateMax(0, Config.InputWindow) - close)/Config.PipSize;
                    double min = (_window.CalculateMin(0, Config.InputWindow) - close)/Config.PipSize;

                    double o = Math.Abs(max) > Math.Abs(min) ? max : min;

                    _maxPiPs = Math.Max(_maxPiPs, (int) o);
                    _minPiPs = Math.Min(_minPiPs, (int) o);
                }
            }
        }

        /// <summary>
        /// Called to generate the training file.
        /// </summary>
        public void Generate()
        {
            string[] list = Directory.GetFiles(_path);

            _trainingFile.Delete();
            var output = new BufferedMLDataSet(_trainingFile.ToString());
            output.BeginLoad(Config.InputWindow, 1);

            foreach (string file in list)
            {
                var fn = new FileInfo(file);
                if (fn.Name.StartsWith("collected") && fn.Name.EndsWith(".csv"))
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
            string[] list = Directory.GetFiles(_path);

            _maxDifference = double.NegativeInfinity;
            _minDifference = double.PositiveInfinity;
            _maxPiPs = int.MinValue;
            _minPiPs = int.MaxValue;

            foreach (string file in list)
            {
                string fn = FileUtil.GetFileName(new FileInfo(file));
                if (fn.StartsWith("collected") && fn.EndsWith(".csv"))
                {
                    CalibrateFile(file);
                }
            }

            Console.WriteLine(@"Max difference: " + _maxDifference);
            Console.WriteLine(@"Min difference: " + _minDifference);
            Console.WriteLine(@"Max PIPs: " + _maxPiPs);
            Console.WriteLine(@"Min PIPs: " + _minPiPs);
            Console.WriteLine(@"\nSuggested calibration: ");
            Console.WriteLine(@"DIFF_RANGE = " + (int) (Math.Max(_maxDifference, Math.Abs(_minDifference))*1.2));
            Console.WriteLine(@"PIP_RANGE = " + (int) (Math.Max(_maxPiPs, Math.Abs(_minPiPs))*1.2));
        }
    }
}
