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
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using Encog.App.Analyst.Analyze;
using Encog.App.Analyst.Commands;
using Encog.App.Analyst.Script;
using Encog.App.Analyst.Script.Normalize;
using Encog.App.Analyst.Script.Prop;
using Encog.App.Analyst.Script.Task;
using Encog.App.Analyst.Wizard;
using Encog.App.Quant;
using Encog.Bot;
using Encog.ML;
using Encog.ML.Bayesian;
using Encog.ML.Train;
using Encog.Util;
using Encog.Util.File;
using Encog.Util.Logging;

namespace Encog.App.Analyst
{
    /// <summary>
    ///     The Encog Analyst runs Encog Analyst Script files (EGA) to perform many
    ///     common machine learning tasks. It is very much like Maven or ANT for Encog.
    ///     Encog analyst files are made up of configuration information and tasks. Tasks
    ///     are series of commands that make use of the configuration information to
    ///     process CSV files.
    /// </summary>
    public class EncogAnalyst
    {
        /// <summary>
        ///     The name of the task that SHOULD everything.
        /// </summary>
        public const String TaskFull = "task-full";

        /// <summary>
        ///     The update time for a download.
        /// </summary>
        public const int UpdateTime = 10;

        /// <summary>
        ///     The commands.
        /// </summary>
        private readonly IDictionary<String, Cmd> _commands;

        /// <summary>
        ///     The listeners.
        /// </summary>
        private readonly IList<IAnalystListener> _listeners;

        /// <summary>
        ///     The analyst script.
        /// </summary>
        private readonly AnalystScript _script;

        /// <summary>
        ///     The current task.
        /// </summary>
        private QuantTask _currentQuantTask;

        /// <summary>
        ///     Holds a copy of the original property data, used to revert.
        /// </summary>
        private IDictionary<String, String> _revertData;

        /// <summary>
        ///     Construct the Encog analyst.
        /// </summary>
        public EncogAnalyst()
        {
            _script = new AnalystScript();
            _listeners = new List<IAnalystListener>();
            _currentQuantTask = null;
            _commands = new Dictionary<String, Cmd>();
            MaxIteration = -1;
            AddCommand(new CmdCreate(this));
            AddCommand(new CmdEvaluate(this));
            AddCommand(new CmdEvaluateRaw(this));
            AddCommand(new CmdGenerate(this));
            AddCommand(new CmdNormalize(this));
            AddCommand(new CmdRandomize(this));
            AddCommand(new CmdSegregate(this));
            AddCommand(new CmdTrain(this));
            AddCommand(new CmdBalance(this));
            AddCommand(new CmdSet(this));
            AddCommand(new CmdReset(this));
            AddCommand(new CmdCluster(this));
            AddCommand(new CmdCode(this));
            AddCommand(new CmdProcess(this));
        }

        /// <summary>
        /// </summary>
        public IMLMethod Method { get; set; }

        /// <value>The lag depth.</value>
        public int LagDepth
        {
            get
            {
                return _script.Normalize.NormalizedFields.Where(field => field.TimeSlice < 0)
                              .Aggregate(0, (current, field) => Math.Max(current, Math.Abs(field.TimeSlice)));
            }
        }


        /// <value>The lead depth.</value>
        public int LeadDepth
        {
            get
            {
                return _script.Normalize.NormalizedFields.Where(field => field.TimeSlice > 0)
                              .Aggregate(0, (current, field) => Math.Max(current, field.TimeSlice));
            }
        }


        /// <value>the listeners</value>
        public IList<IAnalystListener> Listeners
        {
            get { return _listeners; }
        }


        /// <summary>
        ///     Set the max iterations.
        /// </summary>
        public int MaxIteration { get; set; }


        /// <value>The reverted data.</value>
        public IDictionary<String, String> RevertData
        {
            get { return _revertData; }
        }


        /// <value>the script</value>
        public AnalystScript Script
        {
            get { return _script; }
        }

        /// <summary>
        ///     Set the current task.
        /// </summary>
        public QuantTask CurrentQuantTask
        {
            set { _currentQuantTask = value; }
        }

        /// <value>True, if any field has a time slice.</value>
        public bool TimeSeries
        {
            get { return _script.Normalize.NormalizedFields.Any(field => field.TimeSlice != 0); }
        }

        /// <summary>
        ///     Add a listener.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        public void AddAnalystListener(IAnalystListener listener)
        {
            _listeners.Add(listener);
        }

        /// <summary>
        ///     Add a command.
        /// </summary>
        /// <param name="cmd">The command to add.</param>
        public void AddCommand(Cmd cmd)
        {
            _commands[cmd.Name] = cmd;
        }

        /// <summary>
        ///     Analyze the specified file. Used by the wizard.
        /// </summary>
        /// <param name="file">The file to analyze.</param>
        /// <param name="headers">True if headers are present.</param>
        /// <param name="format">The format of the file.</param>
        public void Analyze(FileInfo file, bool headers,
                            AnalystFileFormat format)
        {
            _script.Properties.SetFilename(AnalystWizard.FileRaw,
                                           file.ToString());

            _script.Properties.SetProperty(
                ScriptProperties.SetupConfigInputHeaders, headers);

            var a = new PerformAnalysis(_script,
                                        file.ToString(), headers, format);
            a.Process(this);
        }

        /// <summary>
        ///     Determine the input count.  This is the actual number of columns.
        /// </summary>
        /// <returns>The input count.</returns>
        public int DetermineInputCount()
        {
            int result = 0;

            foreach (AnalystField field in _script.Normalize.NormalizedFields)
            {
                if (field.Input && !field.Ignored)
                {
                    result += field.ColumnsNeeded;
                }
            }
            return result;
        }

        /// <summary>
        ///     Determine the input field count, the fields are higher-level
        ///     than columns.
        /// </summary>
        /// <returns>The input field count.</returns>
        public int DetermineInputFieldCount()
        {
            int result = 0;

            foreach (AnalystField field in _script.Normalize.NormalizedFields)
            {
                if (field.Input && !field.Ignored)
                {
                    result++;
                }
            }
            return result;
        }

        /// <summary>
        ///     Determine the output count, this is the number of output
        ///     columns needed.
        /// </summary>
        /// <returns>The output count.</returns>
        public int DetermineOutputCount()
        {
            int result = 0;

            foreach (AnalystField field in _script.Normalize.NormalizedFields)
            {
                if (field.Output && !field.Ignored)
                {
                    result += field.ColumnsNeeded;
                }
            }
            return result;
        }

        /// <summary>
        ///     Determine the number of output fields.  Fields are higher
        ///     level than columns.
        /// </summary>
        /// <returns>The output field count.</returns>
        public int DetermineOutputFieldCount()
        {
            int result = 0;

            foreach (AnalystField field in _script.Normalize.NormalizedFields)
            {
                if (field.Output && !field.Ignored)
                {
                    result++;
                }
            }

            if (Method is BayesianNetwork)
            {
                result++;
            }

            return result;
        }

        /// <summary>
        ///     Determine how many unique columns there are.  Timeslices are not
        ///     counted multiple times.
        /// </summary>
        /// <returns>The number of columns.</returns>
        public int DetermineUniqueColumns()
        {
            IDictionary<String, Object> used = new Dictionary<String, Object>();
            int result = 0;


            foreach (AnalystField field in _script.Normalize.NormalizedFields)
            {
                if (!field.Ignored)
                {
                    String name = field.Name;
                    if (!used.ContainsKey(name))
                    {
                        result += field.ColumnsNeeded;
                        used[name] = null;
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///     Determine the unique input field count.  Timeslices are not
        ///     counted multiple times.
        /// </summary>
        /// <returns>The number of unique input fields.</returns>
        public int DetermineUniqueInputFieldCount()
        {
            IDictionary<String, Object> map = new Dictionary<String, Object>();

            int result = 0;

            foreach (AnalystField field in _script.Normalize.NormalizedFields)
            {
                if (!map.ContainsKey(field.Name))
                {
                    if (field.Input && !field.Ignored)
                    {
                        result++;
                        map[field.Name] = null;
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///     Determine the unique output field count.  Do not count timeslices
        ///     multiple times.
        /// </summary>
        /// <returns>The unique output field count.</returns>
        public int DetermineUniqueOutputFieldCount()
        {
            IDictionary<String, Object> map = new Dictionary<String, Object>();
            int result = 0;

            foreach (AnalystField field in _script.Normalize.NormalizedFields)
            {
                if (!map.ContainsKey(field.Name))
                {
                    if (field.Output && !field.Ignored)
                    {
                        result++;
                    }
                    map[field.Name] = null;
                }
            }
            return result;
        }

        /// <summary>
        ///     Download a raw file from the Internet.
        /// </summary>
        public void Download()
        {
            Uri sourceURL = _script.Properties.GetPropertyURL(
                ScriptProperties.HeaderDatasourceSourceFile);

            String rawFile = _script.Properties.GetPropertyFile(
                ScriptProperties.HeaderDatasourceRawFile);

            FileInfo rawFilename = Script.ResolveFilename(rawFile);

            if (!rawFilename.Exists)
            {
                DownloadPage(sourceURL, rawFilename);
            }
        }

        /// <summary>
        ///     Down load a file from the specified URL, uncompress if needed.
        /// </summary>
        /// <param name="url">THe URL.</param>
        /// <param name="file">The file to down load into.</param>
        private void DownloadPage(Uri url, FileInfo file)
        {
            try
            {
                // download the URL	
                file.Delete();
                FileInfo tempFile = FileUtil.CombinePath(
                    new FileInfo(file.DirectoryName), "temp.tmp");
                tempFile.Delete();
                FileStream fos = tempFile.OpenWrite();
                int lastUpdate = 0;

                int length = 0;
                int size = 0;
                var buffer = new byte[BotUtil.BufferSize];
                WebRequest http = WebRequest.Create(url);
                var response = (HttpWebResponse) http.GetResponse();
                Stream istream = response.GetResponseStream();


                do
                {
                    length = istream.Read(buffer, 0, buffer.Length);
                    if (length > 0)
                    {
                        if (length >= 0)
                        {
                            fos.Write(buffer, 0, length);
                            size += length;
                        }

                        if (lastUpdate > UpdateTime)
                        {
                            Report(0, (int) (size/Format.MemoryMeg),
                                   "Downloading... " + Format.FormatMemory(size));
                            lastUpdate = 0;
                        }
                        lastUpdate++;
                    }
                } while (length > 0);
                fos.Close();

                // unzip if needed

                if (url.ToString().ToLower().EndsWith(".gz"))
                {
                    // Get the stream of the source file.
                    using (FileStream inFile = tempFile.OpenRead())
                    {
                        //Create the decompressed file.
                        using (FileStream outFile = file.Create())
                        {
                            using (var Decompress = new GZipStream(inFile,
                                                                   CompressionMode.Decompress))
                            {
                                size = 0;
                                lastUpdate = 0;

                                do
                                {
                                    length = Decompress.Read(buffer, 0, buffer.Length);

                                    if (length >= 0)
                                    {
                                        outFile.Write(buffer, 0, length);
                                        size += length;
                                    }

                                    if (lastUpdate > UpdateTime)
                                    {
                                        Report(0, (int) (size/Format.MemoryMeg),
                                               "Uncompressing... " + Format.FormatMemory(size));
                                        lastUpdate = 0;
                                    }
                                    lastUpdate++;
                                } while (length > 0);
                            }
                        }
                        tempFile.Delete();
                    }
                }
                else
                {
                    // rename the temp file to the actual file
                    file.Delete();
                    tempFile.MoveTo(file.ToString());
                }
            }
            catch (IOException e)
            {
                throw new AnalystError(e);
            }
        }

        /// <summary>
        ///     Execute a task.
        /// </summary>
        /// <param name="task">The task to execute.</param>
        public void ExecuteTask(AnalystTask task)
        {
            int total = task.Lines.Count;
            int current = 1;

            foreach (String line in task.Lines)
            {
                EncogLogging.Log(EncogLogging.LevelDebug, "Execute analyst line: "
                                                          + line);
                ReportCommandBegin(total, current, line);

                bool canceled = false;
                String command;
                String args;

                String line2 = line.Trim();
                int index = line2.IndexOf(' ');
                if (index != -1)
                {
                    command = line2.Substring(0, (index) - (0)).ToUpper();
                    args = line2.Substring(index + 1);
                }
                else
                {
                    command = line2.ToUpper();
                    args = "";
                }

                Cmd cmd = _commands[command];

                if (cmd != null)
                {
                    canceled = cmd.ExecuteCommand(args);
                }
                else
                {
                    throw new AnalystError("Unknown Command: " + line);
                }

                ReportCommandEnd(canceled);
                CurrentQuantTask = null;
                current++;

                if (ShouldStopAll())
                {
                    break;
                }
            }
        }

        /// <summary>
        ///     Execute a task.
        /// </summary>
        /// <param name="name">The name of the task to execute.</param>
        public void ExecuteTask(String name)
        {
            EncogLogging.Log(EncogLogging.LevelInfo, "Analyst execute task:"
                                                     + name);
            AnalystTask task = _script.GetTask(name);
            if (task == null)
            {
                throw new AnalystError("Can't find task: " + name);
            }

            ExecuteTask(task);
        }


        /// <summary>
        ///     Load the specified script file.
        /// </summary>
        /// <param name="file">The file to load.</param>
        public void Load(FileInfo file)
        {
            Stream fis = null;
            _script.BasePath = file.DirectoryName;

            try
            {
                fis = file.OpenRead();
                Load(fis);
            }
            catch (IOException ex)
            {
                throw new AnalystError(ex);
            }
            finally
            {
                if (fis != null)
                {
                    try
                    {
                        fis.Close();
                    }
                    catch (IOException e)
                    {
                        throw new AnalystError(e);
                    }
                }
            }
        }

        /// <summary>
        ///     Load from an input stream.
        /// </summary>
        /// <param name="stream">The stream to load from.</param>
        public void Load(Stream stream)
        {
            _script.Load(stream);
            _revertData = _script.Properties.PrepareRevert();
        }

        /// <summary>
        ///     Load from the specified filename.
        /// </summary>
        /// <param name="filename">The filename to load from.</param>
        public void Load(String filename)
        {
            Load(new FileInfo(filename));
        }

        /// <summary>
        ///     Remove a listener.
        /// </summary>
        /// <param name="listener">The listener to remove.</param>
        public void RemoveAnalystListener(IAnalystListener listener)
        {
            _listeners.Remove(listener);
        }

        /// <summary>
        ///     Report progress.
        /// </summary>
        /// <param name="total">The total units.</param>
        /// <param name="current">The current unit.</param>
        /// <param name="message">The message.</param>
        private void Report(int total, int current, String message)
        {
            foreach (IAnalystListener listener in _listeners)
            {
                listener.Report(total, current, message);
            }
        }

        /// <summary>
        ///     Report a command has begin.
        /// </summary>
        /// <param name="total">The total units.</param>
        /// <param name="current">The current unit.</param>
        /// <param name="name">The command name.</param>
        private void ReportCommandBegin(int total, int current,
                                        String name)
        {
            foreach (IAnalystListener listener in _listeners)
            {
                listener.ReportCommandBegin(total, current, name);
            }
        }

        /// <summary>
        ///     Report a command has ended.
        /// </summary>
        /// <param name="canceled">Was the command canceled.</param>
        private void ReportCommandEnd(bool canceled)
        {
            foreach (IAnalystListener listener in _listeners)
            {
                listener.ReportCommandEnd(canceled);
            }
        }

        /// <summary>
        ///     Report training.
        /// </summary>
        /// <param name="train">The trainer.</param>
        public void ReportTraining(IMLTrain train)
        {
            foreach (IAnalystListener listener in _listeners)
            {
                listener.ReportTraining(train);
            }
        }

        /// <summary>
        ///     Report that training has begun.
        /// </summary>
        public void ReportTrainingBegin()
        {
            foreach (IAnalystListener listener in _listeners)
            {
                listener.ReportTrainingBegin();
            }
        }

        /// <summary>
        ///     Report that training has ended.
        /// </summary>
        public void ReportTrainingEnd()
        {
            foreach (IAnalystListener listener in _listeners)
            {
                listener.ReportTrainingEnd();
            }
        }

        /// <summary>
        ///     Save the script to a file.
        /// </summary>
        /// <param name="file">The file to save to.</param>
        public void Save(FileInfo file)
        {
            Stream fos = null;

            try
            {
                _script.BasePath = file.DirectoryName;
                fos = file.OpenWrite();
                Save(fos);
            }
            catch (IOException ex)
            {
                throw new AnalystError(ex);
            }
            finally
            {
                if (fos != null)
                {
                    try
                    {
                        fos.Close();
                    }
                    catch (IOException e)
                    {
                        throw new AnalystError(e);
                    }
                }
            }
        }

        /// <summary>
        ///     Save the script to a stream.
        /// </summary>
        /// <param name="stream">The stream to save to.</param>
        public void Save(Stream stream)
        {
            _script.Save(stream);
        }

        /// <summary>
        ///     Save the script to a filename.
        /// </summary>
        /// <param name="filename">The filename to save to.</param>
        public void Save(String filename)
        {
            Save(new FileInfo(filename));
        }


        /// <summary>
        ///     Should all commands be stopped.
        /// </summary>
        /// <returns>True, if all commands should be stopped.</returns>
        private bool ShouldStopAll()
        {
            foreach (IAnalystListener listener in _listeners)
            {
                if (listener.ShouldShutDown())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Should the current command be stopped.
        /// </summary>
        /// <returns>True if the current command should be stopped.</returns>
        public bool ShouldStopCommand()
        {
            foreach (IAnalystListener listener in _listeners)
            {
                if (listener.ShouldStopCommand())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Stop the current task.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void StopCurrentTask()
        {
            if (_currentQuantTask != null)
            {
                _currentQuantTask.RequestStop();
            }
        }

        /// <summary>
        ///     Determine the total number of columns.
        /// </summary>
        /// <returns>The number of tes</returns>
        public int DetermineTotalColumns()
        {
            int result = 0;

            foreach (AnalystField field in _script.Normalize.NormalizedFields)
            {
                if (!field.Ignored)
                {
                    result += field.ColumnsNeeded;
                }
            }
            return result;
        }

        /// <summary>
        ///     Determine the total input field count, minus ignored fields.
        /// </summary>
        /// <returns>The number of unique input fields.</returns>
        public int DetermineTotalInputFieldCount()
        {
            int result = 0;
            foreach (AnalystField field in _script.Normalize.NormalizedFields)
            {
                if (field.Input && !field.Ignored)
                {
                    result += field.ColumnsNeeded;
                }
            }

            return result;
        }

        public int DetermineMaxTimeSlice()
        {
            int result = int.MinValue;
            foreach (AnalystField field in Script.Normalize.NormalizedFields)
            {
                result = Math.Max(result, field.TimeSlice);
            }
            return result;
        }

        public int DetermineMinTimeSlice()
        {
            int result = int.MaxValue;
            foreach (AnalystField field in Script.Normalize.NormalizedFields)
            {
                result = Math.Min(result, field.TimeSlice);
            }
            return result;
        }
    }
}
