using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MvcKompApp.Validation;

namespace MvcKompApp.ViewModels
{
    public class ProductViewModel
    {
        [Price(MinPrice = 3.98, ErrorMessage = "Price must end in .99 and be greater than 3.98")]
        public double Price { get; set; }

        [Required]
        [RegularExpression(@"(\S)+", ErrorMessage = "White space is not allowed")]
        public string Color { get; set; }

        [Required]
        [ScaffoldColumn(false)]
        public string Name { get; set; }
    }

    public static class ProductList
    {

        public static List<ProductViewModel> init()
        {
            _prodList.Add(new ProductViewModel
            {
                Price = 1.99,
                Color = "Red",
                Name = "Rabbit"
            });
            _prodList.Add(new ProductViewModel
            {
                Price = 11.99,
                Color = "Green",
                Name = "Goblin"
            });
            _prodList.Add(new ProductViewModel
            {
                Price = 5.99,
                Color = "Brown",
                Name = "Bear"
            });

            return _prodList;
        }


        public static List<ProductViewModel> _prodList = new List<ProductViewModel>();

        public static void Update(ProductViewModel umToUpdate)
        {

            foreach (ProductViewModel um in _prodList)
            {
                if (um.Name == umToUpdate.Name)
                {
                    _prodList.Remove(um);
                    _prodList.Add(umToUpdate);
                    break;
                }
            }
        }
    }

}