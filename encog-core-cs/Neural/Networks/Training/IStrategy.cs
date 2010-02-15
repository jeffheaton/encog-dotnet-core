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

namespace Encog.Neural.Networks.Training
{
    /// <summary>
    /// Training strategies can be added to training algorithms.  Training 
    /// strategies allow different additional logic to be added to an existing
    /// training algorithm.  There are a number of different training strategies
    /// that can perform various tasks, such as adjusting the learning rate or 
    /// momentum, or terminating training when improvement diminishes.  Other 
    /// strategies are provided as well.
    /// </summary>
    public interface IStrategy
    {
        /// <summary>
        /// Initialize this strategy.
        /// </summary>
        /// <param name="train">The training algorithm.</param>
        void Init(ITrain train);

        /// <summary>
        /// Called just before a training iteration.
        /// </summary>
        void PreIteration();

        /// <summary>
        /// Called just after a training iteration.
        /// </summary>
        void PostIteration();

    }
}
