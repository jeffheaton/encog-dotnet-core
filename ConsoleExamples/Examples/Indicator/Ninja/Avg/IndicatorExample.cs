using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Cloud.Indicator;
using Encog.ML.Factory;
using Encog.ML;
using Encog.Util.File;
using Encog.ML.Data;
using Encog.Util.Simple;
using System.IO;
using Encog.ML.Train;
using Encog.Neural.Networks.Training.Propagation.Manhattan;
using Encog.ML.Train.Strategy;
using Encog.Persist;
using Encog.Cloud.Indicator.Server;
using ConsoleExamples.Examples;

namespace Encog.Examples.Indicator.Ninja.Avg
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
        public const int PORT = 5128;

        /// <summary>
        /// The path to store files at.
        /// </summary>
        private string path;

        private IExampleInterface app;

        public class MyFactory : IIndicatorFactory
        {
            private IMLRegression _method;
            private string _path;

            public MyFactory(IMLRegression theMethod, string thePath)
            {
                _method = theMethod;
                _path = thePath;
            }

            public String Name
            {
                get
                {
                    return "EMA";
                }
            }

            public IIndicatorListener Create()
            {
                return new MyInd(_method, _path);
            }
        }

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(IndicatorExample),
                    "indicator-sma",
                    "Provide an example indicator.",
                    "Provide a NinjaTrader example indicator built on two SMA's.");
                return info;
            }
        }

        /// <summary>
        /// Construct the indicator example. 
        /// </summary>
        public IndicatorExample()
        {
        }

        /// <summary>
        /// Perform the training option.
        /// </summary>
        public void Train()
        {
            // first, create the machine learning method
            MLMethodFactory methodFactory = new MLMethodFactory();
            IMLMethod method = methodFactory.Create(Config.METHOD_TYPE, Config.METHOD_ARCHITECTURE, Config.INPUT_WINDOW, 1);

            // second, create the data set	
            string filename = FileUtil.CombinePath(new FileInfo(this.path), Config.FILENAME_TRAIN).ToString();
            IMLDataSet dataSet = EncogUtility.LoadEGB2Memory(new FileInfo(filename));

            // third, create the trainer
            MLTrainFactory trainFactory = new MLTrainFactory();
            IMLTrain train = trainFactory.Create(method, dataSet, Config.TRAIN_TYPE, Config.TRAIN_PARAMS);
            // reset if improve is less than 1% over 5 cycles
            if (method is IMLResettable && !(train is ManhattanPropagation))
            {
                train.AddStrategy(new RequiredImprovementStrategy(500));
            }

            // fourth, train and evaluate.
            EncogUtility.TrainToError(train, Config.TARGET_ERROR);
            method = train.Method;
            EncogDirectoryPersistence.SaveObject(FileUtil.CombinePath(new FileInfo(this.path), Config.METHOD_NAME), method);

            // finally, write out what we did
            Console.WriteLine("Machine Learning Type: " + Config.METHOD_TYPE);
            Console.WriteLine("Machine Learning Architecture: " + Config.METHOD_ARCHITECTURE);

            Console.WriteLine("Training Method: " + Config.TRAIN_TYPE);
            Console.WriteLine("Training Args: " + Config.TRAIN_PARAMS);
        }

        /// <summary>
        /// Perform the calibrate option.
        /// </summary>
        private void Calibrate()
        {
            GenerateTraining gen = new GenerateTraining(this.path);
            gen.Calibrate();
        }

        /// <summary>
        /// Perform the generate option.
        /// </summary>
        private void Generate()
        {
            Console.WriteLine("Generating training data... please wait...");
            GenerateTraining gen = new GenerateTraining(this.path);
            gen.Generate();
            Console.WriteLine("Training data has been generated.");
        }

        /// <summary>
        /// Perform the clear option.
        /// </summary>
        private void Clear()
        {
            string[] list = System.IO.Directory.GetFiles(path);

            foreach (string file in list)
            {
                string fn = FileUtil.GetFileName(new FileInfo(file)).ToString();
                if (fn.StartsWith("collected") && fn.EndsWith(".csv"))
                {
                    new FileInfo(file).Delete();
                }
            }

            Console.WriteLine("Directory cleared of captured financial data.");

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
                Console.WriteLine("Ready to collect data from remote indicator.");
            }
            else
            {
                method = (IMLRegression)EncogDirectoryPersistence.LoadObject(FileUtil.CombinePath(new FileInfo(this.path), Config.METHOD_NAME));
                Console.WriteLine("Indicator ready.");
            }

            Console.WriteLine("Waiting for connections on port " + PORT);

            IndicatorServer server = new IndicatorServer();
            server.AddListener(this);

            server.AddIndicatorFactory(new MyFactory(method, this.path));
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

        /// <summary>
        /// The program entry point. 
        /// </summary>
        /// <param name="app">The arguments to run the program with.</param>
        public void Execute(IExampleInterface app)
        {
            this.app = app;

            string[] args = app.Args;

            if (args.Length != 2)
            {
                Console.WriteLine("Usage: IndicatorExample [clear/collect/generate/train/run] [work path]");
            }
            else
            {
                this.path = args[1];
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

        /// <summary>
        /// Called when a connection is made by a trading platform.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="hasOpened">True, if we are opening.</param>
        public void NotifyConnections(IndicatorLink link, bool hasOpened)
        {
            if (hasOpened)
            {
                Console.WriteLine("Connection from " + link.ClientSocket.RemoteEndPoint.ToString()
                        + " established.");
            }
            else if (!hasOpened)
            {
                Console.WriteLine("Connection from " + link.ClientSocket.RemoteEndPoint.ToString()
                        + " terminated.");
            }
        }
    }
}
