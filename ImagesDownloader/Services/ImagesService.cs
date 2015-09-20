using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Hangfire;
using HtmlAgilityPack;
using ImagesDownloader.Contracts;
using static System.String;

namespace ImagesDownloader.Services
{
    public class ImagesService : IImagesService
    {
        [AutomaticRetry(Attempts = 2)]
        public void GetImagesByUrl(string url)
        {
            //var imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
            var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            //var securityRules = new DirectorySecurity();
            //securityRules.AddAccessRule(new FileSystemAccessRule(@"Domain\account1", FileSystemRights.Read, AccessControlType.Allow));
            //securityRules.AddAccessRule(new FileSystemAccessRule(@"Domain\account2", FileSystemRights.FullControl, AccessControlType.Allow));

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
                            // base64
                            // svg
                            if (!src.StartsWith("data"))
                            {
                                var fileName = Guid.NewGuid() + Path.GetExtension(src);

                                client.DownloadFile(src, Path.Combine(tempDirectory, fileName));

                                // TODO: Move to database layer
                                using (var context = new JobsEntities())
                                {
                                    // TODO Add information
                                    var imageInfo = new ImageInfo
                                    {
                                        ContentType = MimeMapping.GetMimeMapping(fileName),
                                        LocalUrl = Empty,
                                        RemoteUrl = src,
                                        JobId = Convert.ToInt32(JobContext.JobId)
                                    };

                                    //imageInfo.Size = 0;
                                    //imageInfo.Width = 0;
                                    //imageInfo.Height = 0;

                                    context.ImageInfoes.Add(imageInfo);
                                    context.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}