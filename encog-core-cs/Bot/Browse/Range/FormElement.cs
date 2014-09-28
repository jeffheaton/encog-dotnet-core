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

namespace Encog.Bot.Browse.Range
{
    /// <summary>
    /// A document range that represents one individual component to a form.
    /// </summary>
    public abstract class FormElement : DocumentRange
    {
        /// <summary>
        /// The name of the element.
        /// </summary>
        private String _name;

        /// <summary>
        /// The owner.
        /// </summary>
        private Form _owner;

        /// <summary>
        /// The value.
        /// </summary>
        private String _value;

        /// <summary>
        /// Construct a form element from the specified web page. 
        /// </summary>
        /// <param name="source">The page that holds this form element.</param>
        protected FormElement(WebPage source)
            : base(source)
        {
        }

        /// <summary>
        /// The name of this form.
        /// </summary>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The owner of this form element.
        /// </summary>
        public Form Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        /// <summary>
        /// The value of this form element.
        /// </summary>
        public String Value
        {
            get { return _value; }
            set { this._value = value; }
        }
        
        /// <summary>
        /// True if this is autosend, which means that the type is 
        /// NOT submit.  This prevents a form that has multiple submit buttons
        /// from sending ALL of them in a single post.
        /// </summary>
        public abstract bool AutoSend { get; }
    }
}
