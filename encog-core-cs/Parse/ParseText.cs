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
using Encog.Parse.Units;
using Encog.Parse.Recognize;
using log4net;
using Encog.Persist.Location;
using System.IO;
using Encog.Persist;

namespace Encog.Parse
{
    /// <summary>
    /// Encog's generic parser, used by the NLP package.
    /// </summary>
    public class ParseText
    {
        /// <summary>
        /// The unit manager.
        /// </summary>
        private static UnitManager unitManager;

        /// <summary>
        /// The template to use.
        /// </summary>
        private ParseTemplate template;

        /// <summary>
        /// The name of the resource that holds the default template.
        /// </summary>
        public const String RESOURCE_NAME = "Encog.Data.template.eg";

        /// <summary>
        /// The item name for the default template.
        /// </summary>
        public const String RESOURCE_ITEM_NAME = "parse-native";

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(FilePersistence));

        /// <summary>
        /// Parse a signal file.  Used for testing.
        /// </summary>
        /// <param name="name">The name of the file.</param>
        /// <returns>The signals returned from the file.</returns>
        public Signal.Signal ParseFile(String name)
        {
            try
            {
                TextReader file = new StreamReader(name);

                String contents = "";
                String line = "";

                while ((line = file.ReadLine()) != null)
                    contents += line + "\r\n";

                file.Close();
                return ParseString(contents);
            }
            catch (IOException e)
            {
                if (logger.IsErrorEnabled)
                {
                    logger.Error("Exception", e);
                }
                throw new ParseError(e);
            }
        }

        /// <summary>
        /// The unit manager to use with this parser.
        /// </summary>
        public static UnitManager UnitManager
        {
            get
            {
                return ParseText.unitManager;
            }
            set
            {
                unitManager = value;
            }
        }

        private bool ParseIteration(Signal.Signal input)
        {
            bool changed = false;

            if (this.template == null)
            {
                throw new ParseError("Must load a template before calling the Parse object.");
            }

            foreach (Recognize.Recognize recognize in template.Recognizers)
            {
                if (recognize.PerformRecognize(input))
                    changed = true;
            }
            return changed;
        }

        /// <summary>
        /// Parse the specified string into a signal.
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <returns>The signal the string was parsed into.</returns>
        public Signal.Signal ParseString(String input)
        {
            Signal.Signal result = new Signal.Signal(input + " ");
            result.ResetDelta();

            do
            {
                result.ResetDelta();
                ParseIteration(result);
            }
            while (result.Delta);
            return result;
        }

        /// <summary>
        /// The template to use for parsing.
        /// </summary>
        public ParseTemplate Template
        {
            get
            {
                return template;
            }
        }

        /// <summary>
        /// Load the default template from a resource.
        /// </summary>
        public void Load()
        {

            ResourcePersistence location = new ResourcePersistence(ParseText.RESOURCE_NAME);
            EncogPersistedCollection encog = new EncogPersistedCollection(location, FileMode.Open);
            this.template = (ParseTemplate)encog.Find(ParseText.RESOURCE_ITEM_NAME);
        }

    }
}
