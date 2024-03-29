using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var readinttest = new ConsoleApp1.Validation();
            Assert.AreEqual(readinttest.ReadInt("123"), 123);
        }
    }
}
