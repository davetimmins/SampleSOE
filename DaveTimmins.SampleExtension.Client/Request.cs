using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using DaveTimmins.SampleExtension.Common.Model;

namespace DaveTimmins.SampleExtension.Client
{
    internal class Request
    {
        private string _url;
        private string _proxyUrl;

        public Request(string url, string proxyUrl)
        {
            _url = url;
            _proxyUrl = proxyUrl;
        }

        public T SubmitRequest<T>(Dictionary<string, string> parameters) where T : JsonableObject, new()
        {
            if (string.IsNullOrEmpty(_url))
                throw new NullReferenceException("You must specify a url.");

            if (parameters == null)
                parameters = new Dictionary<string, string>();
            parameters.Add("f", "json");

            string queryString = GetUrlEncodedQueryString(parameters);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(ProxyEncodeUrlAsString());
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.ContentLength = (long)(queryString.Length + 2);
            bool tmpExpect100Continue = httpWebRequest.ServicePoint.Expect100Continue;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

            var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            streamWriter.WriteLine(queryString);
            streamWriter.Flush();
            var httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;
            httpWebRequest.ServicePoint.Expect100Continue = tmpExpect100Continue;
            if (httpWebResponse != null && httpWebResponse.StatusCode == HttpStatusCode.OK)
            {
                string streamReader = new StreamReader(httpWebResponse.GetResponseStream()).ReadToEnd();
                return JsonableObject.FromJson<T>(streamReader);
            }

            throw new ApplicationException(httpWebResponse.StatusDescription);
        }

        private string ProxyEncodeUrlAsString()
        {
            if (string.IsNullOrEmpty(_proxyUrl))
                return _url;

            return _proxyUrl.Contains("?") ?
                string.Format("{0}{1}", _proxyUrl, Encoders.UrlEncode(_url)) :
                string.Format("{0}?{1}", _proxyUrl, Encoders.UrlEncode(_url));
        }

        private string GetUrlEncodedQueryString(Dictionary<string, string> parameters)
        {
            var stringBuilder = new StringBuilder();
            foreach (string index in parameters.Keys)
                stringBuilder.AppendFormat("{0}={1}&", index, Encoders.UrlEncode(parameters[index]));
            return stringBuilder.ToString().TrimEnd('&');
        }
    }
}
