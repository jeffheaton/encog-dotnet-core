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
using System.IO;
using Encog.App.Analyst.Script;
using Encog.App.Analyst.Script.Normalize;
using Encog.App.Analyst.Script.Prop;
using Encog.Util;
using Encog.Util.CSV;
using Encog.Util.File;

namespace Encog.App.Analyst.Report
{
    /// <summary>
    ///     Produce a simple report on the makeup of the script and data to be analyued.
    /// </summary>
    public class AnalystReport
    {
        /// <summary>
        ///     Used as a col-span.
        /// </summary>
        public const int FiveSpan = 5;

        /// <summary>
        ///     Used as a col-span.
        /// </summary>
        public const int EightSpan = 5;

        /// <summary>
        ///     The analyst to use.
        /// </summary>
        private readonly EncogAnalyst _analyst;

        /// <summary>
        ///     The missing count.
        /// </summary>
        private int _missingCount;

        /// <summary>
        ///     The row count.
        /// </summary>
        private int _rowCount;

        /// <summary>
        ///     Construct the report.
        /// </summary>
        /// <param name="theAnalyst">The analyst to use.</param>
        public AnalystReport(EncogAnalyst theAnalyst)
        {
            _analyst = theAnalyst;
        }

        /// <summary>
        ///     Analyze the file.
        /// </summary>
        private void AnalyzeFile()
        {
            ScriptProperties prop = _analyst.Script.Properties;

            // get filenames, headers & format
            String sourceID = prop.GetPropertyString(
                ScriptProperties.HeaderDatasourceRawFile);

            FileInfo sourceFile = _analyst.Script.ResolveFilename(sourceID);
            CSVFormat format = _analyst.Script.DetermineFormat();
            bool headers = _analyst.Script.ExpectInputHeaders(sourceID);

            // read the file
            _rowCount = 0;
            _missingCount = 0;

            var csv = new ReadCSV(sourceFile.ToString(), headers, format);
            while (csv.Next())
            {
                _rowCount++;
                if (csv.HasMissing())
                {
                    _missingCount++;
                }
            }
            csv.Close();
        }


        /// <summary>
        ///     Produce the report.
        /// </summary>
        /// <returns>The report.</returns>
        public String ProduceReport()
        {
            var report = new HTMLReport();

            AnalyzeFile();
            report.BeginHTML();
            report.Title("Encog Analyst Report");
            report.BeginBody();

            report.H1("General Statistics");
            report.BeginTable();
            report.TablePair("Total row count", Format.FormatInteger(_rowCount));
            report.TablePair("Missing row count", Format.FormatInteger(_missingCount));
            report.EndTable();

            report.H1("Field Ranges");
            report.BeginTable();
            report.BeginRow();
            report.Header("Name");
            report.Header("Class?");
            report.Header("Complete?");
            report.Header("Int?");
            report.Header("Real?");
            report.Header("Max");
            report.Header("Min");
            report.Header("Mean");
            report.Header("Standard Deviation");
            report.EndRow();


            foreach (DataField df  in  _analyst.Script.Fields)
            {
                report.BeginRow();
                report.Cell(df.Name);
                report.Cell(Format.FormatYesNo(df.Class));
                report.Cell(Format.FormatYesNo(df.Complete));
                report.Cell(Format.FormatYesNo(df.Integer));
                report.Cell(Format.FormatYesNo(df.Real));
                report.Cell(Format.FormatDouble(df.Max, FiveSpan));
                report.Cell(Format.FormatDouble(df.Min, FiveSpan));
                report.Cell(Format.FormatDouble(df.Mean, FiveSpan));
                report.Cell(Format.FormatDouble(df.StandardDeviation,
                                                FiveSpan));
                report.EndRow();

                if (df.ClassMembers.Count > 0)
                {
                    report.BeginRow();
                    report.Cell("&nbsp;");
                    report.BeginTableInCell(EightSpan);
                    report.BeginRow();
                    report.Header("Code");
                    report.Header("Name");
                    report.Header("Count");
                    report.EndRow();

                    foreach (AnalystClassItem item  in  df.ClassMembers)
                    {
                        report.BeginRow();
                        report.Cell(item.Code);
                        report.Cell(item.Name);
                        report.Cell(Format.FormatInteger(item.Count));
                        report.EndRow();
                    }
                    report.EndTableInCell();
                    report.EndRow();
                }
            }

            report.EndTable();

            report.H1("Normalization");
            report.BeginTable();
            report.BeginRow();
            report.Header("Name");
            report.Header("Action");
            report.Header("High");
            report.Header("Low");
            report.EndRow();


            foreach (AnalystField item  in  _analyst.Script.Normalize.NormalizedFields)
            {
                report.BeginRow();
                report.Cell(item.Name);
                report.Cell(item.Action.ToString());
                report.Cell(Format.FormatDouble(item.NormalizedHigh, FiveSpan));
                report.Cell(Format.FormatDouble(item.NormalizedLow, FiveSpan));
                report.EndRow();
            }

            report.EndTable();

            report.H1("Machine Learning");
            report.BeginTable();
            report.BeginRow();
            report.Header("Name");
            report.Header("Value");
            report.EndRow();

            String t = _analyst.Script.Properties
                               .GetPropertyString(ScriptProperties.MlConfigType);
            String a = _analyst.Script.Properties
                               .GetPropertyString(ScriptProperties.MlConfigArchitecture);
            String rf = _analyst.Script.Properties
                                .GetPropertyString(
                                    ScriptProperties.MlConfigMachineLearningFile);

            report.TablePair("Type", t);
            report.TablePair("Architecture", a);
            report.TablePair("Machine Learning File", rf);
            report.EndTable();

            report.H1("Files");
            report.BeginTable();
            report.BeginRow();
            report.Header("Name");
            report.Header("Filename");
            report.EndRow();

            foreach (String key  in  _analyst.Script.Properties.Filenames)
            {
                String v = _analyst.Script.Properties
                                   .GetFilename(key);
                report.BeginRow();
                report.Cell(key);
                report.Cell(v);
                report.EndRow();
            }
            report.EndTable();

            report.EndBody();
            report.EndHTML();

            return (report.ToString());
        }

        /// <summary>
        ///     Produce a report for a filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void ProduceReport(FileInfo filename)
        {
            try
            {
                String str = ProduceReport();
                FileUtil.WriteFileAsString(filename, str);
            }
            catch (IOException ex)
            {
                throw new AnalystError(ex);
            }
        }
    }
}
