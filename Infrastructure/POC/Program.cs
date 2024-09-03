using Archiv10.Infrastructure.Shared;
using Archiv10.Infrastructure.Shared.BO;
using Archiv10.Infrastructure.Shared.Locator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Archiv10.Infrastructure.POC
{
    class Program
    {

        //Copy of RepositoryConfig
        private const int DataPartSize = (3 * 16);
        private const int DataSize = DataPartSize * 1024 * 10; //48KB


        static void Main(string[] args)
        {
            var url = "http://localhost:8080/repository1";
            var repository = "default";
            var token = "12345678";


            var bagid = "2543e7fd-b661-416a-bd3d-11506daf8617";

            PocInfo(url);


            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            //PocListAll(url,repository,token);


            // php.ini => upload_max_filesize = 2M  
            //PocUploadFile(url, repository, token, "tempfile_xyz.bin", @"C:\temp\SharedTypes.xsd");
           
            //PocUploadFile(url, repository, token, "tempfile_xyz.bin", @"C:\temp\mail.txt");

            PocGetFilePart(url, repository, token, bagid , "DateiHello.txt");

            stopWatch.Stop();
            Console.WriteLine(stopWatch.Elapsed.ToString());

            Console.WriteLine("Ready");
            Console.ReadKey();
        }



        private static void PocInfo(string url)
        {
            var webConnector = InfrastructureLocator.GetWebConnector();
            var webRequest = new WebRequest();
            webRequest.Url = url + "/info.json";
            var webResponse = webConnector.Get(webRequest);

            Console.WriteLine(webResponse.Status);
            Console.WriteLine(webResponse.Data);

            dynamic resp = new JavaScriptSerializer().DeserializeObject(webResponse.Data);
            Console.WriteLine(resp["state"]);
            Console.WriteLine(resp["message"]);

            Console.WriteLine("ApiVersion = " + resp["api_version"]);
            Console.WriteLine("GrantType  = " + resp["grant_type"]);
            Console.WriteLine("CheckSum   = " + resp["check_sum"]);
            Console.WriteLine(resp["modules"]["create"]);
            Console.WriteLine(resp["modules"]["status"]);
            Console.WriteLine(resp["modules"]["putfilepart"]);
            Console.WriteLine(resp["modules"]["appendfile"]);
            Console.WriteLine(resp["modules"]["listall"]);
            Console.WriteLine(resp["modules"]["listone"]);
            Console.WriteLine(resp["modules"]["getfilepart"]);
            Console.WriteLine(resp["modules"]["downloadfile"]);

        }

        private static void PocListAll(string url, string repository, String token)
        {
            var webConnector = InfrastructureLocator.GetWebConnector();
            var webRequest = new WebRequest();
            webRequest.Url = url + "/listall.php";
            webRequest.Data = new JavaScriptSerializer().Serialize(new
            {
                repository = repository,
                skip = 0,
                take = 100
            });
            webRequest.AccessToken = token;
            var webResponse = webConnector.Post(webRequest);
            Console.WriteLine(webResponse.Status);
            Console.WriteLine(webResponse.Data);

        }


        private static void PocGetFilePart(string url, string repository, string token, string bagit, string filename)
        {
            var webConnector = InfrastructureLocator.GetWebConnector();
            var webRequest = new WebRequest();
            webRequest.Url = url + "/getfilepart.php";
            webRequest.Data = new JavaScriptSerializer().Serialize(new
            {
                repository = repository,
                bagit = bagit,
                name = filename,
                offset = 0,
                maxlength = DataSize
            });

            webRequest.AccessToken = token;
            var webResponse = webConnector.Post(webRequest,(byte[] data,int len) => {

                Console.WriteLine("get {0}", len,DataSize);
                return null;

            });

        }


        private static void PocUploadFile(string url, string repository, string token, string tempname, string filename)
        {
            var webConnector = InfrastructureLocator.GetWebConnector();
            var webRequest = new WebUploadRequest();
            webRequest.Url = url + "/putfilepart.php";
            webRequest.Data = new Dictionary<string, string>();
            webRequest.Data["repository"] = repository;
            webRequest.Data["tempname"] = tempname;

            webRequest.AccessToken = token;
            webRequest.FileData = File.ReadAllBytes(filename);

            var webResponse = webConnector.Post(webRequest);
            dynamic resp = new JavaScriptSerializer().DeserializeObject(webResponse.Data);

        }



    }
}
