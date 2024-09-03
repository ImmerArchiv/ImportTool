using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Archiv10.Application.Shared.Locator;
using Archiv10.Application.Shared.BO;
using Archiv10.Infrastructure.Shared;
using Moq;
using Archiv10.Locator.Internal;
using Archiv10.Domain.Shared.BO;

namespace Archiv10.Application.Tests.Services
{
    [TestClass]
    public class UnitTestIFileMappingService
    {
        private Mock<IFileService> _fileservice;

        [TestMethod]
        public void TestAdd()
        {
            var service = ApplicationLocator.GetFileMappingService();

            var file = new UIFile()
            {
                SourceFolder = new FolderPath("folder1"),
                SourceFile = new LocalFile() { data = new BagData() { Name = "File1" , CheckSum = "CheckSum1"} }
            };

            var data = new UIData()
            {
                SourceBag = new BagId("bagit"),
                SourceData = new BagData() { Name = "__File1" },
                State = new UIState() {  Syncronized = true }
            };
            service.Add(file, data);
            service.Add(file, data);

        }

        [TestMethod]
        public void TestCommit()
        {
            _fileservice.Setup(f => f.WriteCache(It.IsAny<string>(), It.IsAny<object>()));
            var service = ApplicationLocator.GetFileMappingService();
            service.Commit();
            _fileservice.Verify(f => f.WriteCache(It.IsAny<string>(), It.IsAny<object>()), Times.Once);

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
