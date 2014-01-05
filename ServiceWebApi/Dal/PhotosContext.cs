using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ServiceWebApi.Dal
{
    //public static class PhotoContextHelper
    //{
    //    public void UpdatePhotos()
    //    {
    //        using (var ns = new NetworkConnection(@"\\GoFlex_Home\GoFlex Home Public", new System.Net.NetworkCredential("Riccardo", "Jocker74!")))
    //        {
    //            var dir1 = @"\\GoFlex_Home\GoFlex Home Public\Immagini\Bugghina\Halloween 2010";
    //            var dir2 = @"\\GoFlex_Home\GoFlex Home Public\Immagini\Bugghina\Puppy";

    //            var photos1 = Directory.GetFiles(dir1, "*.*", SearchOption.TopDirectoryOnly)
    //                          .Where(f => IsImage(Path.GetExtension(f)))
    //                          .Select(f => new FileInfo(f))
    //                          .Select(f => new Photo { Name = f.Name, Path = f.Directory.FullName, ResizedPhotos = new List<Photo>() })
    //                          .ToList();

    //            var album1 = new Album { Name = "Halloween 2010", Path = dir1, Photos = photos1 };

    //            context.Albums.Add(album1);
    //        }

    //        using (var context = new PhotosContext())
    //        {
    //             using (var ns = new NetworkConnection(@"\\GoFlex_Home\GoFlex Home Public", new System.Net.NetworkCredential("Riccardo", "Jocker74!")))
    //             {
    //            var videosCtx = context..AsEnumerable();

    //            string path = @"\\GoFlex_home\GoFlex Home Public\VideoTech";
    //            var videos = System.IO.Directory.EnumerateFiles(path, "*.mp4", System.IO.SearchOption.AllDirectories).Select(video => new Video
    //                {
    //                    Name = Path.GetFileNameWithoutExtension(video),
    //                    Path = video,
    //                    Size = new FileInfo(video).Length
    //                });
              

    //            foreach (var video in videos.Except(videosCtx, new VideoComparer()))
    //            {
    //                context.Videos.Add(new Video
    //                {
    //                    Name = video.Name,
    //                    Path = video.Path,
    //                    Size = video.Size
    //                });
    //            }
    //            context.SaveChanges();
    //        }
    //        }
    //    }
    //    }

    
    public class PhotosContext : DbContext
    {
        public PhotosContext()
            : base("PhotosContextConnectionString")
        {
            Configuration.AutoDetectChangesEnabled = true;
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Album> Albums { get; set; }
        public DbSet<Photo> Photos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Photo>()
             .HasOptional(e => e.OriginalPhoto)
             .WithMany(e => e.ResizedPhotos)
             .HasForeignKey(e => e.OriginalPhotoId)
             .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }

    public class Photo
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Comments { get; set; }
        [Required]
        public string Path { get; set; }
        public double? Dimension { get; set; }
        public int? OriginalPhotoId { get; set; }
        public Photo OriginalPhoto { get; set; }
        public virtual ICollection<Photo> ResizedPhotos { get; set; }

        [NotMapped]
        public string FullPath { get { return System.IO.Path.Combine(Path, Name); } }
    }

    public class Album
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Comments { get; set; }
        [Required]
        public string Path { get; set; }
        public List<Photo> Photos { get; set; }
    }

    public class PhotoDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public Uri Link { get; set; }
    }

    public class AlbumDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public Uri Link { get; set; }
    }
}