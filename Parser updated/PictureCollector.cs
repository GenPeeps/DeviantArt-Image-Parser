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
            var picsPages = currentPage.DocumentNode.SelectNodes("//div[contains(@class, 'tt-a tt-fh')]");
            //если вдруг на странице нет картинок - bool noPicsLeft - значение будет true и мы выйдем из while-loop в классе Program
            if (picsPages == null)
            {
                noPicsLeft = true;
            }
            else
            {
                //Прокручиваем каждую картинку на странице и качаем ее
                foreach (var picPage in picsPages)
                {
                    var picPageUrl = picPage.SelectSingleNode(".//a[contains(@class, 'thumb')]").Attributes["href"].Value.ToString();

                    //*** 
                    //Имя художника
                    string artistName = picPageUrl.Substring(7, picPageUrl.IndexOf(".") - 7);
                    //Название картинки - надо сделать reverse строки - и в ней искать от конца строки до первого "/". Результат снова сделать обратный reverse
                    string reversedPicPageUrl = Reverse(picPageUrl);
                    string reversedArtName = (reversedPicPageUrl.Substring(0, reversedPicPageUrl.IndexOf(@"/")));
                    string artName = Reverse(reversedArtName);
                    //***

                    string bestPic = getBestPic(picPageUrl);
                    if (bestPic != "")
                    {
                        double random = rand.Next(3, 6);
                        string intervalStr = random.ToString() + "00";
                        int intervalInt = int.Parse(intervalStr);
                        Console.WriteLine($"Start: {intervalInt}");
                        Console.WriteLine(bestPic);
                        Thread.Sleep(intervalInt);
                        Console.WriteLine($"End");
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

        private static string getBestPic(string picPageUrl) //TODO: что то было у БОба про методы <T> - наследующие
        {
            var htmlDocument = NodesCollector.getHtml(picPageUrl);
            if (htmlDocument.DocumentNode.SelectSingleNode("//div[@class='nogo']") == null)
            {
                var nodes = NodesCollector.getNodes(picPageUrl);

                //Из корневища надо найти 2 класса из общего класса class="dev-view-deviation"
                //- в каждом затем надо найти аттрибут height/width и сравнить их друг с другом. Кто больше, того и выбираем.
                var variants = nodes
                    .Where(n => n.GetAttributeValue("class", "")
                    .Equals("dev-view-deviation"))
                    .Single() //находим большой класс 
                    .Descendants("img"); // находим 2 картинки, собираем в коллекцию

                Dictionary<int, string> dictionary = Dictionary.formBasePicsDictionary(variants);
                ////находим лучшую картинку из словаря
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
            var isDownloaded = false;
            do
            {
                try
                {
                    string remoteImageUrl = bestPic;
                    //string strRealname = Path.GetFileName(remoteImageUrl);
                    string exts = Path.GetExtension(remoteImageUrl);
                    string appDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                    string saveloc = (appDir + "\\" + artistName).Remove(0, appDir.IndexOf("\\") + 1);
                    if (!Directory.Exists(saveloc))
                    {
                        Directory.CreateDirectory(saveloc);
                    }
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
                        webClient.DownloadFile(remoteImageUrl, saveloc + "\\" + artName + exts);
                        isDownloaded = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("PictureCollector -- " + ex.Message);
                }
            } while (isDownloaded == false);
        }
    }
}
