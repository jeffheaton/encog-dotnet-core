using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData.SQL;
using encog_test.Neural.Networks;
using NUnit.Framework;

namespace encog_test.Data.SQL
{
    [TestFixture]
    public class TestSQLDataSet
    {
        [Test]
        public void SQLDataSet()
        {
            int bits = IntPtr.Size * 8;

            // no JET db on 64-bit
            if (bits < 64)
            {
                SQLNeuralDataSet data = new SQLNeuralDataSet(
                        "SELECT in1,in2,ideal1 FROM xor ORDER BY id",
                        2, 1, "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=..\\..\\encog.mdb; User Id=admin; Password=");

                XOR.TestXORDataSet(data);
            }

        }
    }
}
