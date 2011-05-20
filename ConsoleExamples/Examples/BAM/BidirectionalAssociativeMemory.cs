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
using Encog.ML.Data.Specific;
using Encog.Neural.Networks;
using ConsoleExamples.Examples;
using Encog.Examples.Adaline;
using Encog.MathUtil;
using Encog.Neural.Pattern;
using Encog.Neural.BAM;

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

        public BiPolarMLData StringToBipolar(String str)
        {
            BiPolarMLData result = new BiPolarMLData(str.Length * BITS_PER_CHAR);
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

        public String BipolalToString(BiPolarMLData data)
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

        public BiPolarMLData RandomBiPolar(int size)
        {
            BiPolarMLData result = new BiPolarMLData(size);
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
            result.Append(BipolalToString((BiPolarMLData)mapping.From));
            result.Append(" -> ");
            result.Append(BipolalToString((BiPolarMLData)mapping.To));
            return result.ToString();
        }

        public void RunBAM(BAMNetwork network, NeuralDataMapping data)
        {
            StringBuilder line = new StringBuilder();
            line.Append(MappingToString(data));
            network.Compute(data);
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
            BAMNetwork network = (BAMNetwork)pattern.Generate();

            // train
            for (int i = 0; i < NAMES.Length; i++)
            {
                 network.AddPattern(
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
