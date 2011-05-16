// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using ConsoleExamples.Examples;
using Encog.Neural.Pattern;
using Encog.Neural.CPN;
using Encog.Neural.CPN.Training;
using Encog.ML.Train;
using Encog.Neural.CPN.Training;

namespace Encog.Examples.CPN
{
    public class RocketCPN: IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(RocketCPN),
                    "cpn",
                    "Counter Propagation Neural Network (CPN)",
                    "CPN neural network that learns to determine at which angle a rocket is traveling.");
                return info;
            }
        }

        public const int WIDTH = 11;
        public const int HEIGHT = 11;

        public String[][] PATTERN1 =  { new String[WIDTH]  { 
		"           ",
        "           ",
        "     O     ",
        "     O     ",
        "    OOO    ",
        "    OOO    ",
        "    OOO    ",
        "   OOOOO   ",
        "   OOOOO   ",
        "           ",
        "           "  },

      new String[WIDTH]  { 
        "           ",
        "           ",
        "        O  ",
        "       O   ",
        "     OOO   ",
        "    OOO    ",
        "   OOO     ",
        " OOOOO     ",
        "OOOOO      ",
        "           ",
        "           "  },

      new String[WIDTH]  { 
        "           ",
        "           ",
        "           ",
        "  OO       ",
        "  OOOOO    ",
        "  OOOOOOO  ",
        "  OOOOO    ",
        "  OO       ",
        "           ",
        "           ",
        "           "  },

      new String[WIDTH]  { 
        "           ",
        "           ",
        "OOOOO      ",
        " OOOOO     ",
        "   OOO     ",
        "    OOO    ",
        "     OOO   ",
        "       O   ",
        "        O  ",
        "           ",
        "           "  },

      new String[WIDTH]  { 
        "           ",
        "           ",
        "   OOOOO   ",
        "   OOOOO   ",
        "    OOO    ",
        "    OOO    ",
        "    OOO    ",
        "     O     ",
        "     O     ",
        "           ",
        "           "  },

      new String[WIDTH]  { 
        "           ",
        "           ",
        "      OOOOO",
        "     OOOOO ",
        "     OOO   ",
        "    OOO    ",
        "   OOO     ",
        "   O       ",
        "  O        ",
        "           ",
        "           "  },

      new String[WIDTH]  { 
        "           ",
        "           ",
        "           ",
        "       OO  ",
        "    OOOOO  ",
        "  OOOOOOO  ",
        "    OOOOO  ",
        "       OO  ",
        "           ",
        "           ",
        "           "  },

      new String[WIDTH]  { 
        "           ",
        "           ",
        "  O        ",
        "   O       ",
        "   OOO     ",
        "    OOO    ",
        "     OOO   ",
        "     OOOOO ",
        "      OOOOO",
        "           ",
        "           "  } };

        String[][] PATTERN2 = { 
                          new String[WIDTH]  { 
		"           ",
        "           ",
        "     O     ",
        "     O     ",
        "     O     ",
        "    OOO    ",
        "    OOO    ",
        "    OOO    ",
        "   OOOOO   ",
        "           ",
        "           "  },

      new String[WIDTH]  { 
        "           ",
        "           ",
        "     O     ",
        "     O     ",
        "    O O    ",
        "    O O    ",
        "    O O    ",
        "   O   O   ",
        "   O   O   ",
        "           ",
        "           "  },

      new String[WIDTH]  { 
        "           ",
        "           ",
        "           ",
        "     O     ",
        "    OOO    ",
        "    OOO    ",
        "    OOO    ",
        "   OOOOO   ",
        "           ",
        "           ",
        "           "  },

      new String[WIDTH]  { 
        "           ",
        "           ",
        "           ",
        "           ",
        "     O     ",
        "     O     ",
        "     O     ",
        "    OOO    ",
        "           ",
        "           ",
        "           "  },

      new String[WIDTH]  { 
        "           ",
        "  O        ",
        "     O     ",
        "     O     ",
        "    OOO    ",
        "    OO     ",
        "    OOO   O",
        "    OOOO   ",
        "   OOOOO   ",
        "           ",
        "       O   "  },

      new String[WIDTH]  { 
        "           ",
        "           ",
        "     O     ",
        "     O     ",
        "    OOO    ",
        "    OOO    ",
        "    OOO    ",
        "   OOOOO   ",
        "   OOOOO   ",
        "           ",
        "           "  },

      new String[WIDTH]  { 
        "           ",
        "           ",
        "       O   ",
        "      O    ",
        "    OOO    ",
        "    OOO    ",
        "   OOO     ",
        "  OOOOO    ",
        " OOOOO     ",
        "           ",
        "           "  },

      new String[WIDTH]  { 
        "           ",
        "           ",
        "        O  ",
        "       O   ",
        "     OOO   ",
        "    OOO    ",
        "   OOO     ",
        " OOOOO     ",
        "OOOOO      ",
        "           ",
        "           "  } };

        public const double HI = 1;
        public const double LO = 0;

        private double[][] input1;
        private double[][] input2;
        private double[][] ideal1;

        private int inputNeurons;
        private int instarNeurons;
        private int outstarNeurons;

        private IExampleInterface app;

        public void PrepareInput()
        {
            int n, i, j;

            this.inputNeurons = WIDTH * HEIGHT;
            this.instarNeurons = PATTERN1.Length;
            this.outstarNeurons = 2;

            this.input1 = new double[PATTERN1.Length][];
            this.input2 = new double[PATTERN2.Length][];
            this.ideal1 = new double[PATTERN1.Length][];

            for (n = 0; n < PATTERN1.Length; n++)
            {
                input1[n] = new double[this.inputNeurons];
                input2[n] = new double[this.inputNeurons];
                ideal1[n] = new double[this.instarNeurons];
                for (i = 0; i < HEIGHT; i++)
                {
                    for (j = 0; j < WIDTH; j++)
                    {
                        input1[n][i * WIDTH + j] = (PATTERN1[n][i][j] == 'O') ? HI : LO;
                        input2[n][i * WIDTH + j] = (PATTERN2[n][i][j] == 'O') ? HI : LO;
                    }
                }
            }
            NormalizeInput();
            for (n = 0; n < PATTERN1.Length; n++)
            {
                this.ideal1[n][0] = Math.Sin(n * 0.25 * Math.PI);
                this.ideal1[n][1] = Math.Cos(n * 0.25 * Math.PI);
            }

        }

        public double Sqr(double x)
        {
            return x * x;
        }


        void NormalizeInput()
        {
            int n, i;
            double length1, length2;

            for (n = 0; n < PATTERN1.Length; n++)
            {
                length1 = 0;
                length2 = 0;
                for (i = 0; i < this.inputNeurons; i++)
                {
                    length1 += Sqr(this.input1[n][i]);
                    length2 += Sqr(this.input2[n][i]);
                }
                length1 = Math.Sqrt(length1);
                length2 = Math.Sqrt(length2);

                for (i = 0; i < this.inputNeurons; i++)
                {
                    input1[n][i] /= length1;
                    input2[n][i] /= length2;
                }
            }
        }

        public CPNNetwork CreateNetwork()
        {
            CPNPattern pattern = new CPNPattern();
            pattern.InputNeurons = this.inputNeurons;
            pattern.InstarCount = this.instarNeurons;
            pattern.OutstarCount = this.outstarNeurons;

            CPNNetwork network = (CPNNetwork)pattern.Generate();
            network.Reset();

            return network;
        }

        public void TrainInstar(CPNNetwork network, MLDataSet training)
        {
            int epoch = 1;

            MLTrain train = new TrainInstar(network, training, 0.1, true);
            for (int i = 0; i < 1000; i++)
            {
                train.Iteration();
                app.WriteLine("Training instar, Epoch #" + epoch);
                epoch++;
            }
        }

        public void TrainOutstar(CPNNetwork network, MLDataSet training)
        {
            int epoch = 1;

            MLTrain train = new TrainOutstar(network, training, 0.1);
            for (int i = 0; i < 1000; i++)
            {
                train.Iteration();
                app.WriteLine("Training outstar, Epoch #" + epoch);
                epoch++;
            }
        }

        public MLDataSet GenerateTraining(double[][] input, double[][] ideal)
        {
            MLDataSet result = new BasicMLDataSet(input, ideal);
            return result;
        }

        public double DetermineAngle(MLData angle)
        {
            double result;

            result = (Math.Atan2(angle[0], angle[1]) / Math.PI) * 180;
            if (result < 0)
                result += 360;

            return result;
        }

        public void Test(CPNNetwork network, String[][] pattern, double[][] input)
        {
            for (int i = 0; i < pattern.Length; i++)
            {
                MLData inputData = new BasicMLData(input[i]);
                MLData outputData = network.Compute(inputData);
                double angle = DetermineAngle(outputData);

                // display image
                for (int j = 0; j < HEIGHT; j++)
                {
                    if (j == HEIGHT - 1)
                        app.WriteLine("[" + pattern[i][j] + "] -> " + ((int)angle) + " deg");
                    else
                        app.WriteLine("[" + pattern[i][j] + "]");
                }

                Console.WriteLine();
            }
        }

        public void Execute(IExampleInterface app)
        {
            this.app = app;
            PrepareInput();
            NormalizeInput();
            CPNNetwork network = CreateNetwork();
            MLDataSet training = GenerateTraining(this.input1, this.ideal1);
            TrainInstar(network, training);
            TrainOutstar(network, training);
            Test(network, PATTERN1, this.input1);
        }
    }
}
