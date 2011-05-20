using System;
using System.Collections.Generic;
using System.Text;
using Encog.Engine.Network.Activation;
using Encog.MathUtil.Matrices;
using Encog.Util;
using Encog.Util.CSV;

namespace Encog.Persist
{
    /// <summary>
    /// This class is used internally to parse Encog files. A file section is part of
    /// a name-value pair file.
    /// </summary>
    ///
    public class EncogFileSection
    {
        /// <summary>
        /// The lines in this section/subsection.
        /// </summary>
        ///
        private readonly IList<String> lines;

        /// <summary>
        /// The name of this section.
        /// </summary>
        ///
        private readonly String sectionName;

        /// <summary>
        /// The name of this subsection.
        /// </summary>
        ///
        private readonly String subSectionName;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="theSectionName">The section name.</param>
        /// <param name="theSubSectionName">The sub section name.</param>
        public EncogFileSection(String theSectionName,
                                String theSubSectionName)
        {
            lines = new List<String>();
            sectionName = theSectionName;
            subSectionName = theSubSectionName;
        }


        /// <value>The lines.</value>
        public IList<String> Lines
        {
            get { return lines; }
        }


        /// <value>All lines separated by a delimiter.</value>
        public String LinesAsString
        {
            get
            {
                var result = new StringBuilder();

                foreach (String line  in  lines)
                {
                    result.Append(line);
                    result.Append("\n");
                }
                return result.ToString();
            }
        }


        /// <value>The section name.</value>
        public String SectionName
        {
            get { return sectionName; }
        }


        /// <value>The section name.</value>
        public String SubSectionName
        {
            get { return subSectionName; }
        }

        /// <summary>
        /// Parse an activation function from a string.
        /// </summary>
        ///
        /// <param name="paras">The params.</param>
        /// <param name="name">The name of the param to parse.</param>
        /// <returns>The parsed activation function.</returns>
        public static IActivationFunction ParseActivationFunction(
            IDictionary<String, String> paras, String name)
        {
            String value_ren = null;
            try
            {
                value_ren = paras[name];
                if (value_ren == null)
                {
                    throw new PersistError("Missing property: " + name);
                }

                IActivationFunction af = null;
                String[] cols = value_ren.Split('|');

                String afName = ReflectionUtil.AF_PATH
                                + cols[0];
                try
                {
                    af = (IActivationFunction) ReflectionUtil.LoadObject(afName);
                }
                catch (Exception e)
                {
                    throw new PersistError(e);
                }

                for (int i = 0; i < af.ParamNames.Length; i++)
                {
                    af.SetParam(i, CSVFormat.EG_FORMAT.Parse(cols[i + 1]));
                }

                return af;
            }
            catch (Exception ex)
            {
                throw new PersistError(ex);
            }
        }

        /// <summary>
        /// Parse a boolean from a name-value collection of params.
        /// </summary>
        ///
        /// <param name="paras">The name-value pairs.</param>
        /// <param name="name">The name to parse.</param>
        /// <returns>The parsed boolean value.</returns>
        public static bool ParseBoolean(IDictionary<String, String> paras,
                                        String name)
        {
            String value_ren = null;
            try
            {
                value_ren = paras[name];
                if (value_ren == null)
                {
                    throw new PersistError("Missing property: " + name);
                }

                return value_ren.Trim().ToLower()[0] == 't';
            }
            catch (FormatException )
            {
                throw new PersistError("Field: " + name + ", "
                                       + "invalid integer: " + value_ren);
            }
        }

        /// <summary>
        /// Parse a double from a name-value collection of params.
        /// </summary>
        ///
        /// <param name="paras">The name-value pairs.</param>
        /// <param name="name">The name to parse.</param>
        /// <returns>The parsed double value.</returns>
        public static double ParseDouble(IDictionary<String, String> paras,
                                         String name)
        {
            String value_ren = null;
            try
            {
                value_ren = paras[name];
                if (value_ren == null)
                {
                    throw new PersistError("Missing property: " + name);
                }

                return CSVFormat.EG_FORMAT.Parse(value_ren);
            }
            catch (FormatException )
            {
                throw new PersistError("Field: " + name + ", "
                                       + "invalid integer: " + value_ren);
            }
        }

        /// <summary>
        /// Parse a double array from a name-value collection of params.
        /// </summary>
        ///
        /// <param name="paras">The name-value pairs.</param>
        /// <param name="name">The name to parse.</param>
        /// <returns>The parsed double array value.</returns>
        public static double[] ParseDoubleArray(IDictionary<String, String> paras,
                                                String name)
        {
            String v = null;
            try
            {
                v = paras[name];
                if (v == null)
                {
                    throw new PersistError("Missing property: " + name);
                }

                return NumberList.FromList(CSVFormat.EG_FORMAT, v);
            }
            catch (FormatException )
            {
                throw new PersistError("Field: " + name + ", "
                                       + "invalid integer: " + v);
            }
        }

        /// <summary>
        /// Parse an int from a name-value collection of params.
        /// </summary>
        ///
        /// <param name="paras">The name-value pairs.</param>
        /// <param name="name">The name to parse.</param>
        /// <returns>The parsed int value.</returns>
        public static int ParseInt(IDictionary<String, String> paras,
                                   String name)
        {
            String value_ren = null;
            try
            {
                value_ren = paras[name];
                if (value_ren == null)
                {
                    throw new PersistError("Missing property: " + name);
                }

                return Int32.Parse(value_ren);
            }
            catch (FormatException ex)
            {
                throw new PersistError("Field: " + name + ", "
                                       + "invalid integer: " + value_ren);
            }
        }

        /// <summary>
        /// Parse an int array from a name-value collection of params.
        /// </summary>
        ///
        /// <param name="paras">The name-value pairs.</param>
        /// <param name="name">The name to parse.</param>
        /// <returns>The parsed int array value.</returns>
        public static int[] ParseIntArray(IDictionary<String, String> paras,
                                          String name)
        {
            String value_ren = null;
            try
            {
                value_ren = paras[name];
                if (value_ren == null)
                {
                    throw new PersistError("Missing property: " + name);
                }

                return NumberList.FromListInt(CSVFormat.EG_FORMAT, value_ren);
            }
            catch (FormatException )
            {
                throw new PersistError("Field: " + name + ", "
                                       + "invalid integer: " + value_ren);
            }
        }

        /// <summary>
        /// Parse a matrix from a name-value collection of params.
        /// </summary>
        ///
        /// <param name="paras">The name-value pairs.</param>
        /// <param name="name">The name to parse.</param>
        /// <returns>The parsed matrix value.</returns>
        public static Matrix ParseMatrix(IDictionary<String, String> paras,
                                         String name)
        {
            if (!paras.ContainsKey(name))
            {
                throw new PersistError("Missing property: " + name);
            }

            String line = paras[name];

            double[] d = NumberList.FromList(CSVFormat.EG_FORMAT, line);
            var rows = (int) d[0];
            var cols = (int) d[1];

            var result = new Matrix(rows, cols);

            int index = 2;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    result[r, c] = d[index++];
                }
            }

            return result;
        }

        /// <summary>
        /// Split a delimited string into columns.
        /// </summary>
        ///
        /// <param name="line">THe string to split.</param>
        /// <returns>The string split.</returns>
        public static IList<String> SplitColumns(String line)
        {
            IList<String> result = new List<String>();
            string[] tok = line.Split(',');
            for (int i = 0; i < tok.Length; i++)
            {
                String str = tok[i].Trim();
                if ((str.Length > 0) && (str[0] == '\"'))
                {
                    str = str.Substring(1);
                    if (str.EndsWith("\""))
                    {
                        str = str.Substring(0, (str.Length - 1) - (0));
                    }
                }
                result.Add(str);
            }
            return result;
        }


        /// <returns>The params.</returns>
        public IDictionary<String, String> ParseParams()
        {
            IDictionary<String, String> result = new Dictionary<String, String>();


            foreach (String line  in  lines)
            {
                String line2 = line.Trim();
                if (line2.Length > 0)
                {
                    int idx = line2.IndexOf('=');
                    if (idx == -1)
                    {
                        throw new EncogError("Invalid setup item: " + line);
                    }
                    String name = line2.Substring(0, (idx) - (0)).Trim();
                    String value_ren = line2.Substring(idx + 1).Trim();

                    result[name] = value_ren;
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" sectionName=");
            result.Append(sectionName);
            result.Append(", subSectionName=");
            result.Append(subSectionName);
            result.Append("]");
            return result.ToString();
        }
    }
}