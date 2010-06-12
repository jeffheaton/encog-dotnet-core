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

namespace Encog.Solve.Genetic.Genes
{
    /// <summary>
    /// A gene that holds an integer.
    /// </summary>
    public class IntegerGene : BasicGene
    {
        /// <summary>
        /// The value of this gene.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Copy another gene to this one.
        /// </summary>
        /// <param name="gene">The other gene to copy.</param>
        public override void Copy(IGene gene)
        {
            Value = ((IntegerGene)gene).Value;

        }

        /// <summary>
        /// Determine if this gene has the same values as another.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>True if equal.</returns>
        public override bool Equals(Object obj)
        {
            if (obj is IntegerGene)
            {
                return (((IntegerGene)obj).Value == Value);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Generate a hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return this.Value;
        }


        /// <summary>
        /// The gene as a string.
        /// </summary>
        /// <returns>The gene as a string.</returns>
        public override String ToString()
        {
            return "" + Value;
        }
    }
}
