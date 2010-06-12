// Encog(tm) Artificial Intelligence Framework v2.4
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

#if !SILVERLIGHT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.CL
{
    /// <summary>
    /// Common data held by OpenCL devices and platforms.
    /// </summary>
    public class EncogCLItem
    {
        /// <summary>
        /// Is this device or platform enabled.  Disabling a platform 
        /// will cause its devices to not be used either, regardless of 
        /// their enabled/disabled status.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// The name of this device or platform.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The vendor of this device or platform.
        /// </summary>
        public String Vender { get; set; }
    }
}
#endif