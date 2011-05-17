using System;
using System.Collections.Generic;
using System.IO;
using Encog.Util.CSV;
using Encog.Util.File;

namespace Encog.App.Analyst.Script.Prop
{
    /// <summary>
    /// Holds constant type information for each of the properties that the script
    /// might have. This constant information allows values to be validated.  
    /// This class is a singleton.
    /// </summary>
    ///
    public sealed class PropertyConstraints
    {
        /// <summary>
        /// The instance.
        /// </summary>
        ///
        private static PropertyConstraints instance;


        /// <summary>
        /// The property data.
        /// </summary>
        ///
        private readonly IDictionary<String, List<PropertyEntry>> data;

        /// <summary>
        /// Private constructor.
        /// </summary>
        ///
        private PropertyConstraints()
        {
            data = new Dictionary<String, List<PropertyEntry>>();
            try
            {
                Stream mask0 = ResourceInputStream
                    .OpenResourceInputStream("Encog.Resources.analyst.csv");
                var csv = new ReadCSV(mask0, false, CSVFormat.EG_FORMAT);

                while (csv.Next())
                {
                    String sectionStr = csv.Get(0);
                    String nameStr = csv.Get(1);
                    String typeStr = csv.Get(2);

                    // determine type
                    PropertyType t = default(PropertyType) /* was: null */;
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
                        t = PropertyType.typeFormat;
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

                    if (data.ContainsKey(sectionStr))
                    {
                        list = data[sectionStr];
                    }
                    else
                    {
                        list = new List<PropertyEntry>();
                        data[sectionStr] = list;
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
            /// <returns>The instance.</returns>
            get
            {
                if (instance == null)
                {
                    instance = new PropertyConstraints();
                }

                return instance;
            }
        }

        /// <summary>
        /// Find an entry based on a string.
        /// </summary>
        ///
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
        /// Get all entries for a section/subsection.
        /// </summary>
        ///
        /// <param name="section">The section to find.</param>
        /// <param name="subSection">The subsection to find.</param>
        /// <returns>A list of property entries.</returns>
        public List<PropertyEntry> GetEntries(String section,
                                              String subSection)
        {
            String key = section + ":" + subSection;
            return data[key];
        }

        /// <summary>
        /// Get a single property entry.  If the section and subsection do 
        /// not exist, an error is thrown.
        /// </summary>
        ///
        /// <param name="section">The section.</param>
        /// <param name="subSection">The subsection.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The property entry, or null if not found.</returns>
        public PropertyEntry GetEntry(String section,
                                      String subSection, String name)
        {
            String key = section.ToUpper() + ":"
                         + subSection.ToUpper();
            IList<PropertyEntry> list = data[key];
            if (list == null)
            {
                throw new AnalystError("Unknown section and subsection: " + section
                                       + "." + subSection);
            }

            foreach (PropertyEntry entry  in  list)
            {
                if (entry.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return entry;
                }
            }

            return null;
        }
    }
}