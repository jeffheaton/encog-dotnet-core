using System;
using System.Diagnostics;
using System.IO;
using Encog;
using Encog.App.Analyst;
using Encog.App.Analyst.CSV;
using Encog.App.Analyst.Missing;
using Encog.App.Analyst.Util;
using Encog.App.Analyst.Wizard;
using Encog.ML;
using Encog.ML.Data.Buffer;
using Encog.ML.Data.Buffer.CODEC;
using Encog.ML.Factory;
using Encog.ML.Train;
using Encog.Persist;
using Encog.Util;
using Encog.Util.CSV;
using Encog.Util.File;
using Encog.Util.Simple;

namespace EncogCmd
{
    public class EncogCmd
    {
        private static Stopwatch _sw;
        private readonly ParseCommand _cmd;

        public EncogCmd(ParseCommand cmd)
        {
            _cmd = cmd;
        }

        private static IHandleMissingValues TranslateMissing(String m)
        {
            if (m.Equals("DiscardMissing"))
            {
                return new DiscardMissing();
            }
            if (m.Equals("MeanAndModeMissing"))
            {
                return new MeanAndModeMissing();
            }
            if (m.Equals("NegateMissing"))
            {
                return new NegateMissing();
            }
            return new DiscardMissing();
        }

        private void WizardCommand()
        {
            String targetCSVFile = _cmd.Args[0];

            String egaFile = FileUtil.ForceExtension(targetCSVFile, "ega");

            var analyst = new EncogAnalyst();
            var wizard = new AnalystWizard(analyst);
            bool headers = _cmd.PromptBoolean("headers", true);

            AnalystFileFormat format =
                ConvertStringConst.String2AnalystFileFormat(_cmd.PromptString("format", "decpnt|comma"));

            String goal = _cmd.PromptString("goal", "c").ToLower().Trim();
            if (goal.Equals("c"))
            {
                wizard.Goal = AnalystGoal.Classification;
            }
            else if (goal.Equals("r"))
            {
                wizard.Goal = AnalystGoal.Regression;
            }
            else
            {
                Console.WriteLine(@"Invalid goal: " + goal);
                return;
            }

            wizard.TargetField = _cmd.PromptString("targetField", "");

            String m = _cmd.PromptString("method", "ff").ToLower();

            if (m.Equals("ff"))
            {
                wizard.MethodType = WizardMethodType.FeedForward;
            }
            else if (m.Equals("neat"))
            {
                wizard.MethodType = WizardMethodType.NEAT;
            }
            else if (m.Equals("pnn"))
            {
                wizard.MethodType = WizardMethodType.PNN;
            }
            else if (m.Equals("rbf"))
            {
                wizard.MethodType = WizardMethodType.RBF;
            }
            else if (m.Equals("som"))
            {
                wizard.MethodType = WizardMethodType.SOM;
            }
            else if (m.Equals("svm"))
            {
                wizard.MethodType = WizardMethodType.SVM;
            }
            else
            {
                Console.WriteLine(@"Invalid method: " + m);
                return;
            }

            String r = _cmd.PromptString("range", "-1t1").Trim().ToLower();
            if (r.Equals("-1t1"))
            {
                wizard.Range = NormalizeRange.NegOne2One;
            }
            else if (r.Equals("0t1"))
            {
                wizard.Range = NormalizeRange.Zero2One;
            }

            wizard.Missing = TranslateMissing(_cmd.PromptString("missing", "DiscardMissing"));

            wizard.LagWindowSize = _cmd.PromptInteger("lagWindow", 0);
            wizard.LeadWindowSize = _cmd.PromptInteger("leadWindow", 0);
            wizard.IncludeTargetField = _cmd.PromptBoolean("includeTarget", false);
            wizard.TaskNormalize = _cmd.PromptBoolean("normalize", true);
            wizard.TaskRandomize = _cmd.PromptBoolean("randomize", true);
            wizard.TaskSegregate = _cmd.PromptBoolean("segregate", true);
            wizard.TaskBalance = _cmd.PromptBoolean("balance", false);
            wizard.TaskCluster = _cmd.PromptBoolean("cluster", false);

            _sw.Start();
            Console.WriteLine(@"Analyzing data");
            wizard.Wizard(new FileInfo(targetCSVFile), headers, format);
            Console.WriteLine(@"Saving analyst file");
            analyst.Save(egaFile);
        }

        private void AnalystCommand()
        {
            String egaFile;
            String task;

            if (_cmd.Args.Count == 0)
            {
                Console.WriteLine(@"Must specify the EGA file to run");
                return;
            }

            if (_cmd.Args.Count == 1)
            {
                egaFile = _cmd.Args[0];
                task = "task-full";
            }
            else
            {
                egaFile = _cmd.Args[0];
                task = _cmd.Args[1];
            }

            _sw.Start();
            var analyst = new EncogAnalyst();
            analyst.AddAnalystListener(new ConsoleAnalystListener());
            analyst.Load(new FileInfo(egaFile));
            analyst.ExecuteTask(task);
        }

        private void ConvertCSV2EGB()
        {
            if (_cmd.Args.Count != 2)
            {
                Console.WriteLine(@"Must specify a source and target.");
                return;
            }

            String sourceFile = _cmd.Args[0];
            String targetFile = _cmd.Args[1];
            bool headers = _cmd.PromptBoolean("headers", true);
            int inputCount = _cmd.PromptInteger("inputCount", 0);
            int outputCount = _cmd.PromptInteger("outputCount", 0);

            if (inputCount == 0)
            {
                Console.WriteLine(@"Must specify an input count.");
                return;
            }

            AnalystFileFormat format1 =
                ConvertStringConst.String2AnalystFileFormat(_cmd.PromptString("format", "decpnt|comma"));
            CSVFormat format = ConvertStringConst.ConvertToCSVFormat(format1);

            new FileInfo(targetFile).Delete();
            IDataSetCODEC codec = new CSVDataCODEC(sourceFile, format, headers,
                                                   inputCount, outputCount, false);
            var loader = new BinaryDataLoader(codec) {Status = new ConsoleStatusReportable()};
            _sw.Start();
            loader.External2Binary(targetFile);
        }

        private void ConvertEGB2CSV()
        {
            if (_cmd.Args.Count != 2)
            {
                Console.WriteLine(@"Must specify a source and target.");
                return;
            }

            String sourceFile = _cmd.Args[0];
            String targetFile = _cmd.Args[1];

            AnalystFileFormat format1 =
                ConvertStringConst.String2AnalystFileFormat(_cmd.PromptString("format", "decpnt|comma"));
            CSVFormat format = ConvertStringConst.ConvertToCSVFormat(format1);

            new FileInfo(targetFile).Delete();
            IDataSetCODEC codec = new CSVDataCODEC(targetFile, format, false);
            var loader = new BinaryDataLoader(codec) {Status = new ConsoleStatusReportable()};
            _sw.Start();
            loader.Binary2External(sourceFile);
        }

        private void TrainCommand()
        {
            String methodFile = _cmd.Args[0];
            String trainingFile = _cmd.Args[1];

            String type = _cmd.PromptString("type", "rprop");
            String args = _cmd.PromptString("args", "");
            double maxError = _cmd.PromptDouble("maxError", 0.01);

            var dataSet = new BufferedMLDataSet(trainingFile);
            var method = (IMLMethod) EncogDirectoryPersistence.LoadObject(new FileInfo(methodFile));
            var factory = new MLTrainFactory();
            IMLTrain train = factory.Create(method, dataSet, type, args);
            _sw.Start();
            EncogUtility.TrainToError(train, maxError);
            Console.WriteLine(@"Saving machine learning method");
            EncogDirectoryPersistence.SaveObject(new FileInfo(methodFile), method);
        }

        private void CreateCommand()
        {
            if (_cmd.Args.Count != 3)
            {
                Console.WriteLine(@"Must specify method filename, type and args.");
                return;
            }

            String methodFile = _cmd.Args[0];
            String type = _cmd.Args[1];
            String args = _cmd.Args[2];

            _sw.Start();
            var factory = new MLMethodFactory();
            IMLMethod method = factory.Create(type, args, 0, 0);
            EncogDirectoryPersistence.SaveObject(new FileInfo(methodFile), method);
        }

        private void EvaluateCommand()
        {
            String methodFile = _cmd.Args[0];
            String trainingFile = _cmd.Args[1];
            String outputFile = _cmd.Args[2];
            var method = (IMLRegression) EncogDirectoryPersistence.LoadObject(new FileInfo(methodFile));
            bool headers = _cmd.PromptBoolean("headers", true);
            AnalystFileFormat format1 =
                ConvertStringConst.String2AnalystFileFormat(_cmd.PromptString("format", "decpnt|comma"));
            CSVFormat format = ConvertStringConst.ConvertToCSVFormat(format1);

            var e = new EvaluateRawCSV {Report = new ConsoleStatusReportable()};
            e.Analyze(method, new FileInfo(trainingFile), headers, format);
            e.Process(new FileInfo(outputFile), method);
            Console.WriteLine(@"Done evaluating file.");
        }


        private static void Main(string[] args)
        {
            try
            {
                var cmd = new ParseCommand(args);
                var p = new EncogCmd(cmd);
                _sw = new Stopwatch();

                Console.Write(@"Encog " + EncogFramework.Version);
                Console.Write((IntPtr.Size == 4) ? "(32-bit)" : "(64-bit)");

                Console.WriteLine(@" Command Line Utility");
                Console.WriteLine(@"Copyright 2011 by Heaton Research, Inc. Released under the Apache License");
                Console.WriteLine();


                if (cmd.Command == null)
                {
                    Console.WriteLine(@"Usage:");
                }
                else
                {
                    Console.WriteLine(@"Executing command: " + cmd.Command);
                    if (cmd.Command.Equals("wizard"))
                    {
                        p.WizardCommand();
                    }
                    else if (cmd.Command.Equals("analyst"))
                    {
                        p.AnalystCommand();
                    }
                    else if (cmd.Command.Equals("csv2egb"))
                    {
                        p.ConvertCSV2EGB();
                    }
                    else if (cmd.Command.Equals("egb2csv"))
                    {
                        p.ConvertEGB2CSV();
                    }
                    else if (cmd.Command.Equals("train"))
                    {
                        p.TrainCommand();
                    }
                    else if (cmd.Command.Equals("create"))
                    {
                        p.CreateCommand();
                    }
                    else if (cmd.Command.Equals("evaluate"))
                    {
                        p.EvaluateCommand();
                    }
                }

                _sw.Stop();
            }
            /*catch (Exception ex)
            {
                Console.WriteLine(@"* * Error * *");
                Console.WriteLine(ex.ToString());
            }*/
            finally
            {
                Console.Write(@"Done.  Runtime was " + Format.FormatTimeSpan((int) _sw.ElapsedMilliseconds/1000));
                Console.WriteLine(@" (" + _sw.ElapsedMilliseconds + @"ms).");
            }
        }
    }
}