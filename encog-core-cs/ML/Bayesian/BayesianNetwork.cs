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
using Encog.ML.Bayesian.Query;
using Encog.Util;
using Encog.ML.Data;
using Encog.ML.Bayesian.Parse;
using Encog.Util.CSV;
using Encog.ML.Bayesian.Query.Enumeration;

namespace Encog.ML.Bayesian
{
    /// <summary>
    /// The Bayesian Network is a machine learning method that is based on
    /// probability, and particularly Bayes' Rule. The Bayesian Network also forms
    /// the basis for the Hidden Markov Model and Naive Bayesian Network. The
    /// Bayesian Network is either constructed directly or inferred from training
    /// data using an algorithm such as K2.
    /// 
    /// http://www.heatonresearch.com/wiki/Bayesian_Network
    /// </summary>
    [Serializable]
    public class BayesianNetwork : IMLClassification, IMLResettable, IMLError
    {
        /// <summary>
        /// Default choices for a boolean event.
        /// </summary>
        public static readonly String[] CHOICES_TRUE_FALSE = { "true", "false" };

        /// <summary>
        /// Mapping between the event string names, and the actual events.
        /// </summary>
        private readonly IDictionary<String, BayesianEvent> eventMap = new Dictionary<String, BayesianEvent>();

        /// <summary>
        /// A listing of all of the events.
        /// </summary>
        private readonly IList<BayesianEvent> events = new List<BayesianEvent>();

        /// <summary>
        /// The current Bayesian query.
        /// </summary>
        public IBayesianQuery Query { get; set; }

        /// <summary>
        /// Specifies if each input is present.
        /// </summary>
        private bool[] inputPresent;

        /// <summary>
        /// Specifies the classification target.
        /// </summary>
        private int classificationTarget;

        /// <summary>
        /// The probabilities of each classification.
        /// </summary>
        private double[] classificationProbabilities;

        /// <summary>
        /// Construct a Bayesian network.
        /// </summary>
        public BayesianNetwork()
        {
            this.Query = new EnumerationQuery(this);
        }

        /// <summary>
        /// The mapping from string names to events.
        /// </summary>
        public IDictionary<String, BayesianEvent> EventMap
        {
            get
            {
                return eventMap;
            }
        }

        /// <summary>
        /// The events.
        /// </summary>
        public IList<BayesianEvent> Events
        {
            get
            {
                return this.events;
            }
        }

        /// <summary>
        /// Get an event based on the string label. 
        /// </summary>
        /// <param name="label">The label to locate.</param>
        /// <returns>The event found.</returns>
        public BayesianEvent GetEvent(String label)
        {
            if (this.eventMap.ContainsKey(label))
                return null;
            else
                return this.eventMap[label];
        }
        
        /// <summary>
        /// Get an event based on label, throw an error if not found.
        /// </summary>
        /// <param name="label">THe event label to find.</param>
        /// <returns>The event.</returns>
        public BayesianEvent GetEventError(String label)
        {
            if (!EventExists(label))
                throw (new BayesianError("Undefined label: " + label));
            return this.eventMap[label];
        }


        /// <summary>
        /// Return true if the specified event exists. 
        /// </summary>
        /// <param name="label">The label we are searching for.</param>
        /// <returns>True, if the event exists by label.</returns>
        public bool EventExists(String label)
        {
            return this.eventMap.ContainsKey(label);
        }
        
        /// <summary>
        /// Create, or register, the specified event with this bayesian network. 
        /// </summary>
        /// <param name="theEvent">The event to add.</param>
        public void CreateEvent(BayesianEvent theEvent)
        {
            if (EventExists(theEvent.Label))
            {
                throw new BayesianError("The label \"" + theEvent.Label
                        + "\" has already been defined.");
            }

            this.eventMap[theEvent.Label] = theEvent;
            this.events.Add(theEvent);
        }

        
        /// <summary>
        /// Create an event specified on the label and options provided. 
        /// </summary>
        /// <param name="label">The label to create this event as.</param>
        /// <param name="options">The options, or states, that this event can have.</param>
        /// <returns>The newly created event.</returns>
        public BayesianEvent CreateEvent(String label, IList<BayesianChoice> options)
        {
            if (label == null)
            {
                throw new BayesianError("Can't create event with null label name");
            }

            if (EventExists(label))
            {
                throw new BayesianError("The label \"" + label
                        + "\" has already been defined.");
            }

            BayesianEvent e;

            if (options.Count == 0)
            {
                e = new BayesianEvent(label);
            }
            else
            {
                e = new BayesianEvent(label, options);

            }
            CreateEvent(e);
            return e;
        }

        /// <summary>
        /// Create the specified events based on a variable number of options, or choices. 
        /// </summary>
        /// <param name="label">The label of the event to create.</param>
        /// <param name="options">The states that the event can have.</param>
        /// <returns>The newly created event.</returns>
        public BayesianEvent CreateEvent(String label, params String[] options)
        {
            if (label == null)
            {
                throw new BayesianError("Can't create event with null label name");
            }

            if (EventExists(label))
            {
                throw new BayesianError("The label \"" + label
                        + "\" has already been defined.");
            }

            BayesianEvent e;

            if (options.Length == 0)
            {
                e = new BayesianEvent(label);
            }
            else
            {
                e = new BayesianEvent(label, options);

            }
            CreateEvent(e);
            return e;
        }

        /// <summary>
        /// Create a dependency between two events. 
        /// </summary>
        /// <param name="parentEvent">The parent event.</param>
        /// <param name="childEvent">The child event.</param>
        public void CreateDependency(BayesianEvent parentEvent,
                BayesianEvent childEvent)
        {
            // does the dependency exist?
            if (!HasDependency(parentEvent, childEvent))
            {
                // create the dependency
                parentEvent.AddChild(childEvent);
                childEvent.AddParent(parentEvent);
            }
        }
        
        /// <summary>
        /// Determine if the two events have a dependency. 
        /// </summary>
        /// <param name="parentEvent">The parent event.</param>
        /// <param name="childEvent">The child event.</param>
        /// <returns>True if a dependency exists.</returns>
        private bool HasDependency(BayesianEvent parentEvent,
                BayesianEvent childEvent)
        {
            return (parentEvent.Children.Contains(childEvent));
        }

        /// <summary>
        /// Create a dependency between a parent and multiple children. 
        /// </summary>
        /// <param name="parentEvent">The parent event.</param>
        /// <param name="children">The child events.</param>
        public void CreateDependency(BayesianEvent parentEvent,
                params BayesianEvent[] children)
        {
            foreach (BayesianEvent childEvent in children)
            {
                parentEvent.AddChild(childEvent);
                childEvent.AddParent(parentEvent);
            }
        }

        /// <summary>
        /// Create a dependency between two labels.
        /// </summary>
        /// <param name="parentEventLabel">The parent event.</param>
        /// <param name="childEventLabel">The child event.</param>
        public void CreateDependency(String parentEventLabel, String childEventLabel)
        {
            BayesianEvent parentEvent = GetEventError(parentEventLabel);
            BayesianEvent childEvent = GetEventError(childEventLabel);
            CreateDependency(parentEvent, childEvent);
        }

        /// <summary>
        /// The contents as a string. Shows both events and dependences.
        /// </summary>
        public String Contents
        {
            get
            {
                StringBuilder result = new StringBuilder();
                bool first = true;

                foreach (BayesianEvent e in this.events)
                {
                    if (!first)
                        result.Append(" ");
                    first = false;
                    result.Append(e.ToFullString());
                }

                return result.ToString();
            }
            set
            {
                IList<ParsedProbability> list = ParseProbability.ParseProbabilityList(this, value);
                IList<String> labelList = new List<String>();

                // ensure that all events are there
                foreach (ParsedProbability prob in list)
                {
                    ParsedEvent parsedEvent = prob.ChildEvent;
                    String eventLabel = parsedEvent.Label;
                    labelList.Add(eventLabel);

                    // create event, if not already here
                    BayesianEvent e = GetEvent(eventLabel);
                    if (e == null)
                    {
                        IList<BayesianChoice> cl = new List<BayesianChoice>();

                        foreach (ParsedChoice c in parsedEvent.ChoiceList)
                        {
                            cl.Add(new BayesianChoice(c.Label, c.Min, c.Max));
                        }

                        CreateEvent(eventLabel, cl);
                    }
                }


                // now remove all events that were not covered
                for (int i = 0; i < events.Count; i++)
                {
                    BayesianEvent e = this.events[i];
                    if (!labelList.Contains(e.Label))
                    {
                        RemoveEvent(e);
                    }
                }

                // handle dependencies
                foreach (ParsedProbability prob in list)
                {
                    ParsedEvent parsedEvent = prob.ChildEvent;
                    String eventLabel = parsedEvent.Label;

                    BayesianEvent e = RequireEvent(eventLabel);

                    // ensure that all "givens" are present
                    IList<String> givenList = new List<String>();
                    foreach (ParsedEvent given in prob.GivenEvents)
                    {
                        if (!e.HasGiven(given.Label))
                        {
                            BayesianEvent givenEvent = RequireEvent(given.Label);
                            this.CreateDependency(givenEvent, e);
                        }
                        givenList.Add(given.Label);
                    }

                    // now remove givens that were not covered
                    for (int i = 0; i < e.Parents.Count; i++)
                    {
                        BayesianEvent event2 = e.Parents[i];
                        if (!givenList.Contains(event2.Label))
                        {
                            RemoveDependency(event2, e);
                        }
                    }
                }

                // finalize the structure
                FinalizeStructure();
                if (this.Query != null)
                {
                    this.Query.FinalizeStructure();
                }

            }
        }

        /// <summary>
        /// Remove a dependency, if it it exists.
        /// </summary>
        /// <param name="parent">The parent event.</param>
        /// <param name="child">The child event.</param>
        private void RemoveDependency(BayesianEvent parent, BayesianEvent child)
        {
            parent.Children.Remove(child);
            child.Parents.Remove(parent);

        }

        /// <summary>
        /// Remove the specified event.
        /// </summary>
        /// <param name="theEvent">The event to remove.</param>
        private void RemoveEvent(BayesianEvent theEvent)
        {
            foreach (BayesianEvent e in theEvent.Parents)
            {
                e.Children.Remove(theEvent);
            }
            this.eventMap.Remove(theEvent.Label);
            this.events.Remove(theEvent);
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            bool first = true;

            foreach (BayesianEvent e in this.events)
            {
                if (!first)
                    result.Append(" ");
                first = false;
                result.Append(e.ToString());
            }

            return result.ToString();
        }

        /**
         * @return The number of parameters in this Bayesian network.
         */
        public int CalculateParameterCount()
        {
            int result = 0;
            foreach (BayesianEvent e in this.eventMap.Values)
            {
                result += e.CalculateParameterCount();
            }
            return result;
        }

        /**
         * Finalize the structure of this Bayesian network.
         */
        public void FinalizeStructure()
        {
            foreach (BayesianEvent e in this.eventMap.Values)
            {
                e.finalizeStructure();
            }

            if (this.Query != null)
            {
                this.Query.FinalizeStructure();
            }

            this.inputPresent = new bool[this.events.Count];
            EngineArray.Fill(this.inputPresent, true);
            this.classificationTarget = -1;
        }

        /// <summary>
        /// Validate the structure of this Bayesian network.
        /// </summary>
        public void Validate()
        {
            foreach (BayesianEvent e in this.eventMap.Values)
            {
                e.Validate();
            }
        }

        /// <summary>
        /// Determine if one Bayesian event is in an array of others. 
        /// </summary>
        /// <param name="given">The events to check.</param>
        /// <param name="e">See if e is amoung given.</param>
        /// <returns>True if e is amoung given.</returns>
        private bool IsGiven(BayesianEvent[] given, BayesianEvent e)
        {
            foreach (BayesianEvent e2 in given)
            {
                if (e == e2)
                    return true;
            }

            return false;
        }


        /// <summary>
        /// Determine if one event is a descendant of another.
        /// </summary>
        /// <param name="a">The event to check.</param>
        /// <param name="b">The event that has children.</param>
        /// <returns>True if a is amoung b's children.</returns>
        public bool IsDescendant(BayesianEvent a, BayesianEvent b)
        {
            if (a == b)
                return true;

            foreach (BayesianEvent e in b.Children)
            {
                if (IsDescendant(a, e))
                    return true;
            }
            return false;
        }

        
        /// <summary>
        /// True if this event is given or conditionally dependant on the others. 
        /// </summary>
        /// <param name="given">The others to check.</param>
        /// <param name="e">The event to check.</param>
        /// <returns>True, if is given or descendant.</returns>
        private bool IsGivenOrDescendant(BayesianEvent[] given, BayesianEvent e)
        {
            foreach (BayesianEvent e2 in given)
            {
                if (IsDescendant(e2, e))
                    return true;
            }

            return false;
        }


        /// <summary>
        /// Help determine if one event is conditionally independent of another.
        /// </summary>
        /// <param name="previousHead">The previous head, as we traverse the list.</param>
        /// <param name="a">The event to check.</param>
        /// <param name="goal">List of events searched.</param>
        /// <param name="searched"></param>
        /// <param name="given">Given events.</param>
        /// <returns>True if conditionally independent.</returns>
        private bool IsCondIndependent(bool previousHead, BayesianEvent a,
                BayesianEvent goal, IDictionary<BayesianEvent, Object> searched,
                params BayesianEvent[] given)
        {

            // did we find it?
            if (a == goal)
            {
                return false;
            }

            // search children
            foreach (BayesianEvent e in a.Children)
            {
                if (!searched.ContainsKey(e) || !IsGiven(given, a))
                {
                    searched[e] = null;
                    if (!IsCondIndependent(true, e, goal, searched, given))
                        return false;
                }
            }

            // search parents
            foreach (BayesianEvent e in a.Parents)
            {
                if (!searched.ContainsKey(e))
                {
                    searched[e] = null;
                    if (!previousHead || IsGivenOrDescendant(given, a))
                        if (!IsCondIndependent(false, e, goal, searched, given))
                            return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determine if two events are conditionally independent.
        /// </summary>
        /// <param name="a">The first event.</param>
        /// <param name="b">The second event.</param>
        /// <param name="given">What is "given".</param>
        /// <returns>True of they are cond. independent.</returns>
        public bool IsCondIndependent(BayesianEvent a, BayesianEvent b,
                params BayesianEvent[] given)
        {
            IDictionary<BayesianEvent, Object> searched = new Dictionary<BayesianEvent, Object>();
            return IsCondIndependent(false, a, b, searched, given);
        }

        /// <inheritdoc/>
        public int InputCount
        {
            get
            {
                return this.events.Count;
            }
        }

        /// <inheritdoc/>
        public int OutputCount
        {
            get
            {
                return 1;
            }
        }

        /// <inheritdoc/>
        public double ComputeProbability(IMLData input)
        {

            // copy the input to evidence
            int inputIndex = 0;
            for (int i = 0; i < this.events.Count; i++)
            {
                BayesianEvent e = this.events[i];
                EventState state = Query.GetEventState(e);
                if (state.CurrentEventType == EventType.Evidence)
                {
                    state.Value = ((int)input[inputIndex++]);
                }
            }

            // execute the query
            this.Query.Execute();

            return this.Query.Probability;
        }


        /// <summary>
        /// Define the probability for an event.
        /// </summary>
        /// <param name="line">The event.</param>
        /// <param name="probability">The probability.</param>
        public void DefineProbability(String line, double probability)
        {
            ParseProbability parse = new ParseProbability(this);
            ParsedProbability parsedProbability = parse.Parse(line);
            parsedProbability.DefineTruthTable(this, probability);
        }

        /// <summary>
        /// Define a probability.
        /// </summary>
        /// <param name="line">The line to define the probability.</param>
        public void DefineProbability(String line)
        {
            int index = line.LastIndexOf('=');
            bool error = false;
            double prob = 0.0;
            String left = "";
            String right = "";

            if (index != -1)
            {
                left = line.Substring(0, index);
                right = line.Substring(index + 1);

                try
                {
                    prob = CSVFormat.EgFormat.Parse(right);
                }
                catch (FormatException ex)
                {
                    error = true;
                }
            }

            if (error || index == -1)
            {
                throw new BayesianError("Probability must be of the form \"P(event|condition1,condition2,etc.)=0.5\".  Conditions are optional.");
            }
            DefineProbability(left, prob);
        }

        /// <summary>
        /// Require the specified event, thrown an error if it does not exist.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <returns>The event.</returns>
        public BayesianEvent RequireEvent(String label)
        {
            BayesianEvent result = GetEvent(label);
            if (result == null)
            {
                throw new BayesianError("The event " + label + " is not defined.");
            }
            return result;
        }

        /// <summary>
        /// Define a relationship.
        /// </summary>
        /// <param name="line">The relationship to define.</param>
        public void DefineRelationship(String line)
        {
            ParseProbability parse = new ParseProbability(this);
            ParsedProbability parsedProbability = parse.Parse(line);
            parsedProbability.DefineRelationships(this);
        }

        /// <summary>
        /// Perform a query.
        /// </summary>
        /// <param name="line">The query.</param>
        /// <returns>The probability.</returns>
        public double PerformQuery(String line)
        {
            if (this.Query == null)
            {
                throw new BayesianError("This Bayesian network does not have a query to define.");
            }

            ParseProbability parse = new ParseProbability(this);
            ParsedProbability parsedProbability = parse.Parse(line);

            // create a temp query
            IBayesianQuery q = this.Query.Clone();

            // first, mark all events as hidden
            q.Reset();

            // deal with evidence (input)
            foreach (ParsedEvent parsedEvent in parsedProbability.GivenEvents)
            {
                BayesianEvent e = this.RequireEvent(parsedEvent.Label);
                q.DefineEventType(e, EventType.Evidence);
                q.SetEventValue(e, parsedEvent.ResolveValue(e));
            }

            // deal with outcome (output)
            foreach (ParsedEvent parsedEvent in parsedProbability.BaseEvents)
            {
                BayesianEvent e = RequireEvent(parsedEvent.Label);
                q.DefineEventType(e, EventType.Outcome);
                q.SetEventValue(e, parsedEvent.ResolveValue(e));
            }

            q.LocateEventTypes();

            q.Execute();
            return q.Probability;
        }

        /// <inheritdoc/>
        public void UpdateProperties()
        {
            // Not needed		
        }

        public int GetEventIndex(BayesianEvent theEvent)
        {
            for (int i = 0; i < this.events.Count; i++)
            {
                if (theEvent == events[i])
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Remove all relations between nodes.
        /// </summary>
        public void RemoveAllRelations()
        {
            foreach (BayesianEvent e in this.events)
            {
                e.RemoveAllRelations();
            }
        }

        /// <inheritdoc/>
        public void Reset()
        {
            Reset(0);

        }

        /// <inheritdoc/>
        public void Reset(int seed)
        {
            foreach (BayesianEvent e in this.events)
            {
                e.Reset();
            }

        }

        
        /// <summary>
        /// Determine the classes for the specified input. 
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>An array of class indexes.</returns>
        public int[] DetermineClasses(IMLData input)
        {
            int[] result = new int[input.Count];

            for (int i = 0; i < input.Count; i++)
            {
                BayesianEvent e = this.events[i];
                int classIndex = e.MatchChoiceToRange(input[i]);
                result[i] = classIndex;
            }

            return result;
        }

        /// <summary>
        /// Classify the input. 
        /// </summary>
        /// <param name="input">The input to classify.</param>
        /// <returns>The classification.</returns>
        public int Classify(IMLData input)
        {

            if (this.classificationTarget < 0 || this.classificationTarget >= this.events.Count)
            {
                throw new BayesianError("Must specify classification target by calling setClassificationTarget.");
            }

            int[] d = this.DetermineClasses(input);

            // properly tag all of the events
            for (int i = 0; i < this.events.Count; i++)
            {
                BayesianEvent e = this.events[i];
                if (i == this.classificationTarget)
                {
                    this.Query.DefineEventType(e, EventType.Outcome);
                }
                else if (this.inputPresent[i])
                {
                    this.Query.DefineEventType(e, EventType.Evidence);
                    this.Query.SetEventValue(e, d[i]);
                }
                else
                {
                    this.Query.DefineEventType(e, EventType.Hidden);
                    this.Query.SetEventValue(e, d[i]);
                }
            }


            // loop over and try each outcome choice
            BayesianEvent outcomeEvent = this.events[this.classificationTarget];
            this.classificationProbabilities = new double[outcomeEvent.Choices.Count];
            for (int i = 0; i < outcomeEvent.Choices.Count; i++)
            {
                this.Query.SetEventValue(outcomeEvent, i);
                this.Query.Execute();
                classificationProbabilities[i] = this.Query.Probability;
            }


            return EngineArray.MaxIndex(this.classificationProbabilities);
        }


        /// <summary>
        /// Get the classification target. 
        /// </summary>
        public int ClassificationTarget
        {
            get
            {
                return classificationTarget;
            }
        }
        
        /// <summary>
        /// Determine if the specified input is present. 
        /// </summary>
        /// <param name="idx">The index of the input.</param>
        /// <returns>True, if the input is present.</returns>
        public bool IsInputPresent(int idx)
        {
            return this.inputPresent[idx];
        }

        /// <summary>
        /// Define a classification structure of the form P(A|B) = P(C)
        /// </summary>
        /// <param name="line">The structure.</param>
        public void DefineClassificationStructure(String line)
        {
            IList<ParsedProbability> list = ParseProbability.ParseProbabilityList(this, line);

            if (list.Count > 1)
            {
                throw new BayesianError("Must only define a single probability, not a chain.");
            }

            if (list.Count == 0)
            {
                throw new BayesianError("Must define at least one probability.");
            }

            // first define everything to be hidden
            foreach (BayesianEvent e in this.events)
            {
                this.Query.DefineEventType(e, EventType.Hidden);
            }

            // define the base event
            ParsedProbability prob = list[0];

            if (prob.BaseEvents.Count == 0)
            {
                return;
            }

            BayesianEvent be = this.GetEvent(prob.ChildEvent.Label);
            this.classificationTarget = this.events.IndexOf(be);
            this.Query.DefineEventType(be, EventType.Outcome);

            // define the given events
            foreach (ParsedEvent parsedGiven in prob.GivenEvents)
            {
                BayesianEvent given = this.GetEvent(parsedGiven.Label);
                this.Query.DefineEventType(given, EventType.Evidence);
            }

            this.Query.LocateEventTypes();

        }

        /// <summary>
        /// The classification target.
        /// </summary>
        public BayesianEvent ClassificationTargetEvent
        {
            get
            {
                if (this.classificationTarget == -1)
                {
                    throw new BayesianError("No classification target defined.");
                }

                return this.events[this.classificationTarget];
            }
        }

        /// <inheritdoc/>
        public double CalculateError(IMLDataSet data)
        {

            if (!this.HasValidClassificationTarget )
                return 1.0;

            // Call the following just to thrown an error if there is no classification target
            ClassificationTarget.ToString();

            int badCount = 0;
            int totalCount = 0;

            foreach (IMLDataPair pair in data)
            {
                int c = this.Classify(pair.Input);
                totalCount++;
                if (c != pair.Input[this.classificationTarget])
                {
                    badCount++;
                }
            }

            return (double)badCount / (double)totalCount;
        }

        /// <summary>
        /// Returns a string representation of the classification structure.
        ///         Of the form P(a|b,c,d)
        /// </summary>
        public String ClassificationStructure
        {
            get
            {
                StringBuilder result = new StringBuilder();

                result.Append("P(");
                bool first = true;

                for (int i = 0; i < this.Events.Count; i++)
                {
                    BayesianEvent e = this.events[i];
                    EventState state = this.Query.GetEventState(e);
                    if (state.CurrentEventType == EventType.Outcome)
                    {
                        if (!first)
                        {
                            result.Append(",");
                        }
                        result.Append(e.Label);
                        first = false;
                    }
                }

                result.Append("|");

                first = true;
                for (int i = 0; i < this.Events.Count; i++)
                {
                    BayesianEvent e = this.events[i];
                    if (this.Query.GetEventState(e).CurrentEventType == EventType.Evidence)
                    {
                        if (!first)
                        {
                            result.Append(",");
                        }
                        result.Append(e.Label);
                        first = false;
                    }
                }

                result.Append(")");
                return result.ToString();
            }
        }

        /// <summary>
        /// True if this network has a valid classification target.
        /// </summary>
        public bool HasValidClassificationTarget
        {
            get
            {
                if (this.classificationTarget < 0
                        || this.classificationTarget >= this.events.Count)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
