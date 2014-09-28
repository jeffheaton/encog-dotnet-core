//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.IO;
using System.Net;
using Encog.Bot.Browse.Range;
using Encog.Util.HTTP;

#if logging

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
        private WebPage _currentPage;


        /// <summary>
        /// The page currently being browsed.
        /// </summary>
        public WebPage CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; }
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
                if (logger.IsInfoEnabled)
                {
                    logger.Info("Posting a form");
                }
#endif
                Stream istream;
                Stream ostream;
                Uri targetURL;
                WebRequest http = null;

                if (form.Method == Form.FormMethod.Get)
                {
                    ostream = new MemoryStream();
                }
                else
                {
                    http = WebRequest.Create(form.Action.Url);
                    http.Timeout = 30000;
                    http.ContentType = "application/x-www-form-urlencoded";
                    http.Method = "POST";
                    ostream = http.GetRequestStream();
                }

                // add the parameters if present
                var formData = new FormUtility(ostream, null);
                foreach (DocumentRange dr in form.Elements)
                {
                    if (dr is FormElement)
                    {
                        var element = (FormElement) dr;
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
                if (form.Method == Form.FormMethod.Get)
                {
                    String action = form.Action.Url.ToString();
                    ostream.Close();
                    action += "?";
                    action += ostream.ToString();
                    targetURL = new Uri(action);
                    http = WebRequest.Create(targetURL);
                    var response = (HttpWebResponse) http.GetResponse();
                    istream = response.GetResponseStream();
                }
                else
                {
                    targetURL = form.Action.Url;
                    ostream.Close();
                    var response = (HttpWebResponse) http.GetResponse();
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
                if (logger.IsInfoEnabled)
                {
                    logger.Info("Navigating to page:" + url);
                }
#endif
                WebRequest http = WebRequest.Create(url);
                var response = (HttpWebResponse) http.GetResponse();
                Stream istream = response.GetResponseStream();
                Navigate(url, istream);
                istream.Close();
                response.Close();
            }
            catch (IOException e)
            {
#if logging
                if (logger.IsDebugEnabled)
                {
                    logger.Debug("Exception", e);
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
            if (logger.IsInfoEnabled)
            {
                logger.Info("POSTing to page:" + url);
            }
#endif
            var load = new LoadWebPage(url);
            _currentPage = load.Load(istream);
        }
    }
}
