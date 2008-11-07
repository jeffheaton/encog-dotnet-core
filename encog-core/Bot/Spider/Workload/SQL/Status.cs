// Encog Neural Network and Bot Library for DotNet v0.5
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2008, Heaton Research Inc., and individual contributors.
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
using System.Text;

namespace Encog.Bot.Spider.Workload.SQL
{
    /// <summary>
    /// Status: This class defines the constant status values for
    /// both the spider_host and spider_workload tables.
    /// </summary>
    class Status
    {
        /// <summary>
        /// The item is waiting to be processed.
        /// </summary>
        public const String STATUS_WAITING = "W";

        /// <summary>
        /// The item was processed, but resulted in an error.
        /// </summary>
        public const String STATUS_ERROR = "E";

        /// <summary>
        /// The item was processed successfully.
        /// </summary>
        public const String STATUS_DONE = "D";

        /// <summary>
        /// The item is currently being processed.
        /// </summary>
        public const String STATUS_PROCESSING = "P";

        /// <summary>
        /// This item should be ignored, only applies to hosts.
        /// </summary>
        public const String STATUS_IGNORE = "I";
    }
}
