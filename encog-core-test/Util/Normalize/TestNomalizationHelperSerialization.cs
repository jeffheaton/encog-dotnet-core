using System;
using Encog.ML.Data.Versatile;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Util.Normalize
{
    [TestClass]
    public class TestNomalizationHelperSerialization
    {
        [TestMethod]
        public void TestNOrmalizationHelperSerialization()
        {
            try
            {
                var trainingData = new VersatileMLDataSet();
                var nh = trainingData.NormHelper;

                SerializeRoundTrip.RoundTrip(nh);
            }
            catch (Exception ex)
            {
                Assert.Fail("Error in TestNomalizationHelperSerialization: {0}", ex);
            }
        }
    }
}
