// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009-2010, Heaton Research Inc., and individual contributors.
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

#if !SILVERLIGHT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

#if logging
using log4net;
#endif

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
        public static int BUFFER_SIZE = 8192;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(BotUtil));
#endif

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
            int location1, location2;

            // convert everything to lower case
            String searchStr = str.ToLower();
            String token1Lower = token1.ToLower();
            String token2Lower = token2.ToLower();

            int count = occurence;

            // now search
            location1 = location2 = index - 1;
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
            location2 = searchStr.IndexOf(token2Lower, location1 + 1);
            if (location2 == -1)
            {
                return null;
            }

            return str.Substring(location1 + token1Lower.Length, location2 - (location1 + token1.Length) );
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
            int location1, location2;

            // convert everything to lower case
            String searchStr = str.ToLower();
            String token1Lower = token1.ToLower();
            String token2Lower = token2.ToLower();

            int count = index;

            // now search
            location1 = location2 = -1;
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
            location2 = searchStr.IndexOf(token2Lower, location1 + 1);
            if (location2 == -1)
            {
                return null;
            }

            return str.Substring(location1 + token1Lower.Length, location2-(location1+token1.Length));
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
                StringBuilder result = new StringBuilder();
                byte[] buffer = new byte[BotUtil.BUFFER_SIZE];

                int length;

                WebRequest http = HttpWebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)http.GetResponse();
                Stream istream = response.GetResponseStream();

                do
                {
                    length = istream.Read(buffer, 0, buffer.Length);
                    if (length > 0)
                    {
                        String str = System.Text.Encoding.UTF8.GetString(buffer, 0, length);
                        result.Append(str);
                    }
                } while (length > 0);

                return result.ToString();
            }
            catch (IOException e)
            {
#if logging
                if (BotUtil.LOGGER.IsErrorEnabled)
                {
                    BotUtil.LOGGER.Error("Exception", e);
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
    }

}
#endif
