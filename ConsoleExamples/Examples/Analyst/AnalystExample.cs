using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Encog.App.Analyst;
using Encog.App.Analyst.Wizard;
using Encog.App.Analyst.Report;
using Encog.Util.File;
using ConsoleExamples.Examples;

namespace Encog.Examples.Analyst
{
    public class AnalystExample : IExample
    {
        public const String IRIS_SOURCE = "http://archive.ics.uci.edu/ml/machine-learning-databases/iris/iris.data";
        public const String FOREST_SOURCE = "http://archive.ics.uci.edu/ml/machine-learning-databases/covtype/covtype.data.gz";

        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(AnalystExample),
                    "analyst",
                    "Encog Analyst",
                    "This example shows how to use Encog analyst from code.");
                return info;
            }
        }

        public void IrisExample(FileInfo dir)
        {
            Console.WriteLine("Starting Iris dataset example.");
            Uri url = new Uri(IRIS_SOURCE);
            FileInfo analystFile = FileUtil.CombinePath(dir, "iris.ega");
            FileInfo rawFile = FileUtil.CombinePath(dir, "iris_raw.csv");

            EncogAnalyst encog = new EncogAnalyst();
            encog.AddAnalystListener(new ConsoleAnalystListener());
            AnalystWizard wiz = new AnalystWizard(encog);
            //wiz.TaskRandomize = false;
            wiz.Wizard(url, analystFile, rawFile, false, AnalystFileFormat.DECPNT_COMMA);
            encog.Save(analystFile);

            encog.ExecuteTask("task-full");

            

            AnalystReport report = new AnalystReport(encog);
            report.ProduceReport(FileUtil.CombinePath(dir, "report.html"));
        }

        public void ForestExample(FileInfo dir)
        {
            Console.WriteLine("Starting forest cover dataset example.");
            Uri url = new Uri(FOREST_SOURCE);
            FileInfo analystFile = FileUtil.CombinePath(dir, "forest.ega");
            FileInfo rawFile = FileUtil.CombinePath(dir, "forest_raw.csv");

            EncogAnalyst encog = new EncogAnalyst();
            encog.AddAnalystListener(new ConsoleAnalystListener());
            AnalystWizard wiz = new AnalystWizard(encog);
            wiz.TaskBalance = true;

            wiz.Wizard(url, analystFile, rawFile, false, AnalystFileFormat.DECPNT_COMMA);

            encog.ExecuteTask("task-full");

            encog.Save(analystFile);

            AnalystReport report = new AnalystReport(encog);
            report.ProduceReport(FileUtil.CombinePath(dir, "report.html"));
        }

        public void Execute(IExampleInterface app)
        {
            if (app.Args.Length != 2)
            {
                Console.WriteLine("Usage: AnalystExample [iris/forest] [data directory]");
                Console.WriteLine("Data directory can be any empty directory.  Raw files will be downloaded to here.");
                return;
            }
            String command = app.Args[0].Trim().ToLower();
            FileInfo dir = new FileInfo(app.Args[1].Trim());

            AnalystExample example = new AnalystExample();


            if (String.Compare(command, "forest", true) == 0)
            {
                example.ForestExample(dir);
            }
            else if (String.Compare(command, "iris", true) == 0)
            {
                example.IrisExample(dir);
            }
            else
            {
                Console.WriteLine("Unknown command: " + command);
            }

        }
    }
}
