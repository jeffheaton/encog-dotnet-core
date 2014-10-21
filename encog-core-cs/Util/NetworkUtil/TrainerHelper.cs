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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Engine.Network.Activation;
using Encog.MathUtil;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Temporal;
using Encog.Util.Arrayutil;

namespace Encog.Util.NetworkUtil
{
    /// <summary>
    /// Use this helper class to build training inputs for neural networks (only memory based datasets).
    /// Mostly this class is used in financial neural networks when receiving multiple inputs (indicators, prices, etc) and 
    /// having to put them in neural datasets.
    /// 
    /// </summary>
    public static class TrainerHelper
    {

        /// <summary>
        /// Makes the double [][] from single array.
        /// this is a very important method used in financial markets when you have multiple inputs (Close price, 1 indicator, 1 ATR, 1 moving average for example) , and each is already formated in an array of doubles.
        /// You just provide the number of inputs (4 here) , and it will create the resulting double [][]
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="numberofinputs">The numberofinputs.</param>
        /// <returns></returns>
        public static double [][] MakeDoubleJaggedInputFromArray(double [] array , int numberofinputs)
        {
            //we must be able to fit all our numbers in the same double array..no spill over.
            if (array.Length % numberofinputs != 0)
                return null;
            int dimension = array.Length / numberofinputs;
            int currentindex = 0;
            double[][] result = EngineArray.AllocateDouble2D(dimension, numberofinputs);

            //now we loop through the index.
            int index = 0;
            for (index = 0; index < result.Length; index++)
            {
                for (int j = 0; j < numberofinputs; j++)
                {
                    result[index][j] = array[currentindex++];
                }
            }
            return (result);
        }



        /// <summary>
        /// Doubles the List of doubles into a jagged array.
        /// This is exactly similar as the MakeDoubleJaggedInputFromArray just it takes a List of double as parameter.
        /// It quite easier to Add doubles to a list than into a double [] Array (so this method is used more).
        /// </summary>
        /// <param name="inputs">The inputs.</param>
        /// <param name="lenght">The lenght.</param>
        /// <returns></returns>
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
        /// Processes the specified double serie into an IMLDataset.
        /// To use this method, you must provide a formated double array.
        /// The number of points in the input window makes the input array , and the predict window will create the array used in ideal.
        /// Example you have an array with 1, 2, 3 , 4 , 5.
        /// You can use this method to make an IMLDataset 4 inputs and 1 ideal (5).
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="_inputWindow">The _input window.</param>
        /// <param name="_predictWindow">The _predict window.</param>
        /// <returns></returns>
        public static IMLDataSet ProcessDoubleSerieIntoIMLDataset(double[] data, int _inputWindow, int _predictWindow)
        {
            var result = new BasicMLDataSet();
            int totalWindowSize = _inputWindow + _predictWindow;
            int stopPoint = data.Length - totalWindowSize;
            for (int i = 0; i < stopPoint; i++)
            {
                var inputData = new BasicMLData(_inputWindow);
                var idealData = new BasicMLData(_predictWindow);
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
                var pair = new BasicMLDataPair(inputData, idealData);
                result.Add(pair);
            }

            return result;
        }



        /// <summary>
        /// Processes the specified double serie into an IMLDataset.
        /// To use this method, you must provide a formated double array with the input data and the ideal data in another double array.
        /// The number of points in the input window makes the input array , and the predict window will create the array used in ideal.
        /// This method will use ALL the data inputs and ideals you have provided.
        /// </summary>
        /// <param name="datainput">The datainput.</param>
        /// <param name="ideals">The ideals.</param>
        /// <param name="_inputWindow">The _input window.</param>
        /// <param name="_predictWindow">The _predict window.</param>
        /// <returns></returns>
        public static IMLDataSet ProcessDoubleSerieIntoIMLDataset(List<double> datainput,List<double>ideals, int _inputWindow, int _predictWindow)
        {
            var result = new BasicMLDataSet();
            //int count = 0;
            ////lets check if there is a modulo , if so we move forward in the List of doubles in inputs.This is just a check
            ////as the data of inputs should be able to fill without having .
            //while (datainput.Count % _inputWindow !=0)
            //{
            //    count++;
            //}
            var inputData = new BasicMLData(_inputWindow);
            var idealData = new BasicMLData(_predictWindow);
            foreach (double d in datainput)
            {
                // handle input window
                for (int j = 0; j < _inputWindow; j++)
                {
                    inputData[j] = d;
                }
            }
            foreach (double ideal in ideals)
            {
                 // handle predict window
                for (int j = 0; j < _predictWindow; j++)
                {
                    idealData[j] =ideal;
                }
            }
            var pair = new BasicMLDataPair(inputData, idealData);
            result.Add(pair);
            return result;
        }

    

    static bool IsNoModulo(double first,int second)
    {
        return first % second == 0;
    }

    /// <summary>
    /// Grabs every Predict point in a double serie.
    /// This is useful if you have a double series and you need to grab every 5 points for examples and make an ourput serie (like in elmhan networks).
    /// E.G , 1, 2, 1, 2,5 ...and you just need the 5..
    /// </summary>
    /// <param name="inputs">The inputs.</param>
    /// <param name="PredictSize">Size of the predict.</param>
    /// <returns></returns>
        public static List<double> CreateIdealFromSerie(List<double> inputs, int PredictSize)
        {
            //we need to copy into a new list only the doubles on each point of predict..

            List<int> Indexes = new List<int>();

            for (int i =0 ; i <  inputs.Count;i++)
            {
                if (i % PredictSize == 0)
                    Indexes.Add(i);
            }
            List<double> Results = Indexes.Select(index => inputs[index]).ToList();
            return Results;
        }
        /// <summary>
        /// Generates the Temporal MLDataset with a given data array.
        /// You must input the "predict" size (inputs) and the windowsize (outputs).
        /// This is oftenly used with Ehlman networks.
        /// The temporal dataset will be in RAW format (no normalization used..Most of the times it means you already have normalized your inputs /ouputs.
        /// 
        /// </summary>
        /// <param name="inputserie">The inputserie.</param>
        /// <param name="windowsize">The windowsize.</param>
        /// <param name="predictsize">The predictsize.</param>
        /// <returns>A temporalMLDataset</returns>
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
        /// This method takes ARRAYS of arrays (parametrable arrays) and places them in double [][]
        /// You can use this method if you have already formatted arrays and you want to create a double [][] ready for network.
        /// Example you could use this method to input the XOR example:
        /// A[0,0]   B[0,1]  C[1, 0]  D[1,1] would format them directly in the double [][] in one method call.
        /// This could also be used in unsupervised learning.
        /// </summary>
        /// <param name="Inputs">The inputs.</param>
        /// <returns></returns>
        public static double[][] NetworkbuilArrayByParams(params double[][] Inputs)
        {
            double[][] Resulting = new double[Inputs.Length][];

            int i = Inputs.Length;
            foreach (double[] doubles in Resulting)
            {
                for (int k = 0; k < Inputs.Length; k++)
                    Resulting[k] = doubles;
            }
            return Resulting;
        }




        /// <summary>
        /// Prints the content of an array to the console.(Mostly used in debugging).
        /// </summary>
        /// <param name="num">The num.</param>
        public static void PrinteJaggedArray(int[][] num)
        {
            foreach (int t1 in num.SelectMany(t => t))
            {
                Console.WriteLine(@"Values : {0}", t1);//displaying values of Jagged Array
            }
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
            var result = new BasicMLDataSet();
            for (int i = 0; i < data.Length; i++)
            {
                var inputData = new BasicMLData(_inputWindow);
                var idealData = new BasicMLData(_predictWindow);
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
        /// Takes 2 inputs arrays and makes a jagged array.
        /// this just copies the second array after the first array in a double [][]
        /// </summary>
        /// <param name="firstArray">The first array.</param>
        /// <param name="SecondArray">The second array.</param>
        /// <returns></returns>
        public static double[][] FromDualToJagged(double[] firstArray, double[] SecondArray)
        {
            return new[] { firstArray, SecondArray };

        }


        ///// <summary>
        ///// Generates the Temporal Training based on an array of doubles.
        ///// You need to have 2 array of doubles [] for this method to work!
        ///// </summary>
        ///// <param name="inputsize">The inputsize.</param>
        ///// <param name="outputsize">The outputsize.</param>
        ///// <param name="Arraydouble">The arraydouble.</param>
        ///// <returns></returns>
        //public static TemporalMLDataSet GenerateTraining(int inputsize, int outputsize, params double[][] Arraydouble)
        //{
        //    if (Arraydouble.Length < 2)
        //        return null;
        //    if (Arraydouble.Length > 2)
        //        return null;
        //    TemporalMLDataSet result = new TemporalMLDataSet(inputsize, outputsize);
        //    TemporalDataDescription desc = new TemporalDataDescription(new ActivationTANH(), TemporalDataDescription.Type.PercentChange, true, true);
        //    result.AddDescription(desc);
        //    TemporalPoint point = new TemporalPoint(Arraydouble.Length);
        //    int currentindex;


        //    for (int w = 0; w < Arraydouble[0].Length; w++)
        //    {
        //        //We have filled in one dimension now lets put them in our temporal dataset.
        //        for (int year = 0; year < Arraydouble.Length - 1; year++)
        //        {
        //            //We have as many points as we passed the array of doubles.
        //            point = new TemporalPoint(Arraydouble.Length);
        //            //our first sequence (0).
        //            point.Sequence = w;
        //            //point 0 is double[0] array.
        //            point.Data[0] = Arraydouble[0][w];
        //            point.Data[1] = Arraydouble[1][w++];
        //            //we add the point..
        //        }
        //        result.Points.Add(point);
        //    }
        //    result.Generate();
        //    return result;
        //}



        /// <summary>
        /// Calculates and returns the copied array.
        /// This is used in live data , when you want have a full array of doubles but you want to cut from a starting position
        /// and return only from that point to the end.
        /// example you have 1000 doubles , but you want only the last 100.
        /// input size is the input you must set to 100.
        /// I use this method next to every day when calculating on an array of doubles which has just received a new price (A quote for example).
        /// As the array of quotes as all the quotes since a few days, i just need the last 100 for example , so this method is used when not training but using the neural network.
        /// </summary>
        /// <param name="inputted">The inputted.</param>
        /// <param name="inputsize">The input neuron size (window size).</param>
        /// <returns></returns>
        public static double[] ReturnArrayOnSize(double[] inputted, int inputsize)
        {
            //lets say we receive an array of 105 doubles ...input size is 100.(or window size)
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
        /// Quickly an IMLDataset from a double array using the TemporalWindow array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="inputsize">The inputsize.</param>
        /// <param name="outputsize">The outputsize.</param>
        /// <returns></returns>
        public static IMLDataSet QuickTrainingFromDoubleArray(double[] array, int inputsize, int outputsize)
        {
            TemporalWindowArray temp = new TemporalWindowArray(inputsize, outputsize);
            temp.Analyze(array);
           
            return temp.Process(array);
        }

        /// <summary>
        /// Generates an array with as many double array inputs as wanted.
        /// This is useful for neural networks when you have already formated your data arrays and need to create a double []
        /// with all the inputs following each others.(Elman type format)
        /// </summary>
        /// <param name="inputs">The inputs.</param>
        /// <returns>
        /// the double [] array with all inputs.
        /// </returns>
        public static double[] GenerateInputz(params double[][] inputs)
        {
            ArrayList al = new ArrayList();
            foreach (double[] doublear in inputs)
            {
                al.Add((double[])doublear);
            }
            return (double[])al.ToArray(typeof(double));
        }


        /// <summary>
        /// Prepare realtime inputs, and place them in an understandable one jagged input neuron array.
        /// This method uses linq.
        /// you can use this method if you have many inputs and need to format them as inputs with a specified "window"/input size.
        /// You can add as many inputs as wanted to this input layer (parametrable inputs).
        /// </summary>
        /// <param name="inputsize">The inputsize.</param>
        /// <param name="firstinputt">The firstinputt.</param>
        /// <returns>a ready to use jagged array with all the inputs setup.</returns>
        public static double[][] AddInputsViaLinq(int inputsize, params double[][] firstinputt)
        {
            ArrayList arlist = new ArrayList(4);
            ArrayList FirstList = new ArrayList();
            List<double> listused = new List<double>();
            int lenghtofArrays = firstinputt[0].Length;
            //There must be NO modulo...or the arrays would not be divisible by this input size.
            if (lenghtofArrays % inputsize != 0)
                return null;
            //we add each input one , after the other in a list of doubles till we reach the input size
            for (int i = 0; i < lenghtofArrays; i++)
            {
                foreach (double[] t in firstinputt.Where(t => listused.Count < inputsize*firstinputt.Length))
                {
                    listused.Add(t[i]);
                    if (listused.Count != inputsize*firstinputt.Length) continue;
                    FirstList.Add(listused.ToArray());
                    listused.Clear();
                }
            }
            return (double[][])FirstList.ToArray(typeof(double[]));
        }


        /// <summary>
        /// Prepare realtime inputs, and place them in an understandable one jagged input neuron array.
        /// you can use this method if you have many inputs and need to format them as inputs with a specified "window"/input size.
        /// You can add as many inputs as wanted to this input layer (parametrable inputs).
        /// </summary>
        /// <param name="inputsize">The inputsize.</param>
        /// <param name="firstinputt">The firstinputt.</param>
        /// <returns>a ready to use jagged array with all the inputs setup.</returns>
        public static double[][] AddInputs(int inputsize, params double[][] firstinputt)
        {
            ArrayList arlist = new ArrayList(4);
            ArrayList FirstList = new ArrayList();
            List<double> listused = new List<double>();
            int lenghtofArrays = firstinputt[0].Length;
            //There must be NO modulo...or the arrays would not be divisile by this input size.
            if (lenghtofArrays % inputsize != 0)
                return null;
            //we add each input one , after the other in a list of doubles till we reach the input size
            for (int i = 0; i < lenghtofArrays; i++)
            {
                for (int k = 0; k < firstinputt.Length; k++)
                {
                    if (listused.Count < inputsize * firstinputt.Length)
                    {
                        listused.Add(firstinputt[k][i]);
                        if (listused.Count == inputsize * firstinputt.Length)
                        {
                            FirstList.Add(listused.ToArray());
                            listused.Clear();
                        }
                    }
                }
            }
            return (double[][])FirstList.ToArray(typeof(double[]));
        }

        /// <summary>
        /// Makes a data set with parametrable inputs and one output double array.
        /// you can provide as many inputs as needed and the timelapse size (input size).
        /// for more information on this method read the AddInputs Method.
        /// </summary>
        /// <param name="outputs">The outputs.</param>
        /// <param name="inputsize">The inputsize.</param>
        /// <param name="firstinputt">The firstinputt.</param>
        /// <returns></returns>
        public static IMLDataSet MakeDataSet(double[] outputs, int inputsize, params double[][] firstinputt)
        {
            IMLDataSet set = new BasicMLDataSet();
            ArrayList outputsar = new ArrayList();
            ArrayList FirstList = new ArrayList();
            List<double> listused = new List<double>();
            int lenghtofArrays = firstinputt[0].Length;

            //There must be NO modulo...or the arrays would not be divisible by this input size.
            if (lenghtofArrays % inputsize != 0)
                return null;
            //we add each input one , after the other in a list of doubles till we reach the input size
            for (int i = 0; i < lenghtofArrays; i++)
            {
                for (int k = 0; k < firstinputt.Length; k++)
                {
                    if (listused.Count < inputsize * firstinputt.Length)
                    {
                        listused.Add(firstinputt[k][i]);
                        if (listused.Count == inputsize * firstinputt.Length)
                        {
                            FirstList.Add(listused.ToArray());
                            listused.Clear();
                        }
                    }
                }
            }
            foreach (double d in outputs)
            {
                listused.Add(d);
                outputsar.Add(listused.ToArray());
                listused.Clear();
            }
            set = new BasicMLDataSet((double[][])FirstList.ToArray(typeof(double[])), (double[][])outputsar.ToArray(typeof(double[])));
            return set;
        }



        
        
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
        /// Makes a random dataset with the number of IMLDatapairs.
        /// Quite useful to test networks (benchmarks).
        /// </summary>
        /// <param name="inputs">The inputs.</param>
        /// <param name="predictWindow">The predict window.</param>
        /// <param name="numberofPairs">The numberof pairs.</param>
        /// <returns></returns>
        public static BasicMLDataSet MakeRandomIMLDataset(int inputs, int predictWindow, int numberofPairs)
        {
            BasicMLDataSet SuperSet = new BasicMLDataSet();
            for (int i = 0; i < numberofPairs;i++ )
            {
                double[] firstinput = MakeInputs(inputs);
                double[] secondideal = MakeInputs(inputs);
                IMLDataPair pair = ProcessPairs(firstinput, secondideal, inputs, predictWindow);
                SuperSet.Add(pair);
            }
            return SuperSet;
        }


        /// <summary>
        /// This is the most used method in finance.
        /// You send directly your double arrays and get an IMLData set ready for network training.
        /// You must place your ideal data as the last double data array.
        /// IF you have 1 data of closing prices, 1 moving average, 1 data series of interest rates , and the data you want to predict 
        /// This method will look the lenght of the first Data input to calculate how many points to take from each array.
        /// this is the method you will use to make your Dataset.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="inputs">The inputs.</param>
        /// <returns></returns>
        public static IMLDataSet MakeSetFromInputsSources(int predwindow, params double[][] inputs)
        {
            ArrayList list = new ArrayList(inputs.Length);
            IMLDataSet set = new BasicMLDataSet();
            //we know now how many items we have in each data series (all series should be of equal lenght).
            int dimension = inputs[0].Length;
            //we will take predictwindow of each serie and make new series.
            List<double> InputList = new List<double>();
            int index = 0;
            int currentArrayObserved = 0;
            //foreach (double[] doubles in inputs)
            //{

            //    for (int k = 0; k < dimension; k++)
            //    {
            //        //We will take predict window number from each of our array and add them up.
            //        for (int i = 0; i < predwindow; i++)
            //        {
            //            InputList.Add(doubles[index]);
                        
            //        }
                   
            //    }
            //    index++;
            //}

            foreach (double[] doublear in inputs)
            {

                list.Add((double[])doublear);
            }


           // return (double[][])list.ToArray(typeof(double[]));

            return set;
        }
    }
}
