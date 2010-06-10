using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Encog.Bot.Browse;

namespace encog_test.Encog.Bot.Browse
{
    [TestFixture]
    public class TestAddress
    {
        [Test]
        public void testAddressFunctions()
        {
            String a = "http://www.httprecipes.com/";
            Address address = new Address(new Uri(a));
            address.ToString();
            Assert.AreEqual(address.Original, a);
            Assert.AreEqual(address.Url.Host, "www.httprecipes.com");
            Address address2 = new Address(new Uri(a), a);
            Assert.AreEqual(address2.Original, a);
            Address address3 = new Address(null, a);
            Assert.AreEqual(address3.Original, a);

        }
    }
}
