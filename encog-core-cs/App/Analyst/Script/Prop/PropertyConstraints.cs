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
using System.IO;
using System.Linq;
using Encog.Util.CSV;
using Encog.Util.File;

namespace Encog.App.Analyst.Script.Prop
{
    /// <summary>
    ///     Holds constant type information for each of the properties that the script
    ///     might have. This constant information allows values to be validated.
    ///     This class is a singleton.
    /// </summary>
    public sealed class PropertyConstraints
    {
        /// <summary>
        ///     The instance.
        /// </summary>
        private static PropertyConstraints _instance;


        /// <summary>
        ///     The property data.
        /// </summary>
        private readonly IDictionary<String, List<PropertyEntry>> _data;

        /// <summary>
        ///     Private constructor.
        /// </summary>
        private PropertyConstraints()
        {
            _data = new Dictionary<String, List<PropertyEntry>>();
            try
            {
                Stream mask0 = ResourceLoader.CreateStream("Encog.Resources.analyst.csv");
                var csv = new ReadCSV(mask0, false, CSVFormat.EgFormat);

                while (csv.Next())
                {
                    String sectionStr = csv.Get(0);
                    String nameStr = csv.Get(1);
                    String typeStr = csv.Get(2);

                    // determine type
                    PropertyType t;
                    if ("boolean".Equals(typeStr, StringComparison.InvariantCultureIgnoreCase))
                    {
                        t = PropertyType.TypeBoolean;
                    }
                    else if ("real".Equals(typeStr, StringComparison.InvariantCultureIgnoreCase))
                    {
                        t = PropertyType.TypeDouble;
                    }
                    else if ("format".Equals(typeStr, StringComparison.InvariantCultureIgnoreCase))
                    {
                        t = PropertyType.TypeFormat;
                    }
                    else if ("int".Equals(typeStr, StringComparison.InvariantCultureIgnoreCase))
                    {
                        t = PropertyType.TypeInteger;
                    }
                    else if ("list-string".Equals(typeStr, StringComparison.InvariantCultureIgnoreCase))
                    {
                        t = PropertyType.TypeListString;
                    }
                    else if ("string".Equals(typeStr, StringComparison.InvariantCultureIgnoreCase))
                    {
                        t = PropertyType.TypeString;
                    }
                    else
                    {
                        throw new AnalystError("Unknown type constraint: "
                                               + typeStr);
                    }

                    var entry = new PropertyEntry(t, nameStr,
                                                  sectionStr);
                    List<PropertyEntry> list;

                    if (_data.ContainsKey(sectionStr))
                    {
                        list = _data[sectionStr];
                    }
                    else
                    {
                        list = new List<PropertyEntry>();
                        _data[sectionStr] = list;
                    }

                    list.Add(entry);
                }

                csv.Close();
                mask0.Close();
            }
            catch (IOException e)
            {
                throw new EncogError(e);
            }
        }

        /// <value>The instance.</value>
        public static PropertyConstraints Instance
        {
            get { return _instance ?? (_instance = new PropertyConstraints()); }
        }

        /// <summary>
        ///     Find an entry based on a string.
        /// </summary>
        /// <param name="v">The property to find.</param>
        /// <returns>The property entry data.</returns>
        public PropertyEntry FindEntry(String v)
        {
            String[] cols = v.Split('.');
            String section = cols[0];
            String subSection = cols[1];
            String name = cols[2];
            return GetEntry(section, subSection, name);
        }

        /// <summary>
        ///     Get all entries for a section/subsection.
        /// </summary>
        /// <param name="section">The section to find.</param>
        /// <param name="subSection">The subsection to find.</param>
        /// <returns>A list of property entries.</returns>
        public List<PropertyEntry> GetEntries(String section,
                                              String subSection)
        {
            String key = section + ":" + subSection;
            return _data[key];
        }

        /// <summary>
        ///     Get a single property entry.  If the section and subsection do
        ///     not exist, an error is thrown.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="subSection">The subsection.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The property entry, or null if not found.</returns>
        public PropertyEntry GetEntry(String section,
                                      String subSection, String name)
        {
            String key = section.ToUpper() + ":"
                         + subSection.ToUpper();
            IList<PropertyEntry> list = _data[key];
            if (list == null)
            {
                throw new AnalystError("Unknown section and subsection: " + section
                                       + "." + subSection);
            }

            return list.FirstOrDefault(entry => entry.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
