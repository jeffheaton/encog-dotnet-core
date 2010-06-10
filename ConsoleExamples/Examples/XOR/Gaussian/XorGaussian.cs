// Encog(tm) Artificial Intelligence Framework v2.3: C# Examples
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
using Encog.Util.Logging;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Activation;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks.Training;
using Encog.Neural.NeuralData;
using Encog.Neural.Data;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using ConsoleExamples.Examples;

namespace Encog.Examples.XOR.Gaussian
{
    public class XorGaussian : IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(XorGaussian),
                    "xor-gaussian",
                    "XOR Operator with Gaussian Activation",
                    "Use a network with a Gaussian activation function to learn the XOR operator.  This will not work very well, it is simply an example using this activation function.");
                return info;
            }
        }

        /// <summary>
        /// Input for the XOR function.
        /// </summary>
        public static double[][] XOR_INPUT ={
            new double[2] { 0.0, 0.0 },
            new double[2] { 1.0, 0.0 },
			new double[2] { 0.0, 1.0 },
            new double[2] { 1.0, 1.0 } };

        /// <summary>
        /// Ideal output for the XOR function.
        /// </summary>
        public static double[][] XOR_IDEAL = {                                              
            new double[1] { 0.0 }, 
            new double[1] { 1.0 }, 
            new double[1] { 1.0 }, 
            new double[1] { 0.0 } };

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">Not used.</param>
        public void Execute(IExampleInterface app)
        {

            Logging.StopConsoleLogging();

            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(new ActivationGaussian(0, 1.0, 0.5), true, 2));
            network.AddLayer(new BasicLayer(new ActivationGaussian(0, 1.0, 0.5), true, 3));
            network.AddLayer(new BasicLayer(new ActivationGaussian(0, 1.0, 0.5), true, 1));
            network.Structure.FinalizeStructure();
            network.Reset();

            INeuralDataSet trainingSet = new BasicNeuralDataSet(XOR_INPUT, XOR_IDEAL);

            // train the neural network
            ITrain train = new ResilientPropagation(network, trainingSet);

            int epoch = 1;

            do
            {
                train.Iteration();
                Console.WriteLine("Epoch #" + epoch + " Error:" + train.Error);
                epoch++;
            } while( (train.Error > 0.001) && epoch<5000 );

            // test the neural network
            Console.WriteLine("Neural Network Results:");
            foreach (INeuralDataPair pair in trainingSet)
            {
                INeuralData output = network.Compute(pair.Input);
                Console.WriteLine(pair.Input[0] + "," + pair.Input[1]
                        + ", actual=" + output[0] + ",ideal=" + pair.Ideal[0]);
            }

        }
    }
}
