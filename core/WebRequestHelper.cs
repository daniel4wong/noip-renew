using System;
using System.IO;
using System.Net;

namespace noip_renew.core
{
    public class WebRequestHelper
    {
        public static string SendRequest(string url, CookieContainer cookie = null, string method = "GET", string contentType = "application/x-www-form-urlencoded", string data = "")
        {
            HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create("https://www.milliontech.com/mt/employee/Directory.aspx");
            if (cookie != null) hwr.CookieContainer = cookie;
            hwr.Method = method;
            hwr.ContentType = contentType;
            StreamWriter swr = new StreamWriter(hwr.GetRequestStream());
            swr.Write(data);
            swr.Close();

            WebResponse wr = hwr.GetResponse();
            string s = new StreamReader(wr.GetResponseStream()).ReadToEnd();
            Console.WriteLine(s);

            return s;
        }
    }
}
