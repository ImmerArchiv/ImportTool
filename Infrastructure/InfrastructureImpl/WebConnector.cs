using Archiv10.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archiv10.Infrastructure.Shared.BO;
using System.Net;
using System.IO;
using Archiv10.Infrastructure.Shared.Locator;
using log4net;
using System.Net.Http;

namespace Archiv10.Infrastructure.Impl
{
    class WebConnector : IWebConnector
    {
        private readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Shared.BO.WebResponse Get(Shared.BO.WebRequest request)
        {
            var response = new Shared.BO.WebResponse();

            log.DebugFormat("GET {0}", request.Url);
            var httpWebRequest = (HttpWebRequest)System.Net.WebRequest.Create(request.Url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            if (!string.IsNullOrWhiteSpace(request.AccessToken))
                httpWebRequest.Headers.Add("Authorization", "Bearer " + request.AccessToken);
            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                response.Status = (int)httpResponse.StatusCode;

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    response.Data = streamReader.ReadToEnd();
                }
            }
            catch (WebException e)
            {
                log.Error(e.Message, e);
                var httpResponse = (HttpWebResponse)e.Response;
                if (httpResponse != null)
                {
                    response.Status = (int)httpResponse.StatusCode;
                    response.Data = httpResponse.StatusDescription;
                }
                else
                {
                    response.Status = -1;
                    response.Data = e.Message;
                }
            }

            log.DebugFormat("Status={0}", response.Status);
            return response;
        }

        public Shared.BO.WebResponse Post(Shared.BO.WebRequest request)
        {
            var response = new Shared.BO.WebResponse();

            var httpWebRequest = (HttpWebRequest)System.Net.WebRequest.Create(request.Url);
            log.DebugFormat("POST {0} / {1}", request.Url, request.Data);

            httpWebRequest.Method = "POST";

            if (!string.IsNullOrWhiteSpace(request.AccessToken))
                httpWebRequest.Headers.Add("Authorization", "Bearer " + request.AccessToken);


            httpWebRequest.ContentType = "application/json";

            var requestData = System.Text.Encoding.Default.GetBytes(request.Data);

            httpWebRequest.ContentLength = requestData.Length;
            httpWebRequest.ServicePoint.Expect100Continue = false;

            using (var requestStream = httpWebRequest.GetRequestStream())
            {
                log.DebugFormat("write {0} Bytes", requestData.Length);
                requestStream.Write(requestData, 0, requestData.Length);
                requestStream.Flush();
            }

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                handleResponse(response, httpResponse);

             
            }
            catch (WebException e)
            {
                log.Error(e.Message, e);
                var httpResponse = (HttpWebResponse)e.Response;
                if (httpResponse != null)
                {
                    handleResponse(response, httpResponse);
                    //response.Status = (int)httpResponse.StatusCode;
                    //response.Data = httpResponse.StatusDescription;
                }
                else
                {
                    response.Status = -1;
                    response.Data = e.Message;
                }
            }
            log.DebugFormat("Status={0}", response.Status);
            return response;
        }

        private void handleResponse(Shared.BO.WebResponse response, HttpWebResponse httpResponse)
        {
            response.Status = (int)httpResponse.StatusCode;
            //var contentLength = httpResponse.ContentLength; - compare with readed bytes
            using (var responseStream = httpResponse.GetResponseStream())
            {
                var buffer = new byte[0xFFFF];
                int bytesRead;
                StringBuilder data = new StringBuilder();

                while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    log.DebugFormat("read {0} Bytes", bytesRead);
                    var str = System.Text.Encoding.Default.GetString(buffer, 0, bytesRead);
                    data.Append(str);
                }
                log.DebugFormat("completed sum={0} bytes", data.Length);
                response.Data = data.ToString();
            }
        }

        public Shared.BO.WebResponse Post(Shared.BO.WebRequest request, Func<byte[], int, object> callBackWrite)
        {


            var response = new Shared.BO.WebResponse();

            var httpWebRequest = (HttpWebRequest)System.Net.WebRequest.Create(request.Url);
            log.DebugFormat("POST {0} / {1}", request.Url, request.Data);

            httpWebRequest.Method = "POST";

            if (!string.IsNullOrWhiteSpace(request.AccessToken))
                httpWebRequest.Headers.Add("Authorization", "Bearer " + request.AccessToken);


            httpWebRequest.ContentType = "application/json";

            var requestData = System.Text.Encoding.Default.GetBytes(request.Data);

            httpWebRequest.ContentLength = requestData.Length;
            httpWebRequest.ServicePoint.Expect100Continue = false;

            using (var requestStream = httpWebRequest.GetRequestStream())
            {
                log.DebugFormat("write {0} Bytes", requestData.Length);
                requestStream.Write(requestData, 0, requestData.Length);
                requestStream.Flush();
            }

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                response.Status = (int)httpResponse.StatusCode;

                //var contentLength = httpResponse.ContentLength; - compare with readed bytes
                using (var responseStream = httpResponse.GetResponseStream())
                {
                    var buffer = new byte[0xFFFF];
                    int bytesRead;
                    long totalRead = 0L;
                    while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        log.DebugFormat("read {0} Bytes", bytesRead);
                        callBackWrite(buffer, bytesRead);
                        totalRead += bytesRead;
                    }
                    log.DebugFormat("completed sum={0} bytes", totalRead);

                }
            }
            catch (WebException e)
            {
                log.Error(e.Message, e);
                var httpResponse = (HttpWebResponse)e.Response;
                if (httpResponse != null)
                {
                    response.Status = (int)httpResponse.StatusCode;
                    response.Data = httpResponse.StatusDescription;
                }
                else
                {
                    response.Status = -1;
                    response.Data = e.Message;
                }
            }
            log.DebugFormat("Status={0}", response.Status);
            return response;

        }


        public Shared.BO.WebResponse Post(Shared.BO.WebUploadRequest request)
        {
            var response = new Shared.BO.WebResponse();

            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

            var httpWebRequest = (HttpWebRequest)System.Net.WebRequest.Create(request.Url);

            log.DebugFormat("POST {0} / {1}", request.Url, DictionaryToString(request.Data));

            httpWebRequest.Method = "POST";

            if (!string.IsNullOrWhiteSpace(request.AccessToken))
                httpWebRequest.Headers.Add("Authorization", "Bearer " + request.AccessToken);


            httpWebRequest.ContentType = "multipart/form-data; boundary=" + boundary;


            Stream memStream = new System.IO.MemoryStream();

            var boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            var endBoundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--");

            string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";
            foreach (KeyValuePair<string, string> entry in request.Data)
            {
                string formitem = string.Format(formdataTemplate, entry.Key, entry.Value);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                memStream.Write(formitembytes, 0, formitembytes.Length);
            }

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" + "Content-Type: application/octet-stream\r\n\r\n";


            memStream.Write(boundarybytes, 0, boundarybytes.Length);
            var header = string.Format(headerTemplate, "data", "filename.dummy");
            var headerbytes = System.Text.Encoding.UTF8.GetBytes(header);

            memStream.Write(headerbytes, 0, headerbytes.Length);
            memStream.Write(request.FileData, 0, request.FileData.Length);
            memStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);


            httpWebRequest.ContentLength = memStream.Length;
            httpWebRequest.ServicePoint.Expect100Continue = false;

            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                memStream.Position = 0;
                byte[] tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                memStream.Close();
                log.DebugFormat("write {0} Bytes", tempBuffer.Length);
                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            }





            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                response.Status = (int)httpResponse.StatusCode;

                //var contentLength = httpResponse.ContentLength; - compare with readed bytes
                using (var responseStream = httpResponse.GetResponseStream())
                {
                    var buffer = new byte[0xFFFF];
                    int bytesRead;
                    StringBuilder data = new StringBuilder();

                    while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        log.DebugFormat("read {0} Bytes", bytesRead);
                        var str = System.Text.Encoding.Default.GetString(buffer, 0, bytesRead);
                        data.Append(str);
                    }
                    log.DebugFormat("completed sum={0} bytes", data.Length);
                    response.Data = data.ToString();

                }
            }
            catch (WebException e)
            {
                log.Error(e.Message, e);
                var httpResponse = (HttpWebResponse)e.Response;
                if (httpResponse != null)
                {
                    response.Status = (int)httpResponse.StatusCode;
                    response.Data = httpResponse.StatusDescription;
                }
                else
                {
                    response.Status = -1;
                    response.Data = e.Message;
                }
            }
            log.DebugFormat("Status={0}", response.Status);



            return response;
        }

        private object DictionaryToString(IDictionary<string, string> source)
        {
            var keyValueSeparator = '=';
            var sequenceSeparator = " ";
            if (source == null)
                throw new ArgumentException("Parameter source can not be null.");
            var pairs = source.Select(x => string.Format("{0}{1}{2}", x.Key, keyValueSeparator, x.Value));
            return string.Join(sequenceSeparator, pairs);
        }

    }
}
