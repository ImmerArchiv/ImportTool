using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Archiv10.Locator.Internal;
using Moq;
using Archiv10.Domain.Shared.Locator;
using Archiv10.Domain.Shared.BO;
using Archiv10.Infrastructure.Shared;
using Archiv10.Infrastructure.Shared.BO;
using System.Web.Script.Serialization;

namespace Archiv10.Domain.Tests.Services
{
    [TestClass]
    public class UnitTestRepositoryService
    {
        private Mock<IWebConnector> _webConnectorMock;

        [TestMethod]
        public void TestInit()
        {
            var repository = new Repository()
            {
                Url = "testurl"
            };
            var webresponse = new WebResponse()
            {
                Data = new JavaScriptSerializer().Serialize(new
                {
                    state = "ok",
                    message = "Responsemessage",
                    api_version = "47.11",
                    grant_type = "testgranttype",
                    check_sum = "testchecksum",
                    modules = new
                    {
                        status = "statusurl",
                        create = "createurl",
                        putfilepart = "putfileparturl",
                        appendfile = "appendfileurl",
                        listall = "listallurl",
                        listone = "listoneurl",
                        getfilepart = "getfileparturl",
                        downloadfile = "downloadfileurl"
                    }
                })
            };
            _webConnectorMock.Setup(wc => wc.Get(It.Is<WebRequest>(wr => wr.Url == "testurl/info.json"))).Returns(webresponse);

            var repositoryservice = DomainLocator.GetRepositoryService();

            var connect = repositoryservice.Init(repository);
            Assert.IsTrue(connect);
            Assert.AreEqual<string>("47.11",repository.Info.ApiVersion);
            Assert.AreEqual<string>("testgranttype", repository.Info.GrantType);
            Assert.AreEqual<string>("testchecksum", repository.Info.CheckSum);
            Assert.AreEqual<string>("statusurl", repository.Info.UrlStatus);
            Assert.AreEqual<string>("createurl", repository.Info.UrlCreate);
            Assert.AreEqual<string>("putfileparturl", repository.Info.UrlPutFilePart);
            Assert.AreEqual<string>("appendfileurl", repository.Info.UrlAppendFile);
            Assert.AreEqual<string>("listallurl", repository.Info.UrlListAll);
            Assert.AreEqual<string>("listoneurl", repository.Info.UrlListOne);
            Assert.AreEqual<string>("getfileparturl", repository.Info.UrlGetFilePart);
            Assert.AreEqual<string>("downloadfileurl", repository.Info.UrlDownloadFile);
            
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _webConnectorMock = new Mock<IWebConnector>(MockBehavior.Strict);
            ContainerHolder.Register(_webConnectorMock.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ContainerHolder.Reset();
        }
    }
}
