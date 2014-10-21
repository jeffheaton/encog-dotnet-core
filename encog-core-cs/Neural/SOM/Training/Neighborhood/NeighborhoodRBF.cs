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
using Encog.Util;

namespace Encog.Neural.SOM.Training.Neighborhood
{
    /// <summary>
    /// Implements a multi-dimensional RBF neighborhood function.  
    /// </summary>
    ///
    public class NeighborhoodRBF : INeighborhoodFunction
    {
        /// <summary>
        /// The radial basis function to use.
        /// </summary>
        ///
        private readonly IRadialBasisFunction _rbf;

        /// <summary>
        /// The size of each dimension.
        /// </summary>
        ///
        private readonly int[] _size;

        /// <summary>
        /// The displacement of each dimension, when mapping the dimensions
        /// to a 1d array.
        /// </summary>
        ///
        private int[] _displacement;

        /// <summary>
        /// Construct a 2d neighborhood function based on the sizes for the
        /// x and y dimensions.
        /// </summary>
        ///
        /// <param name="type">The RBF type to use.</param>
        /// <param name="x">The size of the x-dimension.</param>
        /// <param name="y">The size of the y-dimension.</param>
        public NeighborhoodRBF(RBFEnum type, int x, int y)
        {
            var size = new int[2];
            size[0] = x;
            size[1] = y;

            var centerArray = new double[2];
            centerArray[0] = 0;
            centerArray[1] = 0;

            var widthArray = new double[2];
            widthArray[0] = 1;
            widthArray[1] = 1;

            switch (type)
            {
                case RBFEnum.Gaussian:
                    _rbf = new GaussianFunction(2);
                    break;
                case RBFEnum.InverseMultiquadric:
                    _rbf = new InverseMultiquadricFunction(2);
                    break;
                case RBFEnum.Multiquadric:
                    _rbf = new MultiquadricFunction(2);
                    break;
                case RBFEnum.MexicanHat:
                    _rbf = new MexicanHatFunction(2);
                    break;
            }

            _rbf.Width = 1;
            EngineArray.ArrayCopy(centerArray, _rbf.Centers);

            _size = size;

            CalculateDisplacement();
        }

        /// <summary>
        /// Construct a multi-dimensional neighborhood function.
        /// </summary>
        ///
        /// <param name="size">The sizes of each dimension.</param>
        /// <param name="type">The RBF type to use.</param>
        public NeighborhoodRBF(int[] size, RBFEnum type)
        {
            switch (type)
            {
                case RBFEnum.Gaussian:
                    _rbf = new GaussianFunction(2);
                    break;
                case RBFEnum.InverseMultiquadric:
                    _rbf = new InverseMultiquadricFunction(2);
                    break;
                case RBFEnum.Multiquadric:
                    _rbf = new MultiquadricFunction(2);
                    break;
                case RBFEnum.MexicanHat:
                    _rbf = new MexicanHatFunction(2);
                    break;
            }
            _size = size;
            CalculateDisplacement();
        }

        /// <value>The RBF to use.</value>
        public IRadialBasisFunction RBF
        {
            get { return _rbf; }
        }

        #region INeighborhoodFunction Members

        /// <summary>
        /// Calculate the value for the multi RBF function.
        /// </summary>
        ///
        /// <param name="currentNeuron">The current neuron.</param>
        /// <param name="bestNeuron">The best neuron.</param>
        /// <returns>A percent that determines the amount of training the current
        /// neuron should get.  Usually 100% when it is the bestNeuron.</returns>
        public virtual double Function(int currentNeuron, int bestNeuron)
        {
            var vector = new double[_displacement.Length];
            int[] vectorCurrent = TranslateCoordinates(currentNeuron);
            int[] vectorBest = TranslateCoordinates(bestNeuron);
            for (int i = 0; i < vectorCurrent.Length; i++)
            {
                vector[i] = vectorCurrent[i] - vectorBest[i];
            }
            return _rbf.Calculate(vector);
        }

        /// <summary>
        /// Set the radius.
        /// </summary>
        public virtual double Radius
        {
            get { return _rbf.Width; }
            set { _rbf.Width = value; }
        }

        #endregion

        /// <summary>
        /// Calculate all of the displacement values.
        /// </summary>
        ///
        private void CalculateDisplacement()
        {
            _displacement = new int[_size.Length];
            for (int i = 0; i < _size.Length; i++)
            {
                int v;

                if (i == 0)
                {
                    v = 0;
                }
                else if (i == 1)
                {
                    v = _size[0];
                }
                else
                {
                    v = _displacement[i - 1]*_size[i - 1];
                }

                _displacement[i] = v;
            }
        }


        /// <summary>
        /// Translate the specified index into a set of multi-dimensional
        /// coordinates that represent the same index.  This is how the
        /// multi-dimensional coordinates are translated into a one dimensional
        /// index for the input neurons.
        /// </summary>
        ///
        /// <param name="index">The index to translate.</param>
        /// <returns>The multi-dimensional coordinates.</returns>
        private int[] TranslateCoordinates(int index)
        {
            var result = new int[_displacement.Length];
            int countingIndex = index;

            for (int i = _displacement.Length - 1; i >= 0; i--)
            {
                int v;
                if (_displacement[i] > 0)
                {
                    v = countingIndex/_displacement[i];
                }
                else
                {
                    v = countingIndex;
                }

                countingIndex -= _displacement[i]*v;
                result[i] = v;
            }

            return result;
        }
    }
}
