using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util;
using System.Reflection;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.SCG;
using Encog.Neural.NeuralData;
using Encog.Neural.Data.Basic;
using Encog.Util.Simple;
using Encog.Neural.Data;
using System.IO;
using Encog.Neural.Networks.Pattern;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training.LMA;
using Encog.Neural.Networks.Training.Propagation.Manhattan;
using Encog.Neural.Networks.Training.SVD;
using Encog;
using ConsoleExamples.Examples;


namespace Encog.Examples.Radial
{
    public class MultiRadial : IExample
    {
        /// <summary>
        /// Input for the XOR function.
        /// </summary>
        public static double[][] INPUT;
        public static double[][] IDEAL;

        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(MultiRadial),
                    "radial-multi",
                    "A RBF network example.",
                    "Use a RBF network to learn the XOR operator.");
                return info;
            }
        }

        public void Execute(IExampleInterface app)
        {
            //Specify the number of dimensions and the number of neurons per dimension
            int dimensions = 2;
            int numNeuronsPerDimension = 7;

            //Set the standard RBF neuron width. 
            //Literature seems to suggest this is a good default value.
            double volumeNeuronWidth = 2.0 / numNeuronsPerDimension;

            //RBF can struggle when it comes to flats at the edge of the sample space.
            //We have added the ability to include wider neurons on the sample space boundary which greatly
            //improves fitting to flats
            bool includeEdgeRBFs = true;

            #region Setup
            //General setup is the same as before
            RadialBasisPattern pattern = new RadialBasisPattern();
            pattern.InputNeurons = dimensions;
            pattern.OutputNeurons = 1;

            //Total number of neurons required.
            //Total number of Edges is calculated possibly for future use but not used any further here
            int numNeurons = (int)Math.Pow(numNeuronsPerDimension, dimensions);
            int numEdges = (int)(dimensions * Math.Pow(2, dimensions - 1));

            pattern.AddHiddenLayer(numNeurons);

            BasicNetwork network = pattern.Generate();
            RadialBasisFunctionLayer rbfLayer = (RadialBasisFunctionLayer)network.GetLayer(RadialBasisPattern.RBF_LAYER);
            network.Reset();

            //Position the multidimensional RBF neurons, with equal spacing, within the provided sample space from 0 to 1.
            rbfLayer.SetRBFCentersAndWidthsEqualSpacing(0, 1, RBFEnum.Gaussian, dimensions, volumeNeuronWidth, includeEdgeRBFs);            
            
            #endregion

            //Create some training data that can not easily be represented by gaussians
            //There are other training examples for both 1D and 2D
            //Degenerate training data only provides outputs as 1 or 0 (averaging over all outputs for a given set of inputs would produce something approaching the smooth training data).
            //Smooth training data provides true values for the provided input dimensions.
            Create2DSmoothTainingDataGit();

            //Create the training set and train.
            INeuralDataSet trainingSet = new BasicNeuralDataSet(INPUT, IDEAL);
            ITrain train = new SVDTraining(network, trainingSet);

            //SVD is a single step solve
            int epoch = 1;
            do
            {
                train.Iteration();
                Console.WriteLine("Epoch #" + epoch + " Error:" + train.Error);
                epoch++;
            } while ((epoch < 1) && (train.Error > 0.001));

            // test the neural network
            Console.WriteLine("Neural Network Results:");

            //Create a testing array which may be to a higher resoltion than the original training data
            Set2DTestingArrays(100);
            trainingSet = new BasicNeuralDataSet(INPUT, IDEAL);

            //Write out the results data
            using (var sw = new System.IO.StreamWriter("results.csv", false))
            {
                foreach (INeuralDataPair pair in trainingSet)
                {
                    INeuralData output = network.Compute(pair.Input);
                    //1D//sw.WriteLine(InverseScale(pair.Input[0]) + ", " + Chop(InverseScale(output[0])));// + ", " + pair.Ideal[0]);
                    sw.WriteLine(InverseScale(pair.Input[0]) + ", " + InverseScale(pair.Input[1]) + ", " + Chop(InverseScale(output[0])));// + ", " + pair.Ideal[0]);// + ",ideal=" + pair.Ideal[0]);
                    //3D//sw.WriteLine(InverseScale(pair.Input[0]) + ", " + InverseScale(pair.Input[1]) + ", " + InverseScale(pair.Input[2]) + ", " + Chop(InverseScale(output[0])));// + ", " + pair.Ideal[0]);// + ",ideal=" + pair.Ideal[0]);
                    //Console.WriteLine(pair.Input[0] + ", actual=" + output[0] + ",ideal=" + pair.Ideal[0]);
                }
            }

            Console.WriteLine("\nFit output saved to results.csv");
            Console.WriteLine("\nComplete - Please press the 'any' key to close.");
            Console.ReadKey();
        }

        static double Scale(double x)
        {
            return (x * 0.7) + 0.15;
        }

        static double InverseScale(double x)
        {
            return (x - 0.15) / 0.7;
        }

        static double Chop(double x)
        {
            if (x > 0.99)
                return 0.99;
            else if (x < 0)
                return 0;
            else
                return x;
        }

        private static void SaveOutNeuronCentersAndWeights(double[][] centers, double[][] widths)
        {
            using (var sw = new System.IO.StreamWriter("neuronCentersWeights.csv", false))
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

        private static double[][] LoadMatrix(string fileName)
        {
            string[] allLines = File.ReadAllLines(fileName);

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

        private static double[][] ConvertColumnsTo2DSurface(double[][] cols, int valueCol)
        {
            //if (cols[0].Length != 3)
            //    throw new Exception("Incorrect number of cols detected.");

            double sideLength = Math.Sqrt(cols.Length);
            double[][] surface = new double[(int)sideLength + 1][];

            for (int i = 0; i < surface.Length; i++)
            {
                surface[i] = new double[surface.Length];
            }

            for (int i = 0; i < cols.Length; i++)
            {
                //[0] is x
                //[1] is y
                //Boundary bottom 
                //int rowIndex = (int)Math.Round(((cols[i][0]) * (sideLength-1)), 6);
                //int columnIndex = (int)Math.Round(((cols[i][1]) * (sideLength-1)), 6);

                //Boundary middle
                int rowIndex = (int)Math.Round(((cols[i][0] - 0.05) * (sideLength)), 6);
                int columnIndex = (int)Math.Round(((cols[i][1] - 0.05) * (sideLength)), 6);

                surface[0][rowIndex + 1] = cols[i][0];
                surface[columnIndex + 1][0] = cols[i][1];
                surface[columnIndex + 1][rowIndex + 1] = cols[i][valueCol];
            }

            //fix the 0,0 value
            surface[0][0] = double.NaN;

            return surface;
        }

        private static double[][] Convert2DSurfaceToColumns(double[][] surface)
        {
            int totalRows = (surface.Length - 1) * (surface.Length - 1);
            double[][] cols = new double[totalRows][];

            for (int i = 1; i < surface.Length; i++)
            {
                for (int j = 1; j < surface[i].Length; j++)
                {
                    double cellWidth = (1.0 / (2.0 * (double)(surface.Length - 1)));
                    cols[(i - 1) * (surface.Length - 1) + (j - 1)] = new double[3];
                    //For midpoints
                    cols[(i - 1) * (surface.Length - 1) + (j - 1)][0] = ((double)(i - 1) / (double)(surface.Length - 1)) + cellWidth;
                    cols[(i - 1) * (surface.Length - 1) + (j - 1)][1] = ((double)(j - 1) / (double)(surface.Length - 1)) + cellWidth;
                    //For actual value
                    //cols[(i - 1) * (surface.Length - 1) + (j - 1)][0] = ((double)(i - 1) / (double)(surface.Length - 1));
                    //cols[(i - 1) * (surface.Length - 1) + (j - 1)][1] = ((double)(j - 1) / (double)(surface.Length - 1));
                    cols[(i - 1) * (surface.Length - 1) + (j - 1)][2] = surface[j][i];
                }
            }

            return cols;
        }

        #region LoadRealData

        private static void LoadReal1DTrainingData(string fileName)
        {
            string[] allLines = File.ReadAllLines(fileName);

            INPUT = new double[allLines.Length][];
            IDEAL = new double[allLines.Length][];

            for (int i = 0; i < allLines.Length; i++)
            {
                INPUT[i] = new double[1];
                IDEAL[i] = new double[1];

                string[] values = allLines[i].Split(',');

                INPUT[i][0] = Scale((Convert.ToDouble(values[0]) - 0.05) * (1.0 / 0.9));
                IDEAL[i][0] = Scale(Convert.ToDouble(values[1]));
            }
        }

        private static void LoadReal2DTrainingData(string fileName)
        {
            string[] allLines = File.ReadAllLines(fileName);

            INPUT = new double[allLines.Length][];
            IDEAL = new double[allLines.Length][];

            for (int i = 0; i < allLines.Length; i++)
            {
                INPUT[i] = new double[2];
                IDEAL[i] = new double[1];

                string[] values = allLines[i].Split(',');

                INPUT[i][0] = Scale((Convert.ToDouble(values[0]) - 0.05) * (1.0 / 0.9));
                INPUT[i][1] = Scale((Convert.ToDouble(values[1]) - 0.05) * (1.0 / 0.9));
                IDEAL[i][0] = Scale(Convert.ToDouble(values[2]));
            }
        }

        private static void LoadReal3DTrainingData(string fileName)
        {
            string[] allLines = File.ReadAllLines(fileName);

            INPUT = new double[allLines.Length][];
            IDEAL = new double[allLines.Length][];

            for (int i = 0; i < allLines.Length; i++)
            {
                INPUT[i] = new double[3];
                IDEAL[i] = new double[1];

                string[] values = allLines[i].Split(',');

                INPUT[i][0] = Scale(Convert.ToDouble(values[0]));
                INPUT[i][1] = Scale(Convert.ToDouble(values[1]));
                INPUT[i][2] = Scale(Convert.ToDouble(values[2]));
                IDEAL[i][0] = Scale(Convert.ToDouble(values[3]));
            }
        }

        #endregion

        #region CreateTestingInputs

        private static void Set1DTestingArrays(int sideLength)
        {
            int iLimit = sideLength;

            INPUT = new double[(iLimit + 1)][];
            IDEAL = new double[(iLimit + 1)][];

            for (int i = 0; i <= iLimit; i++)
            {
                INPUT[i] = new double[1];
                IDEAL[i] = new double[1];

                double x = (double)i / (double)iLimit;

                INPUT[i][0] = Scale(((double)i / ((double)iLimit)));
                IDEAL[i][0] = 0;
            }
        }

        private static void Set2DTestingArrays(int sideLength)
        {
            int iLimit = sideLength;
            int kLimit = sideLength;

            INPUT = new double[(iLimit + 1) * (kLimit + 1)][];
            IDEAL = new double[(iLimit + 1) * (kLimit + 1)][];

            for (int i = 0; i <= iLimit; i++)
            {
                for (int k = 0; k <= kLimit; k++)
                {
                    INPUT[i * (kLimit + 1) + k] = new double[2];
                    IDEAL[i * (kLimit + 1) + k] = new double[1];

                    double x = (double)i / (double)iLimit;
                    double y = (double)k / (double)kLimit;

                    INPUT[i * (kLimit + 1) + k][0] = Scale(((double)i / ((double)iLimit)));
                    INPUT[i * (kLimit + 1) + k][1] = Scale(((double)k / ((double)kLimit)));
                    IDEAL[i * (kLimit + 1) + k][0] = 0;
                }
            }
        }

        private static void Set3DTestingArrays(int sideLength)
        {
            int iLimit = sideLength;
            int kLimit = sideLength;
            int jLimit = sideLength;

            INPUT = new double[(iLimit + 1) * (kLimit + 1) * (jLimit + 1)][];
            IDEAL = new double[(iLimit + 1) * (kLimit + 1) * (jLimit + 1)][];

            for (int i = 0; i <= iLimit; i++)
            {
                for (int k = 0; k <= kLimit; k++)
                {
                    for (int j = 0; j <= jLimit; j++)
                    {
                        int index = (i * (kLimit + 1) * (jLimit + 1)) + (j * (kLimit + 1)) + k;
                        INPUT[index] = new double[3];
                        IDEAL[index] = new double[1];

                        //double x = (double)i / (double)iLimit;
                        //double y = (double)k / (double)kLimit;
                        //double z = (double)j / (double)jLimit;

                        INPUT[index][0] = Scale(((double)i / ((double)iLimit)));
                        INPUT[index][1] = Scale(((double)k / ((double)kLimit)));
                        INPUT[index][2] = Scale(((double)j / ((double)jLimit)));
                        IDEAL[index][0] = 0;
                    }
                }
            }
        }

        #endregion

        #region CreateTrainingData
        static void Create2DDegenerateTainingDataHill()
        {
            Random r = new Random();

            int iLimit = 30;
            int kLimit = 30;
            int jLimit = 1;

            INPUT = new double[jLimit * iLimit * kLimit][];
            IDEAL = new double[jLimit * iLimit * kLimit][];

            for (int i = 0; i < iLimit; i++)
            {
                for (int k = 0; k < kLimit; k++)
                {
                    for (int j = 0; j < jLimit; j++)
                    {
                        INPUT[i * jLimit * kLimit + k * jLimit + j] = new double[2];
                        IDEAL[i * jLimit * kLimit + k * jLimit + j] = new double[1];

                        double x = (double)i / (double)iLimit;
                        double y = (double)k / (double)kLimit;

                        INPUT[i * jLimit * kLimit + k * jLimit + j][0] = ((double)i / ((double)iLimit));
                        INPUT[i * jLimit * kLimit + k * jLimit + j][1] = ((double)k / ((double)iLimit));
                        IDEAL[i * jLimit * kLimit + k * jLimit + j][0] = (r.NextDouble() < (Math.Exp(-((x - 0.6) * (x - 0.6) + (y - 0.5) * (y - 0.5)) * 3) - 0.1)) ? 1 : 0;

                    }
                }
            }
        }

        static void Create2DSmoothTainingDataHill()
        {
            Random r = new Random();

            int iLimit = 100;
            int kLimit = 100;
            int jLimit = 10000;

            INPUT = new double[(iLimit + 1) * (kLimit + 1)][];
            IDEAL = new double[(iLimit + 1) * (kLimit + 1)][];

            for (int i = 0; i <= iLimit; i++)
            {
                for (int k = 0; k <= kLimit; k++)
                {
                    INPUT[i * (kLimit + 1) + k] = new double[2];
                    IDEAL[i * (kLimit + 1) + k] = new double[1];

                    double average = 0;

                    double x = (double)i / (double)iLimit;
                    double y = (double)k / (double)kLimit;

                    double expression = (Math.Exp(-((x - 0.5) * (x - 0.5) + (y - 0.6) * (y - 0.6)) * 3) - 0.1);

                    //if (r.NextDouble() < 0.4) jLimit = 5; else jLimit = 10;

                    for (int j = 0; j < jLimit; j++)
                    {
                        average += (r.NextDouble() < expression) ? 1 : 0;
                    }

                    INPUT[i * (kLimit + 1) + k][0] = Scale(((double)i / ((double)iLimit)));
                    INPUT[i * (kLimit + 1) + k][1] = Scale(((double)k / ((double)kLimit)));
                    IDEAL[i * (kLimit + 1) + k][0] = Scale((average / (double)jLimit));
                }
            }
        }

        static void Create2DSmoothTainingDataGit()
        {
            Random r = new Random();

            int iLimit = 10;
            int kLimit = 10;
            //int jLimit = 100;

            INPUT = new double[(iLimit + 1) * (kLimit + 1)][];
            IDEAL = new double[(iLimit + 1) * (kLimit + 1)][];

            for (int i = 0; i <= iLimit; i++)
            {
                for (int k = 0; k <= kLimit; k++)
                {
                    INPUT[i * (kLimit + 1) + k] = new double[2];
                    IDEAL[i * (kLimit + 1) + k] = new double[1];

                    double x = (double)i / (double)iLimit;
                    double y = (double)k / (double)kLimit;

                    double expression = ((x + 1.0 / 3.0) * (2 + Math.Log10((y / (x + 0.1)) + 0.1))) / 3;

                    INPUT[i * (kLimit + 1) + k][0] = Scale(((double)i / ((double)iLimit)));
                    INPUT[i * (kLimit + 1) + k][1] = Scale(((double)k / ((double)kLimit)));
                    IDEAL[i * (kLimit + 1) + k][0] = Scale(expression);
                }
            }
        }

        static void Create2DDegenerateTainingDataGit()
        {
            Random r = new Random();

            int iLimit = 10;
            int kLimit = 10;
            int jLimit = 10;

            INPUT = new double[jLimit * iLimit * kLimit][];
            IDEAL = new double[jLimit * iLimit * kLimit][];

            for (int i = 0; i < iLimit; i++)
            {
                for (int k = 0; k < kLimit; k++)
                {
                    double x = (double)i / (double)iLimit;
                    double y = (double)k / (double)kLimit;

                    for (int j = 0; j < jLimit; j++)
                    {
                        INPUT[i * jLimit * kLimit + k * jLimit + j] = new double[2];
                        IDEAL[i * jLimit * kLimit + k * jLimit + j] = new double[1];

                        double expression = ((x + 1.0 / 3.0) * (2 + Math.Log10((y / (x + 0.1)) + 0.1))) / 3; ;

                        INPUT[i * jLimit * kLimit + k * jLimit + j][0] = ((double)i / ((double)iLimit));
                        INPUT[i * jLimit * kLimit + k * jLimit + j][1] = ((double)k / ((double)iLimit));
                        IDEAL[i * jLimit * kLimit + k * jLimit + j][0] = (r.NextDouble() < expression) ? 1 : 0;

                    }
                }
            }
        }

        static void Create1DDegenerateTrainingDataLine()
        {
            Random r = new Random(14768);

            int iLimit = 10;
            int jLimit = 100;

            INPUT = new double[iLimit * jLimit][];
            IDEAL = new double[iLimit * jLimit][];

            for (int i = 0; i < iLimit; i++)
            {
                for (int j = 0; j < jLimit; j++)
                {
                    INPUT[i * jLimit + j] = new double[1];
                    IDEAL[i * jLimit + j] = new double[1];

                    double x = (double)i / (double)iLimit;

                    INPUT[i * jLimit + j][0] = Scale(x);
                    IDEAL[i * jLimit + j][0] = Scale((r.NextDouble() < x) ? 1 : 0);
                }
            }
        }

        static void Create1DSmoothTrainingDataLine()
        {
            Random r = new Random(14768);

            int iLimit = 1000;
            int jLimit = 1;

            INPUT = new double[iLimit][];
            IDEAL = new double[iLimit][];

            for (int i = 0; i < iLimit; i++)
            {
                INPUT[i] = new double[1];
                IDEAL[i] = new double[1];
                double average = 0;
                double x = (double)i / (double)iLimit;

                for (int j = 0; j < jLimit; j++)
                    average += (r.NextDouble() < x) ? 1 : 0;

                INPUT[i][0] = Scale(x);
                IDEAL[i][0] = Scale((double)average / (double)jLimit);
            }
        }

        static void Create1DSmoothTrainingDataCurveSimple()
        {
            Random r = new Random(14768);

            int iLimit = 20;
            int jLimit = 10;

            INPUT = new double[iLimit][];
            IDEAL = new double[iLimit][];

            for (int i = 0; i < iLimit; i++)
            {
                INPUT[i] = new double[1];
                IDEAL[i] = new double[1];
                double average = 0;
                double x = (double)i / (double)iLimit;

                for (int j = 0; j < jLimit; j++)
                    average += (r.NextDouble() < (-4 * Math.Pow(x, 2) + 4 * x)) ? 1 : 0;

                INPUT[i][0] = Scale(x);
                IDEAL[i][0] = Scale((double)average / (double)jLimit);
            }
        }

        static void Create1DSmoothTrainingDataCurveAdv()
        {
            Random r = new Random(14768);

            int iLimit = 100;
            int jLimit = 100;

            INPUT = new double[iLimit][];
            IDEAL = new double[iLimit][];

            for (int i = 0; i < iLimit; i++)
            {
                INPUT[i] = new double[1];
                IDEAL[i] = new double[1];
                double average = 0;
                double x = (double)i / (double)iLimit;

                //double y = (-7.5 * Math.Pow(x, 4)) + (21.3 * Math.Pow(x, 3)) + (-22.3 * Math.Pow(x, 2)) + (10.4 * x) - 0.8;
                double y = ((Math.Exp(2.0 * (x * 4.0 - 1)) - 1.0) / (Math.Exp(2.0 * (x * 4.0 - 1)) + 1.0)) / 2 + 0.5;

                for (int j = 0; j < jLimit; j++)
                {
                    average += (r.NextDouble() < y) ? 1 : 0;
                }

                INPUT[i][0] = Scale(x);
                IDEAL[i][0] = Scale((double)average / (double)jLimit);
            }
        }

        #endregion
    }
}
