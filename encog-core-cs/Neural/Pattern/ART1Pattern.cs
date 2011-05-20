//
// Encog(tm) Core v3.0 - .Net Version
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
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.Neural.ART;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// Pattern to create an ART-1 neural network.
    /// </summary>
    ///
    public class ART1Pattern : NeuralNetworkPattern
    {
        /// <summary>
        /// A parameter for F1 layer.
        /// </summary>
        ///
        private double a1;

        /// <summary>
        /// B parameter for F1 layer.
        /// </summary>
        ///
        private double b1;

        /// <summary>
        /// C parameter for F1 layer.
        /// </summary>
        ///
        private double c1;

        /// <summary>
        /// D parameter for F1 layer.
        /// </summary>
        ///
        private double d1;

        /// <summary>
        /// The number of input neurons.
        /// </summary>
        ///
        private int inputNeurons;

        /// <summary>
        /// L parameter for net.
        /// </summary>
        ///
        private double l;

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        ///
        private int outputNeurons;

        /// <summary>
        /// The vigilance parameter.
        /// </summary>
        ///
        private double vigilance;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public ART1Pattern()
        {
            a1 = 1;
            b1 = 1.5d;
            c1 = 5;
            d1 = 0.9d;
            l = 3;
            vigilance = 0.9d;
        }

        /// <summary>
        /// Set the A1 parameter.
        /// </summary>
        public double A1
        {
            get { return a1; }
            set { a1 = value; }
        }


        /// <summary>
        /// Set the B1 parameter.
        /// </summary>
        public double B1
        {
            get { return b1; }
            set { b1 = value; }
        }


        /// <summary>
        /// Set the C1 parameter.
        /// </summary>
        public double C1
        {
            get { return c1; }
            set { c1 = value; }
        }


        /// <summary>
        /// Set the D1 parameter.
        /// </summary>
        public double D1
        {
            get { return d1; }
            set { d1 = value; }
        }


        /// <summary>
        /// Set the L parameter.
        /// </summary>
        ///
        /// <value>The new value.</value>
        public double L
        {
            get { return l; }            
            set { l = value; }
        }


        /// <summary>
        /// Set the vigilance for the network.
        /// </summary>
        public double Vigilance
        {
            get { return vigilance; }
            set { vigilance = value; }
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// This will fail, hidden layers are not supported for this type of network.
        /// </summary>
        ///
        /// <param name="count">Not used.</param>
        public void AddHiddenLayer(int count)
        {
            throw new PatternError("A ART1 network has no hidden layers.");
        }

        /// <summary>
        /// Clear any properties set for this network.
        /// </summary>
        ///
        public void Clear()
        {
            inputNeurons = 0;
            outputNeurons = 0;
        }

        /// <summary>
        /// Generate the neural network.
        /// </summary>
        ///
        /// <returns>The generated neural network.</returns>
        public MLMethod Generate()
        {
            var art = new ART1(inputNeurons, outputNeurons);
            art.A1 = a1;
            art.B1 = b1;
            art.C1 = c1;
            art.D1 = d1;
            art.L = l;
            art.Vigilance = vigilance;
            return art;
        }


        /// <summary>
        /// This method will throw an error, you can't set the activation function
        /// for an ART1. type network.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set { throw new PatternError("Can't set the activation function for an ART1."); }
        }


        /// <summary>
        /// Set the input neuron (F1 layer) count.
        /// </summary>
        public int InputNeurons
        {
            set { inputNeurons = value; }
        }


        /// <summary>
        /// Set the output neuron (F2 layer) count.
        /// </summary>
        public int OutputNeurons
        {
            set { outputNeurons = value; }
        }

        #endregion
    }
}
