// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Normalize.Input;
using Encog.Normalize;
using Encog.Normalize.Output.Nominal;
using Encog.Normalize.Target;
using Encog.Normalize.Output;
using Encog.Normalize.Segregate.Index;
using Encog.Normalize.Segregate;
using Encog.Engine;

namespace Encog.Examples.Forest
{
    public class GenerateData : IStatusReportable
    {
        private IExampleInterface app;

        public GenerateData(IExampleInterface app)
        {
            this.app = app;
        }

        public void BuildOutputOneOf(DataNormalization norm, IInputField coverType)
        {
            OutputOneOf outType = new OutputOneOf(0.9, 0.1);
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
            OutputEquilateral outType = new OutputEquilateral(0.9, 0.1);
            outType.AddItem(coverType, 1);
            outType.AddItem(coverType, 2);
            outType.AddItem(coverType, 3);
            outType.AddItem(coverType, 4);
            outType.AddItem(coverType, 5);
            outType.AddItem(coverType, 6);
            outType.AddItem(coverType, 7);
            norm.AddOutputField(outType, true);
        }

        public void Copy(String source, String target, int start, int stop, int size)
        {
            IInputField[] inputField = new IInputField[55];

            DataNormalization norm = new DataNormalization();
            norm.Report = this;
            norm.Storage = new NormalizationStorageCSV(target);
            for (int i = 0; i < 55; i++)
            {
                inputField[i] = new InputFieldCSV(true, source, i);
                norm.AddInputField(inputField[i]);
                IOutputField outputField = new OutputFieldDirect(inputField[i]);
                norm.AddOutputField(outputField);
            }

            // load only the part we actually want, i.e. training or eval
            IndexSampleSegregator segregator2 = new IndexSampleSegregator(start, stop, size);
            norm.AddSegregator(segregator2);

            norm.Process();
        }

        public void Narrow(String source, String target, int field, int count)
        {
            IInputField[] inputField = new IInputField[55];

            DataNormalization norm = new DataNormalization();
            norm.Report = this;
            norm.Storage = new NormalizationStorageCSV(target);
            for (int i = 0; i < 55; i++)
            {
                inputField[i] = new InputFieldCSV(true, source, i);
                norm.AddInputField(inputField[i]);
                IOutputField outputField = new OutputFieldDirect(inputField[i]);
                norm.AddOutputField(outputField);
            }

            IntegerBalanceSegregator segregator = new IntegerBalanceSegregator(inputField[field], count);
            norm.AddSegregator(segregator);

            norm.Process();
            app.WriteLine("Samples per tree type:");
            app.WriteLine(segregator.DumpCounts());
        }

        public void Step1()
        {
            app.WriteLine("Step 1: Generate training and evaluation files");
            app.WriteLine("Generate training file");
            Copy(Constant.COVER_TYPE_FILE, Constant.TRAINING_FILE, 0, 2, 4); // take 3/4
            app.WriteLine("Generate evaluation file");
            Copy(Constant.COVER_TYPE_FILE, Constant.EVALUATE_FILE, 3, 3, 4); // take 1/4
        }

        public void Step2()
        {
            app.WriteLine("Step 2: Balance training to have the same number of each tree");
            Narrow(Constant.TRAINING_FILE, Constant.BALANCE_FILE, 54, 3000);
        }

        public DataNormalization Step3(bool useOneOf)
        {
            app.WriteLine("Step 3: Normalize training data");
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
            IInputField[] wilderness = new IInputField[4];
            IInputField[] soilType = new IInputField[40];
            IInputField coverType;

            DataNormalization norm = new DataNormalization();
            norm.Report = this;
            norm.Storage = new NormalizationStorageCSV(Constant.NORMALIZED_FILE);
            norm.AddInputField(inputElevation = new InputFieldCSV(true, Constant.BALANCE_FILE, 0));
            norm.AddInputField(inputAspect = new InputFieldCSV(true, Constant.BALANCE_FILE, 1));
            norm.AddInputField(inputSlope = new InputFieldCSV(true, Constant.BALANCE_FILE, 2));
            norm.AddInputField(hWater = new InputFieldCSV(true, Constant.BALANCE_FILE, 3));
            norm.AddInputField(vWater = new InputFieldCSV(true, Constant.BALANCE_FILE, 4));
            norm.AddInputField(roadway = new InputFieldCSV(true, Constant.BALANCE_FILE, 5));
            norm.AddInputField(shade9 = new InputFieldCSV(true, Constant.BALANCE_FILE, 6));
            norm.AddInputField(shade12 = new InputFieldCSV(true, Constant.BALANCE_FILE, 7));
            norm.AddInputField(shade3 = new InputFieldCSV(true, Constant.BALANCE_FILE, 8));
            norm.AddInputField(firepoint = new InputFieldCSV(true, Constant.BALANCE_FILE, 9));

            for (int i = 0; i < 4; i++)
            {
                norm.AddInputField(wilderness[i] = new InputFieldCSV(true, Constant.BALANCE_FILE, 10 + i));
            }

            for (int i = 0; i < 40; i++)
            {
                norm.AddInputField(soilType[i] = new InputFieldCSV(true, Constant.BALANCE_FILE, 14 + i));
            }

            norm.AddInputField(coverType = new InputFieldCSV(false, Constant.BALANCE_FILE, 54));

            norm.AddOutputField(new OutputFieldRangeMapped(inputElevation, 0.1, 0.9));
            norm.AddOutputField(new OutputFieldRangeMapped(inputAspect, 0.1, 0.9));
            norm.AddOutputField(new OutputFieldRangeMapped(inputSlope, 0.1, 0.9));
            norm.AddOutputField(new OutputFieldRangeMapped(hWater, 0.1, 0.9));
            norm.AddOutputField(new OutputFieldRangeMapped(vWater, 0.1, 0.9));
            norm.AddOutputField(new OutputFieldRangeMapped(roadway, 0.1, 0.9));
            norm.AddOutputField(new OutputFieldRangeMapped(shade9, 0.1, 0.9));
            norm.AddOutputField(new OutputFieldRangeMapped(shade12, 0.1, 0.9));
            norm.AddOutputField(new OutputFieldRangeMapped(shade3, 0.1, 0.9));
            norm.AddOutputField(new OutputFieldRangeMapped(firepoint, 0.1, 0.9));

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

        public void Report(int total, int current, String message)
        {
            app.WriteLine(current + "/" + total + " " + message);

        }
    }
}
