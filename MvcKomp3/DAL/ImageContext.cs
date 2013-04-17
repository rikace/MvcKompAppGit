using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MvcKompApp.Models;

namespace MvcKompApp.DAL
{
    public class ImageContext : DbContext
    {
        public DbSet<ImageModel> Images { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();
        //}
    }

    public class ImageInitializer : DropCreateDatabaseIfModelChanges<ImageContext>
    {
        //protected override void Seed(ImageContext context)
        //{
        //    var Images = new List<ImageModel>
        //    {
        //    new ImageModel { ImageID =0, Description = "Image Description -1", ImageName = "Image-1"},
        //    new ImageModel {  ImageID=1, Description = "Image Description -3", ImageName = "Image-3"},
        //    new ImageModel { ImageID=3, Description = "Image Description -2", ImageName = "Image-2"}
        //    };
        //    Images.ForEach(s => context.Images.Add(s));
        //    context.SaveChanges();
        //}
    }
}