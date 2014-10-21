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
using Encog.Util.CSV;

namespace Encog.Util
{
    /// <summary>
    /// A class that can be used to parse parameters stored in a map.  Allows the 
    /// params to be accessed as various data types and to be validated.
    /// </summary>
    ///
    public class ParamsHolder
    {
        /// <summary>
        /// The format that numbers will be in.
        /// </summary>
        ///
        private readonly CSVFormat _format;

        /// <summary>
        /// The params that are to be parsed.
        /// </summary>
        ///
        private readonly IDictionary<String, String> _paras;

        /// <summary>
        /// Construct the object. Allow the format to be specified.
        /// </summary>
        ///
        /// <param name="theParams">The params to be used.</param>
        /// <param name="theFormat">The format to be used.</param>
        public ParamsHolder(IDictionary<String, String> theParams, CSVFormat theFormat)
        {
            _paras = theParams;
            _format = theFormat;
        }

        /// <summary>
        /// Construct the object. Allow the format to be specified.
        /// </summary>
        ///
        /// <param name="theParams">The params to be used.</param>
        public ParamsHolder(IDictionary<String, String> theParams) : this(theParams, CSVFormat.EgFormat)
        {
        }


        /// <value>the params</value>
        public IDictionary<String, String> Params
        {
            get { return _paras; }
        }


        /// <summary>
        /// Get a param as a string.
        /// </summary>
        ///
        /// <param name="name">The name of the string.</param>
        /// <param name="required">True if this value is required.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value.</returns>
        public String GetString(String name, bool required, String defaultValue)
        {
            if (_paras.ContainsKey(name) )
            {
                return _paras[name];                
            }
            if (required)
            {
                throw new EncogError("Missing property: " + name);
            }
            return defaultValue;
        }

        /// <summary>
        /// Get a param as a integer.
        /// </summary>
        ///
        /// <param name="name">The name of the integer.</param>
        /// <param name="required">True if this value is required.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value.</returns>
        public int GetInt(String name, bool required, int defaultValue)
        {
            String str = GetString(name, required, null);

            if (str == null)
                return defaultValue;

            try
            {
                return Int32.Parse(str);
            }
            catch (FormatException )
            {
                throw new EncogError("Property " + name
                                     + " has an invalid value of " + str
                                     + ", should be valid integer.");
            }
        }

        /// <summary>
        /// Get a param as a double.
        /// </summary>
        ///
        /// <param name="name">The name of the double.</param>
        /// <param name="required">True if this value is required.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value.</returns>
        public double GetDouble(String name, bool required, double defaultValue)
        {
            String str = GetString(name, required, null);

            if (str == null)
                return defaultValue;

            try
            {
                return _format.Parse(str);
            }
            catch (FormatException )
            {
                throw new EncogError("Property " + name
                                     + " has an invalid value of " + str
                                     + ", should be valid floating point.");
            }
        }

        /// <summary>
        /// Get a param as a boolean.
        /// </summary>
        ///
        /// <param name="name">The name of the double.</param>
        /// <param name="required">True if this value is required.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value.</returns>
        public bool GetBoolean(String name, bool required,
                               bool defaultValue)
        {
            String str = GetString(name, required, null);

            if (str == null)
                return defaultValue;

            if (!str.Equals("true", StringComparison.InvariantCultureIgnoreCase) &&
                !str.Equals("false", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new EncogError("Property " + name
                                     + " has an invalid value of " + str
                                     + ", should be true/false.");
            }

            return str.Equals("true", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
