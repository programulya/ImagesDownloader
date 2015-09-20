﻿using System;
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
    public class ImagesService : IImagesService
    {
        [AutomaticRetry(Attempts = 0)]
        public void GetImagesByUrl(string url)
        {
            try
            {
                var imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                var randomFileName = Path.GetRandomFileName();
                var tempDirectory = Path.Combine(imagesFolder, randomFileName);

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
                                    var outputFile = Path.Combine(tempDirectory, fileName);

                                    client.DownloadFile(src, outputFile);

                                    // TODO: Move to database layer
                                    var size = new FileInfo(outputFile).Length;
                                    var image = Image.FromFile(outputFile);

                                    using (var context = new JobsEntities())
                                    {
                                        // TODO Add information
                                        var imageInfo = new ImageInfo
                                        {
                                            ContentType = MimeMapping.GetMimeMapping(fileName),
                                            LocalUrl =
                                                "http://localhost/ImagesDownloader/Images/" + randomFileName + "/" +
                                                fileName,
                                            RemoteUrl = src,
                                            JobId = Convert.ToInt32(JobContext.JobId),
                                            Size = size,
                                            Height = image.Height,
                                            Width = image.Height
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