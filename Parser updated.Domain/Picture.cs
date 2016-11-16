using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser_updated.Domain
{
    class Picture
    {
        public int Width { get; set; }
        public string Url { get; set; }

        public Picture(int width, string url)
        {
            Width = width;
            Url = url;

        }
    }
}
