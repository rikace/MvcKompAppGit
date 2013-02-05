﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Web;

namespace MediaService.Service
{
    [ServiceContract]
    public interface IMediaService
    {
        [OperationContract]
        [WebGet(UriTemplate = "GetImages")]
        ImageObj[] GetImages();

        [OperationContract]
        [WebGet(UriTemplate = "GetImage/{fileName}")]
        Stream GetImage(string fileName);
    }

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class MediaService : IMediaService
    {
        public ImageObj[] GetImages()
        {
            string dir = Properties.Settings.Default.Folder;
            //return Directory.GetFiles(dir, "*.*", SearchOption.TopDirectoryOnly)
            //    .Where(f => IsImage(Path.GetExtension(f).ToLower())).Select(f => Path.GetFileNameWithoutExtension(f))
            //    .Select(s => new ImageObj{ Name = s, Link = new Uri(string.Format(@"{0}/GetImage/{1}?apikey={2}", OperationContext.Current.Channel.LocalAddress, s, HttpContext.Current.Request.QueryString["apikey"]))})
            //    .ToArray();

            return Directory.GetFiles(dir, "*.*", SearchOption.TopDirectoryOnly)
               .Where(f => IsImage(Path.GetExtension(f).ToLower()))
               .Select(f => Path.GetFileNameWithoutExtension(f))
               .Select(s => new ImageObj
               {
                   Name = s,
                   Link = new Uri(string.Format(@"{0}/GetImage/{1}?apikey={2}",
                              "http://www.riscanet.com/Service/MediaService.svc", s, HttpContext.Current.Request.QueryString["apikey"]))
               })
               .ToArray();
        }

        public Stream GetImage(string fileName)
        {
            string dir = Properties.Settings.Default.Folder;
            var image = Directory.GetFiles(dir, fileName + ".*", SearchOption.TopDirectoryOnly)
                .Where(f => IsImage(Path.GetExtension(f).ToLower())).FirstOrDefault();

            string file = Path.Combine(Properties.Settings.Default.Folder, image);
            if (File.Exists(file))
            {
                FileStream fs = File.OpenRead(file);
                WebOperationContext.Current.OutgoingResponse.ContentType = GetContentTypeFromFileName(file);
                return fs;
            }
            WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
            return null;
        }

        private bool IsImage(string extension)
        {
            return extension.Equals(".jpeg") || extension.Equals(".jpg") ||
                    extension.Equals(".gif") || extension.Equals(".png") ||
                     extension.Equals(".tiff") || extension.Equals(".bmp");
        }

        private static string GetContentTypeFromFileName(string fileName)
        {
            // Return the Content-Type value based on the file extension.
            switch (Path.GetExtension(fileName).ToLower())
            {
                case ".jpeg":
                    return "image/jpeg";
                case ".jpg":
                    return "image/jpeg";
                case ".gif":
                    return "image/gif";
                case ".png":
                    return "image/png";
                case ".tiff":
                    return "image/tiff";
                case ".bmp":
                    return "image/bmp";
                default:
                    return "application/octet-stream";
            }

        }
    }

    [DataContract]
    public class ImageObj
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Uri Link { get; set; }
    }

}