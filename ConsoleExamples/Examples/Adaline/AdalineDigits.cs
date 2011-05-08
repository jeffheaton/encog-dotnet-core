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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.NeuralData;
using Encog.Neural.Data;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Simple;
using Encog.Neural.Networks.Training;
using ConsoleExamples.Examples;
using Encog.MathUtil.Randomize;
using Encog.Engine.Network.Activation;


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
                MLData input = Image2data(DIGITS[i]);

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

        public static MLData Image2data(String[] image)
        {
            MLData result = new BasicMLData(CHAR_WIDTH * CHAR_HEIGHT);

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

            BasicNetwork network = new BasicNetwork();

            ILayer inputLayer = new BasicLayer(new ActivationLinear(), false, inputNeurons);
            ILayer outputLayer = new BasicLayer(new ActivationLinear(), true, outputNeurons);

            network.AddLayer(inputLayer);
            network.AddLayer(outputLayer);
            network.Structure.FinalizeStructure();

            (new RangeRandomizer(-0.5, 0.5)).Randomize(network);

            // train it
            MLDataSet training = GenerateTraining();
            ITrain train = new TrainAdaline(network, training, 0.01);

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

