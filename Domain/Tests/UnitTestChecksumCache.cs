using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Archiv10.Domain.Shared.Locator;
using Archiv10.Infrastructure.Shared;
using Moq;
using Archiv10.Locator.Internal;
using System.Collections.Generic;

namespace Archiv10.Domain.Tests
{
    [TestClass]
    public class UnitTestChecksumCache
    {
        private Mock<IFileService> _fileServiceMock;

    


        [TestMethod]
        public void TestPutAndGet()
        {
            var lastmodified = DateTime.Now;
            var lastmodified2 = DateTime.Now.AddMinutes(1);

            _fileServiceMock.Setup(fs => fs.ReadCache(It.IsAny<string>())).Returns<IDictionary<string, string>>(null);
            _fileServiceMock.Setup(fs => fs.WriteCache(It.IsAny<string>() ,It.Is<IDictionary<string, string>>(d => d.Count == 1)));


            var cache = DomainLocator.GetCheckSumCache();

            cache.Put("filepath", lastmodified, "cksum");
            Assert.AreEqual("cksum",cache.Get("filepath", lastmodified));
            Assert.IsNull(cache.Get("filepath", lastmodified2));
            Assert.IsNull(cache.Get("filepath2", lastmodified));


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
