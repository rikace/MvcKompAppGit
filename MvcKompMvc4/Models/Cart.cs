using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MvcKompApp.Models
{
    public class Cart
    {
        private List<CartLine> lineCollection = new List<CartLine>();

        public void AddItem(Product product, int quantity)
        {
            CartLine line = lineCollection
                .Where(p => p.Product.ProductID == product.ProductID)
                .FirstOrDefault();

            if (line == null)
            {
                lineCollection.Add(new CartLine { Product = product, Quantity = quantity });
            }
            else
            {
                line.Quantity += quantity;
            }
        }

        public void RemoveLine(Product product)
        {
            lineCollection.RemoveAll(l => l.Product.ProductID == product.ProductID);
        }

        public decimal ComputeTotalValue()
        {
            return lineCollection.Sum(e =>/*e.Product.Price*/8 * e.Quantity);

        }
        public void Clear()
        {
            lineCollection.Clear();
        }

        public IEnumerable<CartLine> Lines
        {
            get { return lineCollection; }
        }
    }

    public class CartLine
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}

//[MetadataType(typeof(ProductMetadata))]
//public partial class Product
//{
//}

public class ProductMetadata
{
    //[HiddenInput(DisplayValue = false)]
    //public int ProductID { get; set; }

    //[Required(ErrorMessage = "Please enter a product name")]
    //public string Name { get; set; }

    //[Required(ErrorMessage = "Please enter a description")]
    //[DataType(DataType.MultilineText)]
    //public string Description { get; set; }

    //[Required]
    //[Range(0.01, double.MaxValue, ErrorMessage = "Please enter a positive price")]
    //public decimal Price { get; set; }

    //[Required(ErrorMessage = "Please specify a category")]
    //public string Category { get; set; }

    //public byte[] ImageData { get; set; }

    //[HiddenInput(DisplayValue = false)]
    //public string ImageMimeType { get; set; }
}