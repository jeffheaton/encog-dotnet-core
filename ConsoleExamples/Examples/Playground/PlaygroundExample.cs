using ConsoleExamples.Examples;
using Encog;
using Encog.App.Analyst;
using Encog.App.Analyst.CSV.Normalize;
using Encog.App.Analyst.Wizard;
using Encog.Util.CSV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Encog.Examples.Playground
{
    /// <summary>
    /// Not a real example.  Just used for experimentation.
    /// </summary>
    public class PlaygroundExample: IExample
    {
        static readonly FileInfo sourceCSV = new FileInfo("C:\\Users\\Jeff\\EncogProjects\\MyEncogProject\\large-test.csv");
        static readonly FileInfo targetCSV = new FileInfo("C:\\Users\\Jeff\\EncogProjects\\MyEncogProject\\large-norm.csv");
        static readonly FileInfo scriptEGA = new FileInfo("C:\\Users\\Jeff\\EncogProjects\\MyEncogProject\\large-test.ega");

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(PlaygroundExample),
                    "playground",
                    "Not an actual example.  Do not run.",
                    "Just a playground to use when you want to create code to run in the same workspace as Encog core. (mostly used by Jeff Heaton)");
                return info;
            }
        }

        #region IExample Members

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="app">Holds arguments and other info.</param>
        public void Execute(IExampleInterface app)
        {
            Console.WriteLine("Running wizard...");
            var analyst = new EncogAnalyst();

            var wizard = new AnalystWizard(analyst);
            wizard.TargetFieldName = "field:1";
            wizard.Wizard(sourceCSV,
                false, AnalystFileFormat.DecpntComma);
            

            // customer id
            analyst.Script.Normalize.NormalizedFields[0].Action = Encog.Util.Arrayutil.NormalizationAction.PassThrough;

            var norm = new AnalystNormalizeCSV();
            norm.Report = new ConsoleStatusReportable();
            Console.WriteLine("Analyze for normalize...");
            norm.Analyze(sourceCSV, false, CSVFormat.English, analyst);
            norm.ProduceOutputHeaders = true;
            Console.WriteLine("Normalize...");
            norm.Normalize(targetCSV);
            analyst.Save(scriptEGA);
        }

        #endregion
    }
}
