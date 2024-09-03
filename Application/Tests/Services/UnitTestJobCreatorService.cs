using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Archiv10.Application.Shared.Locator;
using System.Collections.Generic;
using Archiv10.Application.Shared.BO;
using Archiv10.Application.Shared;
using Archiv10.Locator.Internal;
using Moq;
using System.Collections.ObjectModel;
using Archiv10.Domain.Shared.BO;
using Archiv10.Application.Shared.Services;

namespace Archiv10.Application.Tests.Services
{
    [TestClass]
    public class UnitTestJobCreatorService
    {
        private Mock<IApplication> _application;
        private Mock<IFolderBagitMappingService> _folderbagitmappingservice;
        private Mock<IFileMappingService> _filemappingservice;

        [TestMethod]
        public void TestCreateEmptyList()
        {
            ObservableCollection<UIBagSnippet> baglist = new ObservableCollection<UIBagSnippet>();
            ObservableCollection<UIFolder> folderlist = new ObservableCollection<UIFolder>();

            _application.SetupGet(a => a.BagList).Returns(baglist);
            _application.SetupGet(a => a.LocalFolderList).Returns(folderlist);

            _folderbagitmappingservice.Setup(m => m.Commit());
            _filemappingservice.Setup(m => m.Commit());

            var service = ApplicationLocator.GetJobCreatorService();
            var jobs = new List<IJob>();
            service.Create(jobs,() => true);
            Assert.AreEqual(0, jobs.Count);

            _application.VerifyGet(a => a.BagList,Times.Once);
            _application.VerifyGet(a => a.LocalFolderList, Times.Once);

        }

        [TestMethod]
        public void TestCreateUploadBagitAndFile()
        {
            /*
              Noch nix hochgeladen
              Bagit erzeugen und File hochladen
            */
            ObservableCollection<UIBagSnippet> baglist = new ObservableCollection<UIBagSnippet>();
            ObservableCollection<UIFolder> folderlist = new ObservableCollection<UIFolder>();
            ObservableCollection<UIFile> filelist = new ObservableCollection<UIFile>();
            var folder1 = new FolderPath("folder1");

            folderlist.Add(new UIFolder() { SourceFolder = folder1 });
            filelist.Add(new UIFile() {
                SourceFile = new LocalFile() { data = new BagData() { CheckSum = "checksum1" , Name = "File1" }},
                SourceFolder = folder1
            });


            _application.SetupGet(a => a.BagList).Returns(baglist);
            _application.SetupGet(a => a.LocalFolderList).Returns(folderlist);

            _application.Setup(a => a.WriteServiceState(It.Is<bool>(b => b == true),It.IsAny<string>(),It.IsAny<string>(),It.IsAny<object[]>()));
            _application.Setup(a => a.ReadLocalFiles(It.Is<int>(b => b == 0)));
            _application.SetupGet(a => a.FileList).Returns(filelist);

            _folderbagitmappingservice.Setup(m => m.Commit());
            _folderbagitmappingservice.Setup(m => m.Get(It.Is<FolderPath>(p => p.ToString() == "folder1"))).Returns<BagId>(null);
            _folderbagitmappingservice.Setup(m => m.Incr(It.Is<FolderPath>(p => p.ToString() == "folder1"),
                It.Is<BagId>(b => b.ToString().Length > 10),
                It.Is<string>(k => k == "checksum1")));

            _filemappingservice.Setup(m => m.Commit());


            var service = ApplicationLocator.GetJobCreatorService();
            var jobs = new List<IJob>();
            service.Create(jobs, () => true);
            Assert.AreEqual(2, jobs.Count);

            Assert.IsNotNull(jobs[0] as CreateBagitJob);
            Assert.IsNotNull(jobs[1] as UploadFileJob);
     
        }

        [TestMethod]
        public void TestCreateUploadBagitAndFile2()
        {
            /*
              Noch nix hochgeladen
              Bagit erzeugen und File hochladen
            */
            ObservableCollection<UIBagSnippet> baglist = new ObservableCollection<UIBagSnippet>();
            ObservableCollection<UIFolder> folderlist = new ObservableCollection<UIFolder>();
            ObservableCollection<UIFile> filelist = null;

            var folder1 = new FolderPath("folder1");
            var folder2 = new FolderPath("folder2");

            folderlist.Add(new UIFolder() { SourceFolder = folder1 });
            folderlist.Add(new UIFolder() { SourceFolder = folder2 });



            _application.SetupGet(a => a.BagList).Returns(baglist);
            _application.SetupGet(a => a.LocalFolderList).Returns(folderlist);

            _application.Setup(a => a.WriteServiceState(It.Is<bool>(b => b == true), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object[]>()));
            _application.Setup(a => a.ReadLocalFiles(It.Is<int>(b => b == 0))).Callback(() => {
                 filelist = new ObservableCollection<UIFile>();
                filelist.Add(new UIFile()
                {
                    SourceFile = new LocalFile() { data = new BagData() { CheckSum = "checksum", Name = "File1" } },
                    SourceFolder = folder1
                });
            });

            _application.Setup(a => a.ReadLocalFiles(It.Is<int>(b => b == 1))).Callback(() => {
                filelist = new ObservableCollection<UIFile>();
                filelist.Add(new UIFile()
                {
                    SourceFile = new LocalFile() { data = new BagData() { CheckSum = "checksum", Name = "File2" } },
                    SourceFolder = folder2
                });
            });
            _application.SetupGet(a => a.FileList).Returns(() => filelist);

            _folderbagitmappingservice.Setup(m => m.Commit());
            _folderbagitmappingservice.Setup(m => m.Get(It.Is<FolderPath>(p => p.ToString() == "folder1"))).Returns<BagId>(null);
            _folderbagitmappingservice.Setup(m => m.Get(It.Is<FolderPath>(p => p.ToString() == "folder2"))).Returns<BagId>(null);

            _folderbagitmappingservice.Setup(m => m.Incr(It.Is<FolderPath>(p => p.ToString() == "folder1"),
                It.Is<BagId>(b => b.ToString().Length > 10),
                It.Is<string>(k => k == "checksum")));

            _folderbagitmappingservice.Setup(m => m.Incr(It.Is<FolderPath>(p => p.ToString() == "folder2"),
             It.Is<BagId>(b => b.ToString().Length > 10),
             It.Is<string>(k => k == "checksum")));

            _filemappingservice.Setup(m => m.Commit());


            var service = ApplicationLocator.GetJobCreatorService();
            var jobs = new List<IJob>();
            service.Create(jobs, () => true);
            Assert.AreEqual(2, jobs.Count);

            Assert.IsNotNull(jobs[0] as CreateBagitJob);
            Assert.IsNotNull(jobs[1] as UploadFileJob);

        }

        [TestMethod]
        public void TestCreateSynchronized()
        {
            /*
                 Alles schon hochgeladen
                 Nichts zu tun
            */
            ObservableCollection<UIBagSnippet> baglist = new ObservableCollection<UIBagSnippet>();
            ObservableCollection<UIData> datalist = new ObservableCollection<UIData>();
            var bagid1 = new BagId("bagid1");

            baglist.Add(new UIBagSnippet() { });
            datalist.Add(new UIData() {
                SourceData = new BagData() { CheckSum = "checksum1" , Name = "__File1" },  //datei kann anders heißen, nur checksum wird geprüft
                State = new UIState() { Syncronized = true },
                SourceBag = bagid1
            });

            ObservableCollection<UIFolder> folderlist = new ObservableCollection<UIFolder>();
            ObservableCollection<UIFile> filelist = new ObservableCollection<UIFile>();
            var folder1 = new FolderPath("folder1");

            folderlist.Add(new UIFolder() { SourceFolder = folder1 });
            filelist.Add(new UIFile()
            {
                SourceFile = new LocalFile() { data = new BagData() { CheckSum = "checksum1", Name = "File1" } },
                SourceFolder = folder1
            });




            _application.SetupGet(a => a.BagList).Returns(baglist);
            _application.SetupGet(a => a.LocalFolderList).Returns(folderlist);

            _application.Setup(a => a.WriteServiceState(It.Is<bool>(b => b == true), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object[]>()));

            _application.Setup(a => a.ReadFiles(It.Is<int>(b => b == 0)));
            _application.SetupGet(a => a.DataList).Returns(datalist);

            _application.Setup(a => a.ReadLocalFiles(It.Is<int>(b => b == 0)));
            _application.SetupGet(a => a.FileList).Returns(filelist);

            _folderbagitmappingservice.Setup(m => m.Commit());
            _folderbagitmappingservice.Setup(m => m.Get(It.Is<FolderPath>(p => p.ToString() == "folder1"))).Returns(bagid1);
            _folderbagitmappingservice.Setup(m => m.Incr(It.Is<FolderPath>(p => p.ToString() == "folder1"), 
                It.Is<BagId>(b => b.ToString() == bagid1.ToString()),
                It.Is<string>(k => k == "checksum1")));

            _filemappingservice.Setup(m => m.Commit());
            _filemappingservice.Setup(m => m.Add(It.Is<UIFile>(f => f.SourceFile.data.Name == "File1"),
              It.Is<UIData>(d => d.SourceData.Name == "__File1")));

            var service = ApplicationLocator.GetJobCreatorService();
            var jobs = new List<IJob>();
            service.Create(jobs, () => true);
            Assert.AreEqual(0, jobs.Count);

            _folderbagitmappingservice.Verify(m => m.Incr(It.IsAny<FolderPath>(), It.IsAny<BagId>(), It.IsAny<string>()), Times.Once);

        }


        [TestMethod]
        public void TestCreateAppend()
        {
            /*
                 Eine Weitere Datei hochzuladen
                 ein UploadJob
            */
            ObservableCollection<UIBagSnippet> baglist = new ObservableCollection<UIBagSnippet>();
            ObservableCollection<UIData> datalist = new ObservableCollection<UIData>();
            var bagid1 = new BagId("bagid1");

            baglist.Add(new UIBagSnippet() { });
            datalist.Add(new UIData()
            {
                SourceData = new BagData() { CheckSum = "checksum1", Name = "__File1" },  //datei kann anders heißen, nur checksum wird geprüft
                State = new UIState() { Syncronized = true },
                SourceBag = bagid1
            });

            ObservableCollection<UIFolder> folderlist = new ObservableCollection<UIFolder>();
            ObservableCollection<UIFile> filelist = new ObservableCollection<UIFile>();
            var folder1 = new FolderPath("folder1");

            folderlist.Add(new UIFolder() { SourceFolder = folder1 });
            filelist.Add(new UIFile()
            {
                SourceFile = new LocalFile() { data = new BagData() { CheckSum = "checksum1", Name = "File1" } },
                SourceFolder = folder1
            });
            filelist.Add(new UIFile()
            {
                SourceFile = new LocalFile() { data = new BagData() { CheckSum = "checksum2", Name = "File2" } },
                SourceFolder = folder1
            });



            _application.SetupGet(a => a.BagList).Returns(baglist);
            _application.SetupGet(a => a.LocalFolderList).Returns(folderlist);

            _application.Setup(a => a.WriteServiceState(It.Is<bool>(b => b == true), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object[]>()));

            _application.Setup(a => a.ReadFiles(It.Is<int>(b => b == 0)));
            _application.SetupGet(a => a.DataList).Returns(datalist);

            _application.Setup(a => a.ReadLocalFiles(It.Is<int>(b => b == 0)));
            _application.SetupGet(a => a.FileList).Returns(filelist);

            _folderbagitmappingservice.Setup(m => m.Commit());
            _folderbagitmappingservice.Setup(m => m.Get(It.Is<FolderPath>(p => p.ToString() == "folder1"))).Returns(bagid1);
            _folderbagitmappingservice.Setup(m => m.Incr(It.Is<FolderPath>(p => p.ToString() == "folder1"), 
                It.Is<BagId>(b => b.ToString() == bagid1.ToString()), 
                It.Is<string>(k => k == "checksum1")));
            _folderbagitmappingservice.Setup(m => m.Incr(It.Is<FolderPath>(p => p.ToString() == "folder1"),
               It.Is<BagId>(b => b.ToString() == bagid1.ToString()),
               It.Is<string>(k => k == "checksum2")));

            _filemappingservice.Setup(m => m.Commit());
            _filemappingservice.Setup(m => m.Add(It.Is<UIFile>(f => f.SourceFile.data.Name == "File1"),
                It.Is<UIData>(d => d.SourceData.Name == "__File1")));

            var service = ApplicationLocator.GetJobCreatorService();
            var jobs = new List<IJob>();
            service.Create(jobs, () => true);

            _folderbagitmappingservice.Verify(m => m.Incr(It.IsAny<FolderPath>(), It.IsAny<BagId>(), It.IsAny<string>()), Times.Exactly(2));

            Assert.AreEqual(1, jobs.Count);
            Assert.IsNotNull(jobs[0] as UploadFileJob);

        }

      


        [TestInitialize]
        public void TestInitialize()
        {
            _application = new Mock<IApplication>(MockBehavior.Strict);
            ContainerHolder.Register(_application.Object,true);

            _folderbagitmappingservice = new Mock<IFolderBagitMappingService>(MockBehavior.Strict);
            ContainerHolder.Register(_folderbagitmappingservice.Object, true);

            _filemappingservice = new Mock<IFileMappingService>(MockBehavior.Strict);
            ContainerHolder.Register(_filemappingservice.Object, true);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ContainerHolder.Reset();
        }
    }
}
