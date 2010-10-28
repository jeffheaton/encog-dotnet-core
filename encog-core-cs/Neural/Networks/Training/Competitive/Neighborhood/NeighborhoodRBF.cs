using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Engine.Network.RBF;
using Encog.Neural.Networks.Layers;
using Encog.MathUtil.RBF;
using Encog.Engine.Util;

namespace Encog.Neural.Networks.Training.Competitive.Neighborhood
{
    /// <summary>
    /// Implements a multi-dimensional RBF neighborhood function.
    /// </summary>
    public class NeighborhoodRBF : INeighborhoodFunction
    {
        /// <summary>
        /// The radial basis function to use.
        /// </summary>
        private IRadialBasisFunction rbf;

        /// <summary>
        /// The size of each dimension.
        /// </summary>
        private int[] size;

        /// <summary>
        /// The displacement of each dimension, when mapping the dimensions
        /// to a 1d array.
        /// </summary>
        private int[] displacement;
        
        /// <summary>
        /// Construct a 2d neighborhood function based on the sizes for the
        /// x and y dimensions. 
        /// </summary>
        /// <param name="type">The RBF type to use.</param>
        /// <param name="x">The size of the x-dimension.</param>
        /// <param name="y">The size of the y-dimension.</param>
        public NeighborhoodRBF(RBFEnum type,
                 int x, int y)
        {
            int[] size = new int[2];
            size[0] = x;
            size[1] = y;

            double[] centerArray = new double[2];
            centerArray[0] = 0;
            centerArray[1] = 0;

            double[] widthArray = new double[2];
            widthArray[0] = 1;
            widthArray[1] = 1;

            switch (type)
            {
                case RBFEnum.Gaussian:
                    this.rbf = new GaussianFunction(2);
                    break;
                case RBFEnum.InverseMultiquadric:
                    this.rbf = new InverseMultiquadricFunction(2);
                    break;
                case RBFEnum.Multiquadric:
                    this.rbf = new MultiquadricFunction(2);
                    break;
                case RBFEnum.MexicanHat:
                    this.rbf = new MexicanHatFunction(2);
                    break;
            }

            this.rbf.Width = 1;
            EngineArray.ArrayCopy(centerArray, this.rbf.Centers);

            this.size = size;

            CalculateDisplacement();
        }
        
        /// <summary>
        /// Construct a multi-dimensional neighborhood function. 
        /// </summary>
        /// <param name="size">The sizes of each dimension.</param>
        /// <param name="rbf">The multi-dimensional RBF to use.</param>
        public NeighborhoodRBF(int[] size,
                IRadialBasisFunction rbf)
        {
            this.rbf = rbf;
            this.size = size;
            CalculateDisplacement();
        }

        /// <summary>
        /// Calculate all of the displacement values.
        /// </summary>
        private void CalculateDisplacement()
        {
            this.displacement = new int[this.size.Length];
            for (int i = 0; i < this.size.Length; i++)
            {
                int value;

                if (i == 0)
                {
                    value = 0;
                }
                else if (i == 1)
                {
                    value = this.size[0];
                }
                else
                {
                    value = this.displacement[i - 1] * this.size[i - 1];
                }

                this.displacement[i] = value;
            }
        }

        /// <summary>
        /// Calculate the value for the multi RBF function. 
        /// </summary>
        /// <param name="currentNeuron">The current neuron.</param>
        /// <param name="bestNeuron">The best neuron.</param>
        /// <returns>A percent that determines the amount of training the current
        /// neuron should get.  Usually 100% when it is the bestNeuron.</returns>
        public double Function(int currentNeuron, int bestNeuron)
        {
            double[] vector = new double[this.displacement.Length];
            int[] vectorCurrent = TranslateCoordinates(currentNeuron);
            int[] vectorBest = TranslateCoordinates(bestNeuron);
            for (int i = 0; i < vectorCurrent.Length; i++)
            {
                vector[i] = vectorCurrent[i] - vectorBest[i];
            }
            return this.rbf.Calculate(vector);

        }

        /// <summary>
        /// The radius.
        /// </summary>
        public double Radius
        {
            get
            {
                return this.rbf.Width;
            }
            set
            {
                this.rbf.Width = value;
            }
        }

        /// <summary>
        /// The RBF to use.
        /// </summary>
        public IRadialBasisFunction RBF
        {
            get
            {
                return this.rbf;
            }
        }


        /// <summary>
        /// Translate the specified index into a set of multi-dimensional
        /// coordinates that represent the same index.  This is how the
        /// multi-dimensional coordinates are translated into a one dimensional
        /// index for the input neurons. 
        /// </summary>
        /// <param name="index">The index to translate.</param>
        /// <returns>The multi-dimensional coordinates.</returns>
        private int[] TranslateCoordinates(int index)
        {
            int[] result = new int[this.displacement.Length];
            int countingIndex = index;

            for (int i = this.displacement.Length - 1; i >= 0; i--)
            {
                int value;
                if (this.displacement[i] > 0)
                {
                    value = countingIndex / this.displacement[i];
                }
                else
                {
                    value = countingIndex;
                }

                countingIndex -= this.displacement[i] * value;
                result[i] = value;

            }

            return result;
        }
    }
}
