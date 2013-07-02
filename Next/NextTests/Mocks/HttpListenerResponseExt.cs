using System.Net;
using System.Text;

namespace NextTests.Mocks
{
    public static class HttpListenerResponseExt
    {
        public static void Write(this HttpListenerResponse resonse, string s)
        {
            byte[] data = Encoding.UTF8.GetBytes(s);
            resonse.ContentLength64 = data.Length;
            resonse.OutputStream.Write(data, 0, data.Length);
        }

        public static void Write<T>(this HttpListenerResponse resonse, T item)
        {
            string serialize = new RestSharp.Serializers.JsonSerializer().Serialize(item);
            byte[] data = Encoding.UTF8.GetBytes(serialize);
            resonse.ContentLength64 = data.Length;
            resonse.ContentType = "application/json; charset=utf-8";
            resonse.OutputStream.Write(data,0,data.Length);

        }
    }
}