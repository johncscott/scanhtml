using scanhtml.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace scanhtml.Methods
{
    internal class WriteHtml
    {
        public string WriteCodeTable(Dictionary<string,string> codes) { 
        
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table><tr><th>Code</th><th>Count</th></tr>");
            foreach (var code in codes)
            {
                sb.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>",code.Key,code.Value);
            }
            sb.AppendLine("</table>");
            return sb.ToString();
        }
        public string WriteDuplicateSourcesTable(List<DuplicateSources> duplicates)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<h3>TOTAL DOCUMENTS:{0}</h3>", duplicates.Count()); 
            sb.AppendLine("<table border='1'><tr><th>Document</th><th>Pages</th><th>Page Count</th><th>Link Count</th></tr>");
            foreach (var dupe in duplicates)
            {
                sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>", dupe.Destination, string.Join("<br/>",dupe.Sources),dupe.totalSources.ToString(), dupe.totalLinks.ToString());
            }
            sb.AppendLine("</table>");
            return sb.ToString();
        }
        public string WriteTopLevels(List<string> TopLevels)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<h3>TOTAL TOP LEVEL SOURCE:{0}</h3>", TopLevels.Count());
            sb.AppendLine("<table border='1'><tr><th>Top Level Folders</th></tr>");
            foreach (var folder in TopLevels)
            {
                sb.AppendFormat("<tr><td>{0}</td></tr>", folder);
            }
            sb.AppendLine("</table>");
            return sb.ToString();
        }
        public string WriteFolders(List<DocsByFolder> folders)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<h3>TOTAL DOCUMENTS:{0}</h3>", folders.Select(s=>s.totalDocuments).Sum());
            sb.AppendLine("<table border='1'><tr><th>Folder</th><th>Documents</th><th>Count</th></tr>");
            foreach (var folder in folders)
            {
                sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>",
                    folder.Folder,
                    string.Join("<br/>", folder.Documents.Select(s=>HttpUtility.UrlDecode(s))),
                    folder.totalDocuments.ToString());
            }
            sb.AppendLine("</table>");
            return sb.ToString();
        }
    }
}
