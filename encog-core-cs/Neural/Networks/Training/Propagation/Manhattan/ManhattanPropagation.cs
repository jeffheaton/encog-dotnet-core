// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;
#if logging
using log4net;
#endif

namespace Encog.Neural.Networks.Training.Propagation.Manhattan
{
    /// <summary>
    /// One problem that the backpropagation technique has is that the magnitude of
    /// the partial derivative may be calculated too large or too small. The
    /// Manhattan update algorithm attempts to solve this by using the partial
    /// derivative to only indicate the sign of the update to the weight matrix. The
    /// actual amount added or subtracted from the weight matrix is obtained from a
    /// simple constant. This constant must be adjusted based on the type of neural
    /// network being trained. In general, start with a higher constant and decrease
    /// it as needed.
    /// 
    /// The Manhattan update algorithm can be thought of as a simplified version of
    /// the resilient algorithm. The resilient algorithm uses more complex techniques
    /// to determine the update value.
    /// </summary>
    public class ManhattanPropagation : Propagation, ILearningRate
    {

        /// <summary>
        /// The default tolerance to determine of a number is close to zero.
        /// </summary>
        static double DEFAULT_ZERO_TOLERANCE = 0.001;

        /// <summary>
        /// The zero tolearnce to use.
        /// </summary>
        private double zeroTolerance;

        /// <summary>
        /// The learning gate to use.
        /// </summary>
        private double learningRate;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(ManhattanPropagation));
#endif

        /// <summary>
        /// Construct a class to train with Manhattan propagation.  Use default zero 
        /// tolerance.
        /// </summary>
        /// <param name="network">The network that is to be trained.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="learnRate">A fixed learning to the weight matrix for each 
        /// training iteration.</param>
        public ManhattanPropagation(BasicNetwork network,
                 INeuralDataSet training, double learnRate)
            : this(network, training, learnRate,
                ManhattanPropagation.DEFAULT_ZERO_TOLERANCE)
        {

        }

        /// <summary>
        /// Construct a Manhattan propagation training object.  
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="learnRate">The learning rate.s</param>
        /// <param name="zeroTolerance">The zero tolerance.</param>
        public ManhattanPropagation(BasicNetwork network,
                 INeuralDataSet training, double learnRate,
                 double zeroTolerance)
            : base(network, new ManhattanPropagationMethod(zeroTolerance,learnRate), training)
        {
            this.zeroTolerance = zeroTolerance;
            this.learningRate = learnRate;
        }

        /// <summary>
        /// The learning rate that was specified in the
        /// constructor.
        /// </summary>
        public double LearningRate
        {
            get
            {
                return this.learningRate;
            }
            set
            {
                this.learningRate = value;
            }
        }

        /// <summary>
        /// The zero tolerance that was specified in the
        /// constructor.
        /// </summary>
        public double ZeroTolerance
        {
            get
            {
                return this.zeroTolerance;
            }
            set
            {
                this.zeroTolerance = value;
            }
        }
    }
}
