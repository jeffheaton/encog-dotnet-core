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
#if logging
using log4net;
using Encog.MathUtil;
using Encog.Engine.Util;
using Encog.Engine.Network.RBF;
#endif

namespace Encog.MathUtil.RBF
{
    /// <summary>
    /// Implements a radial function based on the inverse multiquadric function.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class InverseMultiquadricFunction : BasicRBF
    {
        	/**
	 * Create centered at zero, width 0, and peak 0.
	 */
	public InverseMultiquadricFunction(int dimensions) {
		this.Centers = new double[dimensions];
		this.Peak = 1.0;
		this.Width = 1.0;
	}

	/**
	 * Construct a multi-dimension Inverse-Multiquadric function with the
	 * specified peak, centers and widths.
	 * 
	 * @param peak
	 *            The peak for all dimensions.
	 * @param center
	 *            The centers for each dimension.
	 * @param width
	 *            The widths for each dimension.
	 */
	public InverseMultiquadricFunction(double peak,
			double[] center, double width) {
		this.Centers = center;
		this.Peak = peak;
		this.Width = width;
	}

	/**
	 * Construct a single-dimension Inverse-Multiquadric function with the
	 * specified peak, centers and widths.
	 * 
	 * @param peak
	 *            The peak for all dimensions.
	 * @param center
	 *            The centers for each dimension.
	 * @param width
	 *            The widths for each dimension.
	 */
	public InverseMultiquadricFunction(double center, double peak,
			double width) {
		this.Centers = new double[1];
		this.Centers[0] = center;
		this.Peak = peak;
		this.Width = width;
	}

	/**
	 * {@inheritDoc}
	 */
	public override double Calculate( double[] x) {
		double value = 0;
		double[] center = Centers;
		double width = Width;

		for (int i = 0; i < center.Length; i++) {
			value += Math.Pow(x[i] - center[i], 2) + (width * width);
		}
		return this.Peak / BoundMath.Sqrt(value);
	}

    }

}

