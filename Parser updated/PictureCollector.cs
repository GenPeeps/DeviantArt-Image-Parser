using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Parser_updated
{
    class PictureCollector
    {
        static Random rand = new Random();
        public static void DownloadBestPicOnPage(HtmlDocument currentPage, out bool noPicsLeft)
        {
            //Все url страниц картинок на общей странице
            var picsPages = currentPage.DocumentNode.SelectNodes("//div[contains(@class, 'tt-a tt-fh')]");
            //если вдруг на странице нет картинок - bool noPicsLeft - значение будет true и мы выйдем из while-loop в классе Program
            if (picsPages == null)
            {
                noPicsLeft = true;
            }
            else
            {
                foreach (var picPage in picsPages)
                {
                    //адрес страницы с картинкой
                    var picPageUrl = picPage.SelectSingleNode(".//a[contains(@class, 'thumb')]").Attributes["href"].Value.ToString();

                    //*** Используется в методе downloadPic()
                    //Имя художника
                    string artistName = picPageUrl.Substring(7, picPageUrl.IndexOf(".") - 7);
                    //Название картинки - надо сделать reverse строки - и в ней искать от конца строки до первого "/". Результат снова сделать обратный reverse
                    string reversedPicPageUrl = Reverse(picPageUrl);
                    string reversedArtName = (reversedPicPageUrl.Substring(0, reversedPicPageUrl.IndexOf(@"/")));
                    string artName = Reverse(reversedArtName);
                    //***
                    //Картинка с лучшим разрешением на странице
                    string bestPic = getBestPic(picPageUrl);
                    if (bestPic != "")
                    {
                        double random = rand.Next(3, 6);
                        string intervalStr = random.ToString() + "00";
                        int intervalInt = int.Parse(intervalStr);
                        Console.WriteLine($"Start: {intervalInt}");
                        Console.WriteLine(bestPic);
                        //Пауза перед скачиванием
                        Thread.Sleep(intervalInt);
                        Console.WriteLine($"End");
                        //Картинка скачивается
                        downloadPic(bestPic, artistName, artName);
                    }
                    else
                    {
                        noPicsLeft = false;
                    }
                   
                }
                noPicsLeft = false;
            }

        }

        private static string Reverse(string picPageUrl)
        {
            char[] charArray = picPageUrl.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        private static string getBestPic(string picPageUrl)
        {
            var htmlDocument = NodesCollector.getHtml(picPageUrl);
            if (htmlDocument.DocumentNode.SelectSingleNode("//div[@class='nogo']") == null)
            {
                //Задача - найти imgs из общего класса class="dev-view-deviation" - в каждом найти атрибут width и сравнить их друг с другом. Картинка с наибольшим атрибутом - лучшая.

                //Вариант 1 c Linq:                 
                //var nodes = NodesCollector.getNodes(htmlDocument);
                //var variants = nodes
                //    .Where(n => n.GetAttributeValue("class", "")
                //    .Equals("dev-view-deviation"))
                //    .Single() //находим большой класс 
                //    .Descendants("img"); // находим 2 картинки, собираем в коллекцию

                //Вариант 2 с Xpath
                var variantsXpath = htmlDocument.DocumentNode.SelectNodes("//div[@class='dev-view-deviation']/img");

                Dictionary<int, string> dictionary = Dictionary.formBasePicsDictionary(variantsXpath);
                //Находим лучшую картинку из словаря
                var bestPicIndex = dictionary.Keys.Max();
                string bestPic = dictionary[bestPicIndex];
                return bestPic;
            }
            else
            {
                return "";
            }
            
        }
        private static void downloadPic(string bestPic, string artistName, string artName)
        {
            try
            {
                string exts = Path.GetExtension(bestPic);
                string appDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                //Убираем "file:\\" из appDir и прописываем место хранения картинок: директория программы + имя художника
                string saveloc = (appDir + "\\" + artistName).Remove(0, appDir.IndexOf("\\") + 1);
                if (!Directory.Exists(saveloc))
                {
                    Directory.CreateDirectory(saveloc);
                }
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
                    webClient.DownloadFile(bestPic, saveloc + "\\" + artName + exts);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("PictureCollector -- " + ex.Message);
            }
        }
    }
}
