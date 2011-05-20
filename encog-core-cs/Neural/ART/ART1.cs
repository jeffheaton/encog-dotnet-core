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
    public class ART1 : BasicART, MLResettable, MLClassification
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
        /// The F1 layer neuron count.
        /// </summary>
        ///
        private int f1Count;

        /// <summary>
        /// The F2 layer neuron count.
        /// </summary>
        ///
        private int f2Count;

        /// <summary>
        /// Allows members of the F2 layer to be inhibited.
        /// </summary>
        [NonSerialized]
        private bool[] inhibitF2;

        /// <summary>
        /// L parameter for net.
        /// </summary>
        ///
        private double l;

        /// <summary>
        /// This is the value that is returned if there is no winner.  
        /// This value is generally set to the number of classes, plus 1.
        /// </summary>
        ///
        private int noWinner;

        /// <summary>
        /// The output from the F1 layer.
        /// </summary>
        ///
        private BiPolarMLData outputF1;

        /// <summary>
        /// The output from the F2 layer.
        /// </summary>
        ///
        private BiPolarMLData outputF2;

        /// <summary>
        /// The vigilance parameter.
        /// </summary>
        ///
        private double vigilance;

        /// <summary>
        /// Weights from f1 to f2.
        /// </summary>
        ///
        private Matrix weightsF1toF2;

        /// <summary>
        /// Weights from f2 to f1.
        /// </summary>
        ///
        private Matrix weightsF2toF1;

        /// <summary>
        /// last winner in F2 layer.
        /// </summary>
        ///
        private int winner;

        /// <summary>
        /// Default constructor, used mainly for persistence.
        /// </summary>
        ///
        public ART1()
        {
            a1 = 1;
            b1 = 1.5d;
            c1 = 5;
            d1 = 0.9d;
            l = 3;
            vigilance = 0.9d;
        }

        /// <summary>
        /// Construct the ART1 network.
        /// </summary>
        ///
        /// <param name="theF1Count">The neuron count for the f1 layer.</param>
        /// <param name="theF2Count">The neuron count for the f2 layer.</param>
        public ART1(int theF1Count, int theF2Count)
        {
            a1 = 1;
            b1 = 1.5d;
            c1 = 5;
            d1 = 0.9d;
            l = 3;
            vigilance = 0.9d;
            f1Count = theF1Count;
            f2Count = theF2Count;

            weightsF1toF2 = new Matrix(f1Count, f2Count);
            weightsF2toF1 = new Matrix(f2Count, f1Count);

            inhibitF2 = new bool[f2Count];

            outputF1 = new BiPolarMLData(f1Count);
            outputF2 = new BiPolarMLData(f2Count);

            noWinner = f2Count;
            Reset();
        }

        /// <summary>
        /// Set the A1 parameter.
        /// </summary>
        ///
        /// <value>The new value.</value>
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
        ///
        /// <value>The new value.</value>
        public double C1
        {
            get { return c1; }
            set { c1 = value; }
        }


        /// <summary>
        /// Set the D1 parameter.
        /// </summary>
        ///
        /// <value>The new value.</value>
        public double D1
        {
            get { return d1; }
            set { d1 = value; }
        }


        /// <summary>
        /// Set the F1 count.  The F1 layer is the input layer.
        /// </summary>
        public int F1Count
        {
            get { return f1Count; }
            set
            {
                f1Count = value;
                outputF1 = new BiPolarMLData(f1Count);
            }
        }


        /// <summary>
        /// Set the F2 count.  The F2 layer is the output layer.
        /// </summary>
        ///
        /// <value>The count.</value>
        public int F2Count
        {
            get { return f2Count; }
            set
            {
                f2Count = value;
                inhibitF2 = new bool[f2Count];
                outputF2 = new BiPolarMLData(f2Count);
            }
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
        /// This is the value that is returned if there is no winner.  
        /// This value is generally set to the index of the last classes, plus 1.
        /// For example, if there were 3 classes, the network would return 0-2 to
        /// represent what class was found, in this case the no winner property
        /// would be set to 3.
        /// </summary>
        public int NoWinner
        {
            get { return noWinner; }
            set { noWinner = value; }
        }


        /// <summary>
        /// Set the vigilance.
        /// </summary>
        public double Vigilance
        {
            get { return vigilance; }
            set { vigilance = value; }
        }


        /// <summary>
        /// Set the f1 to f2 matrix.
        /// </summary>
        public Matrix WeightsF1toF2
        {
            get { return weightsF1toF2; }
            set { weightsF1toF2 = value; }
        }


        /// <summary>
        /// Set the f2 to f1 matrix.
        /// </summary>
        public Matrix WeightsF2toF1
        {
            get { return weightsF2toF1; }
            set { weightsF2toF1 = value; }
        }


        /// <value>The winning neuron.</value>
        public int Winner
        {
            get { return winner; }
        }


        /// <returns>Does this network have a "winner"?</returns>
        public bool HasWinner
        {
            get { return winner != noWinner; }
        }

        /// <summary>
        /// Set the input to the neural network.
        /// </summary>
        private BiPolarMLData Input
        {
            set
            {
                double activation;

                for (int i = 0; i < f1Count; i++)
                {
                    activation = ((value.GetBoolean(i)) ? 1 : 0)
                                 /(1 + a1*(((value.GetBoolean(i)) ? 1 : 0) + b1) + c1);
                    outputF1.SetBoolean(i, (activation > 0));
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
        public int Classify(MLData input)
        {
            var input2 = new BiPolarMLData(f1Count);
            var output = new BiPolarMLData(f2Count);

            if (input.Count != input2.Count)
            {
                throw new NeuralNetworkError("Input array size does not match.");
            }

            for (int i = 0; i < input2.Count; i++)
            {
                input2.SetBoolean(i, input[i] > 0);
            }

            Compute(input2, output);

            if (HasWinner)
            {
                return winner;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// The input count.
        /// </summary>
        public int InputCount
        {
            get { return f1Count; }
        }

        /// <value>The number of neurons in the output count, which is the f2 layer
        /// count.</value>
        public int OutputCount
        {
            get { return f2Count; }
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
            for (int i = 0; i < f1Count; i++)
            {
                for (int j = 0; j < f2Count; j++)
                {
                    weightsF1toF2[i, j] = (b1 - 1)/d1 + 0.2d;
                    weightsF2toF1[j, i] = l
                                          /(l - 1 + f1Count) - 0.1d;
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
            double magnitudeInput;

            for (int i = 0; i < f1Count; i++)
            {
                if (outputF1.GetBoolean(i))
                {
                    magnitudeInput = Magnitude(outputF1);
                    weightsF1toF2[i, winner] = 1;
                    weightsF2toF1[winner, i] = l
                                               /(l - 1 + magnitudeInput);
                }
                else
                {
                    weightsF1toF2[i, winner] = 0;
                    weightsF2toF1[winner, i] = 0;
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
            bool resonance, exhausted;
            double magnitudeInput1, magnitudeInput2;

            for (i = 0; i < f2Count; i++)
            {
                inhibitF2[i] = false;
            }
            resonance = false;
            exhausted = false;
            do
            {
                Input = input;
                ComputeF2();
                GetOutput(output);
                if (winner != noWinner)
                {
                    ComputeF1(input);
                    magnitudeInput1 = Magnitude(input);
                    magnitudeInput2 = Magnitude(outputF1);
                    if ((magnitudeInput2/magnitudeInput1) < vigilance)
                    {
                        inhibitF2[winner] = true;
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
        public MLData Compute(MLData input)
        {
            if (!(input is BiPolarMLData))
            {
                throw new NeuralNetworkError(
                    "Input to ART1 logic network must be BiPolarNeuralData.");
            }

            var output = new BiPolarMLData(f1Count);
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
            double sum, activation;

            for (int i = 0; i < f1Count; i++)
            {
                sum = weightsF1toF2[i, winner]
                      *((outputF2.GetBoolean(winner)) ? 1 : 0);
                activation = (((input.GetBoolean(i)) ? 1 : 0) + d1*sum - b1)
                             /(1 + a1
                                   *(((input.GetBoolean(i)) ? 1 : 0) + d1*sum) + c1);
                outputF1.SetBoolean(i, activation > 0);
            }
        }

        /// <summary>
        /// Compute the output from the F2 layer.
        /// </summary>
        ///
        private void ComputeF2()
        {
            int i, j;
            double sum, maxOut;

            maxOut = Double.NegativeInfinity;
            winner = noWinner;
            for (i = 0; i < f2Count; i++)
            {
                if (!inhibitF2[i])
                {
                    sum = 0;
                    for (j = 0; j < f1Count; j++)
                    {
                        sum += weightsF2toF1[i, j]
                               *((outputF1.GetBoolean(j)) ? 1 : 0);
                    }
                    if (sum > maxOut)
                    {
                        maxOut = sum;
                        winner = i;
                    }
                }
                outputF2.SetBoolean(i, false);
            }
            if (winner != noWinner)
            {
                outputF2.SetBoolean(winner, true);
            }
        }

        /// <summary>
        /// Copy the output from the network to another object.
        /// </summary>
        ///
        /// <param name="output">The target object for the output from the network.</param>
        private void GetOutput(BiPolarMLData output)
        {
            for (int i = 0; i < f2Count; i++)
            {
                output.SetBoolean(i, outputF2.GetBoolean(i));
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
            for (int i = 0; i < f1Count; i++)
            {
                result += (input.GetBoolean(i)) ? 1 : 0;
            }
            return result;
        }
    }
}
