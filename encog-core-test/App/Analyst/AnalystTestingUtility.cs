//
// Encog(tm) Core v3.2 - .Net Version (Unit Test)
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2013 Heaton Research, Inc.
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
using System;
using System.IO;
using Encog.App.Analyst.Report;
using Encog.App.Analyst.Script;
using Encog.App.Analyst.Script.Normalize;
using Encog.App.Analyst.Script.Prop;
using Encog.App.Analyst.Wizard;
using Encog.ML;
using Encog.ML.Data;
using Encog.Persist;
using Encog.Util;
using Encog.Util.File;
using Encog.Util.Simple;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace Encog.App.Analyst
{
    public class AnalystTestingUtility
    {
        public const int MaxIterations = 100000;
        public const int MaxCycles = 10;
        private readonly FileInfo _analystFile;
        private readonly FileInfo _rawFile;
        private readonly TempDir _tempDir = new TempDir();

        public AnalystTestingUtility(String theBaseDataFile)
        {
            _tempDir.ClearContents();
            BaseDataFile = theBaseDataFile;
            _rawFile = _tempDir.CreateFile("test.csv");
            FileUtil.CopyResource(theBaseDataFile, _rawFile);
            _analystFile = _tempDir.CreateFile("test.ega");
            EncogAnalyst = new EncogAnalyst();

            FileFormat = AnalystFileFormat.DecpntComma;
        }

        public String BaseDataFile { get; set; }

        public EncogAnalyst EncogAnalyst { get; set; }
        public AnalystFileFormat FileFormat { get; set; }


        public void Wizard(AnalystGoal goal, WizardMethodType methodType, bool headers)
        {
            EncogAnalyst.MaxIteration = MaxIterations;
            var wiz = new AnalystWizard(EncogAnalyst) {Goal = goal, MethodType = methodType, EvidenceSegements = 3};
            wiz.Wizard(_rawFile, headers, FileFormat);

            EncogAnalyst.Save(_analystFile);
            EncogAnalyst.Load(_analystFile);
        }

        public void Process(double maxError)
        {
            int cycles = 0;
            double e;

            EncogAnalyst.AddAnalystListener(new ConsoleAnalystListener());
            
            do
            {
                EncogAnalyst.ExecuteTask("task-full");
                e = CalculateError();
                cycles++;
				Debug.WriteLine(cycles + ": Error = " + e);
            } while (cycles <= MaxCycles && e > maxError);

            Assert.IsTrue(cycles <= MaxCycles, "Too many cycles to perform successful train.");
        }

        public void Report()
        {
            var report = new AnalystReport(EncogAnalyst);
            report.ProduceReport(_tempDir.CreateFile("report.html"));
        }

        public void ValidateDataField(
            int i,
            double max,
            double mean,
            double min,
            double sd,
            String name,
            bool isClass,
            bool isComplete,
            bool isInteger,
            bool isReal)
        {
            DataField df = EncogAnalyst.Script.Fields[i];
            Assert.AreEqual(max, df.Max, 0.001);
            Assert.AreEqual(mean, df.Mean, 0.001);
            Assert.AreEqual(min, df.Min, 0.001);
            Assert.AreEqual(sd, df.StandardDeviation, 0.001);
            Assert.AreEqual(name, df.Name);
            Assert.AreEqual(isClass, df.Class);
            Assert.AreEqual(isComplete, df.Complete);
            Assert.AreEqual(isInteger, df.Integer);
            Assert.AreEqual(isReal, df.Real);
        }

        public void DumpDataField(int i)
        {
            DataField df = EncogAnalyst.Script.Fields[i];
            Console.Write(Format.FormatDouble(df.Max, 6));
            Console.Write(@";");
            Console.Write(Format.FormatDouble(df.Mean, 6));
            Console.Write(@";");
            Console.Write(Format.FormatDouble(df.Min, 6));
            Console.Write(@";");
            Console.Write(Format.FormatDouble(df.StandardDeviation, 6));
            Console.Write(@";");
            Console.Write(df.Name);
            Console.Write(@";");
            Console.Write(df.Class ? '1' : '0');
            Console.Write(@";");
            Console.Write(df.Complete ? '1' : '0');
            Console.Write(@";");
            Console.Write(df.Integer ? '1' : '0');
            Console.Write(@";");
            Console.WriteLine(df.Real ? '1' : '0');
        }

        public void ValidateDataClass(int i, params String[] args)
        {
            DataField df = EncogAnalyst.Script.Fields[i];
            Assert.AreEqual(args.Length, df.ClassMembers.Count);
            for (int j = 0; j < df.ClassMembers.Count; j++)
            {
                Assert.AreEqual(args[j], df.ClassMembers[j].Name);
            }
        }

        /**
	 * Obtain the ML method.
	 * @return The method.
	 */

        public IMLMethod ObtainMethod()
        {
            ScriptProperties prop = EncogAnalyst.Script.Properties;
            String resourceID = prop.GetPropertyString(
                ScriptProperties.MlConfigMachineLearningFile);
            FileInfo resourceFile = EncogAnalyst.Script.ResolveFilename(resourceID);

            var method = (IMLMethod) EncogDirectoryPersistence
                                         .LoadObject(resourceFile);

            if (!(method is IMLMethod))
            {
                throw new AnalystError(
                    "The object to be trained must be an instance of MLMethod. "
                    + method.GetType().Name);
            }

            return method;
        }

        /**
	 * Obtain the training set.
	 * @return The training set.
	 */

        private IMLDataSet ObtainTrainingSet()
        {
            ScriptProperties prop = EncogAnalyst.Script.Properties;
            String trainingID = prop.GetPropertyString(
                ScriptProperties.MlConfigTrainingFile);

            FileInfo trainingFile = EncogAnalyst.Script.ResolveFilename(trainingID);

            IMLDataSet trainingSet = EncogUtility.LoadEGB2Memory(trainingFile);
            return trainingSet;
        }

        public void ValidateMethodError(double targetError)
        {
            double e = CalculateError();
            Assert.IsTrue(e < targetError);
        }

        public double CalculateError()
        {
            IMLMethod method = ObtainMethod();
            IMLDataSet data = ObtainTrainingSet();
            return ((IMLError) method).CalculateError(data);
        }

        public void DumpAnalystField(int i)
        {
            AnalystField af = EncogAnalyst.Script.Normalize.NormalizedFields[i];

            Console.Write(Format.FormatDouble(af.ActualHigh, 6));
            Console.Write(@";");
            Console.Write(Format.FormatDouble(af.ActualLow, 6));
            Console.Write(@";");
            Console.Write(Format.FormatDouble(af.NormalizedHigh, 6));
            Console.Write(@";");
            Console.Write(Format.FormatDouble(af.NormalizedLow, 6));
            Console.Write(@";");
            Console.Write(af.Name);
            Console.Write(@";");
            Console.Write(af.TimeSlice);
            Console.Write(@";");
            Console.WriteLine(af.Action.ToString());
        }

        public void ValidateAnalystField(int i, double high, double low, double normHigh, double normLow,
                                         String name, int timeSlice, String action)
        {
            AnalystField af = EncogAnalyst.Script.Normalize.NormalizedFields[i];
            Assert.AreEqual(high, af.ActualHigh, 0.001);
            Assert.AreEqual(low, af.ActualLow, 0.001);
            Assert.AreEqual(normHigh, af.NormalizedHigh, 0.001);
            Assert.AreEqual(normLow, af.NormalizedLow, 0.001);
            Assert.AreEqual(name, af.Name);
            Assert.AreEqual(timeSlice, af.TimeSlice);
            Assert.AreEqual(action, af.Action.ToString());
        }
    }
}
