using ConsoleExamples.Examples;
using Encog.Util.CSV;
using Encog.Util.Normalize;
using Encog.Util.Normalize.Input;
using Encog.Util.Normalize.Output;
using Encog.Util.Normalize.Output.Nominal;
using Encog.Util.Normalize.Target;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Examples.Iris
{
    public class IrisPredict : IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(IrisPredict),
                    "iris",
                    "Classify fisher's iris dataset.",
                    "This example shows how to classify the classic iris data set.");
                return info;
            }
        }

        public void Execute(IExampleInterface app)
        {
            string inputFile = "C:\\jth\\iris.csv";
            DataNormalization normalize = new DataNormalization();
            IInputField a, b, c, d;
            normalize.AddInputField(a = new InputFieldCSV(true, inputFile, "sepal_l"));
            normalize.AddInputField(b = new InputFieldCSV(true, inputFile, "sepal_w"));
            normalize.AddInputField(c = new InputFieldCSV(true, inputFile, "petal_l"));
            normalize.AddInputField(d = new InputFieldCSV(true, inputFile, "petal_w"));
            normalize.AddInputField(new InputFieldCSV(false, inputFile, "species"));
            normalize.AddOutputField(new OutputFieldRangeMapped(a));
            normalize.AddOutputField(new OutputFieldRangeMapped(b));
            normalize.AddOutputField(new OutputFieldRangeMapped(c));
            normalize.AddOutputField(new OutputFieldRangeMapped(d));
            //normalize.AddOutputField(new OutputOneOf(1,0));
            NormalizationStorageMLDataSet store = new NormalizationStorageMLDataSet(4, 0);
            normalize.Storage = store;
            normalize.Report = new ConsoleStatusReportable();

            normalize.Process(true);
            Console.WriteLine(store.DataSet.Count);
            
        }
    }
}
