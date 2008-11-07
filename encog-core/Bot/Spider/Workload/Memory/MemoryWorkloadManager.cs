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
using System.Threading;

namespace Encog.Bot.Spider.Workload.Memory
{
    /// <summary>
    /// MemoryWorkloadManager: This class implements a workload
    /// manager that stores the list of URL's in memory. This
    /// workload manager only supports spidering against a single
    /// host.
    /// </summary>
    public class MemoryWorkloadManager : WorkloadManager
    {
        /// <summary>
        /// The current workload, a map between URL and URLStatus
        /// objects.
        /// </summary>
        private Dictionary<Uri, URLStatus> workload = new Dictionary<Uri, URLStatus>();

        /// <summary>
        /// The list of those items, which are already in the
        /// workload, that are waiting for processing.
        /// </summary>
        private Queue<Uri> waiting = new Queue<Uri>();

        /// <summary>
        /// How many URL's are currently being processed.
        /// </summary>
        private int workingCount = 0;

        /// <summary>
        /// Because the MemoryWorkloadManager only supports a
        /// single host, the currentHost is set to the host of the
        /// first URL added.
        /// </summary>
        private String currentHost;

        /// <summary>
        /// Allows other threads to wait for the status of the
        /// workload to change.
        /// </summary>
        private AutoResetEvent workloadEvent = new AutoResetEvent(true);

        /// <summary>
        /// Add the specified URL to the workload.
        /// </summary>
        /// <param name="url">The URL to be added.</param>
        /// <param name="source">The page that contains this URL.</param>
        /// <param name="depth">The depth of this URL.</param>
        /// <returns>True if the URL was added, false otherwise.</returns>
        public bool Add(Uri url, Uri source, int depth)
        {
            if (!Contains(url))
            {
                this.waiting.Enqueue(url);
                SetStatus(url, source, URLStatus.Status.WAITING, depth);
                if (this.currentHost == null)
                {
                    this.currentHost = url.Host.ToLower();
                }
                this.workloadEvent.Set();
                return true;
            }
            return false;

        }

        /// <summary>
        /// Clear the workload.
        /// </summary>
        public void Clear()
        {
            this.workload.Clear();
            this.waiting.Clear();
            this.workingCount = 0;
            this.workloadEvent.Set();
        }

        /// <summary>
        /// Determine if the workload contains the specified URL.
        /// </summary>
        /// <param name="url">The URL to check.</param>
        /// <returns>Returns true if the specified URL is contained in the workload</returns>
        public bool Contains(Uri url)
        {
            return (this.workload.ContainsKey(url));
        }

        /// <summary>
        /// Convert the specified String to a URL. If the string is
        /// too long or has other issues, throw a
        /// WorkloadException.
        /// </summary>
        /// <param name="url">A String to convert into a URL.</param>
        /// <returns>The URL converted.</returns>
        public Uri ConvertURL(String url)
        {
            try
            {
                return new Uri(url);
            }
            catch (UriFormatException e)
            {
                throw new WorkloadException(e);
            }
        }


        /// <summary>
        /// Get the current host.
        /// </summary>
        /// <returns>The current host.</returns>
        public String GetCurrentHost()
        {
            return this.currentHost;
        }


        /// <summary>
        /// Get the depth of the specified URL.
        /// </summary>
        /// <param name="url">The URL to get the depth of.</param>
        /// <returns>The depth of the specified URL.</returns>
        public int GetDepth(Uri url)
        {
            URLStatus s = this.workload[url];
            if (s != null)
            {
                return s.Depth;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// Get the source page that contains the specified URL.
        /// </summary>
        /// <param name="url">The URL to seek the source for.</param>
        /// <returns>The source of the specified URL.</returns>
        public Uri GetSource(Uri url)
        {
            URLStatus s = this.workload[url];
            if (s == null)
            {
                return null;
            }
            else
            {
                return s.Source;
            }
        }

        /// <summary>
        /// Get a new URL to work on. Wait if there are no URL's
        /// currently available. Return null if done with the
        /// current host. The URL being returned will be marked as
        /// in progress.
        /// </summary>
        /// <returns>The next URL to work on.</returns>
        public Uri GetWork()
        {
            Uri url = null;

            if (this.waiting.Count > 0)
            {
                url = this.waiting.Dequeue();
                SetStatus(url, null, URLStatus.Status.WORKING, -1);
                this.workingCount++;
            }
            return url;
        }


        /// <summary>
        /// Setup this workload manager for the specified spider.
        /// This method is not used by the MemoryWorkloadManager.
        /// </summary>
        /// <param name="spider">The spider using this workload manager.</param>
        public void Init(Spider spider)
        {
        }


        /// <summary>
        /// Mark the specified URL as error.
        /// </summary>
        /// <param name="url">The URL that had an error.</param>
        public void MarkError(Uri url)
        {
            this.workingCount--;
            SetStatus(url, null, URLStatus.Status.ERROR, -1);

        }


        /// <summary>
        /// Mark the specified URL as successfully processed.
        /// </summary>
        /// <param name="url">The URL to mark as processed.</param>
        public void MarkProcessed(Uri url)
        {
            this.workingCount--;
            SetStatus(url, null, URLStatus.Status.PROCESSED, -1);
        }

        /// <summary>
        /// Move on to process the next host. This should only be
        /// called after getWork returns null. Because the
        /// MemoryWorkloadManager is single host only, this
        /// function simply returns null.
        /// </summary>
        /// <returns>The name of the next host.</returns>
        public String NextHost()
        {
            return null;
        }


        /// <summary>
        /// Setup the workload so that it can be resumed from where
        /// the last spider left the workload.
        /// </summary>
        public void Resume()
        {
            throw (new WorkloadException(
                "Memory based workload managers can not resume."));
        }


        /// <summary>
        /// If there is currently no work available, then wait
        /// until a new URL has been added to the workload. Because
        /// the MemoryWorkloadManager uses a blocking queue, this
        /// method is not needed. It is implemented to support the
        /// interface.
        /// </summary>
        /// <param name="time">The amount of time to wait.</param>
        public void WaitForWork(int time)
        {
            DateTime start = DateTime.Now;
            while (!WorkloadEmpty() && this.workingCount > 0)
            {
                if (!workloadEvent.WaitOne(1000, false))
                {
                    TimeSpan span = DateTime.Now - start;
                    if (span.TotalSeconds > time)
                        return;
                }

            }
        }

        /// <summary>
        /// Return true if there are no more workload units.
        /// </summary>
        /// <returns>Returns true if there are no more workload units.</returns>
        public bool WorkloadEmpty()
        {
            try
            {
                Monitor.Enter(this);

                if (this.waiting.Count != 0)
                {
                    return false;
                }

                if (this.workingCount < 1)
                    return true;
                else
                    return false;
            }
            finally
            {
                Monitor.Exit(this);
            }
        }

        /// <summary>
        /// Set the source, status and depth for the specified URL.
        /// </summary>
        /// <param name="url">The URL to set.</param>
        /// <param name="source">The source of this URL.</param>
        /// <param name="status"> The status of this URL.</param>
        /// <param name="depth">The depth of this URL.</param>
        private void SetStatus(Uri url, Uri source, URLStatus.Status status, int depth)
        {
            URLStatus s;
            if (!this.workload.ContainsKey(url))
            {
                s = new URLStatus();
                this.workload.Add(url, s);
            }
            else
                s = this.workload[url];

            s.CurrentStatus = status;

            if (source != null)
            {
                s.Source = source;
            }

            if (depth != -1)
            {
                s.Depth = depth;
            }

            workloadEvent.Set();
        }
    }
}
