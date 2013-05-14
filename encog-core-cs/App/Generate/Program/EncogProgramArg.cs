using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Generate.Program
{
    /// <summary>
    /// A function argument for Encog created code.
    /// </summary>
    public class EncogProgramArg
    {
        /// <summary>
        /// The type of this argument.
        /// </summary>
        EncogArgType type;

        /// <summary>
        /// The value of this argument.
        /// </summary>
        Object value;

        /// <summary>
        /// Construct the argument. Default to float type. 
        /// </summary>
        /// <param name="value">The argument value.</param>
        public EncogProgramArg(double value)
            : this(EncogArgType.Float, "" + value)
        {

        }

        /// <summary>
        /// Construct the argument. 
        /// </summary>
        /// <param name="type">The type of argument.</param>
        /// <param name="value">The value of the argument.</param>
        public EncogProgramArg(EncogArgType type, Object value)
        {
            this.type = type;
            this.value = value;
        }

        /// <summary>
        /// Construct a floating point arguement from an integer. 
        /// </summary>
        /// <param name="value">The value.</param>
        public EncogProgramArg(int value)
            : this(EncogArgType.Float, "" + value)
        {

        }

        /// <summary>
        /// Construct using an object. 
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        public EncogProgramArg(Object argValue)
            : this(EncogArgType.ObjectType, argValue)
        {
        }

        /// <summary>
        /// Construct a string argument. 
        /// </summary>
        /// <param name="value">The string value.</param>
        public EncogProgramArg(String value)
            : this(EncogArgType.String, value)
        {

        }

        /// <summary>
        /// The type of argument.
        /// </summary>
        public EncogArgType Type
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>
        /// The value.
        /// </summary>
        public Object Value
        {
            get
            {
                return this.value;
            }
        }
    }
}
