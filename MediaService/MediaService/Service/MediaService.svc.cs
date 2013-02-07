using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Web;
using System.Data.Entity;
using MediaService.DAL;

namespace MediaService.Service
{
    [ServiceContract]
    public interface IMediaService
    {
        [OperationContract]
        [WebGet(UriTemplate = "GetAlbums")]
        AlbumDTO[] GetAlbums();

        [OperationContract]
        [WebGet(UriTemplate = "GetPhotos/{albumId}")]
        PhotoDTO[] GetPhotos(string albumId);

        [OperationContract]
        [WebGet(UriTemplate = "GetPhoto/{albumId}/{photoId}")]
        Stream GetPhoto(string albumId, string photoId);

        [OperationContract]
        [WebGet(UriTemplate = "GetPhoto/{albumId}/{photoId}")]
        Stream GetPhotoResized(string albumId, string photoId, string size);
    }

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class MediaService : IMediaService
    {
        public MediaService()
        {
#if DEBUG
            //Database.SetInitializer(new PhotoAlbumDatabaseInitializer());
#endif
        }

        public AlbumDTO[] GetAlbums()
        {
            using (var db = new PhotoContext())
                return db.Albums.ToList().Select(a => new AlbumDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    Comments = a.Comments,
                    Link = new Uri(string.Format(@"http://www.riscanet.com/Service/MediaService.svc/GetPhotos/{0}?apikey={1}",
                    a.Id, HttpContext.Current.Request.QueryString["apikey"]))
                })
                    .ToArray();
        }

        public PhotoDTO[] GetPhotos(string albumId)
        {
            int id;
            if (int.TryParse(albumId, out id))
            {
                using (var db = new PhotoContext())
                {
                    var photos = (from a in db.Albums//.Include(p => p.Photos)
                                  where a.Id == id
                                  select a.Photos.Where(p => p.SizeX == null)).FirstOrDefault();
                    if (photos != null)
                        return photos.Select(a => new PhotoDTO
                            {
                                Id = a.Id,
                                Name = a.Name,
                                Comments = a.Comments,
                                Link = new Uri(string.Format(@"http://www.riscanet.com/Service/MediaService.svc/GetPhoto/{0}/{1}?apikey={2}",
                                                             albumId, a.Id, HttpContext.Current.Request.QueryString["apikey"]))
                            })
                                .ToArray();
                }
            }
            return null;
        }

        public Stream GetPhoto(string albumId, string photoId)
        {
            int albumid, photoid;
            if (int.TryParse(albumId, out albumid) && int.TryParse(photoId, out photoid))
            {
                using (var db = new PhotoContext())
                {
                    var photo = (from a in db.Albums//.Include(p => p.Photos)
                                 where a.Id == albumid
                                 select a.Photos.FirstOrDefault(p => p.Id == photoid && p.SizeX == null)).FirstOrDefault();
                    var fullPath = System.IO.Path.Combine(photo.Path, photo.Name);
                    if (File.Exists(fullPath))
                    {
                        FileStream fs = File.OpenRead(fullPath);
                        WebOperationContext.Current.OutgoingResponse.ContentType = GetContentTypeFromFileName(photo.Name);
                        return fs;
                    }
                }
            }
            WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
            return null;
        }

        public Stream GetPhotoResized(string albumId, string photoId, string size)
        {
            int albumid, photoid;
            if (int.TryParse(albumId, out albumid) && int.TryParse(photoId, out photoid))
            {
                using (var db = new PhotoContext())
                {
                    var photo = (from a in db.Albums//.Include(p => p.Photos)
                                 where a.Id == albumid
                                 select a.Photos.FirstOrDefault(p => p.Id == photoid)).FirstOrDefault();
                    var fullPath = System.IO.Path.Combine(photo.Path, photo.Name);
                    if (File.Exists(fullPath))
                    {
                        FileStream fs = File.OpenRead(fullPath);
                        WebOperationContext.Current.OutgoingResponse.ContentType = GetContentTypeFromFileName(photo.Name);
                        return fs;
                    }
                }
            }
            WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
            return null;
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
}