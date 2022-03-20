using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Data.Context;
using ProductCatalog.Models;
using ProductCatalog.Services;

namespace ProductCatalog.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductService _productService;

        public ProductController(ApplicationDbContext context, ProductService productService)
        {
            _context = context;
            _productService = productService;
        }

        public IActionResult Index()
        {
            ViewData["Message"] = "Your product page.";

            var model = _productService.GetProducts();
            return View(model);
        }

        public IActionResult AddProduct()
        {
            ViewData["Message"] = "Your create product page.";

            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductDtoViewModel productDto)
        {
            _productService.AddProduct(productDto);

            return RedirectToAction(nameof(Index));
        }

        [HttpPut]
        public IActionResult Update(ProductDtoViewModel productDto)
        {
            _productService.UpdateProduct(productDto);

            return RedirectToAction(nameof(Index));
        }

        [HttpDelete]
        public IActionResult Delete(ProductDtoViewModel productDto)
        {
            _productService.DeleteProduct(productDto);

            return RedirectToAction(nameof(Index));
        }
    }
}
