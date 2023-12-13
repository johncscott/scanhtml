using scanhtml.DAL;
using scanhtml.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace scanhtml.Methods
{
    public class Web
    {
        public List<Link>? Links {  get; set; }

        public async void CheckAllLinks(List<Link>? links, string? Url, ScanHtmlDAL? dal)
        {
            if (links != null && Url != null && dal != null)
            {
                List<Task<Link>> tasks = new List<Task<Link>>();
                foreach (var link in links)
                {
                    tasks.Add(GetResponseCode(link.Href));
                }
                var results = await Task.WhenAll(tasks);
                dal.WriteLinks(Url, results.ToList());
            }
            return;
        }
        public async void CheckAllRedirection(List<string>? links)
        {
            List<Task<Redirection>> tasks = new List<Task<Redirection>>();
            foreach (var link in links)
            {
                tasks.Add(GetRedirection(link));
            }
            var results = await Task.WhenAll(tasks);
            WriteRedirections(results.ToList());
            return;
        }

        private async Task<Link> GetResponseCode(string? url)
        {
            HttpClient httpClient = new HttpClient();
            var clientResults = await httpClient.GetAsync(url);
            
            return new Link { Href = url, StatusCode = (int)clientResults.StatusCode };
        }


        private static async Task<Redirection> GetRedirection(string? url)
        {
            Console.WriteLine(url);
            HttpClient httpClient = new HttpClient();
            var httpGet = await httpClient.GetAsync(url);
            return new Redirection { Link = url, Redirects = false, Destination = "nothingyet" };
            //HttpClient httpClient = new HttpClient();
            //try
            //{
            //    var httpGet = await httpClient.GetAsync(url);
            //    Console.WriteLine(url);
            //}
            //catch (Exception ex) { Console.WriteLine(ex.Message); }
            //return new Redirection { Link = url, Redirects = false, Destination="nothingyet" };
        }
        public void WriteRedirections(List<Redirection> redirs)
        { 
            foreach (var redir in redirs)
            {
                Console.WriteLine(redir.Link);
            }
        }


    }
}
