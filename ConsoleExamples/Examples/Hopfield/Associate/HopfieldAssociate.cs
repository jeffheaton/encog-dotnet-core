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
using ConsoleExamples.Examples;
using Encog.ML.Data.Specific;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Logic;
using Encog.Neural.Networks.Pattern;

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

        public BiPolarMlData ConvertPattern(String[][] data, int index)
        {
            int resultIndex = 0;
            BiPolarMlData result = new BiPolarMlData(WIDTH * HEIGHT);
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

        public void Display(BiPolarMlData pattern1, BiPolarMlData pattern2)
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


        public void Evaluate(BasicNetwork hopfield, String[][] pattern)
        {
            HopfieldLogic hopfieldLogic = (HopfieldLogic)hopfield.Logic;
            for (int i = 0; i < pattern.Length; i++)
            {
                BiPolarMlData pattern1 = ConvertPattern(pattern, i);
                hopfieldLogic.CurrentState = pattern1;
                int cycles = hopfieldLogic.RunUntilStable(100);
                BiPolarMlData pattern2 = (BiPolarMlData)hopfieldLogic.CurrentState;
                Console.WriteLine("Cycles until stable(max 100): " + cycles + ", result=");
                Display(pattern1, pattern2);
                Console.WriteLine("----------------------");
            }
        }

        public void Execute(IExampleInterface app)
        {
            this.app = app;
            HopfieldPattern pattern = new HopfieldPattern();
            pattern.InputNeurons = WIDTH * HEIGHT;
            BasicNetwork hopfield = pattern.Generate();
            HopfieldLogic hopfieldLogic = (HopfieldLogic)hopfield.Logic;

            for (int i = 0; i < PATTERN.Length; i++)
            {
                hopfieldLogic.AddPattern(ConvertPattern(PATTERN, i));
            }

            Evaluate(hopfield, PATTERN);
            Evaluate(hopfield, PATTERN2);
        }
    }
}
