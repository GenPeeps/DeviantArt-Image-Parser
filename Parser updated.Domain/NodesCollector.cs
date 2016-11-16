using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;

namespace Parser_updated.Domain
{
    public static class NodesCollector
    {
        static public HtmlDocument GetHtml(string url)
        {

            var html = new HtmlDocument();
            string htmlString = getResponseFromServer(url);
            html.LoadHtml(htmlString);
            return html;
        }
        public static IEnumerable<HtmlNode> GetNodes(HtmlDocument html)
        {
            var root = html.DocumentNode;
            var nodes = root.Descendants();
            return nodes;
        }
        public static IEnumerable<HtmlNode> GetNodes(string url)
        {

            var html = GetHtml(url);
            var root = html.DocumentNode;
            var nodes = root.Descendants();
            return nodes;
        }

        private static string getResponseFromServer(string url)
        {
            string responseFromServer = "";
            try
            {
               
                //Создаем запрос с url 
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                //Устанавливаем UserAgent
                //int randIndex = random.Next(0, _userAgentList.Count - 1);
                request.UserAgent = "[MOZILLA/5.0 (WINDOWS NT 6.1; WOW64) APPLEWEBKIT/537.1 (KHTML, LIKE GECKO) CHROME/21.0.1180.75 SAFARI/537.1]";
                //Получаем от сервера compressed files
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
                //Декомпрессия
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                // Получаем response от сервера.
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //Получаем объект stream с ответом от сервера.
                using (Stream dataStream = response.GetResponseStream())
                {

                    //Открываем stream через StreamReader для более удобного доступа.
                    StreamReader reader = new StreamReader(dataStream);
                    // Считываем содержимое.
                    responseFromServer = reader.ReadToEnd();
                }
                response.Close();
                return responseFromServer;
            }
            catch (Exception ex)
            {                
                throw ex;
            }
        }
    }
}
