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

namespace Encog.Bot.Browse.Range
{
    /// <summary>
    /// A form element that represents for input for text.  These are of the
    /// form name=value.
    /// </summary>
    public class Input : FormElement
    {
        /// <summary>
        /// The type of input element that this is.
        /// </summary>
        private String _type;

        /// <summary>
        /// Construct this Input element.
        /// </summary>
        /// <param name="source">The source for this input ent.</param>
        public Input(WebPage source)
            : base(source)
        {
        }

        /// <summary>
        /// The type of this input.
        /// </summary>
        public String Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// True if this is autosend, which means that the type is NOT
        /// submit. This prevents a form that has multiple submit buttons
        /// from sending ALL of them in a single post.
        /// </summary>
        public override bool AutoSend
        {
            get { return string.Compare(_type, "submit", true) != 0; }
        }

        /// <summary>
        /// This object as a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            var builder = new StringBuilder();
            builder.Append("[Input:");
            builder.Append("type=");
            builder.Append(Type);
            builder.Append(",name=");
            builder.Append(Name);
            builder.Append(",value=");
            builder.Append(Value);
            builder.Append("]");
            return builder.ToString();
        }
    }
}
