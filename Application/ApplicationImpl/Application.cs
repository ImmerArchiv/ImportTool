using Archiv10.Application.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archiv10.Domain.Shared.BO;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using Archiv10.Infrastructure.Shared.Locator;
using Newtonsoft.Json;
using Archiv10.Domain.Shared.Locator;
using Archiv10.Application.Shared.BO;
using Archiv10.Application.Shared.Locator;
using System.IO;
using Archiv10.Domain.Shared;
using log4net;
using Archiv10.Domain.Shared.Services;
using Archiv10.Application.Shared.Services;

namespace Archiv10.Application.Impl
{
    class Application : IApplication
    {
        private readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IRepositoryConfig _repositoryConfig;
        private readonly IFilenameService _filenameService;
        private readonly IBagitCacheService _bagitCacheService;
        private readonly Random random = new Random();
        public Application(IRepositoryConfig repositoryConfig, IFilenameService filenameService, IBagitCacheService bagitCacheService)
        {
            _repositoryConfig = repositoryConfig;
            _filenameService = filenameService;
            _bagitCacheService = bagitCacheService;
            
            RepositoryList = new ObservableCollection<UIRepository>();
            BagList = new ObservableCollection<UIBagSnippet>();
            DataList = new ObservableCollection<UIData>();

            RootFolderList = new ObservableCollection<UIRootFolder>();
            LocalFolderList = new ObservableCollection<UIFolder>();
            FileList = new ObservableCollection<UIFile>();
            RootFolderNodeList = new ObservableCollection<UINodeItem>();

        }

        public ObservableCollection<UIBagSnippet> BagList { get; set; }

        public ObservableCollection<UIData> DataList { get; set; }

        public ObservableCollection<UIRepository> RepositoryList { get; set; }

        public ObservableCollection<UIFile> FileList { get; set; }
       
        public ObservableCollection<UIFolder> LocalFolderList { get; set; }

        public ObservableCollection<UIRootFolder> RootFolderList { get; set; }

        public ObservableCollection<UINodeItem> RootFolderNodeList { get; set; }

        public void ReadConfig()
        {
            //Init from Config File
            log.DebugFormat("Init from config file {0}", "repositories.json");

            var fileService = InfrastructureLocator.GetFileService();
            var json1 = fileService.ReadConfigFile("repositories.json");
            if (json1 != null)
            {
                RepositoryList.Clear();
                var db = JArray.Parse(json1);
                foreach (var o in db)
                {
                    var repository = new Repository()
                    {
                        Url = o["url"].Value<string>(),
                        RepositoryName = o["name"].Value<string>(),
                        Token = o["token"].Value<string>()
                    };
                    //AddRepository to List

                    RepositoryList.Add(new UIRepository()
                    {
                        Active = true,
                        Source = repository,
                        State = RepositoryState.Init
                    });
                }
            }

            log.DebugFormat("Init from config file {0}", "rootfolder.json");

            var json2 = fileService.ReadConfigFile("rootfolder.json");
            if (json2 != null)
            {
                RootFolderList.Clear();
                var db = JArray.Parse(json2);
                foreach (var o in db)
                {
                    var rootfolder = new RootFolder()
                    {
                        Path = o["path"].Value<string>(),
                        Naming = (NameType)Enum.Parse(typeof(NameType), o["naming"].Value<string>()),
                        Filter = o["filter"].ToObject<string[]>()
                    };
                    //AddRootFolder to List

                    RootFolderList.Add(new UIRootFolder()
                    {
                        Active = true,
                        Source = rootfolder,
                        State = RootFolderState.Init,
                        Icon = CreateRootFolderState(RootFolderSyncStatus.StateInit)
                    });
                }
            }
        }

   
        public void CommitSyncFolderStatus(IList<RootFolderSyncStatus> syncstates)
        {
            try
            {
                var fileService = InfrastructureLocator.GetFileService();
                fileService.SaveConfigFile("syncfolderstate.json", JsonConvert.SerializeObject(syncstates, Formatting.Indented));
            }
            catch (Exception io)
            {
                log.Error(io);
            }
        }



        private string _lastSyncfolderstate = null;
        public bool UpdateSyncFolderStatus()
        {

            try
            {
                var fileService = InfrastructureLocator.GetFileService();
                var json = fileService.ReadConfigFile("syncfolderstate.json");

                if (json == null) return false; //Nicht forhanden

                //Alt-Neu-Vergleich
                if (json.Equals(_lastSyncfolderstate)) return false;
                _lastSyncfolderstate = json;

                var syncstates = JsonConvert.DeserializeObject<List<RootFolderSyncStatus>>(json);
                foreach (var state in syncstates)
                {
                    foreach (var rf in RootFolderList.Where(rf => rf.Source.Path == state.path))
                    {
                        if(state.state == RootFolderSyncStatus.StateSynchronized)
                        {
                            //validate local copy
                            var states = ReadRootFolderNodes(rf).Select(x => x.State).ToList();
                            if (states.Count > 0)
                            {
                                rf.Icon = GetIconForLocalFileState(states.OrderBy(x => x).First());
                                continue;
                            }
                        }
                        
                        rf.Icon = CreateRootFolderState(state.state);
                    }
                }
                return true;

            }
            catch (IOException e)
            {
                log.Error(e);
                return false;
            }
        }

        private UICanvasIcon CreateRootFolderState(string state)
        {

            var Icon = new UICanvasIcon();

            switch (state)
            {
                case RootFolderSyncStatus.StateInit:
                    Icon.Fill = UICanvasIcon.Disable;
                    Icon.Data = UICanvasIcon.HourGlassEmpty;
                    break;
                case RootFolderSyncStatus.StateUploading:
                    Icon.Fill = UICanvasIcon.Active;
                    Icon.Data = UICanvasIcon.BackUp;
                    break;
                case RootFolderSyncStatus.StateSynchronized:
                    Icon.Fill = UICanvasIcon.Ok;
                    Icon.Data = UICanvasIcon.CheckCircle;
                    break;
                case RootFolderSyncStatus.StateError:
                default:
                    Icon.Fill = UICanvasIcon.Error;
                    Icon.Data = UICanvasIcon.ErrorCircle;
                    break;
            }
            return Icon;
        }

        public void AddRepository(string url, string name, string token)
        {
            //ReadConfig
            var db = new JArray();
            var fileService = InfrastructureLocator.GetFileService();
            var json = fileService.ReadConfigFile("repositories.json");
            if(json != null)
              db = JArray.Parse(json);

            JObject repo = new JObject();
            repo["url"]   = url;
            repo["name"]  = name;
            repo["token"] = token;


            db.Add(repo);
            //Save ConfigFile
            fileService.SaveConfigFile("repositories.json",db.ToString(Formatting.Indented));
            //Init
            var repository = new Repository() { Url = url, RepositoryName = name, Token = token };
            //AddRepository to List
            RepositoryList.Add(new UIRepository()
            {
                Active = true,
                Source = repository,
                State = RepositoryState.Init
            });
        }


      
        public void AppendFile(int index, string filename, Func<object> partTransfered)
        {
            if (index == -1) return;

            FileInfo fileInfo = new FileInfo(filename);
            var tempName = _filenameService.GetTemporaryName(DateTime.Now,random.Next(),"bin");  
            var bagsnippet = BagList[index];
          
         
            foreach (var repository in bagsnippet.SourceRepositories)
            {
                var fileService = InfrastructureLocator.GetFileService();
                var repositoryService = DomainLocator.GetRepositoryService();
                //Upload File Parts
                for (long l = 0; l < fileInfo.Length; l += _repositoryConfig.DataSize)
                {
                    BagFilePart filepart = new BagFilePart();
                    filepart.TempName = tempName;
                    filepart.Data = fileService.ReadBytes(filename,l, _repositoryConfig.DataSize);
                    repositoryService.UploadFilePart(repository, filepart);
                    partTransfered();
                }

                var checkSumService = DomainLocator.GetCheckSumService(repository.Info.CheckSum);
                var originFileName = _filenameService.CleanName(fileInfo.Name); //Path.GetFileName(filename);
                var originCheckSum = checkSumService.CalcForFile(filename);
                BagFile file = new BagFile();
                file.FileName = _filenameService.SaltedFileName(originFileName, originCheckSum);
                file.TempName = tempName;
                file.CheckSum = originCheckSum;
                repositoryService.AppendFile(repository, bagsnippet.SourceId, file);
            }
        }

        public void CreateBag(BagId bagid,string description)
        {
            var repositoryService = DomainLocator.GetRepositoryService();
            
            var baginfo = new BagInfo();
            baginfo.Add(new KeyValuePair<string, string>("Description", description));

            foreach (var repository in RepositoryList)
            {
                if (!repository.Active) continue;
                if (repository.State != RepositoryState.Connected) continue;
                repositoryService.Create(repository.Source,bagid,baginfo);
            }

        }

        public void DownloadFile(UIData uiData, string folder, Func<long,object> transfered)
        {

            var repositoryService = DomainLocator.GetRepositoryService();

            var fileservice = InfrastructureLocator.GetFileService();

            var repository = uiData.SourceRepositories[0];

            var tempname = fileservice.GetNewFileName(folder, uiData.SourceData.Name);

            var total = 0L;
            repositoryService.DownloadFile(repository, uiData.SourceBag, uiData.SourceData.Name, (byte[] data,int length) => {
                    total += length;
                    fileservice.WriteBytes(tempname,data,0,length);
                    transfered(total);
                    return null;
            });
            

            //check
            var checkSumService = DomainLocator.GetCheckSumService(repository.Info.CheckSum);
            var checkSum = checkSumService.CalcForFile(tempname);

            if (checkSum != uiData.SourceData.CheckSum)
                throw new Exception("ChecksumTest failed");

        }

        public void ReadFiles(int index)
        {
            DataList.Clear();

            if (index == -1) return;

            var repositoryService = DomainLocator.GetRepositoryService();
            var bagsnippet = BagList[index];

            foreach (var repository in bagsnippet.SourceRepositories)
            {
                //read from Cache
                var fullbag = _bagitCacheService.Get(repository.Key, bagsnippet.SourceId);
                if (fullbag == null || !fullbag.Status.Equals(bagsnippet.Status[repository.Key]))
                {
                    fullbag = repositoryService.ListOne(repository, bagsnippet.SourceId);
                    //add to cache
                    _bagitCacheService.Add(repository.Key, bagsnippet.SourceId, fullbag);
                }


                foreach (var data in fullbag.Data)
                {
                    var uidata = DataList.SingleOrDefault(d => d.SourceData.Name == data.Name && d.SourceData.CheckSum == data.CheckSum);

                    if (uidata == null)
                    {
                        uidata = new UIData()
                        {
                            SourceBag = bagsnippet.SourceId,
                            SourceData = data
                        };

                        //Before adding to DataList TODO schoebner machen, kein code redundanz
                        uidata.State = CreateState(RepositoryList, uidata.SourceRepositories);
                        uidata.SearchString = CreateSearchString(uidata.SourceData.Name, uidata.State);

                        DataList.Add(uidata);
                    }

                    uidata.SourceRepositories.Add(repository);
                    uidata.State = CreateState(RepositoryList, uidata.SourceRepositories);
                    uidata.SearchString = CreateSearchString(uidata.SourceData.Name, uidata.State);
                }

            }
        }

        private string CreateSearchString(string name, UIState state)
        {
            return name + " " + state.Text;
        }

        public void ReadRepositories()
        {
            log.DebugFormat("ReadRepositories: Init and resolve bagits for connected repositories.");
            var repositoryService = DomainLocator.GetRepositoryService();
            BagList.Clear();
            //BagList lesen
            foreach (var repository in RepositoryList)
            {

                //init and check if available
                if (!repository.Active)
                {
                    log.DebugFormat("Repository {0}{1} is not active, ignore it",repository.Source.Url, repository.Source.RepositoryName);
                    continue;
                }
                if (repositoryService.Init(repository.Source))
                {
                    repository.State = RepositoryState.Connected;
                }
                else
                {
                    repository.State = RepositoryState.Disconnected;
                    log.WarnFormat("Repository {0}{1} is not connected, skip it", repository.Source.Url, repository.Source.RepositoryName);
                    continue;
                }

                try
                {

                    foreach (var snippet in repositoryService.ListAll(repository.Source))
                    {
                        var wrapper = ApplicationLocator.GetBagInfoWrapperService(snippet);

                        var bagSnippet = BagList.SingleOrDefault(s => s.SourceId.ToString() == snippet.Id.ToString());
                        if (bagSnippet == null)
                        {
                            bagSnippet = new UIBagSnippet()
                            {
                                SourceId = snippet.Id,
                                Description = wrapper.GetDescription()
                            };

                            //Before adding to BagList  TODO schoener machen, kein code redundanz
                            bagSnippet.State = CreateState(RepositoryList, bagSnippet.SourceRepositories);
                            bagSnippet.SearchString = CreateSearchString(bagSnippet.Description, bagSnippet.State);

                            BagList.Add(bagSnippet);
                        }
                        bagSnippet.Status[repository.Source.Key] = snippet.Status;
                        bagSnippet.SourceRepositories.Add(repository.Source);
                        bagSnippet.State = CreateState(RepositoryList, bagSnippet.SourceRepositories);
                        bagSnippet.SearchString = CreateSearchString(bagSnippet.Description, bagSnippet.State);

                    }
                }
                catch (Exception e)
                {
                    repository.State = RepositoryState.Disconnected;
                    log.ErrorFormat("Repository {0}{1} has exception: {2}", repository.Source.Url, repository.Source.RepositoryName,e.Message);
                    continue;
                }
            }
        }

     

        public void ReadStatus()
        {
            var repositoryService = DomainLocator.GetRepositoryService();
            foreach (var repository in RepositoryList)
            {

                //init and check if available
                if (!repository.Active)
                {
                    repository.Icon = CreateRepositoryStateIcon(RepositoryState.Init);
                    continue;
                }
                repository.Icon = CreateRepositoryStateIcon(repository.State);
                if (repository.State != RepositoryState.Connected) continue;
                //Status
                repositoryService.Status(repository.Source);
                repository.EndpointState = CreateEndpointState(repository.Source.Status);
                repository.Icon = CreateRepositoryStateIcon(repository.State);
            }
        }

        private UICanvasIcon CreateRepositoryStateIcon(RepositoryState state)
        {
            var Icon = new UICanvasIcon();

            switch (state)
            {
                case RepositoryState.Init:
                    Icon.Fill = UICanvasIcon.Disable;
                    Icon.Data = UICanvasIcon.HourGlassEmpty;
                    break;
                 case RepositoryState.Connected:
                    Icon.Fill = UICanvasIcon.Ok;
                    Icon.Data = UICanvasIcon.CheckCircle;
                    break;
                case RepositoryState.Disconnected:
                default:
                    Icon.Fill = UICanvasIcon.Error;
                    Icon.Data = UICanvasIcon.ErrorCircle;
                    break;
            }
            return Icon;
        }

        private string CreateEndpointState(RepositoryEndpointStatus status)
        {
            var usage = status.Size * 100 / status.MaxSize;

            string[] sizes = { "B", "KB", "MB", "GB" , "TB" };
            var size = status.MaxSize;
            int order = 0;
            while (size >= 1024 && ++order < sizes.Length)
            {
                size = size / 1024;
            }
            
            return String.Format("{0}% of {1} {2}", usage, size, sizes[order]);
        }

        private UIState CreateState(ObservableCollection<UIRepository> repositoryList, IList<Repository> sourceRepositories)
        {
            if(sourceRepositories.Count != repositoryList.Count)
            {
                return new UIState() {
                    Text = "nicht synchronisiert",
                    Syncronized = false,
                    Color = "Red"
                };
            }

           if(repositoryList.Count == 1)
           {
                return new UIState()
                {
                    Text = "Ok",
                    Syncronized = true,
                    Color = "Transparent"
                };
            }
            return new UIState()
            {
                Text = "Synchronisiert",
                Syncronized = true,
                Color = "Transparent"
            };
        }

        public void SyncBag(UIBagSnippet uiBagSnippet)
        {
            var repositoryService = DomainLocator.GetRepositoryService();

            foreach (var repository in RepositoryList)
            {
                if (!repository.Active) continue;
                if (repository.State != RepositoryState.Connected) continue;

                //Schon vorhanden?
                if (uiBagSnippet.SourceRepositories.SingleOrDefault(r => r.Url == repository.Source.Url && r.RepositoryName == repository.Source.RepositoryName) != null) continue;


                //Copy Bag from bagsnippet.SourceRepositories[0] to  repository.Source;
                var fullbag = repositoryService.ListOne(uiBagSnippet.SourceRepositories[0], uiBagSnippet.SourceId);
                repositoryService.Create(repository.Source, fullbag.Id, fullbag.Info);
            }


        }

        public int CountPartsOfFile(string filename)
        {
           FileInfo fileInfo = new FileInfo(filename);
           return (int)(fileInfo.Length / _repositoryConfig.DataSize) + 1;
        }

        public int CountRepositories(int index)
        {
            var bagsnippet = BagList[index];
            return bagsnippet.SourceRepositories.Count();
        }

        public int CountPartsOfFile(UIData uiData)
        {
            return (int)(uiData.SourceData.Length / _repositoryConfig.DataSize) + 1;
        }

        public int CountActiveRepositoriesToSync(UIData uiData)
        {
            var uiBag = BagList.Single(b => b.SourceId.ToString() == uiData.SourceBag.ToString());
            var cnt = 0;

            foreach (var repository in RepositoryList)
            {
                if (!repository.Active) continue;
                if (repository.State != RepositoryState.Connected) continue;

                //Bagit muss in Repository  
                var targetRepository = uiBag.SourceRepositories.SingleOrDefault(r => r.Url == repository.Source.Url && r.RepositoryName == repository.Source.RepositoryName);
                if (targetRepository == null) continue;

                //Data schon Vorhanden?
                if (uiData.SourceRepositories.SingleOrDefault(r => r.Url == repository.Source.Url && r.RepositoryName == repository.Source.RepositoryName) != null) continue;
                cnt++;
            }

            return cnt;
        }

        public void SyncData(UIData uiData, Func<object> partTransfered)
        {
            var repositoryService = DomainLocator.GetRepositoryService();

            var uiBag = BagList.Single(b => b.SourceId.ToString() == uiData.SourceBag.ToString());

            var tempName = String.Format("{0:yyyyMMdd_HHmmss}.bin", DateTime.Now);

            foreach (var repository in RepositoryList)
            {
                if (!repository.Active) continue;
                if (repository.State != RepositoryState.Connected) continue;

                //Bagit muss in Repository  
                var targetRepository = uiBag.SourceRepositories.SingleOrDefault(r => r.Url == repository.Source.Url && r.RepositoryName == repository.Source.RepositoryName);
                if (targetRepository == null) continue;

                //Data schon Vorhanden?
                if (uiData.SourceRepositories.SingleOrDefault(r => r.Url == repository.Source.Url && r.RepositoryName == repository.Source.RepositoryName) != null) continue;


                //Copy uiData.SourceData from uiData.SourceRepositories[0] to  targetRepository
                for (var l = 0; l < uiData.SourceData.Length; l += _repositoryConfig.DataSize)
                {
                    var filePart = repositoryService.GetFilePart(uiData.SourceRepositories[0], uiData.SourceBag, uiData.SourceData.Name, l, _repositoryConfig.DataSize);
                    filePart.TempName = tempName;
                    repositoryService.UploadFilePart(targetRepository, filePart);
                    partTransfered();
                }
                BagFile file = new BagFile();
                file.FileName = uiData.SourceData.Name;
                file.CheckSum = uiData.SourceData.CheckSum;
                file.TempName = tempName;
                repositoryService.AppendFile(targetRepository, uiData.SourceBag, file);
            }


        }

        public void AddRootFolder(string path, string[] filter, NameType naming)
        {
            //ReadConfig
            var db = new JArray();
            var fileService = InfrastructureLocator.GetFileService();
            var json = fileService.ReadConfigFile("rootfolder.json");
            if (json != null)
                db = JArray.Parse(json);

            JObject repo = new JObject();
            repo["path"] = path;
            repo["filter"] = new JArray(filter);
            repo["naming"] = naming.ToString();

            db.Add(repo);
            //Save ConfigFile
            fileService.SaveConfigFile("rootfolder.json", db.ToString(Formatting.Indented));
            //Init
            var rootfolder = new RootFolder() { Path = path , Naming = naming, Filter = filter };
            //AddRepository to List
            RootFolderList.Add(new UIRootFolder()
            {
                Active = true,
                Source = rootfolder,
                State = RootFolderState.Init,
                Icon = CreateRootFolderState(RootFolderSyncStatus.StateInit)
            });
        }

        public void UpdateRootFolder(string path, string[] filter, NameType naming)
        {
            //ReadConfig
            var db = new JArray();
            var fileService = InfrastructureLocator.GetFileService();
            var json = fileService.ReadConfigFile("rootfolder.json");
            if (json != null)
                db = JArray.Parse(json);


            var repo = db.Single(x => x["path"].Value<string>() == path);
            repo["path"] = path;
            repo["filter"] = new JArray(filter);
            repo["naming"] = naming.ToString();

            
            //Save ConfigFile
            fileService.SaveConfigFile("rootfolder.json", db.ToString(Formatting.Indented));
            //Init
            var rootfolder = new RootFolder() { Path = path, Naming = naming, Filter = filter };
            //AddRepository to List
            RootFolderList.Single(x => x.Source.Path == path).Source = rootfolder;
        }


        public void ReadRootFolders()
        {
            log.DebugFormat("ReadRootFolders: Read all root folder and all containing folders");

            var rootfolderService = DomainLocator.GetRootFolderService();
            LocalFolderList.Clear();
            //BagList lesen
            foreach (var rootfolder in RootFolderList)
            {

                //init and check if available
                if (!rootfolder.Active)
                {
                    log.DebugFormat("Rootfolder {0} is not active, ignore it",rootfolder.Source.Path);
                    continue;
                }
                if (rootfolderService.Init(rootfolder.Source))
                {
                    rootfolder.State = RootFolderState.Connected;
                }
                else
                {
                    rootfolder.State = RootFolderState.Disconnected;
                    log.WarnFormat("Rootfolder {0} is not connected, skip it", rootfolder.Source.Path);
                    continue;
                }

                foreach (var folder in rootfolderService.ListAll(rootfolder.Source))
                {
                    var uifolder = new UIFolder()
                    {
                        RootSourceFolder = rootfolder.Source,
                        SourceFolder = folder.Path
                    };
                    LocalFolderList.Add(uifolder);
                }
            }
        }

        public void ReadLocalFiles(int index)
        {

            FileList.Clear();
            if (index == -1) return;

            var rootfolderService = DomainLocator.GetRootFolderService();
            var folder = LocalFolderList[index];

            var files = rootfolderService.ListFiles(folder.SourceFolder,folder.RootSourceFolder.Filter);

            foreach (var file in files)
            {
                //var uidata = DataList.SingleOrDefault(d => d.SourceData.Name == data.Name && d.SourceData.CheckSum == data.CheckSum);
                var uifile = new UIFile()
                {
                    SourceFolder = folder.SourceFolder,
                    RootSourceFolder = folder.RootSourceFolder,
                    SourceFile = file,
                };
                FileList.Add(uifile);
            }


        }



        private IList<UINodeItem> ReadRootFolderNodes(UIRootFolder rootFolder)
        {
            var list = new List<UINodeItem>();
            var rootfolderService = DomainLocator.GetRootFolderService();
            var nameService = DomainLocator.GetNameService();
            var fileMappingValidatorService = DomainLocator.GetFileMappingValidatorService();

            var localFolder = rootfolderService.ListAll(rootFolder.Source);
            foreach (var folder in localFolder)
            {
                var node = new UINodeItem()
                {
                    Title = nameService.CreateBagitName(rootFolder.Source.Path, folder.Path.ToString(), rootFolder.Source.Naming),
                    Icon = new UICanvasIcon { Data = UICanvasIcon.CheckCircle, Fill = UICanvasIcon.Ok },
                    State = LocalFileState.Ok
                };
                list.Add(node);
                var files = rootfolderService.ListFiles(folder.Path, rootFolder.Source.Filter);
                var states = new List<LocalFileState>();
                foreach (var file in files)
                {
                    var state = fileMappingValidatorService.Validate(folder.Path, file.data);
                    states.Add(state);
                    node.Items.Add(new UINodeItem()
                    {
                        Title = file.data.Name,
                        State = state,
                        Icon = GetIconForLocalFileState(state)
                    });
                }

                if (states.Count > 0)
                {
                    var sumState = states.OrderBy(x => x).First();
                    node.Icon = GetIconForLocalFileState(sumState);
                    node.State = sumState;
                }
            }
            return list;
        }


        public void ReadRootFolderTree(int index)
        {
            RootFolderNodeList.Clear();
            if (index == -1) return;

          
            var rootFolder = RootFolderList[index];
            foreach(var node in ReadRootFolderNodes(rootFolder))
                RootFolderNodeList.Add(node);

        }

        private UICanvasIcon GetIconForLocalFileState(LocalFileState state)
        {

           switch(state)
            {
                default: //Alle Fehler
                    return new UICanvasIcon { Data = UICanvasIcon.ErrorCircle, Fill = UICanvasIcon.Error };
                case LocalFileState.MustUpload:
                    return new UICanvasIcon { Data = UICanvasIcon.HourGlassEmpty, Fill = UICanvasIcon.Active };
                case LocalFileState.Ok:
                    return new UICanvasIcon { Data = UICanvasIcon.CheckCircle, Fill = UICanvasIcon.Ok };
            }

        }

        public string CreateDescription(FolderPath path)
        {
            try {
                foreach (var rootfolder in RootFolderList)
                {
                    var rootpath = rootfolder.Source.Path;
                    if (path.ToString().StartsWith(rootpath))
                    {
                        //Zugeordneten Rootfolder gefunden
                        var nameService = DomainLocator.GetNameService();
                        return nameService.CreateBagitName(rootpath, path.ToString(), rootfolder.Source.Naming);
                    }
                }
            }
            catch(Exception e)
            {
                log.Error(e);
            }
            return string.Format(" - {0}", new DirectoryInfo(path.ToString()).Name);
        }


        public void ReadFolderBagitMapping()
        {
            /*
            FolderBagitMapping = new FolderBagitMapping();
            var fileService = InfrastructureLocator.GetFileService();

            var json = fileService.ReadConfigFile("mapping.json");
            if (json != null)
            {
                var db = JArray.Parse(json);
                foreach (var o in db)
                {
                    var path = new FolderPath(o["path"].Value<string>());
                    var bagid = new BagId(o["bagid"].Value<string>());

                    if (fileService.CheckDirectory(path.ToString()))
                    {
                        FolderBagitMapping.Register(path, bagid);
                    }
                }
            }
            */
        }

        public void UpdateFolderBagitMapping(FolderPath path, BagId bagid)
        {

           /*
                //Wurde neu erzeugt, also in mapping.json aufnehmen
                //ReadConfig
                var db = new JArray();
                var fileService = InfrastructureLocator.GetFileService();
                var json = fileService.ReadConfigFile("mapping.json");
                if (json != null)
                    db = JArray.Parse(json);

                JObject mapping = new JObject();
                mapping["path"] = path.ToString();
                mapping["bagid"] = bagid.ToString();

                db.Add(mapping);
                //Save ConfigFile
                fileService.SaveConfigFile("mapping.json", db.ToString(Formatting.Indented));
            */
        }

        public void WriteServiceState(bool working, string state, string Format, params object[] args)
        {
            var message = String.Format(Format, args);

            log.DebugFormat("Service [{0}] workCorrect={1} message={2}", state, working, message);
         
            //persitence state
            JObject obj = new JObject();
            obj["state"] = state;
            obj["working"] = working;
            obj["message"] = message;
            obj["date"] = DateTime.Now;

            //Save ConfigFile
            try
            {
                var fileService = InfrastructureLocator.GetFileService();
                fileService.SaveConfigFile("servicestate.json", obj.ToString(Formatting.Indented));
            }
            catch(IOException io)
            {
                log.Error(io);
            }
        }

        public ServiceStatus ReadServiceStatus()
        {
            try
            {
                var fileService = InfrastructureLocator.GetFileService();
                var json = fileService.ReadConfigFile("servicestate.json");
                if (json != null)
                {
                    var o = JObject.Parse(json);
                    return new ServiceStatus()
                    {
                        Working = o["working"].Value<bool>(),
                        State = o["state"].Value<string>(),
                        Message = o["message"].Value<string>(),
                        Date = o["date"].Value<DateTime>()
                    };
                }
            }
            catch(IOException e)
            {
                log.Error(e);
            }
            return null;
        }





        private Dictionary<string, FileSystemWatcher> filesystemWatcher = new Dictionary<string, FileSystemWatcher>();
        private DateTime filewatcherLastChanged = DateTime.MinValue;

        public void UpdateFileWatcher()
        {
            foreach (var rootfolder in RootFolderList)
            {
                if (filesystemWatcher.ContainsKey(rootfolder.Source.Path)) continue;

                var FSW = new FileSystemWatcher();

                // Pfad und Filter festlegen
                FSW.Path = rootfolder.Source.Path;
                FSW.Filter = "*";

                // Events definieren
                FSW.Changed += new FileSystemEventHandler(delegate { filewatcherLastChanged = DateTime.Now; });
                FSW.Created += new FileSystemEventHandler(delegate { filewatcherLastChanged = DateTime.Now; });
                FSW.Deleted += new FileSystemEventHandler(delegate { filewatcherLastChanged = DateTime.Now; });
                FSW.Renamed += new RenamedEventHandler(delegate { filewatcherLastChanged = DateTime.Now; });

                FSW.IncludeSubdirectories = true;
                // Filesystemwatcher aktivieren
                FSW.EnableRaisingEvents = true;

                filesystemWatcher.Add(rootfolder.Source.Path, FSW);
            }
        }

        public DateTime GetLastmodifiedForLocalFiles()
        {
            return filewatcherLastChanged + TimeSpan.FromSeconds(30);
        }

        public DateTime GetLastmodifiedForRepositories()
        {
            var max = DateTime.MinValue;

            foreach (var repository in RepositoryList)
            {
                if (!repository.Active) continue;
                if (repository.State != RepositoryState.Connected) continue;

                if (repository.Source.Status.LastModified > max)
                    max = repository.Source.Status.LastModified;
            }
            return max;
        }

        public DateTime GetLastmodifiedForConfiguration()
        {
            var fileService = InfrastructureLocator.GetFileService();
            return fileService.ConfigPathLastmodified(new string[]{ "repositories.json", "rootfolder.json" });
        }

       
    }
}
