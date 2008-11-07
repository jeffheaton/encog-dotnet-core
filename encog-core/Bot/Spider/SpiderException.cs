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

namespace Encog.Bot.Spider
{
    /// <summary>
    /// SpiderException: This exception is thrown when the spider
    /// encounters an error.
    /// </summary>
    public class SpiderException:Exception
    {
        /// <summary>
        /// Throw a spider exception.
        /// </summary>
        /// <param name="msg">The exception message.</param>
        public SpiderException(String msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Pass on an exception as a SpiderException.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public SpiderException(Exception e)
            : base("Exception while spidering", e)
        {
        }
    }
}
