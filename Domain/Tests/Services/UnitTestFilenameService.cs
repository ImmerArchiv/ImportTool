using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Archiv10.Domain.Shared.Locator;

namespace Archiv10.Domain.Tests.Services
{
    [TestClass]
    public class UnitTestFilenameService
    {
        [TestMethod]
        public void TestCleanName()
        {
            var filenameservice = DomainLocator.GetFileNameService();
            var clean = filenameservice.CleanName("äüö _-");
            Assert.AreEqual<string>("_____-", clean);
        }


        [TestMethod]
        public void TestSaltedFileName()
        {
            var filenameservice = DomainLocator.GetFileNameService();
            var name = filenameservice.SaltedFileName("hello.tar.GZ","123456789");
            Assert.AreEqual("hello_1234.tar.GZ", name);
        }


        [TestMethod]
        public void TestSaltedFileNameNoName()
        {
            var filenameservice = DomainLocator.GetFileNameService();
            var name = filenameservice.SaltedFileName(".tar.GZ", "123456789");
            Assert.AreEqual("1234.tar.GZ", name);
        }

        [TestMethod]
        public void TestSaltedFileNameNoExtension()
        {
            var filenameservice = DomainLocator.GetFileNameService();
            var name = filenameservice.SaltedFileName("hello", "123456789");
            Assert.AreEqual("hello_1234", name);
        }




        [TestMethod]
        public void TestGetTemporaryName()
        {
            var filenameservice = DomainLocator.GetFileNameService();
            var temp = filenameservice.GetTemporaryName(new DateTime(2017,12,10), 999 , "ext");
            Assert.AreEqual<string>("20171210_000000.ext", temp);
        }

        
    }
}
