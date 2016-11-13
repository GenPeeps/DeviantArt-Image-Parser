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
            string artist = Console.ReadLine();
            string urlGallery = "http://" + artist + ".deviantart.com/gallery/?catpath=/";
            HtmlDocument gallery = new HtmlDocument();
            gallery.LoadHtml(NodesCollector.getResponseFromServer(urlGallery));

            //Определить интервал между оффсетами: считаем сколько картинок на 1-ой странице
            var picsPages = gallery.DocumentNode.SelectNodes("//div[contains(@class, 'tt-a tt-fh')]");
            int PicsPerPage = picsPages.Count();

            //первичное значение оффсет - 0
            int currenPageOffset = 0;
            //bool - есть ли картинки еще на странице
            bool noPicsLeft;
            //создаем переменную с текущей страницей
            //var currentPageUrl = urlGallery + "?catpath=%2F&offset=0"; 
            //качаем самую первую страницу
            PictureCollector.DownloadBestPicOnPage(gallery, out noPicsLeft);

            while (noPicsLeft == false)
            {
                //после первой страницы галереи - увеличиваем число оффсет
                currenPageOffset += PicsPerPage;
                //используем оффсет в адресе следующей страницы галереи
                var currentPageUrl = urlGallery + "&offset=" + currenPageOffset;
                //если закачка картинки вернет false, выходим из while. 
                PictureCollector.DownloadBestPicOnPage(NodesCollector.getHtml(currentPageUrl), out noPicsLeft);                               
            }

        }
    }
}
