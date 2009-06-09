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
        public const int ALLOW_ONE = 1;
        public const int ALLOW_MULTIPLE = 2;

        private String charsKnown = "";
        private int allow = ALLOW_ONE;
        private String name = null;
        private ICollection<AcceptedSignal> recognizedSignals = new List<AcceptedSignal>();

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(RecognizeElement));

        public RecognizeElement()
        {
        }

        public void AddAcceptedSignal(String type, String value)
        {
            AcceptedSignal accepted = new AcceptedSignal(type, value);
            recognizedSignals.Add(accepted);
        }

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

        public RecognizeElement(int allow)
        {
            this.allow = allow;
        }

        public int GetAllow()
        {
            return allow;
        }


        public void AddRange(char low, char high)
        {
            for (char ch = low; ch <= high; ch++)
                charsKnown += ch;
        }

        public void Add(char ch)
        {
            charsKnown += ch;
        }


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
