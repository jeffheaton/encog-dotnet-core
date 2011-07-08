using Encog.ML.Data.Basic;
using Encog.Util.Normalize.Input;
using Encog.Util.Normalize.Output;
using Encog.Util.Normalize.Target;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Util.Normalize
{
    [TestClass]
    public class TestNormDataSet
    {
        public static readonly double[][] ARRAY_2D = {
                                                         new[] {1.0, 2.0, 3.0, 4.0, 5.0},
                                                         new[] {6.0, 7.0, 8.0, 9.0, 0.0}
                                                     };


        private DataNormalization Create()
        {
            IInputField a, b;
            double[][] arrayOutput = EngineArray.AllocateDouble2D(2, 2);

            var dataset = new BasicMLDataSet(ARRAY_2D, null);

            var target = new NormalizationStorageArray2D(arrayOutput);

            var norm = new DataNormalization();
            norm.Report = new NullStatusReportable();
            norm.Storage = target;
            norm.AddInputField(a = new InputFieldMLDataSet(false, dataset, 0));
            norm.AddInputField(b = new InputFieldMLDataSet(false, dataset, 1));
            norm.AddOutputField(new OutputFieldRangeMapped(a, 0.1, 0.9));
            norm.AddOutputField(new OutputFieldRangeMapped(b, 0.1, 0.9));
            return norm;
        }

        private void Check(DataNormalization norm)
        {
            double[][] arrayOutput = ((NormalizationStorageArray2D) norm.Storage).GetArray();
            Assert.AreEqual(arrayOutput[0][0], 0.1, 0.1);
            Assert.AreEqual(arrayOutput[1][0], 0.9, 0.1);
            Assert.AreEqual(arrayOutput[0][1], 0.1, 0.1);
            Assert.AreEqual(arrayOutput[1][1], 0.9, 0.1);
        }

        [TestMethod]
        public void TestDataSet()
        {
            DataNormalization norm = Create();
            norm.Process();
            Check(norm);
        }

        [TestMethod]
        public void TestDataSetSerial()
        {
            DataNormalization norm = Create();
            norm = (DataNormalization) SerializeRoundTrip.RoundTrip(norm);
            norm.Process();
            Check(norm);
        }
    }
}