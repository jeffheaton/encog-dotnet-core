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
using System.Text;
using Encog.App.Analyst.Util;
using Encog.Util.CSV;

namespace Encog.App.Analyst.Script.Prop
{
    /// <summary>
    ///     A property entry for the Encog Analyst. Properties have a name and section.
    /// </summary>
    public class PropertyEntry : IComparable<PropertyEntry>
    {
        /// <summary>
        ///     The type of property.
        /// </summary>
        private readonly PropertyType _entryType;

        /// <summary>
        ///     The name of the property.
        /// </summary>
        private readonly String _name;

        /// <summary>
        ///     The section of the property.
        /// </summary>
        private readonly String _section;

        /// <summary>
        ///     Construct a property entry.
        /// </summary>
        /// <param name="theEntryType">The entry type.</param>
        /// <param name="theName">The name of the property.</param>
        /// <param name="theSection">The section of the property.</param>
        public PropertyEntry(PropertyType theEntryType, String theName,
                             String theSection)
        {
            _entryType = theEntryType;
            _name = theName;
            _section = theSection;
        }


        /// <value>the entryType</value>
        public PropertyType EntryType
        {
            get { return _entryType; }
        }


        /// <value>The key.</value>
        public String Key
        {
            get { return _section + "_" + _name; }
        }


        /// <value>the name</value>
        public String Name
        {
            get { return _name; }
        }


        /// <value>the section</value>
        public String Section
        {
            get { return _section; }
        }

        #region IComparable<PropertyEntry> Members

        /// <summary>
        /// </summary>
        public int CompareTo(PropertyEntry o)
        {
            return String.CompareOrdinal(_name, o._name);
        }

        #endregion

        /// <summary>
        ///     Put a property in dot form, which is "section.subsection.name".
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="subSection">The subsection.</param>
        /// <param name="name">The name.</param>
        /// <returns>The property in dot form.</returns>
        public static String DotForm(String section, String subSection,
                                     String name)
        {
            var result = new StringBuilder();
            result.Append(section);
            result.Append('.');
            result.Append(subSection);
            result.Append('.');
            result.Append(name);
            return result.ToString();
        }


        /// <summary>
        /// </summary>
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" name=");
            result.Append(_name);
            result.Append(", section=");
            result.Append(_section);
            result.Append("]");
            return result.ToString();
        }

        /// <summary>
        ///     Validate the specified property.
        /// </summary>
        /// <param name="theSection">The section.</param>
        /// <param name="subSection">The sub section.</param>
        /// <param name="theName">The name of the property.</param>
        /// <param name="v">The value of the property.</param>
        public void Validate(String theSection,
                             String subSection, String theName, String v)
        {
            if (string.IsNullOrEmpty(v))
            {
                return;
            }

            try
            {
                switch (EntryType)
                {
                    case PropertyType.TypeBoolean:
                        if ((Char.ToUpper(v[0]) != 'T')
                            && (Char.ToUpper(v[0]) != 'F'))
                        {
                            var result = new StringBuilder();
                            result.Append("Illegal boolean for ");
                            result.Append(DotForm(_section, subSection,
                                                  _name));
                            result.Append(", value is ");
                            result.Append(v);
                            result.Append(".");
                            throw new AnalystError(result.ToString());
                        }
                        break;
                    case PropertyType.TypeDouble:
                        CSVFormat.EgFormat.Parse(v);
                        break;
                    case PropertyType.TypeFormat:
                        if (ConvertStringConst.String2AnalystFileFormat(v) == AnalystFileFormat.Unknown)
                        {
                            var result = new StringBuilder();
                            result.Append("Invalid file format for ");
                            result.Append(DotForm(_section, subSection,
                                                  _name));
                            result.Append(", value is ");
                            result.Append(v);
                            result.Append(".");
                            throw new AnalystError(result.ToString());
                        }
                        break;
                    case PropertyType.TypeInteger:
                        Int32.Parse(v);
                        break;
                    case PropertyType.TypeListString:
                        break;
                    case PropertyType.TypeString:
                        break;
                    default:
                        throw new AnalystError("Unsupported property type.");
                }
            }
            catch (FormatException)
            {
                var result = new StringBuilder();
                result.Append("Illegal value for ");
                result.Append(DotForm(_section, subSection, _name));
                result.Append(", expecting a ");
                result.Append(EntryType.ToString());
                result.Append(", but got ");
                result.Append(v);
                result.Append(".");
                throw new AnalystError(result.ToString());
            }
        }
    }
}
