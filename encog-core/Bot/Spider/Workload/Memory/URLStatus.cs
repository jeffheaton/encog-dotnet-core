// Encog Neural Network and Bot Library v1.x (DotNet)
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
using System.Net;

namespace Encog.Bot.Spider.Workload.Memory
{
    /// <summary>
    /// URLStatus: This class holds in memory status information
    /// for URLs. Specifically it holds their processing status,
    /// depth and source URL.
    /// </summary>
    public class URLStatus
    {
        /// <summary>
        /// The current status of this URL.
        /// </summary>
        public Status CurrentStatus
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }

        /// <summary>
        /// The page that this URL was found on.
        /// </summary>
        public Uri Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
            }
        }

        /// <summary>
        /// The depth of this URL from the starting URL.
        /// </summary>
        public int Depth
        {
            get
            {
                return depth;
            }
            set
            {
                depth = value;
            }
        }

        /// <summary>
        /// The values for URL statues.
        /// </summary>
        public enum Status
        {
            /// <summary>
            /// Waiting to be processed.
            /// </summary>
            WAITING, 
            /// <summary>
            /// Successfully processed.
            /// </summary>
            PROCESSED,
            /// <summary>
            /// Unsuccessfully processed.
            /// </summary>
            ERROR,
            /// <summary>
            /// Currently being processed.
            /// </summary>
            WORKING
        };


        private Status status;
        private int depth;
        private Uri source;
    }
}
