using Archiv10.Application.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archiv10.Application.Shared.BO;
using Archiv10.Application.Shared;
using log4net;
using Archiv10.Domain.Shared.BO;

namespace Archiv10.Application.Impl.Services
{
    class JobCreatorService : IJobCreatorService
    {

        private readonly IApplication _application;
        private readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IFolderBagitMappingService _folderBagitMappingService;
        private readonly IFileMappingService _fileMappingService;

        public JobCreatorService(IApplication application,IFolderBagitMappingService folderBagitMappingService, IFileMappingService fileMappingService)
        {
            _application = application;
            _folderBagitMappingService = folderBagitMappingService;
            _fileMappingService = fileMappingService;
        }

        public void Create(IList<IJob> jobs, Func<bool> ShouldRun)
        {

            IDictionary<String, FileMapping> fileMappings = new Dictionary<String, FileMapping>();

            //read BagIts
            log.DebugFormat("read files in BagIts");
            var uiBagitList = _application.BagList;
            for (var index = 0; index < uiBagitList.Count; index++)
            {
                if (!ShouldRun()) return; //Aussprung, Service wird gestoppt
                _application.WriteServiceState(true, "CHECK", "ReadFiles for bagit {0}/{1}", index + 1, uiBagitList.Count);
                _application.ReadFiles(index);

                foreach (var data in _application.DataList)
                {
                    if (!ShouldRun()) return; //Aussprung, Service wird gestoppt
                    if (!fileMappings.ContainsKey(data.SourceData.CheckSum))
                        fileMappings[data.SourceData.CheckSum] = new FileMapping();

                    //add to statecache
                    var fileMapping = fileMappings[data.SourceData.CheckSum];
                    //Add Orgin
                    fileMapping.BagItOrigins.Add(new BagItOrigin() { Data = data });
                }

            }

            //read folders 
            log.DebugFormat("read files in Folders");

            var uiFolderList = _application.LocalFolderList;
            for (var index = 0; index < uiFolderList.Count; index++)
            {
                if (!ShouldRun()) return; //Aussprung, Service wird gestoppt

                _application.WriteServiceState(true, "CHECK", "ReadLocalFiles for folder {0}/{1}", index + 1, uiFolderList.Count);
                _application.ReadLocalFiles(index);
                foreach (var file in _application.FileList)
                {
                    if (!ShouldRun()) return; //Aussprung, Service wird gestoppt

                    //add to statecache
                    if (!fileMappings.ContainsKey(file.SourceFile.data.CheckSum))
                        fileMappings[file.SourceFile.data.CheckSum] = new FileMapping();

                    var fileMapping = fileMappings[file.SourceFile.data.CheckSum];

                    //Add orgin
                    fileMapping.LocalFileOrigins.Add(new LocalFileOrigin() { File = file });

                }
            }

            //existierende Bagit's  ermitteln
            log.Debug("lookup for existing mapping");
            /*
               Die Vorghandene Mappings zaehlen
            */
            foreach (KeyValuePair<String, FileMapping> entry in fileMappings)
            {
                FileMapping ds = entry.Value;
                string chksum = entry.Key;
                foreach (var bagitOrigin in ds.BagItOrigins)
                    foreach (var fileorigin in ds.LocalFileOrigins)
                    {
                        _folderBagitMappingService.Incr(fileorigin.File.SourceFolder, bagitOrigin.Data.SourceBag, chksum);
                    }
            }


            //Dump - Mapping Persistieren für OverlayIcon ( Filename + CheckSum ) => (bagit + Dateiname )
            foreach (KeyValuePair<String, FileMapping> entry in fileMappings)
            {
                FileMapping ds = entry.Value;
                string chksum = entry.Key;

                if (ds.LocalFileOrigins.Count == 0)
                {
                    //File existiert nur online, Alles OK
                    foreach (var bagitOrigin in ds.BagItOrigins)
                    {
                        log.DebugFormat("File only exists online name={0}, bagid={1}", 
                            bagitOrigin.Data.SourceData.Name, bagitOrigin.Data.SourceBag);
                    }
                    continue;
                }

                //ds.LocalFileOrigins.Count > 0, Datei existiert online
            
                if (ds.BagItOrigins.Count >= 1)
                {
                    //Alles OK - Eventuell Mehrfach vorhanden
                    foreach (var fileorigin in ds.LocalFileOrigins)
                        _fileMappingService.Add(fileorigin.File, ds.BagItOrigins[0].Data);
                    continue;
                }

                if (ds.BagItOrigins.Count == 0)
                {
                    //Noch kein UpLoad
                    var file = ds.LocalFileOrigins[0].File;
                    var bagid = _folderBagitMappingService.Get(file.SourceFolder);
                    log.InfoFormat("Must Upload name={0}, folder={1} bagid={2}", file.SourceFile.data.Name, file.SourceFolder,bagid);
                    for(int i = 1;i < ds.LocalFileOrigins.Count;i++)
                    {
                        var file1 = ds.LocalFileOrigins[i].File;
                        log.InfoFormat("Implicit Upload name={0}, folder={1} bagid={2}", file1.SourceFile.data.Name, file1.SourceFolder, bagid);
                    }
                    continue;
                }

                //sonst Error 
                log.ErrorFormat("Unknown Mapping: CheckSum={0} LocalFileOrgins={1} BagItOrigins={2}", entry.Key, ds.LocalFileOrigins.Count, ds.BagItOrigins.Count);

                if (chksum.Equals("d41d8cd98f00b204e9800998ecf8427e"))
                    log.Error("CheckSum is MD5-Hash for Empty file");

                foreach (var fileorigin in ds.LocalFileOrigins)
                {
                    log.ErrorFormat("name={0}, folder={1}", fileorigin.File.SourceFile.data.Name, fileorigin.File.SourceFolder);
                }
                foreach (var bagitOrigin in ds.BagItOrigins)
                {
                    log.ErrorFormat("name={0}, bagit={1}", bagitOrigin.Data.SourceData.Name, bagitOrigin.Data.SourceBag);
                }
            }

            //persistence to cache
            _fileMappingService.Commit();

            //Create jobs
            log.Debug("Create jobs");
            foreach (KeyValuePair<String, FileMapping> entry in fileMappings)
            {
                FileMapping ds = entry.Value;
                string chksum = entry.Key;
                if (chksum.Equals("d41d8cd98f00b204e9800998ecf8427e"))
                {
                    //Ignore Empty files
                    continue;
                }

                //check if is synced
                switch (ds.BagItOrigins.Count)
                {
                    case 0:
                        //Create or Add to Bagit for File
                        BagId bagid = null;
                        foreach (var fileorigin in ds.LocalFileOrigins)
                        {
                            if (bagid != null) break;
                            bagid = _folderBagitMappingService.Get(fileorigin.File.SourceFolder);
                        }

                        if(bagid == null)
                        {
                                bagid = BagId.CreateId();
                                //Create Job
                                jobs.Add(new CreateBagitJob(bagid, ds.LocalFileOrigins[0].File.SourceFolder));
                        }
                        
                        //Create Job
                        jobs.Add(new UploadFileJob(bagid, ds.LocalFileOrigins[0].File));

                        //Add to or update  Mapping
                        foreach (var fileorigin in ds.LocalFileOrigins)
                        {
                            _folderBagitMappingService.Incr(fileorigin.File.SourceFolder, bagid, chksum);
                        }

                        break;
                    case 1:
                        var orgin = ds.BagItOrigins[0];
                        if (!orgin.Data.State.Syncronized)
                        {
                            log.Warn("=> Synchronize Not implemented bagit=" + orgin.Data.SourceBag);
                        }
                        break;
                    default:
                        //something wrong
                        break;
                }
                //check if file is in Repository
            }
            //persist mapping
            _folderBagitMappingService.Commit();
        }
       
    }
}
