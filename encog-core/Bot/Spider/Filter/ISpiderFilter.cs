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

namespace Encog.Bot.Spider.Filter
{
    /// <summary>
    /// SpiderFilter: Filters will cause the spider to skip
    /// URL's.
    /// </summary>
    public interface ISpiderFilter
    {
        /// <summary>
        /// Check to see if the specified URL is to be excluded.
        /// </summary>
        /// <param name="url">The URL to be checked.</param>
        /// <returns>Returns true if the URL should be excluded.</returns>
        bool IsExcluded(Uri url);

        /// <summary>
        /// Called when a new host is to be processed. SpiderFilter
        /// classes can not be shared among hosts.
        /// </summary>
        /// <param name="host">The new host.</param>
        /// <param name="userAgent">The user agent being used by the spider. Leave
        /// null for default.</param>
        void NewHost(String host, String userAgent);
    }
}

