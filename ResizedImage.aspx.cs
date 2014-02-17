using System;
using System.Drawing;
using System.Web;

/*
 * This page will take image name from url,
 * load it from disk, and if the image is wider that 1024,
 * scale it down to width 1024, and serve it in response.
 * In this way we avoid sending too big images over
 * the Internet.
 */
public partial class ResizedImage : System.Web.UI.Page
{
    private int _maxWidth = int.Parse(System.Configuration.ConfigurationManager.AppSettings["MaxWidth"]);
    protected void Page_Load(object sender, EventArgs e)
    {
        string img = Request.QueryString["img"];
        if (String.IsNullOrEmpty(img))
        {
            Response.Write(img + "? <br />");
            return;
        }

        string imgFile = HttpContext.Current.Server.MapPath(img);
        using (System.Drawing.Image imageFromFile = System.Drawing.Image.FromFile(imgFile))
        {
            Size originalSize = imageFromFile.Size;
            if (originalSize.Width > _maxWidth)
            {
                float ratio = (float)_maxWidth / originalSize.Width;
                Size newSize = new Size(_maxWidth, (int)(originalSize.Height * ratio));

                using(System.Drawing.Image newScaledImage = (System.Drawing.Image)(new Bitmap(imageFromFile, newSize))) //image scaling
                {
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        newScaledImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        Response.ContentType = "image/jpeg";
                        ms.WriteTo(Response.OutputStream);
                    }
                }
            }
        }
    }
}