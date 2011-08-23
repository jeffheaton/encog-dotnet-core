using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Temporal;
using Encog.ML.SVM;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Pattern;
using Encog.Persist;
using Encog.Util.Arrayutil;
using Encog.Util.CSV;
using Encog.Util.File;
using Encog.Util.Normalize;
using Encog.Util.Normalize.Input;
using Encog.Util.Normalize.Output;
using Encog.Util.Normalize.Target;
using Encog.Util.Simple;

namespace Encog.Util.NetworkUtil
{
    /// <summary>
    /// A class with many helpers.
    /// </summary>
    public class NetworkUtility
    {
        /// <summary>
        /// Quickly an IMLDataset from a double array using the TemporalWindow array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="inputsize">The inputsize.</param>
        /// <param name="outputsize">The outputsize.</param>
        /// <returns></returns>
        public static IMLDataSet QuickTrainingFromDoubleArray(double [] array, int inputsize, int outputsize)
        {
            TemporalWindowArray temp = new TemporalWindowArray(inputsize,outputsize);
            temp.Analyze(array);
            return temp.Process(array);
        }

        /// <summary>
        /// Saves the matrix to the desired file location.
        /// </summary>
        /// <param name="surface">The surface.</param>
        /// <param name="fileName">Name of the file.</param>
        private static void SaveMatrix(double[][] surface, string fileName)
        {
            using (var sw = new System.IO.StreamWriter(fileName, false))
            {
                for (int i = 0; i < surface.Length; i++)
                {
                    for (int j = 0; j < surface[i].Length; j++)
                    {
                        if (double.IsNaN(surface[i][j]))
                            sw.Write(",");
                        else
                            sw.Write(surface[i][j] + ",");
                    }
                    sw.WriteLine();
                }
            }
        }



        /// <summary>
        /// Loads a matrix from a file to a double[][] array.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        private static double[][] LoadMatrix(string fileName)
        {
            string[] allLines = System.IO.File.ReadAllLines(fileName);

            double[][] matrix = new double[allLines.Length][];

            for (int i = 0; i < allLines.Length; i++)
            {
                string[] values = allLines[i].Split(',');
                matrix[i] = new double[values.Length];

                for (int j = 0; j < values.Length; j++)
                {
                    if (values[j] != "")
                        matrix[i][j] = Convert.ToDouble(values[j]);
                    else
                        matrix[i][j] = Double.NaN;
                }
            }

            return matrix;
        }
        /// <summary>
        /// Processes  a data and ideal double array into a IMLDatapair.
        /// This is used to build inputs for a neural network.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="ideal">The ideal.</param>
        /// <param name="_inputWindow">The _input window.</param>
        /// <param name="_predictWindow">The _predict window.</param>
        /// <returns></returns>
        public static IMLDataPair ProcessPair(double[] data, double[] ideal, int _inputWindow, int _predictWindow)
        {
            IMLDataSet result = new BasicMLDataSet();
            for (int i = 0; i < data.Length; i++)
            {
                IMLData inputData = new BasicMLData(_inputWindow);
                IMLData idealData = new BasicMLData(_predictWindow);
                int index = i;
                // handle input window
                for (int j = 0; j < _inputWindow; j++)
                {
                    inputData[j] = data[index++];
                }
                index = 0;
                // handle predict window
                for (int j = 0; j < _predictWindow; j++)
                {
                    idealData[j] = ideal[index++];
                }
                IMLDataPair pair = new BasicMLDataPair(inputData, idealData);
                return pair;
            }
            return null;
        }

        /// <summary>
        /// Calculates the percents change from one number to the next.
        /// First input the current number , then input the previous value.
        /// </summary>
        public static double CalculatePercents(double CurrentValue, double PreviousValue)
        {
            if (CurrentValue > 0.0 && PreviousValue > 0.0)
            {
                return (CurrentValue - PreviousValue) / PreviousValue;
            }
            return 0.0;
        }

        public static double[] CalculateRanges(double[] closingValues, double[] OpeningValues)
        {
            double range = 0;
            List<double> results = new List<double>();
            //if one is bigger than the other, something is wrong.
            if (closingValues.Length != OpeningValues.Length)
                return null;

            for (int i = 0; i < closingValues.Length; i++)
            {
                range = Math.Abs(closingValues[i] - OpeningValues[i]);
                results.Add(range);
                i++;
            }
            return results.ToArray();

        }
        /// <summary>
        /// Calculates the percents in a double serie.
        /// </summary>
        /// <param name="inputs">The inputs.</param>
        /// <returns></returns>
        public static double[] CalculatePercents(double[] inputs)
        {
            int index = 0;
            List<double> result = new List<double>();
            foreach (double input in inputs)
            {
                //we dont continue if we have no more doubles to calculate.
                if (index > 0)
                {
                    result.Add(CalculatePercents(input, inputs[index - 1]));
                }
                index++;
            }
            return result.ToArray();
        }
        /// <summary>
        /// parses one column of a csv and returns an array of doubles.
        /// you can only return one double array with this method.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="formatused">The formatused.</param>
        /// <param name="Name">The name of the column to parse..</param>
        /// <returns></returns>
        public static List<double> QuickParseCSV(string file, CSVFormat formatused, string Name)
        {
            List<double> returnedArrays = new List<double>();

            ReadCSV csv = new ReadCSV(file, true, formatused);
            List<string> ColumnNames = csv.ColumnNames.ToList();
            while (csv.Next())
            {
                returnedArrays.Add(csv.GetDouble(Name));
            }
            return returnedArrays;
        }
        /// <summary>
        /// parses one column of a csv and returns an array of doubles.
        /// you can only return one double array with this method.
        /// We are assuming CSVFormat english in this quick parse csv method.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="Name">The name of the column to parse.</param>
        /// <returns></returns>
        public static List<double> QuickParseCSV(string file, string Name)
        {
            List<double> returnedArrays = new List<double>();

            ReadCSV csv = new ReadCSV(file, true, CSVFormat.English);
            List<string> ColumnNames = csv.ColumnNames.ToList();
            while (csv.Next())
            {
                returnedArrays.Add(csv.GetDouble(Name));
            }
            return returnedArrays;
        }


        /// <summary>
        /// parses one column of a csv and returns an array of doubles.
        /// you can only return one double array with this method.
        /// We are assuming CSVFormat english in this quick parse csv method.
        /// You can input the size (number of lines) to read.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="Name">The name of the column to parse.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static List<double> QuickParseCSV(string file, string Name, int size)
        {
            List<double> returnedArrays = new List<double>();
            ReadCSV csv = new ReadCSV(file, true, CSVFormat.English);
            List<string> ColumnNames = csv.ColumnNames.ToList();
            int currentRead = 0;
            while (csv.Next() && currentRead < size)
            {
                returnedArrays.Add(csv.GetDouble(Name));
                currentRead++;
            }
            return returnedArrays;
        }

        /// <summary>
        /// parses one column of a csv and returns an array of doubles.
        /// you can only return one double array with this method.
        /// We are assuming CSVFormat english in this quick parse csv method.
        /// You can input the size (number of lines) to read and the number of lines you want to skip from the start line
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="Name">The name of the column to parse.</param>
        /// <param name="size">The size.</param>
        /// <param name="StartLine">The start line (how many lines you want to skip from the start of the file.</param>
        /// <returns></returns>
        public static List<double> QuickParseCSV(string file, string Name, int size, int StartLine)
        {
            List<double> returnedArrays = new List<double>();
            ReadCSV csv = new ReadCSV(file, true, CSVFormat.English);
            List<string> ColumnNames = csv.ColumnNames.ToList();
            int currentRead = 0;
            int currentLine = 0;
            while (csv.Next())
            {
                if (currentRead < size && currentLine > StartLine)
                {
                    returnedArrays.Add(csv.GetDouble(Name));
                    currentRead++;
                }
                currentLine++;
            }
            return returnedArrays;
        }
        /// <summary>
        /// parses one column of a csv and returns an array of doubles.
        /// you can only return one double array with this method.
        /// We are assuming CSVFormat english in this quick parse csv method.
        /// You can input the size (number of lines) to read.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="columnNumber">The column number to get.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static List<double> QuickParseCSV(string file, int columnNumber, int size)
        {
            List<double> returnedArrays = new List<double>();
            ReadCSV csv = new ReadCSV(file, true, CSVFormat.English);
            List<string> ColumnNames = csv.ColumnNames.ToList();
            int currentRead = 0;
            while (csv.Next() && currentRead < size)
            {
                returnedArrays.Add(csv.GetDouble(columnNumber));
                currentRead++;
            }
            return returnedArrays;
        }


        /// <summary>
        /// parses one column of a csv and returns an array of doubles.
        /// you can only return one double array with this method.
        /// We are assuming CSVFormat english in this quick parse csv method.
        /// You can input the size (number of lines) to read , and the number of lines you want to skip start from the first line.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="columnNumber">The column number to get.</param>
        /// <param name="size">The size.</param>
        /// <param name="startLine">The start line (how many lines you want to skip from the first line.</param>
        /// <returns></returns>
        public static List<double> QuickParseCSV(string file, int columnNumber, int size, int startLine)
        {
            List<double> returnedArrays = new List<double>();
            ReadCSV csv = new ReadCSV(file, true, CSVFormat.English);
            List<string> ColumnNames = csv.ColumnNames.ToList();
            int currentRead = 0;
            int currentLine = 0;
            while (csv.Next())
            {
                if (currentRead < size && currentLine > startLine)
                {
                    returnedArrays.Add(csv.GetDouble(columnNumber));
                    currentRead++;
                }
                currentLine++;
            }
            return returnedArrays;
        }


        /// <summary>
        /// Loads an IMLDataset training file in a given directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static IMLDataSet LoadTraining(string directory, string file)
        {
            FileInfo networkFile = FileUtil.CombinePath(new FileInfo(directory), file);
            if (!networkFile.Exists)
            {
                return null;
            }
            IMLDataSet network = (IMLDataSet)EncogUtility.LoadEGB2Memory(networkFile);
            return network;
        }


        /// <summary>
        /// Saves the network to the specified directory with the specified parameter name.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="file">The file.</param>
        /// <param name="anetwork">The network to save..</param>
        public static void SaveNetwork(string directory, string file, BasicNetwork anetwork)
        {
            FileInfo networkFile = FileUtil.CombinePath(new FileInfo(directory), file);
            EncogDirectoryPersistence.SaveObject(networkFile, anetwork);
            return;
        }

        /// <summary>
        /// Saves the network to the specified directory with the specified parameter name.
        /// This version saves super machine to file.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="file">The file.</param>
        /// <param name="anetwork">The network to save..</param>
        public static void SaveNetwork(string directory, string file, SupportVectorMachine anetwork)
        {
            FileInfo networkFile = FileUtil.CombinePath(new FileInfo(directory), file);
            EncogDirectoryPersistence.SaveObject(networkFile, anetwork);
            return;
        }

        /// <summary>
        /// Loads an basic network from the specified directory and file.
        /// You must load the network like this Loadnetwork(@directory,@file);
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static BasicNetwork LoadNetwork(string directory, string file)
        {
            FileInfo networkFile = FileUtil.CombinePath(new FileInfo(directory), file);
            // network file
            if (!networkFile.Exists)
            {
                Console.WriteLine(@"Can't read file: " + networkFile);
                return null;
            }
            var network = (BasicNetwork)EncogDirectoryPersistence.LoadObject(networkFile);
            return network;
        }

        /// <summary>
        /// Loads an basic network from the specified directory and file.
        /// You must load the network like this Loadnetwork(@directory,@file);
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static SupportVectorMachine LoadNetwork(string directory, string file, string net)
        {
            FileInfo networkFile = FileUtil.CombinePath(new FileInfo(directory), file);
            // network file
            if (!networkFile.Exists)
            {
                Console.WriteLine(@"Can't read file: " + networkFile);
                return null;
            }
            var network = (SupportVectorMachine)EncogDirectoryPersistence.LoadObject(networkFile);
            return network;
        }
        /// <summary>
        /// Saves an IMLDataset to a file.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="file">The file.</param>
        /// <param name="trainintoSave">The traininto save.</param>
        public static void SaveTraining(string directory, string file, IMLDataSet trainintoSave)
        {
            FileInfo networkFile = FileUtil.CombinePath(new FileInfo(directory), file);
            //save our training file.
            EncogUtility.SaveEGB(networkFile, trainintoSave);
            return;
        }

        /// <summary>
        /// Generates a temporal data set with a given double serie.
        /// uses Type percent change.
        /// </summary>
        /// <param name="inputserie">The inputserie.</param>
        /// <param name="windowsize">The windowsize.</param>
        /// <param name="predictsize">The predictsize.</param>
        /// <returns></returns>
        public static TemporalMLDataSet GenerateTrainingWithPercentChangeOnSerie(double[] inputserie, int windowsize, int predictsize)
        {
            TemporalMLDataSet result = new TemporalMLDataSet(windowsize, predictsize);
            TemporalDataDescription desc = new TemporalDataDescription(TemporalDataDescription.Type.PercentChange, true, true);
            result.AddDescription(desc);
            for (int index = 0; index < inputserie.Length - 1; index++)
            {
                TemporalPoint point = new TemporalPoint(1);
                point.Sequence = index;
                point.Data[0] = inputserie[index];
                result.Points.Add(point);
            }
            result.Generate();
            return result;
        }
        /// <summary>
        /// Makes random inputs by randomizing with encog randomize , the normal random from net framework library.
        /// a quick and easy way to test data and networks.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        public static double[] MakeInputs(int number)
        {
            Random rdn = new Random();
            Encog.MathUtil.Randomize.RangeRandomizer encogRnd = new Encog.MathUtil.Randomize.RangeRandomizer(-1, 1);
            double[] x = new double[number];
            for (int i = 0; i < number; i++)
            {
                x[i] = encogRnd.Randomize((rdn.NextDouble()));

            }
            return x;
        }

        /// <summary>
        /// Calculates and returns the copied array.
        /// This is used in live data , when you want have a full array of doubles but you want to cut from a starting position
        /// and return only from that point to the end.
        /// example you have 1000 doubles , but you want only the last 100.
        /// input size is the input you must set to 100.
        /// </summary>
        /// <param name="inputted">The inputted.</param>
        /// <param name="inputsize">The input neuron size (window size).</param>
        /// <returns></returns>
        public double[] ReturnArrayOnSize(double[] inputted, int inputsize)
        {
            double[] arr = new double[inputsize];
            int howBig = 0;
            if (inputted.Length > inputsize)
            {
                howBig = inputted.Length - inputsize;
            }
            EngineArray.ArrayCopy(inputted, howBig, arr, 0, inputsize);
            return arr;
        }


        /// <summary>
        /// Returns a randomized IMLDataset ready for use.
        /// </summary>
        /// <param name="sizeofInputs">The sizeof inputs.</param>
        /// <param name="sizeOfOutputs">The size of outputs.</param>
        /// <returns></returns>
        public static IMLDataSet ReturnRandomizedDataSet(int sizeofInputs, int sizeOfOutputs)
        {
            double[] res = MakeInputs(sizeofInputs);
           return QuickTrainingFromDoubleArray(res, sizeofInputs, sizeOfOutputs);

        }
        /// <summary>
        /// Generates the Temporal Training based on an array of doubles.
        /// You need to have 2 array of doubles [] for this method to work!
        /// </summary>
        /// <param name="inputsize">The inputsize.</param>
        /// <param name="outputsize">The outputsize.</param>
        /// <param name="Arraydouble">The arraydouble.</param>
        /// <returns></returns>
        public static TemporalMLDataSet GenerateTraining(int inputsize, int outputsize, params double[][] Arraydouble)
        {
            if (Arraydouble.Length <2)
                return null;
            if (Arraydouble.Length > 2)
                return null;
            TemporalMLDataSet result = new TemporalMLDataSet(inputsize, outputsize);
            TemporalDataDescription desc = new TemporalDataDescription(new ActivationTANH(),TemporalDataDescription.Type.PercentChange, true, true);
            result.AddDescription(desc);
            TemporalPoint point = new TemporalPoint(Arraydouble.Length);
            int currentindex;
           for (int w =0 ;w <Arraydouble[0].Length;w++)
           {
               //We have filled in one dimension now lets put them in our temporal dataset.
               for (int year = 0; year < Arraydouble.Length-1; year++)
               {
                   //We have as many points as we passed the array of doubles.
                   point = new TemporalPoint(Arraydouble.Length);
                   //our first sequence (0).
                   point.Sequence = w;
                   //point 0 is double[0] array.
                   point.Data[0] = Arraydouble[0][w];
                   point.Data[1] = Arraydouble[1][w];
                   //we add the point..
               }
               result.Points.Add(point);
           }
            result.Generate();
            return result;
        }

        /// <summary>
        /// Generates a temporal data set with a given double serie or a any number of double series , making your inputs.
        /// uses Type percent change.
        /// </summary>
        /// <param name="windowsize">The windowsize.</param>
        /// <param name="predictsize">The predictsize.</param>
        /// <param name="inputserie">The inputserie.</param>
        /// <returns></returns>
        public static TemporalMLDataSet GenerateTrainingWithPercentChangeOnSerie(int windowsize, int predictsize, params double[][] inputserie)
        {
            TemporalMLDataSet result = new TemporalMLDataSet(windowsize, predictsize);
            TemporalDataDescription desc = new TemporalDataDescription(TemporalDataDescription.Type.PercentChange, true,
                                                                       true);
            result.AddDescription(desc);
            foreach (double[] t in inputserie)
            {
                for (int j = 0; j < t.Length; j++)
                {
                    TemporalPoint point = new TemporalPoint(1);
                    point.Sequence = j;
                    point.Data[0] = t[j];
                    result.Points.Add(point);
                }
                result.Generate();
                return result;
            }
            return null;
        }

        /// <summary>
        /// Generates the training with raw serie.
        /// </summary>
        /// <param name="inputserie">The inputserie.</param>
        /// <param name="windowsize">The windowsize.</param>
        /// <param name="predictsize">The predictsize.</param>
        /// <returns></returns>
        public static TemporalMLDataSet GenerateTrainingWithRawSerie(double[] inputserie, int windowsize, int predictsize)
        {
            TemporalMLDataSet result = new TemporalMLDataSet(windowsize, predictsize);

            TemporalDataDescription desc = new TemporalDataDescription(
                    TemporalDataDescription.Type.Raw, true, true);
            result.AddDescription(desc);
            for (int index = 0; index < inputserie.Length - 1; index++)
            {
                TemporalPoint point = new TemporalPoint(1);
                point.Sequence = index;
                point.Data[0] = inputserie[index];
                result.Points.Add(point);
            }

            result.Generate();
            return result;
        }

        /// <summary>
        /// Generates the training with delta change on serie.
        /// </summary>
        /// <param name="inputserie">The inputserie.</param>
        /// <param name="windowsize">The windowsize.</param>
        /// <param name="predictsize">The predictsize.</param>
        /// <returns></returns>
        public static TemporalMLDataSet GenerateTrainingWithDeltaChangeOnSerie(double[] inputserie, int windowsize, int predictsize)
        {
            TemporalMLDataSet result = new TemporalMLDataSet(windowsize, predictsize);

            TemporalDataDescription desc = new TemporalDataDescription(
                    TemporalDataDescription.Type.DeltaChange, true, true);
            result.AddDescription(desc);

            for (int index = 0; index < inputserie.Length - 1; index++)
            {
                TemporalPoint point = new TemporalPoint(1);
                point.Sequence = index;
                point.Data[0] = inputserie[index];
                result.Points.Add(point);
            }
            result.Generate();
            return result;
        }



        /// <summary>
        /// Processes the specified double serie into an IMLDataset.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="_inputWindow">The _input window.</param>
        /// <param name="_predictWindow">The _predict window.</param>
        /// <returns></returns>
        public static IMLDataSet ProcessDoubleSerieIntoIMLDataset(double[] data, int _inputWindow, int _predictWindow)
        {
            IMLDataSet result = new BasicMLDataSet();
            int totalWindowSize = _inputWindow + _predictWindow;
            int stopPoint = data.Length - totalWindowSize;
            for (int i = 0; i < stopPoint; i++)
            {
                IMLData inputData = new BasicMLData(_inputWindow);
                IMLData idealData = new BasicMLData(_predictWindow);

                int index = i;

                // handle input window
                for (int j = 0; j < _inputWindow; j++)
                {
                    inputData[j] = data[index++];
                }

                // handle predict window
                for (int j = 0; j < _predictWindow; j++)
                {
                    idealData[j] = data[index++];
                }
                IMLDataPair pair = new BasicMLDataPair(inputData, idealData);
                result.Add(pair);
            }

            return result;
        }


        /// <summary>
        /// Normalizes arrays.
        /// You can retrieve the array normalizer stats throughout the program's life without having to instantiate a new normalizer.
        /// </summary>
        public static readonly Encog.Util.Arrayutil.NormalizeArray ArrayNormalizer = new Encog.Util.Arrayutil.NormalizeArray();

        
        /// <summary>
        /// Normalizes an array.
        /// </summary>
        /// <param name="inputArray">The input array.</param>
        /// <returns>a normalized array of doubles</returns>
        public static double[] NormalizeThisArray(double[] inputArray)
        {
            return ArrayNormalizer.Process(inputArray);
        }
        /// <summary>
        /// Denormalizes the double.
        /// </summary>
        /// <param name="doubleToDenormalize">The double to denormalize.</param>
        /// <returns></returns>
        public static double DenormalizeDouble(double doubleToDenormalize)
        {
            return ArrayNormalizer.Stats.DeNormalize(doubleToDenormalize);
        }

        /// <summary>
        /// Loads a normalization from the specified directory and file.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="file">The file.</param>
        /// <returns>a datanormalization object</returns>
        public static DataNormalization LoadNormalization(string directory, string file)
        {
            DataNormalization norm = null;
            FileInfo networkFile = FileUtil.CombinePath(new FileInfo(directory), file);
            if (networkFile.Exists)
            {
                norm = (DataNormalization)SerializeObject.Load(networkFile.FullName);
            }

            if (norm == null)
            {
                Console.WriteLine(@"Can't find normalization resource: "
                                  + directory + file);
                return null;
            }
            return norm;
        }

        /// <summary>
        /// Saves a normalization to the specified folder with the specified name.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="file">The file.</param>
        /// <param name="normTosave">The norm tosave.</param>
        public static void SaveNormalization(string directory, string file, DataNormalization normTosave)
        {
            SerializeObject.Save(directory + file, normTosave);
        }



        /// <summary>
        /// Creates the elman network as a basicnetwork.
        /// This method create a elmhan patterns but returns a basic network (ready for load/save).
        /// </summary>
        /// <param name="inputsize">The inputsize.</param>
        /// <param name="outputsize">The outputsize.</param>
        /// <param name="hiddenlayers">The hiddenlayers.</param>
        /// <param name="activate">The activation function to use..</param>
        /// <returns>a basic network</returns>
        public static BasicNetwork CreateElmanNetwork(int inputsize, int outputsize, int hiddenlayers, IActivationFunction activate)
        {
            // construct an Elman type network
            ElmanPattern pattern = new ElmanPattern();
            pattern.ActivationFunction = activate;
            pattern.InputNeurons = inputsize;
            pattern.AddHiddenLayer(hiddenlayers);
            pattern.OutputNeurons = outputsize;
            return (BasicNetwork)pattern.Generate();
        }
    }
}
