using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.IO;

namespace MediaService.DAL
{
    public class PhotosContextInitializer : DropCreateDatabaseIfModelChanges<PhotosContext>// DropCreateDatabaseIfModelChanges<PhotosContext>//  
    {
        protected override void Seed(PhotosContext context)
        {
            using (var ns = new NetworkConnection(@"\\GoFlex_Home\GoFlex Home Public", new System.Net.NetworkCredential("Riccardo", "Jocker74!")))
            {
                var dir1 = @"\\GoFlex_Home\GoFlex Home Public\Immagini\Bugghina\Halloween 2010";
                var dir2 = @"\\GoFlex_Home\GoFlex Home Public\Immagini\Bugghina\Puppy";

                var photos1 = Directory.GetFiles(dir1, "*.*", SearchOption.TopDirectoryOnly)
                              .Where(f => IsImage(Path.GetExtension(f)))
                              .Select(f => new FileInfo(f))
                              .Select(f => new Photo { Name = f.Name, Path = f.Directory.FullName, ResizedPhotos = new List<Photo>() })
                              .ToList();

                var album1 = new Album { Name = "Halloween 2010", Path = dir1, Photos = photos1 };

                context.Albums.Add(album1);
            }

            context.SaveChanges();
            base.Seed(context);
        }

        private bool IsImage(string extension)
        {
            extension = extension.ToLower();
            return extension.Equals(".jpeg") || extension.Equals(".jpg") ||
                    extension.Equals(".gif") || extension.Equals(".png") ||
                     extension.Equals(".tiff") || extension.Equals(".bmp");
        }
    }
}