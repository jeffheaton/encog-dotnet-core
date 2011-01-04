using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Engine;
using System.Diagnostics;
using Encog.Engine.Util;
using System.Text.RegularExpressions;
using Encog.App.Script.Commands;
using Encog.Util;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks;
using System.IO;

namespace Encog.App.Script
{
    public class EncogQuantScript
    {
        private IStatusReportable report;
        private String[] lines;
        private IDictionary<String, IQuantCommand> commands = new Dictionary<String, IQuantCommand>();
        private IDictionary<String, object> memory = new Dictionary<String, object>();

        public EncogQuantScript(IStatusReportable report)
        {
            this.report = report;
            AddCommand(new CmdWriteLine());
            AddCommand(new CmdSet());
            AddCommand(new CmdDatasetCreate());
            AddCommand(new CmdDatasetAdd());
            AddCommand(new CmdDatasetDump());
            AddCommand(new CmdNetworkCreate());
            AddCommand(new CmdTrain());
            AddCommand(new CmdDatasetEvaluate());
            AddCommand(new CmdNinjaConvert());
        }

        public IDictionary<String, object> Memory
        {
            get
            {
                return this.memory;
            }
        }

        public void WriteLine(String str)
        {
            report.Report(0, 0, str);
        }

        public void WriteLine()
        {
            WriteLine("");
        }

        public void ExecuteScript(String script)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            this.lines = lines = Regex.Split(script, "\r\n");
            WriteLine("Script beginning: " + DateTime.Now);
            WriteLine();
            Execute();
            WriteLine();
            sw.Stop();
            WriteLine("Script has ended: " + DateTime.Now);
            WriteLine("Total execution time: " + Format.FormatTimeSpan((int)sw.ElapsedMilliseconds/1000));
        }

        private void Execute()
        {
            for (int i = 0; i < lines.Length; i++)
            {
                ExecuteLine(this.lines[i]);
            }
        }

        public void ExecuteLine(String line)
        {
            ParseLine parse = new ParseLine(line);
            if( parse.Command.Length == 0)
            {
                // ignore blank lines
            }
            else if (this.commands.ContainsKey(parse.Command))
            {
                this.commands[parse.Command].Execute(this, parse);
            }
            else
            {
                WriteLine("Unknown command: " + parse.Command);
            }
        }

        public void AddCommand(IQuantCommand command)
        {
            this.commands[command.CommandName] = command;
        }        

        public String ResolveVariables(String str)
        {
            StringBuilder result = new StringBuilder();

            SimpleParser parse = new SimpleParser(str);

            while (!parse.EOL())
            {
                if (parse.LookAhead("${", false))
                {
                    bool success = false;
                    parse.Mark();
                    parse.Advance(2);
                    foreach (String key in Memory.Keys)
                    {
                        if (parse.LookAhead(key, true))
                        {
                            parse.Advance(key.Length);
                            if (parse.LookAhead("}",false))
                            {
                                parse.Advance();
                                result.Append( ConvertToString(this.memory[key]) );
                                success = true;
                            }
                        }
                    }

                    if (!success)
                    {
                        parse.Reset();
                        parse.Advance();
                        result.Append("$");
                    }
                }
                else
                {
                    result.Append(parse.ReadChar());
                }
            }

            return result.ToString();
        }


        public String ConvertToString(object o)
        {
            StringBuilder result = new StringBuilder();

            if (o is BasicNeuralDataSet)
            {
                BasicNeuralDataSet set = (BasicNeuralDataSet)o;
                result.Append("[Dataset: records=");
                result.Append(set.Count);
                result.Append(",inputs=");
                result.Append(set.InputSize);
                result.Append(",ideals=");
                result.Append(set.IdealSize);
                result.Append("]");
            }
            else if (o is BasicNetwork)
            {
                BasicNetwork network = (BasicNetwork)o;
                    result.Append("[NeuralNetwork: input=");
                    result.Append(network.InputCount);
                    result.Append(",output=");
                    result.Append(network.OutputCount);
                    result.Append("]");
            }
            else
            {
                result.Append(o.ToString());
            }

            return result.ToString();
        }

        public void ExecuteFile(String filename)
        {
            try
            {
                TextReader tr = new StreamReader(filename);
                String script = tr.ReadToEnd();
                tr.Close();
                ExecuteScript(script);
            }
            catch (IOException ex)
            {
                throw new EncogError(ex);
            }
        }
    }
}
