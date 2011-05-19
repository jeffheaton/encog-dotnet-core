using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.Util.File;

namespace Encog.Util
{
    [TestClass]
    public class TestResourceLoader
    {
        [TestMethod]
        public void TestResourceLoad()
        {
            string data = ResourceLoader.LoadString("Encog.Resources.hello.txt");
            Assert.AreEqual(0,data.IndexOf("Hello World!"));
        }
    }
}
