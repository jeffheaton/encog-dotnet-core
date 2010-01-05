// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009-2010, Heaton Research Inc., and individual contributors.
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



    }

}
