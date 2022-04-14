using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            return View();
        }

        public IActionResult AddProduct()
        {
            ViewData["Message"] = "Your create product page.";

            return View();
        }

        [HttpGet]
        public DataSourceResult Get([DataSourceRequest] DataSourceRequest request)
        {
            return _productService.Read().ToDataSourceResult(request);
        }

        [HttpPost]
        public IActionResult AddProduct(ProductDtoViewModel productDto)
        {
            if (!ModelState.IsValid)
                return View(productDto);
            else
            {
                _productService.Create(productDto);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public IActionResult Create(ProductDtoViewModel productDto)
        {
            ModelState.Remove("CreationDate");
            ModelState.Remove("LastUpdateDate");
            ModelState.Remove("Id");

            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(v => v.Errors).Select(error => error.ErrorMessage));

            var newProduct = _productService.Create(productDto);

            return new ObjectResult(new DataSourceResult { Data = new[] { newProduct }, Total = 1 });
        }

        [HttpPut]
        public IActionResult Update(Guid id, ProductDtoViewModel productDto)
        {
            ModelState.Remove("CreationDate");
            ModelState.Remove("LastUpdateDate");
            ModelState.Remove("Id");
        
            if (ModelState.IsValid && id == productDto.Id)
            {
                try
                {
                    _productService.Update(productDto);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return new NotFoundResult();
                }

                return new StatusCodeResult(200);
            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(v => v.Errors).Select(error => error.ErrorMessage));
            }
        }

        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _productService.Destroy(new ProductDtoViewModel { Id = id });
            }
            catch (DbUpdateConcurrencyException)
            {
                return new NotFoundResult();
            }

            return new StatusCodeResult(200);
        }
    }
}
