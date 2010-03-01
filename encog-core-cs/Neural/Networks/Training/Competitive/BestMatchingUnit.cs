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
using Encog.Neural.Networks.Synapse;
using Encog.Neural.Data;
using Encog.MathUtil.MathUtil;

namespace Encog.Neural.Networks.Training.Competitive
{
    /// <summary>
    /// The "Best Matching Unit" or BMU is a very important concept in the training
    /// for a SOM. The BMU is the output neuron that has weight connections to the
    /// input neurons that most closely match the current input vector. This neuron
    /// (and its "neighborhood") are the neurons that will receive training.
    /// 
    /// This class also tracks the worst distance (of all BMU's). This gives some
    /// indication of how well the network is trained, and thus becomes the "error"
    /// of the entire network.
    /// </summary>
    public class BestMatchingUnit
    {

        /// <summary>
        /// The owner of this class.
        /// </summary>
        private CompetitiveTraining training;

        /// <summary>
        /// What is the worst BMU distance so far, this becomes the error for the
        /// entire SOM.
        /// </summary>
        private double worstDistance;

        /// <summary>
        /// Construct a BestMatchingUnit class.  The training class must be provided.
        /// </summary>
        /// <param name="training">The parent class.</param>
        public BestMatchingUnit(CompetitiveTraining training)
        {
            this.training = training;
        }

        /// <summary>
        /// Calculate the best matching unit (BMU). This is the output neuron that
        /// has the lowest Euclidean distance to the input vector.
        /// </summary>
        /// <param name="synapse">The synapse to calculate for.</param>
        /// <param name="input">The input vector.</param>
        /// <returns>The output neuron number that is the BMU.</returns>
        public int CalculateBMU(ISynapse synapse, INeuralData input)
        {
            int result = 0;

            // Track the lowest distance so far.
            double lowestDistance = double.MaxValue;

            for (int i = 0; i < this.training.OutputNeuronCount; i++)
            {
                double distance = CalculateEuclideanDistance(synapse, input,
                       i);

                // Track the lowest distance, this is the BMU.
                if (distance < lowestDistance)
                {
                    lowestDistance = distance;
                    result = i;
                }
            }

            // Track the worst distance, this is the error for the entire network.
            if (lowestDistance > this.worstDistance)
            {
                this.worstDistance = lowestDistance;
            }

            return result;
        }

        /// <summary>
        /// Calculate the Euclidean distance for the specified output neuron and the
        /// input vector.  This is the square root of the squares of the differences
        /// between the weight and input vectors.
        /// </summary>
        /// <param name="synapse">The synapse to get the weights from.</param>
        /// <param name="input">The input vector.</param>
        /// <param name="outputNeuron">The neuron we are calculating the distance for.</param>
        /// <returns>The Euclidean distance.</returns>
        public double CalculateEuclideanDistance(ISynapse synapse,
                 INeuralData input, int outputNeuron)
        {
            double result = 0;

            // Loop over all input data.
            for (int i = 0; i < input.Count; i++)
            {
                double diff = input[i]
                       - synapse.WeightMatrix[i, outputNeuron];
                result += diff * diff;
            }
            return BoundMath.Sqrt(result);
        }

        /// <summary>
        /// What is the worst BMU distance so far, this becomes the error 
        /// for the entire SOM.
        /// </summary>
        public double WorstDistance
        {
            get
            {
                return this.worstDistance;
            }
        }

        /// <summary>
        /// Reset the "worst distance" back to a minimum value.  This should be
        /// called for each training iteration.
        /// </summary>
        public void Reset()
        {
            this.worstDistance = double.MinValue;
        }
    }

}
