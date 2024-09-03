using Archiv10.Infrastructure.Shared.BO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Infrastructure.Shared
{
    public interface IFileService
    {
        string ReadConfigFile(string configFileName);
        void SaveConfigFile(string configFileName, string text);

        T ReadCache<T>(string name) where T : class;
        void WriteCache<T>(string name, T mapping) where T : class;

        Stream OpenRead(string filename);

        byte[] ReadBytes(string filename, long offset, int maxSize);
        string GetNewFileName(string folder, string name);
        void WriteBytes(string filename, byte[] buffer, int offset, int length);
        bool CheckDirectory(string path);
        IList<String> ResolveDirectories(string path);
        IList<String> ListFiles(string path, IFileFilter filter);
        long GetFileLength(string path);
        DateTime GetLastModified(string filepath);
        DateTime ConfigPathLastmodified(string[] files);
    }
}
