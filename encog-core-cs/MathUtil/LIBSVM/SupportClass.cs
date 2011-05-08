

using System;

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
        //Provides access to a static System.Random class instance
        static public System.Random Random = new System.Random();

        /*******************************/
        /// <summary>
        /// The class performs token processing in strings
        /// </summary>
        public class Tokenizer : System.Collections.IEnumerator
        {
            /// Position over the string
            private long currentPos = 0;

            /// Include demiliters in the results.
            private bool includeDelims = false;

            /// Char representation of the String to tokenize.
            private char[] chars = null;

            //The tokenizer uses the default delimiter set: the space character, the tab character, the newline character, and the carriage-return character and the form-feed character
            private string delimiters = " \t\n\r\f";

            /// <summary>
            /// Initializes a new class instance with a specified string to process
            /// </summary>
            /// <param name="source">String to tokenize</param>
            public Tokenizer(System.String source)
            {
                this.chars = source.ToCharArray();
            }

            /// <summary>
            /// Initializes a new class instance with a specified string to process
            /// and the specified token delimiters to use
            /// </summary>
            /// <param name="source">String to tokenize</param>
            /// <param name="delimiters">String containing the delimiters</param>
            public Tokenizer(System.String source, System.String delimiters)
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
            public Tokenizer(System.String source, System.String delimiters, bool includeDelims)
                : this(source, delimiters)
            {
                this.includeDelims = includeDelims;
            }


            /// <summary>
            /// Returns the next token from the token list
            /// </summary>
            /// <returns>The string value of the token</returns>
            public System.String NextToken()
            {
                return NextToken(this.delimiters);
            }

            /// <summary>
            /// Returns the next token from the source string, using the provided
            /// token delimiters
            /// </summary>
            /// <param name="delimiters">String containing the delimiters to use</param>
            /// <returns>The string value of the token</returns>
            public System.String NextToken(System.String delimiters)
            {
                //According to documentation, the usage of the received delimiters should be temporary (only for this call).
                //However, it seems it is not true, so the following line is necessary.
                this.delimiters = delimiters;

                //at the end 
                if (this.currentPos == this.chars.Length)
                    throw new System.ArgumentOutOfRangeException();
                //if over a delimiter and delimiters must be returned
                else if ((System.Array.IndexOf(delimiters.ToCharArray(), chars[this.currentPos]) != -1)
                         && this.includeDelims)
                    return "" + this.chars[this.currentPos++];
                //need to get the token wo delimiters.
                else
                    return nextToken(delimiters.ToCharArray());
            }

            //Returns the nextToken wo delimiters
            private System.String nextToken(char[] delimiters)
            {
                string token = "";
                long pos = this.currentPos;

                //skip possible delimiters
                while (System.Array.IndexOf(delimiters, this.chars[currentPos]) != -1)
                    //The last one is a delimiter (i.e there is no more tokens)
                    if (++this.currentPos == this.chars.Length)
                    {
                        this.currentPos = pos;
                        throw new System.ArgumentOutOfRangeException();
                    }

                //getting the token
                while (System.Array.IndexOf(delimiters, this.chars[this.currentPos]) == -1)
                {
                    token += this.chars[this.currentPos];
                    //the last one is not a delimiter
                    if (++this.currentPos == this.chars.Length)
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
                long pos = this.currentPos;

                try
                {
                    this.NextToken();
                }
                catch (System.ArgumentOutOfRangeException)
                {
                    return false;
                }
                finally
                {
                    this.currentPos = pos;
                }
                return true;
            }

            /// <summary>
            /// Remaining tokens count
            /// </summary>
            public int Count
            {
                get
                {
                    //keeping the current pos
                    long pos = this.currentPos;
                    int i = 0;

                    try
                    {
                        while (true)
                        {
                            this.NextToken();
                            i++;
                        }
                    }
                    catch (System.ArgumentOutOfRangeException)
                    {
                        this.currentPos = pos;
                        return i;
                    }
                }
            }

            /// <summary>
            ///  Performs the same action as NextToken.
            /// </summary>
            public System.Object Current
            {
                get
                {
                    return (Object)this.NextToken();
                }
            }

            /// <summary>
            ///  Performs the same action as HasMoreTokens.
            /// </summary>
            /// <returns>True or false, depending if there are more tokens</returns>
            public bool MoveNext()
            {
                return this.HasMoreTokens();
            }

            /// <summary>
            /// Does nothing.
            /// </summary>
            public void Reset()
            {
                ;
            }
        }
    }
}