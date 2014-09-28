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
using System.Text;

namespace Encog.Util.Arrayutil
{
    /// <summary>
    /// A class item.
    /// </summary>
    ///
    public class ClassItem
    {
        /// <summary>
        /// The index of the class.
        /// </summary>
        ///
        private int _index;

        /// <summary>
        /// The name of the class.
        /// </summary>
        ///
        private String _name;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="theName">The name of the class.</param>
        /// <param name="theIndex">The index of the class.</param>
        public ClassItem(String theName, int theIndex)
        {
            _name = theName;
            _index = theIndex;
        }

        /// <summary>
        /// Set the index of the class.
        /// </summary>
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }


        /// <summary>
        /// Set the name of the class.
        /// </summary>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }


        /// <inheritdoc/>
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" name=");
            result.Append(_name);
            result.Append(", index=");
            result.Append(_index);

            result.Append("]");
            return result.ToString();
        }
    }
}
