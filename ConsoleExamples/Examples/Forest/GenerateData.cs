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
using Encog.Util.Normalize;
using Encog.Util.Normalize.Input;
using Encog.Util.Normalize.Output;
using Encog.Util.Normalize.Output.Nominal;
using Encog.Util.Normalize.Segregate;
using Encog.Util.Normalize.Segregate.Index;
using Encog.Util.Normalize.Target;

namespace Encog.Examples.Forest
{
    public class GenerateData : IStatusReportable
    {
        private readonly ForestConfig _config;

        public GenerateData(ForestConfig config)
        {
            _config = config;
        }

        #region IStatusReportable Members

        public void Report(int total, int current, String message)
        {
            Console.WriteLine(current + @"/" + total + @" " + message);
        }

        #endregion

        public void BuildOutputOneOf(DataNormalization norm, IInputField coverType)
        {
            var outType = new OutputOneOf();
            outType.AddItem(coverType, 1);
            outType.AddItem(coverType, 2);
            outType.AddItem(coverType, 3);
            outType.AddItem(coverType, 4);
            outType.AddItem(coverType, 5);
            outType.AddItem(coverType, 6);
            outType.AddItem(coverType, 7);
            norm.AddOutputField(outType, true);
        }

        public void BuildOutputEquilateral(DataNormalization norm, IInputField coverType)
        {
            var outType = new OutputEquilateral();
            outType.AddItem(coverType, 1);
            outType.AddItem(coverType, 2);
            outType.AddItem(coverType, 3);
            outType.AddItem(coverType, 4);
            outType.AddItem(coverType, 5);
            outType.AddItem(coverType, 6);
            outType.AddItem(coverType, 7);
            norm.AddOutputField(outType, true);
        }

        public void Copy(FileInfo source, FileInfo target, int start, int stop, int size)
        {
            var inputField = new IInputField[55];

            var norm = new DataNormalization {Report = this, Storage = new NormalizationStorageCSV(target.ToString())};
            for (int i = 0; i < 55; i++)
            {
                inputField[i] = new InputFieldCSV(true, source.ToString(), i);
                norm.AddInputField(inputField[i]);
                IOutputField outputField = new OutputFieldDirect(inputField[i]);
                norm.AddOutputField(outputField);
            }

            // load only the part we actually want, i.e. training or eval
            var segregator2 = new IndexSampleSegregator(start, stop, size);
            norm.AddSegregator(segregator2);

            norm.Process();
        }

        public void Narrow(FileInfo source, FileInfo target, int field, int count)
        {
            var inputField = new IInputField[55];

            var norm = new DataNormalization {Report = this, Storage = new NormalizationStorageCSV(target.ToString())};
            for (int i = 0; i < 55; i++)
            {
                inputField[i] = new InputFieldCSV(true, source.ToString(), i);
                norm.AddInputField(inputField[i]);
                IOutputField outputField = new OutputFieldDirect(inputField[i]);
                norm.AddOutputField(outputField);
            }

            var segregator = new IntegerBalanceSegregator(inputField[field], count);
            norm.AddSegregator(segregator);

            norm.Process();
            Console.WriteLine(@"Samples per tree type:");
            Console.WriteLine(segregator.DumpCounts());
        }

        public void Step1()
        {
            Console.WriteLine(@"Step 1: Generate training and evaluation files");
            Console.WriteLine(@"Generate training file");
            Copy(_config.CoverTypeFile, _config.TrainingFile, 0, 2, 4); // take 3/4
            Console.WriteLine(@"Generate evaluation file");
            Copy(_config.CoverTypeFile, _config.EvaluateFile, 3, 3, 4); // take 1/4
        }

        public void Step2()
        {
            Console.WriteLine(@"Step 2: Balance training to have the same number of each tree");
            Narrow(_config.TrainingFile, _config.BalanceFile, 54, 3000);
        }

        public DataNormalization Step3(bool useOneOf)
        {
            Console.WriteLine(@"Step 3: Normalize training data");
            IInputField inputElevation;
            IInputField inputAspect;
            IInputField inputSlope;
            IInputField hWater;
            IInputField vWater;
            IInputField roadway;
            IInputField shade9;
            IInputField shade12;
            IInputField shade3;
            IInputField firepoint;
            var wilderness = new IInputField[4];
            var soilType = new IInputField[40];
            IInputField coverType;

            var norm = new DataNormalization
                           {
                               Report = this,
                               Storage = new NormalizationStorageCSV(_config.NormalizedDataFile.ToString())
                           };
            norm.AddInputField(inputElevation = new InputFieldCSV(true, _config.BalanceFile.ToString(), 0));
            norm.AddInputField(inputAspect = new InputFieldCSV(true, _config.BalanceFile.ToString(), 1));
            norm.AddInputField(inputSlope = new InputFieldCSV(true, _config.BalanceFile.ToString(), 2));
            norm.AddInputField(hWater = new InputFieldCSV(true, _config.BalanceFile.ToString(), 3));
            norm.AddInputField(vWater = new InputFieldCSV(true, _config.BalanceFile.ToString(), 4));
            norm.AddInputField(roadway = new InputFieldCSV(true, _config.BalanceFile.ToString(), 5));
            norm.AddInputField(shade9 = new InputFieldCSV(true, _config.BalanceFile.ToString(), 6));
            norm.AddInputField(shade12 = new InputFieldCSV(true, _config.BalanceFile.ToString(), 7));
            norm.AddInputField(shade3 = new InputFieldCSV(true, _config.BalanceFile.ToString(), 8));
            norm.AddInputField(firepoint = new InputFieldCSV(true, _config.BalanceFile.ToString(), 9));

            for (int i = 0; i < 4; i++)
            {
                norm.AddInputField(wilderness[i] = new InputFieldCSV(true, _config.BalanceFile.ToString(), 10 + i));
            }

            for (int i = 0; i < 40; i++)
            {
                norm.AddInputField(soilType[i] = new InputFieldCSV(true, _config.BalanceFile.ToString(), 14 + i));
            }

            norm.AddInputField(coverType = new InputFieldCSV(false, _config.BalanceFile.ToString(), 54));

            norm.AddOutputField(new OutputFieldRangeMapped(inputElevation));
            norm.AddOutputField(new OutputFieldRangeMapped(inputAspect));
            norm.AddOutputField(new OutputFieldRangeMapped(inputSlope));
            norm.AddOutputField(new OutputFieldRangeMapped(hWater));
            norm.AddOutputField(new OutputFieldRangeMapped(vWater));
            norm.AddOutputField(new OutputFieldRangeMapped(roadway));
            norm.AddOutputField(new OutputFieldRangeMapped(shade9));
            norm.AddOutputField(new OutputFieldRangeMapped(shade12));
            norm.AddOutputField(new OutputFieldRangeMapped(shade3));
            norm.AddOutputField(new OutputFieldRangeMapped(firepoint));

            for (int i = 0; i < 40; i++)
            {
                norm.AddOutputField(new OutputFieldDirect(soilType[i]));
            }

            if (useOneOf)
                BuildOutputOneOf(norm, coverType);
            else
                BuildOutputEquilateral(norm, coverType);

            norm.Process();
            return norm;
        }
    }
}
