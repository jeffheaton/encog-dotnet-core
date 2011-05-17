using System;
using System.Text;
using Encog.App.Analyst.Util;
using Encog.Util.CSV;

namespace Encog.App.Analyst.Script.Prop
{
    /// <summary>
    /// A property entry for the Encog Analyst. Properties have a name and section.
    /// </summary>
    ///
    public class PropertyEntry : IComparable<PropertyEntry>
    {
        /// <summary>
        /// The type of property.
        /// </summary>
        ///
        private readonly PropertyType entryType;

        /// <summary>
        /// The name of the property.
        /// </summary>
        ///
        private readonly String name;

        /// <summary>
        /// The section of the property.
        /// </summary>
        ///
        private readonly String section;

        /// <summary>
        /// Construct a property entry.
        /// </summary>
        ///
        /// <param name="theEntryType">The entry type.</param>
        /// <param name="theName">The name of the property.</param>
        /// <param name="theSection">The section of the property.</param>
        public PropertyEntry(PropertyType theEntryType, String theName,
                             String theSection)
        {
            entryType = theEntryType;
            name = theName;
            section = theSection;
        }


        /// <value>the entryType</value>
        public PropertyType EntryType
        {
            /// <returns>the entryType</returns>
            get { return entryType; }
        }


        /// <value>The key.</value>
        public String Key
        {
            /// <returns>The key.</returns>
            get { return section + "_" + name; }
        }


        /// <value>the name</value>
        public String Name
        {
            /// <returns>the name</returns>
            get { return name; }
        }


        /// <value>the section</value>
        public String Section
        {
            /// <returns>the section</returns>
            get { return section; }
        }

        #region IComparable<PropertyEntry> Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public int CompareTo(PropertyEntry o)
        {
            return String.CompareOrdinal(name, o.name);
        }

        #endregion

        /// <summary>
        /// Put a property in dot form, which is "section.subsection.name".
        /// </summary>
        ///
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
        /// 
        /// </summary>
        ///
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" name=");
            result.Append(name);
            result.Append(", section=");
            result.Append(section);
            result.Append("]");
            return result.ToString();
        }

        /// <summary>
        /// Validate the specified property.
        /// </summary>
        ///
        /// <param name="theSection">The section.</param>
        /// <param name="subSection">The sub section.</param>
        /// <param name="theName">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        public void Validate(String theSection,
                             String subSection, String theName, String value_ren)
        {
            if ((value_ren == null) || (value_ren.Length == 0))
            {
                return;
            }

            try
            {
                switch (EntryType)
                {
                    case PropertyType.TypeBoolean:
                        if ((Char.ToUpper(value_ren[0]) != 'T')
                            && (Char.ToUpper(value_ren[0]) != 'F'))
                        {
                            var result = new StringBuilder();
                            result.Append("Illegal boolean for ");
                            result.Append(DotForm(section, subSection,
                                                  name));
                            result.Append(", value is ");
                            result.Append(value_ren);
                            result.Append(".");
                            throw new AnalystError(result.ToString());
                        }
                        break;
                    case PropertyType.TypeDouble:
                        CSVFormat.EG_FORMAT.Parse(value_ren);
                        break;
                    case PropertyType.typeFormat:
                        if (ConvertStringConst.String2AnalystFileFormat(value_ren) == null)
                        {
                            var result_0 = new StringBuilder();
                            result_0.Append("Invalid file format for ");
                            result_0.Append(DotForm(section, subSection,
                                                    name));
                            result_0.Append(", value is ");
                            result_0.Append(value_ren);
                            result_0.Append(".");
                            throw new AnalystError(result_0.ToString());
                        }
                        break;
                    case PropertyType.TypeInteger:
                        Int32.Parse(value_ren);
                        break;
                    case PropertyType.TypeListString:
                        break;
                    case PropertyType.TypeString:
                        break;
                    default:
                        throw new AnalystError("Unsupported property type.");
                }
            }
            catch (FormatException ex)
            {
                var result_1 = new StringBuilder();
                result_1.Append("Illegal value for ");
                result_1.Append(DotForm(section, subSection, name));
                result_1.Append(", expecting a ");
                result_1.Append(EntryType.ToString());
                result_1.Append(", but got ");
                result_1.Append(value_ren);
                result_1.Append(".");
                throw new AnalystError(result_1.ToString());
            }
        }
    }
}