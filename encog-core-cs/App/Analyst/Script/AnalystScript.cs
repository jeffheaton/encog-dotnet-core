using System;
using System.Collections.Generic;
using System.IO;
using Encog.App.Analyst.Script.Normalize;
using Encog.App.Analyst.Script.Prop;
using Encog.App.Analyst.Script.Segregate;
using Encog.App.Analyst.Script.Task;
using Encog.Util.CSV;
using Encog.Util.File;

namespace Encog.App.Analyst.Script
{
    /// <summary>
    /// Holds a script for the Encog Analyst.
    /// </summary>
    ///
    public class AnalystScript
    {
        /// <summary>
        /// The default MAX size for a class.
        /// </summary>
        ///
        public const int DEFAULT_MAX_CLASS = 50;

        /// <summary>
        /// Tracks which files were generated.
        /// </summary>
        ///
        private readonly IList<String> generated;

        /// <summary>
        /// Information about how to normalize.
        /// </summary>
        ///
        private readonly AnalystNormalize normalize;

        /// <summary>
        /// The properties.
        /// </summary>
        ///
        private readonly ScriptProperties properties;

        /// <summary>
        /// Information about how to segregate.
        /// </summary>
        ///
        private readonly AnalystSegregate segregate;

        /// <summary>
        /// The tasks.
        /// </summary>
        ///
        private readonly IDictionary<String, AnalystTask> tasks;

        /// <summary>
        /// The base path.
        /// </summary>
        ///
        private String basePath;

        /// <summary>
        /// The data fields, these are the raw data from the CSV file.
        /// </summary>
        ///
        private DataField[] fields;

        /// <summary>
        /// Construct an analyst script.
        /// </summary>
        ///
        public AnalystScript()
        {
            normalize = new AnalystNormalize();
            segregate = new AnalystSegregate();
            generated = new List<String>();
            tasks = new Dictionary<String, AnalystTask>();
            properties = new ScriptProperties();
            properties.SetProperty(ScriptProperties.SETUP_CONFIG_CSV_FORMAT,
                                   AnalystFileFormat.DECPNT_COMMA);
            properties.SetProperty(
                ScriptProperties.SETUP_CONFIG_MAX_CLASS_COUNT,
                DEFAULT_MAX_CLASS);
            properties
                .SetProperty(ScriptProperties.SETUP_CONFIG_ALLOWED_CLASSES,
                             "integer,string");
        }

        /// <summary>
        /// Set the base path.
        /// </summary>
        ///
        /// <value>The base path.</value>
        public String BasePath
        {
            /// <returns>The base path.</returns>
            get { return basePath; }
            /// <summary>
            /// Set the base path.
            /// </summary>
            ///
            /// <param name="theBasePath">The base path.</param>
            set { basePath = value; }
        }


        /// <value>the fields to set</value>
        public DataField[] Fields
        {
            /// <returns>the data fields.</returns>
            get { return fields; }
            /// <param name="theFields">the fields to set</param>
            set { fields = value; }
        }


        /// <value>the normalize</value>
        public AnalystNormalize Normalize
        {
            /// <returns>the normalize</returns>
            get { return normalize; }
        }


        /// <value>The precision.</value>
        public int Precision
        {
            /// <returns>The precision.</returns>
            get { return EncogFramework.DEFAULT_PRECISION; }
        }


        /// <value>the properties</value>
        public ScriptProperties Properties
        {
            /// <returns>the properties</returns>
            get { return properties; }
        }


        /// <value>the segregate</value>
        public AnalystSegregate Segregate
        {
            /// <returns>the segregate</returns>
            get { return segregate; }
        }

        /// <value>The tasks.</value>
        public IDictionary<String, AnalystTask> Tasks
        {
            /// <returns>The tasks.</returns>
            get { return tasks; }
        }

        /// <summary>
        /// Add a task.
        /// </summary>
        ///
        /// <param name="task">The task to add.</param>
        public void AddTask(AnalystTask task)
        {
            tasks[task.Name] = task;
        }

        /// <summary>
        /// Clear all tasks.
        /// </summary>
        ///
        public void ClearTasks()
        {
            tasks.Clear();
        }

        /// <summary>
        /// Determine the input format for the specified file.
        /// </summary>
        ///
        /// <param name="sourceID">The file.</param>
        /// <returns>The input format.</returns>
        public CSVFormat DetermineInputFormat(String sourceID)
        {
            String rawID = Properties.GetPropertyString(
                ScriptProperties.HEADER_DATASOURCE_RAW_FILE);
            CSVFormat result;

            if (sourceID.Equals(rawID))
            {
                result = Properties.GetPropertyCSVFormat(
                    ScriptProperties.HEADER_DATASOURCE_SOURCE_FORMAT);
            }
            else
            {
                result = Properties.GetPropertyCSVFormat(
                    ScriptProperties.SETUP_CONFIG_CSV_FORMAT);
            }

            return result;
        }

        /// <summary>
        /// Determine the output format.
        /// </summary>
        ///
        /// <returns>The output format.</returns>
        public CSVFormat DetermineOutputFormat()
        {
            return Properties.GetPropertyCSVFormat(
                ScriptProperties.SETUP_CONFIG_CSV_FORMAT);
        }

        /// <summary>
        /// Determine if input headers are expected.
        /// </summary>
        ///
        /// <param name="filename">The filename.</param>
        /// <returns>True if headers are expected.</returns>
        public bool ExpectInputHeaders(String filename)
        {
            if (IsGenerated(filename))
            {
                return true;
            }
            else
            {
                return properties
                    .GetPropertyBoolean(ScriptProperties.SETUP_CONFIG_INPUT_HEADERS);
            }
        }

        /// <summary>
        /// Find the specified data field.  Use name to find, and ignore case.
        /// </summary>
        ///
        /// <param name="name">The name to search for.</param>
        /// <returns>The specified data field.</returns>
        public DataField FindDataField(String name)
        {
            foreach (DataField dataField  in  fields)
            {
                if (dataField.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return dataField;
                }
            }

            return null;
        }

        /// <summary>
        /// Find the specified data field and return its index.
        /// </summary>
        ///
        /// <param name="df">The data field to search for.</param>
        /// <returns>The index of the specified data field, or -1 if not found.</returns>
        public int FindDataFieldIndex(DataField df)
        {
            for (int result = 0; result < fields.Length; result++)
            {
                if (df == fields[result])
                {
                    return result;
                }
            }
            return -1;
        }

        /// <summary>
        /// Find the specified normalized field.  Search without case.
        /// </summary>
        ///
        /// <param name="name">The name of the field we are searching for.</param>
        /// <param name="slice">The timeslice.</param>
        /// <returns>The analyst field that was found.</returns>
        public AnalystField FindNormalizedField(String name,
                                                int slice)
        {
            foreach (AnalystField field  in  Normalize.NormalizedFields)
            {
                if (field.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                    && (field.TimeSlice == slice))
                {
                    return field;
                }
            }

            return null;
        }


        /// <summary>
        /// Get the specified task.
        /// </summary>
        ///
        /// <param name="name">The name of the testk.</param>
        /// <returns>The analyst task.</returns>
        public AnalystTask GetTask(String name)
        {
            if (!tasks.ContainsKey(name))
            {
                return null;
            }
            return tasks[name];
        }


        /// <summary>
        /// Init this script.
        /// </summary>
        ///
        public void Init()
        {
            normalize.Init(this);
        }

        /// <summary>
        /// Determine if the specified file was generated.
        /// </summary>
        ///
        /// <param name="filename">The filename to check.</param>
        /// <returns>True, if the specified file was generated.</returns>
        public bool IsGenerated(String filename)
        {
            return generated.Contains(filename);
        }

        /// <summary>
        /// Load the script.
        /// </summary>
        ///
        /// <param name="stream">The stream to load from.</param>
        public void Load(Stream stream)
        {
            var s = new ScriptLoad(this);
            s.Load(stream);
        }

        /// <summary>
        /// Mark the sepcified filename as generated.
        /// </summary>
        ///
        /// <param name="filename">The filename.</param>
        public void MarkGenerated(String filename)
        {
            if (!generated.Contains(filename))
                generated.Add(filename);
        }

        /// <summary>
        /// Resolve the specified filename.
        /// </summary>
        ///
        /// <param name="sourceID">The filename to resolve.</param>
        /// <returns>The file path.</returns>
        public FileInfo ResolveFilename(String sourceID)
        {
            String name = Properties.GetFilename(sourceID);

            if ( name.IndexOf(Path.PathSeparator) == -1 && basePath != null)
            {
                return FileUtil.CombinePath(new FileInfo(basePath) , name);
            }
            else
            {
                return new FileInfo(name);
            }
        }

        /// <summary>
        /// Save to the specified output stream.
        /// </summary>
        ///
        /// <param name="stream">The output stream.</param>
        public void Save(Stream stream)
        {
            var s = new ScriptSave(this);
            s.Save(stream);
        }
    }
}