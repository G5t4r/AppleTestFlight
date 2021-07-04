using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AppleTestFlight.Core
{
    /// <summary>
    /// HTTP请求工具类
    /// </summary>
    public class HttpRequest
    {
        public static string HttpRequestByPost(string url, string parameters, string contentType, ref WebHeaderCollection header)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Headers = header;
            request.ContentType = contentType;
            request.Accept = "*/*";
            request.UserAgent = "ImXiongStu Runtime";
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(Encoding.UTF8.GetBytes(parameters), 0, Encoding.UTF8.GetBytes(parameters).Length);
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            header = response.Headers;
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }


        public static string HttpRequestByDelete(string url, string parameters, string contentType, WebHeaderCollection header)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "Delete";
            request.Headers = header;
            request.ContentType = contentType;
            request.Accept = "*/*";
            request.UserAgent = "ImXiongStu Runtime";
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(Encoding.UTF8.GetBytes(parameters), 0, Encoding.UTF8.GetBytes(parameters).Length);
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }


        public static string HttpRequestByGet(string url, WebHeaderCollection header)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Headers = header;
            request.Accept = "*/*";
            request.UserAgent = "ImXiongStu Runtime";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
