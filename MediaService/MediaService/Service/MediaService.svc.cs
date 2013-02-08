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
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;

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
        [WebGet(UriTemplate = "GetPhoto/{albumId}/{photoId}/{dimension}")]
        Stream GetPhotoResized(string albumId, string photoId, string dimension);
    }

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class MediaService : IMediaService
    {
        public AlbumDTO[] GetAlbums()
        {
            using (var db = new PhotosContext())
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
                using (var db = new PhotosContext())
                {
                    var photos = (from a in db.Albums//.Include(p => p.Photos)
                                  where a.Id == id
                                  select a.Photos.Where(p => p.Dimension == null)).FirstOrDefault();

                    if (photos != null)
                    {
                        var arrayPhotos = photos.Select(a => new PhotoDTO
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Comments = a.Comments,
                            Link = new Uri(string.Format(@"http://www.riscanet.com/Service/MediaService.svc/GetPhoto/{0}/{1}?apikey={2}",
                                                         albumId, a.Id, HttpContext.Current.Request.QueryString["apikey"]))
                        })
                            .ToArray();
                        return arrayPhotos;
                    }
                }
            }
            return null;
        }

        public Stream GetPhoto(string albumId, string photoId)
        {
            int albumid, photoid;
            if (int.TryParse(albumId, out albumid) && int.TryParse(photoId, out photoid))
            {
                using (var db = new PhotosContext())
                {
                    var photo = (from a in db.Albums//.Include(p => p.Photos)
                                 where a.Id == albumid
                                 select a.Photos.FirstOrDefault(p => p.Id == photoid && p.OriginalPhotoId == null)).FirstOrDefault();
                    if (photo != null)
                    {
                        using (new NetworkConnection())
                        {
                            var fullPath = System.IO.Path.Combine(photo.Path, photo.Name);
                            if (File.Exists(fullPath))
                            {
                                FileStream fs = File.OpenRead(fullPath);
                                WebOperationContext.Current.OutgoingResponse.ContentType = GetContentTypeFromFileName(photo.Name);
                                return fs;
                            }
                        }
                    }
                }
            }
            WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
            return null;
        }

        public Stream GetPhotoResized(string albumId, string photoId, string dimension)
        {
            int albumid, photoid;
            double size;
            if (int.TryParse(albumId, out albumid) && int.TryParse(photoId, out photoid) && double.TryParse(dimension, out size))
            {
                using (var db = new PhotosContext())
                {
                    var photos = (from a in db.Albums//.Include(p => p.Photos)                                 
                                  where a.Id == albumid
                                  select a.Photos).FirstOrDefault();
                    if (photos != null)
                    {
                        var photo = (from p in photos
                                     where p.Id == photoid && p.OriginalPhotoId == null && p.Dimension == null
                                     select p).FirstOrDefault();

                        if (photo != null)
                        {
                            if (photo.ResizedPhotos == null)
                                photo.ResizedPhotos = new List<Photo>();

                            using (new NetworkConnection())
                            {
                                var resizedPhoto = (from p in db.Photos
                                                    where p.OriginalPhotoId == photoid && p.Dimension.HasValue && p.Dimension.Value == size
                                                    select p).FirstOrDefault();

                                string fullPath = string.Empty;
                                if (resizedPhoto != null)
                                {
                                    fullPath = System.IO.Path.Combine(resizedPhoto.Path, resizedPhoto.Name);
                                    if (!File.Exists(fullPath))
                                    {
                                        var newPhotoPath = System.IO.Path.Combine(photo.Path, string.Format("{0}-{1}{2}", System.IO.Path.GetFileNameWithoutExtension(photo.Name), ((int)size).ToString(), System.IO.Path.GetExtension(photo.Name)));

                                        ImageResizer.ResizeImage(System.IO.Path.Combine(photo.Path, photo.Name), newPhotoPath, size, 50);
                                        photo.ResizedPhotos.Add(new Photo { Name = string.Format("{0}-{1}{2}", System.IO.Path.GetFileNameWithoutExtension(photo.Name), ((int)size).ToString(), System.IO.Path.GetExtension(photo.Name)), Path = photo.Path, Dimension = size });
                                        db.SaveChanges();
                                    }

                                }
                                else
                                {
                                    var newPhotoFileName = string.Format("{0}-{1}{2}", System.IO.Path.GetFileNameWithoutExtension(photo.Name), ((int)size).ToString(), System.IO.Path.GetExtension(photo.Name));
                                    fullPath = System.IO.Path.Combine(photo.Path, newPhotoFileName);
                                    ImageResizer.ResizeImage(System.IO.Path.Combine(photo.Path, photo.Name), fullPath, size, 50);
                                    photo.ResizedPhotos.Add(new Photo { Name = newPhotoFileName, Path = photo.Path, Dimension = size });
                                    db.SaveChanges();
                                }

                                FileStream fs = File.OpenRead(fullPath);
                                WebOperationContext.Current.OutgoingResponse.ContentType = GetContentTypeFromFileName(photo.Name);
                                return fs;
                            }
                        }
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