//
// Encog(tm) Core v3.2 - .Net Version
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
#region

using System;
using System.Collections.Generic;
using System.IO;
using ConsoleExamples.Examples;
using Encog.MathUtil.RBF;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Data.Basic;
using Encog.Neural.Pattern;
using Encog.Neural.RBF;
using Encog.Neural.Rbf.Training;

#endregion

namespace Encog.Examples.MultiRadial
{
    public class MultiRadial : IExample
    {
        /// <summary>
        ///   Input for the XOR function.
        /// </summary>
        public static double[][] INPUT;

        public static double[][] IDEAL;

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof (MultiRadial),
                    "radial-multi",
                    "A RBF network example.",
                    "Use a RBF network to learn the XOR operator.");
                return info;
            }
        }

        #region IExample Members

        public void Execute(IExampleInterface app)
        {
            //Specify the number of dimensions and the number of neurons per dimension
            const int dimensions = 2;
            const int numNeuronsPerDimension = 7;

            //Set the standard RBF neuron width. 
            //Literature seems to suggest this is a good default value.
            const double volumeNeuronWidth = 2.0/numNeuronsPerDimension;

            //RBF can struggle when it comes to flats at the edge of the sample space.
            //We have added the ability to include wider neurons on the sample space boundary which greatly
            //improves fitting to flats
            const bool includeEdgeRBFs = true;

            #region Setup

            //General setup is the same as before
            var pattern = new RadialBasisPattern();
            pattern.InputNeurons = dimensions;
            pattern.OutputNeurons = 1;

            //Total number of neurons required.
            //Total number of Edges is calculated possibly for future use but not used any further here
            int numNeurons = (int) Math.Pow(numNeuronsPerDimension, dimensions);
           // int numEdges = (int) (dimensions*Math.Pow(2, dimensions - 1));

            pattern.AddHiddenLayer(numNeurons);

            var network = (RBFNetwork) pattern.Generate();
            //RadialBasisFunctionLayer rbfLayer = (RadialBasisFunctionLayer)network.GetLayer(RadialBasisPattern.RBF_LAYER);


            //Position the multidimensional RBF neurons, with equal spacing, within the provided sample space from 0 to 1.
            //rbfLayer.SetRBFCentersAndWidthsEqualSpacing(0, 1, RBFEnum.Gaussian, dimensions, volumeNeuronWidth, includeEdgeRBFs);
            network.SetRBFCentersAndWidthsEqualSpacing(0, 1, RBFEnum.Gaussian, volumeNeuronWidth, includeEdgeRBFs);

            #endregion

            //Create some training data that can not easily be represented by gaussians
            //There are other training examples for both 1D and 2D
            //Degenerate training data only provides outputs as 1 or 0 (averaging over all outputs for a given set of inputs would produce something approaching the smooth training data).
            //Smooth training data provides true values for the provided input dimensions.
            Create2DSmoothTainingDataGit();

            //Create the training set and train.
            var trainingSet = new BasicMLDataSet(INPUT, IDEAL);
            var train = new SVDTraining(network, trainingSet);

            //SVD is a single step solve
            int epoch = 1;
            do
            {
                train.Iteration();
                Console.WriteLine(@"Epoch #" + epoch + @" Error:" + train.Error);
                epoch++;
            } while ((epoch < 1) && (train.Error > 0.001));

            // test the neural network
            Console.WriteLine(@"Neural Network Results:");

            //Create a testing array which may be to a higher resolution than the original training data
            Set2DTestingArrays(100);
            trainingSet = new BasicNeuralDataSet(INPUT, IDEAL);

            //Write out the results data
            using (var sw = new StreamWriter("results.csv", false))
            {
                foreach (IMLDataPair pair in trainingSet)
                {
                    var output = network.Compute(pair.Input);
                    //1D//sw.WriteLine(InverseScale(pair.Input[0]) + ", " + Chop(InverseScale(output[0])));// + ", " + pair.Ideal[0]);
                    sw.WriteLine(InverseScale(pair.Input[0]) + ", " + InverseScale(pair.Input[1]) + ", " +
                                 Chop(InverseScale(output[0])));
                        // + ", " + pair.Ideal[0]);// + ",ideal=" + pair.Ideal[0]);
                    //3D//sw.WriteLine(InverseScale(pair.Input[0]) + ", " + InverseScale(pair.Input[1]) + ", " + InverseScale(pair.Input[2]) + ", " + Chop(InverseScale(output[0])));// + ", " + pair.Ideal[0]);// + ",ideal=" + pair.Ideal[0]);
                    //Console.WriteLine(pair.Input[0] + ", actual=" + output[0] + ",ideal=" + pair.Ideal[0]);
                }
            }

            Console.WriteLine(@"\nFit output saved to results.csv");
            Console.WriteLine(@"\nComplete - Please press the 'any' key to close.");
            Console.ReadKey();
        }

        #endregion

        private static double Scale(double x)
        {
            return (x*0.7) + 0.15;
        }

        private static double InverseScale(double x)
        {
            return (x - 0.15)/0.7;
        }

        private static double Chop(double x)
        {
            if (x > 0.99)
                return 0.99;
            return x < 0 ? 0 : x;
        }

// ReSharper disable UnusedMember.Local
        private static void SaveOutNeuronCentersAndWeights(double[][] centers, double[][] widths)
// ReSharper restore UnusedMember.Local
        {
            using (var sw = new StreamWriter("neuronCentersWeights.csv", false))
            {
                for (int i = 0; i < centers.Length; i++)
                {
                    foreach (double value in centers[i])
                        sw.Write(value + ",");

                    foreach (double value in widths[i])
                        sw.Write(value + ",");

                    sw.WriteLine();
                }
            }
        }

// ReSharper disable UnusedMember.Local
        private static double[][] LoadMatrix(string fileName)
// ReSharper restore UnusedMember.Local
        {
            var allLines = File.ReadAllLines(fileName);

            var matrix = new double[allLines.Length][];

            for (int i = 0; i < allLines.Length; i++)
            {
                var values = allLines[i].Split(',');
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

// ReSharper disable UnusedMember.Local
        private static void SaveMatrix(IEnumerable<double[]> surface, string fileName)
// ReSharper restore UnusedMember.Local
        {
            using (var sw = new StreamWriter(fileName, false))
            {
                foreach (double[] t in surface)
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

// ReSharper disable UnusedMember.Local
        private static double[][] ConvertColumnsTo2DSurface(double[][] cols, int valueCol)
// ReSharper restore UnusedMember.Local
        {
            //if (cols[0].Length != 3)
            //    throw new Exception("Incorrect number of cols detected.");

            double sideLength = Math.Sqrt(cols.Length);
            var surface = new double[(int) sideLength + 1][];

            for (int i = 0; i < surface.Length; i++)
            {
                surface[i] = new double[surface.Length];
            }

            foreach (double[] t in cols)
            {
//[0] is x
                //[1] is y
                //Boundary bottom 
                //int rowIndex = (int)Math.Round(((cols[i][0]) * (sideLength-1)), 6);
                //int columnIndex = (int)Math.Round(((cols[i][1]) * (sideLength-1)), 6);

                //Boundary middle
                int rowIndex = (int) Math.Round(((t[0] - 0.05)*(sideLength)), 6);
                int columnIndex = (int) Math.Round(((t[1] - 0.05)*(sideLength)), 6);

                surface[0][rowIndex + 1] = t[0];
                surface[columnIndex + 1][0] = t[1];
                surface[columnIndex + 1][rowIndex + 1] = t[valueCol];
            }

            //fix the 0,0 value
            surface[0][0] = double.NaN;

            return surface;
        }

// ReSharper disable UnusedMember.Local
        private static double[][] Convert2DSurfaceToColumns(IList<double[]> surface)
// ReSharper restore UnusedMember.Local
        {
            int totalRows = (surface.Count - 1)*(surface.Count - 1);
            var cols = new double[totalRows][];

            for (int i = 1; i < surface.Count; i++)
            {
                for (int j = 1; j < surface[i].Length; j++)
                {
                    double cellWidth = (1.0/(2.0*(surface.Count - 1)));
                    cols[(i - 1)*(surface.Count - 1) + (j - 1)] = new double[3];
                    //For midpoints
                    cols[(i - 1)*(surface.Count - 1) + (j - 1)][0] = ((i - 1)/(double) (surface.Count - 1)) +
                                                                      cellWidth;
                    cols[(i - 1)*(surface.Count - 1) + (j - 1)][1] = ((j - 1)/(double) (surface.Count - 1)) +
                                                                      cellWidth;
                    //For actual value
                    //cols[(i - 1) * (surface.Length - 1) + (j - 1)][0] = ((double)(i - 1) / (double)(surface.Length - 1));
                    //cols[(i - 1) * (surface.Length - 1) + (j - 1)][1] = ((double)(j - 1) / (double)(surface.Length - 1));
                    cols[(i - 1)*(surface.Count - 1) + (j - 1)][2] = surface[j][i];
                }
            }

            return cols;
        }

        #region LoadRealData

// ReSharper disable UnusedMember.Local
        private static void LoadReal1DTrainingData(string fileName)
// ReSharper restore UnusedMember.Local
        {
            var allLines = File.ReadAllLines(fileName);

            INPUT = new double[allLines.Length][];
            IDEAL = new double[allLines.Length][];

            for (int i = 0; i < allLines.Length; i++)
            {
                INPUT[i] = new double[1];
                IDEAL[i] = new double[1];

                var values = allLines[i].Split(',');

                INPUT[i][0] = Scale((Convert.ToDouble(values[0]) - 0.05)*(1.0/0.9));
                IDEAL[i][0] = Scale(Convert.ToDouble(values[1]));
            }
        }

// ReSharper disable UnusedMember.Local
        private static void LoadReal2DTrainingData(string fileName)
// ReSharper restore UnusedMember.Local
        {
            var allLines = File.ReadAllLines(fileName);

            INPUT = new double[allLines.Length][];
            IDEAL = new double[allLines.Length][];

            for (int i = 0; i < allLines.Length; i++)
            {
                INPUT[i] = new double[2];
                IDEAL[i] = new double[1];

                var values = allLines[i].Split(',');

                INPUT[i][0] = Scale((Convert.ToDouble(values[0]) - 0.05)*(1.0/0.9));
                INPUT[i][1] = Scale((Convert.ToDouble(values[1]) - 0.05)*(1.0/0.9));
                IDEAL[i][0] = Scale(Convert.ToDouble(values[2]));
            }
        }

// ReSharper disable UnusedMember.Local
        private static void LoadReal3DTrainingData(string fileName)
// ReSharper restore UnusedMember.Local
        {
            var allLines = File.ReadAllLines(fileName);

            INPUT = new double[allLines.Length][];
            IDEAL = new double[allLines.Length][];

            for (int i = 0; i < allLines.Length; i++)
            {
                INPUT[i] = new double[3];
                IDEAL[i] = new double[1];

                var values = allLines[i].Split(',');

                INPUT[i][0] = Scale(Convert.ToDouble(values[0]));
                INPUT[i][1] = Scale(Convert.ToDouble(values[1]));
                INPUT[i][2] = Scale(Convert.ToDouble(values[2]));
                IDEAL[i][0] = Scale(Convert.ToDouble(values[3]));
            }
        }

        #endregion

        #region CreateTestingInputs

// ReSharper disable UnusedMember.Local
        private static void Set1DTestingArrays(int sideLength)
// ReSharper restore UnusedMember.Local
        {
            int iLimit = sideLength;

            INPUT = new double[(iLimit + 1)][];
            IDEAL = new double[(iLimit + 1)][];

            for (int i = 0; i <= iLimit; i++)
            {
                INPUT[i] = new double[1];
                IDEAL[i] = new double[1];

                //double x = i/(double) iLimit;

                INPUT[i][0] = Scale((i/((double) iLimit)));
                IDEAL[i][0] = 0;
            }
        }

        private static void Set2DTestingArrays(int sideLength)
        {
            int iLimit = sideLength;
            int kLimit = sideLength;

            INPUT = new double[(iLimit + 1)*(kLimit + 1)][];
            IDEAL = new double[(iLimit + 1)*(kLimit + 1)][];

            for (int i = 0; i <= iLimit; i++)
            {
                for (int k = 0; k <= kLimit; k++)
                {
                    INPUT[i*(kLimit + 1) + k] = new double[2];
                    IDEAL[i*(kLimit + 1) + k] = new double[1];

                    //double x = i/(double) iLimit;
                    //double y = k/(double) kLimit;
                    INPUT[i*(kLimit + 1) + k][0] = Scale((i/((double) iLimit)));
                    INPUT[i*(kLimit + 1) + k][1] = Scale((k/((double) kLimit)));
                    IDEAL[i*(kLimit + 1) + k][0] = 0;
                }
            }
        }

// ReSharper disable UnusedMember.Local
        private static void Set3DTestingArrays(int sideLength)
// ReSharper restore UnusedMember.Local
        {
            int iLimit = sideLength;
            int kLimit = sideLength;
            int jLimit = sideLength;

            INPUT = new double[(iLimit + 1)*(kLimit + 1)*(jLimit + 1)][];
            IDEAL = new double[(iLimit + 1)*(kLimit + 1)*(jLimit + 1)][];

            for (int i = 0; i <= iLimit; i++)
            {
                for (int k = 0; k <= kLimit; k++)
                {
                    for (int j = 0; j <= jLimit; j++)
                    {
                        int index = (i*(kLimit + 1)*(jLimit + 1)) + (j*(kLimit + 1)) + k;
                        INPUT[index] = new double[3];
                        IDEAL[index] = new double[1];

                        //double x = (double)i / (double)iLimit;
                        //double y = (double)k / (double)kLimit;
                        //double z = (double)j / (double)jLimit;

                        INPUT[index][0] = Scale((i/((double) iLimit)));
                        INPUT[index][1] = Scale((k/((double) kLimit)));
                        INPUT[index][2] = Scale((j/((double) jLimit)));
                        IDEAL[index][0] = 0;
                    }
                }
            }
        }

        #endregion

        #region CreateTrainingData

// ReSharper disable UnusedMember.Local
        private static void Create2DDegenerateTainingDataHill()
// ReSharper restore UnusedMember.Local
        {
            var r = new Random();

            const int iLimit = 30;
            const int kLimit = 30;
            const int jLimit = 1;

            INPUT = new double[jLimit*iLimit*kLimit][];
            IDEAL = new double[jLimit*iLimit*kLimit][];

            for (int i = 0; i < iLimit; i++)
            {
                for (int k = 0; k < kLimit; k++)
                {
                    for (int j = 0; j < jLimit; j++)
                    {
                        INPUT[i*jLimit*kLimit + k*jLimit + j] = new double[2];
                        IDEAL[i*jLimit*kLimit + k*jLimit + j] = new double[1];

                        double x = i/(double) iLimit;
                        double y = k/(double) kLimit;

                        INPUT[i*jLimit*kLimit + k*jLimit + j][0] = (i/((double) iLimit));
                        INPUT[i*jLimit*kLimit + k*jLimit + j][1] = (k/((double) iLimit));
                        IDEAL[i*jLimit*kLimit + k*jLimit + j][0] = (r.NextDouble() <
                                                                    (Math.Exp(
                                                                        -((x - 0.6)*(x - 0.6) + (y - 0.5)*(y - 0.5))*3) -
                                                                     0.1))
                                                                       ? 1
                                                                       : 0;
                    }
                }
            }
        }

// ReSharper disable UnusedMember.Local
        private static void Create2DSmoothTainingDataHill()
// ReSharper restore UnusedMember.Local
        {
            var r = new Random();

            const int iLimit = 100;
            const int kLimit = 100;
            const int jLimit = 10000;

            INPUT = new double[(iLimit + 1)*(kLimit + 1)][];
            IDEAL = new double[(iLimit + 1)*(kLimit + 1)][];

            for (int i = 0; i <= iLimit; i++)
            {
                for (int k = 0; k <= kLimit; k++)
                {
                    INPUT[i*(kLimit + 1) + k] = new double[2];
                    IDEAL[i*(kLimit + 1) + k] = new double[1];

                    double average = 0;

                    double x = i/(double) iLimit;
                    double y = k/(double) kLimit;

                    double expression = (Math.Exp(-((x - 0.5)*(x - 0.5) + (y - 0.6)*(y - 0.6))*3) - 0.1);

                    //if (r.NextDouble() < 0.4) jLimit = 5; else jLimit = 10;

                    for (int j = 0; j < jLimit; j++)
                    {
                        average += (r.NextDouble() < expression) ? 1 : 0;
                    }

                    INPUT[i*(kLimit + 1) + k][0] = Scale((i/((double) iLimit)));
                    INPUT[i*(kLimit + 1) + k][1] = Scale((k/((double) kLimit)));
                    IDEAL[i*(kLimit + 1) + k][0] = Scale((average/jLimit));
                }
            }
        }

        private static void Create2DSmoothTainingDataGit()
        {
            const int iLimit = 10;
            const int kLimit = 10;
            //int jLimit = 100;

            INPUT = new double[(iLimit + 1)*(kLimit + 1)][];
            IDEAL = new double[(iLimit + 1)*(kLimit + 1)][];

            for (int i = 0; i <= iLimit; i++)
            {
                for (int k = 0; k <= kLimit; k++)
                {
                    INPUT[i*(kLimit + 1) + k] = new double[2];
                    IDEAL[i*(kLimit + 1) + k] = new double[1];

                    double x = i/(double) iLimit;
                    double y = k/(double) kLimit;

                    double expression = ((x + 1.0/3.0)*(2 + Math.Log10((y/(x + 0.1)) + 0.1)))/3;

                    INPUT[i*(kLimit + 1) + k][0] = Scale((i/((double) iLimit)));
                    INPUT[i*(kLimit + 1) + k][1] = Scale((k/((double) kLimit)));
                    IDEAL[i*(kLimit + 1) + k][0] = Scale(expression);
                }
            }
        }

// ReSharper disable UnusedMember.Local
        private static void Create2DDegenerateTainingDataGit()
// ReSharper restore UnusedMember.Local
        {
            var r = new Random();

            const int iLimit = 10;
            const int kLimit = 10;
            const int jLimit = 10;

            INPUT = new double[jLimit*iLimit*kLimit][];
            IDEAL = new double[jLimit*iLimit*kLimit][];

            for (int i = 0; i < iLimit; i++)
            {
                for (int k = 0; k < kLimit; k++)
                {
                    double x = i/(double) iLimit;
                    double y = k/(double) kLimit;

                    for (int j = 0; j < jLimit; j++)
                    {
                        INPUT[i*jLimit*kLimit + k*jLimit + j] = new double[2];
                        IDEAL[i*jLimit*kLimit + k*jLimit + j] = new double[1];

                        double expression = ((x + 1.0/3.0)*(2 + Math.Log10((y/(x + 0.1)) + 0.1)))/3;
                        

                        INPUT[i*jLimit*kLimit + k*jLimit + j][0] = (i/((double) iLimit));
                        INPUT[i*jLimit*kLimit + k*jLimit + j][1] = (k/((double) iLimit));
                        IDEAL[i*jLimit*kLimit + k*jLimit + j][0] = (r.NextDouble() < expression) ? 1 : 0;
                    }
                }
            }
        }

// ReSharper disable UnusedMember.Local
        private static void Create1DDegenerateTrainingDataLine()
// ReSharper restore UnusedMember.Local
        {
            var r = new Random(14768);

            const int iLimit = 10;
            const int jLimit = 100;

            INPUT = new double[iLimit*jLimit][];
            IDEAL = new double[iLimit*jLimit][];

            for (int i = 0; i < iLimit; i++)
            {
                for (int j = 0; j < jLimit; j++)
                {
                    INPUT[i*jLimit + j] = new double[1];
                    IDEAL[i*jLimit + j] = new double[1];

                    double x = i/(double) iLimit;

                    INPUT[i*jLimit + j][0] = Scale(x);
                    IDEAL[i*jLimit + j][0] = Scale((r.NextDouble() < x) ? 1 : 0);
                }
            }
        }

// ReSharper disable UnusedMember.Local
        private static void Create1DSmoothTrainingDataLine()
// ReSharper restore UnusedMember.Local
        {
            var r = new Random(14768);

            const int iLimit = 1000;
            const int jLimit = 1;

            INPUT = new double[iLimit][];
            IDEAL = new double[iLimit][];

            for (int i = 0; i < iLimit; i++)
            {
                INPUT[i] = new double[1];
                IDEAL[i] = new double[1];
                double average = 0;
                double x = i/(double) iLimit;

                for (int j = 0; j < jLimit; j++)
                    average += (r.NextDouble() < x) ? 1 : 0;

                INPUT[i][0] = Scale(x);
                IDEAL[i][0] = Scale(average/jLimit);
            }
        }

// ReSharper disable UnusedMember.Local
        private static void Create1DSmoothTrainingDataCurveSimple()
// ReSharper restore UnusedMember.Local
        {
            var r = new Random(14768);

            const int iLimit = 20;
            const int jLimit = 10;

            INPUT = new double[iLimit][];
            IDEAL = new double[iLimit][];

            for (int i = 0; i < iLimit; i++)
            {
                INPUT[i] = new double[1];
                IDEAL[i] = new double[1];
                double average = 0;
                double x = i/(double) iLimit;

                for (int j = 0; j < jLimit; j++)
                    average += (r.NextDouble() < (-4*Math.Pow(x, 2) + 4*x)) ? 1 : 0;

                INPUT[i][0] = Scale(x);
                IDEAL[i][0] = Scale(average/jLimit);
            }
        }

// ReSharper disable UnusedMember.Local
        private static void Create1DSmoothTrainingDataCurveAdv()
// ReSharper restore UnusedMember.Local
        {
            var r = new Random(14768);

            const int iLimit = 100;
            const int jLimit = 100;

            INPUT = new double[iLimit][];
            IDEAL = new double[iLimit][];

            for (int i = 0; i < iLimit; i++)
            {
                INPUT[i] = new double[1];
                IDEAL[i] = new double[1];
                double average = 0;
                double x = i/(double) iLimit;

                //double y = (-7.5 * Math.Pow(x, 4)) + (21.3 * Math.Pow(x, 3)) + (-22.3 * Math.Pow(x, 2)) + (10.4 * x) - 0.8;
                double y = ((Math.Exp(2.0*(x*4.0 - 1)) - 1.0)/(Math.Exp(2.0*(x*4.0 - 1)) + 1.0))/2 + 0.5;

                for (int j = 0; j < jLimit; j++)
                {
                    average += (r.NextDouble() < y) ? 1 : 0;
                }

                INPUT[i][0] = Scale(x);
                IDEAL[i][0] = Scale(average/jLimit);
            }
        }

        #endregion
    }
}
