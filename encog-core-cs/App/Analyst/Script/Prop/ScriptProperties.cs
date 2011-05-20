using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public const String HeaderDatasourceSourceFile = "HEADER:DATASOURCE_sourceFile";

        /// <summary>
        /// Property for: "HEADER:DATASOURCE_rawFile".
        /// </summary>
        ///
        public const String HeaderDatasourceRawFile = "HEADER:DATASOURCE_rawFile";

        /// <summary>
        /// Property for: "HEADER:DATASOURCE_sourceFormat".
        /// </summary>
        ///
        public const String HeaderDatasourceSourceFormat = "HEADER:DATASOURCE_sourceFormat";

        /// <summary>
        /// Property for: "HEADER:DATASOURCE_sourceHeaders".
        /// </summary>
        ///
        public const String HeaderDatasourceSourceHeaders = "HEADER:DATASOURCE_sourceHeaders";

        /// <summary>
        /// Property for: "SETUP:CONFIG_maxClassCount".
        /// </summary>
        ///
        public const String SetupConfigMaxClassCount = "SETUP:CONFIG_maxClassCount";

        /// <summary>
        /// Property for: = "SETUP:CONFIG_allowedClasses". 
        /// </summary>
        ///
        public const String SetupConfigAllowedClasses = "SETUP:CONFIG_allowedClasses";

        /// <summary>
        /// Property for: "SETUP:CONFIG_inputHeaders". 
        /// </summary>
        ///
        public const String SetupConfigInputHeaders = "SETUP:CONFIG_inputHeaders";

        /// <summary>
        /// Property for: "SETUP:CONFIG_csvFormat". 
        /// </summary>
        ///
        public const String SetupConfigCSVFormat = "SETUP:CONFIG_csvFormat";

        /// <summary>
        /// Property for: "DATA:CONFIG_goal". 
        /// </summary>
        ///
        public const String DataConfigGoal = "DATA:CONFIG_goal";

        /// <summary>
        /// Property for: "NORMALIZE:CONFIG_sourceFile". 
        /// </summary>
        ///
        public const String NormalizeConfigSourceFile = "NORMALIZE:CONFIG_sourceFile";

        /// <summary>
        /// Property for: "NORMALIZE:CONFIG_targetFile". 
        /// </summary>
        ///
        public const String NormalizeConfigTargetFile = "NORMALIZE:CONFIG_targetFile";

        /// <summary>
        /// Property for: "BALANCE:CONFIG_sourceFile".
        /// </summary>
        ///
        public const String BalanceConfigSourceFile = "BALANCE:CONFIG_sourceFile";

        /// <summary>
        /// Property for: "BALANCE:CONFIG_targetFile". 
        /// </summary>
        ///
        public const String BalanceConfigTargetFile = "BALANCE:CONFIG_targetFile";

        /// <summary>
        /// Property for: "BALANCE:CONFIG_balanceField". 
        /// </summary>
        ///
        public const String BalanceConfigBalanceField = "BALANCE:CONFIG_balanceField";

        /// <summary>
        /// Property for: "BALANCE:CONFIG_countPer". 
        /// </summary>
        ///
        public const String BalanceConfigCountPer = "BALANCE:CONFIG_countPer";

        /// <summary>
        /// Property for: "RANDOMIZE:CONFIG_sourceFile".
        /// </summary>
        ///
        public const String RandomizeConfigSourceFile = "RANDOMIZE:CONFIG_sourceFile";

        /// <summary>
        /// Property for: "RANDOMIZE:CONFIG_targetFile". 
        /// </summary>
        ///
        public const String RandomizeConfigTargetFile = "RANDOMIZE:CONFIG_targetFile";

        /// <summary>
        /// Property for: "SEGREGATE:CONFIG_sourceFile". 
        /// </summary>
        ///
        public const String SegregateConfigSourceFile = "SEGREGATE:CONFIG_sourceFile";

        /// <summary>
        /// Property for: "GENERATE:CONFIG_sourceFile". 
        /// </summary>
        ///
        public const String GenerateConfigSourceFile = "GENERATE:CONFIG_sourceFile";

        /// <summary>
        /// Property for: "GENERATE:CONFIG_targetFile". 
        /// </summary>
        ///
        public const String GenerateConfigTargetFile = "GENERATE:CONFIG_targetFile";

        /// <summary>
        /// Property for: "ML:CONFIG_trainingFile". 
        /// </summary>
        ///
        public const String MlConfigTrainingFile = "ML:CONFIG_trainingFile";

        /// <summary>
        /// Property for: "ML:CONFIG_evalFile".
        /// </summary>
        ///
        public const String MlConfigEvalFile = "ML:CONFIG_evalFile";

        /// <summary>
        /// Property for: "ML:CONFIG_machineLearningFile".
        /// </summary>
        ///
        public const String MlConfigMachineLearningFile = "ML:CONFIG_machineLearningFile";

        /// <summary>
        /// Property for: "ML:CONFIG_outputFile". 
        /// </summary>
        ///
        public const String MlConfigOutputFile = "ML:CONFIG_outputFile";

        /// <summary>
        /// Property for: = ML:CONFIG_type". 
        /// </summary>
        ///
        public const String MlConfigType = "ML:CONFIG_type";

        /// <summary>
        /// Property for: "ML:CONFIG_architecture". 
        /// </summary>
        ///
        public const String MlConfigArchitecture = "ML:CONFIG_architecture";

        /// <summary>
        /// Property for: "ML:TRAIN_type". 
        /// </summary>
        ///
        public const String MlTrainType = "ML:TRAIN_type";

        /// <summary>
        /// Property for: "ML:TRAIN_arguments". 
        /// </summary>
        ///
        public const String MlTrainArguments = "ML:TRAIN_arguments";

        /// <summary>
        /// Property for: "ML:TRAIN_targetError". 
        /// </summary>
        ///
        public const String MlTrainTargetError = "ML:TRAIN_targetError";

        /// <summary>
        /// Property for: "ML:TRAIN_cross". 
        /// </summary>
        ///
        public const String MlTrainCross = "ML:TRAIN_cross";

        /// <summary>
        /// Property for: "CLUSTER:CONFIG_sourceFile".
        /// </summary>
        ///
        public const String ClusterConfigSourceFile = "CLUSTER:CONFIG_sourceFile";

        /// <summary>
        /// Property for: "CLUSTER:CONFIG_targetFile". 
        /// </summary>
        ///
        public const String ClusterConfigTargetFile = "CLUSTER:CONFIG_targetFile";

        /// <summary>
        /// Property for: "CLUSTER:CONFIG_type". 
        /// </summary>
        ///
        public const String ClusterConfigType = "CLUSTER:CONFIG_type";

        /// <summary>
        /// Property for: "CLUSTER:CONFIG_clusters". 
        /// </summary>
        ///
        public const String ClusterConfigClusters = "CLUSTER:CONFIG_clusters";

        /// <summary>
        /// Properties are stored in this map.
        /// </summary>
        ///
        private readonly IDictionary<String, String> _data;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public ScriptProperties()
        {
            _data = new Dictionary<String, String>();
        }

        /// <summary>
        /// Get all filenames.
        /// </summary>
        public IList<String> Filenames
        {
            get
            {
                return (from key in _data.Keys
                        where key.StartsWith("SETUP:FILENAMES")
                        let index = key.IndexOf('_')
                        where index != -1
                        select key.Substring(index + 1)).ToList();
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
            var array = new string[_data.Keys.Count];

            int idx = 0;
            foreach (string item in _data.Keys)
            {
                array[idx++] = item;
            }

            foreach (string element  in  array)
            {
                var key = element;
                if (key.StartsWith("SETUP:FILENAMES"))
                {
                    _data.Remove(key);
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

            if (!_data.ContainsKey(key2))
            {
                throw new AnalystError("Undefined file: " + file);
            }

            return _data[key2];
        }


        /// <summary>
        /// Get a property as an object.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <returns>The property value.</returns>
        public Object GetProperty(String name)
        {
            return _data[name];
        }

        /// <summary>
        /// Get a property as a boolean.
        /// </summary>
        ///
        /// <param name="name">The property name.</param>
        /// <returns>A boolean value.</returns>
        public bool GetPropertyBoolean(String name)
        {
            if (!_data.ContainsKey(name))
            {
                return false;
            }
            return _data[name].ToLower().StartsWith("t");
        }

        /// <summary>
        /// Get a property as a format.
        /// </summary>
        ///
        /// <param name="name">The property name.</param>
        /// <returns>A format value.</returns>
        public CSVFormat GetPropertyCSVFormat(String name)
        {
            String v = _data[name];
            AnalystFileFormat code = ConvertStringConst
                .String2AnalystFileFormat(v);
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
            String v = _data[name];
            return CSVFormat.EG_FORMAT.Parse(v);
        }

        /// <summary>
        /// Get a property as a file.
        /// </summary>
        ///
        /// <param name="name">The property name.</param>
        /// <returns>A file value.</returns>
        public String GetPropertyFile(String name)
        {
            return _data[name];
        }

        /// <summary>
        /// Get a property as a format.
        /// </summary>
        ///
        /// <param name="name">The property name.</param>
        /// <returns>A format value.</returns>
        public AnalystFileFormat GetPropertyFormat(String name)
        {
            String v = _data[name];
            return ConvertStringConst.String2AnalystFileFormat(v);
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
                String v = _data[name];
                if (v == null)
                {
                    return 0;
                }
                return Int32.Parse(v);
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
            if (!_data.ContainsKey(name))
            {
                return null;
            }
            return _data[name];
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
                return new Uri(_data[name]);
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
            _data.Clear();
            EngineArray.PutAll(revertedData, _data);
        }

        /// <summary>
        /// Prepare a revert. 
        /// </summary>
        ///
        /// <returns>Data that can be used to revert properties.</returns>
        public IDictionary<String, String> PrepareRevert()
        {
            IDictionary<String, String> result = new Dictionary<String, String>();
            EngineArray.PutAll(_data, result);
            return result;
        }

        /// <summary>
        /// Set a filename.
        /// </summary>
        ///
        /// <param name="key">The key.</param>
        /// <param name="v">The value.</param>
        public void SetFilename(String key, String v)
        {
            String key2 = "SETUP:FILENAMES_" + key;
            _data[key2] = v;
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
            _data[name] = ConvertStringConst.AnalystFileFormat2String(format);
        }

        /// <summary>
        /// Set a property.
        /// </summary>
        ///
        /// <param name="name">The name.</param>
        /// <param name="v">The value.</param>
        public void SetProperty(String name, AnalystGoal v)
        {
            switch (v)
            {
                case AnalystGoal.Classification:
                    _data[name] = "classification";
                    break;
                case AnalystGoal.Regression:
                    _data[name] = "regression";
                    break;
                default:
                    _data[name] = "";
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
                _data[name] = "t";
            }
            else
            {
                _data[name] = "f";
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
            _data[name] = CSVFormat.EG_FORMAT.Format(d, EncogFramework.DEFAULT_PRECISION);
        }

        /// <summary>
        /// Get a property as an object.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="f">The filename value.</param>
        public void SetProperty(String name, FileInfo f)
        {
            _data[name] = f.ToString();
        }

        /// <summary>
        /// Set a property to an int.
        /// </summary>
        ///
        /// <param name="name">The property name.</param>
        /// <param name="i">The value.</param>
        public void SetProperty(String name, int i)
        {
            _data[name] = ("" + i);
        }

        /// <summary>
        /// Set the property to the specified value.
        /// </summary>
        ///
        /// <param name="name">The property name.</param>
        /// <param name="v">The property value.</param>
        public void SetProperty(String name, String v)
        {
            _data[name] = v;
        }

        /// <summary>
        /// Get a property as an object.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="url">The url of the property.</param>
        public void SetProperty(String name, Uri url)
        {
            _data[name] = url.ToString();
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
            result.Append(_data.ToString());
            result.Append("]");
            return result.ToString();
        }
    }
}