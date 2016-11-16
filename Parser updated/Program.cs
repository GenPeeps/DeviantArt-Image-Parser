using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

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
                Domain.PictureCollector.DownloadGallery(urlGallery);
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("Sorry, this artist has no deviations yet.");
                Console.ReadLine();
            }
            catch (NotSupportedException)
            {
                Console.WriteLine("There is a problem with downloading this file. Please restart the application.");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException + ex.Message);
                Console.ReadLine();
            }
            
        }
    }
}
