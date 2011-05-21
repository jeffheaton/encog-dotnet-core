//
// Encog(tm) Console Examples v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
using ConsoleExamples.Examples;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.Neural.CPN;
using Encog.Neural.CPN.Training;
using Encog.Neural.Pattern;

namespace Encog.Examples.CPN
{
    public class RocketCPN : IExample
    {
        public const int WIDTH = 11;
        public const int HEIGHT = 11;

        public const double HI = 1;
        public const double LO = 0;

        private readonly String[][] PATTERN2 = {
                                                   new String[WIDTH]
                                                       {
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
                                                           "           "
                                                       },
                                                   new String[WIDTH]
                                                       {
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
                                                           "           "
                                                       },
                                                   new String[WIDTH]
                                                       {
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
                                                           "           "
                                                       },
                                                   new String[WIDTH]
                                                       {
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
                                                           "           "
                                                       },
                                                   new String[WIDTH]
                                                       {
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
                                                           "       O   "
                                                       },
                                                   new String[WIDTH]
                                                       {
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
                                                           "           "
                                                       },
                                                   new String[WIDTH]
                                                       {
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
                                                           "           "
                                                       },
                                                   new String[WIDTH]
                                                       {
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
                                                           "           "
                                                       }
                                               };

        public String[][] PATTERN1 = {
                                         new String[WIDTH]
                                             {
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
                                                 "           "
                                             },
                                         new String[WIDTH]
                                             {
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
                                                 "           "
                                             },
                                         new String[WIDTH]
                                             {
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
                                                 "           "
                                             },
                                         new String[WIDTH]
                                             {
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
                                                 "           "
                                             },
                                         new String[WIDTH]
                                             {
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
                                                 "           "
                                             },
                                         new String[WIDTH]
                                             {
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
                                                 "           "
                                             },
                                         new String[WIDTH]
                                             {
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
                                                 "           "
                                             },
                                         new String[WIDTH]
                                             {
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
                                                 "           "
                                             }
                                     };

        private IExampleInterface app;
        private double[][] ideal1;

        private double[][] input1;
        private double[][] input2;

        private int inputNeurons;
        private int instarNeurons;
        private int outstarNeurons;

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof (RocketCPN),
                    "cpn",
                    "Counter Propagation Neural Network (CPN)",
                    "CPN neural network that learns to determine at which angle a rocket is traveling.");
                return info;
            }
        }

        #region IExample Members

        public void Execute(IExampleInterface app)
        {
            this.app = app;
            PrepareInput();
            NormalizeInput();
            CPNNetwork network = CreateNetwork();
            IMLDataSet training = GenerateTraining(input1, ideal1);
            TrainInstar(network, training);
            TrainOutstar(network, training);
            Test(network, PATTERN1, input1);
        }

        #endregion

        public void PrepareInput()
        {
            int n, i, j;

            inputNeurons = WIDTH*HEIGHT;
            instarNeurons = PATTERN1.Length;
            outstarNeurons = 2;

            input1 = new double[PATTERN1.Length][];
            input2 = new double[PATTERN2.Length][];
            ideal1 = new double[PATTERN1.Length][];

            for (n = 0; n < PATTERN1.Length; n++)
            {
                input1[n] = new double[inputNeurons];
                input2[n] = new double[inputNeurons];
                ideal1[n] = new double[instarNeurons];
                for (i = 0; i < HEIGHT; i++)
                {
                    for (j = 0; j < WIDTH; j++)
                    {
                        input1[n][i*WIDTH + j] = (PATTERN1[n][i][j] == 'O') ? HI : LO;
                        input2[n][i*WIDTH + j] = (PATTERN2[n][i][j] == 'O') ? HI : LO;
                    }
                }
            }
            NormalizeInput();
            for (n = 0; n < PATTERN1.Length; n++)
            {
                ideal1[n][0] = Math.Sin(n*0.25*Math.PI);
                ideal1[n][1] = Math.Cos(n*0.25*Math.PI);
            }
        }

        public double Sqr(double x)
        {
            return x*x;
        }


        private void NormalizeInput()
        {
            int n, i;
            double length1, length2;

            for (n = 0; n < PATTERN1.Length; n++)
            {
                length1 = 0;
                length2 = 0;
                for (i = 0; i < inputNeurons; i++)
                {
                    length1 += Sqr(input1[n][i]);
                    length2 += Sqr(input2[n][i]);
                }
                length1 = Math.Sqrt(length1);
                length2 = Math.Sqrt(length2);

                for (i = 0; i < inputNeurons; i++)
                {
                    input1[n][i] /= length1;
                    input2[n][i] /= length2;
                }
            }
        }

        public CPNNetwork CreateNetwork()
        {
            var pattern = new CPNPattern();
            pattern.InputNeurons = inputNeurons;
            pattern.InstarCount = instarNeurons;
            pattern.OutstarCount = outstarNeurons;

            var network = (CPNNetwork) pattern.Generate();
            network.Reset();

            return network;
        }

        public void TrainInstar(CPNNetwork network, IMLDataSet training)
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

        public void TrainOutstar(CPNNetwork network, IMLDataSet training)
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

        public IMLDataSet GenerateTraining(double[][] input, double[][] ideal)
        {
            IMLDataSet result = new BasicMLDataSet(input, ideal);
            return result;
        }

        public double DetermineAngle(IMLData angle)
        {
            double result;

            result = (Math.Atan2(angle[0], angle[1])/Math.PI)*180;
            if (result < 0)
                result += 360;

            return result;
        }

        public void Test(CPNNetwork network, String[][] pattern, double[][] input)
        {
            for (int i = 0; i < pattern.Length; i++)
            {
                IMLData inputData = new BasicMLData(input[i]);
                IMLData outputData = network.Compute(inputData);
                double angle = DetermineAngle(outputData);

                // display image
                for (int j = 0; j < HEIGHT; j++)
                {
                    if (j == HEIGHT - 1)
                        app.WriteLine("[" + pattern[i][j] + "] -> " + ((int) angle) + " deg");
                    else
                        app.WriteLine("[" + pattern[i][j] + "]");
                }

                Console.WriteLine();
            }
        }
    }
}
