using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;

namespace Helpers
{
    public class ImageHelpers
    {
        static List<string> urlList = new List<string>();
        public static string ImageFind(string html, string searchTextStart, string searchTextEnd)
        {

            int pos1 = 0;
            int pos2 = 0;
            string filePath = "/ReportImages/";
            string guidId = Guid.NewGuid().ToString();
            int imageCount = 4; //your images limit
            for (int index = 0; index < imageCount; index++)
            {
                try
                {
                    string imageName = guidId + index.ToString();
                    pos1 = html.IndexOf(searchTextStart, pos1 + 1);
                    if (pos1 == -1)
                        return html;
                    pos2 = html.IndexOf(searchTextEnd, pos1);
                    string image = html.Substring(pos1 + searchTextStart.Length, pos2 - pos1 - searchTextStart.Length);


                    //find extention
                    int ExPos1 = html.LastIndexOf(".", pos2);
                    int ExPos2 = html.IndexOf(searchTextEnd, ExPos1);
                    string format = ImageFindFormat(html, ExPos1, ExPos2 - ExPos1);
                    imageName = imageName + format;

                    string newImagePath = HttpContext.Current.Server.MapPath("~" + filePath + imageName);
                    string newImageUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + filePath + imageName;

                    bool imageDownloadSuccess = ImageDownload(image, newImagePath);

                    if (imageDownloadSuccess)
                        html = html.Replace(image, newImageUrl);

                    urlList.Add(newImagePath);
                }
                catch (Exception ex)
                {
                    //Add Log
                }

            }

            return html;

        }

        public static string ImageFindFormat(string html, int positionStart, int positionEnd)
        {
            string format = "";
            html.Substring(positionStart, positionEnd);
            if (format.Length > 4)
            {
                if (format.IndexOf("?") > -1)
                    format = format.Substring(0, format.IndexOf("?"));
                else if (format.IndexOf("&") > -1)
                    format = format.Substring(0, format.IndexOf("&"));
                else if (format.IndexOf(".", 2) > -1)
                    format = format.Substring(0, format.IndexOf(".", 2));
                else
                    format = ".jpg";
            }
            return format;
        }

        public static Boolean ImageDownload(string Url, string newImagePath)
        {
            bool result;
            try
            {
                if (Url.IndexOf("https://") == 0)
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (WebClient webClient = new WebClient())
                {
                    byte[] data = webClient.DownloadData(Url);
                    File.WriteAllBytes(@newImagePath, data);
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                //Add Log
            }
            return result;
        }
    }
}
