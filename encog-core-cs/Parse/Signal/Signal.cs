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

        /// <summary>
        /// The name of this signal.
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
        /// The elements of this signal.
        /// </summary>
        public IList<Signal> Data
        {
            get
            {
                return this.data;
            }
        }

        /// <summary>
        /// The size of the signal.
        /// </summary>
        public int Size
        {
            get
            {
                return data.Count;
            }
        }

        /// <summary>
        /// Clear the signal.
        /// </summary>
        public void Clear()
        {
            data.Clear();
        }

        /// <summary>
        /// The value of this signal.
        /// </summary>
        public char Value
        {
            get
            {
                return value;
            }
        }

        /// <summary>
        /// Is this a character?
        /// </summary>
        public bool IsChar
        {
            get
            {
                return (value != 0);
            }
        }

        /// <summary>
        /// Pack the signal into a new signal.
        /// </summary>
        /// <param name="begin">The beginning of where to pack.</param>
        /// <param name="end">The end of where to pack</param>
        /// <param name="type">The type.</param>
        /// <returns>The packed signal.</returns>
        public Signal Pack(int begin, int end, String type)
        {
            return Pack(begin, end, type, typeof(Signal));
        }

        /// <summary>
        /// Pack the signal into a new signal.
        /// </summary>
        /// <param name="begin">The beginning of where to pack.</param>
        /// <param name="end">The end of where to pack.</param>
        /// <param name="type">The type to use.</param>
        /// <param name="signalClass">The signal class to use.</param>
        /// <returns>The packed signal.</returns>
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

        /// <summary>
        /// Insert one signal into another.
        /// </summary>
        /// <param name="begin">Where to begin the insert.</param>
        /// <param name="signal">The signal to insert.</param>
        public void Insert(int begin, Signal signal)
        {
            delta = true;
            data.Insert(begin, signal);
        }

        /// <summary>
        /// Cut an area from the signal.
        /// </summary>
        /// <param name="begin">The beginning of where to cut.</param>
        /// <param name="end">The end of where to cut.</param>
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


        /// <summary>
        /// Dump this signal to a string.
        /// </summary>
        /// <returns>A string that is a dump of the signal.</returns>
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

        /// <summary>
        /// Add one signal to another.
        /// </summary>
        /// <param name="signal">The signal to add.</param>
        public void Add(Signal signal)
        {
            data.Add(signal);
            delta = true;
        }

        /// <summary>
        /// Add a type to the list of types that this signal is.
        /// </summary>
        /// <param name="type">The new type.</param>
        public void AddType(String type)
        {
            types.Add(type);
            delta = true;
        }

        /// <summary>
        /// Does this signal have the specified type.
        /// </summary>
        /// <param name="str">The type to search for.</param>
        /// <returns>True if the signal has the specified type.</returns>
        public bool HasType(String str)
        {
            foreach (String type in types)
            {
                if (string.Compare(type, str, true) == 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Find all parts of this signal by type.
        /// </summary>
        /// <param name="type">The type we are searching for.</param>
        /// <returns>A collection of the types that were found.</returns>
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

        /// <summary>
        /// Find by the specific type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="count"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Convert this object to a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
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

        /// <summary>
        /// Should this signal be ignored?
        /// </summary>
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

        /// <summary>
        /// Called to parse a specific sort of signal.
        /// </summary>
        public void Parse()
        {
        }

        /// <summary>
        /// Reset the delta.
        /// </summary>
        public void ResetDelta()
        {
            delta = false;
        }

        /// <summary>
        /// The current delta value, was there a change?
        /// </summary>
        public bool Delta
        {
            get
            {
                return delta;
            }
        }

    }
}
