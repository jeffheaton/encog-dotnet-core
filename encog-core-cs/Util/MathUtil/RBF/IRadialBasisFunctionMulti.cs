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

namespace Encog.Util.MathUtil.RBF
{
    /// <summary>
    /// A multi-dimension RBF.
    /// </summary>
    public interface IRadialBasisFunctionMulti
    {
        /// <summary>
        /// Calculate the RBF result for the specified value.
        /// </summary>
        /// <param name="x">The value to be passed into the RBF.</param>
        /// <returns>The RBF value.</returns>
        double Calculate(double[] x);

        /// <summary>
        /// Get the center of this RBD.
        /// </summary>
        /// <param name="dimension">The dimension to get the center for.</param>
        /// <returns>The center of the RBF.</returns>
        double GetCenter(int dimension);

        /// <summary>
        /// The center of the RBF.
        /// </summary>
        double Peak
        {
            get;
        }

        /// <summary>
        /// Get the center of this RBD.
        /// </summary>
        /// <param name="dimension">The dimension to get the center for.</param>
        /// <returns>The center of the RBF.</returns>
        double GetWidth(int dimension);

        /// <summary>
        /// The dimensions in this RBF.
        /// </summary>
        int Dimensions
        {
            get;
        }

        /// <summary>
        /// Set the width.
        /// </summary>
        /// <param name="radius">The width.</param>
        void SetWidth(double radius);
    }
}
