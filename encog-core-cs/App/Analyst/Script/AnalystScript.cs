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
using Encog.App.Analyst.Script.ML;
using Encog.App.Analyst.Script.Normalize;
using Encog.App.Analyst.Script.Process;
using Encog.App.Analyst.Script.Prop;
using Encog.App.Analyst.Script.Segregate;
using Encog.App.Analyst.Script.Task;
using Encog.Util.CSV;
using Encog.Util.File;

namespace Encog.App.Analyst.Script
{
    /// <summary>
    ///     Holds a script for the Encog Analyst.
    /// </summary>
    public class AnalystScript
    {
        /// <summary>
        ///     The default MAX size for a class.
        /// </summary>
        public const int DefaultMaxClass = 50;

        /// <summary>
        ///     Tracks which files were generated.
        /// </summary>
        private readonly IList<String> _generated;

        /// <summary>
        ///     Information about how to normalize.
        /// </summary>
        private readonly AnalystNormalize _normalize;

        /// <summary>
        ///     The opcodes.
        /// </summary>
        private readonly IList<ScriptOpcode> _opcodes = new List<ScriptOpcode>();

        /// <summary>
        ///     Information about the process command.
        /// </summary>
        private readonly AnalystProcess _process = new AnalystProcess();

        /// <summary>
        ///     The properties.
        /// </summary>
        private readonly ScriptProperties _properties;

        /// <summary>
        ///     Information about how to segregate.
        /// </summary>
        private readonly AnalystSegregate _segregate;

        /// <summary>
        ///     The tasks.
        /// </summary>
        private readonly IDictionary<String, AnalystTask> _tasks;

        /// <summary>
        ///     The base path.
        /// </summary>
        private String _basePath;

        /// <summary>
        ///     The data fields, these are the raw data from the CSV file.
        /// </summary>
        private DataField[] _fields;

        /// <summary>
        ///     Construct an analyst script.
        /// </summary>
        public AnalystScript()
        {
            _normalize = new AnalystNormalize(this);
            _segregate = new AnalystSegregate();
            _generated = new List<String>();
            _tasks = new Dictionary<String, AnalystTask>();
            _properties = new ScriptProperties();
            _properties.SetProperty(ScriptProperties.SetupConfigCSVFormat,
                                    AnalystFileFormat.DecpntComma);
            _properties.SetProperty(
                ScriptProperties.SetupConfigMaxClassCount,
                DefaultMaxClass);
            _properties
                .SetProperty(ScriptProperties.SetupConfigAllowedClasses,
                             "integer,string");
        }

        /// <summary>
        ///     Set the base path.
        /// </summary>
        public String BasePath
        {
            get { return _basePath; }
            set { _basePath = value; }
        }


        /// <value>the fields to set</value>
        public DataField[] Fields
        {
            get { return _fields; }
            set { _fields = value; }
        }


        /// <value>the normalize</value>
        public AnalystNormalize Normalize
        {
            get { return _normalize; }
        }


        /// <value>The precision.</value>
        public int Precision
        {
            get { return EncogFramework.DefaultPrecision; }
        }


        /// <value>the properties</value>
        public ScriptProperties Properties
        {
            get { return _properties; }
        }


        /// <value>the segregate</value>
        public AnalystSegregate Segregate
        {
            get { return _segregate; }
        }

        /// <value>The tasks.</value>
        public IDictionary<String, AnalystTask> Tasks
        {
            get { return _tasks; }
        }

        /// <summary>
        ///     Preprocess information.
        /// </summary>
        public AnalystProcess Process
        {
            get { return _process; }
        }

        /// <summary>
        ///     Opcode information.
        /// </summary>
        public IList<ScriptOpcode> Opcodes
        {
            get { return _opcodes; }
        }

        /// <summary>
        ///     Add a task.
        /// </summary>
        public void AddTask(AnalystTask task)
        {
            _tasks[task.Name] = task;
        }

        /// <summary>
        ///     Clear all tasks.
        /// </summary>
        public void ClearTasks()
        {
            _tasks.Clear();
        }

        /// <summary>
        ///     Determine the output format.
        /// </summary>
        /// <returns>The output format.</returns>
        public CSVFormat DetermineFormat()
        {
            return Properties.GetPropertyCSVFormat(
                ScriptProperties.SetupConfigCSVFormat);
        }

        /// <summary>
        ///     Determine if input headers are expected.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>True if headers are expected.</returns>
        public bool ExpectInputHeaders(String filename)
        {
            if (IsGenerated(filename))
            {
                return true;
            }
            return _properties
                .GetPropertyBoolean(ScriptProperties.SetupConfigInputHeaders);
        }

        /// <summary>
        ///     Find the specified data field.  Use name to find, and ignore case.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <returns>The specified data field.</returns>
        public DataField FindDataField(String name)
        {
            return
                _fields.FirstOrDefault(
                    dataField => dataField.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        ///     Find the specified data field and return its index.
        /// </summary>
        /// <param name="df">The data field to search for.</param>
        /// <returns>The index of the specified data field, or -1 if not found.</returns>
        public int FindDataFieldIndex(DataField df)
        {
            for (int result = 0; result < _fields.Length; result++)
            {
                if (df == _fields[result])
                {
                    return result;
                }
            }
            return -1;
        }

        /// <summary>
        ///     Find the specified normalized field.  Search without case.
        /// </summary>
        /// <param name="name">The name of the field we are searching for.</param>
        /// <param name="slice">The timeslice.</param>
        /// <returns>The analyst field that was found.</returns>
        public AnalystField FindNormalizedField(String name,
                                                int slice)
        {
            return
                Normalize.NormalizedFields.FirstOrDefault(
                    field =>
                    field.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) && (field.TimeSlice == slice));
        }


        /// <summary>
        ///     Get the specified task.
        /// </summary>
        /// <param name="name">The name of the testk.</param>
        /// <returns>The analyst task.</returns>
        public AnalystTask GetTask(String name)
        {
            if (!_tasks.ContainsKey(name))
            {
                return null;
            }
            return _tasks[name];
        }


        /// <summary>
        ///     Init this script.
        /// </summary>
        public void Init()
        {
            _normalize.Init(this);
        }

        /// <summary>
        ///     Determine if the specified file was generated.
        /// </summary>
        /// <param name="filename">The filename to check.</param>
        /// <returns>True, if the specified file was generated.</returns>
        public bool IsGenerated(String filename)
        {
            return _generated.Contains(filename);
        }

        /// <summary>
        ///     Load the script.
        /// </summary>
        /// <param name="stream">The stream to load from.</param>
        public void Load(Stream stream)
        {
            var s = new ScriptLoad(this);
            s.Load(stream);
        }

        /// <summary>
        ///     Mark the sepcified filename as generated.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void MarkGenerated(String filename)
        {
            if (!_generated.Contains(filename))
                _generated.Add(filename);
        }

        /// <summary>
        ///     Resolve the specified filename.
        /// </summary>
        /// <param name="sourceID">The filename to resolve.</param>
        /// <returns>The file path.</returns>
        public FileInfo ResolveFilename(String sourceID)
        {
            String name = Properties.GetFilename(sourceID);

            if (name.IndexOf(Path.PathSeparator) == -1 && _basePath != null)
            {
                return FileUtil.CombinePath(new FileInfo(_basePath), name);
            }
            return new FileInfo(name);
        }

        /// <summary>
        ///     Save to the specified output stream.
        /// </summary>
        /// <param name="stream">The output stream.</param>
        public void Save(Stream stream)
        {
            var s = new ScriptSave(this);
            s.Save(stream);
        }

        /// <summary>
        ///     Find the specified analyst field, by name.
        /// </summary>
        /// <param name="name">The name of the analyst field.</param>
        /// <returns>The analyst field.</returns>
        public AnalystField FindAnalystField(string name)
        {
            return _normalize.NormalizedFields.FirstOrDefault(f => string.Compare(name, f.Name, true) == 0);
        }
    }
}
