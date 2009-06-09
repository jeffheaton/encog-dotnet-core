using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Parse.Tags.Write;
using Encog.Parse.Tags.Read;
using log4net;
using Encog.Util;
using System.Reflection;

namespace Encog.Persist.Persistors
{
    /**
 * This class contains some utilities for persisting objects.
 * 
 * @author jheaton
 */
    public class PersistorUtil
    {

        /**
         * The rows in the matrix.
         */
        public const String ATTRIBUTE_MATRIX_ROWS = "rows";

        /**
         * The columns in the matrix.
         */
        public const String ATTRIBUTE_MATRIX_COLS = "cols";

        /**
         * A matrix row.
         */
        public const String ROW = "row";

        /**
         * Private constructor.
         */
        private PersistorUtil()
        {
        }

        /**
         * Write the beginning XML for an Encog object.
         * @param objectType The object type to persist.
         * @param out The object that is being persisted.
         * @param obj The XML writer.
         * @param top Is this a top-level object, that needs a name
         * and description?
         */
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

        /**
         * Load a matrix from the reader.
         * @param in The XML reader.
         * @return The loaded matrix.
         */
        public static Matrix.Matrix loadMatrix(ReadXML xmlIn)
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
                    double[] d = ReadCSV.FromCommas(str);
                    for (int col = 0; col < d.Length; col++)
                    {
                        matrix[row, col] = d[col];
                    }
                    row++;
                }
            }

            return matrix;
        }

        /**
         * Save the specified matrix.
         * @param matrix The matrix to save.
         * @param out The XML writer.
         */
        public static void saveMatrix(Matrix.Matrix matrix, WriteXML xmlOut)
        {
            xmlOut.AddAttribute(PersistorUtil.ATTRIBUTE_MATRIX_ROWS, ""
                    + matrix.Rows);
            xmlOut.AddAttribute(PersistorUtil.ATTRIBUTE_MATRIX_COLS, ""
                    + matrix.Cols);
            xmlOut.BeginTag("Matrix");

            for (int row = 0; row < matrix.Rows; row++)
            {
                StringBuilder builder = new StringBuilder();

                for (int col = 0; col < matrix.Cols; col++)
                {
                    if (col > 0)
                    {
                        builder.Append(',');
                    }
                    builder.Append(matrix[row, col]);
                }
                xmlOut.BeginTag(PersistorUtil.ROW);
                xmlOut.AddText(builder.ToString());
                xmlOut.EndTag();
            }

            xmlOut.EndTag();
        }

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(PersistorUtil));

    }
}
