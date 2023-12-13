using HtmlAgilityPack;
using scanhtml.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace scanhtml.Methods
{
    internal class Extract
    {
        public List<Link>? Links(HtmlNode node)
        {
            var links = new List<Link>();
            var anchors = node.Descendants("a").ToList();
            foreach (HtmlNode a in anchors)
            {
                if (a.Attributes["href"] != null && a.Attributes["href"].ToString() != string.Empty && a.Attributes["href"].ToString().Contains("www.acturaries.org.uk")) 
                {
                    links.Add(new Link { Href = a.Attributes["href"].ToString() });
                }
            }
            var imgs = node.Descendants("img").ToList();
            foreach (HtmlNode img in anchors)
            {
                if (img.Attributes["src"] != null && img.Attributes["src"].ToString() != string.Empty && img.Attributes["src"].ToString().Contains("www.acturaries.org.uk"))
                {
                    links.Add(new Link { Href = img.Attributes["src"].ToString() });
                }
            }

            return links;
        }

        public List<Linked> DocumentsByExtension(List<Linked> LinkList)
        {
            var docs = new List<Linked>();
            foreach (var link in LinkList)
            {
                if (HasExtension(link.Destination))
                {
                    docs.Add(link);
                }
            }
            return docs;
        }
        
        public bool HasExtension (string href) 
        {
            int LastDot = href.LastIndexOf('.');
            int Length = href.Trim().Length;
            return LastDot > 0 && Length - LastDot < 5;
        }

    }

}
