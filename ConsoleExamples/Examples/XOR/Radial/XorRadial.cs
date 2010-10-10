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
using Encog.Util.Logging;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks;
using Encog.Neural.NeuralData;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Activation;
using Encog.Neural.Networks.Synapse;
using Encog.Neural.Networks.Training;
using Encog.Neural.Data;
using ConsoleExamples.Examples;
using Encog.Neural.Networks.Pattern;
using Encog.Util.Simple;

namespace Encog.Examples.XOR.Radial
{
    /// <summary>
    /// XOR: This example is essentially the "Hello World" of neural network
    /// programming.  This example shows how to construct an Encog neural
    /// network to predict the output from the XOR operator.  This example
    /// uses a radial network.
    /// </summary>
    public class XorRadial : IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(XorRadial),
                    "xor-radial",
                    "XOR Operator with a RBF Network",
                    "Use a RBF network to learn the XOR operator.");
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

            RadialBasisPattern pattern = new RadialBasisPattern();
            pattern.InputNeurons = 2;
            pattern.AddHiddenLayer(4);
            pattern.OutputNeurons = 1;
            BasicNetwork network = pattern.Generate();
            RadialBasisFunctionLayer rbfLayer = (RadialBasisFunctionLayer)network.GetLayer(RadialBasisPattern.RBF_LAYER);
            network.Reset();
            rbfLayer.RandomizeRBFCentersAndWidths(0, 1, RBFEnum.Gaussian);


            INeuralDataSet trainingSet = new BasicNeuralDataSet(XOR_INPUT, XOR_IDEAL);

            // train the neural network
            ITrain train = new ResilientPropagation(network, trainingSet);

            EncogUtility.TrainToError(network, trainingSet, 0.01);

            // test the neural network
            EncogUtility.Evaluate(network, trainingSet);
        }
    }
}
