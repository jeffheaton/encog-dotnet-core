using System;
using System.IO;
using System.Text;
using ConsoleExamples.Examples;
using Encog.App.Analyst;
using Encog.App.Analyst.CSV.Normalize;
using Encog.App.Analyst.Script.Normalize;
using Encog.App.Analyst.Wizard;
using Encog.Util.CSV;

namespace Encog.Examples.Normalize
{
    public class NormalizeFile : IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(NormalizeFile),
                    "normalize-file",
                    "Normalize a file.",
                    "See how to use Encog Analyst to normalize a file");
                return info;
            }
        }

        #region IExample Members

        public void Execute(IExampleInterface app)
        {
            if (app.Args.Length != 2)
            {
                Console.WriteLine(@"Note: This example assumes that headers are present in the CSV files.");
                Console.WriteLine(@"NormalizeFile [input file] [target file]");
            }
            else
            {
                var sourceFile = new FileInfo(app.Args[0]);
                var targetFile = new FileInfo(app.Args[1]);

                var analyst = new EncogAnalyst();
                var wizard = new AnalystWizard(analyst);
                wizard.Wizard(sourceFile, true, AnalystFileFormat.DecpntComma);

                DumpFieldInfo(analyst);

                var norm = new AnalystNormalizeCSV();
                norm.Analyze(sourceFile, true, CSVFormat.English, analyst);
                norm.ProduceOutputHeaders = true;
                norm.Normalize(targetFile);
                EncogFramework.Instance.Shutdown();
            }
        }

        #endregion

        public static void DumpFieldInfo(EncogAnalyst analyst)
        {
            Console.WriteLine(@"Fields found in file:");
            foreach (AnalystField field in analyst.Script.Normalize.NormalizedFields)
            {
                var line = new StringBuilder();
                line.Append(field.Name);
                line.Append(",action=");
                line.Append(field.Action);
                line.Append(",min=");
                line.Append(field.ActualLow);
                line.Append(",max=");
                line.Append(field.ActualHigh);
                Console.WriteLine(line.ToString());
            }
        }
    }
}