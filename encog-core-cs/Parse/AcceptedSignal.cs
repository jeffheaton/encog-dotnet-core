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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Encog.Parse
{
    /**
 * A signal that has been accepted by the parser.
 * @author jheaton
 *
 */
    public class AcceptedSignal
    {
        /// <summary>
        /// The type of signal that this is.
        /// </summary>
        private String type;

        /// <summary>
        /// The value of this signal.
        /// </summary>
        private String value;

        /// <summary>
        /// The logging object.
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(typeof(AcceptedSignal));

        public AcceptedSignal(String type, String value)
        {
            this.type = type;
            this.value = value;
        }

        /// <summary>
        /// The value of this signal.
        /// </summary>
        public String Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        /// <summary>
        /// The type of signal that this is.
        /// </summary>
        public String Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }
    }
}
