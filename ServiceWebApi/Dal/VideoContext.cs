using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;

namespace ServiceWebApi.Dal
{
    public class VideoContext : DbContext
    {
        public VideoContext()
            : base("name=VideoContext")
        { }

        public DbSet<Video> Videos { get; set; }
    }

    public class Video
    {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public long? Size { get; set; }
        [NotMapped]
        public Uri Link { get; set; }
    }

    public class VideoContextInit : DropCreateDatabaseAlways<VideoContext>
    {
        protected override void Seed(VideoContext context)
        {
            string path = @"\\GoFlex_home\GoFlex Home Public\VideoTech";
            var videos = System.IO.Directory.EnumerateFiles(path, "*.mp4", System.IO.SearchOption.AllDirectories);

            foreach (var video in videos)
            {
                context.Videos.Add(new Video
                {
                    Name = Path.GetFileNameWithoutExtension(video),
                    Path = video,
                    Size = new FileInfo(video).Length
                });
            }
            context.SaveChanges();

            base.Seed(context);
        }
    }

    public static class VideoContextHelper
    {
        public static void UpdateVideos()
        {
            using (var context = new VideoContext())
            {
                var videosCtx = context.Videos.AsEnumerable();

                string path = @"\\GoFlex_home\GoFlex Home Public\VideoTech";
                var videos = System.IO.Directory.EnumerateFiles(path, "*.mp4", System.IO.SearchOption.AllDirectories).Select(video => new Video
                    {
                        Name = Path.GetFileNameWithoutExtension(video),
                        Path = video,
                        Size = new FileInfo(video).Length
                    });
              

                foreach (var video in videos.Except(videosCtx, new VideoComparer()))
                {
                    context.Videos.Add(new Video
                    {
                        Name = video.Name,
                        Path = video.Path,
                        Size = video.Size
                    });
                }
                context.SaveChanges();
            }
        }
    }
}