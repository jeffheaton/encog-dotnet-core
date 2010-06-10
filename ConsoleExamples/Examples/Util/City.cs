// Encog(tm) Artificial Intelligence Framework v2.3: C# Examples
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

namespace Encog.Examples.Util
{
    /// <summary>
    /// Holds the x and y location for a city in the traveling salesman problem.
    /// </summary>
    public class City
    {
        /// <summary>
        /// The city's x position.
        /// </summary>
        int xpos;

        /// <summary>
        /// The city's y position.
        /// </summary>
        int ypos;

        /// <summary>
        /// The city's x position.
        /// </summary>
        int X
        {
            get
            {
                return this.xpos;
            }
        }

        /// <summary>
        /// The city's y position.
        /// </summary>
        int Y
        {
            get
            {
                return this.ypos;
            }
        }

        /// <summary>
        /// Construct a city.
        /// </summary>
        /// <param name="x">The city's x location.</param>
        /// <param name="y">The city's y location.</param>
        public City(int x, int y)
        {
            this.xpos = x;
            this.ypos = y;
        }

        /// <summary>
        /// Returns how close the city is to another city.
        /// </summary>
        /// <param name="cother">The other city.</param>
        /// <returns>A distance.</returns>
        public int Proximity(City cother)
        {
            return Proximity(cother.X, cother.Y);
        }

        /// <summary>
        /// Returns how far this city is from a a specific point. This method uses
        /// the pythagorean theorum to calculate the distance.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>The distance.</returns>
        int Proximity(int x, int y)
        {
            int xdiff = this.xpos - x;
            int ydiff = this.ypos - y;
            return (int)Math.Sqrt(xdiff * xdiff + ydiff * ydiff);
        }
    }
}
