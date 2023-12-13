using scanhtml.DAL;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using HtmlAgilityPack;
using scanhtml.Methods;
using System.Threading.Tasks.Dataflow;
using scanhtml.Model;
using System.Text;

namespace ConsoleApp
{
    class Program
    {
        private static IConfiguration? _iconfiguration;
        static void Main(string[] args)
        {
            GetAppSettingsFile();

            Console.WriteLine("[a] for all links, [c] to check redirections for links that are not documents, [d] to get duplicates, {r] to generate report & [u] to update links.");
            var k = Console.ReadKey();
            switch (k.KeyChar)
            {
                case 'u':
                    GetLinks();
                    break;
                case 'r':
                    GetReport();
                    break;
                case 'd':
                    GetDuplicates();
                    break;
                case 'a':
                    GetAllLinks();
                    break;
                case 'c':
                    CheckRedirectionsForNotDocument();
                    break;
            }
        }

        static void GetAppSettingsFile()
        {
            var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();
        }



        static void GetLinks()
        {
            var scanHtmlDAL = new ScanHtmlDAL(_iconfiguration);
            var listScanHtmlModel = scanHtmlDAL.GetList();
            listScanHtmlModel.ForEach(item =>
            {

                var path = @"https://actuaries.org.uk/" + item.URL;
                HtmlWeb htmlWeb = new HtmlWeb();
                HtmlDocument doc = htmlWeb.Load(path);
                var nodes = doc.DocumentNode.SelectNodes("//section");
                var extract = new Extract();
                Console.WriteLine(item.URL);
                if (nodes != null && nodes.Count() >= 3)
                {


                    var contentSection = nodes[1];
                    if (contentSection.Attributes.Contains("class") && contentSection.Attributes["class"].Value.Contains("page-intro-container"))
                    { contentSection = nodes[2]; }
                    var links = extract.Links(contentSection);
                    scanHtmlDAL.WriteHtml(item.URL, contentSection.InnerHtml);
                    if (links != null && links.Count > 0)
                    {
                        var web = new scanhtml.Methods.Web();
                        web.CheckAllLinks(links, item.URL, scanHtmlDAL);
                    }
                }
            });
        }
        static void GetAllLinks()
        {
            var scanHtmlDAL = new ScanHtmlDAL(_iconfiguration);
            var listScanHtmlModel = scanHtmlDAL.GetList();
            listScanHtmlModel.ForEach(item =>
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(item.RawHtml);

                var nodes = doc.DocumentNode.SelectNodes("//section");
                var extract = new Extract();
                var links = new List<Link>();
                if (nodes != null)
                {
                    foreach (var node in nodes)
                    {
                        var nodeLinks = extract.Links(node);
                        if (nodeLinks != null && nodeLinks.Count > 0)
                        {
                            links.AddRange(nodeLinks);
                        }
                    }
                    if (links != null && links.Count > 0)
                    {
                        scanHtmlDAL.WriteAllLinks(item.URL, links);
                    }
                }
            });
        }

        static void GetReport()
        {
            var scanHtmlDAL = new ScanHtmlDAL(_iconfiguration);
            var writeHtml = new WriteHtml();
            var output = new StringBuilder();
            output.AppendLine("<html><body><h1>All Links</h1><h2>All Links by Response Code</h2>");
            output.AppendLine(writeHtml.WriteCodeTable(scanHtmlDAL.GetCodes()));
            File.WriteAllText("C:\\Users\\johns\\OneDrive\\Documents\\LinksReport.html", output.ToString());
            Console.WriteLine("Links Report Written.");
        }
        static void GetDuplicates()
        {
            var scanHtmlDAL = new ScanHtmlDAL(_iconfiguration);
            var writeHtml = new WriteHtml();
            var extract = new Extract();
            var linkedList = scanHtmlDAL.GetLinks();
            var docs = extract.DocumentsByExtension(linkedList);

            var duplicatesCount = docs.GroupBy(g => g.Destination)
                .Select(s => new { doc = s.Key, Count = s.Count() })
                .OrderByDescending(x => x.Count)
                .Where(w => w.Count > 1);

            var duplicateLinks = new List<DuplicateSources>();
            foreach (var source in duplicatesCount)
            {
                var sources = linkedList.Where(w => w.Destination == source.doc).Select(s => s.Source).ToList();
                var distinctSources = sources.Distinct().ToList();

                duplicateLinks.Add(new DuplicateSources { Destination = source.doc, Sources = distinctSources, totalLinks = source.Count, totalSources = distinctSources.Count() });
            }
            var duplicateSources = duplicateLinks.Where(w => w.totalSources > 1).ToList();

            var output = new StringBuilder();

            output.AppendLine("<html><body><h1>Documents With Duplicate Sources</h1><h2>Documents Linked From Multiple Pages</h2>");
            output.AppendLine(writeHtml.WriteDuplicateSourcesTable(duplicateSources));



            var topLevels = docs.Select(s => s.TopLevel).Distinct().ToList();
            output.AppendLine("<h1>All Top Level Folders</h1>");
            output.AppendLine(writeHtml.WriteTopLevels(topLevels));

            var folders = new List<DocsByFolder>();
            foreach (string folder in topLevels)
            {
                var docsByFolder = docs.Where(w => w.TopLevel == folder).Select(s => s.Destination).ToList();
                folders.Add(new DocsByFolder { Folder = folder, Documents = docsByFolder, totalDocuments = docsByFolder.Count() });
            }
            output.AppendLine("<h1>All Folders With Documents</h1>");
            output.AppendLine(writeHtml.WriteFolders(folders));


            output.AppendLine("</body></html>");
            File.WriteAllText("C:\\Users\\johns\\OneDrive\\Documents\\DuplicatesReport.html", output.ToString());


        }
        static void CheckRedirectionsForNotDocument()
        {
            var scanHtmlDAL = new ScanHtmlDAL(_iconfiguration);
            var extract = new Extract();
            var web = new Web();

            var uniqueDestinations = scanHtmlDAL.GetLinks().Select(s => s.Destination).Distinct().Take(10).ToList();
            web.CheckAllRedirection(uniqueDestinations);

            
            


        }
    }
}
