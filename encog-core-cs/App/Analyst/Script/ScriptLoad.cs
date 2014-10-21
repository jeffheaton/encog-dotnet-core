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
using Encog.App.Analyst.Script.ML;
using Encog.App.Analyst.Script.Normalize;
using Encog.App.Analyst.Script.Process;
using Encog.App.Analyst.Script.Prop;
using Encog.App.Analyst.Script.Segregate;
using Encog.App.Analyst.Script.Task;
using Encog.Persist;
using Encog.Util.Arrayutil;
using Encog.Util.CSV;

namespace Encog.App.Analyst.Script
{
    /// <summary>
    ///     Used to load an Encog Analyst script.
    /// </summary>
    public class ScriptLoad
    {
        /// <summary>
        ///     Column 1.
        /// </summary>
        public const int ColumnOne = 1;

        /// <summary>
        ///     Column 2.
        /// </summary>
        public const int ColumnTwo = 2;

        /// <summary>
        ///     Column 3.
        /// </summary>
        public const int ColumnThree = 3;

        /// <summary>
        ///     Column 4.
        /// </summary>
        public const int ColumnFour = 4;

        /// <summary>
        ///     Column 5.
        /// </summary>
        public const int ColumnFive = 5;

        /// <summary>
        ///     The script being loaded.
        /// </summary>
        private readonly AnalystScript _script;

        /// <summary>
        ///     Construct a script loader.
        /// </summary>
        /// <param name="theScript">The script to load into.</param>
        public ScriptLoad(AnalystScript theScript)
        {
            _script = theScript;
        }

        /// <summary>
        ///     Handle loading the data classes.
        /// </summary>
        /// <param name="section">The section being loaded.</param>
        private void HandleDataClasses(EncogFileSection section)
        {
            var map = new Dictionary<String, List<AnalystClassItem>>();

            bool first = true;

            foreach (String line in section.Lines)
            {
                if (!first)
                {
                    IList<String> cols = EncogFileSection.SplitColumns(line);

                    if (cols.Count < ColumnFour)
                    {
                        throw new AnalystError("Invalid data class: " + line);
                    }

                    String field = cols[0];
                    String code = cols[1];
                    String name = cols[2];
                    int count = Int32.Parse(cols[3]);

                    DataField df = _script.FindDataField(field);

                    if (df == null)
                    {
                        throw new AnalystError(
                            "Attempting to add class to unknown field: " + name);
                    }

                    List<AnalystClassItem> classItems;

                    if (!map.ContainsKey(field))
                    {
                        classItems = new List<AnalystClassItem>();
                        map[field] = classItems;
                    }
                    else
                    {
                        classItems = map[field];
                    }

                    classItems.Add(new AnalystClassItem(code, name, count));
                }
                else
                {
                    first = false;
                }
            }


            foreach (DataField field in _script.Fields)
            {
                if (field.Class)
                {
                    List<AnalystClassItem> classList = map[field.Name];
                    if (classList != null)
                    {
                        classList.Sort();
                        field.ClassMembers.Clear();
                        foreach (AnalystClassItem item in classList)
                        {
                            field.ClassMembers.Add(item);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Handle loading data stats.
        /// </summary>
        /// <param name="section">The section being loaded.</param>
        private void HandleDataStats(EncogFileSection section)
        {
            IList<DataField> dfs = new List<DataField>();
            bool first = true;

            foreach (String line in section.Lines)
            {
                if (!first)
                {
                    IList<String> cols = EncogFileSection.SplitColumns(line);
                    String name = cols[0];
                    bool isclass = Int32.Parse(cols[1]) > 0;
                    bool iscomplete = Int32.Parse(cols[2]) > 0;
                    bool isint = Int32.Parse(cols[ColumnThree]) > 0;
                    bool isreal = Int32.Parse(cols[ColumnFour]) > 0;
                    double amax = CSVFormat.EgFormat.Parse(cols[5]);
                    double amin = CSVFormat.EgFormat.Parse(cols[6]);
                    double mean = CSVFormat.EgFormat.Parse(cols[7]);
                    double sdev = CSVFormat.EgFormat.Parse(cols[8]);
                    String source = "";

                    // source was added in Encog 3.2, so it might not be there
                    if (cols.Count > 9)
                    {
                        source = cols[9];
                    }

                    var df = new DataField(name)
                        {
                            Class = isclass,
                            Complete = iscomplete,
                            Integer = isint,
                            Real = isreal,
                            Max = amax,
                            Min = amin,
                            Mean = mean,
                            StandardDeviation = sdev,
                            Source = source
                        };
                    dfs.Add(df);
                }
                else
                {
                    first = false;
                }
            }

            var array = new DataField[dfs.Count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = dfs[i];
            }

            _script.Fields = array;
        }

        /// <summary>
        ///     Handle loading the filenames.
        /// </summary>
        /// <param name="section">The section being loaded.</param>
        private void HandleFilenames(EncogFileSection section)
        {
            IDictionary<String, String> prop = section.ParseParams();
            _script.Properties.ClearFilenames();


            foreach (var e in prop)
            {
                _script.Properties.SetFilename(e.Key, e.Value);
            }
        }

        /// <summary>
        ///     Handle normalization ranges.
        /// </summary>
        /// <param name="section">The section being loaded.</param>
        private void HandleNormalizeRange(EncogFileSection section)
        {
            _script.Normalize.NormalizedFields.Clear();
            bool first = true;

            foreach (String line in section.Lines)
            {
                if (!first)
                {
                    IList<String> cols = EncogFileSection.SplitColumns(line);
                    String name = cols[0];
                    bool isOutput = cols[1].ToLower()
                                           .Equals("output");
                    int timeSlice = Int32.Parse(cols[2]);
                    String action = cols[3];
                    double high = CSVFormat.EgFormat.Parse(cols[4]);
                    double low = CSVFormat.EgFormat.Parse(cols[5]);

                    NormalizationAction des;
                    if (action.Equals("range"))
                    {
                        des = NormalizationAction.Normalize;
                    }
                    else if (action.Equals("ignore"))
                    {
                        des = NormalizationAction.Ignore;
                    }
                    else if (action.Equals("pass"))
                    {
                        des = NormalizationAction.PassThrough;
                    }
                    else if (action.Equals("equilateral"))
                    {
                        des = NormalizationAction.Equilateral;
                    }
                    else if (action.Equals("single"))
                    {
                        des = NormalizationAction.SingleField;
                    }
                    else if (action.Equals("oneof"))
                    {
                        des = NormalizationAction.OneOf;
                    }
                    else
                    {
                        throw new AnalystError("Unknown field type:" + action);
                    }

                    var nf = new AnalystField(name, des, high, low) {TimeSlice = timeSlice, Output = isOutput};
                    _script.Normalize.NormalizedFields.Add(nf);
                }
                else
                {
                    first = false;
                }
            }
        }

        /// <summary>
        ///     Handle loading segregation info.
        /// </summary>
        /// <param name="section">The section being loaded.</param>
        private void HandleSegregateFiles(EncogFileSection section)
        {
            IList<AnalystSegregateTarget> nfs = new List<AnalystSegregateTarget>();
            bool first = true;

            foreach (String line in section.Lines)
            {
                if (!first)
                {
                    IList<String> cols = EncogFileSection.SplitColumns(line);
                    String filename = cols[0];
                    int percent = Int32.Parse(cols[1]);

                    var nf = new AnalystSegregateTarget(
                        filename, percent);
                    nfs.Add(nf);
                }
                else
                {
                    first = false;
                }
            }

            var array = new AnalystSegregateTarget[nfs.Count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = nfs[i];
            }

            _script.Segregate.SegregateTargets = array;
        }

        /// <summary>
        ///     Handle loading a task.
        /// </summary>
        /// <param name="section">The section.</param>
        private void HandleTask(EncogFileSection section)
        {
            var task = new AnalystTask(section.SubSectionName);

            foreach (String line in section.Lines)
            {
                task.Lines.Add(line);
            }
            _script.AddTask(task);
        }

        /// <summary>
        ///     Load an Encog script.
        /// </summary>
        /// <param name="stream">The stream to load from.</param>
        public void Load(Stream stream)
        {
            EncogReadHelper reader = null;

            try
            {
                EncogFileSection section;
                reader = new EncogReadHelper(stream);

                while ((section = reader.ReadNextSection()) != null)
                {
                    ProcessSubSection(section);
                }

                // init the script
                _script.Init();
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        ///     Load a generic subsection.
        /// </summary>
        /// <param name="section">The section to load from.</param>
        private void LoadSubSection(EncogFileSection section)
        {
            IDictionary<String, String> prop = section.ParseParams();


            foreach (String name in prop.Keys)
            {
                String key = section.SectionName.ToUpper() + ":"
                             + section.SubSectionName.ToUpper() + "_" + name;
                String v = prop[name] ?? "";
                ValidateProperty(section.SectionName,
                                 section.SubSectionName, name, v);
                _script.Properties.SetProperty(key, v);
            }
        }

        /// <summary>
        ///     Process one of the subsections.
        /// </summary>
        /// <param name="section">The section.</param>
        private void ProcessSubSection(EncogFileSection section)
        {
            String currentSection = section.SectionName;
            String currentSubsection = section.SubSectionName;

            if (currentSection.Equals("SETUP")
                && currentSubsection.Equals("CONFIG", StringComparison.InvariantCultureIgnoreCase))
            {
                LoadSubSection(section);
            }
            else if (currentSection.Equals("SETUP")
                     && currentSubsection.Equals("FILENAMES", StringComparison.InvariantCultureIgnoreCase))
            {
                HandleFilenames(section);
            }
            else if (currentSection.Equals("DATA")
                     && currentSubsection.Equals("CONFIG", StringComparison.InvariantCultureIgnoreCase))
            {
                LoadSubSection(section);
            }
            else if (currentSection.Equals("DATA")
                     && currentSubsection.Equals("STATS", StringComparison.InvariantCultureIgnoreCase))
            {
                HandleDataStats(section);
            }
            else if (currentSection.Equals("DATA")
                     && currentSubsection.Equals("CLASSES", StringComparison.InvariantCultureIgnoreCase))
            {
                HandleDataClasses(section);
            }
            else if (currentSection.Equals("NORMALIZE")
                     && currentSubsection.Equals("RANGE", StringComparison.InvariantCultureIgnoreCase))
            {
                HandleNormalizeRange(section);
            }
            else if (currentSection.Equals("NORMALIZE")
                     && currentSubsection.Equals("CONFIG", StringComparison.InvariantCultureIgnoreCase))
            {
                LoadSubSection(section);
            }
            else if (currentSection.Equals("NORMALIZE")
                     && currentSubsection.Equals("CONFIG", StringComparison.InvariantCultureIgnoreCase))
            {
                LoadSubSection(section);
            }
            else if (currentSection.Equals("CLUSTER")
                     && currentSubsection.Equals("CONFIG", StringComparison.InvariantCultureIgnoreCase))
            {
                LoadSubSection(section);
            }
            else if (currentSection.Equals("SERIES")
                     && currentSubsection.Equals("CONFIG", StringComparison.InvariantCultureIgnoreCase))
            {
                LoadSubSection(section);
            }
            else if (currentSection.Equals("RANDOMIZE")
                     && currentSubsection.Equals("CONFIG", StringComparison.InvariantCultureIgnoreCase))
            {
                LoadSubSection(section);
            }
            else if (currentSection.Equals("SEGREGATE")
                     && currentSubsection.Equals("CONFIG", StringComparison.InvariantCultureIgnoreCase))
            {
                LoadSubSection(section);
            }
            else if (currentSection.Equals("SEGREGATE")
                     && currentSubsection.Equals("FILES", StringComparison.InvariantCultureIgnoreCase))
            {
                HandleSegregateFiles(section);
            }
            else if (currentSection.Equals("GENERATE")
                     && currentSubsection.Equals("CONFIG", StringComparison.InvariantCultureIgnoreCase))
            {
                LoadSubSection(section);
            }
            else if (currentSection.Equals("HEADER")
                     && currentSubsection.Equals("DATASOURCE", StringComparison.InvariantCultureIgnoreCase))
            {
                LoadSubSection(section);
            }
            else if (currentSection.Equals("ML")
                     && currentSubsection.Equals("CONFIG", StringComparison.InvariantCultureIgnoreCase))
            {
                LoadSubSection(section);
            }
            else if (currentSection.Equals("ML")
                     && currentSubsection.Equals("TRAIN", StringComparison.InvariantCultureIgnoreCase))
            {
                LoadSubSection(section);
            }
            else if (currentSection.Equals("TASKS")
                     && (currentSubsection.Length > 0))
            {
                HandleTask(section);
            }
            else if (currentSection.Equals("BALANCE")
                     && currentSubsection.Equals("CONFIG", StringComparison.InvariantCultureIgnoreCase))
            {
                LoadSubSection(section);
            }
            else if (currentSection.Equals("CODE")
                     && currentSubsection.Equals("CONFIG"))
            {
                LoadSubSection(section);
            }
            else if (currentSection.Equals("PROCESS")
                     && currentSubsection.Equals("CONFIG"))
            {
                LoadSubSection(section);
            }
            else if (currentSection.Equals("PROCESS")
                     && currentSubsection.Equals("FIELDS"))
            {
                HandleProcessFields(section);
            }
        }

        /// <summary>
        ///     Validate a property.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="subSection">The sub section.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value_ren">The new value for the property.</param>
        private void ValidateProperty(String section,
                                      String subSection, String name, String value_ren)
        {
            PropertyEntry entry = PropertyConstraints.Instance.GetEntry(
                section, subSection, name);
            if (entry == null)
            {
                throw new AnalystError("Unknown property: "
                                       + PropertyEntry.DotForm(section, subSection, name));
            }
            entry.Validate(section, subSection, name, value_ren);
        }

        private void HandleProcessFields(EncogFileSection section)
        {
            IList<ProcessField> fields = _script.Process.Fields;
            bool first = true;

            fields.Clear();

            foreach (string line in section.Lines)
            {
                if (!first)
                {
                    IList<string> cols = EncogFileSection.SplitColumns(line);
                    String name = cols[0];
                    String command = cols[1];
                    var pf = new ProcessField(name, command);
                    fields.Add(pf);
                }
                else
                {
                    first = false;
                }
            }
        }

        private void LoadOpcodes(EncogFileSection section)
        {
            bool first = true;
            foreach (string line in section.Lines)
            {
                if (!first)
                {
                    IList<string> cols = EncogFileSection.SplitColumns(line);
                    string name = cols[0];
                    int childCount = int.Parse(cols[1]);
                    var opcode = new ScriptOpcode(name, childCount);
                    _script.Opcodes.Add(opcode);
                }
                else
                {
                    first = false;
                }
            }
        }
    }
}
