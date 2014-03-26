//
// Encog(tm) Core v3.2 - .Net Version
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
