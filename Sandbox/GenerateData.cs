using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Normalize;
using Encog.Normalize.Input;
using Encog.Normalize.Output.Mapped;
using Encog.Normalize.Output.Nominal;
using Encog;
using Encog.Neural.Data.Buffer;
using Encog.Normalize.Target;
using Encog.Normalize.Segregate.Index;
using Encog.Normalize.Segregate;
using Encog.Normalize.Output;

namespace Sandbox
{
    public class GenerateData : IStatusReportable
    {
        public void BuildOutputMapped(DataNormalization norm, IInputField coverType)
        {
            OutputFieldEncode mapped1 = new OutputFieldEncode(coverType);
            mapped1.addRange(1, 1, 0.9);
            mapped1.CatchAll = 0.1;
            norm.AddOutputField(mapped1);
            OutputFieldEncode mapped2 = new OutputFieldEncode(coverType);
            mapped2.addRange(2, 2, 0.9);
            mapped2.CatchAll = 0.1;
            norm.AddOutputField(mapped2);
            OutputFieldEncode mapped3 = new OutputFieldEncode(coverType);
            mapped3.addRange(3, 3, 0.9);
            mapped3.CatchAll = 0.1;
            norm.AddOutputField(mapped3);
            OutputFieldEncode mapped4 = new OutputFieldEncode(coverType);
            mapped4.addRange(4, 4, 0.9);
            mapped4.CatchAll = 0.1;
            norm.AddOutputField(mapped4);
            OutputFieldEncode mapped5 = new OutputFieldEncode(coverType);
            mapped5.addRange(5, 5, 0.9);
            mapped5.CatchAll = 0.1;
            norm.AddOutputField(mapped5);
            OutputFieldEncode mapped6 = new OutputFieldEncode(coverType);
            mapped6.addRange(6, 6, 0.9);
            mapped6.CatchAll = 0.1;
            norm.AddOutputField(mapped6);
            OutputFieldEncode mapped7 = new OutputFieldEncode(coverType);
            mapped7.addRange(7, 7, 0.9);
            mapped7.CatchAll = 0.1;
            norm.AddOutputField(mapped7);
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
            norm.AddOutputField(outType);
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

        public DataNormalization GenerateTraining(String output, int area, int start, int stop, int sample)
        {
            Console.WriteLine("Generating training");
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

            BufferedNeuralDataSet buffer = new BufferedNeuralDataSet(output);

            DataNormalization norm = new DataNormalization();
            norm.Report = this;
            norm.Storage = new NormalizationStorageNeuralDataSet(buffer);
            //norm.setTarget(new NormalizationTargetCSV(Constant.TRAINING_FILE));
            norm.AddInputField(inputElevation = new InputFieldCSV(true, Constant.COVER_TYPE_FILE, 0));
            norm.AddInputField(inputAspect = new InputFieldCSV(true, Constant.COVER_TYPE_FILE, 1));
            norm.AddInputField(inputSlope = new InputFieldCSV(true, Constant.COVER_TYPE_FILE, 2));
            norm.AddInputField(hWater = new InputFieldCSV(true, Constant.COVER_TYPE_FILE, 3));
            norm.AddInputField(vWater = new InputFieldCSV(true, Constant.COVER_TYPE_FILE, 4));
            norm.AddInputField(roadway = new InputFieldCSV(true, Constant.COVER_TYPE_FILE, 5));
            norm.AddInputField(shade9 = new InputFieldCSV(true, Constant.COVER_TYPE_FILE, 6));
            norm.AddInputField(shade12 = new InputFieldCSV(true, Constant.COVER_TYPE_FILE, 7));
            norm.AddInputField(shade3 = new InputFieldCSV(true, Constant.COVER_TYPE_FILE, 8));
            norm.AddInputField(firepoint = new InputFieldCSV(true, Constant.COVER_TYPE_FILE, 9));

            for (int i = 0; i < 4; i++)
            {
                norm.AddInputField(wilderness[i] = new InputFieldCSV(false, Constant.COVER_TYPE_FILE, 10 + i));
            }

            for (int i = 0; i < 40; i++)
            {
                norm.AddInputField(soilType[i] = new InputFieldCSV(true, Constant.COVER_TYPE_FILE, 14 + i));
            }

            norm.AddInputField(coverType = new InputFieldCSV(false, Constant.COVER_TYPE_FILE, 54));

            // select the wilderness area
            /*		RangeSegregator segregator = new RangeSegregator(wilderness[area],false);
                    segregator.addRange(1, 1, true);
                    norm.addSegregator(segregator);*/

            // load only the part we actually want, i.e. training or eval
            IndexSampleSegregator segregator2 = new IndexSampleSegregator(start, stop, sample);
            norm.AddSegregator(segregator2);
            IntegerBalanceSegregator segregator3 = new IntegerBalanceSegregator(coverType, 3000);
            norm.AddSegregator(segregator3);

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

            //buildOutputMapped(norm,coverType);
            //buildOutputOneOf(norm,coverType);
            BuildOutputEquilateral(norm, coverType);

            int inputLayerSize = norm.GetNetworkInputLayerSize();
            int outputLayerSize = norm.GetNetworkOutputLayerSize();

            buffer.BeginLoad(inputLayerSize, outputLayerSize);

            norm.Process();
            buffer.EndLoad();
            Console.WriteLine(segregator3.DumpCounts());
            return norm;
        }

        public DataNormalization GenerateIdeal(String output, int area, int start, int stop, int sample)
        {
            Console.WriteLine("Generating training");
            IInputField[] inputField = new IInputField[55];

            DataNormalization norm = new DataNormalization();
            norm.Report = this;
            norm.Storage = new NormalizationStorageCSV(Constant.EVAL_FILE);
            for (int i = 0; i < 55; i++)
            {
                inputField[i] = new InputFieldCSV(true, Constant.COVER_TYPE_FILE, i);
                norm.AddInputField(inputField[i]);
                IOutputField outputField = new OutputFieldDirect(inputField[i]);
                norm.AddOutputField(outputField);
            }


            // select the wilderness area
            //RangeSegregator segregator = new RangeSegregator(inputField[10+area],false);
            //segregator.addRange(1, 1, true);
            //norm.addSegregator(segregator);

            // load only the part we actually want, i.e. training or eval
            IndexSampleSegregator segregator2 = new IndexSampleSegregator(start, stop, sample);
            norm.AddSegregator(segregator2);

            norm.Process();
            return norm;
        }


        public void Report(int total, int current, String message)
        {
            Console.WriteLine(current + "/" + total + " " + message);

        }
    }
}
