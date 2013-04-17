﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities;

namespace MvcKompApp.Models
{
    public interface IProductRepository
    {

        IQueryable<Product> Products { get; }

        void SaveProduct(Product product);

        void DeleteProduct(Product product);
    }
}