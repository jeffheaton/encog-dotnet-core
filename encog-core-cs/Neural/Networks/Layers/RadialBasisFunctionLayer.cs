// Encog(tm) Artificial Intelligence Framework v2.3
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
using Encog.MathUtil.RBF;
using Encog.MathUtil;
using Encog.MathUtil.Randomize;
using Encog.Neural.Data;
using Encog.Neural.Data.Basic;
using Encog.Persist;
using Encog.Persist.Persistors;
using Encog.Engine.Network.Activation;
using Encog.Engine.Util;
using Encog.Engine.Network.RBF;

#if logging
using log4net;
#endif
namespace Encog.Neural.Networks.Layers
{
    /// <summary>
    /// This layer type makes use of several radial basis function to scale the
    /// output from this layer. Each RBF can have a different center, peak, and
    /// width. Proper selection of these values will greatly impact the success of
    /// the layer. Currently, Encog provides no automated way of determining these
    /// values. There is one RBF per neuron.
    /// 
    /// Radial basis function layers have neither thresholds nor a regular activation
    /// function. Calling any methods that deal with the activation function or
    /// thresholds will result in an error.
    /// 
    /// Contributed to Encog By M.Dean and M.Fletcher
    /// University of Cambridge, Dept. of Physics, UK
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class RadialBasisFunctionLayer : BasicLayer
    {

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private static readonly ILog logger = LogManager.GetLogger(typeof(BasicNetwork));
#endif

        /// <summary>
        /// The radial basis functions to use, there should be one for each neuron.
        /// </summary>
        private IRadialBasisFunction[] radialBasisFunction;

        /// <summary>
        /// Default constructor, mainly so the workbench can easily create a default
        /// layer.
        /// </summary>
        public RadialBasisFunctionLayer()
            : this(1)
        {

        }

        /// <summary>
        /// Construct a radial basis function layer.
        /// </summary>
        /// <param name="neuronCount">The neuron count.</param>
        public RadialBasisFunctionLayer(int neuronCount)
            : base(new ActivationLinear(), false, neuronCount)
        {
            this.radialBasisFunction = new IRadialBasisFunction[neuronCount];
        }


        /// <summary>
        /// Compute the values before sending output to the next layer.
        /// This function allows the activation functions to be called.
        /// </summary>
        /// <param name="pattern">The incoming Project.</param>
        /// <returns>The output from this layer.</returns>
        public override INeuralData Compute(INeuralData pattern)
        {
            INeuralData result = new BasicNeuralData(NeuronCount);

            for (int i = 0; i < NeuronCount; i++)
            {

                if (this.radialBasisFunction[i] == null)
                {
                    String str =
               "Error, must define radial functions for each neuron";
#if logging
                    if (RadialBasisFunctionLayer.logger.IsErrorEnabled)
                    {
                        RadialBasisFunctionLayer.logger.Error(str);
                    }
#endif
                    throw new NeuralNetworkError(str);
                }

                IRadialBasisFunction f = this.radialBasisFunction[i];

                if (pattern.Data.Length != f.Dimensions)
                    throw new Exception("Inputs must equal the number of dimensions.");

                result[i] = f.Calculate(pattern.Data);
            }

            return result;
        }

        /// <summary>
        /// Create a persistor for this layer.
        /// </summary>
        /// <returns></returns>
        public override IPersistor CreatePersistor()
        {
            return new RadialBasisFunctionLayerPersistor();
        }

        /// <summary>
        /// An array of radial basis functions.
        /// </summary>
        public IRadialBasisFunction[] RadialBasisFunction
        {
            get
            {
                return this.radialBasisFunction;
            }
            set
            {
                this.radialBasisFunction = value;
            }
        }

        /// <summary>
        /// Set the gausian components to random values.
        /// </summary>
        /// <param name="dimensions">The number of dimensions in the network.</param>
        /// <param name="min">The minimum value for the centers, widths and peaks.</param>
        /// <param name="max">The maximum value for the centers, widths and peaks.</param>
        /// <param name="t">The RBF to use.</param>
        public void RandomizeRBFCentersAndWidths(int dimensions, double min, double max, RBFEnum t)
        {
            double[] centers = new double[dimensions];

            for (int i = 0; i < dimensions; i++)
            {
                centers[i] = RangeRandomizer.Randomize(min, max);
            }

            for (int i = 0; i < this.NeuronCount; i++)
            {
                SetRBFFunction(i, t, centers, RangeRandomizer.Randomize(min, max));
            }
        }

        /// <summary>
        /// Specify specific centers and widths for the provided RBFType
        /// </summary>
        /// <param name="centers">Array containing center position. Row n contains centers for neuron n. Row n contains x elements for x number of dimensions.</param>
        /// <param name="widths">Array containing widths. Row n contains widths for neuron n. Row n contains x elements for x number of dimensions.</param>
        /// <param name="RBFType">The RBF Function to use for this layer.</param>
        public void SetRBFCentersAndWidths(double[][] centers, double[] widths, RBFEnum RBFType)
        {
            for (int i = 0; i < this.NeuronCount; i++)
            {
                SetRBFFunction(i, RBFType, centers[i], widths[i]);
            }
        }

        private void SetRBFFunction(int RBFIndex, RBFEnum RBFType, double[] centers, double width)
        {
            if (RBFType == RBFEnum.Gaussian)
                this.radialBasisFunction[RBFIndex] = new GaussianFunction(0.5, centers, width);
            else if (RBFType == RBFEnum.Multiquadric)
                this.radialBasisFunction[RBFIndex] = new MultiquadricFunction(0.5, centers, width);
            else if (RBFType == RBFEnum.InverseMultiquadric)
                this.radialBasisFunction[RBFIndex] = new InverseMultiquadricFunction(0.5, centers, width);
        }

        /// <summary>
        /// Equally spaces all hidden neurons within the n dimensional variable space.
        /// </summary>
        /// <param name="minPosition">The minimum position neurons should be centered. Typically 0.</param>
        /// <param name="maxPosition">The maximum position neurons should be centered. Typically 1</param>
        /// <param name="RBFType">The RBF type to use.</param>
        /// <param name="dimensions">The number of dimensions.</param>
        /// <param name="volumeNeuronRBFWidth">The neuron width of neurons within the mesh.</param>
        /// <param name="useWideEdgeRBFs">Enables wider RBF's around the boundary of the neuron mesh.</param>
        public void SetRBFCentersAndWidthsEqualSpacing(double minPosition, double maxPosition, RBFEnum RBFType, int dimensions, double volumeNeuronRBFWidth, bool useWideEdgeRBFs)
        {
            int totalNumHiddenNeurons = NeuronCount;

            double disMinMaxPosition = Math.Abs(maxPosition - minPosition);

            //Check to make sure we have the correct number of neurons for the provided dimensions
            int expectedSideLength = (int)Math.Pow(totalNumHiddenNeurons, 1.0 / dimensions);
            if ((double)expectedSideLength != Math.Pow(totalNumHiddenNeurons, 1.0 / dimensions))
                throw new Exception("Total number of RBF neurons must be some integer to the power of 'dimensions'.");

            double edgeNeuronRBFWidth = 2.5 * volumeNeuronRBFWidth;

            double[][] centers = new double[totalNumHiddenNeurons][];
            double[] widths = new double[totalNumHiddenNeurons];

            #region buildCentersAndWidths Volume Neurons
            for (int i = 0; i < totalNumHiddenNeurons; i++)
            {
                centers[i] = new double[dimensions];

                int sideLength = expectedSideLength;

                //Evenly distribute the volume neurons.
                int temp = i;

                //First determine the centers
                for (int j = dimensions; j > 0; j--)
                {
                    //i + j * sidelength + k * sidelength ^2 + ... l * sidelength ^ n
                    //i - neuron number in x direction, i.e. 0,1,2,3
                    //j - neuron number in y direction, i.e. 0,1,2,3
                    //Following example assumes sidelength of 4
                    //e.g Neuron 5 - x position is (int)5/4 * 0.33 = 0.33
                    //then take modulus of 5%4 = 1
                    //Neuron 5 - y position is (int)1/1 * 0.33 = 0.33
                    centers[i][j - 1] = ((int)(temp / Math.Pow(sideLength, j - 1)) * (disMinMaxPosition / (sideLength - 1))) + minPosition;
                    temp = temp % (int)(Math.Pow(sideLength, j - 1));
                }

                //Now set the widths
                if ((centers[i].Contains(1) || centers[i].Contains(0)) && useWideEdgeRBFs)
                {
                    widths[i] = edgeNeuronRBFWidth;
                }
                else
                {
                    widths[i] = volumeNeuronRBFWidth;
                }

                //centers[i] = (double)(1 / (double)(neuronCount - 1)) * (double)i;
            }
            #endregion

            SetRBFCentersAndWidths(centers, widths, RBFType);
            //SaveOutNeuronCentersAndWeights(centers, widths);
        }

        public void SaveOutNeuronCentersAndWeights(double[][] centers, double[][] widths)
        {
            using (var sw = new System.IO.StreamWriter("neuronCentersWeights.csv", false))
            {
                for (int i = 0; i < centers.Length; i++)
                {
                    foreach (double value in centers[i])
                        sw.Write(value + ",");

                    foreach (double value in widths[i])
                        sw.Write(value + ",");

                    sw.WriteLine();
                }
            }
        }
    }
}
