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
        /// The children, or events that use us as a given.
        /// </summary>
        private readonly IList<BayesianEvent> _children = new List<BayesianEvent>();

        /// <summary>
        /// The discrete choices that make up the state of this event.
        /// </summary>
        private readonly ICollection<BayesianChoice> _choices = new SortedSet<BayesianChoice>();

        /// <summary>
        /// The label for this event.
        /// </summary>
        private readonly String _label;

        /// <summary>
        /// The parents, or given.
        /// </summary>
        private readonly IList<BayesianEvent> _parents = new List<BayesianEvent>();

        /// <summary>
        /// The truth table for this event.
        /// </summary>
        private BayesianTable _table;

        /// <summary>
        /// The value of the maximum choice.
        /// </summary>
        private double _maximumChoice;

        /// <summary>
        /// THe value of the minimum choice.
        /// </summary>
        private double _minimumChoice;

        /// <summary>
        /// The index of the minimum choice.
        /// </summary>
        private int _minimumChoiceIndex;

        /// <summary>
        /// Construct an event with the specified label and choices.
        /// </summary>
        /// <param name="theLabel">The label.</param>
        /// <param name="theChoices">The choices, or states.</param>
        public BayesianEvent(String theLabel, IEnumerable<BayesianChoice> theChoices)
        {
            _label = theLabel;
            foreach (BayesianChoice choice in theChoices)
            {
                _choices.Add(choice);
            }
        }

        /// <summary>
        /// Construct an event with the specified label and choices. 
        /// </summary>
        /// <param name="theLabel">The label.</param>
        /// <param name="theChoices">The choices, or states.</param>
        public BayesianEvent(String theLabel, IEnumerable<string> theChoices)
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
            : this(theLabel, BayesianNetwork.ChoicesTrueFalse)
        {
        }

        /// <summary>
        /// the parents
        /// </summary>
        public IList<BayesianEvent> Parents
        {
            get { return _parents; }
        }

        /// <summary>
        /// the children
        /// </summary>
        public IList<BayesianEvent> Children
        {
            get { return _children; }
        }

        /// <summary>
        /// the label
        /// </summary>
        public String Label
        {
            get { return _label; }
        }

        /// <summary>
        /// True, if this event has parents.
        /// </summary>
        public bool HasParents
        {
            get { return _parents.Count > 0; }
        }

        /// <summary>
        /// True, if this event has parents.
        /// </summary>
        public bool HasChildren
        {
            get { return _parents.Count > 0; }
        }

        /// <summary>
        /// the choices
        /// </summary>
        public ICollection<BayesianChoice> Choices
        {
            get { return _choices; }
        }

        /// <summary>
        /// the table
        /// </summary>
        public BayesianTable Table
        {
            get { return _table; }
        }

        /// <summary>
        /// True, if this is a boolean event.
        /// </summary>
        public bool IsBoolean
        {
            get { return _choices.Count == 2; }
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
        /// A full string that contains all info for this event.
        /// </summary>
        /// <returns>A full string that contains all info for this event.</returns>
        public String ToFullString()
        {
            var result = new StringBuilder();

            result.Append("P(");
            result.Append(Label);

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
        public override String ToString()
        {
            var result = new StringBuilder();

            result.Append("P(");
            result.Append(Label);

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

            return _parents.Aggregate(result, (current, parent) => current*_choices.Count);
        }

        /// <summary>
        /// Finalize the structure.
        /// </summary>
        public void FinalizeStructure()
        {
            // find min/max choice
            _minimumChoiceIndex = -1;
            _minimumChoice = Double.PositiveInfinity;
            _maximumChoice = Double.NegativeInfinity;

            int index = 0;
            foreach (BayesianChoice choice in _choices)
            {
                if (choice.Min < _minimumChoice)
                {
                    _minimumChoice = choice.Min;
                    _minimumChoiceIndex = index;
                }
                if (choice.Max > _maximumChoice)
                {
                    _maximumChoice = choice.Max;
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
                var v = (int) args[currentIndex];
                v++;
                if (v >= _parents[currentIndex].Choices.Count)
                {
                    args[currentIndex] = 0;
                }
                else
                {
                    args[currentIndex] = v;
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
            var str = new StringBuilder();

            if (theEvent.IsBoolean)
            {
                str.Append(value == 0 ? "+" : "-");
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
            return _parents.Any(e => e.Label.Equals(l));
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
            if (Choices.Count > 0 && Choices.First().IsIndex)
            {
                var result = (int) d;              
                if (result > Choices.Count)
                {
                    throw new BayesianError("The item id " + result + " is not valid for event " + this.ToString());
                }
                return result;
            }

            var index = 0;
            foreach (var choice in Choices)
            {
                if (d < choice.Max)
                {
                    return index;
                }

                index++;
            }

            return Math.Min(index, Choices.Count - 1);
        }

        /// <summary>
        /// Return the choice specified by the index.  This requires searching
        /// through a list.  Do not call in performance critical areas.
        /// </summary>
        /// <param name="arg">The argument number.</param>
        /// <returns>The bayesian choice found.</returns>
        public BayesianChoice GetChoice(int arg)
        {
            int a = arg;

            foreach (BayesianChoice choice in _choices)
            {
                if (a == 0)
                {
                    return choice;
                }
                a--;
            }
            return null;
        }
    }
}
