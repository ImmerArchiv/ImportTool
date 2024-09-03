using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Archiv10.Domain.Shared.Locator;
using Archiv10.Domain.Shared.BO;

namespace Archiv10.Domain.Tests.Services
{
    [TestClass]
    public class UnitTestNameService
    {
        [TestMethod]
        public void TestCreateBagitName()
        {
            var nameservice = DomainLocator.GetNameService();

            Assert.AreEqual("Root", nameservice.CreateBagitName(@"C:\Root", @"C:\Root", NameType.NameFromRelativePath));
            Assert.AreEqual("Root", nameservice.CreateBagitName(@"C:\Root", @"C:\Root", NameType.NameFromFolderName));



            Assert.AreEqual("Root/Blub", nameservice.CreateBagitName(@"C:\Root", @"C:\Root\Blub",NameType.NameFromRelativePath));
            Assert.AreEqual("Blub", nameservice.CreateBagitName(@"C:\Root", @"C:\Root\Blub", NameType.NameFromFolderName));



        }
    }
}
