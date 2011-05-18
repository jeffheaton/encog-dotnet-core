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
        private readonly CSVFormat format;

        /// <summary>
        /// The params that are to be parsed.
        /// </summary>
        ///
        private readonly IDictionary<String, String> paras;

        /// <summary>
        /// Construct the object. Allow the format to be specified.
        /// </summary>
        ///
        /// <param name="theParams">The params to be used.</param>
        /// <param name="theFormat">The format to be used.</param>
        public ParamsHolder(IDictionary<String, String> theParams, CSVFormat theFormat)
        {
            paras = theParams;
            format = theFormat;
        }

        /// <summary>
        /// Construct the object. Allow the format to be specified.
        /// </summary>
        ///
        /// <param name="theParams">The params to be used.</param>
        public ParamsHolder(IDictionary<String, String> theParams) : this(theParams, CSVFormat.EG_FORMAT)
        {
        }


        /// <value>the params</value>
        public IDictionary<String, String> Params
        {
            /// <returns>the params</returns>
            get { return paras; }
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
            if (paras.ContainsKey(name) )
            {
                return paras[name];                
            }
            else
            {
                if (required)
                {
                    throw new EncogError("Missing property: " + name);
                }
                else
                {
                    return defaultValue;
                }
            }                       
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
                return format.Parse(str);
            }
            catch (FormatException ex)
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