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
using System;
using ConsoleExamples.Examples;
using Encog.ML.Data.Specific;
using Encog.Neural.ART;
using Encog.Neural.Pattern;

namespace Encog.Examples.ARTExample
{
    public class ClassifyART1 : IExample
    {
        public const int INPUT_NEURONS = 5;
        public const int OUTPUT_NEURONS = 10;

        public String[] PATTERN = {
                                      "   O ",
                                      "  O O",
                                      "    O",
                                      "  O O",
                                      "    O",
                                      "  O O",
                                      "    O",
                                      " OO O",
                                      " OO  ",
                                      " OO O",
                                      " OO  ",
                                      "OOO  ",
                                      "OO   ",
                                      "O    ",
                                      "OO   ",
                                      "OOO  ",
                                      "OOOO ",
                                      "OOOOO",
                                      "O    ",
                                      " O   ",
                                      "  O  ",
                                      "   O ",
                                      "    O",
                                      "  O O",
                                      " OO O",
                                      " OO  ",
                                      "OOO  ",
                                      "OO   ",
                                      "OOOO ",
                                      "OOOOO"
                                  };

        private IExampleInterface app;
        private bool[][] input;

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof (ClassifyART1),
                    "art1-classify",
                    "Classify Patterns with ART1",
                    "Uses an ART1 neural network to classify input patterns into groups.  The ART1 network learns these groups as it is presented with items to classify.");
                return info;
            }
        }

        #region IExample Members

        public void Execute(IExampleInterface app)
        {
            this.app = app;
            SetupInput();
            var pattern = new ART1Pattern();
            pattern.InputNeurons = INPUT_NEURONS;
            pattern.OutputNeurons = OUTPUT_NEURONS;
            var network = (ART1) pattern.Generate();


            for (int i = 0; i < PATTERN.Length; i++)
            {
                var dataIn = new BiPolarMLData(input[i]);
                var dataOut = new BiPolarMLData(OUTPUT_NEURONS);
                network.Compute(dataIn, dataOut);
                if (network.HasWinner)
                {
                    app.WriteLine(PATTERN[i] + " - " + network.Winner);
                }
                else
                {
                    app.WriteLine(PATTERN[i] + " - new Input and all Classes exhausted");
                }
            }
        }

        #endregion

        public void SetupInput()
        {
            input = new bool[PATTERN.Length][];
            for (int n = 0; n < PATTERN.Length; n++)
            {
                input[n] = new bool[INPUT_NEURONS];
                for (int i = 0; i < INPUT_NEURONS; i++)
                {
                    input[n][i] = (PATTERN[n][i] == 'O');
                }
            }
        }
    }
}
