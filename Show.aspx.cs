using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Show : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //if there is dir parameter, and download=all parameter, then create zip file and send it to response
        string dir = Request.QueryString["dir"];
        string download = Request.QueryString["download"];
        if (String.IsNullOrEmpty(dir) || String.IsNullOrEmpty(download) || !download.Equals("all", StringComparison.CurrentCultureIgnoreCase))
            return;
        
        //create zip file from image folder
        string sourceDir = HttpContext.Current.Server.MapPath(dir);
        string dirName = Path.GetFileName(sourceDir);//strange to have GetFileName hier because it returns name of folder.
        string parentDir = Directory.GetParent(sourceDir).FullName;
        string targetFile = String.Format(ConfigurationManager.AppSettings["ZipTempDir"], dirName); //use tmp folder for zip file, that should avoid access rights problems for IIS directories
        if (File.Exists(targetFile))
            File.Delete(targetFile);
        System.IO.Compression.ZipFile.CreateFromDirectory(sourceDir, targetFile); //make zip file!
        
        //send created zip file to response, for user to download
        Response.ContentType = "application/octet-stream";
        Response.AppendHeader("Content-Disposition", String.Format("attachment; filename={0}.zip", dirName));
        Response.TransmitFile(targetFile);
        Response.End();
    }

    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        string dir = Request.QueryString["dir"];
        if (String.IsNullOrEmpty(dir))
        {
            //show a little bit of help
            Response.Write("?dir=xxxxxx");
            return;
        }

        string imagePath = HttpContext.Current.Server.MapPath(dir);
        string[] images = FindFilesFromDir(imagePath, new[] { ".jpg", ".jpeg", ".gif", ".tiff", ".bmp", ".png" });
        var pageTopArea = FindControl("Buttons");

        //for chief show list of available directories
        if (pageTopArea != null && IsChiefUser())
        {
            //for the chief we show a list of available directories, no one can memorize it all
            IEnumerable<string> imgDirs = FindImageDirs();
            if (imgDirs != null && imgDirs.Count() > 0)
            {
                foreach (string d in imgDirs)
                {
                    HyperLink h = new HyperLink();
                    h.NavigateUrl = String.Format("Show.aspx?dir={0}", d);
                    h.Text = d;
                    pageTopArea.Controls.Add(h);
                    pageTopArea.Controls.Add(new LiteralControl("<br />"));
                }
            }
        }

        //download button
        if (pageTopArea != null && images != null && images.Length > 0)
        {
            HyperLink downloadAllLink = new HyperLink();
            downloadAllLink.NavigateUrl = String.Format("Show.aspx?dir={0}&download=all", dir);
            downloadAllLink.Text = "Download All ZIP";
            pageTopArea.Controls.Add(downloadAllLink);
        }

        //for all users show images, if they know the correct link
        var pageMainArea = FindControl("Images");
        if (pageMainArea != null)
        {
            if (images != null && images.Length > 0)
            {
                foreach (string img in images)
                {
                    using (Image i = new Image())
                    {
                        i.ImageUrl = "ResizedImage?img=" + dir + "/" + img;
                        i.Style.Add(HtmlTextWriterStyle.Width, "100%");
                        pageMainArea.Controls.Add(i);
                    }
                }
            }
            else
            {
                pageMainArea.Controls.Add(new Label { Text = "No images found in given directory!" });
            }
        }
    }

    #region Helpers
    protected string[] FindFilesFromDir(string directoryWithImages, string[] extensions)
    {
        if (!Directory.Exists(directoryWithImages))
            return null;

        return Directory.GetFiles(directoryWithImages)
                        .Where(f => extensions.Contains(Path.GetExtension(f).ToLower()))
                        .Select(f => Path.GetFileName(f))
                        .ToArray();
    }

    private IEnumerable<string> FindImageDirs()
    {
        return Directory.GetDirectories(HttpContext.Current.Server.MapPath("~/"))
                        .Where(d => Path.GetFileName(d).StartsWith("20")) //image directories start with "20" for me, change this to detect your directories holding images
                        .Select(d => Path.GetFileName(d));
    }

    private bool IsChiefUser()
    {
        string user = Request.QueryString["user"];
        if (!String.IsNullOrEmpty(user) && user.Equals(ConfigurationManager.AppSettings["ChiefUser"]))
        {
            return true;
        }
        return false;
    }
    #endregion
}