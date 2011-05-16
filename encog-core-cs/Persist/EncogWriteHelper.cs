using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Encog.Engine.Network.Activation;
using Encog.MathUtil.Matrices;
using Encog.Util.CSV;

namespace Encog.Persist
{
    /// <summary>
    /// Used to write an Encog EG/EGA file. EG files are used to hold Encog objects.
    /// EGA files are used to hold Encog Analyst scripts.
    /// </summary>
    ///
    public class EncogWriteHelper
    {
        /// <summary>
        /// A quote char.
        /// </summary>
        ///
        public const char QUOTE = '\"';

        /// <summary>
        /// A comma char.
        /// </summary>
        ///
        public const char COMMA = ',';

        /// <summary>
        /// The current line.
        /// </summary>
        ///
        private readonly StringBuilder line;

        /// <summary>
        /// The file to write to.
        /// </summary>
        ///
        private readonly StreamWriter xout;

        /// <summary>
        /// The current section.
        /// </summary>
        ///
        private String currentSection;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="stream">The stream to write to.</param>
        public EncogWriteHelper(Stream stream)
        {
            line = new StringBuilder();
            xout = new StreamWriter(stream);
        }

        /// <value>The current section.</value>
        public String CurrentSection
        {
            /// <returns>The current section.</returns>
            get { return currentSection; }
        }

        /// <summary>
        /// Add a boolean value as a column.  
        /// </summary>
        ///
        /// <param name="b">The boolean value.</param>
        public void AddColumn(bool b)
        {
            if (line.Length > 0)
            {
                line.Append(COMMA);
            }

            line.Append((b) ? 1 : 0);
        }

        /// <summary>
        /// Add a column as a double.
        /// </summary>
        ///
        /// <param name="d">The double to add.</param>
        public void AddColumn(double d)
        {
            if (line.Length > 0)
            {
                line.Append(COMMA);
            }

            line.Append(CSVFormat.ENGLISH.Format(d, EncogFramework.DEFAULT_PRECISION));
        }

        /// <summary>
        /// Add a column as an integer.
        /// </summary>
        ///
        /// <param name="i">The integer to add.</param>
        public void AddColumn(int i)
        {
            if (line.Length > 0)
            {
                line.Append(COMMA);
            }

            line.Append(i);
        }

        /// <summary>
        /// Add a column as a string.
        /// </summary>
        ///
        /// <param name="str">The string to add.</param>
        public void AddColumn(String str)
        {
            if (line.Length > 0)
            {
                line.Append(COMMA);
            }

            line.Append(QUOTE);
            line.Append(str);
            line.Append(QUOTE);
        }

        /// <summary>
        /// Add a list of string columns.
        /// </summary>
        ///
        /// <param name="cols">The columns to add.</param>
        public void AddColumns(IList<String> cols)
        {
            foreach (String str  in  cols)
            {
                AddColumn(str);
            }
        }

        /// <summary>
        /// Add a line.
        /// </summary>
        ///
        /// <param name="l">The line to add.</param>
        public void AddLine(String l)
        {
            if (line.Length > 0)
            {
                WriteLine();
            }
            xout.WriteLine(l);
        }

        /// <summary>
        /// Add the specified properties.
        /// </summary>
        ///
        /// <param name="properties">The properties.</param>
        public void AddProperties(IDictionary<String, String> properties)
        {
            foreach (String key  in  properties.Keys)
            {
                String value_ren = properties[key];
                WriteProperty(key, value_ren);
            }
        }

        /// <summary>
        /// Add a new section.
        /// </summary>
        ///
        /// <param name="str">The section to add.</param>
        public void AddSection(String str)
        {
            currentSection = str;
            xout.WriteLine("[" + str + "]");
        }

        /// <summary>
        /// Add a new subsection.
        /// </summary>
        ///
        /// <param name="str">The subsection.</param>
        public void AddSubSection(String str)
        {
            xout.WriteLine("[" + currentSection + ":" + str + "]");
        }

        /// <summary>
        /// Flush the file.
        /// </summary>
        ///
        public void Flush()
        {
            xout.Flush();
        }


        /// <summary>
        /// Write the specified string.
        /// </summary>
        ///
        /// <param name="str">The string to write.</param>
        public void Write(String str)
        {
            xout.Write(str);
        }

        /// <summary>
        /// Write the line.
        /// </summary>
        ///
        public void WriteLine()
        {
            xout.WriteLine(line.ToString());
            line.Length = 0;
        }

        /// <summary>
        /// Write a property as an activation function.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="act">The activation function.</param>
        public void WriteProperty(String name,
                                  IActivationFunction act)
        {
            var result = new StringBuilder();
            result.Append(act.GetType().Name);

            for (int i = 0; i < act.Params.Length; i++)
            {
                result.Append('|');
                result.Append(CSVFormat.EG_FORMAT.Format(act.Params[i],
                                                         EncogFramework.DEFAULT_PRECISION));
            }
            WriteProperty(name, result.ToString());
        }

        /// <summary>
        /// Write the property as a boolean.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The boolean value.</param>
        public void WriteProperty(String name, bool value_ren)
        {
            xout.WriteLine(name + "=" + ((value_ren) ? 't' : 'f'));
        }

        /// <summary>
        /// Write a property as a CSV format.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="csvFormat">The format.</param>
        public void WriteProperty(String name, CSVFormat csvFormat)
        {
            String fmt;
            if ((csvFormat == CSVFormat.ENGLISH)
                || (csvFormat == CSVFormat.ENGLISH)
                || (csvFormat == CSVFormat.DECIMAL_POINT))
            {
                fmt = "decpnt";
            }
            else if (csvFormat == CSVFormat.DECIMAL_COMMA)
            {
                fmt = "deccomma";
            }
            else
            {
                fmt = "decpnt";
            }
            xout.WriteLine(name + "=" + fmt);
        }

        /// <summary>
        /// Write the property as a double.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value.</param>
        public void WriteProperty(String name, double value_ren)
        {
            xout.WriteLine(name + "="
                           + CSVFormat.EG_FORMAT.Format(value_ren, EncogFramework.DEFAULT_PRECISION));
        }

        /// <summary>
        /// Write the property as a double array.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="d">The double value.</param>
        public void WriteProperty(String name, double[] d)
        {
            var result = new StringBuilder();
            NumberList.ToList(CSVFormat.EG_FORMAT, result, d);
            WriteProperty(name, result.ToString());
        }

        /// <summary>
        /// Write a property as an int value.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The int value.</param>
        public void WriteProperty(String name, int value_ren)
        {
            xout.WriteLine(name + "=" + value_ren);
        }

        /// <summary>
        /// Write a property as an int array.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="array">The array.</param>
        public void WriteProperty(String name, int[] array)
        {
            var result = new StringBuilder();
            NumberList.ToListInt(CSVFormat.EG_FORMAT, result, array);
            WriteProperty(name, result.ToString());
        }

        /// <summary>
        /// Write a matrix as a property.
        /// </summary>
        ///
        /// <param name="name">The property name.</param>
        /// <param name="matrix">The matrix.</param>
        public void WriteProperty(String name, Matrix matrix)
        {
            var result = new StringBuilder();
            result.Append(matrix.Rows);
            result.Append(',');
            result.Append(matrix.Cols);

            for (int row = 0; row < matrix.Rows; row++)
            {
                for (int col = 0; col < matrix.Cols; col++)
                {
                    result.Append(',');
                    result.Append(CSVFormat.EG_FORMAT.Format(matrix[row, col],
                                                             EncogFramework.DEFAULT_PRECISION));
                }
            }

            WriteProperty(name, result.ToString());
        }

        /// <summary>
        /// Write the property a s string.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value.</param>
        public void WriteProperty(String name, String value_ren)
        {
            xout.WriteLine(name + "=" + value_ren);
        }
    }
}