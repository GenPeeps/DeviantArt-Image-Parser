using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using HtmlAgilityPack;

namespace Parser_updated
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Which artist would you like to find?");
            string artist = Console.ReadLine().Trim().ToLower();
            if (artist == "")
            {
                Console.WriteLine("The artist is not specified. Please restart the application and try again.");
                Console.ReadLine();
                return;
            }
            string urlGallery = "http://" + artist + ".deviantart.com/gallery/?catpath=/";
            try
            {
                HtmlDocument gallery = NodesCollector.getHtml(urlGallery);
                //каждая страница имеет оффсет. Определить интервал между оффсетами - по числу картинок на 1-ой странице
                var picsPages = gallery.DocumentNode.SelectNodes("//div[contains(@class, 'tt-a tt-fh')]");
                int PicsPerPage = picsPages.Count();

                //bool - есть ли на странице еще картинки для скачивания (out param. для метода DownloadBestPicOnPage)
                bool noPicsLeft;
                //качаем самую первую страницу
                PictureCollector.DownloadBestPicOnPage(gallery, out noPicsLeft);

                //первичное значение оффсет - 0
                int currenPageOffset = 0;

                while (noPicsLeft == false)
                {
                    //после первой страницы галереи - увеличиваем число оффсет
                    currenPageOffset += PicsPerPage;
                    //используем оффсет в адресе следующей страницы галереи
                    var currentPageUrl = urlGallery + "&offset=" + currenPageOffset;
                    //если закачка картинки вернет false, выходим из while - в галерее больше нет страниц. 
                    PictureCollector.DownloadBestPicOnPage(NodesCollector.getHtml(currentPageUrl), out noPicsLeft);
                }
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("Sorry, this artist has no deviations yet.");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Sorry, this artist couldn't be found.");
                Console.ReadLine();
            }
            
        }
    }
}
