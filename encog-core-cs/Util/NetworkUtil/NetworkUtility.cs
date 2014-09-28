//
// Encog(tm) Core v3.3 - .Net Version
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
using System.Collections.Generic;
using System.IO;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Temporal;
using Encog.ML.SVM;
using Encog.MathUtil.Randomize;
using Encog.Neural.Networks;
using Encog.Neural.Pattern;
using Encog.Persist;
using Encog.Util.Arrayutil;
using Encog.Util.File;
using Encog.Util.Normalize;
using Encog.Util.Simple;

namespace Encog.Util.NetworkUtil
{
    /// <summary>
    ///     A class with many helpers.
    /// </summary>
    public class NetworkUtility
    {
        /// <summary>
        ///     Normalizes arrays.
        ///     You can retrieve the array normalizer stats throughout the program's life without having to instantiate a new normalizer.
        /// </summary>
        public static readonly NormalizeArray ArrayNormalizer = new NormalizeArray();

        /// <summary>
        ///     Saves the matrix to the desired file location.
        /// </summary>
        /// <param name="surface">The surface.</param>
        /// <param name="fileName">Name of the file.</param>
        public static void SaveMatrix(double[][] surface, string fileName)
        {
            using (var sw = new StreamWriter(fileName, false))
            {
                foreach (var t in surface)
                {
                    foreach (double t1 in t)
                    {
                        if (double.IsNaN(t1))
                            sw.Write(",");
                        else
                            sw.Write(t1 + ",");
                    }
                    sw.WriteLine();
                }
            }
        }


        /// <summary>
        ///     Loads a matrix from a file to a double[][] array.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static double[][] LoadMatrix(string fileName)
        {
            string[] allLines = System.IO.File.ReadAllLines(fileName);

            var matrix = new double[allLines.Length][];

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
        ///     Calculates the percents change from one number to the next.
        ///     First input the current number , then input the previous value.
        /// </summary>
        public static double CalculatePercents(double currentValue, double previousValue)
        {
            if (currentValue > 0.0 && previousValue > 0.0)
            {
                return (currentValue - previousValue)/previousValue;
            }
            return 0.0;
        }

        /// <summary>
        ///     Calculates the ranges in two double series.
        ///     Can be used to make inputs for a neural network.
        /// </summary>
        /// <param name="closingValues">The closing values.</param>
        /// <param name="openingValues">The opening values.</param>
        /// <returns></returns>
        public static double[] CalculateRanges(double[] closingValues, double[] openingValues)
        {
            var results = new List<double>();
            //if one is bigger than the other, something is wrong.
            if (closingValues.Length != openingValues.Length)
                return null;

            for (int i = 0; i < closingValues.Length; i++)
            {
                double range = Math.Abs(closingValues[i] - openingValues[i]);
                results.Add(range);
                i++;
            }
            return results.ToArray();
        }

        /// <summary>
        ///     Calculates the percent changes in a double serie.
        ///     Emulates how the temporal datasets are normalizing.
        ///     useful if you are using a temporal dataset and you want to pass inputs to your neural network after it has been trained.
        /// </summary>
        /// <param name="inputs">The inputs.</param>
        /// <returns></returns>
        public static double[] CalculatePercents(double[] inputs)
        {
            int index = 0;
            var result = new List<double>();
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
        ///     Loads an IMLDataset training file in a given directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static IMLDataSet LoadTraining(string directory, string file)
        {
            FileInfo networkFile = FileUtil.CombinePath(new FileInfo(@directory), @file);
            if (!networkFile.Exists)
            {
                return null;
            }
            IMLDataSet network = EncogUtility.LoadEGB2Memory(networkFile);
            return network;
        }


        /// <summary>
        ///     Loads an IMLDataset training file in a given directory.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>an imldataset ready to use.</returns>
        public static IMLDataSet LoadTraining(string file)
        {
            var networkFile = new FileInfo(@file);
            if (!networkFile.Exists)
            {
                return null;
            }
            IMLDataSet network = EncogUtility.LoadEGB2Memory(networkFile);
            return network;
        }


        /// <summary>
        ///     Saves the network to the specified directory with the specified parameter name.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="file">The file.</param>
        /// <param name="anetwork">The network to save..</param>
        public static BasicNetwork SaveNetwork(string directory, string file, BasicNetwork anetwork)
        {
            FileInfo networkFile = FileUtil.CombinePath(new FileInfo(directory), @file);
            EncogDirectoryPersistence.SaveObject(networkFile, anetwork);
            return anetwork;
        }

        /// <summary>
        ///     Saves the network to the specified directory with the specified parameter name.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="anetwork">The network to save..</param>
        public static BasicNetwork SaveNetwork(string file, BasicNetwork anetwork)
        {
            var networkFile = new FileInfo(@file);
            EncogDirectoryPersistence.SaveObject(networkFile, anetwork);
            return anetwork;
        }

        /// <summary>
        ///     Saves the network to the specified directory with the specified parameter name.
        ///     This version saves super machine to file.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="file">The file.</param>
        /// <param name="anetwork">The network to save..</param>
        public static SupportVectorMachine SaveNetwork(string directory, string file, SupportVectorMachine anetwork)
        {
            FileInfo networkFile = FileUtil.CombinePath(new FileInfo(directory), @file);
            EncogDirectoryPersistence.SaveObject(networkFile, anetwork);
            return anetwork;
        }

        /// <summary>
        ///     Saves the network to the specified directory with the specified parameter name.
        ///     This version saves super machine to file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="anetwork">The network to save..</param>
        public static SupportVectorMachine SaveNetwork(string file, SupportVectorMachine anetwork)
        {
            var networkFile = new FileInfo(@file);
            EncogDirectoryPersistence.SaveObject(networkFile, anetwork);
            return anetwork;
        }

        /// <summary>
        ///     Loads an basic network from the specified directory and file.
        ///     You must load the network like this Loadnetwork(directory,file);
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static BasicNetwork LoadNetwork(string directory, string file)
        {
            FileInfo networkFile = FileUtil.CombinePath(new FileInfo(@directory), @file);
            // network file
            if (!networkFile.Exists)
            {
                Console.WriteLine(@"Can't read file: " + networkFile);
                return null;
            }
            var network = (BasicNetwork) EncogDirectoryPersistence.LoadObject(networkFile);
            return network;
        }

        /// <summary>
        ///     Loads an basic network from the specified directory and file.
        ///     You must load the network like this Loadnetwork(file);
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static BasicNetwork LoadNetwork(string file)
        {
            var networkFile = new FileInfo(@file);
            // network file
            if (!networkFile.Exists)
            {
                Console.WriteLine(@"Can't read file: " + networkFile);
                return null;
            }
            var network = (BasicNetwork) EncogDirectoryPersistence.LoadObject(networkFile);
            return network;
        }

        /// <summary>
        ///     Saves an IMLDataset to a file.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="file">The file.</param>
        /// <param name="trainintoSave">The traininto save.</param>
        public static void SaveTraining(string directory, string file, IMLDataSet trainintoSave)
        {
            FileInfo networkFile = FileUtil.CombinePath(new FileInfo(@directory), @file);
            //save our training file.
            EncogUtility.SaveEGB(networkFile, trainintoSave);
        }

        /// <summary>
        ///     Saves an IMLDataset to a file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="trainintoSave">The traininto save.</param>
        public static void SaveTraining(string file, IMLDataSet trainintoSave)
        {
            var networkFile = new FileInfo(@file);
            //save our training file.
            EncogUtility.SaveEGB(networkFile, trainintoSave);
        }

        /// <summary>
        ///     Generates a temporal data set with a given double serie.
        ///     uses Type percent change.
        /// </summary>
        /// <param name="inputserie">The inputserie.</param>
        /// <param name="windowsize">The windowsize.</param>
        /// <param name="predictsize">The predictsize.</param>
        /// <returns></returns>
        public static TemporalMLDataSet GenerateTrainingWithPercentChangeOnSerie(double[] inputserie, int windowsize,
                                                                                 int predictsize)
        {
            var result = new TemporalMLDataSet(windowsize, predictsize);
            var desc = new TemporalDataDescription(TemporalDataDescription.Type.PercentChange, true, true);
            result.AddDescription(desc);
            for (int index = 0; index < inputserie.Length - 1; index++)
            {
                var point = new TemporalPoint(1) {Sequence = index};
                point.Data[0] = inputserie[index];
                result.Points.Add(point);
            }
            result.Generate();
            return result;
        }

        /// <summary>
        ///     Makes random inputs by randomizing with encog randomize , the normal random from net framework library.
        ///     a quick and easy way to test data and and train networks.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        public static double[] MakeRandomInputs(int number)
        {
            var rdn = new Random();
            var encogRnd = new RangeRandomizer(-1, 1);
            var x = new double[number];
            for (int i = 0; i < number; i++)
            {
                x[i] = encogRnd.Randomize((rdn.NextDouble()));
            }
            return x;
        }


        /// <summary>
        ///     Determines the angle on IMLData.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns></returns>
        public static double DetermineAngle(IMLData angle)
        {
            double result = (Math.Atan2(angle[0], angle[1])/Math.PI)*180;
            if (result < 0)
                result += 360;

            return result;
        }

        /// <summary>
        ///     Normalizes an array , and you can retrieve the normalizedArray by ArrayNormalizer object), this lets you do ArrayNormalizer.Stats.Denormalize(2);
        /// </summary>
        /// <param name="inputArray">The input array.</param>
        /// <returns>a normalized array of doubles</returns>
        public static double[] NormalizeThisArray(double[] inputArray)
        {
            return ArrayNormalizer.Process(inputArray);
        }

        /// <summary>
        ///     Normalizes an array with a given max low and max high.
        /// </summary>
        /// <param name="inputArray">The input array.</param>
        /// <param name="low">The low for the normalized resulting arrray..</param>
        /// <param name="high">The high for the normalized resulting array..</param>
        /// <returns>
        ///     a normalized array of doubles
        /// </returns>
        public static double[] NormalizeThisArray(double[] inputArray, int low, int high)
        {
            return ArrayNormalizer.Process(inputArray, low, high);
        }


        /// <summary>
        ///     Normalizes an array using Normalize Array (and not DataNormalization way : Faster).
        /// </summary>
        /// <param name="lo">The lo.</param>
        /// <param name="hi">The hi.</param>
        /// <param name="arrays">The arrays.</param>
        /// <returns></returns>
        public static double[] NormalizeArray(double lo, double hi, double[] arrays)
        {
            var norm = new NormalizeArray {NormalizedHigh = hi, NormalizedLow = lo};
            return norm.Process(arrays);
        }


        /// <summary>
        ///     Normalizes an array using Normalize Array (and not DataNormalization way : Faster).
        ///     The high and low are the standard -1,1.
        /// </summary>
        /// <param name="arrays">The arrays.</param>
        /// <returns>returns a tuple with the array in item1 and the normalization in item 2.</returns>
        public static Tuple<double[], NormalizeArray> NormalizeArray(double[] arrays)
        {
            var norm = new NormalizeArray();
            return new Tuple<double[], NormalizeArray>(norm.Process(arrays), norm);
        }

        /// <summary>
        ///     Denormalizes the double.
        /// </summary>
        /// <param name="doubleToDenormalize">The double to denormalize.</param>
        /// <returns></returns>
        public static double DenormalizeDouble(double doubleToDenormalize)
        {
            return ArrayNormalizer.Stats.DeNormalize(doubleToDenormalize);
        }

        /// <summary>
        ///     Loads a normalization from the specified directory and file.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="file">The file.</param>
        /// <returns>a datanormalization object</returns>
        public static DataNormalization LoadNormalization(string directory, string file)
        {
            DataNormalization norm = null;
            FileInfo networkFile = FileUtil.CombinePath(new FileInfo(@directory), @file);
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
        ///     Returns the difference between two doubles in percents.
        /// </summary>
        /// <param name="first">The first double.</param>
        /// <param name="second">The second double.</param>
        /// <returns>return the absolute percentage difference between two numbers.</returns>
        public static double AveragePercents(double first, double second)
        {
            //double diffs = (first - second);
            //double result = (first + second)/2;
            //return 1-((result/diffs)*0.01);

            return Math.Abs(first - second)/(first + second)*100;
        }


        /// <summary>
        ///     Saves a normalization to the specified folder with the specified name.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="file">The file.</param>
        /// <param name="normTosave">The norm tosave.</param>
        public static void SaveNormalization(string directory, string file, DataNormalization normTosave)
        {
            SerializeObject.Save(directory + file, normTosave);
        }


        /// <summary>
        ///     Creates the elman network as a basicnetwork.
        ///     This method create a elmhan patterns but returns a basic network (ready for load/save).
        /// </summary>
        /// <param name="inputsize">The inputsize.</param>
        /// <param name="outputsize">The outputsize.</param>
        /// <param name="hiddenlayers">The hiddenlayers.</param>
        /// <param name="activate">The activation function to use..</param>
        /// <returns>a basic network</returns>
        public static BasicNetwork CreateElmanNetwork(int inputsize, int outputsize, int hiddenlayers,
                                                      IActivationFunction activate)
        {
            // construct an Elman type network
            var pattern = new ElmanPattern {ActivationFunction = activate, InputNeurons = inputsize};
            pattern.AddHiddenLayer(hiddenlayers);
            pattern.OutputNeurons = outputsize;
            return (BasicNetwork) pattern.Generate();
        }

        /// <summary>
        ///     Creates the feedforward network.
        /// </summary>
        /// <param name="inputsize">The inputsize.</param>
        /// <param name="outputsize">The outputsize.</param>
        /// <param name="hiddenlayers">The hiddenlayers.</param>
        /// <param name="hidden2Layers">The hidden2layers.</param>
        /// <returns></returns>
        public static BasicNetwork CreateFeedforwardNetwork(int inputsize, int outputsize, int hiddenlayers,
                                                            int hidden2Layers)
        {
            // construct an Elman type network
            var pattern = new FeedForwardPattern {ActivationFunction = new ActivationTANH(), InputNeurons = inputsize};
            pattern.AddHiddenLayer(hiddenlayers);
            pattern.AddHiddenLayer(hidden2Layers);
            pattern.OutputNeurons = outputsize;
            IMLMethod network = pattern.Generate();
            return (BasicNetwork) network;
        }
    }
}
