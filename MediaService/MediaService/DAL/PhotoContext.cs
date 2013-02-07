using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MediaService.DAL
{
    public class PhotoContext : DbContext
    {
        public PhotoContext()
        {
            Configuration.AutoDetectChangesEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Album> Albums { get; set; }
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

    public class Photo
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Comments { get; set; }
        public long? Length { get; set; }
        [Required]
        public string Path { get; set; }
        public int SizeX { get; set; }
        //[NotMapped, NonSerialized]
        //public string FullPath { get { return System.IO.Path.Combine(Path, Name); } }
    }

    [DataContract]
    public class PhotoDTO
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Comments { get; set; }
        [DataMember]
        public Uri Link { get; set; }
    }


    [DataContract]
    public class AlbumDTO
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Comments { get; set; }        
        [DataMember]
        public Uri Link { get; set; }
    }

}