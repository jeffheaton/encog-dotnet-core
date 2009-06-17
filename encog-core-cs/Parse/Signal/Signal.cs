using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;

namespace Encog.Parse.Signal
{
    /// <summary>
    /// Patterns that the parser should recognize are stored in signals.
    /// </summary>
    public class Signal
    {
        private char value = (char)0;
        private IList<Signal> data = new List<Signal>();
        private IList<String> types = new List<String>();
        private String name;
        private bool ignore;
        private bool delta;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(Signal));

        /// <summary>
        /// Construct an empty signal.
        /// </summary>
        public Signal()
        {
        }

        /// <summary>
        /// Construct a signal with the specified value.
        /// </summary>
        /// <param name="value">The value to construct the signal for.</param>
        public Signal(char value)
        {
            this.value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public Signal(String value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                Signal signal = new Signal(value[i]);
                data.Add(signal);
            }
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

        public IList<Signal> Data
        {
            get
            {
                return this.data;
            }
        }

        public int Size
        {
            get
            {
                return data.Count;
            }
        }

        public void Clear()
        {
            data.Clear();
        }

        public char Value
        {
            get
            {
                return value;
            }
        }

        public bool IsChar
        {
            get
            {
                return (value != 0);
            }
        }

        public Signal Pack(int begin, int end, String type)
        {
            return Pack(begin, end, type, typeof(Signal));
        }

        public Signal Pack(int begin, int end, String type, Type signalClass)
        {
            delta = true;
            Object[] array = data.ToArray();

            // create the new recognized signal of the correct type, 
            // defaults to Signal if none specified
            Signal temp;

            try
            {
                temp = (Signal)Assembly.GetExecutingAssembly().CreateInstance(signalClass.ToString());

            }
            catch (Exception e)
            {
                if (logger.IsErrorEnabled)
                {
                    logger.Error("Exception", e);
                }
                throw new ParseError(e);
            }

            for (int i = 0; i < array.Length; i++)
            {
                Signal signal = (Signal)array[i];
                if ((i >= begin) && (i < end))
                {
                    data.Remove(signal);
                    temp.Add(signal);
                }
            }
            temp.AddType(type);

            // now allow the new type to parse internally.
            // if this new type is just a Signal, nothing will
            // happen, as Signal.parse does nothing.

            temp.Parse();

            // now insert the newly created subsignal into
            // this signal.

            if (begin > data.Count)
                data.Add(temp);
            else
                data.Insert(begin, temp);

            return temp;
        }

        public void Insert(int begin, Signal signal)
        {
            delta = true;
            data.Insert(begin, signal);
        }

        public void Cut(int begin, int end)
        {
            Object[] array = data.ToArray();
            delta = true;

            for (int i = 0; i < array.Length; i++)
            {
                Signal signal = (Signal)array[i];
                if ((i >= begin) && (i < end))
                    data.Remove(signal);
            }
        }


        public String Dump()
        {
            String result = "";

            result += "[";

            if (types.Count > 0)
            {

                foreach (String type in types)
                {
                    if (result.Length > 1)
                        result += ",";
                    result += type;
                }

                result += ":";
            }

            if (!this.IsChar)
            {

                if (data.Count > 1)
                    result += "{";

                foreach (Signal signal in data)
                {
                    result += signal.Dump();
                }
                if (data.Count > 1)
                    result += "}";
            }
            else
                result += value;
            result += "]";
            return result;
        }

        public void Add(Signal signal)
        {
            data.Add(signal);
            delta = true;
        }

        public void AddType(String type)
        {
            types.Add(type);
            delta = true;
        }

        public bool HasType(String str)
        {
            foreach (String type in types)
            {
                if (string.Compare(type, str, true) == 0)
                    return true;
            }
            return false;
        }

        public ICollection<Signal> FindByType(String type)
        {
            ICollection<Signal> result = new List<Signal>();

            foreach (Signal signal in data)
            {
                if (signal.HasType(type))
                    result.Add(signal);
            }
            return result;
        }

        public Signal FindByType(String type, int count)
        {
            foreach (Signal signal in data)
            {

                if (signal.HasType(type))
                {
                    if (count == 0)
                        return signal;
                    count--;
                }

                Signal signal2 = signal.FindByType(name, count);
                if (signal2 != null)
                    return signal2;
            }
            return null;
        }

        public override String ToString()
        {
            String result = "";

            if (this.IsChar)
            {
                return "" + this.Value;
            }
            else
            {
                foreach (Signal signal in data)
                {
                    result += signal.ToString();
                }

            }
            return result;
        }

        public bool Ignore
        {
            get
            {
                return ignore;
            }
            set
            {
                this.ignore = value;
            }
        }

        public void Parse()
        {
        }

        public void ResetDelta()
        {
            delta = false;
        }

        public bool Delta
        {
            get
            {
                return delta;
            }
        }

    }
}
