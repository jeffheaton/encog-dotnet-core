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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.ML.Bayesian.Parse
{
    /// <summary>
    /// An event that has been parsed.
    /// </summary>
    public class ParsedEvent
    {
        /// <summary>
        /// The event label.
        /// </summary>
        private readonly String label;

        /// <summary>
        /// The event value.
        /// </summary>
        public String Value { get; set; }

        /// <summary>
        /// The choices.
        /// </summary>
        private readonly IList<ParsedChoice> list = new List<ParsedChoice>();

        /// <summary>
        /// Construct a parsed even with the specified label.
        /// </summary>
        /// <param name="theLabel">The label.</param>
        public ParsedEvent(String theLabel)
        {
            this.label = theLabel;
        }

        /// <summary>
        /// The label for this event.
        /// </summary>
        public String Label
        {
            get
            {
                return label;
            }
        }

        /// <summary>
        /// Resolve the event to an actual value.
        /// </summary>
        /// <param name="actualEvent">The actual event.</param>
        /// <returns>The value.</returns>
        public int ResolveValue(BayesianEvent actualEvent)
        {
            int result = 0;

            if (this.Value == null)
            {
                throw new BayesianError("Value is undefined for " + this.label + " should express a value with +, - or =.");
            }

            foreach (BayesianChoice choice in actualEvent.Choices)
            {
                if (this.Value.Equals(choice.Label))
                {
                    return result;
                }
                result++;
            }

            // resolve true/false if not found, probably came from +/- notation
            if (String.Compare(Value, "true", true) == 0)
            {
                return 0;
            }
            else if (String.Compare(Value, "false", true) == 0)
            {
                return 1;
            }

            // try to resolve numeric index
            try
            {
                int i = int.Parse(this.Value);
                if (i < actualEvent.Choices.Count)
                {
                    return i;
                }
            }
            catch (FormatException ex)
            {
                // well, we tried
            }

            // error out if nothing found
            throw new BayesianError("Can'f find choice " + this.Value + " in the event " + this.label);
        }


        /// <summary>
        /// A list of choices.
        /// </summary>
        public IList<ParsedChoice> ChoiceList
        {
            get
            {
                return list;
            }
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[ParsedEvent:label=");
            result.Append(this.label);
            result.Append(",value=");
            result.Append(this.Value);
            result.Append("]");
            return result.ToString();
        }

    }
}
