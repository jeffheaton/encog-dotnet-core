using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Engine;
using System.Diagnostics;
using Encog.Engine.Util;
using System.Text.RegularExpressions;

namespace Encog.App.Quant.Script
{
    public class EncogQuantScript
    {
        IStatusReportable report;
        String[] lines;

        public EncogQuantScript(IStatusReportable report)
        {
            this.report = report;
        }

        private void WriteLine(String str)
        {
            report.Report(0, 0, str);
        }

        private void WriteLine()
        {
            WriteLine("");
        }

        public void run(String script)
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
                Execute(this.lines[i]);
            }
        }

        private void Execute(String line)
        {
            WriteLine("Line:" + line);
        }
    }
}
