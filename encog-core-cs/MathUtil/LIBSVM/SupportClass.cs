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
using System.Collections;

namespace Encog.MathUtil.LIBSVM
{
    // This class was taken from the libsvm package.  We have made some
    // modifications for use in Encog.
    // 
    // http://www.csie.ntu.edu.tw/~cjlin/libsvm/
    //
    // The libsvm Copyright/license is listed here.
    // 
    // Copyright (c) 2000-2010 Chih-Chung Chang and Chih-Jen Lin
    // All rights reserved.
    // 
    // Redistribution and use in source and binary forms, with or without
    // modification, are permitted provided that the following conditions
    // are met:
    // 
    // 1. Redistributions of source code must retain the above copyright
    // notice, this list of conditions and the following disclaimer.
    // 
    // 2. Redistributions in binary form must reproduce the above copyright
    // notice, this list of conditions and the following disclaimer in the
    // documentation and/or other materials provided with the distribution.
    //
    // 3. Neither name of copyright holders nor the names of its contributors
    // may be used to endorse or promote products derived from this software
    // without specific prior written permission.
    // 
    // 
    // THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
    // ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
    // LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
    // A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE REGENTS OR
    // CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
    // EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
    // PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
    // PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
    // LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
    // NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
    // SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


    //
    // In order to convert some functionality to Visual C#, the Java Language Conversion Assistant
    // creates "support classes" that duplicate the original functionality.  
    //
    // Support classes replicate the functionality of the original code, but in some cases they are 
    // substantially different architecturally. Although every effort is made to preserve the 
    // original architecture of the application in the converted project, the user should be aware that 
    // the primary goal of these support classes is to replicate functionality, and that at times 
    // the architecture of the resulting solution may differ somewhat.
    //

    /// <summary>
    /// Contains conversion support elements such as classes, interfaces and static methods.
    /// </summary>
    public class SupportClass
    {
        /// <summary>
        /// Provides access to a static System.Random class instance
        /// </summary>
        public static Random Random = new Random();

        /*******************************/

        #region Nested type: Tokenizer

        /// <summary>
        /// The class performs token processing in strings
        /// </summary>
        public class Tokenizer : IEnumerator
        {
            /// Char representation of the String to tokenize.
            private readonly char[] chars;

            /// Include demiliters in the results.
            private readonly bool includeDelims;

            /// Position over the string
            private long currentPos;

            //The tokenizer uses the default delimiter set: the space character, the tab character, the newline character, and the carriage-return character and the form-feed character
            private string delimiters = " \t\n\r\f";

            /// <summary>
            /// Initializes a new class instance with a specified string to process
            /// </summary>
            /// <param name="source">String to tokenize</param>
            public Tokenizer(String source)
            {
                chars = source.ToCharArray();
            }

            /// <summary>
            /// Initializes a new class instance with a specified string to process
            /// and the specified token delimiters to use
            /// </summary>
            /// <param name="source">String to tokenize</param>
            /// <param name="delimiters">String containing the delimiters</param>
            public Tokenizer(String source, String delimiters)
                : this(source)
            {
                this.delimiters = delimiters;
            }


            /// <summary>
            /// Initializes a new class instance with a specified string to process, the specified token 
            /// delimiters to use, and whether the delimiters must be included in the results.
            /// </summary>
            /// <param name="source">String to tokenize</param>
            /// <param name="delimiters">String containing the delimiters</param>
            /// <param name="includeDelims">Determines if delimiters are included in the results.</param>
            public Tokenizer(String source, String delimiters, bool includeDelims)
                : this(source, delimiters)
            {
                this.includeDelims = includeDelims;
            }


            /// <summary>
            /// Remaining tokens count
            /// </summary>
            public int Count
            {
                get
                {
                    //keeping the current pos
                    long pos = currentPos;
                    int i = 0;

                    try
                    {
                        while (true)
                        {
                            NextToken();
                            i++;
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        currentPos = pos;
                        return i;
                    }
                }
            }

            #region IEnumerator Members

            /// <summary>
            ///  Performs the same action as NextToken.
            /// </summary>
            public Object Current
            {
                get { return NextToken(); }
            }

            /// <summary>
            ///  Performs the same action as HasMoreTokens.
            /// </summary>
            /// <returns>True or false, depending if there are more tokens</returns>
            public bool MoveNext()
            {
                return HasMoreTokens();
            }

            /// <summary>
            /// Does nothing.
            /// </summary>
            public void Reset()
            {
                ;
            }

            #endregion

            /// <summary>
            /// Returns the next token from the token list
            /// </summary>
            /// <returns>The string value of the token</returns>
            public String NextToken()
            {
                return NextToken(delimiters);
            }

            /// <summary>
            /// Returns the next token from the source string, using the provided
            /// token delimiters
            /// </summary>
            /// <param name="delimiters">String containing the delimiters to use</param>
            /// <returns>The string value of the token</returns>
            public String NextToken(String delimiters)
            {
                //According to documentation, the usage of the received delimiters should be temporary (only for this call).
                //However, it seems it is not true, so the following line is necessary.
                this.delimiters = delimiters;

                //at the end 
                if (currentPos == chars.Length)
                    throw new ArgumentOutOfRangeException();
                    //if over a delimiter and delimiters must be returned
                else if ((Array.IndexOf(delimiters.ToCharArray(), chars[currentPos]) != -1)
                         && includeDelims)
                    return "" + chars[currentPos++];
                    //need to get the token wo delimiters.
                else
                    return nextToken(delimiters.ToCharArray());
            }

            //Returns the nextToken wo delimiters
            private String nextToken(char[] delimiters)
            {
                string token = "";
                long pos = currentPos;

                //skip possible delimiters
                while (Array.IndexOf(delimiters, chars[currentPos]) != -1)
                    //The last one is a delimiter (i.e there is no more tokens)
                    if (++currentPos == chars.Length)
                    {
                        currentPos = pos;
                        throw new ArgumentOutOfRangeException();
                    }

                //getting the token
                while (Array.IndexOf(delimiters, chars[currentPos]) == -1)
                {
                    token += chars[currentPos];
                    //the last one is not a delimiter
                    if (++currentPos == chars.Length)
                        break;
                }
                return token;
            }


            /// <summary>
            /// Determines if there are more tokens to return from the source string
            /// </summary>
            /// <returns>True or false, depending if there are more tokens</returns>
            public bool HasMoreTokens()
            {
                //keeping the current pos
                long pos = currentPos;

                try
                {
                    NextToken();
                }
                catch (ArgumentOutOfRangeException)
                {
                    return false;
                }
                finally
                {
                    currentPos = pos;
                }
                return true;
            }
        }

        #endregion
    }
}
