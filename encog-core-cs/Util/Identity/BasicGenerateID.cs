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

namespace Encog.Util.Identity
{
    /// <summary>
    /// Generate unique id's.  ID's start at 1.
    /// </summary>
    public class BasicGenerateID : IGenerateID
    {
        /// <summary>
        /// Construct an ID generator.
        /// </summary>
        public BasicGenerateID()
        {
            CurrentID = 1;
        }

        #region IGenerateID Members

        /// <summary>
        /// The current id to generate.  This is the next id returned.
        /// </summary>
        public long CurrentID { get; set; }

        /// <summary>
        /// Generate a unique id.
        /// </summary>
        /// <returns>The unique id.</returns>
        public long Generate()
        {
            lock (this)
            {
                return CurrentID++;
            }
        }

        #endregion
    }
}