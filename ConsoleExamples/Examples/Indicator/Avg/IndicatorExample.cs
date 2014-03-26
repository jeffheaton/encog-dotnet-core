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
using ConsoleExamples.Examples;
using Encog.Cloud.Indicator;
using Encog.Cloud.Indicator.Server;
using Encog.Examples.Indicator.ImportData;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Factory;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks.Training.Propagation.Manhattan;
using Encog.Persist;
using Encog.Util.File;
using Encog.Util.Simple;
using Directory = System.IO.Directory;

namespace Encog.Examples.Indicator.Avg
{
    /// <summary>
    /// This example shows how to create an indicator based on two existing
    /// NinjaTrader indicators.  For more information on this example, visit
    /// the following URL.
    /// 
    /// http://www.heatonresearch.com/wiki/Neural_Network_Indicator_for_NinjaTrader_with_C_Sharp
    /// </summary>
    public class IndicatorExample : IExample, IIndicatorConnectionListener
    {
        /// <summary>
        /// The port to use.
        /// </summary>
        public const int Port = 5128;

        private IExampleInterface _app;

        /// <summary>
        /// The path to store files at.
        /// </summary>
        private string _path;

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof (IndicatorExample),
                    @"indicator-sma",
                    @"Provide an example indicator.",
                    @"Provide a NinjaTrader example indicator built on two SMA's.");
                return info;
            }
        }

        #region IExample Members

        /// <summary>
        /// The program entry point. 
        /// </summary>
        /// <param name="app">The arguments to run the program with.</param>
        public void Execute(IExampleInterface app)
        {
            _app = app;

            string[] args = app.Args;

            if (args.Length != 2)
            {
                Console.WriteLine(@"Usage: IndicatorExample [clear/collect/generate/train/run] [work path]");
            }
            else
            {
                _path = args[1];
                if (string.Compare(args[0], "collect", true) == 0)
                {
                    Run(true);
                }
                else if (string.Compare(args[0], "train", true) == 0)
                {
                    Train();
                }
                else if (string.Compare(args[0], "run", true) == 0)
                {
                    Run(false);
                }
                else if (string.Compare(args[0], "clear", true) == 0)
                {
                    Clear();
                }
                else if (string.Compare(args[0], "generate", true) == 0)
                {
                    Generate();
                }
                else if (string.Compare(args[0], "calibrate", true) == 0)
                {
                    Calibrate();
                }
            }
        }

        #endregion

        #region IIndicatorConnectionListener Members

        /// <summary>
        /// Called when a connection is made by a trading platform.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="hasOpened">True, if we are opening.</param>
        public void NotifyConnections(IndicatorLink link, bool hasOpened)
        {
            if (hasOpened)
            {
                Console.WriteLine(@"Connection from " + link.ClientSocket.RemoteEndPoint
                                  + @" established.");
            }
            else
            {
                Console.WriteLine(@"Connection from " + link.ClientSocket.RemoteEndPoint
                                  + @" terminated.");
            }
        }

        #endregion

        /// <summary>
        /// Perform the training option.
        /// </summary>
        public void Train()
        {
            // first, create the machine learning method
            var methodFactory = new MLMethodFactory();
            IMLMethod method = methodFactory.Create(Config.MethodType, Config.MethodArchitecture, Config.InputWindow, 1);

            // second, create the data set	
            string filename = FileUtil.CombinePath(new FileInfo(_path), Config.FilenameTrain).ToString();
            IMLDataSet dataSet = EncogUtility.LoadEGB2Memory(new FileInfo(filename));

            // third, create the trainer
            var trainFactory = new MLTrainFactory();
            IMLTrain train = trainFactory.Create(method, dataSet, Config.TrainType, Config.TrainParams);
            // reset if improve is less than 1% over 5 cycles
            if (method is IMLResettable && !(train is ManhattanPropagation))
            {
                train.AddStrategy(new RequiredImprovementStrategy(500));
            }

            // fourth, train and evaluate.
            EncogUtility.TrainToError(train, Config.TargetError);
            method = train.Method;
            EncogDirectoryPersistence.SaveObject(FileUtil.CombinePath(new FileInfo(_path), Config.MethodName), method);

            // finally, write out what we did
            Console.WriteLine(@"Machine Learning Type: " + Config.MethodType);
            Console.WriteLine(@"Machine Learning Architecture: " + Config.MethodArchitecture);

            Console.WriteLine(@"Training Method: " + Config.TrainType);
            Console.WriteLine(@"Training Args: " + Config.TrainParams);
        }

        /// <summary>
        /// Perform the calibrate option.
        /// </summary>
        private void Calibrate()
        {
            var gen = new GenerateTraining(_path);
            gen.Calibrate();
        }

        /// <summary>
        /// Perform the generate option.
        /// </summary>
        private void Generate()
        {
            Console.WriteLine(@"Generating training data... please wait...");
            var gen = new GenerateTraining(_path);
            gen.Generate();
            Console.WriteLine(@"Training data has been generated.");
        }

        /// <summary>
        /// Perform the clear option.
        /// </summary>
        private void Clear()
        {
            string[] list = Directory.GetFiles(_path);

            foreach (string file in list)
            {
                string fn = FileUtil.GetFileName(new FileInfo(file));
                if (fn.StartsWith("collected") && fn.EndsWith(".csv"))
                {
                    new FileInfo(file).Delete();
                }
            }

            Console.WriteLine(@"Directory cleared of captured financial data.");
        }

        /// <summary>
        /// Run the indicator in either collection or indicator mode. 
        /// </summary>
        /// <param name="collectMode">True to run the indicator in collection mode,
        /// false otherwise.</param>
        public void Run(bool collectMode)
        {
            IMLRegression method;

            if (collectMode)
            {
                method = null;
                Console.WriteLine(@"Ready to collect data from remote indicator.");
            }
            else
            {
                method =
                    (IMLRegression)
                    EncogDirectoryPersistence.LoadObject(FileUtil.CombinePath(new FileInfo(_path), Config.MethodName));
                Console.WriteLine(@"Indicator ready.");
            }

            Console.WriteLine(@"Waiting for connections on port " + Port);

            var server = new IndicatorServer();
            server.AddListener(this);

            server.AddIndicatorFactory(new MyFactory(method, _path));
            server.Start();

            if (collectMode)
            {
                server.WaitForIndicatorCompletion();
            }
            else
            {
                server.WaitForShutdown();
            }
        }

        #region Nested type: MyFactory

        public class MyFactory : IIndicatorFactory
        {
            private readonly IMLRegression _method;
            private readonly string _path;

            public MyFactory(IMLRegression theMethod, string thePath)
            {
                _method = theMethod;
                _path = thePath;
            }

            #region IIndicatorFactory Members

            public String Name
            {
                get { return "EMA"; }
            }

            public IIndicatorListener Create()
            {
                return new MyInd(_method, _path);
            }

            #endregion
        }

        #endregion
    }
}
