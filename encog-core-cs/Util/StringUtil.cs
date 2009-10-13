// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
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
using System.Text;


namespace Encog.Util
{
    /// <summary>
    /// Simple class for string utilities.
    /// </summary>
    public class StringUtil
    {
        /// <summary>
        /// Compare two strings, ignore case.
        /// </summary>
        /// <param name="a">The first string.</param>
        /// <param name="b">The second string.</param>
        /// <returns></returns>
        public static Boolean EqualsIgnoreCase(String a, String b)
        {
            return a.Equals(b, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Simple utility to take an array of ASCII bytes and convert to
        /// a String.  Works with Silverlight as well.
        /// </summary>
        /// <param name="b">The byte array.</param>
        /// <returns>The string created from the byte array.</returns>
        public static String FromBytes(byte[] b)
        {
            byte[] b2 = new byte[b.Length * 2];
            for (int i = 0; i < b.Length; i++)
            {
                b2[i * 2] = b[i];
                b2[(i * 2) + 1] = 0;
            }
            
            return (new UnicodeEncoding()).GetString(b2, 0, b2.Length);
        }
    }
}
