using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Archiv10.Domain.Shared.Locator;
using Moq;
using Archiv10.Infrastructure.Shared;
using Archiv10.Locator.Internal;
using System.Text;
using System.IO;

namespace Archiv10.Domain.Tests.Services
{
    [TestClass]
    public class UnitTestCheckSumService
    {
        private Mock<IFileService> _fileServiceMock;

        [TestMethod]
        public void TestCalcForFileMD5()
        {
            _fileServiceMock.Setup(fs => fs.OpenRead(It.Is<string>(fn => fn == "testfile"))).Returns(new MemoryStream(Encoding.UTF8.GetBytes("Hello World")));
            var checksumservice = DomainLocator.GetCheckSumService("md5");
            var checksum = checksumservice.CalcForFile("testfile");

            Assert.AreEqual<string>("b10a8db164e0754105b7a99be72e3fe5", checksum);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _fileServiceMock = new Mock<IFileService>(MockBehavior.Strict);
            ContainerHolder.Register(_fileServiceMock.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ContainerHolder.Reset();
        }
    }
}
