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
using Encog.MathUtil.RBF;

namespace Encog.Neural.SOM.Training.Neighborhood
{
    /// <summary>
    /// A neighborhood function based on an RBF function.
    /// </summary>
    ///
    public class NeighborhoodRBF1D : INeighborhoodFunction
    {
        /// <summary>
        /// The radial basis function (RBF) to use to calculate the training falloff
        /// from the best neuron.
        /// </summary>
        ///
        private readonly IRadialBasisFunction _radial;

        /// <summary>
        /// Construct the neighborhood function with the specified radial function.
        /// Generally this will be a Gaussian function but any RBF should do.
        /// </summary>
        ///
        /// <param name="radial">The radial basis function to use.</param>
        public NeighborhoodRBF1D(IRadialBasisFunction radial)
        {
            _radial = radial;
        }

        /// <summary>
        /// Construct a 1d neighborhood function.
        /// </summary>
        ///
        /// <param name="type">The RBF type to use.</param>
        public NeighborhoodRBF1D(RBFEnum type)
        {
            switch (type)
            {
                case RBFEnum.Gaussian:
                    _radial = new GaussianFunction(1);
                    break;
                case RBFEnum.InverseMultiquadric:
                    _radial = new InverseMultiquadricFunction(1);
                    break;
                case RBFEnum.Multiquadric:
                    _radial = new MultiquadricFunction(1);
                    break;
                case RBFEnum.MexicanHat:
                    _radial = new MexicanHatFunction(1);
                    break;
                default:
                    throw new NeuralNetworkError("Unknown RBF type: " + type);
            }

            _radial.Width = 1.0d;
        }

        #region INeighborhoodFunction Members

        /// <summary>
        /// Compute the RBF function.
        /// </summary>
        /// <param name="currentNeuron">The current neuron.</param>
        /// <param name="bestNeuron">The best neuron.</param>
        /// <returns>The distance.</returns>
        public virtual double Function(int currentNeuron, int bestNeuron)
        {
            var d = new double[1];
            d[0] = currentNeuron - bestNeuron;
            return _radial.Calculate(d);
        }

        /// <summary>
        /// Set the radius.
        /// </summary>
        ///
        /// <value>The new radius.</value>
        public virtual double Radius
        {
            get { return _radial.Width; }
            set { _radial.Width = value; }
        }

        #endregion
    }
}
