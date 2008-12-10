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

namespace Encog.Bot.Spider.Workload
{
    /// <summary>
    /// WorkloadManager: This interface defines a workload
    /// manager. A workload manager handles the lists of URLs
    /// that have been processed, resulted in an error, and 
    /// are waiting to be processed.
    /// </summary>
    public interface IWorkloadManager
    {
        /// <summary>
        /// Add the specified URL to the workload.
        /// </summary>
        /// <param name="url">The URL to be added.</param>
        /// <param name="source">The page that contains this URL.</param>
        /// <param name="depth">The depth of this URL.</param>
        /// <returns>True if the URL was added, false otherwise.</returns>
        bool Add(Uri url, Uri source, int depth);

        /// <summary>
        /// Clear the workload.
        /// </summary>
        void Clear();

        /// <summary>
        /// Determine if the workload contains the specified URL.
        /// </summary>
        /// <param name="url">The URL to check.</param>
        /// <returns>True if the URL is contained in the workload.</returns>
        bool Contains(Uri url);

        /// <summary>
        /// Convert the specified String to a URL. If the string is too long or has other issues, throw a WorkloadException.
        /// </summary>
        /// <param name="url">A String to convert into a URL.</param>
        /// <returns>The URL.</returns>
        Uri ConvertURL(String url);

        /// <summary>
        /// Get the current host.
        /// </summary>
        /// <returns>The current host.</returns>
        String GetCurrentHost();

        /// <summary>
        /// Get the depth of the specified URL.
        /// </summary>
        /// <param name="url">The URL to get the depth of.</param>
        /// <returns>The depth of the specified URL.</returns>
        int GetDepth(Uri url);

        /// <summary>
        /// Get the source page that contains the specified URL.
        /// </summary>
        /// <param name="url">The URL to seek the source for.</param>
        /// <returns>The source of the specified URL</returns>
        Uri GetSource(Uri url);

        /// <summary>
        /// * Get a new URL to work on. Wait if there are no URL's currently available. Return null if done with the current host. The URL being returned will be marked as in progress.
        /// </summary>
        /// <returns>The next URL to work on.</returns>
        Uri GetWork();

        /// <summary>
        /// Setup this workload manager for the specified spider.
        /// </summary>
        /// <param name="spider">The spider using this workload manager.</param>
        void Init(Spider spider);

        /// <summary>
        /// Mark the specified URL as error.
        /// </summary>
        /// <param name="url">The URL that had an error.</param>
        void MarkError(Uri url);

        /// <summary>
        /// Mark the specified URL as successfully processed.
        /// </summary>
        /// <param name="url">The URL to mark as processed.</param>
        void MarkProcessed(Uri url);

        /// <summary>
        /// Move on to process the next host. This should only be called after getWork returns null.
        /// </summary>
        /// <returns>The name of the next host.</returns>
        String NextHost();

        /// <summary>
        /// Setup the workload so that it can be resumed from where the last spider left the workload.
        /// </summary>
        void Resume();

        /// <summary>
        /// If there is currently no work available, then wait until a new URL has been added to the workload.
        /// </summary>
        /// <param name="time">The amount of time to wait.</param>
        void WaitForWork(int time);

        /// <summary>
        /// Return true if there are no more workload units.
        /// </summary>
        /// <returns></returns>
        bool WorkloadEmpty();


    }
}
