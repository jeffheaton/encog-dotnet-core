// Encog(tm) Artificial Intelligence Framework v2.3
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Parse.Tags.Write;
using Encog.Parse.Tags.Read;
using Encog.MathUtil;
using System.Reflection;
using Encog.MathUtil.CSV;
#if logging
using log4net;
#endif

namespace Encog.Persist.Persistors
{
    /// <summary>
    /// This class contains some utilities for persisting objects.
    /// </summary>
    public class PersistorUtil
    {

        /// <summary>
        /// The rows in the matrix.
        /// </summary>
        public const String ATTRIBUTE_MATRIX_ROWS = "rows";

        /// <summary>
        /// The columns in the matrix.
        /// </summary>
        public const String ATTRIBUTE_MATRIX_COLS = "cols";

        /// <summary>
        /// A matrix row.
        /// </summary>
        public const String ROW = "row";

        /// <summary>
        /// Private constructor.
        /// </summary>
        private PersistorUtil()
        {
        }
        
        /// <summary>
        /// Write the beginning XML for an Encog object.
        /// </summary>
        /// <param name="objectType">The object type to persist.</param>
        /// <param name="xmlOut">The object that is being persisted.</param>
        /// <param name="obj">The XML writer.</param>
        /// <param name="top">Is this a top-level object, that needs a name
        /// and description?</param>
        public static void BeginEncogObject(String objectType,
                 WriteXML xmlOut, IEncogPersistedObject obj,
                 bool top)
        {
            if (top)
            {
                if (obj.Name == null)
                {
                    throw new PersistError(
                            "Encog object must have a name to be saved.");
                }
                xmlOut.AddAttribute("name", obj.Name);
                if (obj.Description != null)
                {
                    xmlOut.AddAttribute("description", obj.Description);
                }
                else
                {
                    xmlOut.AddAttribute("description", "");
                }

            }
            xmlOut.AddAttribute("native", obj.GetType().Name);
            xmlOut.AddAttribute("id", "1");
            xmlOut.BeginTag(objectType);
        }

        /// <summary>
        /// Create a persistor object.  These objects know how to persist
        /// certain types of classes.
        /// </summary>
        /// <param name="className">The name of the class to create a persistor for.</param>
        /// <returns>The persistor for the specified class.</returns>
        public static IPersistor CreatePersistor(String className)
        {
            // handle any hard coded ones
            if (className.Equals("TrainingData"))
            {
                return new BasicNeuralDataSetPersistor();
            }

            String name = "Encog.Persist.Persistors." + className + "Persistor";
            IPersistor persistor = (IPersistor)Assembly.GetExecutingAssembly().CreateInstance(name);
            return persistor;
        }

        /// <summary>
        /// Load a matrix from the reader.
        /// </summary>
        /// <param name="xmlIn">The XML reader.</param>
        /// <returns>The loaded matrix.</returns>
        public static Matrix.Matrix LoadMatrix(ReadXML xmlIn)
        {
            int rows = xmlIn.LastTag.GetAttributeInt(
                   PersistorUtil.ATTRIBUTE_MATRIX_ROWS);
            int cols = xmlIn.LastTag.GetAttributeInt(
                   PersistorUtil.ATTRIBUTE_MATRIX_COLS);
            Matrix.Matrix matrix = new Matrix.Matrix(rows, cols);

            int row = 0;

            String end = xmlIn.LastTag.Name;
            while (xmlIn.ReadToTag())
            {
                if (xmlIn.IsIt(end, false))
                {
                    break;
                }
                if (xmlIn.IsIt(PersistorUtil.ROW, true))
                {
                    String str = xmlIn.ReadTextToTag();
                    double[] d = NumberList.FromList(CSVFormat.EG_FORMAT, str);
                    for (int col = 0; col < d.Length; col++)
                    {
                        matrix[row, col] = d[col];
                    }
                    row++;
                }
            }

            return matrix;
        }


        /// <summary>
        /// Save the specified matrix.
        /// </summary>
        /// <param name="matrix">The matrix to save.</param>
        /// <param name="xmlOut">The XML writer.</param>
        public static void SaveMatrix(Matrix.Matrix matrix, WriteXML xmlOut)
        {
            xmlOut.AddAttribute(PersistorUtil.ATTRIBUTE_MATRIX_ROWS, ""
                    + matrix.Rows);
            xmlOut.AddAttribute(PersistorUtil.ATTRIBUTE_MATRIX_COLS, ""
                    + matrix.Cols);
            xmlOut.BeginTag("Matrix");

            CSVFormat format = CSVFormat.EG_FORMAT;

            for (int row = 0; row < matrix.Rows; row++)
            {
                StringBuilder builder = new StringBuilder();

                for (int col = 0; col < matrix.Cols; col++)
                {
                    if (col > 0)
                    {
                        builder.Append(',');
                    }

                    double d = matrix[row, col];
                    builder.Append(format.Format(d, 20));
                }
                xmlOut.BeginTag(PersistorUtil.ROW);
                xmlOut.AddText(builder.ToString());
                xmlOut.EndTag();
            }

            xmlOut.EndTag();
        }
#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(PersistorUtil));
#endif
    }
}
