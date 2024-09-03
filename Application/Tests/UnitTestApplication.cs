using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Archiv10.Application.Shared.Locator;
using Moq;
using Archiv10.Locator.Internal;
using Archiv10.Infrastructure.Shared;
using Archiv10.Domain.Shared;
using Archiv10.Domain.Shared.Services;
using Archiv10.Domain.Shared.BO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Archiv10.Application.Tests
{
    [TestClass]
    public class UnitTestApplication
    {
        private Mock<IFileService> _fileServiceMock;
        private Mock<IRepositoryConfig> _repositoryConfig;
        private Mock<IFilenameService> _filenameService;
        private Mock<INameService> _nameService;

        [TestMethod]
        public void TestAddRepository()
        {      

            _fileServiceMock.Setup(fs => fs.ReadConfigFile(It.Is<string>(cf => cf == "repositories.json"))).Returns<string>(null);
            _fileServiceMock.Setup(fs => fs.ReadConfigFile(It.Is<string>(cf => cf == "rootfolder.json"))).Returns<string>(null);
            _fileServiceMock.Setup(fs => fs.SaveConfigFile(It.Is<string>(cf => cf == "repositories.json"),It.Is<string>(data => data.Contains("testurl"))));
            var application = ApplicationLocator.GetApplication();
            application.AddRepository("testurl", "testname", "testtoken");

        }

        [TestMethod]
        public void TestAddRootFolder()
        {
            _fileServiceMock.Setup(fs => fs.ReadConfigFile(It.Is<string>(cf => cf == "repositories.json"))).Returns<string>(null);
            _fileServiceMock.Setup(fs => fs.ReadConfigFile(It.Is<string>(cf => cf == "rootfolder.json"))).Returns<string>(null);
            _fileServiceMock.Setup(fs => fs.SaveConfigFile(It.Is<string>(cf => cf == "rootfolder.json"), It.Is<string>(data => data.Contains("testpath"))));
            var application = ApplicationLocator.GetApplication();
            application.AddRootFolder("testpath",new string[] { "*" },NameType.NameFromRelativePath);
        }


        [TestMethod]
        public void TestCreateDescription()
        {


            var rootfolder = new[] { new { path = @"C:\testpath\test\", filter = new string[] { " * " }, naming = NameType.NameFromFolderName } };

            _fileServiceMock.Setup(fs => fs.ReadConfigFile(It.Is<string>(cf => cf == "repositories.json"))).Returns<string>(null);
            _fileServiceMock.Setup(fs => fs.ReadConfigFile(It.Is<string>(cf => cf == "rootfolder.json"))).Returns(JsonConvert.SerializeObject(rootfolder));
            _fileServiceMock.Setup(fs => fs.ReadConfigFile(It.Is<string>(cf => cf == "mapping.json"))).Returns<string>(null);

            _nameService.Setup(ns => ns.CreateBagitName(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NameType>())).Returns("testname");
            var folderPath = new FolderPath(@"C:\testpath\test\blub/blab\blim");
            var application = ApplicationLocator.GetApplication();
            application.ReadConfig();
            var description = application.CreateDescription(folderPath);
            Assert.AreEqual(@"testname", description);
        }


        [TestInitialize]
        public void TestInitialize()
        {
            _fileServiceMock = new Mock<IFileService>(MockBehavior.Strict);
            ContainerHolder.Register(_fileServiceMock.Object);
            _repositoryConfig = new Mock<IRepositoryConfig>(MockBehavior.Strict);
            ContainerHolder.Register(_repositoryConfig.Object);
            _filenameService = new Mock<IFilenameService>(MockBehavior.Strict);
            ContainerHolder.Register(_filenameService.Object);
            _nameService = new Mock<INameService>(MockBehavior.Strict);
            ContainerHolder.Register(_nameService.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ContainerHolder.Reset();
        }
    }
}
