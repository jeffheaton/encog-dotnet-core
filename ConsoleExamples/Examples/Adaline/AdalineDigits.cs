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
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Simple;
using Encog.Neural.Networks.Training;
using ConsoleExamples.Examples;
using Encog.MathUtil.Randomize;
using Encog.Engine.Network.Activation;
using Encog.Neural.Pattern;
using Encog.ML.Train;


namespace Encog.Examples.Adaline
{
    public class AdalineDigits: IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(AdalineDigits),
                    "adalinedigits",
                    "ADALINE Digits",
                    "Simple ADALINE neural network that recognizes the digits.");
                return info;
            }
        }

        public const int CHAR_WIDTH = 5;
        public const int CHAR_HEIGHT = 7;

        public static String[][] DIGITS = { 
      new String[CHAR_HEIGHT] { 
        " OOO ",
        "O   O",
        "O   O",
        "O   O",
        "O   O",
        "O   O",
        " OOO "  },

      new String[CHAR_HEIGHT] {           
        "  O  ",
        " OO  ",
        "O O  ",
        "  O  ",
        "  O  ",
        "  O  ",
        "  O  "  },

      new String[CHAR_HEIGHT] { 
        " OOO ",
        "O   O",
        "    O",
        "   O ",
        "  O  ",
        " O   ",
        "OOOOO"  },

      new String[CHAR_HEIGHT] { 
        " OOO ",
        "O   O",
        "    O",
        " OOO ",
        "    O",
        "O   O",
        " OOO "  },

      new String[CHAR_HEIGHT] { 
        "   O ",
        "  OO ",
        " O O ",
        "O  O ",
        "OOOOO",
        "   O ",
        "   O "  },

      new String[CHAR_HEIGHT] { 
        "OOOOO",
        "O    ",
        "O    ",
        "OOOO ",
        "    O",
        "O   O",
        " OOO "  },

      new String[CHAR_HEIGHT] { 
        " OOO ",
        "O   O",
        "O    ",
        "OOOO ",
        "O   O",
        "O   O",
        " OOO "  },

      new String[CHAR_HEIGHT] {
        "OOOOO",
        "    O",
        "    O",
        "   O ",
        "  O  ",
        " O   ",
        "O    "  },

      new String[CHAR_HEIGHT] { 
        " OOO ",
        "O   O",
        "O   O",
        " OOO ",
        "O   O",
        "O   O",
        " OOO "  },

      new String[CHAR_HEIGHT] { 
        " OOO ",
        "O   O",
        "O   O",
        " OOOO",
        "    O",
        "O   O",
        " OOO "  } };

        public static MLDataSet GenerateTraining()
        {
            MLDataSet result = new BasicMLDataSet();
            for (int i = 0; i < DIGITS.Length; i++)
            {
                BasicMLData ideal = new BasicMLData(DIGITS.Length);

                // setup input
                IMLData input = Image2data(DIGITS[i]);

                // setup ideal
                for (int j = 0; j < DIGITS.Length; j++)
                {
                    if (j == i)
                        ideal[j] = 1;
                    else
                        ideal[j] = -1;
                }

                // add training element
                result.Add(input, ideal);
            }
            return result;
        }

        public static IMLData Image2data(String[] image)
        {
            IMLData result = new BasicMLData(CHAR_WIDTH * CHAR_HEIGHT);

            for (int row = 0; row < CHAR_HEIGHT; row++)
            {
                for (int col = 0; col < CHAR_WIDTH; col++)
                {
                    int index = (row * CHAR_WIDTH) + col;
                    char ch = image[row][col];
                    result[index] = (ch == 'O' ? 1 : -1);
                }
            }

            return result;
        }

        public void Execute(IExampleInterface app)
        {
            int inputNeurons = CHAR_WIDTH * CHAR_HEIGHT;
            int outputNeurons = DIGITS.Length;

            ADALINEPattern pattern = new ADALINEPattern();
            pattern.InputNeurons = inputNeurons;
            pattern.OutputNeurons = outputNeurons;
            BasicNetwork network = (BasicNetwork)pattern.Generate();

            (new RangeRandomizer(-0.5, 0.5)).Randomize(network);

            // train it
            MLDataSet training = GenerateTraining();
            MLTrain train = new TrainAdaline(network, training, 0.01);

            int epoch = 1;
            do
            {
                train.Iteration();
                app.WriteLine("Epoch #" + epoch + " Error:" + train.Error);
                epoch++;
            } while (train.Error > 0.01);

            //
            app.WriteLine("Error:" + network.CalculateError(training));

            // test it
            for (int i = 0; i < DIGITS.Length; i++)
            {
                int output = network.Winner(Image2data(DIGITS[i]));

                for (int j = 0; j < CHAR_HEIGHT; j++)
                {
                    if (j == CHAR_HEIGHT - 1)
                        app.WriteLine(DIGITS[i][j] + " -> " + output);
                    else
                        app.WriteLine(DIGITS[i][j]);

                }

                app.WriteLine();
            }
        }
    }
    }

