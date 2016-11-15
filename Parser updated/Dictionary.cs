using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Parser_updated
{
    class Dictionary
    {
        internal static Dictionary<int, string> formBasePicsDictionary(HtmlNodeCollection variants)
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            foreach (var variant in variants)
            {
                int width = int.Parse(variant.GetAttributeValue("width", ""));
                string url = variant.GetAttributeValue("src", "");
                var pic = new Picture(width, url);
                //если словарь уже содержит картинку с определенным width параметром - break
                if (dictionary.ContainsKey(pic.Width))
                {
                    break;
                }
                else
                { dictionary.Add(pic.Width, pic.Url); }
            }
            return dictionary;
        }
    }
}
