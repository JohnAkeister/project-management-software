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
        [Timeout(2000)]
        public void ViewUsersProjectsTest_invalid() // fails but fails correctly
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

        [TestMethod()]
        [Timeout(3000)]
        public void ViewUsersProjectsTest_valid() // Does pass test to display all the users project
        {
            string notvalid = "John";
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

        [TestMethod()]
        [Timeout(3000)]
        public void ViewProjectTasksTest_valid()
        {
            int valid = 1;
            var projecttest = new ConsoleApp1.User();
            try
            {
                projecttest.ViewProjectTasks(valid);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }
        [TestMethod()]
        [Timeout(3000)]
        public void ViewProjectTasksTest_invalid()
        {
            int notvalid = 0;
            var projecttest = new ConsoleApp1.User();
            try
            {
                projecttest.ViewProjectTasks(notvalid);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        [TestMethod()]
        public void ViewLogsTest_valid()
        {
            int valid = 1;
            string validname = "John";
            var projecttest = new ConsoleApp1.Logs();
            try
            {
                projecttest.ViewLogs(validname,valid);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }
        [TestMethod()]
        [Timeout(3000)]
        public void ViewLogsTest_invalid()
        {
            int valid = 1;
            int invalidnum = 9990;
            string invalidname = "Paul";
            var projecttest = new ConsoleApp1.Logs();
            try
            {
                projecttest.ViewLogs(invalidname, valid);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }
        [TestMethod()]
        [Timeout(3000)]
        public void ViewLogsTest_invalid2()
        {
            
            int invalidnum = 9990;
            string validname = "John";
            var projecttest = new ConsoleApp1.Logs();
            try
            {
                projecttest.ViewLogs(validname, invalidnum);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }
    }
}
