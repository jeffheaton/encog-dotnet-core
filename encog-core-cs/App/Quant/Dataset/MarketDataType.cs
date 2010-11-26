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

namespace Encog.App.Quant.Dataset
{

        /// <summary>
        /// The types of market data that can be used.
        /// </summary>
        public enum MarketDataType
        {
            /// <summary>
            /// The market open for the day.
            /// </summary>
            OPEN,

            /// <summary>
            /// The market close for the day.
            /// </summary>
            CLOSE,

            /// <summary>
            /// The adjusted market open for the day.
            /// </summary>
            ADJUSTED_OPEN,

            /// <summary>
            /// The adjusted market close for the day.
            /// </summary>
            ADJUSTED_CLOSE,

            /// <summary>
            /// The volume for the day.
            /// </summary>
            VOLUME,

            /// <summary>
            /// The high for the day.
            /// </summary>
            HIGH,

            /// <summary>
            /// The low for the day.
            /// </summary>
            LOW,

            /// <summary>
            /// The high for the day.
            /// </summary>
            ADJUSTED_HIGH,

            /// <summary>
            /// The low for the day.
            /// </summary>
            ADJUSTED_LOW

        }
    
}
