using Archiv10.Infrastructure.Shared;
using Archiv10.Infrastructure.Shared.BO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Infrastructure.Impl
{
    class FileService : IFileService
    {
     

        public string GetNewFileName(string folder, string name)
        {
            var n = Path.GetFileNameWithoutExtension(name);
            var e = Path.GetExtension(name);

            for(int i = 0;i < 100;i++)
            {
                var fname = i == 0 ? String.Format("{0}{1}", n, e) : String.Format("{0}({1}){2}", n, i, e);
                var path = Path.Combine(folder, fname);

                if (File.Exists(path)) continue;
                return path;

            }
            throw new Exception("More then 100 tempfiles with same name not implemented");
        }

        public Stream OpenRead(string filename)
        {
            return File.OpenRead(filename);
        }


        public byte[] ReadBytes(string filename, long offset, int maxSize)
        {

            byte[] buffer = new byte[maxSize];
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            fs.Seek(offset, SeekOrigin.Begin);

            var r = fs.Read(buffer,0,maxSize);

            fs.Close();

            if (r == maxSize)
                return buffer;

            byte[] tbuffer = new byte[r];

            Array.Copy(buffer, tbuffer, r);

            return tbuffer;
        }

        public string ReadConfigFile(string configFileName)
        {
            var path = ConfigurationManager.AppSettings["ConfigPath"];
            var file = string.Format(@"{0}\{1}",path,configFileName);
            if (!File.Exists(file)) return null;
            return File.ReadAllText(file);
        }
    

        public void SaveConfigFile(string configFileName, string text)
        {
            var path = ConfigurationManager.AppSettings["ConfigPath"];
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var file = string.Format(@"{0}\{1}", path, configFileName);

            File.WriteAllText(file,text);
        }

        public void WriteBytes(string filename, byte[] buffer, int offset, int length)
        {
            using (var stream = new FileStream(filename, FileMode.Append))
            {
                stream.Write(buffer, offset, length);
            }
        }


        public bool CheckDirectory(string path)
        {
            return Directory.Exists(path);
        }

        public IList<string> ResolveDirectories(string path)
        {
            var list = new List<string>();
            AddDirectories(path, list);
            return list;
        }

        private void AddDirectories(string path, List<string> list)
        {
            if (Directory.GetFiles(path).Count() > 0)
            {
                list.Add(path);
            }

            foreach (var dir in Directory.GetDirectories(path))
            {
                AddDirectories(dir, list);
            }
        }

        public IList<string> ListFiles(string path,IFileFilter filter)
        {
            var list = new List<string>();
            foreach(var file in Directory.GetFiles(path))
            {
                if (filter.Match(file))
                    list.Add(file);
            }

            return list;
        }

        public long GetFileLength(string path)
        {
            return new FileInfo(path).Length;
        }

        public DateTime GetLastModified(string path)
        {
            return new FileInfo(path).LastWriteTime;
        }

        public DateTime ConfigPathLastmodified(string[] files)
        {
            var max =  DateTime.MinValue;
            var path = ConfigurationManager.AppSettings["ConfigPath"];
            foreach(var configFileName in files)
            {
                var file = string.Format(@"{0}\{1}", path, configFileName);
                var lm = GetLastModified(file);
                if (lm > max)
                    max = lm;
            }
            return max;
        }

        public T ReadCache<T>(string name) where T : class
        {
            var path = ConfigurationManager.AppSettings["ConfigPath"];
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);


            var cachePath = Path.Combine(path, "cache");
            if (!Directory.Exists(cachePath))
                Directory.CreateDirectory(cachePath);

            var cacheFile = Path.Combine(cachePath, name + ".json");

            if (!File.Exists(cacheFile))
                return null;

            return JsonConvert.DeserializeObject<T>(File.ReadAllText(cacheFile));
        }

        public void WriteCache<T>(string name, T data) where T : class
        {
            var path = ConfigurationManager.AppSettings["ConfigPath"];
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);


            var cachePath = Path.Combine(path, "cache");
            if (!Directory.Exists(cachePath))
                Directory.CreateDirectory(cachePath);

            var cacheFile = Path.Combine(cachePath, name + ".json");
            File.WriteAllText(cacheFile, JsonConvert.SerializeObject(data, Formatting.Indented));
        }
    }
}
