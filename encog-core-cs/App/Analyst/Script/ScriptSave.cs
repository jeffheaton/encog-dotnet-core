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
using Encog.App.Analyst.Script.Normalize;
using Encog.App.Analyst.Script.Prop;
using Encog.App.Analyst.Script.Segregate;
using Encog.App.Analyst.Script.Task;
using Encog.Persist;
using Encog.Util.Arrayutil;

namespace Encog.App.Analyst.Script
{
    /// <summary>
    ///     Used to save an Encog Analyst script.
    /// </summary>
    public class ScriptSave
    {
        /// <summary>
        ///     The script to save.
        /// </summary>
        private readonly AnalystScript _script;

        /// <summary>
        ///     Construct the script.
        /// </summary>
        /// <param name="theScript">The script to save.</param>
        public ScriptSave(AnalystScript theScript)
        {
            _script = theScript;
        }

        /// <summary>
        ///     Save the script to a stream.
        /// </summary>
        /// <param name="stream">The output stream.</param>
        public void Save(Stream stream)
        {
            var xout = new EncogWriteHelper(stream);
            SaveSubSection(xout, "HEADER", "DATASOURCE");
            SaveConfig(xout);

            if (_script.Fields != null)
            {
                SaveData(xout);
                SaveNormalize(xout);
            }

            SaveSubSection(xout, "RANDOMIZE", "CONFIG");
            SaveSubSection(xout, "CLUSTER", "CONFIG");
            SaveSubSection(xout, "BALANCE", "CONFIG");
            SaveSubSection(xout, "CODE", "CONFIG");

            if (_script.Segregate.SegregateTargets != null)
            {
                SaveSegregate(xout);
            }
            SaveSubSection(xout, "GENERATE", "CONFIG");
            SaveMachineLearning(xout);
            SaveTasks(xout);
            xout.Flush();
        }

        /// <summary>
        ///     Save the config info.
        /// </summary>
        /// <param name="xout">THe output file.</param>
        private void SaveConfig(EncogWriteHelper xout)
        {
            SaveSubSection(xout, "SETUP", "CONFIG");
            xout.AddSubSection("FILENAMES");

            IList<String> list = _script.Properties.Filenames;


            foreach (string key  in  list)
            {
                String v = _script.Properties.GetFilename(key);
                var f = new FileInfo(v);
                xout.WriteProperty(key,
                                   f.DirectoryName.Equals(_script.BasePath, StringComparison.InvariantCultureIgnoreCase)
                                       ? f.Name
                                       : v);
            }
        }

        /// <summary>
        ///     Save the data fields.
        /// </summary>
        /// <param name="xout">The output file.</param>
        private void SaveData(EncogWriteHelper xout)
        {
            SaveSubSection(xout, "DATA", "CONFIG");
            xout.AddSubSection("STATS");
            xout.AddColumn("name");
            xout.AddColumn("isclass");
            xout.AddColumn("iscomplete");
            xout.AddColumn("isint");
            xout.AddColumn("isreal");
            xout.AddColumn("amax");
            xout.AddColumn("amin");
            xout.AddColumn("mean");
            xout.AddColumn("sdev");
            xout.AddColumn("source");
            xout.WriteLine();


            foreach (DataField field  in  _script.Fields)
            {
                xout.AddColumn(field.Name);
                xout.AddColumn(field.Class);
                xout.AddColumn(field.Complete);
                xout.AddColumn(field.Integer);
                xout.AddColumn(field.Real);
                xout.AddColumn(field.Max);
                xout.AddColumn(field.Min);
                xout.AddColumn(field.Mean);
                xout.AddColumn(field.StandardDeviation);
                xout.AddColumn(field.Source);
                xout.WriteLine();
            }
            xout.Flush();

            xout.AddSubSection("CLASSES");
            xout.AddColumn("field");
            xout.AddColumn("code");
            xout.AddColumn("name");
            xout.WriteLine();


            foreach (DataField field  in  _script.Fields)
            {
                if (field.Class)
                {
                    foreach (AnalystClassItem col  in  field.ClassMembers)
                    {
                        xout.AddColumn(field.Name);
                        xout.AddColumn(col.Code);
                        xout.AddColumn(col.Name);
                        xout.AddColumn(col.Count);
                        xout.WriteLine();
                    }
                }
            }
        }

        /// <summary>
        ///     Save the ML sections.
        /// </summary>
        /// <param name="xout">The output file.</param>
        private void SaveMachineLearning(EncogWriteHelper xout)
        {
            SaveSubSection(xout, "ML", "CONFIG");
            SaveSubSection(xout, "ML", "TRAIN");
        }

        /// <summary>
        ///     Save the normalization data.
        /// </summary>
        /// <param name="xout">The output file.</param>
        private void SaveNormalize(EncogWriteHelper xout)
        {
            SaveSubSection(xout, "NORMALIZE", "CONFIG");

            xout.AddSubSection("RANGE");
            xout.AddColumn("name");
            xout.AddColumn("io");
            xout.AddColumn("timeSlice");
            xout.AddColumn("action");
            xout.AddColumn("high");
            xout.AddColumn("low");
            xout.WriteLine();

            foreach (AnalystField field  in  _script.Normalize.NormalizedFields)
            {
                xout.AddColumn(field.Name);
                xout.AddColumn(field.Input ? "input" : "output");
                xout.AddColumn(field.TimeSlice);
                switch (field.Action)
                {
                    case NormalizationAction.Ignore:
                        xout.AddColumn("ignore");
                        break;
                    case NormalizationAction.Normalize:
                        xout.AddColumn("range");
                        break;
                    case NormalizationAction.PassThrough:
                        xout.AddColumn("pass");
                        break;
                    case NormalizationAction.OneOf:
                        xout.AddColumn("oneof");
                        break;
                    case NormalizationAction.Equilateral:
                        xout.AddColumn("equilateral");
                        break;
                    case NormalizationAction.SingleField:
                        xout.AddColumn("single");
                        break;
                    default:
                        throw new AnalystError("Unknown action: " + field.Action);
                }

                xout.AddColumn(field.NormalizedHigh);
                xout.AddColumn(field.NormalizedLow);
                xout.WriteLine();
            }
        }

        /// <summary>
        ///     Save segregate info.
        /// </summary>
        /// <param name="xout">The output file.</param>
        private void SaveSegregate(EncogWriteHelper xout)
        {
            SaveSubSection(xout, "SEGREGATE", "CONFIG");
            xout.AddSubSection("FILES");
            xout.AddColumn("file");
            xout.AddColumn("percent");
            xout.WriteLine();


            foreach (AnalystSegregateTarget target  in  _script.Segregate.SegregateTargets)
            {
                xout.AddColumn(target.File);
                xout.AddColumn(target.Percent);
                xout.WriteLine();
            }
        }

        /// <summary>
        ///     Save a subsection.
        /// </summary>
        /// <param name="xout">The output file.</param>
        /// <param name="section">The section.</param>
        /// <param name="subSection">The subsection.</param>
        private void SaveSubSection(EncogWriteHelper xout,
                                    String section, String subSection)
        {
            if (!section.Equals(xout.CurrentSection))
            {
                xout.AddSection(section);
            }
            xout.AddSubSection(subSection);
            List<PropertyEntry> list = PropertyConstraints.Instance
                                                          .GetEntries(section, subSection);
            list.Sort();

            foreach (PropertyEntry entry  in  list)
            {
                String key = section + ":" + subSection + "_"
                             + entry.Name;
                String v = _script.Properties.GetPropertyString(
                    key);
                xout.WriteProperty(entry.Name, v ?? "");
            }
        }

        /// <summary>
        ///     Save the tasks.
        /// </summary>
        /// <param name="xout">The output file.</param>
        private void SaveTasks(EncogWriteHelper xout)
        {
            xout.AddSection("TASKS");
            List<string> list = _script.Tasks.Keys.ToList();

            list.Sort();

            foreach (String key  in  list)
            {
                AnalystTask task = _script.GetTask(key);
                xout.AddSubSection(task.Name);

                foreach (String line  in  task.Lines)
                {
                    xout.AddLine(line);
                }
            }
        }
    }
}
