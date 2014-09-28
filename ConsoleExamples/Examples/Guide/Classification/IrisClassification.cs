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
using Encog.ML.Data.Versatile.Sources;
using Encog.ML.Factory;
using Encog.ML.Model;
using Encog.Util.CSV;

namespace Encog.Examples.Guide.Classification
{
    public class IrisClassification : IExample
    {
        public const string DataUrl = "https://archive.ics.uci.edu/ml/machine-learning-databases/iris/iris.data";

        private string _tempPath;

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof (IrisClassification),
                    "guide-iris",
                    "Encog Guide: Classification: The Iris Data Set.",
                    "This example shows how to use EncogModel with the Iris Data Set.");
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
            string irisFile = DownloadData(app.Args);

            // Define the format of the data file.
            // This area will change, depending on the columns and 
            // format of the file that you are trying to model.
            IVersatileDataSource source = new CSVDataSource(irisFile, false,
                CSVFormat.DecimalPoint);
            var data = new VersatileMLDataSet(source);
            data.DefineSourceColumn("sepal-length", 0, ColumnType.Continuous);
            data.DefineSourceColumn("sepal-width", 1, ColumnType.Continuous);
            data.DefineSourceColumn("petal-length", 2, ColumnType.Continuous);
            data.DefineSourceColumn("petal-width", 3, ColumnType.Continuous);

            // Define the column that we are trying to predict.
            ColumnDefinition outputColumn = data.DefineSourceColumn("species", 4,
                ColumnType.Nominal);

            // Analyze the data, determine the min/max/mean/sd of every column.
            data.Analyze();

            // Map the prediction column to the output of the model, and all
            // other columns to the input.
            data.DefineSingleOutputOthersInput(outputColumn);

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
            Console.WriteLine(@"Final model: " + bestMethod);

            // Loop over the entire, original, dataset and feed it through the model.
            // This also shows how you would process new data, that was not part of your
            // training set.  You do not need to retrain, simply use the NormalizationHelper
            // class.  After you train, you can save the NormalizationHelper to later
            // normalize and denormalize your data.
            source.Close();
            var csv = new ReadCSV(irisFile, false, CSVFormat.DecimalPoint);
            var line = new String[4];
            IMLData input = helper.AllocateInputVector();

            while (csv.Next())
            {
                var result = new StringBuilder();
                line[0] = csv.Get(0);
                line[1] = csv.Get(1);
                line[2] = csv.Get(2);
                line[3] = csv.Get(3);
                String correct = csv.Get(4);
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

            // Delete data file ande shut down.
            File.Delete(irisFile);
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


            string irisFile = Path.Combine(_tempPath, "iris.csv");
            BotUtil.DownloadPage(new Uri(DataUrl), irisFile);
            Console.WriteLine(@"Downloading Iris dataset to: " + irisFile);
            return irisFile;
        }
    }
}