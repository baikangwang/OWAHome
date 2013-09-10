using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orange.OWA.Gateway;

namespace Orange.OWA.Test
{
    [TestClass]
    public class InBoxGatewayTest
    {
        [TestMethod]
        public void GetEmailSimpleListTest()
        {
            string actual = InBoxGateway.GetEmailSimpleList(0, 24);
            Console.WriteLine(actual);
            Assert.IsTrue(!string.IsNullOrEmpty(actual));
        }

        [TestMethod]
        public void GetEmailFullListTest()
        {
            string actual = InBoxGateway.GetEmailFullList(0, 24);
            Console.WriteLine(actual);
            Assert.IsTrue(!string.IsNullOrEmpty(actual));
        }
    }
}
