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

        public Recognize()
        {
            this.type = "Untitled";
        }

        public Recognize(String type)
        {
            this.type = type;
        }

        public void Add(RecognizeElement re)
        {
            this.pattern.Add(re);
        }

        public RecognizeElement CreateElement(int allow)
        {
            RecognizeElement result = new RecognizeElement(allow);
            Add(result);
            return result;
        }

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

        public IList<RecognizeElement> Pattern
        {
            get
            {
                return this.pattern;
            }

        }

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

                switch (re.GetAllow())
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
