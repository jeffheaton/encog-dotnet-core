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
using Encog.Neural.NeuralData.Bipolar;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Logic;
using Encog.Neural.Networks.Pattern;
using ConsoleExamples.Examples;
using Encog.Examples.Adaline;
using Encog.MathUtil;

namespace Encog.Examples.BAM
{
    /**
* Simple class to recognize some patterns with a Bidirectional
* Associative Memory (BAM) Neural Network.
* This is very loosely based on a an example by Karsten Kutza, 
* written in C on 1996-01-24.
* http://www.neural-networks-at-your-fingertips.com/bam.html
* 
* I translated it to Java and adapted it to use Encog for neural
* network processing.  I mainly kept the patterns from the 
* original example.
*
*/
    public class BidirectionalAssociativeMemory: IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(BidirectionalAssociativeMemory),
                    "bam",
                    "Bidirectional Associative Memory",
                    "Uses a BAM to associate patterns in a bidirectional way.");
                return info;
            }
        }

        public String[] NAMES = { "TINA ", "ANTJE", "LISA " };

        public String[] NAMES2 = { "TINE ", "ANNJE", "RITA " };

        public String[] PHONES = { "6843726", "8034673", "7260915" };

        public const int IN_CHARS = 5;
        public const int OUT_CHARS = 7;

        public const int BITS_PER_CHAR = 6;
        public const char FIRST_CHAR = ' ';

        public const int INPUT_NEURONS = (IN_CHARS * BITS_PER_CHAR);
        public const int OUTPUT_NEURONS = (OUT_CHARS * BITS_PER_CHAR);

        private IExampleInterface app;

        public BiPolarNeuralData StringToBipolar(String str)
        {
            BiPolarNeuralData result = new BiPolarNeuralData(str.Length * BITS_PER_CHAR);
            int currentIndex = 0;
            for (int i = 0; i < str.Length; i++)
            {
                char ch = char.ToUpper(str[i]);
                int idx = (int)ch - FIRST_CHAR;

                int place = 1;
                for (int j = 0; j < BITS_PER_CHAR; j++)
                {
                    bool value = (idx & place) > 0;
                    result.SetBoolean(currentIndex++, value);
                    place *= 2;
                }

            }
            return result;
        }

        public String BipolalToString(BiPolarNeuralData data)
        {
            StringBuilder result = new StringBuilder();

            int j, a, p;

            for (int i = 0; i < (data.Count / BITS_PER_CHAR); i++)
            {
                a = 0;
                p = 1;
                for (j = 0; j < BITS_PER_CHAR; j++)
                {
                    if (data.GetBoolean(i * BITS_PER_CHAR + j))
                        a += p;

                    p *= 2;
                }
                result.Append((char)(a + FIRST_CHAR));
            }


            return result.ToString();
        }

        public BiPolarNeuralData RandomBiPolar(int size)
        {
            BiPolarNeuralData result = new BiPolarNeuralData(size);
            for (int i = 0; i < size; i++)
            {
                if (ThreadSafeRandom.NextDouble() > 0.5)
                    result[i] = -1;
                else
                    result[i] = 1;
            }
            return result;
        }

        public String MappingToString(NeuralDataMapping mapping)
        {
            StringBuilder result = new StringBuilder();
            result.Append(BipolalToString((BiPolarNeuralData)mapping.From));
            result.Append(" -> ");
            result.Append(BipolalToString((BiPolarNeuralData)mapping.To));
            return result.ToString();
        }

        public void RunBAM(BasicNetwork network, NeuralDataMapping data)
        {
            BAMLogic logic = (BAMLogic)network.Logic;
            StringBuilder line = new StringBuilder();
            line.Append(MappingToString(data));
            logic.Compute(data);
            line.Append("  |  ");
            line.Append(MappingToString(data));
            app.WriteLine(line.ToString());
        }

        public void Execute(IExampleInterface app)
        {
            this.app = app;
            BAMPattern pattern = new BAMPattern();
            pattern.F1Neurons = INPUT_NEURONS;
            pattern.F2Neurons = OUTPUT_NEURONS;
            BasicNetwork network = pattern.Generate();
            BAMLogic logic = (BAMLogic)network.Logic;

            // train
            for (int i = 0; i < NAMES.Length; i++)
            {
                logic.AddPattern(
                        StringToBipolar(NAMES[i]),
                        StringToBipolar(PHONES[i]));
            }

            // test
            for (int i = 0; i < NAMES.Length; i++)
            {
                NeuralDataMapping data = new NeuralDataMapping(
                        StringToBipolar(NAMES[i]),
                        RandomBiPolar(OUT_CHARS * BITS_PER_CHAR));
                RunBAM(network, data);
            }

            app.WriteLine();

            for (int i = 0; i < PHONES.Length; i++)
            {
                NeuralDataMapping data = new NeuralDataMapping(
                        StringToBipolar(PHONES[i]),
                        RandomBiPolar(IN_CHARS * BITS_PER_CHAR));
                RunBAM(network, data);
            }

            app.WriteLine();

            for (int i = 0; i < NAMES.Length; i++)
            {
                NeuralDataMapping data = new NeuralDataMapping(
                        StringToBipolar(NAMES2[i]),
                        RandomBiPolar(OUT_CHARS * BITS_PER_CHAR));
                RunBAM(network, data);
            }


        }
    }
}
