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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleExamples.Examples;
using Encog.ML.Data.Specific;
using Encog.Neural.Networks;
using Encog.Neural.Thermal;
using Encog.Neural.Pattern;

namespace Encog.Examples.Hopfield.Associate
{
    /**
* Simple class to recognize some patterns with a Hopfield Neural Network.
* This is very loosely based on a an example by Karsten Kutza, 
* written in C on 1996-01-30.
* http://www.neural-networks-at-your-fingertips.com/hopfield.html
* 
* I translated it to Java and adapted it to use Encog for neural
* network processing.  I mainly kept the patterns from the 
* original example.
*
*/
    public class HopfieldAssociate: IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(HopfieldAssociate),
                    "hopfield-associate",
                    "Hopfield Associates Patterns",
                    "Simple Hopfield neural network that learns to associate patterns.");
                return info;
            }
        }


        public const int HEIGHT = 10;
        public const int WIDTH = 10;

        private IExampleInterface app;

        /**
         * The neural network will learn these patterns.
         */
        public String[][] PATTERN = { 
        new String[WIDTH] { 
		"O O O O O ",
        " O O O O O",
        "O O O O O ",
        " O O O O O",
        "O O O O O ",
        " O O O O O",
        "O O O O O ",
        " O O O O O",
        "O O O O O ",
        " O O O O O"  },

      new String[WIDTH] { 
        "OO  OO  OO",
        "OO  OO  OO",
        "  OO  OO  ",
        "  OO  OO  ",
        "OO  OO  OO",
        "OO  OO  OO",
        "  OO  OO  ",
        "  OO  OO  ",
        "OO  OO  OO",
        "OO  OO  OO"  },

      new String[WIDTH]  { 
        "OOOOO     ",
        "OOOOO     ",
        "OOOOO     ",
        "OOOOO     ",
        "OOOOO     ",
        "     OOOOO",
        "     OOOOO",
        "     OOOOO",
        "     OOOOO",
        "     OOOOO"  },

      new String[WIDTH] { 
        "O  O  O  O",
        " O  O  O  ",
        "  O  O  O ",
        "O  O  O  O",
        " O  O  O  ",
        "  O  O  O ",
        "O  O  O  O",
        " O  O  O  ",
        "  O  O  O ",
        "O  O  O  O"  },

      new String[WIDTH]  { 
        "OOOOOOOOOO",
        "O        O",
        "O OOOOOO O",
        "O O    O O",
        "O O OO O O",
        "O O OO O O",
        "O O    O O",
        "O OOOOOO O",
        "O        O",
        "OOOOOOOOOO"  } };

        /**
         * The neural network will be tested on these patterns, to see
         * which of the last set they are the closest to.
         */
        public String[][] PATTERN2 = { 
        new String[WIDTH] { 
		"          ",
        "          ",
        "          ",
        "          ",
        "          ",
        " O O O O O",
        "O O O O O ",
        " O O O O O",
        "O O O O O ",
        " O O O O O"  },

        new String[WIDTH] { 
        "OOO O    O",
        " O  OOO OO",
        "  O O OO O",
        " OOO   O  ",
        "OO  O  OOO",
        " O OOO   O",
        "O OO  O  O",
        "   O OOO  ",
        "OO OOO  O ",
        " O  O  OOO"  },

        new String[WIDTH] { 
        "OOOOO     ",
        "O   O OOO ",
        "O   O OOO ",
        "O   O OOO ",
        "OOOOO     ",
        "     OOOOO",
        " OOO O   O",
        " OOO O   O",
        " OOO O   O",
        "     OOOOO"  },

        new String[WIDTH] { 
        "O  OOOO  O",
        "OO  OOOO  ",
        "OOO  OOOO ",
        "OOOO  OOOO",
        " OOOO  OOO",
        "  OOOO  OO",
        "O  OOOO  O",
        "OO  OOOO  ",
        "OOO  OOOO ",
        "OOOO  OOOO"  },

        new String[WIDTH] { 
        "OOOOOOOOOO",
        "O        O",
        "O        O",
        "O        O",
        "O   OO   O",
        "O   OO   O",
        "O        O",
        "O        O",
        "O        O",
        "OOOOOOOOOO"  } };

        public BiPolarMLData ConvertPattern(String[][] data, int index)
        {
            int resultIndex = 0;
            BiPolarMLData result = new BiPolarMLData(WIDTH * HEIGHT);
            for (int row = 0; row < HEIGHT; row++)
            {
                for (int col = 0; col < WIDTH; col++)
                {
                    char ch = data[index][row][col];
                    result.SetBoolean(resultIndex++, ch == 'O');
                }
            }
            return result;
        }

        public void Display(BiPolarMLData pattern1, BiPolarMLData pattern2)
        {
            int index1 = 0;
            int index2 = 0;

            for (int row = 0; row < HEIGHT; row++)
            {
                StringBuilder line = new StringBuilder();

                for (int col = 0; col < WIDTH; col++)
                {
                    if (pattern1.GetBoolean(index1++))
                        line.Append('O');
                    else
                        line.Append(' ');
                }

                line.Append("   ->   ");

                for (int col = 0; col < WIDTH; col++)
                {
                    if (pattern2.GetBoolean(index2++))
                        line.Append('O');
                    else
                        line.Append(' ');
                }

                Console.WriteLine(line.ToString());
            }
        }


        public void Evaluate(HopfieldNetwork hopfield, String[][] pattern)
        {
            
            for (int i = 0; i < pattern.Length; i++)
            {
                BiPolarMLData pattern1 = ConvertPattern(pattern, i);
                hopfield.CurrentState = pattern1;
                int cycles = hopfield.RunUntilStable(100);
                BiPolarMLData pattern2 = (BiPolarMLData)hopfield.CurrentState;
                Console.WriteLine("Cycles until stable(max 100): " + cycles + ", result=");
                Display(pattern1, pattern2);
                Console.WriteLine(@"----------------------");
            }
        }

        public void Execute(IExampleInterface app)
        {
            this.app = app;
            HopfieldPattern pattern = new HopfieldPattern();
            pattern.InputNeurons = WIDTH * HEIGHT;
            HopfieldNetwork hopfield = (HopfieldNetwork)pattern.Generate();            

            for (int i = 0; i < PATTERN.Length; i++)
            {
                hopfield.AddPattern(ConvertPattern(PATTERN, i));
            }

            Evaluate(hopfield, PATTERN);
            Evaluate(hopfield, PATTERN2);
        }
    }
}
