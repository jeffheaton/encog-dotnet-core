using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Temporal;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.NeuralData;
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
    /// A few method to help load , save training and networks.
    /// Also a few helper method to use normalization methods.
    /// </summary>
    public class NetworkUtility
    {


        /// <summary>
        /// Creates the ideal/input array from a list of inputs (double arrays).
        /// can be as many inputs/outputs as needed.
        /// </summary>
        /// <param name="nbofSeCondDimendsion">The nb of points in the inputs (must be the same).</param>
        /// <param name="inputs">The inputs.</param>
        /// <returns></returns>
        public static double[][] CreateIdealorInputs(int nbofSeCondDimendsion,params object[] inputs)
        {
            double[][] result = EngineArray.AllocateDouble2D(inputs.Length, nbofSeCondDimendsion);
            int i = 0, k = 0;
            foreach (double[] doubles in inputs)
            {
                foreach (double d in doubles)
                {
                    result[i][k] = d;
                    k++;
                }
                if (i < inputs.Length - 1)
                {
                    i++;
                    k = 0;
                }
            }
            return result;
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
            IMLDataSet network = (IMLDataSet) EncogUtility.LoadEGB2Memory(networkFile);
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
       /// Calculates the percents change from one number to the next.
       /// First input the current number , then input the previous value.
       /// </summary>
       public static double CalculatePercents(double CurrentValue , double PreviousValue)
       {
          if (CurrentValue > 0.0 && PreviousValue > 0.0) 
          {
              return (CurrentValue - PreviousValue) / PreviousValue;
          }  
           return 0.0;
       }


       /// <summary>
       /// Calculates the percents in a double serie.
       /// </summary>
       /// <param name="inputs">The inputs.</param>
       /// <returns></returns>
        public static double[] CalculatePercents (double[] inputs)
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
        /// Copy a doubles series to a List of double.
        /// return the filled List of doubles.
        /// </summary>
        /// <param name="inputs">The inputs.</param>
        /// <returns></returns>
        public static List<double> CopydoubleArrayToList(double[]inputs)
        {
            List<double> result = new List<double>();

            foreach (var input in inputs)
            {
                result.Add(input);
            }
            return result;
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
        public static TemporalMLDataSet GenerateTrainingWithPercentChangeOnSerie(double [] inputserie, int windowsize , int predictsize)
        {
            TemporalMLDataSet result = new TemporalMLDataSet(windowsize,predictsize);

            TemporalDataDescription desc = new TemporalDataDescription(
                    TemporalDataDescription.Type.PercentChange, true, true);
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
        /// Generates a temporal data set with a given double serie.
        /// uses Type raw.
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
        /// this method needs the column number.
        /// We are assuming english format for the csvformat.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="colnumber">The colnumber.</param>
        /// <returns></returns>
        public static List<double> QuickParseCSV(string file, int colnumber)
        {
            List<double> returnedArrays = new List<double>();
            ReadCSV csv = new ReadCSV(file, true, CSVFormat.English);
            List<string> ColumnNames = csv.ColumnNames.ToList();
            while (csv.Next())
            {
                returnedArrays.Add(csv.GetDouble(colnumber));
            }
            return returnedArrays;
        }


        /// <summary>
        /// Writes an array of double to file using a CSVFormat.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="arrayToWrite">The array to write.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="Appender">The appender.</param>
        public static void WriteQuickCSV(string file , double [] arrayToWrite,string columnName, CSVFormat format)
        {
            StringBuilder res = new StringBuilder();
            NumberList.ToList(format, res, arrayToWrite);
            System.IO.StreamWriter wrt = new StreamWriter(file,false);
            wrt.WriteLine(columnName);
            //foreach (double d in arrayToWrite)
            //{
            //    wrt.WriteLine(d);
            //}
            //wrt.Close();

            wrt.Write(res);
        }

        /// <summary>
        /// Writes an array of double to file using a CSVFormat.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="arrayToWrite">The array to write.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="Appender">The appender.</param>
        public static void WriteQuickCSV(string file, int[] arrayToWrite, string columnName, CSVFormat format)
        {
            //We make a string builder.
            StringBuilder res = new StringBuilder();
            //put the array of int in the stringbuilder using the csvformat.
            NumberList.ToListInt(format, res, arrayToWrite);
            //make a streamwriter.
            System.IO.StreamWriter wrt = new StreamWriter(file, false);
            //write the column heading.
            wrt.WriteLine(columnName);
            //write the csv.
            wrt.Write(res);
            //release the csv.
            wrt.Close();
        }



       

        /// <summary>
        /// Processes and normalize a double serie.
        /// </summary>
        /// <param name="data">The double serie to normalize.</param>
        /// <returns></returns>
        public static double [] ProcessAndNormalizeDoubles(double [] data)
        {
            double[] returnedArray = NormalizeThisArray(data);
            return returnedArray;
        }

        /// <summary>
        /// Normalizes this array.
        /// </summary>
        /// <param name="inputArray">The input array.</param>
        /// <returns></returns>
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
        /// Loads a normalization file.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static DataNormalization LoadNormalization(string directory , string file)
        {
            DataNormalization norm = null;
            FileInfo networkFile = FileUtil.CombinePath(new FileInfo(directory), file);
            if (networkFile.Exists)
            {
                norm = (DataNormalization) SerializeObject.Load(networkFile.FullName);
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
    }
}
