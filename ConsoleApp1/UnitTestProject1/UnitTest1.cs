using ConsoleApp1;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {       

        [TestMethod()]
        public void ReadIntTest()
        {
            string enteredstring = "123";
            var intTest = new ConsoleApp1.Validation();
            Assert.AreEqual(intTest.ReadInt(enteredstring), int.Parse(enteredstring));
        }
    }
}
