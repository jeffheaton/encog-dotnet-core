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

namespace Encog.App.Generate.Program
{
    /// <summary>
    ///     A function argument for Encog created code.
    /// </summary>
    public class EncogProgramArg
    {
        /// <summary>
        ///     The type of this argument.
        /// </summary>
        private readonly EncogArgType type;

        /// <summary>
        ///     The value of this argument.
        /// </summary>
        private readonly Object value;

        /// <summary>
        ///     Construct the argument. Default to float type.
        /// </summary>
        /// <param name="value">The argument value.</param>
        public EncogProgramArg(double value)
            : this(EncogArgType.Float, "" + value)
        {
        }

        /// <summary>
        ///     Construct the argument.
        /// </summary>
        /// <param name="type">The type of argument.</param>
        /// <param name="value">The value of the argument.</param>
        public EncogProgramArg(EncogArgType type, Object value)
        {
            this.type = type;
            this.value = value;
        }

        /// <summary>
        ///     Construct a floating point arguement from an integer.
        /// </summary>
        /// <param name="value">The value.</param>
        public EncogProgramArg(int value)
            : this(EncogArgType.Float, "" + value)
        {
        }

        /// <summary>
        ///     Construct using an object.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        public EncogProgramArg(Object argValue)
            : this(EncogArgType.ObjectType, argValue)
        {
        }

        /// <summary>
        ///     Construct a string argument.
        /// </summary>
        /// <param name="value">The string value.</param>
        public EncogProgramArg(String value)
            : this(EncogArgType.String, value)
        {
        }

        /// <summary>
        ///     The type of argument.
        /// </summary>
        public EncogArgType Type
        {
            get { return type; }
        }

        /// <summary>
        ///     The value.
        /// </summary>
        public Object Value
        {
            get { return value; }
        }
    }
}
