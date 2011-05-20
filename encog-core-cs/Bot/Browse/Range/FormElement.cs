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

#if !SILVERLIGHT
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
        private String name;

        /// <summary>
        /// The owner.
        /// </summary>
        private Form owner;

        /// <summary>
        /// The value.
        /// </summary>
        private String value;


        /**
         * Construct a form element from the specified web page.
         * @param source The page that holds this form element.
         */

        public FormElement(WebPage source)
            : base(source)
        {
        }

        /**
         * @return The name of this form.
         */

        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        /**
         * @return The owner of this form element.
         */

        public Form Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        /**
         * @return The value of this form element.
         */

        public String Value
        {
            get { return value; }
            set { this.value = value; }
        }

        /**
         * @return True if this is autosend, which means that the type is 
         * NOT submit.  This prevents a form that has multiple submit buttons
         * from sending ALL of them in a single post.
         */
        public abstract bool AutoSend { get; }
    }
}

#endif