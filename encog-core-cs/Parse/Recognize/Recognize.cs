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
using log4net;

namespace Encog.Parse.Recognize
{
    /// <summary>
    /// Holds the template of something that the parser should recognize. This
    /// consists of a collection of RecognizeElement objects that define the
    /// structure of what is to be recognized.
    /// </summary>
    public class Recognize
    {

        /// <summary>
        /// Recognize elements used by this recognize class. 
        /// </summary>
        private IList<RecognizeElement> pattern = new List<RecognizeElement>();
        private int index = 0;
        private String type;
        private int startIndex = -1;
        private int stopIndex;
        private int currentIndex = 0;
        private bool ignore = false;
        private Type signalClass = typeof(Signal.Signal);

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(Recognize));

        /// <summary>
        /// Construct a recognize object.
        /// </summary>
        public Recognize()
        {
            this.type = "Untitled";
        }

        /// <summary>
        /// Construct a recognize object of the specified type.
        /// </summary>
        /// <param name="type"></param>
        public Recognize(String type)
        {
            this.type = type;
        }

        /// <summary>
        /// Add a recognize element.
        /// </summary>
        /// <param name="re">The element to add.</param>
        public void Add(RecognizeElement re)
        {
            this.pattern.Add(re);
        }

        /// <summary>
        /// Create an element.
        /// </summary>
        /// <param name="allow">How many to allow, one or many.</param>
        /// <returns>The new element.</returns>
        public RecognizeElement CreateElement(int allow)
        {
            RecognizeElement result = new RecognizeElement(allow);
            Add(result);
            return result;
        }

        /// <summary>
        /// Should this element be ignored.
        /// </summary>
        public bool Ignore
        {
            get
            {
                return this.ignore;
            }
            set
            {
                this.ignore = value;
            }
        }

        /// <summary>
        /// The pattern that this recognizer should recognize.
        /// </summary>
        public IList<RecognizeElement> Pattern
        {
            get
            {
                return this.pattern;
            }

        }

        /// <summary>
        /// The type of signal this is.
        /// </summary>
        public Type SignalClass
        {
            get
            {
                return this.signalClass;
            }
            set
            {
                this.signalClass = value;
            }
        }

        /// <summary>
        /// Attempt to recognize this signal.
        /// </summary>
        /// <param name="signal">The signal to recognize.</param>
        /// <returns>True if recognized.</returns>
        public bool PerformRecognize(Signal.Signal signal)
        {

            bool found;
            do
            {
                found = RecognizeIteration(signal);
            } while (found);
            return found;

        }

        /// <summary>
        /// Perform one iteration of recognition.
        /// </summary>
        /// <param name="signal">The current input to recognize.</param>
        /// <returns>True if this was recognized.</returns>
        protected bool RecognizeIteration(Signal.Signal signal)
        {
            this.startIndex = -1;
            this.index = 0;
            this.currentIndex = 0;

            Object[] array = signal.Data.ToArray();
            while (this.currentIndex < array.Length)
            {

                RecognizeElement re = this.pattern[this.index];
                Signal.Signal signalElement = (Signal.Signal)array[this.currentIndex];

                if (signalElement.Ignore)
                {
                    this.currentIndex++;
                    continue;
                }

                // System.out.println("Recognize Element:" + signalElement.dump() );
                bool success = re.Recognize(signalElement);

                switch (re.Allow)
                {
                    case RecognizeElement.ALLOW_ONE:
                        if (success)
                        {
                            StartTracking();
                            this.index++;
                        }
                        else
                        {
                            StopTracking();
                        }
                        break;

                    case RecognizeElement.ALLOW_MULTIPLE:
                        if (success)
                        {
                            StartTracking();
                        }
                        else
                        {
                            this.index++;
                        }

                        break;
                }

                if (this.index >= this.pattern.Count)
                {

                    if (this.startIndex != -1)
                    {
                        Signal.Signal temp = signal.Pack(this.startIndex,
                               this.stopIndex, this.type, this.SignalClass);
                        temp.Name = re.Name;
                        temp.Ignore = this.ignore;
                        return true;
                    }
                    this.index = 0;
                }
                this.currentIndex++;

            }
            return false;
        }

        /// <summary>
        /// Start tracking something that was recognized.
        /// </summary>
        private void StartTracking()
        {
            if (this.startIndex == -1)
            {
                this.startIndex = this.currentIndex;
            }
            this.stopIndex = this.currentIndex + 1;
        }

        /// <summary>
        /// Stop tracking what was currently being parsed.
        /// </summary>
        private void StopTracking()
        {
            this.startIndex = -1;
            this.index = 0;
        }

        /// <summary>
        /// Convert this object to a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[Recognize:");
            result.Append(this.type);
            result.Append(']');
            return result.ToString();
        }
    }
}
