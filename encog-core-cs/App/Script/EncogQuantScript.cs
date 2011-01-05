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
using Encog.App.Script.Util;

namespace Encog.App.Script
{
    public class EncogQuantScript
    {
        private IStatusReportable report;
        private String[] lines;
        private IDictionary<String, IQuantCommand> commands = new Dictionary<String, IQuantCommand>();
        private IDictionary<String, IScriptedObject> memory = new Dictionary<String, IScriptedObject>();

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
            AddCommand(new CmdNetworkLayer());
            AddCommand(new CmdFinalize());
            AddCommand(new CmdFileAdd());
            AddCommand(new CmdFileFind());
            AddCommand(new CmdFileLoad());
            AddCommand(new CmdFileRemove());
            AddCommand(new CmdFileSave());
        }

        public IDictionary<String, IScriptedObject> Memory
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
            try
            {
                Execute();
            }
            catch (ScriptError ex)
            {
                WriteLine("Script ended in an error:");
                WriteLine(ex.Message);
            }
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
            // ignore remarks
            if (line.Trim().StartsWith("//"))
                return;

            // parse the line
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
                throw new ScriptError( "Unknown command: " + parse.Command);
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
                                result.Append( this.memory[key].ToString() );
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

        public  IScriptedObject RequireObject(string name, Type t, bool requireFinal)
        {
            if (!Memory.ContainsKey(name))
            {
                throw new ScriptError("Undefined variable: " + name);
            }

            IScriptedObject result = Memory[name];

            if( t!=null && result.GetType() != t)
            {
                throw new ScriptError("Type mismatch: expected " 
                    + ScriptedObjects.FormatName(t.Name) + " but got " 
                    + ScriptedObjects.FormatName(Memory[name].GetType().Name) );
            }

            if (requireFinal && !result.IsFinal())
            {
                throw new ScriptError("Variable " + name + ", must be finalized.");
            }

            return result;
        }

        internal bool IsDefined(string name)
        {
            return this.memory.ContainsKey(name);
        }

        internal void SetVariable(string name, IScriptedObject obj)
        {
            this.memory[name] = obj;
        }
    }
}
