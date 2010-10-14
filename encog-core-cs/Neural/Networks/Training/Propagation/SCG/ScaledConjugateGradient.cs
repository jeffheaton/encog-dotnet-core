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
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Structure;
using Encog.Neural.Networks.Layers;
using Encog.MathUtil;
using Encog.Util;

namespace Encog.Neural.Networks.Training.Propagation.SCG
{
    /// <summary>
    ///  This is a training class that makes use of scaled conjugate 
    /// gradient methods.  It is a very fast and efficient training
    /// algorithm.
    /// </summary>
    public class ScaledConjugateGradient : Propagation
    {
        /// <summary>
        /// The starting value for sigma.
        /// </summary>
        protected const double FIRST_SIGMA = 1E-4;

        /// <summary>
        /// The starting value for lambda.
        /// </summary>
        protected const double FIRST_LAMBDA = 1E-6;

        /// <summary>
        /// Should we restart?
        /// </summary>
        private bool restart;

        /// <summary>
        /// The second lambda value.
        /// </summary>
        private double lambda2;

        /// <summary>
        /// The first lambda value.
        /// </summary>
        private double lambda;

        /// <summary>
        /// The number of iterations.  The network will reset when this value
        /// increases over the number of weights in the network.
        /// </summary>
        private int k;

        /// <summary>
        /// Success?
        /// </summary>
        private bool success = true;

        /// <summary>
        /// The magnitude of p.
        /// </summary>
        private double magP;

        /// <summary>
        /// Step direction vector.
        /// </summary>
        private double[] p;

        /// <summary>
        /// Step direction vector.
        /// </summary>
        private double[] r;

        /// <summary>
        /// The neural network weights.
        /// </summary>
        private double[] weights;

        /// <summary>
        /// The gradients after the training cycles.
        /// </summary>
        private double[] gradient;

        /// <summary>
        /// The current delta.
        /// </summary>
        private double delta;

        /// <summary>
        /// The old error value, used to make sure an improvement happened.
        /// </summary>
        private double oldError;

        /// <summary>
        /// The old weight values, used to restore the neural network.
        /// </summary>
        private double[] oldWeights;

        /// <summary>
        /// The old gradients, used to compare.
        /// </summary>
        private double[] oldGradient;

        /// <summary>
        /// Construct a training class.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data.</param>
        public ScaledConjugateGradient(BasicNetwork network,
                INeuralDataSet training)
            : base(network, training)
        {
        }




    }
}
