using Archiv10.Application.Shared.BO;
using Archiv10.Domain.Shared.BO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Application.Shared
{
    public interface IApplication
    {
        ObservableCollection<UIData> DataList { get; set; }
        ObservableCollection<UIBagSnippet> BagList { get; set; }
        ObservableCollection<UIRepository> RepositoryList { get; set; }

        ObservableCollection<UIFile> FileList { get; set; }
        ObservableCollection<UIFolder> LocalFolderList { get; set; }
        ObservableCollection<UIRootFolder> RootFolderList { get; set; }

        ObservableCollection<UINodeItem> RootFolderNodeList { get; set; }

        void ReadConfig();

        void AddRepository(string url, string name, string token);
        void ReadRepositories();

        void ReadStatus();

        void ReadFiles(int index);
        void CreateBag(BagId bagid,string description);
        //Ermittelt Anzahl an Packages die ber der aktuellen Blockgröße zu übertragen sind (Upload)
        int CountPartsOfFile(string filename);

        //Anzahl der aktiven Repositories für dieses BagIt
        int CountRepositories(int index);
        // Ermittelt Anzahl an Packages die ber der aktuellen Blockgröße zu übertragen sind (Sync + Download)
        int CountPartsOfFile(UIData data);
        // Ermittelt die Anzahl der aktiven Repositories, die diese Datei noch nicht beinhalten
        int CountActiveRepositoriesToSync(UIData data);

        // Ermiitelt die Anzahl der zu übertragenen 
        void AppendFile(int index, string filename, Func<object> partTransfered);
        void SyncBag(UIBagSnippet uiBagSnippet);

      
        void SyncData(UIData data, Func<object> partTransfered);
        void DownloadFile(UIData uiData, string folder, Func<long,object> partTransfered);


        //local functionality
        void AddRootFolder(string path, string[] filter, NameType naming);
        void UpdateRootFolder(string path, string[] filter, NameType naming);

        void ReadRootFolders();
        void ReadLocalFiles(int index);

        void ReadRootFolderTree(int index);

        string CreateDescription(FolderPath path);

  
        bool UpdateSyncFolderStatus();
        void CommitSyncFolderStatus(IList<RootFolderSyncStatus> syncstates);


        //Service state
        void WriteServiceState(bool working, string state, string Format, params Object[] args);
        ServiceStatus ReadServiceStatus();


        //last modified
        void UpdateFileWatcher();
        DateTime GetLastmodifiedForRepositories();
        DateTime GetLastmodifiedForLocalFiles();
        DateTime GetLastmodifiedForConfiguration();
    }
}
