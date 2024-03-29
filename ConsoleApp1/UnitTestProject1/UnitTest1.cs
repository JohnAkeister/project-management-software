using ConsoleApp1;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        private Validation validation = new Validation();
        [TestMethod()]
        public void ReadIntTest()
        {
            string enteredstring = "123";
            var intTest = new ConsoleApp1.Validation();
            Assert.AreEqual(intTest.ReadInt(enteredstring), int.Parse(enteredstring));
        }

        [TestMethod()]
        public void SetUpUserTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        [Timeout(2000)]
        public void ViewUsersProjectsTest_valid()
        {
            string notvalid = "Not Valid";
            var projecttest = new ConsoleApp1.User();
            try
            {
                projecttest.ViewUsersProjects(notvalid, validation);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }
    }
}
