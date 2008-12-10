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
using System.IO;

namespace Encog.Bot.Spider.Filter
{
    /// <summary>
    /// This filter causes the spider so skip URL's from a robots.txt file.
    /// </summary>
    public class RobotsFilter : ISpiderFilter
    {
        /// <summary>
        /// Returns a list of URL's to be excluded.
        /// </summary>
        public List<String> Exclude
        {
            get
            {
                return exclude;
            }
        }

        /// <summary>
        /// The full URL of the robots.txt file.
        /// </summary>
        public Uri RobotURL
        {
            get
            {
                return robotURL;
            }
        }

        /// <summary>
        /// The full URL of the robots.txt file.
        /// </summary>
        private Uri robotURL;

        /// <summary>
        /// A list of full URL's to exclude.
        /// </summary>
        private List<String> exclude = new List<String>();

        /// <summary>
        /// Is the parser active? It can become inactive when
        /// parsing sections of the file for other user agents.
        /// </summary>
        private bool active;

        /// <summary>
        /// The user agent string we are to use. null for default.
        /// </summary>
        private String userAgent;


        /// <summary>
        /// Check to see if the specified URL is to be excluded.
        /// </summary>
        /// <param name="url">The URL to be checked.</param>
        /// <returns>Returns true if the URL should be excluded.</returns>
        public bool IsExcluded(Uri url)
        {
            foreach (String str in this.exclude)
            {
                if (url.PathAndQuery.StartsWith(str))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Called when a new host is to be processed. SpiderFilter
        /// classes can not be shared among hosts.
        /// </summary>
        /// <param name="host">The new host.</param>
        /// <param name="userAgent">The user agent being used by the spider. Leave 
        /// null for default.</param>
        public void NewHost(String host, String userAgent)
        {
            try
            {
                String str;
                this.active = false;
                this.userAgent = userAgent;

                StringBuilder robotStr = new StringBuilder();
                robotStr.Append("http://");
                robotStr.Append(host);
                robotStr.Append("/robots.txt");
                this.robotURL = new Uri(robotStr.ToString());

                WebRequest http = HttpWebRequest.Create(this.robotURL);

                if (userAgent != null)
                {
                    http.Headers.Set("User-Agent", userAgent);
                }

                HttpWebResponse response = (HttpWebResponse)http.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.ASCII);

                exclude.Clear();

                try
                {
                    while ((str = reader.ReadLine()) != null)
                    {

                        LoadLine(str);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            catch (Exception)
            {
                // Site does not have a robots.txt file
                // this is common.
            }
        }

        /// <summary>
        /// Add the specified string to the exclude list.
        /// </summary>
        /// <param name="str">This string to add.  This is the path part of a URL.</param>
        private void Add(String str)
        {
            if (!this.exclude.Contains(str))
            {
                this.exclude.Add(str);
            }
        }

        /// <summary>
        /// Called internally to process each line of the
        /// robots.txt file.
        /// </summary>
        /// <param name="str">The line that was read in.</param>
        private void LoadLine(String str)
        {
            str = str.Trim();
            int i = str.IndexOf(':');

            if ((str.Length == 0) || (str[0] == '#') || (i == -1))
            {
                return;
            }

            String command = str.Substring(0, i);
            String rest = str.Substring(i + 1).Trim();
            if (String.Compare(command, "User-agent", true) == 0)
            {
                this.active = false;
                if (rest.Equals("*"))
                {
                    this.active = true;
                }
                else
                {
                    if ((this.userAgent != null) && String.Compare(rest, this.userAgent, true) == 0)
                    {
                        this.active = true;
                    }
                }
            }
            if (this.active)
            {
                if (String.Compare(command, "disallow", true) == 0)
                {
                    if (rest.Trim().Length > 0)
                    {
                        Uri url = new Uri(this.robotURL, rest);
                        Add(url.PathAndQuery);
                    }
                }
            }
        }
    }
}
