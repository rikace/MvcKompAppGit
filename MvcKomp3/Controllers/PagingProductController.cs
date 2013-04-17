﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Product = MvcKompApp.Models.ProductModel;
using MvcAjaxPaging;

namespace MvcKompApp.Controllers
{
    public class PagingProductController : Controller
    {
       private const int defaultPageSize = 10;
		private IList<Product> allProducts = new List<Product>();
		private string[] categories = new string[3] {"Shoes", "Electronics", "Food"};

        public PagingProductController()
		{
			InitializeProducts();
		}

		private void InitializeProducts()
		{
			// Create a list of 500 products. 250 are in the Shoes category, 125 in the Electronics 
			// category and 125 in the Food category.
			for (int i = 0; i < 500; i++)
			{
				var product = new Product();
				product.Name = "Product " + (i + 1);
				int categoryIndex = i%4;
				if (categoryIndex > 2)
				{
					categoryIndex = categoryIndex - 3;
				}
				product.Category = categories[categoryIndex];
				allProducts.Add(product);
			}          
		}

		public ActionResult Index(int? page)
		{
			ViewData["Title"] = "Browse all products";
			int currentPageIndex = page.HasValue ? page.Value : 1;

           var productList = this.allProducts.Skip(currentPageIndex * defaultPageSize).Take(defaultPageSize).ToList().ToPagedList(currentPageIndex, defaultPageSize, allProducts.Count);

           return View(productList);
			//return View(this.allProducts.ToPagedList(currentPageIndex, defaultPageSize));
		}

		public ActionResult ViewByCategory(string categoryName, int? page)
		{
			ViewData["Title"] = "Browse products by category";
			categoryName = categoryName ?? this.categories[0];
			int currentPageIndex = page.HasValue ? page.Value : 1;

            var productsByCategory = this.allProducts.Where(p => p.Category.Equals(categoryName)).Skip(currentPageIndex * defaultPageSize).Take(defaultPageSize).ToList().ToPagedList(currentPageIndex, defaultPageSize, allProducts.Count);

            //var productsByCategory = this.allProducts.Where(p => p.Category.Equals(categoryName)).ToPagedList(currentPageIndex,defaultPageSize);


			ViewData["CategoryName"] = new SelectList(this.categories, categoryName);
			ViewData["CategoryDisplayName"] = categoryName;
            return View("ViewByCategory", productsByCategory);
		}


        public ActionResult IndexAjax(int? page)
        {
            ViewData["Title"] = "Browse all products";
            int currentPageIndex = page.HasValue ? page.Value : 1;
            //    var list = this.allProducts.ToPagedList(currentPageIndex, defaultPageSize);

            var list = this.allProducts.Skip(currentPageIndex * defaultPageSize).Take(defaultPageSize).ToList().ToPagedList(currentPageIndex, defaultPageSize, allProducts.Count);

            if (Request.IsAjaxRequest())
                return PartialView("_ListControl", list);
            else
                return View(list);
        }

        public ActionResult ViewByCategoryAjax(string categoryName, int? page)
        {
            ViewData["Title"] = "Browse products by category";
            categoryName = categoryName ?? this.categories[0];
            int currentPageIndex = page.HasValue ? page.Value : 1;

            var productsByCategory = this.allProducts.Where(p => p.Category.Equals(categoryName)).Skip(currentPageIndex * defaultPageSize).Take(defaultPageSize).ToList().ToPagedList(currentPageIndex, defaultPageSize, allProducts.Count);

            //var productsByCategory = this.allProducts.Where(p => p.Category.Equals(categoryName)).ToPagedList(currentPageIndex,                                                                                            defaultPageSize);
            ViewData["CategoryName"] = new SelectList(this.categories, categoryName);
            ViewData["CategoryDisplayName"] = categoryName;

            if (Request.IsAjaxRequest())
                return PartialView("_ListControlByCategory", productsByCategory);
            else
                return View("ViewByCategoryAjax", productsByCategory);
        }
	}
}