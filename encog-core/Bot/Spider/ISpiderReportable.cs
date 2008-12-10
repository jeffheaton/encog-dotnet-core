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
using System.IO;

namespace Encog.Bot.Spider
{
    /// <summary>
    /// The SpiderReportable interface defines how the spider reports
    /// its findings to an outside class.
    /// </summary>
    public interface ISpiderReportable
    {
        /// <summary>
        /// This function is called when the spider is ready to
        /// process a new host.
        /// </summary>
        /// <param name="host">The new host that is about to be processed.</param>
        /// <returns>True if this host should be processed, false otherwise.</returns>
        bool BeginHost(String host);

        /// <summary>
        /// Called when the spider is starting up. This method
        /// provides the SpiderReportable class with the spider
        /// object.
        /// </summary>
        /// <param name="spider">The spider that will be working with this object.</param>
        void Init(Spider spider);

        /// <summary>
        /// Called when the spider encounters a URL.
        /// </summary>
        /// <param name="url">The URL that the spider found.</param>
        /// <param name="source">The page that the URL was found on.</param>
        /// <param name="type">The type of link this URL is.</param>
        /// <returns>True if the spider should scan for links on this page.</returns>
        bool SpiderFoundURL(Uri url, Uri source, Spider.URLType type);

        /// <summary>
        /// Called when the spider is about to process a NON-HTML
        /// URL.
        /// </summary>
        /// <param name="url">The URL that the spider found.</param>
        /// <param name="stream">An InputStream to read the page contents from.</param>
        void SpiderProcessURL(Uri url, Stream stream);

        /// <summary>
        /// Called when the spider is ready to process an HTML
        /// URL.
        /// </summary>
        /// <param name="url">The URL that the spider is about to process.</param>
        /// <param name="parse">An object that will allow you you to parse the
        /// HTML on this page.</param>
        void SpiderProcessURL(Uri url, SpiderParseHTML parse);

        /// <summary>
        /// Called when the spider tries to process a URL but gets
        /// an error.
        /// </summary>
        /// <param name="url">The URL that generated an error.</param>
        void SpiderURLError(Uri url);



    }
}
