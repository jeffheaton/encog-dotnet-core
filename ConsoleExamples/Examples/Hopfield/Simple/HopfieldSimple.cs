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
using System.Text;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Specific;
using Encog.Util.Logging;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks;
using ConsoleExamples.Examples;
using Encog.Engine.Network.Activation;
using Encog.Neural.Thermal;

namespace Encog.Examples.Hopfield.Simple
{
    public class HopfieldSimple:IExample
    {
        private IExampleInterface app;

        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(HopfieldSimple),
                    "hopfield-simple",
                    "Hopfield Recognize Simple Patterns",
                    "Teach the Hopfield neural network to recognize a few very simple patterns.");
                return info;
            }
        }


        /// <summary>
        /// Convert a boolean array to the form [T,T,F,F]
        /// </summary>
        /// <param name="b">A boolen array.</param>
        /// <returns>The boolen array in string form.</returns>
        public String FormatBoolean(MLData b)
        {
            StringBuilder result = new StringBuilder();
            result.Append('[');
            for (int i = 0; i < b.Count; i++)
            {
                if (b[i] > 0)
                {
                    result.Append("T");
                }
                else
                {
                    result.Append("F");
                }
                if (i != b.Count - 1)
                {
                    result.Append(",");
                }
            }
            result.Append(']');
            return (result.ToString());
        }


        public void Execute(IExampleInterface app)
        {
            this.app = app;

            // Create the neural network.
            BasicLayer hopfield;
            HopfieldNetwork network = new HopfieldNetwork(4);

            // This pattern will be trained
            bool[] pattern1 = { true, true, false, false };
            // This pattern will be presented
            bool[] pattern2 = { true, false, false, false };
            MLData result;

            BiPolarMLData data1 = new BiPolarMLData(pattern1);
            BiPolarMLData data2 = new BiPolarMLData(pattern2);
            BasicMLDataSet set = new BasicMLDataSet();
            set.Add(data1);

            // train the neural network with pattern1
            app.WriteLine("Training Hopfield network with: "
                    + FormatBoolean(data1));

            network.AddPattern(data1);
            // present pattern1 and see it recognized
            result = network.Compute(data1);
            app.WriteLine("Presenting pattern:" + FormatBoolean(data1)
                    + ", and got " + FormatBoolean(result));
            // Present pattern2, which is similar to pattern 1. Pattern 1
            // should be recalled.
            result = network.Compute(data2);
            app.WriteLine("Presenting pattern:" + FormatBoolean(data2)
                    + ", and got " + FormatBoolean(result));
        }
    }
}
