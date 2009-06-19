using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Encog.Parse.Recognize
{
    /// <summary>
    /// An individual element of what the parser should recognize, these are
    /// helid in the Recognize class.
    /// </summary>
    public class RecognizeElement
    {
        /// <summary>
        /// Allow one of the element to be recognized.
        /// </summary>
        public const int ALLOW_ONE = 1;

        /// <summary>
        /// Allow multiple of the element to be recognized.
        /// </summary>
        public const int ALLOW_MULTIPLE = 2;

        /// <summary>
        /// The characters known by this recognizer.
        /// </summary>
        private String charsKnown = "";
        private int allow = ALLOW_ONE;
        private String name = null;
        private ICollection<AcceptedSignal> recognizedSignals = new List<AcceptedSignal>();

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(RecognizeElement));

        /// <summary>
        /// Construct a recognize element.
        /// </summary>
        public RecognizeElement()
        {
        }

        /// <summary>
        /// Add a signal of the specified type and value.
        /// </summary>
        /// <param name="type">The type for this signal.</param>
        /// <param name="value">The value for this signal.</param>
        public void AddAcceptedSignal(String type, String value)
        {
            AcceptedSignal accepted = new AcceptedSignal(type, value);
            recognizedSignals.Add(accepted);
        }

        /// <summary>
        /// The name of this element.
        /// </summary>
        public String Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// Create a recognize element.
        /// </summary>
        /// <param name="allow">Allow one, or allow many.</param>
        public RecognizeElement(int allow)
        {
            this.allow = allow;
        }

        /// <summary>
        /// Allow one, or allow many?
        /// </summary>
        public int Allow
        {
            get
            {
                return allow;
            }
        }

        /// <summary>
        /// Add a character range.
        /// </summary>
        /// <param name="low">The low character.</param>
        /// <param name="high">The high character.</param>
        public void AddRange(char low, char high)
        {
            for (char ch = low; ch <= high; ch++)
                charsKnown += ch;
        }

        /// <summary>
        /// Add a single character.
        /// </summary>
        /// <param name="ch">The character to add.</param>
        public void Add(char ch)
        {
            charsKnown += ch;
        }

        /// <summary>
        /// The type of recognizer this is.
        /// </summary>
        public String Type
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        /// <summary>
        /// Attempt to recognize the specified signal.
        /// </summary>
        /// <param name="signal">The signal to recognize.</param>
        /// <returns>True if it was recognized.</returns>
        public bool Recognize(Signal.Signal signal)
        {
            if (!signal.IsChar)
            {
                foreach (AcceptedSignal accepted in recognizedSignals)
                {
                    if (signal.HasType(accepted.Type))
                    {
                        if (accepted.Value == null)
                            return true;
                        if (accepted.Value.Equals(signal.ToString()))
                            return true;
                    }
                }
                return false;
            }
            else
                return (charsKnown.IndexOf(signal.Value) != -1);
        }

        /// <summary>
        /// Convert this object to a string.
        /// </summary>
        /// <returns>The string form of this object.</returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[RecognizeElement:");
            result.Append(this.name);
            result.Append(']');
            return result.ToString();
        }
    }
}
