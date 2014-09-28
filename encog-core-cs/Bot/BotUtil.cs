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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Encog.Util.HTTP;
using Encog.Util.File;


namespace Encog.Bot
{
    /// <summary>
    /// Utility class for bots.
    /// </summary>
    public class BotUtil
    {
        /// <summary>
        /// How much data to read at once.
        /// </summary>
        public static int BufferSize = 8192;

        /// <summary>
        /// This method is very useful for grabbing information from a HTML page.
        /// </summary>
        /// <param name="str">The string to search.</param>
        /// <param name="token1">The text, or tag, that comes before the desired text</param>
        /// <param name="token2">The text, or tag, that comes after the desired text</param>
        /// <param name="index">Index in the string to start searching from.</param>
        /// <param name="occurence">What occurence.</param>
        /// <returns>The contents of the URL that was downloaded.</returns>
        public static String ExtractFromIndex(String str, String token1,
                                              String token2, int index, int occurence)
        {
            // convert everything to lower case
            String searchStr = str.ToLower();
            String token1Lower = token1.ToLower();
            String token2Lower = token2.ToLower();

            int count = occurence;

            // now search
            int location1 = index - 1;
            do
            {
                location1 = searchStr.IndexOf(token1Lower, location1 + 1);

                if (location1 == -1)
                {
                    return null;
                }

                count--;
            } while (count > 0);


            // return the result from the original string that has mixed
            // case
            int location2 = searchStr.IndexOf(token2Lower, location1 + 1);
            if (location2 == -1)
            {
                return null;
            }

            return str.Substring(location1 + token1Lower.Length, location2 - (location1 + token1.Length));
        }

        /// <summary>
        /// This method is very useful for grabbing information from a HTML page.
        /// </summary>
        /// <param name="str">The string to search.</param>
        /// <param name="token1">The text, or tag, that comes before the desired text.</param>
        /// <param name="token2">The text, or tag, that comes after the desired text.</param>
        /// <param name="index">Which occurrence of token1 to use, 1 for the first.</param>
        /// <returns>The contents of the URL that was downloaded.</returns>
        public static String Extract(String str, String token1,
                                     String token2, int index)
        {
            // convert everything to lower case
            String searchStr = str.ToLower();
            String token1Lower = token1.ToLower();
            String token2Lower = token2.ToLower();

            int count = index;

            // now search
            int location1 = -1;
            do
            {
                location1 = searchStr.IndexOf(token1Lower, location1 + 1);

                if (location1 == -1)
                {
                    return null;
                }

                count--;
            } while (count > 0);

            // return the result from the original string that has mixed
            // case
            int location2 = searchStr.IndexOf(token2Lower, location1 + 1);
            if (location2 == -1)
            {
                return null;
            }

            return str.Substring(location1 + token1Lower.Length, location2 - (location1 + token1.Length));
        }

        /// <summary>
        /// Post to a page.
        /// </summary>
        /// <param name="uri">The URI to post to.</param>
        /// <param name="param">The post params.</param>
        /// <returns>The HTTP response.</returns>
        public static String POSTPage(Uri uri, IDictionary<String, String> param)
        {
            WebRequest req = WebRequest.Create(uri);

            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";

            var postString = new StringBuilder();
            foreach (String key in param.Keys)
            {
                String value = param[key];
                if (value != null)
                {
                    if (postString.Length != 0)
                        postString.Append('&');
                    postString.Append(key);
                    postString.Append('=');
                    postString.Append(FormUtility.Encode(value));
                }
            }

            byte[] bytes = Encoding.ASCII.GetBytes(postString.ToString());
            req.ContentLength = bytes.Length;

            Stream os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length);
            os.Close();

            WebResponse resp = req.GetResponse();
            if (resp == null) return null;

            var sr = new StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

        /// <summary>
        /// Post bytes to a page.
        /// </summary>
        /// <param name="uri">The URI to post to.</param>
        /// <param name="bytes">The bytes to post.</param>
        /// <param name="length">The length of the posted data.</param>
        /// <returns>The HTTP response.</returns>
        public static String POSTPage(Uri uri, byte[] bytes, int length)
        {
            WebRequest webRequest = WebRequest.Create(uri);
            //webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            webRequest.ContentLength = length;


            Stream os = null;
            try
            {
                // send the Post
                webRequest.ContentLength = bytes.Length; //Count bytes to send
                os = webRequest.GetRequestStream();
                os.Write(bytes, 0, length); //Send it
            }
            catch (WebException ex)
            {
                throw new BotError(ex);
            }
            finally
            {
                if (os != null)
                {
                    os.Flush();
                }
            }

            try
            {
                // get the response
                WebResponse webResponse = webRequest.GetResponse();
                if (webResponse == null)
                {
                    return null;
                }
                var sr = new StreamReader(webResponse.GetResponseStream());
                return sr.ReadToEnd();
            }
            catch (WebException ex)
            {
                throw new BotError(ex);
            }
        }

        /// <summary>
        /// Load the specified web page into a string.
        /// </summary>
        /// <param name="url">The url to load.</param>
        /// <returns>The web page as a string.</returns>
        public static String LoadPage(Uri url)
        {
            try
            {
                var result = new StringBuilder();
                var buffer = new byte[BufferSize];

                int length;

                WebRequest http = WebRequest.Create(url);
                var response = (HttpWebResponse) http.GetResponse();
                Stream istream = response.GetResponseStream();

                do
                {
                    length = istream.Read(buffer, 0, buffer.Length);
                    if (length > 0)
                    {
                        String str = Encoding.UTF8.GetString(buffer, 0, length);
                        result.Append(str);
                    }
                } while (length > 0);

                return result.ToString();
            }
            catch (IOException e)
            {
#if logging
                if (LOGGER.IsErrorEnabled)
                {
                    LOGGER.Error("Exception", e);
                }
#endif
                throw new BotError(e);
            }
        }

        /// <summary>
        /// Private constructor.
        /// </summary>
        private BotUtil()
        {
        }

        /// <summary>
        /// Post to a page.
        /// </summary>
        /// <param name="uri">The URI to post to.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>The page returned.</returns>
        public static string POSTPage(Uri uri, Stream stream)
        {
            WebRequest req = WebRequest.Create(uri);

            //req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";

            Stream os = req.GetRequestStream();

            FileUtil.CopyStream(stream, os);
            os.Close();

            WebResponse resp = req.GetResponse();
            if (resp == null) return null;

            var sr = new StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

        public static void DownloadPage(Uri uri, string file)
        {
            var Client = new WebClient();
            Client.DownloadFile(uri, file);
        }
    }
}
