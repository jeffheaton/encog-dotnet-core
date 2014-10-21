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
using System;
using Encog.MathUtil.Matrices;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Specific;

namespace Encog.Neural.ART
{
    /// <summary>
    /// Implements an ART1 neural network. An ART1 neural network is trained to
    /// recognize bipolar patterns as it is presented data. There is no distinct
    /// learning phase, like there is with other neural network types.
    /// The ART1 neural network is a type of Adaptive Resonance Theory (ART) neural 
    /// network. ART1 was developed by Stephen Grossberg and Gail Carpenter. 
    /// This neural network type supports only bipolar input. The ART1 neural 
    /// network is trained as it is used. New patterns are presented to the ART1 
    /// network, and they are classified into either new, or existing, classes. 
    /// Once the maximum number of classes have been used the network will report 
    /// that it is out of classes. ART1 neural networks are used for classification. 
    /// There are essentially 2 layers in an ART1 network. The first, named the 
    /// F1 layer, acts as the input. The F1 layer receives bipolar patterns that 
    /// the network is to classify. The F2 layer specifies the maximum number 
    /// classes that the ART1 network can recognize. 
    /// Plasticity is an important part for all Adaptive Resonance Theory (ART) 
    /// neural networks. Unlike most neural networks, ART1 does not have a 
    /// distinct training and usage stage. The ART1 network will learn as it is 
    /// used. 
    /// </summary>
    [Serializable]
    public class ART1 : BasicART, IMLResettable, IMLClassification
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
        /// The F1 layer neuron count.
        /// </summary>
        ///
        private int _f1Count;

        /// <summary>
        /// The F2 layer neuron count.
        /// </summary>
        ///
        private int _f2Count;

        /// <summary>
        /// Allows members of the F2 layer to be inhibited.
        /// </summary>
        [NonSerialized]
        private bool[] _inhibitF2;

        /// <summary>
        /// L parameter for net.
        /// </summary>
        ///
        private double _l;

        /// <summary>
        /// This is the value that is returned if there is no winner.  
        /// This value is generally set to the number of classes, plus 1.
        /// </summary>
        ///
        private int _noWinner;

        /// <summary>
        /// The output from the F1 layer.
        /// </summary>
        ///
        private BiPolarMLData _outputF1;

        /// <summary>
        /// The output from the F2 layer.
        /// </summary>
        ///
        private BiPolarMLData _outputF2;

        /// <summary>
        /// The vigilance parameter.
        /// </summary>
        ///
        private double _vigilance;

        /// <summary>
        /// Weights from f1 to f2.
        /// </summary>
        ///
        private Matrix _weightsF1ToF2;

        /// <summary>
        /// Weights from f2 to f1.
        /// </summary>
        ///
        private Matrix _weightsF2ToF1;

        /// <summary>
        /// Default constructor, used mainly for persistence.
        /// </summary>
        ///
        public ART1()
        {
            _a1 = 1;
            _b1 = 1.5d;
            _c1 = 5;
            _d1 = 0.9d;
            _l = 3;
            _vigilance = 0.9d;
        }

        /// <summary>
        /// Construct the ART1 network.
        /// </summary>
        ///
        /// <param name="theF1Count">The neuron count for the f1 layer.</param>
        /// <param name="theF2Count">The neuron count for the f2 layer.</param>
        public ART1(int theF1Count, int theF2Count)
        {
            _a1 = 1;
            _b1 = 1.5d;
            _c1 = 5;
            _d1 = 0.9d;
            _l = 3;
            _vigilance = 0.9d;
            _f1Count = theF1Count;
            _f2Count = theF2Count;

            _weightsF1ToF2 = new Matrix(_f1Count, _f2Count);
            _weightsF2ToF1 = new Matrix(_f2Count, _f1Count);

            _inhibitF2 = new bool[_f2Count];

            _outputF1 = new BiPolarMLData(_f1Count);
            _outputF2 = new BiPolarMLData(_f2Count);

            _noWinner = _f2Count;
            Reset();
        }

        /// <summary>
        /// Set the A1 parameter.
        /// </summary>
        ///
        /// <value>The new value.</value>
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
        ///
        /// <value>The new value.</value>
        public double C1
        {
            get { return _c1; }
            set { _c1 = value; }
        }


        /// <summary>
        /// Set the D1 parameter.
        /// </summary>
        ///
        /// <value>The new value.</value>
        public double D1
        {
            get { return _d1; }
            set { _d1 = value; }
        }


        /// <summary>
        /// Set the F1 count.  The F1 layer is the input layer.
        /// </summary>
        public int F1Count
        {
            get { return _f1Count; }
            set
            {
                _f1Count = value;
                _outputF1 = new BiPolarMLData(_f1Count);
            }
        }


        /// <summary>
        /// Set the F2 count.  The F2 layer is the output layer.
        /// </summary>
        ///
        /// <value>The count.</value>
        public int F2Count
        {
            get { return _f2Count; }
            set
            {
                _f2Count = value;
                _inhibitF2 = new bool[_f2Count];
                _outputF2 = new BiPolarMLData(_f2Count);
            }
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
        /// This is the value that is returned if there is no winner.  
        /// This value is generally set to the index of the last classes, plus 1.
        /// For example, if there were 3 classes, the network would return 0-2 to
        /// represent what class was found, in this case the no winner property
        /// would be set to 3.
        /// </summary>
        public int NoWinner
        {
            get { return _noWinner; }
            set { _noWinner = value; }
        }


        /// <summary>
        /// Set the vigilance.
        /// </summary>
        public double Vigilance
        {
            get { return _vigilance; }
            set { _vigilance = value; }
        }


        /// <summary>
        /// Set the f1 to f2 matrix.
        /// </summary>
        public Matrix WeightsF1ToF2
        {
            get { return _weightsF1ToF2; }
            set { _weightsF1ToF2 = value; }
        }


        /// <summary>
        /// Set the f2 to f1 matrix.
        /// </summary>
        public Matrix WeightsF2ToF1
        {
            get { return _weightsF2ToF1; }
            set { _weightsF2ToF1 = value; }
        }


        /// <value>The winning neuron.</value>
        public int Winner { get; private set; }


        /// <returns>Does this network have a "winner"?</returns>
        public bool HasWinner
        {
            get { return Winner != _noWinner; }
        }

        /// <summary>
        /// Set the input to the neural network.
        /// </summary>
        private BiPolarMLData Input
        {
            set
            {
                for (int i = 0; i < _f1Count; i++)
                {
                    double activation = ((value.GetBoolean(i)) ? 1 : 0)
                                        /(1 + _a1*(((value.GetBoolean(i)) ? 1 : 0) + _b1) + _c1);
                    _outputF1.SetBoolean(i, (activation > 0));
                }
            }
        }

        #region MLClassification Members

        /// <summary>
        /// Classify the input data to a class number.
        /// </summary>
        ///
        /// <param name="input">The input data.</param>
        /// <returns>The class that the data belongs to.</returns>
        public int Classify(IMLData input)
        {
            var input2 = new BiPolarMLData(_f1Count);
            var output = new BiPolarMLData(_f2Count);

            if (input.Count != input2.Count)
            {
                throw new NeuralNetworkError("Input array size does not match.");
            }

            for (int i = 0; i < input2.Count; i++)
            {
                input2.SetBoolean(i, input[i] > 0);
            }

            Compute(input2, output);

            return HasWinner ? Winner : -1;
        }

        /// <summary>
        /// The input count.
        /// </summary>
        public int InputCount
        {
            get { return _f1Count; }
        }

        /// <value>The number of neurons in the output count, which is the f2 layer
        /// count.</value>
        public int OutputCount
        {
            get { return _f2Count; }
        }

        #endregion

        #region MLResettable Members

        /// <summary>
        /// Reset the weight matrix back to starting values.
        /// </summary>
        ///
        public void Reset()
        {
            Reset(0);
        }

        /// <summary>
        /// Reset with a specic seed.
        /// </summary>
        ///
        /// <param name="seed">The seed to reset with.</param>
        public void Reset(int seed)
        {
            for (int i = 0; i < _f1Count; i++)
            {
                for (int j = 0; j < _f2Count; j++)
                {
                    _weightsF1ToF2[i, j] = (_b1 - 1)/_d1 + 0.2d;
                    _weightsF2ToF1[j, i] = _l
                                          /(_l - 1 + _f1Count) - 0.1d;
                }
            }
        }

        #endregion

        /// <summary>
        /// Adjust the weights for the pattern just presented.
        /// </summary>
        ///
        public void AdjustWeights()
        {
            for (int i = 0; i < _f1Count; i++)
            {
                if (_outputF1.GetBoolean(i))
                {
                    double magnitudeInput = Magnitude(_outputF1);
                    _weightsF1ToF2[i, Winner] = 1;
                    _weightsF2ToF1[Winner, i] = _l
                                               /(_l - 1 + magnitudeInput);
                }
                else
                {
                    _weightsF1ToF2[i, Winner] = 0;
                    _weightsF2ToF1[Winner, i] = 0;
                }
            }
        }

        /// <summary>
        /// Compute the output from the ART1 network. This can be called directly or
        /// used by the BasicNetwork class. Both input and output should be bipolar
        /// numbers.
        /// </summary>
        ///
        /// <param name="input">The input to the network.</param>
        /// <param name="output">The output from the network.</param>
        public void Compute(BiPolarMLData input,
                            BiPolarMLData output)
        {
            int i;

            for (i = 0; i < _f2Count; i++)
            {
                _inhibitF2[i] = false;
            }
            bool resonance = false;
            bool exhausted = false;
            do
            {
                Input = input;
                ComputeF2();
                GetOutput(output);
                if (Winner != _noWinner)
                {
                    ComputeF1(input);
                    double magnitudeInput1 = Magnitude(input);
                    double magnitudeInput2 = Magnitude(_outputF1);
                    if ((magnitudeInput2/magnitudeInput1) < _vigilance)
                    {
                        _inhibitF2[Winner] = true;
                    }
                    else
                    {
                        resonance = true;
                    }
                }
                else
                {
                    exhausted = true;
                }
            } while (!(resonance || exhausted));
            if (resonance)
            {
                AdjustWeights();
            }
        }

        /// <summary>
        /// Compute the output for the BasicNetwork class.
        /// </summary>
        ///
        /// <param name="input">The input to the network.</param>
        /// <returns>The output from the network.</returns>
        public IMLData Compute(IMLData input)
        {
            if (!(input is BiPolarMLData))
            {
                throw new NeuralNetworkError(
                    "Input to ART1 logic network must be BiPolarNeuralData.");
            }

            var output = new BiPolarMLData(_f1Count);
            Compute((BiPolarMLData) input, output);
            return output;
        }

        /// <summary>
        /// Compute the output from the F1 layer.
        /// </summary>
        ///
        /// <param name="input">The input to the F1 layer.</param>
        private void ComputeF1(BiPolarMLData input)
        {
            for (int i = 0; i < _f1Count; i++)
            {
                double sum = _weightsF1ToF2[i, Winner]
                             *((_outputF2.GetBoolean(Winner)) ? 1 : 0);
                double activation = (((input.GetBoolean(i)) ? 1 : 0) + _d1*sum - _b1)
                                    /(1 + _a1
                                          *(((input.GetBoolean(i)) ? 1 : 0) + _d1*sum) + _c1);
                _outputF1.SetBoolean(i, activation > 0);
            }
        }

        /// <summary>
        /// Compute the output from the F2 layer.
        /// </summary>
        ///
        private void ComputeF2()
        {
            int i;

            double maxOut = Double.NegativeInfinity;
            Winner = _noWinner;
            for (i = 0; i < _f2Count; i++)
            {
                if (!_inhibitF2[i])
                {
                    double sum = 0;
                    int j;
                    for (j = 0; j < _f1Count; j++)
                    {
                        sum += _weightsF2ToF1[i, j]
                               *((_outputF1.GetBoolean(j)) ? 1 : 0);
                    }
                    if (sum > maxOut)
                    {
                        maxOut = sum;
                        Winner = i;
                    }
                }
                _outputF2.SetBoolean(i, false);
            }
            if (Winner != _noWinner)
            {
                _outputF2.SetBoolean(Winner, true);
            }
        }

        /// <summary>
        /// Copy the output from the network to another object.
        /// </summary>
        ///
        /// <param name="output">The target object for the output from the network.</param>
        private void GetOutput(BiPolarMLData output)
        {
            for (int i = 0; i < _f2Count; i++)
            {
                output.SetBoolean(i, _outputF2.GetBoolean(i));
            }
        }

        /// <summary>
        /// Get the magnitude of the specified input.
        /// </summary>
        ///
        /// <param name="input">The input to calculate the magnitude for.</param>
        /// <returns>The magnitude of the specified pattern.</returns>
        public double Magnitude(BiPolarMLData input)
        {
            double result;

            result = 0;
            for (int i = 0; i < _f1Count; i++)
            {
                result += (input.GetBoolean(i)) ? 1 : 0;
            }
            return result;
        }
    }
}
