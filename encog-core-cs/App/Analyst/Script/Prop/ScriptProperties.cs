using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Encog.App.Analyst.Util;
using Encog.Util;
using Encog.Util.CSV;

namespace Encog.App.Analyst.Script.Prop
{
    /// <summary>
    /// Holds all of the properties for a script. Constants are provided to define
    /// "well known" properties.
    /// </summary>
    ///
    public class ScriptProperties
    {
        /// <summary>
        /// Property for: "HEADER:DATASOURCE_sourceFile".
        /// </summary>
        ///
        public const String HEADER_DATASOURCE_SOURCE_FILE = "HEADER:DATASOURCE_sourceFile";

        /// <summary>
        /// Property for: "HEADER:DATASOURCE_rawFile".
        /// </summary>
        ///
        public const String HEADER_DATASOURCE_RAW_FILE = "HEADER:DATASOURCE_rawFile";

        /// <summary>
        /// Property for: "HEADER:DATASOURCE_sourceFormat".
        /// </summary>
        ///
        public const String HEADER_DATASOURCE_SOURCE_FORMAT = "HEADER:DATASOURCE_sourceFormat";

        /// <summary>
        /// Property for: "HEADER:DATASOURCE_sourceHeaders".
        /// </summary>
        ///
        public const String HEADER_DATASOURCE_SOURCE_HEADERS = "HEADER:DATASOURCE_sourceHeaders";

        /// <summary>
        /// Property for: "SETUP:CONFIG_maxClassCount".
        /// </summary>
        ///
        public const String SETUP_CONFIG_MAX_CLASS_COUNT = "SETUP:CONFIG_maxClassCount";

        /// <summary>
        /// Property for: = "SETUP:CONFIG_allowedClasses". 
        /// </summary>
        ///
        public const String SETUP_CONFIG_ALLOWED_CLASSES = "SETUP:CONFIG_allowedClasses";

        /// <summary>
        /// Property for: "SETUP:CONFIG_inputHeaders". 
        /// </summary>
        ///
        public const String SETUP_CONFIG_INPUT_HEADERS = "SETUP:CONFIG_inputHeaders";

        /// <summary>
        /// Property for: "SETUP:CONFIG_csvFormat". 
        /// </summary>
        ///
        public const String SETUP_CONFIG_CSV_FORMAT = "SETUP:CONFIG_csvFormat";

        /// <summary>
        /// Property for: "DATA:CONFIG_goal". 
        /// </summary>
        ///
        public const String DATA_CONFIG_GOAL = "DATA:CONFIG_goal";

        /// <summary>
        /// Property for: "NORMALIZE:CONFIG_sourceFile". 
        /// </summary>
        ///
        public const String NORMALIZE_CONFIG_SOURCE_FILE = "NORMALIZE:CONFIG_sourceFile";

        /// <summary>
        /// Property for: "NORMALIZE:CONFIG_targetFile". 
        /// </summary>
        ///
        public const String NORMALIZE_CONFIG_TARGET_FILE = "NORMALIZE:CONFIG_targetFile";

        /// <summary>
        /// Property for: "BALANCE:CONFIG_sourceFile".
        /// </summary>
        ///
        public const String BALANCE_CONFIG_SOURCE_FILE = "BALANCE:CONFIG_sourceFile";

        /// <summary>
        /// Property for: "BALANCE:CONFIG_targetFile". 
        /// </summary>
        ///
        public const String BALANCE_CONFIG_TARGET_FILE = "BALANCE:CONFIG_targetFile";

        /// <summary>
        /// Property for: "BALANCE:CONFIG_balanceField". 
        /// </summary>
        ///
        public const String BALANCE_CONFIG_BALANCE_FIELD = "BALANCE:CONFIG_balanceField";

        /// <summary>
        /// Property for: "BALANCE:CONFIG_countPer". 
        /// </summary>
        ///
        public const String BALANCE_CONFIG_COUNT_PER = "BALANCE:CONFIG_countPer";

        /// <summary>
        /// Property for: "RANDOMIZE:CONFIG_sourceFile".
        /// </summary>
        ///
        public const String RANDOMIZE_CONFIG_SOURCE_FILE = "RANDOMIZE:CONFIG_sourceFile";

        /// <summary>
        /// Property for: "RANDOMIZE:CONFIG_targetFile". 
        /// </summary>
        ///
        public const String RANDOMIZE_CONFIG_TARGET_FILE = "RANDOMIZE:CONFIG_targetFile";

        /// <summary>
        /// Property for: "SEGREGATE:CONFIG_sourceFile". 
        /// </summary>
        ///
        public const String SEGREGATE_CONFIG_SOURCE_FILE = "SEGREGATE:CONFIG_sourceFile";

        /// <summary>
        /// Property for: "GENERATE:CONFIG_sourceFile". 
        /// </summary>
        ///
        public const String GENERATE_CONFIG_SOURCE_FILE = "GENERATE:CONFIG_sourceFile";

        /// <summary>
        /// Property for: "GENERATE:CONFIG_targetFile". 
        /// </summary>
        ///
        public const String GENERATE_CONFIG_TARGET_FILE = "GENERATE:CONFIG_targetFile";

        /// <summary>
        /// Property for: "ML:CONFIG_trainingFile". 
        /// </summary>
        ///
        public const String ML_CONFIG_TRAINING_FILE = "ML:CONFIG_trainingFile";

        /// <summary>
        /// Property for: "ML:CONFIG_evalFile".
        /// </summary>
        ///
        public const String ML_CONFIG_EVAL_FILE = "ML:CONFIG_evalFile";

        /// <summary>
        /// Property for: "ML:CONFIG_machineLearningFile".
        /// </summary>
        ///
        public const String ML_CONFIG_MACHINE_LEARNING_FILE = "ML:CONFIG_machineLearningFile";

        /// <summary>
        /// Property for: "ML:CONFIG_outputFile". 
        /// </summary>
        ///
        public const String ML_CONFIG_OUTPUT_FILE = "ML:CONFIG_outputFile";

        /// <summary>
        /// Property for: = ML:CONFIG_type". 
        /// </summary>
        ///
        public const String ML_CONFIG_TYPE = "ML:CONFIG_type";

        /// <summary>
        /// Property for: "ML:CONFIG_architecture". 
        /// </summary>
        ///
        public const String ML_CONFIG_ARCHITECTURE = "ML:CONFIG_architecture";

        /// <summary>
        /// Property for: "ML:TRAIN_type". 
        /// </summary>
        ///
        public const String ML_TRAIN_TYPE = "ML:TRAIN_type";

        /// <summary>
        /// Property for: "ML:TRAIN_arguments". 
        /// </summary>
        ///
        public const String ML_TRAIN_ARGUMENTS = "ML:TRAIN_arguments";

        /// <summary>
        /// Property for: "ML:TRAIN_targetError". 
        /// </summary>
        ///
        public const String ML_TRAIN_TARGET_ERROR = "ML:TRAIN_targetError";

        /// <summary>
        /// Property for: "ML:TRAIN_cross". 
        /// </summary>
        ///
        public const String ML_TRAIN_CROSS = "ML:TRAIN_cross";

        /// <summary>
        /// Property for: "CLUSTER:CONFIG_sourceFile".
        /// </summary>
        ///
        public const String CLUSTER_CONFIG_SOURCE_FILE = "CLUSTER:CONFIG_sourceFile";

        /// <summary>
        /// Property for: "CLUSTER:CONFIG_targetFile". 
        /// </summary>
        ///
        public const String CLUSTER_CONFIG_TARGET_FILE = "CLUSTER:CONFIG_targetFile";

        /// <summary>
        /// Property for: "CLUSTER:CONFIG_type". 
        /// </summary>
        ///
        public const String CLUSTER_CONFIG_TYPE = "CLUSTER:CONFIG_type";

        /// <summary>
        /// Property for: "CLUSTER:CONFIG_clusters". 
        /// </summary>
        ///
        public const String CLUSTER_CONFIG_CLUSTERS = "CLUSTER:CONFIG_clusters";

        /// <summary>
        /// Properties are stored in this map.
        /// </summary>
        ///
        private readonly IDictionary<String, String> data;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public ScriptProperties()
        {
            data = new Dictionary<String, String>();
        }

        /// <summary>
        /// Get all filenames.
        /// </summary>
        ///
        /// <value>The filenames in a list.</value>
        public IList<String> Filenames
        {
            /// <summary>
            /// Get all filenames.
            /// </summary>
            ///
            /// <returns>The filenames in a list.</returns>
            get
            {
                IList<String> result = new List<String>();

                foreach (String key  in  data.Keys)
                {
                    if (key.StartsWith("SETUP:FILENAMES"))
                    {
                        int index = key.IndexOf('_');
                        if (index != -1)
                        {
                            result.Add(key.Substring(index + 1));
                        }
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Convert a key to the dot form.
        /// </summary>
        ///
        /// <param name="str">The key form.</param>
        /// <returns>The dot form.</returns>
        public static String ToDots(String str)
        {
            int index1 = str.IndexOf(':');
            if (index1 == -1)
            {
                return null;
            }
            int index2 = str.IndexOf('_');
            if (index2 == -1)
            {
                return null;
            }
            String section = str.Substring(0, (index1) - (0));
            String subSection = str.Substring(index1 + 1, (index2) - (index1 + 1));
            String name = str.Substring(index2 + 1);
            return section + "." + subSection + "." + name;
        }

        /// <summary>
        /// Clear out all filenames.
        /// </summary>
        ///
        public void ClearFilenames()
        {
            var array = new string[data.Keys.Count];

            int idx = 0;
            foreach (string item in data.Keys)
            {
                array[idx++] = item;
            }

            foreach (Object element  in  array)
            {
                var key = (String) element;
                if (key.StartsWith("SETUP:FILENAMES"))
                {
                    data.Remove(key);
                }
            }
        }

        /// <summary>
        /// Get a filename.
        /// </summary>
        ///
        /// <param name="file">The file.</param>
        /// <returns>The filename.</returns>
        public String GetFilename(String file)
        {
            String key2 = "SETUP:FILENAMES_" + file;

            if (!data.ContainsKey(key2))
            {
                throw new AnalystError("Undefined file: " + file);
            }

            return data[key2];
        }


        /// <summary>
        /// Get a property as an object.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <returns>The property value.</returns>
        public Object GetProperty(String name)
        {
            return data[name];
        }

        /// <summary>
        /// Get a property as a boolean.
        /// </summary>
        ///
        /// <param name="name">The property name.</param>
        /// <returns>A boolean value.</returns>
        public bool GetPropertyBoolean(String name)
        {
            if (!data.ContainsKey(name))
            {
                return false;
            }
            else
            {
                return data[name].ToLower().StartsWith("t");
            }
        }

        /// <summary>
        /// Get a property as a format.
        /// </summary>
        ///
        /// <param name="name">The property name.</param>
        /// <returns>A format value.</returns>
        public CSVFormat GetPropertyCSVFormat(String name)
        {
            String value_ren = data[name];
            AnalystFileFormat code = ConvertStringConst
                .String2AnalystFileFormat(value_ren);
            return ConvertStringConst.ConvertToCSVFormat(code);
        }

        /// <summary>
        /// Get a property as a double.
        /// </summary>
        ///
        /// <param name="name">The property name.</param>
        /// <returns>A double value.</returns>
        public double GetPropertyDouble(String name)
        {
            String value_ren = data[name];
            return CSVFormat.EG_FORMAT.Parse(value_ren);
        }

        /// <summary>
        /// Get a property as a file.
        /// </summary>
        ///
        /// <param name="name">The property name.</param>
        /// <returns>A file value.</returns>
        public String GetPropertyFile(String name)
        {
            return data[name];
        }

        /// <summary>
        /// Get a property as a format.
        /// </summary>
        ///
        /// <param name="name">The property name.</param>
        /// <returns>A format value.</returns>
        public AnalystFileFormat GetPropertyFormat(String name)
        {
            String value_ren = data[name];
            return ConvertStringConst.String2AnalystFileFormat(value_ren);
        }

        /// <summary>
        /// Get a property as a int.
        /// </summary>
        ///
        /// <param name="name">The property name.</param>
        /// <returns>A int value.</returns>
        public int GetPropertyInt(String name)
        {
            try
            {
                String value_ren = data[name];
                if (value_ren == null)
                {
                    return 0;
                }
                return Int32.Parse(value_ren);
            }
            catch (FormatException ex)
            {
                throw new AnalystError(ex);
            }
        }

        /// <summary>
        /// Get a property as a string.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <returns>The property value.</returns>
        public String GetPropertyString(String name)
        {
            if (!data.ContainsKey(name))
            {
                return null;
            }
            return data[name];
        }

        /// <summary>
        /// Get a property as a url.
        /// </summary>
        ///
        /// <param name="name">The property name.</param>
        /// <returns>A url value.</returns>
        public Uri GetPropertyURL(String name)
        {
            try
            {
                return new Uri(data[name]);
            }
            catch (UriFormatException e)
            {
                throw new AnalystError(e);
            }
        }

        /// <summary>
        /// Perform a revert.
        /// </summary>
        ///
        /// <param name="revertedData">The source data to revert from.</param>
        public void PerformRevert(IDictionary<String, String> revertedData)
        {
            data.Clear();
            EngineArray.PutAll(revertedData, data);
        }

        /// <summary>
        /// Prepare a revert. 
        /// </summary>
        ///
        /// <returns>Data that can be used to revert properties.</returns>
        public IDictionary<String, String> PrepareRevert()
        {
            IDictionary<String, String> result = new Dictionary<String, String>();
            EngineArray.PutAll(data, result);
            return result;
        }

        /// <summary>
        /// Set a filename.
        /// </summary>
        ///
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetFilename(String key, String value_ren)
        {
            String key2 = "SETUP:FILENAMES_" + key;
            data[key2] = value_ren;
        }

        /// <summary>
        /// Set the property to a format.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="format">The value of the property.</param>
        public void SetProperty(String name,
                                AnalystFileFormat format)
        {
            if (format == null)
            {
                data[name] = "";
            }
            else
            {
                data[name] = ConvertStringConst.AnalystFileFormat2String(format);
            }
        }

        /// <summary>
        /// Set a property.
        /// </summary>
        ///
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void SetProperty(String name, AnalystGoal value_ren)
        {
            switch (value_ren)
            {
                case AnalystGoal.Classification:
                    data[name] = "classification";
                    break;
                case AnalystGoal.Regression:
                    data[name] = "regression";
                    break;
                default:
                    data[name] = "";
                    break;
            }
        }

        /// <summary>
        /// Set a property as a boolean.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="b">The value to set.</param>
        public void SetProperty(String name, bool b)
        {
            if (b)
            {
                data[name] = "t";
            }
            else
            {
                data[name] = "f";
            }
        }

        /// <summary>
        /// Set a property as a double.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="d">The value.</param>
        public void SetProperty(String name, double d)
        {
            data[name] = CSVFormat.EG_FORMAT.Format(d, EncogFramework.DEFAULT_PRECISION);
        }

        /// <summary>
        /// Get a property as an object.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="f">The filename value.</param>
        public void SetProperty(String name, FileInfo f)
        {
            data[name] = f.ToString();
        }

        /// <summary>
        /// Set a property to an int.
        /// </summary>
        ///
        /// <param name="name">The property name.</param>
        /// <param name="i">The value.</param>
        public void SetProperty(String name, int i)
        {
            data[name] = ("" + i);
        }

        /// <summary>
        /// Set the property to the specified value.
        /// </summary>
        ///
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public void SetProperty(String name, String value_ren)
        {
            data[name] = value_ren;
        }

        /// <summary>
        /// Get a property as an object.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="url">The url of the property.</param>
        public void SetProperty(String name, Uri url)
        {
            data[name] = url.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" :");
            result.Append(data.ToString());
            result.Append("]");
            return result.ToString();
        }
    }
}