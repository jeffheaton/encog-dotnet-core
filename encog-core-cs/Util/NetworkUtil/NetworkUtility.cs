using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Encog.Engine.Network.Activation;
using Encog.MathUtil;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Temporal;
using Encog.ML.SVM;
using Encog.Neural.Data.Basic;
using Encog.Neural.NEAT;
using Encog.Neural.NEAT.Training;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
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
using Convert = System.Convert;

namespace Encog.Util.NetworkUtil
{
    /// <summary>
    /// A class with many helpers.
    /// </summary>
    public partial class NetworkUtility
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
        public static void SaveMatrix(double[][] surface, string fileName)
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
        public static double[][] LoadMatrix(string fileName)
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

        /// <summary>
        /// Calculates the ranges in two double series.
        /// Can be used to make inputs for a neural network.
        /// </summary>
        /// <param name="closingValues">The closing values.</param>
        /// <param name="OpeningValues">The opening values.</param>
        /// <returns></returns>
        public static double[] CalculateRanges(double[] closingValues, double[] OpeningValues)
        {
            List<double> results = new List<double>();
            //if one is bigger than the other, something is wrong.
            if (closingValues.Length != OpeningValues.Length)
                return null;

            for (int i = 0; i < closingValues.Length; i++)
            {
                double range = Math.Abs(closingValues[i] - OpeningValues[i]);
                results.Add(range);
                i++;
            }
            return results.ToArray();

        }
        /// <summary>
        /// Calculates the percent changes in a double serie.
        /// Emulates how the temporal datasets are normalizing.
        /// useful if you are using a temporal dataset and you want to pass inputs to your neural network after it has been trained.
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
        public static BasicNetwork SaveNetwork(string directory, string file, BasicNetwork anetwork)
        {
            FileInfo networkFile = FileUtil.CombinePath(new FileInfo(directory), file);
            EncogDirectoryPersistence.SaveObject(networkFile, anetwork);
            return anetwork;
        }

        /// <summary>
        /// Saves the network to the specified directory with the specified parameter name.
        /// This version saves super machine to file.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="file">The file.</param>
        /// <param name="anetwork">The network to save..</param>
        public static SupportVectorMachine SaveNetwork(string directory, string file, SupportVectorMachine anetwork)
        {
            FileInfo networkFile = FileUtil.CombinePath(new FileInfo(directory), file);
            EncogDirectoryPersistence.SaveObject(networkFile, anetwork);
            return anetwork;
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
        /// a quick and easy way to test data and and train networks.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        public static double[] MakeRandomInputs(int number)
        {
            Random rdn = new Random();
            MathUtil.Randomize.RangeRandomizer encogRnd = new Encog.MathUtil.Randomize.RangeRandomizer(-1, 1);
            double[] x = new double[number];
            for (int i = 0; i < number; i++)
            {
                x[i] = encogRnd.Randomize((rdn.NextDouble()));

            }
            return x;
        }


        /// <summary>
        /// Determines the angle on IMLData.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns></returns>
        public static double DetermineAngle(IMLData angle)
        {
            double result = (Math.Atan2(angle[0], angle[1]) / Math.PI) * 180;
            if (result < 0)
                result += 360;

            return result;
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
        public static double[] ReturnArrayOnSize(double[] inputted, int inputsize)
        {
            //lets say we receive an array of 105 doubles ...input size is 100.
            //we need to just copy the last 100.
            //so if inputted.Lenght > inputsize :
            // start index = inputtedLenght - inputsize.

            //if inputtedlenght is equal to input size , well our index will be 0..
            //if inputted lenght is smaller than input...We return  null.
            double[] arr = new double[inputsize];
            int howBig = 0;
            if (inputted.Length >= inputsize)
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
            double[] res = MakeRandomInputs(sizeofInputs);
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
        /// Takes 2 inputs arrays and makes a jagged array.
        /// this is useful for creating neural network inputs.
        /// </summary>
        /// <param name="firstArray">The first array.</param>
        /// <param name="SecondArray">The second array.</param>
        /// <returns></returns>
        public static double[][] FromDualToJagged(double []firstArray, double []SecondArray)
        {
            return  new[] {firstArray, SecondArray};

        }

      


        /// <summary>
        /// Builds a jagged array ready for network input.
        /// The first are the input , the second is the ideal.
        /// This makes an input (IMLDataPair for a BasicMLDataset).
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns></returns>
        public static double [][] NetworkbuilArray(double [] first , double [] second)
        {
            double[][] Resulting = new double[2][];
            Resulting[0] = first;
            Resulting[1] = second;
            return Resulting;
        }


        public static double[][] NetworkbuilArray(double[] first)
        {
            double[][] Resulting = new double[first.Length][];
            Resulting[0] = first;
            return Resulting;
        }

        /// <summary>
        /// Processes a double array of data of input and a second array of data for ideals
        /// you must input the input and output size.
        /// this typically builds a supervised IMLDatapair, which you must add to a IMLDataset. 
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="ideal">The ideal.</param>
        /// <param name="_inputWindow">The _input window.</param>
        /// <param name="_predictWindow">The _predict window.</param>
        /// <returns></returns>
        public static IMLDataPair ProcessPairs(double[] data, double[] ideal, int _inputWindow, int _predictWindow)
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
        /// Prints the content of an array to the console.
        /// </summary>
        /// <param name="num">The num.</param>
        public static void  PrinteJaggedArray(int[][] num)
        {
            foreach (int[] t in num)
            {
                foreach (int t1 in t)
                {
                    Console.WriteLine(@"Values : {0}", t1);//displaying values of Jagged Array
                }
            }
        }

        /// <summary>
        /// Networkbuils the array by params.
        /// </summary>
        /// <param name="Inputs">The inputs.</param>
        /// <returns></returns>
        public static double[][] NetworkbuilArrayByParams(params double [][] Inputs)
        {
            double[][] Resulting = new double[Inputs.Length][];

            int i = Inputs.Length;
            foreach (double[] doubles in Resulting)
            {
                for (int k = 0; k < Inputs.Length;k++ )
                    Resulting[k] = doubles;
            }
            return Resulting;
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
        /// Builds and trains a neat network.
        /// </summary>
        /// <param name="aset">The IMLDataset.</param>
        /// <param name="inputcounts">The inputcounts.</param>
        /// <param name="outputcounts">The outputcounts.</param>
        /// <param name="populationsize">The populationsize.</param>
        /// <param name="ToErrorTraining">To error rate you want to train too.</param>
        /// <returns>a trained netnetwork.</returns>
        public static NEATNetwork BuildTrainNeatNetwork(IMLDataSet aset, int inputcounts, int outputcounts, int populationsize, double ToErrorTraining)
        {
            NEATPopulation pop = new NEATPopulation(inputcounts, outputcounts, populationsize);
            ICalculateScore score = new TrainingSetScore(aset);
            // train the neural network
            ActivationStep step = new ActivationStep();
            step.Center = 0.5;
            pop.OutputActivationFunction = step;
            NEATTraining train = new NEATTraining(score, pop);
            EncogUtility.TrainToError(train, ToErrorTraining);
            NEATNetwork network = (NEATNetwork)train.Method;
            return network;
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
        /// Generates a jagged array with as many inputs as needed one one ideal.
        /// This is useful for neural networks with multiple inputs.
        /// </summary>
        /// <param name="ideal">The ideal.</param>
        /// <param name="inputs">The inputs.</param>
        /// <returns>the jagged array used as input for the neural network.</returns>
        public static double[][] GenerateInputz(params double[][] inputs)
        {
            ArrayList al = new ArrayList();
            foreach (double[] doublear in inputs)
            {
                al.Add((double[])doublear);
            }
            return (double[][])al.ToArray(typeof(double[]));
        }



        /// <summary>
        /// Generates from an input and an ideal double array a jagged array.
        /// Jagged array are used in supervised learning.
        /// both ideal and array must have the same lenght.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="ideal">The ideal.</param>
        /// <returns>the jagged array.</returns>
        public static double[][] Generate(double []input, double[] ideal)
        {
            double[][] result = EngineArray.AllocateDouble2D(2, input.Length);
            int first = 0;
            foreach (double d in input)
            {
                result[0][first++] = d;
            }
            first = 0;
            foreach (double d in ideal)
            {
                result[1][first++] = d;
            }
            return result;
        }

        /// <summary>
        /// Generates from an input and an ideal double array a jagged array.
        /// Jagged array are used in supervised learning.
        /// both ideal and array must have the same lenght.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="ideal">The ideal.</param>
        /// <returns>the jagged array.</returns>
        public static double[][] GenerateSimpleJagged(double[] input)
        {
            double[][] result = EngineArray.AllocateDouble2D(1, input.Length);
            int first = 0;
            foreach (double d in input)
            {
                result[0][first++] = d;
            }
            return result;
        }


        /// <summary>
        /// Doubles the inputs to array.
        /// Lets say you have 600 numbers, but each of those number actually represent a serie
        /// in which there is 2 inputs, so you actually have 300 inputs with 2 numbers.
        /// This makes a jagged array ready for neural networks and arranges each double in its proper double array.
        /// Lets say you are receiving prices from multiple instruments and want to put them in a neural network..This would be useful.
        /// Could also be useful if you are reading a csv, and need to make each cell in a row an input....
        /// You are reading the keno.csv , there are 10 Balls, but 15000 lines of drawings..
        /// With this method , you can just pass the 15000 lines, and give a lenght of 10..The rest is done automatically.
        /// </summary>
        /// <param name="inputs">The inputs.</param>
        /// <param name="lenght">The lenght.</param>
        /// <returns>the jagged array of doubles</returns>
        public static double [][]DoubleInputsToArray(List<double> inputs, int lenght)
        {
            //we must be able to fit all our numbers in the same double array..no spill over.
           if (inputs.Count % lenght != 0)
                return null;
           int dimension = inputs.Count / lenght;
            int total = inputs.Count;
            int currentindex = 0;
            double[][] result = EngineArray.AllocateDouble2D(dimension, lenght);
           
                    //now we loop through the index.
                    int index = 0;
                    for (index =0; index <result.Length;index++)
                    {
                        for(int j =0;j< lenght;j++)
                        {
                            result[index][j] = inputs[currentindex++];
                        }
                    }
            return (result);
        }


        public static double[][] DoubleInputsToArraySimple(List<double> inputs, int lenght)
        {
            //we must be able to fit all our numbers in the same double array..no spill over.
            if (inputs.Count % lenght != 0)
                return null;
            int dimension = inputs.Count / lenght;

            double[][] result = EngineArray.AllocateDouble2D(dimension, lenght);
            foreach (double doubles in inputs)
            {
                for (int index = 0; index < result.Length; index++)
                {
                    for (int j = 0; j < lenght; j++)
                    {
                        result[index][j] = doubles;
                    }
                }

            }
            return (result);
        }
        /// <summary>
        /// Makes the data set with a jagged input array and a jagged ideals array.
        /// </summary>
        /// <param name="inputs">The inputs.</param>
        /// <param name="ideals">The ideals.</param>
        /// <returns></returns>
        public static IMLDataSet MakeDataSet (double [][] inputs , double [][] ideals)
        {
            IMLDataSet theset = new BasicMLDataSet(inputs, ideals);

            return theset;

        }
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
        /// Normalizes an array with a given max low and max high.
        /// </summary>
        /// <param name="inputArray">The input array.</param>
        /// <param name="low">The low for the normalized resulting arrray..</param>
        /// <param name="high">The high for the normalized resulting array..</param>
        /// <returns>
        /// a normalized array of doubles
        /// </returns>
        public static double[] NormalizeThisArray(double[] inputArray,int low,int high)
        {
            return ArrayNormalizer.Process(inputArray,low,high);
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
        /// Returns the difference between two doubles in percents.
        ///  </summary>
        /// <param name="first">The first double.</param>
        /// <param name="second">The second double.</param>
        /// <returns>return the absolute percentage difference between two numbers.</returns>
        public static double AveragePercents (double first,double second)
        {
            //double diffs = (first - second);
            //double result = (first + second)/2;
            //return 1-((result/diffs)*0.01);

           return  Math.Abs(first - second) / (first + second) * 100;

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

        /// <summary>
        /// Creates the feedforward network.
        /// </summary>
        /// <param name="inputsize">The inputsize.</param>
        /// <param name="outputsize">The outputsize.</param>
        /// <param name="hiddenlayers">The hiddenlayers.</param>
        /// <param name="hidden2layers">The hidden2layers.</param>
        /// <returns></returns>
        public static BasicNetwork CreateFeedforwardNetwork(int inputsize, int outputsize, int hiddenlayers, int hidden2layers)
        {
            // construct an Elman type network
            FeedForwardPattern pattern = new FeedForwardPattern();
            pattern.ActivationFunction = new ActivationTANH();
            pattern.InputNeurons = inputsize;
            pattern.AddHiddenLayer(hiddenlayers);
            pattern.AddHiddenLayer(hidden2layers);
            pattern.OutputNeurons = outputsize;
            var network = pattern.Generate();
            return (BasicNetwork)network;
        }


    }
}
