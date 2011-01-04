using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Script;
using Encog.Engine;

namespace encog_script
{
    public class EncogScriptRunner : IStatusReportable
    {
        public void InteractiveMode()
        {
            Console.WriteLine("EncogScript running in interactive mode.");

            EncogQuantScript script = new EncogQuantScript(this);
            
            for (; ; )
            {
                String str = Console.ReadLine();
                if (str.Trim().ToLower().Equals("quit") || str.Trim().ToLower().Equals("exit"))
                {
                    return;
                }
                else
                {
                    script.ExecuteLine(str);
                }
            }
        }

        public void Execute(String[] args)
        {
            int index = 0;
            bool pause = false;
            bool success = false;

            // process any options

            while (index < args.Length && args[index][0] == '-')
            {
                String option = args[index].Substring(1).ToLower();
                if ("pause".Equals(option))
                    pause = true;
                index++;
            }

            if (index >= args.Length)
            {
                Console.WriteLine("Must specify the script file to execute.");
                if (pause)
                {
                    Pause();
                }
                return;
            }

            String filename = args[index++];

            // get any arguments
            String[] pargs = new String[args.Length - index];
            for (int i = 0; i < pargs.Length; i++)
            {
                pargs[i] = args[index + i];
            }

            EncogQuantScript script = new EncogQuantScript(this);
            script.ExecuteFile(filename);

            if (pause)
            {
                Pause();
            }
        }

        static void Main(string[] args)
        {
            EncogScriptRunner runner = new EncogScriptRunner();

            if (args.Length < 1)
            {
                runner.InteractiveMode();
            }
            else
            {
                runner.Execute(args);
            }
        }

        public void Report(int total, int current, string message)
        {
            if (total != current)
            {
                StringBuilder line = new StringBuilder();
                line.Append(current);
                line.Append("/");
                line.Append(total);
                line.Append(": ");
                line.Append(message);
                Console.WriteLine(line.ToString());
            }
            else
            {
                Console.WriteLine(message);
            }
        }

        public void Pause()
        {
            Console.Write("\n\nPress ENTER to continue.");
            Console.ReadLine();
        }
    }
}
