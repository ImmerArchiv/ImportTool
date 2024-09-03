using Archiv10.Domain.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archiv10.Domain.Shared.BO;
using Archiv10.Infrastructure.Shared.Locator;
using Archiv10.Infrastructure.Shared.BO;
using System.Web.Script.Serialization;
using System.IO;
using System.Globalization;
using System.Numerics;
using log4net;

namespace Archiv10.Domain.Impl.Services
{
    class RepositoryService : IRepositoryService
    {
        private readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void UploadFilePart(Repository repository, BagFilePart filepart)
        {
            var webConnector = InfrastructureLocator.GetWebConnector();
            var webRequest = new WebUploadRequest();
            webRequest.Url = String.Format("{0}/{1}", repository.Url, repository.Info.UrlPutFilePart);
            webRequest.Data = new Dictionary<string, string>();
            webRequest.Data["repository"] = repository.RepositoryName;
            webRequest.Data["tempname"] = filepart.TempName;
            webRequest.FileData = filepart.Data;

            webRequest.AccessToken = repository.Token;
            var webResponse = webConnector.Post(webRequest);
            dynamic resp = DeserializeObject(webResponse);

            return;
        }

        public void AppendFile(Repository repository, BagId bagId, BagFile file)
        {
            var webConnector = InfrastructureLocator.GetWebConnector();
            var webRequest = new WebRequest();
            webRequest.Url = String.Format("{0}/{1}", repository.Url, repository.Info.UrlAppendFile);
            webRequest.Data = new JavaScriptSerializer().Serialize(new
            {
                repository = repository.RepositoryName,
                bagit = bagId.ToString(),
                name = file.FileName,
                tempname = file.TempName,
                md5 = file.CheckSum,
                mode = "complete"
            });

            webRequest.AccessToken = repository.Token;
            var webResponse = webConnector.Post(webRequest);
            dynamic resp = DeserializeObject(webResponse);

            return;

        }

        public BagId Create(Repository repository, BagId bagId, BagInfo info)
        {
            var webConnector = InfrastructureLocator.GetWebConnector();
            var webRequest = new WebRequest();
            webRequest.Url = String.Format("{0}/{1}", repository.Url, repository.Info.UrlCreate);
            webRequest.Data = EncodeNonAsciiCharacters(new JavaScriptSerializer().Serialize(new
            {
                repository = repository.RepositoryName,
                bagit = bagId.ToString(),
                info = ToArray(info)
            }));


            webRequest.AccessToken = repository.Token;
            var webResponse = webConnector.Post(webRequest);
            dynamic resp = DeserializeObject(webResponse);

            return new BagId(resp["bagit"]);
        }

        public BagFilePart GetFilePart(Repository repository, BagId bagId, string fileName, long offset, int maxSize)
        {
            var webConnector = InfrastructureLocator.GetWebConnector();
            var webRequest = new WebRequest();
            webRequest.Url = String.Format("{0}/{1}", repository.Url, repository.Info.UrlGetFilePart);
            webRequest.Data = new JavaScriptSerializer().Serialize(new
            {
                repository = repository.RepositoryName,
                bagit = bagId.ToString(),
                name = fileName,
                offset = offset,
                maxlength = maxSize
            });
            webRequest.AccessToken = repository.Token;

            MemoryStream buffer = new MemoryStream();
            var webResponse = webConnector.Post(webRequest, (byte[] data,int length) => {
                buffer.Write(data, 0, length);
                return null;
            });

            /*
            dynamic resp = new JavaScriptSerializer().DeserializeObject(webResponse.Data);

            string state = resp["state"];
            string message = resp["message"];
            */
            BagFilePart filepart = new BagFilePart();
            filepart.TempName = null;
            filepart.Data = buffer.ToArray(); 

            return filepart;
        }

        public void DownloadFile(Repository repository, BagId bagId, string fileName, Func<byte[], int, object> callBackWrite)
        {
            var webConnector = InfrastructureLocator.GetWebConnector();
            var webRequest = new WebRequest();
            webRequest.Url = String.Format("{0}/{1}", repository.Url, repository.Info.UrlDownloadFile);
            webRequest.Data = new JavaScriptSerializer().Serialize(new
            {
                repository = repository.RepositoryName,
                bagit = bagId.ToString(),
                name = fileName,
            });
            webRequest.AccessToken = repository.Token;
            var webResponse = webConnector.Post(webRequest, callBackWrite);
            return;
        }

        public bool Init(Repository repository)
        {
            try
            {
                var webConnector = InfrastructureLocator.GetWebConnector();
                var webRequest = new WebRequest();
                webRequest.Url = String.Format("{0}/{1}", repository.Url, repository.Info.UrlInfo);
                var webResponse = webConnector.Get(webRequest);

                dynamic resp = DeserializeObject(webResponse);

                repository.Info.ApiVersion = resp["api_version"];
                repository.Info.GrantType = resp["grant_type"];
                repository.Info.CheckSum = resp["check_sum"];
                repository.Info.UrlStatus = resp["modules"]["status"];
                repository.Info.UrlCreate = resp["modules"]["create"];
                repository.Info.UrlPutFilePart = resp["modules"]["putfilepart"];
                repository.Info.UrlAppendFile = resp["modules"]["appendfile"];
                repository.Info.UrlListAll = resp["modules"]["listall"];
                repository.Info.UrlListOne = resp["modules"]["listone"];
                repository.Info.UrlGetFilePart = resp["modules"]["getfilepart"];
                repository.Info.UrlDownloadFile = resp["modules"]["downloadfile"];                

            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }


        public void Status(Repository repository)
        {
            var webConnector = InfrastructureLocator.GetWebConnector();
            var webRequest = new WebRequest();
            webRequest.Url = String.Format("{0}/{1}", repository.Url, repository.Info.UrlStatus);
            webRequest.Data = new JavaScriptSerializer().Serialize(new
            {
                repository = repository.RepositoryName,
                skip = 0,
                take = 100
            });
            webRequest.AccessToken = repository.Token;
            var webResponse = webConnector.Post(webRequest);

            dynamic resp = DeserializeObject(webResponse);
            string state = resp["state"];
            string message = resp["message"];


            repository.Status.LastModified = DateTime.Parse(resp["lastmodified"]);
            repository.Status.Files = resp["files"];
            repository.Status.Bagits= resp["bagits"];
            repository.Status.Size = BigInteger.Parse(resp["size"]);
            repository.Status.MaxSize = BigInteger.Parse(resp["maxsize"]);



        }


        private int ListAll(Repository repository, IList<BagSnippet> list, long skip,long take)
        {
            var webConnector = InfrastructureLocator.GetWebConnector();
            var webRequest = new WebRequest();
            webRequest.Url = String.Format("{0}/{1}", repository.Url, repository.Info.UrlListAll);
            webRequest.Data = new JavaScriptSerializer().Serialize(new
            {
                repository = repository.RepositoryName,
                skip = skip,
                take = take
            });
            webRequest.AccessToken = repository.Token;
            var webResponse = webConnector.Post(webRequest);

            dynamic resp = DeserializeObject(webResponse);
            string state = resp["state"];
            string message = resp["message"];

            var cnt = 0;
            foreach (var bagit in resp["bagits"])
            {
                cnt++;
                var snippet = new BagSnippet();
                snippet.Id = new BagId(bagit["bagit"]);
                snippet.Info = new BagInfo();
                foreach (var info in bagit["info"])
                    foreach (KeyValuePair<string, object> elm in info)
                    {
                        snippet.Info.Add(new KeyValuePair<string, string>(elm.Key, elm.Value.ToString()));
                    }
                snippet.Status = new BagStatus();
                var status = bagit["status"];
                snippet.Status.LastModified = DateTime.Parse(status["lastmodified"]);
                snippet.Status.Files = status["files"];
                snippet.Status.Size = BigInteger.Parse(status["size"]);
                list.Add(snippet);
            }
            return cnt;
        }
        public IList<BagSnippet> ListAll(Repository repository)
        {
            var stepSize = 400L;
            var list = new List<BagSnippet>();
          
            for(long index = 0L;index < 100 * stepSize; index += stepSize) //MAX 10000 Bagit's 
            {
                if (ListAll(repository, list, index, stepSize) < stepSize) break;
            }


            return list;

        }

        public Bag ListOne(Repository repository, BagId bagId)
        {
         
            var webConnector = InfrastructureLocator.GetWebConnector();
            var webRequest = new WebRequest();
            webRequest.Url = String.Format("{0}/{1}", repository.Url, repository.Info.UrlListOne);
            webRequest.Data = new JavaScriptSerializer().Serialize(new
            {
               repository = repository.RepositoryName,
               bagit = bagId.ToString(),
            });
            webRequest.AccessToken = repository.Token;
            var webResponse = webConnector.Post(webRequest);

            dynamic resp = DeserializeObject(webResponse);

            string state = resp["state"];
            string message = resp["message"];

            var bag = new Bag();

            bag.Id = new BagId(resp["bagit"]);
            bag.Info = new BagInfo();
            foreach (var info in resp["info"])
                foreach (KeyValuePair<string, object> elm in info)
                {
                    bag.Info.Add(new KeyValuePair<string, string>(elm.Key, elm.Value.ToString()));
                }

            var status = resp["status"];
            bag.Status = new BagStatus();
            bag.Status.LastModified = DateTime.Parse(status["lastmodified"]);
            bag.Status.Files = status["files"];
            bag.Status.Size = BigInteger.Parse(status["size"]);

            foreach (var data in resp["data"])
            {

                bag.Data.Add(new BagData()
                {
                    Name = data["name"],
                    CheckSum = data["md5"],
                    Length = data["length"]
               });
               
            }
            return bag;
        }

        private static string EncodeNonAsciiCharacters(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                if (c > 127)
                {
                    // This character is too big for ASCII
                    string encodedValue = "\\u" + ((int)c).ToString("x4");
                    sb.Append(encodedValue);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private IDictionary<string,string>[] ToArray(BagInfo info)
        {
            var list = new List<IDictionary<string, string>>();
            foreach(var kvp in info)
            {
                var dict = new Dictionary<string, string>();
                dict[kvp.Key] = kvp.Value;
                list.Add(dict);
            }
            return list.ToArray();
        }


        private dynamic DeserializeObject(WebResponse webResponse)
        {
            try
            {
                dynamic resp = new JavaScriptSerializer().DeserializeObject(webResponse.Data);

                string state = resp["state"];
                string message = resp["message"];

                if (state != "ok")
                    throw new Exception(String.Format("{0}:{1}", state, message));

                return resp;
            }
            catch (System.ArgumentException) //parsing json
            {
                log.Error(webResponse.Data);
                throw new Exception("webresponse not in JSON Format");
            }
           
        }


    }
}
