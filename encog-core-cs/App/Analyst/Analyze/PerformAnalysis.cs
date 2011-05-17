using System;
using System.Collections.Generic;
using System.Text;
using Encog.App.Analyst.Script;
using Encog.App.Analyst.Script.Prop;
using Encog.App.Analyst.Util;
using Encog.Util.CSV;

namespace Encog.App.Analyst.Analyze
{
    /// <summary>
    /// This class is used to perform an analysis of a CSV file. This will help Encog
    /// to determine how the fields should be normalized.
    /// </summary>
    ///
    public class PerformAnalysis
    {
        /// <summary>
        /// The file name to analyze.
        /// </summary>
        ///
        private readonly String filename;

        /// <summary>
        /// The format of this file.
        /// </summary>
        ///
        private readonly AnalystFileFormat format;

        /// <summary>
        /// True, if headers are present.
        /// </summary>
        ///
        private readonly bool headers;

        /// <summary>
        /// The script to use.
        /// </summary>
        ///
        private readonly AnalystScript script;

        /// <summary>
        /// The fields to analyze.
        /// </summary>
        ///
        private AnalyzedField[] fields;

        /// <summary>
        /// Construct the analysis object.
        /// </summary>
        ///
        /// <param name="theScript">The script to use.</param>
        /// <param name="theFilename">The name of the file to analyze.</param>
        /// <param name="theHeaders">True if headers are present.</param>
        /// <param name="theFormat">The format of the file being analyzed.</param>
        public PerformAnalysis(AnalystScript theScript,
                               String theFilename, bool theHeaders,
                               AnalystFileFormat theFormat)
        {
            filename = theFilename;
            headers = theHeaders;
            format = theFormat;
            script = theScript;
        }

        /// <summary>
        /// Generate the header fields.
        /// </summary>
        ///
        /// <param name="csv">The CSV file to use.</param>
        private void GenerateFields(ReadCSV csv)
        {
            if (headers)
            {
                GenerateFieldsFromHeaders(csv);
            }
            else
            {
                GenerateFieldsFromCount(csv);
            }
        }

        /// <summary>
        /// Generate the fields using counts, no headers provided.
        /// </summary>
        ///
        /// <param name="csv">The CSV file to use.</param>
        private void GenerateFieldsFromCount(ReadCSV csv)
        {
            fields = new AnalyzedField[csv.ColumnCount];
            for (int i = 0; i < fields.Length; i++)
            {
                fields[i] = new AnalyzedField(script, "field:" + (i + 1));
            }
        }

        /// <summary>
        /// Generate the fields using header values.
        /// </summary>
        ///
        /// <param name="csv">The CSV file to use.</param>
        private void GenerateFieldsFromHeaders(ReadCSV csv)
        {
            var h = new CSVHeaders(csv.ColumnNames);
            fields = new AnalyzedField[csv.ColumnCount];
            for (int i = 0; i < fields.Length; i++)
            {
                if (i >= csv.ColumnCount)
                {
                    throw new AnalystError(
                        "CSV header count does not match column count");
                }
                fields[i] = new AnalyzedField(script, h.GetHeader(i));
            }
        }

        /// <summary>
        /// Perform the analysis.
        /// </summary>
        ///
        /// <param name="target">The Encog analyst object to analyze.</param>
        public void Process(EncogAnalyst target)
        {
            CSVFormat csvFormat = ConvertStringConst
                .ConvertToCSVFormat(format);
            var csv = new ReadCSV(filename, headers, csvFormat);

            // pass one, calculate the min/max
            while (csv.Next())
            {
                if (fields == null)
                {
                    GenerateFields(csv);
                }

                for (int i = 0; i < csv.ColumnCount; i++)
                {
                    fields[i].Analyze1(csv.Get(i));
                }
            }


            foreach (AnalyzedField field  in  fields)
            {
                field.CompletePass1();
            }

            csv.Close();

            // pass two, standard deviation
            csv = new ReadCSV(filename, headers, csvFormat);
            while (csv.Next())
            {
                for (int i_0 = 0; i_0 < csv.ColumnCount; i_0++)
                {
                    fields[i_0].Analyze2(csv.Get(i_0));
                }
            }


            foreach (AnalyzedField field_1  in  fields)
            {
                field_1.CompletePass2();
            }

            csv.Close();

            String str = script.Properties.GetPropertyString(
                ScriptProperties.SETUP_CONFIG_ALLOWED_CLASSES);
            if (str == null)
            {
                str = "";
            }

            bool allowInt = str.Contains("int");
            bool allowReal = str.Contains("real")
                             || str.Contains("double");
            bool allowString = str.Contains("string");


            // remove any classes that did not qualify
            foreach (AnalyzedField field_2  in  fields)
            {
                if (field_2.Class)
                {
                    if (!allowInt && field_2.Integer)
                    {
                        field_2.Class = false;
                    }

                    if (!allowString && (!field_2.Integer && !field_2.Real))
                    {
                        field_2.Class = false;
                    }

                    if (!allowReal && field_2.Real && !field_2.Integer)
                    {
                        field_2.Class = false;
                    }

                    if (field_2.Integer
                        && (field_2.AnalyzedClassMembers.Count <= 2))
                    {
                        field_2.Class = false;
                    }
                }
            }

            // merge with existing
            if ((target.Script.Fields != null)
                && (fields.Length == target.Script.Fields.Length))
            {
                for (int i_3 = 0; i_3 < fields.Length; i_3++)
                {
                    // copy the old field name
                    fields[i_3].Name = target.Script.Fields[i_3].Name;

                    if (fields[i_3].Class)
                    {
                        IList<AnalystClassItem> t = fields[i_3].AnalyzedClassMembers;
                        IList<AnalystClassItem> s = target.Script.Fields[i_3].ClassMembers;

                        if (s.Count == t.Count)
                        {
                            for (int j = 0; j < s.Count; j++)
                            {
                                if (t[j].Code.Equals(s[j].Code))
                                {
                                    t[j].Name = s[j].Name;
                                }
                            }
                        }
                    }
                }
            }

            // now copy the fields
            var df = new DataField[fields.Length];

            for (int i_4 = 0; i_4 < df.Length; i_4++)
            {
                df[i_4] = fields[i_4].FinalizeField();
            }

            target.Script.Fields = df;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" filename=");
            result.Append(filename);
            result.Append(", headers=");
            result.Append(headers);
            result.Append("]");
            return result.ToString();
        }
    }
}