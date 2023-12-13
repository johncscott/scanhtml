using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scanhtml.Model
{
    public class ScanHtmlModel
    {
        public string? URL { get; set; }
        public string? RawHtml { get; set; }
    }
    public class Link
    {
        public string? Href { get; set; }
        public int? StatusCode { get; set; }
    }
    public class Redirection
    {
        public string? Link { get; set; }
        public bool? Redirects { get; set; }
        public string? Destination { get; set; }
    }
    public class Linked
    {
        public string? Source { get; set; }
        public string? Destination { get; set; }
        public string? TopLevel { get; set;}
    }
    public class DuplicateSources
    {
        public string? Destination { get; set; }
        public List<string?>? Sources { get; set; }
        public int totalSources { get; set; }
        public int totalLinks { get; set; }
    }

    public class DocsByFolder
    {
        public string? Folder { get; set; }
        public List<string?>? Documents { get; set; }
        public int totalDocuments { get; set; }
        public int totalLinks { get; set; }
    }
}