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
using Encog.Engine.Network.Train.Prop;
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
        /// Construct a Manhattan propagation training object. 
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="profile">The learning rate.</param>
        /// <param name="learnRate">The OpenCL profile to use, null for CPU.</param>
        public ManhattanPropagation(BasicNetwork network,
                 INeuralDataSet training, OpenCLTrainingProfile profile, double learnRate)
            : base(network, training)
        {

            if (profile == null)
            {
                FlatTraining = new TrainFlatNetworkManhattan(
                        network.Structure.Flat,
                        this.Training,
                        learnRate);
            }
#if !SILVERLIGHT
            else
            {
                TrainFlatNetworkOpenCL rpropFlat = new TrainFlatNetworkOpenCL(
                        network.Structure.Flat, this.Training,
                        profile);
                rpropFlat.LearnManhattan(learnRate);
                this.FlatTraining = rpropFlat;
            }
#endif
        }

        /// <summary>
        /// The learning rate, this is value is essentially a percent. It is the
        /// degree to which the gradients are applied to the weight matrix to allow
        /// learning.
        /// </summary>
        public double LearningRate
        {
            get
            {
                return ((TrainFlatNetworkManhattan)this.FlatTraining).LearningRate;
            }
            set
            {
                ((TrainFlatNetworkManhattan)this.FlatTraining).LearningRate = value;
            }
        }

        /// <summary>
        /// Construct a Manhattan propagation training object.  Use the CPU to train. 
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="learnRate">The learning rate.</param>
        public ManhattanPropagation(BasicNetwork network,
                 INeuralDataSet training, double learnRate)
            : this(network, training, null, learnRate)
        {

        }

    }
}
