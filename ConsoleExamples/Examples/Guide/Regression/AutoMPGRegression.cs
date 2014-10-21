using System;
using System.IO;
using System.Text;
using ConsoleExamples.Examples;
using Encog.Bot;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Versatile;
using Encog.ML.Data.Versatile.Columns;
using Encog.ML.Data.Versatile.Missing;
using Encog.ML.Data.Versatile.Sources;
using Encog.ML.Factory;
using Encog.ML.Model;
using Encog.Util.CSV;

namespace Encog.Examples.Guide.Regression
{
    public class AutoMPGRegression : IExample
    {
        public const string DataUrl = "https://archive.ics.uci.edu/ml/machine-learning-databases/auto-mpg/auto-mpg.data";

        private string _tempPath;

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof (AutoMPGRegression),
                    "guide-auto-mpg",
                    "Encog Guide: Regression: Predict an auto's MPG.",
                    "This example shows how to use EncogModel with the auto MPG Data Set.");
                return info;
            }
        }

        /// <summary>
        ///     Program entry point.
        /// </summary>
        /// <param name="app">Holds arguments and other info.</param>
        public void Execute(IExampleInterface app)
        {
            // Download the data that we will attempt to model.
            string filename = DownloadData(app.Args);

            // Define the format of the data file.
            // This area will change, depending on the columns and 
            // format of the file that you are trying to model.
            var format = new CSVFormat('.', ' '); // decimal point and space separated
            IVersatileDataSource source = new CSVDataSource(filename, false, format);

            var data = new VersatileMLDataSet(source);
            data.NormHelper.Format = format;

            ColumnDefinition columnMPG = data.DefineSourceColumn("mpg", 0, ColumnType.Continuous);
            ColumnDefinition columnCylinders = data.DefineSourceColumn("cylinders", 1, ColumnType.Ordinal);
            // It is very important to predefine ordinals, so that the order is known.
            columnCylinders.DefineClass(new[] {"3", "4", "5", "6", "8"});
            data.DefineSourceColumn("displacement", 2, ColumnType.Continuous);
            ColumnDefinition columnHorsePower = data.DefineSourceColumn("horsepower", 3, ColumnType.Continuous);
            data.DefineSourceColumn("weight", 4, ColumnType.Continuous);
            data.DefineSourceColumn("acceleration", 5, ColumnType.Continuous);
            ColumnDefinition columnModelYear = data.DefineSourceColumn("model_year", 6, ColumnType.Ordinal);
            columnModelYear.DefineClass(new[]
            {"70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "80", "81", "82"});
            data.DefineSourceColumn("origin", 7, ColumnType.Nominal);

            // Define how missing values are represented.
            data.NormHelper.DefineUnknownValue("?");
            data.NormHelper.DefineMissingHandler(columnHorsePower, new MeanMissingHandler());

            // Analyze the data, determine the min/max/mean/sd of every column.
            data.Analyze();

            // Map the prediction column to the output of the model, and all
            // other columns to the input.
            data.DefineSingleOutputOthersInput(columnMPG);

            // Create feedforward neural network as the model type. MLMethodFactory.TYPE_FEEDFORWARD.
            // You could also other model types, such as:
            // MLMethodFactory.SVM:  Support Vector Machine (SVM)
            // MLMethodFactory.TYPE_RBFNETWORK: RBF Neural Network
            // MLMethodFactor.TYPE_NEAT: NEAT Neural Network
            // MLMethodFactor.TYPE_PNN: Probabilistic Neural Network
            var model = new EncogModel(data);
            model.SelectMethod(data, MLMethodFactory.TypeFeedforward);

            // Send any output to the console.
            model.Report = new ConsoleStatusReportable();

            // Now normalize the data.  Encog will automatically determine the correct normalization
            // type based on the model you chose in the last step.
            data.Normalize();

            // Hold back some data for a final validation.
            // Shuffle the data into a random ordering.
            // Use a seed of 1001 so that we always use the same holdback and will get more consistent results.
            model.HoldBackValidation(0.3, true, 1001);

            // Choose whatever is the default training type for this model.
            model.SelectTrainingType(data);

            // Use a 5-fold cross-validated train.  Return the best method found.
            var bestMethod = (IMLRegression) model.Crossvalidate(5, true);

            // Display the training and validation errors.
            Console.WriteLine(@"Training error: " + model.CalculateError(bestMethod, model.TrainingDataset));
            Console.WriteLine(@"Validation error: " + model.CalculateError(bestMethod, model.ValidationDataset));

            // Display our normalization parameters.
            NormalizationHelper helper = data.NormHelper;
            Console.WriteLine(helper.ToString());

            // Display the final model.
            Console.WriteLine("Final model: " + bestMethod);

            // Loop over the entire, original, dataset and feed it through the model.
            // This also shows how you would process new data, that was not part of your
            // training set.  You do not need to retrain, simply use the NormalizationHelper
            // class.  After you train, you can save the NormalizationHelper to later
            // normalize and denormalize your data.
            source.Close();
            var csv = new ReadCSV(filename, false, format);
            var line = new String[7];
            IMLData input = helper.AllocateInputVector();

            while (csv.Next())
            {
                var result = new StringBuilder();

                line[0] = csv.Get(1);
                line[1] = csv.Get(2);
                line[2] = csv.Get(3);
                line[3] = csv.Get(4);
                line[4] = csv.Get(5);
                line[5] = csv.Get(6);
                line[6] = csv.Get(7);

                String correct = csv.Get(0);
                helper.NormalizeInputVector(line, ((BasicMLData) input).Data, false);
                IMLData output = bestMethod.Compute(input);
                String irisChosen = helper.DenormalizeOutputVectorToString(output)[0];

                result.Append(line);
                result.Append(" -> predicted: ");
                result.Append(irisChosen);
                result.Append("(correct: ");
                result.Append(correct);
                result.Append(")");

                Console.WriteLine(result.ToString());
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


            string irisFile = Path.Combine(_tempPath, "auto-mpg.csv");
            BotUtil.DownloadPage(new Uri(DataUrl), irisFile);
            Console.WriteLine(@"Downloading dataset to: " + irisFile);
            return irisFile;
        }
    }
}