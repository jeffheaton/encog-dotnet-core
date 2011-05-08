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
using Encog.Neural.Data;

namespace Encog.Neural.Networks
{
    /// <summary>
    /// A mapping between two INeuralData classes.
    /// </summary>
    public class NeuralDataMapping
    {
        /// <summary>
        /// The source.
        /// </summary>
        private INeuralData from;

        /// <summary>
        /// The target.
        /// </summary>
        private INeuralData to;

        /// <summary>
        /// Construct an empty mapping.
        /// </summary>
        public NeuralDataMapping()
        {
            this.from = this.to = null;
        }

        /// <summary>
        /// Construct a neural data mapping.
        /// </summary>
        /// <param name="from">The source.</param>
        /// <param name="to">The target.</param>
        public NeuralDataMapping(INeuralData from, INeuralData to)
        {
            this.from = from;
            this.to = to;
        }

        /// <summary>
        /// The source.
        /// </summary>
        public INeuralData From
        {
            get
            {
                return from;
            }
            set
            {
                this.from = value;
            }
        }

        /// <summary>
        /// The target.
        /// </summary>
        public INeuralData To
        {
            get
            {
                return to;
            }
            set
            {
                this.to = value;
            }
        }

        /// <summary>
        /// Copy from one mapping to another.  Deep copy.
        /// </summary>
        /// <param name="source">The source mapping.</param>
        /// <param name="target">The target mapping.</param>
        public static void Copy(NeuralDataMapping source, NeuralDataMapping target)
        {
            for (int i = 0; i < source.From.Count; i++)
            {
                target.From[i] = source.From[i];
            }

            for (int i = 0; i < source.To.Count; i++)
            {
                target.To[i] = source.To[i];
            }
        }
    }

}
