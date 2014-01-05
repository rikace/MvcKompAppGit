using ServiceWebApi.Dal;
using ServiceWebApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace ServiceWebApi.Controllers
{
    public class AlbumsController : ApiController
    {
        // GET api/albums
        public IEnumerable<AlbumDTO> Get()
        {
            using (var db = new PhotosContext())
                return db.Albums.ToList().Select(a => new AlbumDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    Comments = a.Comments,
                    //Link = new Uri(string.Format(@"http://www.riscanet.com/Service/MediaService.svc/GetPhotos/{0}?apikey={1}",
                    Link = new Uri(string.Format(@"http://www.riscanet.com/Api/Albums/{0}?apikey={1}",
                    a.Id, HttpContext.Current.Request.QueryString["apikey"]))
                });
        }

        // GET api/albums/5
        public IEnumerable<PhotoDTO> Get(int id)
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
                        Link = new Uri(string.Format(@"http://www.riscanet.com/Api/Albums/{0}?photoid={1}&apikey={2}",
                                                  id, a.Id, HttpContext.Current.Request.QueryString["apikey"]))
                        //Link = new Uri(string.Format(@"http://www.riscanet.com/Service/MediaService.svc/GetPhoto/{0}/{1}?apikey={2}",
                        //                             id, a.Id, HttpContext.Current.Request.QueryString["apikey"]))
                    })
                        .ToArray();
                    return arrayPhotos;
                }
            }
            return null;
        }

        public HttpResponseMessage Get(int id, int photoId, int dimension = 0)
        {
            using (var db = new PhotosContext())
            {
                var response = Request.CreateResponse();
                if (dimension == 0)
                {
                    var photo = (from a in db.Albums//.Include(p => p.Photos)
                                 where a.Id == id
                                 select a.Photos.FirstOrDefault(p => p.Id == photoId && p.OriginalPhotoId == null)).FirstOrDefault();
                    if (photo != null)
                    {
                        using (new NetworkConnection())
                        {
                            var fullPath = System.IO.Path.Combine(photo.Path, photo.Name);
                            var mediaStreaming = new MediaStreaming(fullPath);
                            response.Content = new PushStreamContent(mediaStreaming.WriteToStream, new MediaTypeHeaderValue(MediaContentHelper.GetContentTypeFromFileName(photo.FullPath)));

                        }
                    }
                }
                else
                {
                    var photos = (from a in db.Albums//.Include(p => p.Photos)                                 
                                  where a.Id == id
                                  select a.Photos).FirstOrDefault();
                    if (photos != null)
                    {
                        var photo = (from p in photos
                                     where p.Id == photoId && p.OriginalPhotoId == null && p.Dimension == null
                                     select p).FirstOrDefault();

                        if (photo != null)
                        {
                            if (photo.ResizedPhotos == null)
                                photo.ResizedPhotos = new List<Photo>();

                            using (new NetworkConnection())
                            {
                                var resizedPhoto = (from p in db.Photos
                                                    where p.OriginalPhotoId == photoId && p.Dimension.HasValue && p.Dimension.Value == dimension
                                                    select p).FirstOrDefault();

                                string fullPath = string.Empty;
                                if (resizedPhoto != null)
                                {
                                    fullPath = System.IO.Path.Combine(resizedPhoto.Path, resizedPhoto.Name);
                                    if (!File.Exists(fullPath))
                                    {
                                        var newPhotoPath = System.IO.Path.Combine(photo.Path, string.Format("{0}-{1}{2}", System.IO.Path.GetFileNameWithoutExtension(photo.Name), ((int)dimension).ToString(), System.IO.Path.GetExtension(photo.Name)));

                                        ImageResizer.ResizeImage(System.IO.Path.Combine(photo.Path, photo.Name), newPhotoPath, dimension, 50);
                                        photo.ResizedPhotos.Add(new Photo { Name = string.Format("{0}-{1}{2}", System.IO.Path.GetFileNameWithoutExtension(photo.Name), dimension.ToString(), System.IO.Path.GetExtension(photo.Name)), Path = photo.Path, Dimension = dimension });
                                        db.SaveChanges();
                                    }
                                }
                                else
                                {
                                    var newPhotoFileName = string.Format("{0}-{1}{2}", System.IO.Path.GetFileNameWithoutExtension(photo.Name), dimension.ToString(), System.IO.Path.GetExtension(photo.Name));
                                    fullPath = System.IO.Path.Combine(photo.Path, newPhotoFileName);
                                    ImageResizer.ResizeImage(System.IO.Path.Combine(photo.Path, photo.Name), fullPath, dimension, 50);
                                    photo.ResizedPhotos.Add(new Photo { Name = newPhotoFileName, Path = photo.Path, Dimension = dimension });
                                    db.SaveChanges();
                                }
                                var mediaStreaming = new MediaStreaming(fullPath);
                                response.Content = new PushStreamContent(mediaStreaming.WriteToStream, new MediaTypeHeaderValue(MediaContentHelper.GetContentTypeFromFileName(photo.FullPath)));
                            }
                        }
                    }
                }

                return response;
            }
        }
    }
}