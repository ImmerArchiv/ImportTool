using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Archiv10.Application.Shared.Locator;
using Archiv10.Domain.Shared.BO;
using Archiv10.Infrastructure.Shared;
using Moq;
using Archiv10.Locator.Internal;

namespace Archiv10.Application.Tests.Services
{
    [TestClass]
    public class UnitTestFolderBagitMappingService
    {
        private Mock<IFileService> _fileservice;

        [TestMethod]
        public void TestIncrAndGet()
        {

            var service = ApplicationLocator.GetFolderBagitMappingService();

            var folder1 = new FolderPath("folder1");
            var folder2 = new FolderPath("folder2");
            var bagid1 = new BagId("bagid1");

            service.Incr(folder1, bagid1, "key1");

            Assert.AreEqual(bagid1.ToString(), service.Get(folder1).ToString());
            Assert.IsNull(service.Get(folder2));

        }


        [TestMethod]
        public void TestIncrAndGet2()
        {

            var service = ApplicationLocator.GetFolderBagitMappingService();

            var folder1 = new FolderPath("folder1");
            var bagid1 = new BagId("bagid1");
            var bagid2 = new BagId("bagid2");

            service.Incr(folder1, bagid1, "key1");
            service.Incr(folder1, bagid1, "key2");
            service.Incr(folder1, bagid1, "key3");
            service.Incr(folder1, bagid2, "keyx");
            service.Incr(folder1, bagid2, "keyx");
            service.Incr(folder1, bagid2, "keyx");
            service.Incr(folder1, bagid2, "keyx");

            Assert.AreEqual(bagid1.ToString(), service.Get(folder1).ToString());

        }

        [TestMethod]
        public void TestCommit()
        {
            _fileservice.Setup(f => f.WriteCache(It.IsAny<string>(), It.IsAny<object>()));
            var service = ApplicationLocator.GetFolderBagitMappingService();
            service.Commit();
            _fileservice.Verify(f => f.WriteCache(It.IsAny<string>(), It.IsAny<object>()),Times.Once);
        }


        [TestInitialize]
        public void TestInitialize()
        {
            _fileservice = new Mock<IFileService>(MockBehavior.Strict);
            ContainerHolder.Register(_fileservice.Object, true);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ContainerHolder.Reset();
        }
    }
}
