using scanhtml.Model;  
using Microsoft.Extensions.Configuration;  
using System;  
using System.Collections.Generic;  
using System.Data;  
using System.Data.SqlClient;
namespace scanhtml.DAL
{
    public class ScanHtmlDAL
    {
        private string? _connectionString;
        public ScanHtmlDAL(IConfiguration? iconfiguration)
        {
            _connectionString = iconfiguration.GetConnectionString("Default");
        }
        public List<ScanHtmlModel> GetList()
        {
            var listScanHtmlModel = new List<ScanHtmlModel>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * from [scan-html].[dbo].[ifoaprod-allUrls]", con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    listScanHtmlModel.Add(new ScanHtmlModel
                    {
                        URL = rdr[0].ToString(),
                        RawHtml = rdr[1].ToString()
                    });
                }
            }
            return listScanHtmlModel;
        }
        public void WriteHtml(string? url, string? html)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("UPDATE [dbo].[ifoaprod-allUrls] SET [rawhtml] = @html WHERE url = @url", con);
                cmd.Parameters.AddWithValue("url", url);
                cmd.Parameters.AddWithValue("html", html);
                cmd.CommandType = CommandType.Text;
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public void WriteLinks(string? pageUrl, List<Link>? links)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                //delete all previous values for this page
                SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[links] WHERE pageUrl = @url", con);
                cmd.Parameters.AddWithValue("url", pageUrl);
                con.Open();
                cmd.ExecuteNonQuery();
                //write new values for this page
                foreach (var link in links)
                {
                    cmd = new SqlCommand("INSERT INTO [dbo].[links] ([pageUrl],[href],[statuscode]) VALUES (@pageUrl, @href, @statuscode)", con);
                    cmd.Parameters.AddWithValue("pageUrl", pageUrl);
                    cmd.Parameters.AddWithValue("href", link.Href);
                    cmd.Parameters.AddWithValue("statuscode", link.StatusCode);
                    cmd.ExecuteNonQuery();
                }
                SqlDataReader insrdr = cmd.ExecuteReader();
                con.Close();
            }
        }
        public void WriteAllLinks(string? pageUrl, List<Link>? links)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                //delete all previous values for this page
                SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[alllinks] WHERE pageUrl = @url", con);
                cmd.Parameters.AddWithValue("url", pageUrl);
                con.Open();
                cmd.ExecuteNonQuery();
                //write new values for this page
                foreach (var link in links)
                {
                    cmd = new SqlCommand("INSERT INTO [dbo].[alllinks] ([pageUrl],[href]) VALUES (@pageUrl, @href)", con);
                    cmd.Parameters.AddWithValue("pageUrl", pageUrl);
                    cmd.Parameters.AddWithValue("href", link.Href);
                    cmd.ExecuteNonQuery();
                }
                SqlDataReader insrdr = cmd.ExecuteReader();
                con.Close();
            }
        }
        public void UpdateAllLinkWithRedirection(string? linkUrl, string redirectTo, string htmlResponse)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                //write new values for this link
                SqlCommand cmd = new SqlCommand("UPDATE [dbo].[alllinks] SET [redirectTo] = @redirectTo,[htmlResponse]=@htmlResponse WHERE href=@linkUrl", con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                cmd.Parameters.AddWithValue("redirectTo", redirectTo);
                cmd.Parameters.AddWithValue("htmlResponse", htmlResponse!=null?htmlResponse:"");
                cmd.Parameters.AddWithValue("linkUrl", linkUrl);
                cmd.ExecuteNonQuery();
                SqlDataReader insrdr = cmd.ExecuteReader();
                con.Close();
            }
        }
        public void UpdateLinkWithRedirection(string? linkUrl, string redirectTo, string htmlResponse)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                //write new values for this link
                SqlCommand cmd = new SqlCommand("UPDATE [dbo].[links] SET [href] = @redirectTo WHERE href=@linkUrl", con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                cmd.Parameters.AddWithValue("redirectTo", redirectTo);
                cmd.Parameters.AddWithValue("linkUrl", linkUrl);
                cmd.ExecuteNonQuery();
                SqlDataReader insrdr = cmd.ExecuteReader();
                con.Close();
            }
        }
        public void UpdateLinkRedirection(string? href, string destination)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                //write new values for this link
                SqlCommand cmd = new SqlCommand("UPDATE [dbo].[links] SET [destination] = @redirectTo WHERE href=@linkUrl", con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                cmd.Parameters.AddWithValue("redirectTo", destination);
                cmd.Parameters.AddWithValue("linkUrl", href);
                cmd.ExecuteNonQuery();
                SqlDataReader insrdr = cmd.ExecuteReader();
                con.Close();
            }
        }

        public Dictionary<string, string> GetCodes()
        {
            var listCodes = new Dictionary<string, string>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT statuscode, count(*)\r\n  FROM [scan-html].[dbo].[links]\r\n  GROUP BY statuscode\r\n  ORDER BY statuscode desc", con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    listCodes.Add(rdr[0].ToString(), rdr[1].ToString());
                }
            }
            return listCodes;

        }
        public List<Linked> GetLinks()
        {
            var listLinks = new List<Linked>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT [pageUrl],[href],[statuscode] FROM [scan-html].[dbo].[links] where statuscode=200", con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    listLinks.Add(new Linked { Source= rdr[0].ToString(),Destination= rdr[1].ToString(), TopLevel= rdr[0].ToString().Split('/')[1] });
                }
            }
            return listLinks;

        }
    }
}