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

#if !SILVERLIGHT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Encog.Bot.Browse.Range;
using System.Net;
using Encog.Util.HTTP;

#if logging
using log4net;
#endif

namespace Encog.Bot.Browse
{
    /// <summary>
    /// The main class for web browsing. This class allows you to navigate to a
    /// specific URL. Once you navigate to one URL, you can naviage to any URL
    /// contained on the page.
    /// </summary>
    public class Browser
    {

        /// <summary>
        /// The page that is currently being browsed.
        /// </summary>
        private WebPage currentPage;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(Browser));
#endif

        /// <summary>
        /// The page currently being browsed.
        /// </summary>
        public WebPage CurrentPage
        {
            get
            {
                return this.currentPage;
            }
            set
            {
                this.currentPage = value;
            }
        }

        /// <summary>
        /// Navigate to the specified form by performing a submit of that form.
        /// </summary>
        /// <param name="form">The form to be submitted.</param>
        public void Navigate(Form form)
        {
            Navigate(form, null);
        }

        /// <summary>
        /// Navigate based on a form. Complete and post the form.
        /// </summary>
        /// <param name="form">The form to be posted.</param>
        /// <param name="submit">The submit button on the form to simulate clicking.</param>
        public void Navigate(Form form, Input submit)
        {

            try
            {
#if logging
                if (this.logger.IsInfoEnabled)
                {
                    this.logger.Info("Posting a form");
                }
#endif
                Stream istream;
                Stream ostream;
                Uri targetURL;
                WebRequest http = null;

                if (form.Method == Form.FormMethod.GET)
                {
                    ostream = new MemoryStream();
                }
                else
                {
                    http = HttpWebRequest.Create(form.Action.Url);
                    http.Timeout = 30000;
                    http.ContentType = "application/x-www-form-urlencoded";
                    http.Method = "POST";
                    ostream = http.GetRequestStream();
                }

                // add the parameters if present
                FormUtility formData = new FormUtility(ostream, null);
                foreach (DocumentRange dr in form.Elements)
                {
                    if (dr is FormElement)
                    {
                        FormElement element = (FormElement)dr;
                        if ((element == submit) || element.AutoSend)
                        {
                            String name = element.Name;
                            String value = element.Value;
                            if (name != null)
                            {
                                if (value == null)
                                {
                                    value = "";
                                }
                                formData.Add(name, value);
                            }
                        }
                    }
                }

                // now execute the command
                if (form.Method == Form.FormMethod.GET)
                {
                    String action = form.Action.Url.ToString();
                    ostream.Close();
                    action += "?";
                    action += ostream.ToString();
                    targetURL = new Uri(action);
                    http = HttpWebRequest.Create(targetURL);
                    HttpWebResponse response = (HttpWebResponse)http.GetResponse();
                    istream = response.GetResponseStream();
                }
                else
                {
                    targetURL = form.Action.Url;
                    ostream.Close();
                    HttpWebResponse response = (HttpWebResponse)http.GetResponse();
                    istream = response.GetResponseStream();
                }

                Navigate(targetURL, istream);
                istream.Close();
            }
            catch (IOException e)
            {
                throw new BrowseError(e);
            }
        }

        /// <summary>
        /// Navigate to a new page based on a link.
        /// </summary>
        /// <param name="link">The link to navigate to.</param>
        public void Navigate(Link link)
        {

            Address address = link.Target;

            if (address.Url != null)
            {
                Navigate(address.Url);
            }
            else
            {
                Navigate(address.Original);
            }

        }

        /// <summary>
        /// Navigate based on a string URL.
        /// </summary>
        /// <param name="url">The URL to navigate to.</param>
        public void Navigate(String url)
        {
            Navigate(new Uri(url));

        }

        /// <summary>
        /// Navigate to a page based on a URL object. This will be an HTTP GET
        /// operation.
        /// </summary>
        /// <param name="url">The URL to navigate to.</param>
        public void Navigate(Uri url)
        {
            try
            {
#if logging
                if (this.logger.IsInfoEnabled)
                {
                    this.logger.Info("Navigating to page:" + url);
                }
#endif
                WebRequest http = HttpWebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)http.GetResponse();
                Stream istream = response.GetResponseStream();
                Navigate(url, istream);
                istream.Close();
                response.Close();
            }
            catch (IOException e)
            {
#if logging
                if (this.logger.IsDebugEnabled)
                {
                    this.logger.Debug("Exception", e);
                }
#endif
                throw new BrowseError(e);
            }

        }

        /// <summary>
        /// Navigate to a page and post the specified data.
        /// </summary>
        /// <param name="url">The URL to post the data to.</param>
        /// <param name="istream">The data to post to the page.</param>
        public void Navigate(Uri url, Stream istream)
        {
#if logging
            if (this.logger.IsInfoEnabled)
            {
                this.logger.Info("POSTing to page:" + url);
            }
#endif
            LoadWebPage load = new LoadWebPage(url);
            this.currentPage = load.Load(istream);
        }

    }
}

#endif
