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
using Encog.Neural.NeuralData;
using Encog.Cloud;

namespace Encog.Neural.Networks.Training
{
    /// <summary>
    /// Interface for all neural network training methods. This allows the training
    /// methods to be largely interchangeable. Though some training methods require
    /// specific types of neural network structure.
    /// </summary>
    public interface ITrain
    {
        /// <summary>
        /// Training strategies can be added to improve the training results. There
        /// are a number to choose from, and several can be used at once.
        /// </summary>
        /// <param name="strategy">The strategy to add.</param>
        void AddStrategy(IStrategy strategy);

        /// <summary>
        /// Get the current error percent from the training. You can also set the current error rate. 
        /// This is usually used by training strategies.
        /// </summary>
        double Error
        {
            get;
            set;
        }

        /// <summary>
        /// Get the current best network from the training.
        /// </summary>
        BasicNetwork Network
        {
            get;
        }

        /// <summary>
        /// The strategies to use.
        /// </summary>
        IList<IStrategy> Strategies
        {
            get;
        }

        /// <summary>
        /// The training data to use.
        /// </summary>
        INeuralDataSet Training
        {
            get;
        }

        /// <summary>
        /// Perform one iteration of training.
        /// </summary>
        void Iteration();

#if !SILVERLIGHT
        /// <summary>
        /// The Encog cloud to use.
        /// </summary>
        EncogCloud Cloud { get; set; }
#endif

        /// <summary>
        /// Should be called once training is complete and no more iterations are
        /// needed. Calling iteration again will simply begin the training again, and
        /// require finishTraining to be called once the new training session is
        /// complete.
        /// 
        /// It is particularly important to call finishTraining for multithreaded
        /// training techniques.
        /// </summary>
        void FinishTraining();

    }

}
