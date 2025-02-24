﻿using BestStoreMVC.Models;
using BestStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BestStoreMVC.Controllers
{
	public class ProductsController : Controller
	{
		private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

		public ProductsController(ApplicationDbContext context,IWebHostEnvironment environment)
        {
			this.context = context;
            this.environment = environment;
		}
        public IActionResult Index()
		{
            var products = this.context.Products.ToList();
            return View(products);
            
        }

		// Create
		public IActionResult Create()
		{
			return View();
		}

        [HttpPost]
        public IActionResult Create(ProductDto productDto,IFormFile ImageFile)
        {
            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The Image File is Required");
            }
            if (!ModelState.IsValid)
            {
                return View(productDto);
            }
			//save the imgfile  
			string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
			newFileName += Path.GetExtension(productDto.ImageFile!.FileName);
			string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
			using (var stream = System.IO.File.Create(imageFullPath))
			{
				productDto.ImageFile.CopyTo(stream);
			}

			// save the new product in the database
        Product product = new Product()
         {
            Name = productDto.Name,
            Brand = productDto.Brand,
            Category = productDto.Category,
            Price = productDto.Price,
            Description = productDto.Description,
            ImageFileName = newFileName,
            CreatedAt = DateTime.Now
           };
             context.Products.Add(product);
            context.SaveChanges();

			return RedirectToAction("Index","products");
		}
    }
}
