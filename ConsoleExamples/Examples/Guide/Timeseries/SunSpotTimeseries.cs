using System;
using System.IO;
using System.Text;
using ConsoleExamples.Examples;
using Encog.Bot;
using Encog.MathUtil.Error;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Versatile;
using Encog.ML.Data.Versatile.Columns;
using Encog.ML.Data.Versatile.Sources;
using Encog.ML.Factory;
using Encog.ML.Model;
using Encog.Util.Arrayutil;
using Encog.Util.CSV;

namespace Encog.Examples.Guide.Timeseries
{
    public class SunSpotTimeseries : IExample
    {
        public const string DataUrl = "http://solarscience.msfc.nasa.gov/greenwch/spot_num.txt";
        public const int WindowSize = 3;
        private string _tempPath;

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof (SunSpotTimeseries),
                    "guide-sunspots",
                    "Encog Guide: Time Series Regression: Predict Sunspots.",
                    "This example shows how to use EncogModel with the sunspot Data Set.");
                return info;
            }
        }


        /// <summary>
        ///     Program entry point.
        /// </summary>
        /// <param name="app">Holds arguments and other info.</param>
        public void Execute(IExampleInterface app)
        {
            ErrorCalculation.Mode = ErrorCalculationMode.RMS;
            // Download the data that we will attempt to model.
            string filename = DownloadData(app.Args);

            // Define the format of the data file.
            // This area will change, depending on the columns and
            // format of the file that you are trying to model.
            var format = new CSVFormat('.', ' '); // decimal point and
            // space separated
            IVersatileDataSource source = new CSVDataSource(filename, true,
                format);

            var data = new VersatileMLDataSet(source);
            data.NormHelper.Format = format;

            ColumnDefinition columnSSN = data.DefineSourceColumn("SSN",
                ColumnType.Continuous);
            ColumnDefinition columnDEV = data.DefineSourceColumn("DEV",
                ColumnType.Continuous);

            // Analyze the data, determine the min/max/mean/sd of every column.
            data.Analyze();

            // Use SSN & DEV to predict SSN. For time-series it is okay to have
            // SSN both as
            // an input and an output.
            data.DefineInput(columnSSN);
            data.DefineInput(columnDEV);
            data.DefineOutput(columnSSN);

            // Create feedforward neural network as the model type.
            // MLMethodFactory.TYPE_FEEDFORWARD.
            // You could also other model types, such as:
            // MLMethodFactory.SVM: Support Vector Machine (SVM)
            // MLMethodFactory.TYPE_RBFNETWORK: RBF Neural Network
            // MLMethodFactor.TYPE_NEAT: NEAT Neural Network
            // MLMethodFactor.TYPE_PNN: Probabilistic Neural Network
            var model = new EncogModel(data);
            model.SelectMethod(data, MLMethodFactory.TypeFeedforward);

            // Send any output to the console.
            model.Report = new ConsoleStatusReportable();

            // Now normalize the data. Encog will automatically determine the
            // correct normalization
            // type based on the model you chose in the last step.
            data.Normalize();

            // Set time series.
            data.LeadWindowSize = 1;
            data.LagWindowSize = WindowSize;

            // Hold back some data for a final validation.
            // Do not shuffle the data into a random ordering. (never shuffle
            // time series)
            // Use a seed of 1001 so that we always use the same holdback and
            // will get more consistent results.
            model.HoldBackValidation(0.3, false, 1001);

            // Choose whatever is the default training type for this model.
            model.SelectTrainingType(data);

            // Use a 5-fold cross-validated train. Return the best method found.
            // (never shuffle time series)
            var bestMethod = (IMLRegression) model.Crossvalidate(5,
                false);

            // Display the training and validation errors.
            Console.WriteLine(@"Training error: "
                              + model.CalculateError(bestMethod,
                                  model.TrainingDataset));
            Console.WriteLine(@"Validation error: "
                              + model.CalculateError(bestMethod,
                                  model.ValidationDataset));

            // Display our normalization parameters.
            NormalizationHelper helper = data.NormHelper;
            Console.WriteLine(helper.ToString());

            // Display the final model.
            Console.WriteLine(@"Final model: " + bestMethod);

            // Loop over the entire, original, dataset and feed it through the
            // model. This also shows how you would process new data, that was
            // not part of your training set. You do not need to retrain, simply
            // use the NormalizationHelper class. After you train, you can save
            // the NormalizationHelper to later normalize and denormalize your
            // data.
            source.Close();
            var csv = new ReadCSV(filename, true, format);
            var line = new String[2];

            // Create a vector to hold each time-slice, as we build them.
            // These will be grouped together into windows.
            var slice = new double[2];
            var window = new VectorWindow(WindowSize + 1);
            IMLData input = helper.AllocateInputVector(WindowSize + 1);

            // Only display the first 100
            int stopAfter = 100;

            while (csv.Next() && stopAfter > 0)
            {
                var result = new StringBuilder();

                line[0] = csv.Get(2); // ssn
                line[1] = csv.Get(3); // dev
                helper.NormalizeInputVector(line, slice, false);

                // enough data to build a full window?
                if (window.IsReady())
                {
                    window.CopyWindow(((BasicMLData) input).Data, 0);
                    String correct = csv.Get(2); // trying to predict SSN.
                    IMLData output = bestMethod.Compute(input);
                    String predicted = helper
                        .DenormalizeOutputVectorToString(output)[0];

                    result.Append(line);
                    result.Append(" -> predicted: ");
                    result.Append(predicted);
                    result.Append("(correct: ");
                    result.Append(correct);
                    result.Append(")");

                    Console.WriteLine(result.ToString());
                }

                // Add the normalized slice to the window. We do this just after
                // the after checking to see if the window is ready so that the
                // window is always one behind the current row. This is because
                // we are trying to predict next row.
                window.Add(slice);

                stopAfter--;
            }
            csv.Close();

            // Delete data file and shut down.
            File.Delete(filename);
            EncogFramework.Instance.Shutdown();
        }

        public string DownloadData(string[] args)
        {
            if (args.Length != 0)
            {
                _tempPath = args[0];
            }
            else
            {
                _tempPath = Path.GetTempPath();
            }


            string irisFile = Path.Combine(_tempPath, "sunspot.csv");
            BotUtil.DownloadPage(new Uri(DataUrl), irisFile);
            Console.WriteLine(@"Downloading dataset to: " + irisFile);
            return irisFile;
        }
    }
}