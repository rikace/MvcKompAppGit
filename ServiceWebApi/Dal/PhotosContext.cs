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