using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MvcKompApp.Models
{
    [Table("Image")]
    public class ImageModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ImageID { get; set; }
         [Column("ImageName")]
        public string ImageName { get; set; }
         [Column("Description")]
         public string Description { get; set; }
    }
}