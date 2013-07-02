using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace NextTests.Mocks
{
    public static class HttpListenerRequestExt
    {
        public static bool IsPost(this HttpListenerRequest request)
        {
            return request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsGet(this HttpListenerRequest request)
        {
            return request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsPut(this HttpListenerRequest request)
        {
            return request.HttpMethod.Equals("PUT", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsDelete(this HttpListenerRequest request)
        {
            return request.HttpMethod.Equals("DELETE", StringComparison.OrdinalIgnoreCase);
        }

        public static NameValueCollection GetNameValueCollection(this HttpListenerRequest request)
        {
            string body;
            using (var reader = new StreamReader(request.InputStream, new UTF8Encoding(false), true))
            {
                body = reader.ReadToEnd();
            }
            string[] pairs = body.Split('&');
            var nvc = new NameValueCollection();
            foreach (var pair in pairs)
            {
                string[] split = pair.Split('=');
                nvc.Add(split[0], System.Web.HttpUtility.UrlDecode(split[1]));
            }
            return nvc;
        }
    }
}