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
using Encog.Neural.Data;
using Encog.Util.Logging;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks;
using Encog.Neural.NeuralData.Bipolar;
using Encog.Neural.Activation;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks.Training.Hopfield;
using ConsoleExamples.Examples;

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
        public String FormatBoolean(INeuralData b)
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
            Logging.StopConsoleLogging();

            // Create the neural network.
            BasicLayer hopfield;
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(hopfield = new BasicLayer(new ActivationBiPolar(), false, 4));
            hopfield.AddNext(hopfield);
            // This pattern will be trained
            bool[] pattern1 = { true, true, false, false };
            // This pattern will be presented
            bool[] pattern2 = { true, false, false, false };
            INeuralData result;

            BiPolarNeuralData data1 = new BiPolarNeuralData(pattern1);
            BiPolarNeuralData data2 = new BiPolarNeuralData(pattern2);
            BasicNeuralDataSet set = new BasicNeuralDataSet();
            set.Add(data1);
            network.Structure.FinalizeStructure();

            // train the neural network with pattern1
            app.WriteLine("Training Hopfield network with: "
                    + FormatBoolean(data1));

            TrainHopfield train = new TrainHopfield(set, network);
            train.Iteration();
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
