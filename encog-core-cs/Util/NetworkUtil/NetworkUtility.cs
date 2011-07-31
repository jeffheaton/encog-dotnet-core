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
using Encog.Persist;
using Encog.Util.Arrayutil;
using Encog.Util.File;
using Encog.Util.Normalize;
using Encog.Util.Normalize.Input;
using Encog.Util.Normalize.Output;
using Encog.Util.Normalize.Target;
using Encog.Util.Simple;

namespace Encog.Util.NetworkUtil
{
    public class NetworkUtility
    {


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
