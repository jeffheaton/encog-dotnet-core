//
// Encog(tm) Core v3.3 - .Net Version
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
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.Neural.ART;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// Pattern to create an ART-1 neural network.
    /// </summary>
    ///
    public class ART1Pattern : INeuralNetworkPattern
    {
        /// <summary>
        /// A parameter for F1 layer.
        /// </summary>
        ///
        private double _a1;

        /// <summary>
        /// B parameter for F1 layer.
        /// </summary>
        ///
        private double _b1;

        /// <summary>
        /// C parameter for F1 layer.
        /// </summary>
        ///
        private double _c1;

        /// <summary>
        /// D parameter for F1 layer.
        /// </summary>
        ///
        private double _d1;

        /// <summary>
        /// The number of input neurons.
        /// </summary>
        ///
        private int _inputNeurons;

        /// <summary>
        /// L parameter for net.
        /// </summary>
        ///
        private double _l;

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        ///
        private int _outputNeurons;

        /// <summary>
        /// The vigilance parameter.
        /// </summary>
        ///
        private double _vigilance;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public ART1Pattern()
        {
            _a1 = 1;
            _b1 = 1.5d;
            _c1 = 5;
            _d1 = 0.9d;
            _l = 3;
            _vigilance = 0.9d;
        }

        /// <summary>
        /// Set the A1 parameter.
        /// </summary>
        public double A1
        {
            get { return _a1; }
            set { _a1 = value; }
        }


        /// <summary>
        /// Set the B1 parameter.
        /// </summary>
        public double B1
        {
            get { return _b1; }
            set { _b1 = value; }
        }


        /// <summary>
        /// Set the C1 parameter.
        /// </summary>
        public double C1
        {
            get { return _c1; }
            set { _c1 = value; }
        }


        /// <summary>
        /// Set the D1 parameter.
        /// </summary>
        public double D1
        {
            get { return _d1; }
            set { _d1 = value; }
        }


        /// <summary>
        /// Set the L parameter.
        /// </summary>
        ///
        /// <value>The new value.</value>
        public double L
        {
            get { return _l; }            
            set { _l = value; }
        }


        /// <summary>
        /// Set the vigilance for the network.
        /// </summary>
        public double Vigilance
        {
            get { return _vigilance; }
            set { _vigilance = value; }
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
            _inputNeurons = 0;
            _outputNeurons = 0;
        }

        /// <summary>
        /// Generate the neural network.
        /// </summary>
        ///
        /// <returns>The generated neural network.</returns>
        public IMLMethod Generate()
        {
            var art = new ART1(_inputNeurons, _outputNeurons)
                          {
                              A1 = _a1,
                              B1 = _b1,
                              C1 = _c1,
                              D1 = _d1,
                              L = _l,
                              Vigilance = _vigilance
                          };
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
            set { _inputNeurons = value; }
        }


        /// <summary>
        /// Set the output neuron (F2 layer) count.
        /// </summary>
        public int OutputNeurons
        {
            set { _outputNeurons = value; }
        }

        #endregion
    }
}
