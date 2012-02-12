using Encog.Util.Normalize.Input;
using Encog.Util.Normalize.Output.ZAxis;
using Encog.Util.Normalize.Target;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Util.Normalize
{
    [TestClass]
    public class TestZAxis
    {
        private readonly double[][] SAMPLE1 = {new[] {-10.0, 5.0, 15.0}, new[] {-2.0, 1.0, 3.0}};

        public DataNormalization Create()
        {
            IInputField a;
            IInputField b;
            IInputField c;
            double[][] arrayOutput = EngineArray.AllocateDouble2D(2, 4);

            var target = new NormalizationStorageArray2D(arrayOutput);
            var group = new ZAxisGroup();
            var norm = new DataNormalization();
            norm.Report = new NullStatusReportable();
            norm.Storage = target;
            norm.AddInputField(a = new InputFieldArray2D(false, SAMPLE1, 0));
            norm.AddInputField(b = new InputFieldArray2D(false, SAMPLE1, 1));
            norm.AddInputField(c = new InputFieldArray2D(false, SAMPLE1, 2));
            norm.AddOutputField(new OutputFieldZAxis(group, a));
            norm.AddOutputField(new OutputFieldZAxis(group, b));
            norm.AddOutputField(new OutputFieldZAxis(group, c));
            norm.AddOutputField(new OutputFieldZAxisSynthetic(group));
            return norm;
        }

        private void Check(DataNormalization norm)
        {
            double[][] arrayOutput = ((NormalizationStorageArray2D) norm.Storage).GetArray();

            Assert.AreEqual(-5.0, arrayOutput[0][0]);
            Assert.AreEqual(2.5, arrayOutput[0][1]);
            Assert.AreEqual(7.5, arrayOutput[0][2]);
            Assert.AreEqual(0.0, arrayOutput[0][3]);
            Assert.AreEqual(-1.0, arrayOutput[1][0]);
            Assert.AreEqual(0.5, arrayOutput[1][1]);
            Assert.AreEqual(1.5, arrayOutput[1][2]);
            Assert.AreEqual(0.0, arrayOutput[1][3]);
        }

        [TestMethod]
        public void TestAbsolute()
        {
            DataNormalization norm = Create();
            norm.Process();
            Check(norm);
        }

        [TestMethod]
        public void TestAbsoluteSerial()
        {
            DataNormalization norm = Create();
            norm = (DataNormalization) SerializeRoundTrip.RoundTrip(norm);
            norm.Process();
            Check(norm);
        }
    }
}