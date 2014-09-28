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
        /// Any large arrays that were read.
        /// </summary>
        private IList<double[]> _largeArrays = new List<double[]>();

        /// <summary>
        /// The lines in this section/subsection.
        /// </summary>
        ///
        private readonly IList<String> _lines;

        /// <summary>
        /// The name of this section.
        /// </summary>
        ///
        private readonly String _sectionName;

        /// <summary>
        /// The name of this subsection.
        /// </summary>
        ///
        private readonly String _subSectionName;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="theSectionName">The section name.</param>
        /// <param name="theSubSectionName">The sub section name.</param>
        public EncogFileSection(String theSectionName,
                                String theSubSectionName)
        {
            _lines = new List<String>();
            _sectionName = theSectionName;
            _subSectionName = theSubSectionName;
        }

        /// <summary>
        /// Parse an activation function from a value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The activation function.</returns>
        public static IActivationFunction ParseActivationFunction(string value) {
            IActivationFunction af;
            String[] cols = value.Split('|');

            String afName = ReflectionUtil.AfPath
                            + cols[0];
            try
            {
                af = (IActivationFunction)ReflectionUtil.LoadObject(afName);
            }
            catch (Exception e)
            {
                throw new PersistError(e);
            }

            for (int i = 0; i < af.ParamNames.Length; i++)
            {
                af.Params[i] = CSVFormat.EgFormat.Parse(cols[i + 1]);
            }

            return af;
	    }


        /// <value>The lines.</value>
        public IList<String> Lines
        {
            get { return _lines; }
        }


        /// <value>All lines separated by a delimiter.</value>
        public String LinesAsString
        {
            get
            {
                var result = new StringBuilder();

                foreach (String line  in  _lines)
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
            get { return _sectionName; }
        }


        /// <value>The section name.</value>
        public String SubSectionName
        {
            get { return _subSectionName; }
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
            String v;
            try
            {
                v = paras[name];
                if (v == null)
                {
                    throw new PersistError("Missing property: " + name);
                }

                IActivationFunction af;
                String[] cols = v.Split('|');

                String afName = ReflectionUtil.AfPath
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
                    af.Params[i] = CSVFormat.EgFormat.Parse(cols[i + 1]);
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
            String v = null;
            try
            {
                v = paras[name];
                if (v == null)
                {
                    throw new PersistError("Missing property: " + name);
                }

                return v.Trim().ToLower()[0] == 't';
            }
            catch (FormatException )
            {
                throw new PersistError("Field: " + name + ", "
                                       + "invalid integer: " + v);
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
            String v = null;
            try
            {
                v = paras[name];
                if (v == null)
                {
                    throw new PersistError("Missing property: " + name);
                }

                return CSVFormat.EgFormat.Parse(v);
            }
            catch (FormatException )
            {
                throw new PersistError("Field: " + name + ", "
                                       + "invalid integer: " + v);
            }
        }

        /// <summary>
        /// Parse a double array from a name-value collection of params.
        /// </summary>
        ///
        /// <param name="paras">The name-value pairs.</param>
        /// <param name="name">The name to parse.</param>
        /// <returns>The parsed double array value.</returns>
        public double[] ParseDoubleArray(IDictionary<String, String> paras,
                                                String name)
        {
            String v = null;
            try
            {
                
                if ( !paras.ContainsKey(name) )
                {
                    throw new PersistError("Missing property: " + name);
                }

                v = paras[name];

                if (v.StartsWith("##"))
                {
                    int i = int.Parse(v.Substring(2));
                    return _largeArrays[i];
                }
                else
                {
                    return NumberList.FromList(CSVFormat.EgFormat, v);
                }
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
            String v = null;
            try
            {
                v = paras[name];
                if (v == null)
                {
                    throw new PersistError("Missing property: " + name);
                }

                return Int32.Parse(v);
            }
            catch (FormatException)
            {
                throw new PersistError("Field: " + name + ", "
                                       + "invalid integer: " + v);
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
            String v = null;
            try
            {
                v = paras[name];
                if (v == null)
                {
                    throw new PersistError("Missing property: " + name);
                }

                return NumberList.FromListInt(CSVFormat.EgFormat, v);
            }
            catch (FormatException )
            {
                throw new PersistError("Field: " + name + ", "
                                       + "invalid integer: " + v);
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

            double[] d = NumberList.FromList(CSVFormat.EgFormat, line);
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
            foreach (string t in tok)
            {
                String str = t.Trim();
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


            foreach (String line  in  _lines)
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
                    String v = line2.Substring(idx + 1).Trim();

                    result[name] = v;
                }
            }

            return result;
        }

        /// <summary>
        /// Large arrays.
        /// </summary>
        public IList<double[]> LargeArrays
        {
            get
            {
                return _largeArrays;
            }
            set
            {
                _largeArrays = value;
            }
        }

        /// <inheritdoc/>
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" sectionName=");
            result.Append(_sectionName);
            result.Append(", subSectionName=");
            result.Append(_subSectionName);
            result.Append("]");
            return result.ToString();
        }
    }
}
