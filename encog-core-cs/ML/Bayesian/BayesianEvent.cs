//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
using Encog.ML.Bayesian.Table;

namespace Encog.ML.Bayesian
{
    /// <summary>
    /// Events make up a Bayesian network. Each evidence or outcome event usually
    /// corresponds to one number in the training data.  A event is always discrete.
    /// However, continues values can be range-mapped to discrete values.
    /// </summary>
    [Serializable]
    public class BayesianEvent
    {
        /// <summary>
        /// The label for this event.
        /// </summary>
        private readonly String _label;

        /// <summary>
        /// The parents, or given.
        /// </summary>
        private readonly IList<BayesianEvent> _parents = new List<BayesianEvent>();

        /// <summary>
        /// The children, or events that use us as a given.
        /// </summary>
        private readonly IList<BayesianEvent> _children = new List<BayesianEvent>();

        /// <summary>
        /// The discrete choices that make up the state of this event.
        /// </summary>
        private readonly IList<BayesianChoice> _choices = new List<BayesianChoice>();

        /// <summary>
        /// The truth table for this event.
        /// </summary>
        private BayesianTable _table;

        /// <summary>
        /// The index of the minimum choice.
        /// </summary>
        private int minimumChoiceIndex;

        /// <summary>
        /// THe value of the minimum choice.
        /// </summary>
        private double minimumChoice;

        /// <summary>
        /// The index of the maximum choice.
        /// </summary>
        private int maximumChoiceIndex;

        /// <summary>
        /// The value of the maximum choice.
        /// </summary>
        private double maximumChoice;

        /// <summary>
        /// Construct an event with the specified label and choices.
        /// </summary>
        /// <param name="theLabel">The label.</param>
        /// <param name="theChoices">The choices, or states.</param>
        public BayesianEvent(String theLabel, IList<BayesianChoice> theChoices)
        {
            _label = theLabel;
            _choices.CopyTo(theChoices.ToArray(), 0);
        }

        /// <summary>
        /// Construct an event with the specified label and choices. 
        /// </summary>
        /// <param name="theLabel">The label.</param>
        /// <param name="theChoices">The choices, or states.</param>
        public BayesianEvent(String theLabel, String[] theChoices)
        {
            _label = theLabel;

            int index = 0;
            foreach (String str in theChoices)
            {
                _choices.Add(new BayesianChoice(str, index++));
            }
        }

        /// <summary>
        /// Construct a boolean event.
        /// </summary>
        /// <param name="theLabel">The label.</param>
        public BayesianEvent(String theLabel)
            : this(theLabel, BayesianNetwork.CHOICES_TRUE_FALSE)
        {

        }

        /// <summary>
        /// the parents
        /// </summary>
        public IList<BayesianEvent> Parents
        {
            get
            {
                return _parents;
            }
        }

        /// <summary>
        /// the children
        /// </summary>
        public IList<BayesianEvent> Children
        {
            get
            {
                return _children;
            }
        }

        /// <summary>
        /// the label
        /// </summary>
        public String Label
        {
            get
            {
                return _label;
            }
        }

        /// <summary>
        /// Add a child event.
        /// </summary>
        /// <param name="e">The child event.</param>
        public void AddChild(BayesianEvent e)
        {
            _children.Add(e);
        }

        /// <summary>
        /// Add a parent event.
        /// </summary>
        /// <param name="e">The parent event.</param>
        public void AddParent(BayesianEvent e)
        {
            _parents.Add(e);
        }

        /// <summary>
        /// True, if this event has parents.
        /// </summary>
        public bool HasParents
        {
            get
            {
                return _parents.Count > 0;
            }
        }

        /// <summary>
        /// True, if this event has parents.
        /// </summary>
        public bool HasChildren
        {
            get
            {
                return _parents.Count > 0;
            }
        }

        /// <summary>
        /// A full string that contains all info for this event.
        /// </summary>
        /// <returns>A full string that contains all info for this event.</returns>
        public String ToFullString()
        {
            StringBuilder result = new StringBuilder();

            result.Append("P(");
            result.Append(this.Label);

            result.Append("[");
            bool first = true;
            foreach (BayesianChoice choice in _choices)
            {
                if (!first)
                {
                    result.Append(",");
                }
                result.Append(choice.ToFullString());
                first = false;
            }
            result.Append("]");

            if (HasParents)
            {
                result.Append("|");
            }

            first = true;
            foreach (BayesianEvent e in _parents)
            {
                if (!first)
                    result.Append(",");
                first = false;
                result.Append(e.Label);
            }

            result.Append(")");
            return result.ToString();
        }

        /// <inheritdoc/>
        public String ToString()
        {
            StringBuilder result = new StringBuilder();

            result.Append("P(");
            result.Append(this.Label);

            if (HasParents)
            {
                result.Append("|");
            }

            bool first = true;
            foreach (BayesianEvent e in _parents)
            {
                if (!first)
                    result.Append(",");
                first = false;
                result.Append(e.Label);
            }

            result.Append(")");
            return result.ToString();
        }

        /// <summary>
        /// The parameter count.
        /// </summary>
        /// <returns>The parameter count.</returns>
        public int CalculateParameterCount()
        {
            int result = _choices.Count - 1;

            foreach (BayesianEvent parent in _parents)
            {
                result *= _choices.Count;
            }

            return result;
        }

        /// <summary>
        /// the choices
        /// </summary>
        public IList<BayesianChoice> Choices
        {
            get
            {
                return _choices;
            }
        }

        /// <summary>
        /// the table
        /// </summary>
        public BayesianTable Table
        {
            get
            {
                return _table;
            }
        }

        /// <summary>
        /// Finalize the structure.
        /// </summary>
        public void finalizeStructure()
        {
            // find min/max choice
            this.minimumChoiceIndex = -1;
            this.maximumChoiceIndex = -1;
            this.minimumChoice = Double.PositiveInfinity;
            this.maximumChoice = Double.NegativeInfinity;

            int index = 0;
            foreach (BayesianChoice choice in _choices)
            {
                if (choice.Min < this.minimumChoice)
                {
                    this.minimumChoice = choice.Min;
                    this.minimumChoiceIndex = index;
                }
                if (choice.Max > this.maximumChoice)
                {
                    this.maximumChoice = choice.Max;
                    this.maximumChoiceIndex = index;
                }
                index++;
            }

            // build truth table
            if (_table == null)
            {
                _table = new BayesianTable(this);
                _table.Reset();
            }
            else
            {
                _table.Reset();
            }

        }

        /// <summary>
        /// Validate the event.
        /// </summary>
        public void Validate()
        {
            _table.Validate();
        }

        /// <summary>
        /// True, if this is a boolean event.
        /// </summary>
        public bool IsBoolean
        {
            get
            {
                return _choices.Count == 2;
            }
        }

        /// <summary>
        /// Roll the specified arguments through all of the possible values, return
        /// false if we are at the final iteration. This is used to enumerate through
        /// all of the possible argument values of this event.
        /// </summary>
        /// <param name="args">The arguments to enumerate.</param>
        /// <returns>True if there are more iterations.</returns>
        public bool RollArgs(double[] args)
        {
            int currentIndex = 0;
            bool done = false;
            bool eof = false;

            if (_parents.Count == 0)
            {
                done = true;
                eof = true;
            }

            while (!done)
            {

                // EventState state = this.parents.get(currentIndex);
                int v = (int)args[currentIndex];
                v++;
                if (v >= _parents[currentIndex].Choices.Count)
                {
                    args[currentIndex] = 0;
                }
                else
                {
                    args[currentIndex] = v;
                    done = true;
                    break;
                }

                currentIndex++;

                if (currentIndex >= _parents.Count)
                {
                    done = true;
                    eof = true;
                }
            }

            return !eof;
        }

        /// <summary>
        /// Remove all relations.
        /// </summary>
        public void RemoveAllRelations()
        {
            _children.Clear();
            _parents.Clear();
        }
        
        /// <summary>
        /// Format the event name with +, - and =.  For example +a or -1, or a=red.
        /// </summary>
        /// <param name="theEvent">The event to format.</param>
        /// <param name="value">The value to format for.</param>
        /// <returns>The formatted name.</returns>
        public static String FormatEventName(BayesianEvent theEvent, int value)
        {
            StringBuilder str = new StringBuilder();

            if (theEvent.IsBoolean)
            {
                if (value == 0)
                {
                    str.Append("+");
                }
                else
                {
                    str.Append("-");
                }
            }
            str.Append(theEvent.Label);
            if (!theEvent.IsBoolean)
            {
                str.Append("=");
                str.Append(value);
            }

            return str.ToString();

        }

        /// <summary>
        /// Return true if the event has the specified given event.
        /// </summary>
        /// <param name="l">The event to check for.</param>
        /// <returns>True if the event has the specified given.</returns>
        public bool HasGiven(String l)
        {
            foreach (BayesianEvent e in _parents)
            {
                if (e.Label.Equals(l))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Reset the logic table.
        /// </summary>
        public void Reset()
        {
            if (_table == null)
            {
                _table = new BayesianTable(this);
            }
            _table.Reset();
        }


        /// <summary>
        /// Match a continuous value to a discrete range. This is how floating point
        /// numbers can be used as input to a Bayesian network.
        /// </summary>
        /// <param name="d">The continuous value.</param>
        /// <returns>The range that the value was mapped into.</returns>
        public int MatchChoiceToRange(double d)
        {
            if (_choices.Count > 0 && _choices[0].IsIndex)
            {
                return (int)d;
            }

            int index = 0;
            foreach (BayesianChoice choice in _choices)
            {
                if (d > choice.Min && d < choice.Max)
                {
                    return index;
                }

                if (Math.Abs(d - choice.Min) < EncogFramework.DefaultDoubleEqual)
                    return index;

                if (Math.Abs(d - choice.Max) < EncogFramework.DefaultDoubleEqual)
                    return index;

                index++;
            }

            // out of range?

            if (d < this.minimumChoice)
                return this.minimumChoiceIndex;
            if (d > this.maximumChoice)
                return this.minimumChoiceIndex;

            throw new BayesianError("Can't find a choice to map the value of " + d
                    + " to for event " + this.ToString());
        }
    }
}
