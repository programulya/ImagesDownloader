using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Hangfire;
using HtmlAgilityPack;
using ImagesDownloader.Contracts;
using static System.String;
using System.Diagnostics;
using System.Drawing;

namespace ImagesDownloader.Services
{
    /// <summary>
    /// Download images service
    /// </summary>
    public class ImagesService : IImagesService
    {
        /// <summary>
        /// Get images by URL
        /// </summary>
        /// <param name="url">URL</param>
        [AutomaticRetry(Attempts = 0)]
        public void GetImagesByUrl(string url)
        {
            try
            {
                var imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                var randomFileName = Path.GetRandomFileName();
                var tempDirectory = Path.Combine(imagesFolder, randomFileName);

                Directory.CreateDirectory(tempDirectory);

                using (var client = new WebClient())
                {
                    var html = client.DownloadString(url);

                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    var imageNodes = doc.DocumentNode.SelectNodes("//img").Where(node => node.Name == "img").ToList();

                    foreach (var node in imageNodes)
                    {
                        if (node.Attributes.Contains("src"))
                        {
                            var src = node.Attributes["src"].Value;

                            if (!IsNullOrEmpty(src))
                            {
                                if (src.StartsWith("http"))
                                {
                                    var fileName = Guid.NewGuid() + Path.GetExtension(src);
                                    var outputFile = Path.Combine(tempDirectory, fileName);

                                    client.DownloadFile(src, outputFile);

                                    var size = new FileInfo(outputFile).Length;
                                    int width;
                                    int height;

                                    using (var stream = new FileStream(outputFile, FileMode.Open, FileAccess.Read))
                                    {
                                        var image = Image.FromStream(stream);
                                        width = image.Width;
                                        height = image.Height;
                                    }

                                    

                                    using (var context = new JobsEntities())
                                    {
                                        var imageInfo = new ImageInfo
                                        {
                                            ContentType = MimeMapping.GetMimeMapping(fileName),
                                            LocalUrl =
                                                "http://localhost/ImagesDownloader/Images/" + randomFileName + "/" +
                                                fileName,
                                            RemoteUrl = src,
                                            JobId = Convert.ToInt32(JobContext.JobId),
                                            Size = size,
                                            Height = width,
                                            Width = height
                                        };

                                        context.ImageInfoes.Add(imageInfo);
                                        context.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);

                throw;
            }
        }
    }
}