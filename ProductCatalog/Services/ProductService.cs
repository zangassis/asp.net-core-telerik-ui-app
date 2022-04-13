using ProductCatalog.Data.Context;
using ProductCatalog.Helpers;
using ProductCatalog.Models;

namespace ProductCatalog.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<ProductDtoViewModel> GetProducts()
        {
            var products = _dbContext.Products.ToList();
            var categories = _dbContext.Categories.ToList();
            var brands = _dbContext.Brands.ToList();
            var prices = _dbContext.ProductPrices.ToList();
            var productsDto = new List<ProductDtoViewModel>();

            foreach (var product in products)
            {
                var productCategory = categories.Where(p => p.ProductId == product.Id).FirstOrDefault().Name;
                var productBrand = brands.FirstOrDefault(b => b.Id == product.BrandId).Name;
                var productPrice = prices.FirstOrDefault(x => x.ProductId == product.Id).Price;

                var productDto = new ProductDtoViewModel()
                {
                    Id = product.Id,
                    Displayname = product.Displayname,
                    CreationDate = product.CreationDate,
                    LastUpdateDate = product.LastUpdateDate,
                    Price = productPrice,
                    Active = product.Active,
                    Brand = productBrand,
                    Category = productCategory
                };

                productsDto.Add(productDto);
            }
            return productsDto;
        }

        public IEnumerable<ProductDtoViewModel> Read() => GetAll();

        public IList<ProductDtoViewModel> GetAll()
        {
            var result = _dbContext.Session.GetObjectFromJson<IList<ProductDtoViewModel>>("Products");

            if (result == null)
            {
                var products = _dbContext.Products.ToList();
                var categories = _dbContext.Categories.ToList();
                var brands = _dbContext.Brands.ToList();
                var prices = _dbContext.ProductPrices.ToList();

                result = products.Select(product =>
                      {
                          var productCategory = categories.FirstOrDefault(p => p.ProductId == product.Id).Name;
                          var productBrand = brands.FirstOrDefault(b => b.Id == product.BrandId).Name;
                          var productPrice = prices.FirstOrDefault(x => x.ProductId == product.Id).Price;

                          return new ProductDtoViewModel
                          {
                              Id = product.Id,
                              Displayname = product.Displayname,
                              CreationDate = product.CreationDate,
                              LastUpdateDate = product.LastUpdateDate,
                              Price = productPrice,
                              Active = product.Active,
                              Brand = productBrand,
                              Category = productCategory
                          };
                      }).ToList();
                _dbContext.Session.SetObjectAsJson("Products", result);
            }
            return result;
        }

        public ProductDtoViewModel Create(ProductDtoViewModel productDto)
        {
            var entity = new ProductViewModel();
            Guid id = Guid.NewGuid();

            entity.Id = id;
            entity.Displayname = productDto.Displayname;
            entity.Active = true;
            entity.Price = new ProductPriceViewModel() { Id = Guid.NewGuid(), Price = productDto.Price, ProductId = id };
            entity.CreationDate = DateTime.UtcNow;
            entity.LastUpdateDate = DateTime.UtcNow;
            entity.Brand = new BrandViewModel() { Id = Guid.NewGuid(), Name = productDto.Brand };
            entity.Category = new CategoryViewModel() { Id = Guid.NewGuid(), Name = productDto.Category, ProductId = id };

            _dbContext.Products.Add(entity);
            _dbContext.SaveChanges();

            var products = GetAll();

            productDto.Id = id;
            productDto.Active = true;

            products.Insert(0, productDto);

            _dbContext.Session.SetObjectAsJson("Products", products);

            return productDto;
        }

        public void Update(ProductDtoViewModel productDto)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.Id == productDto.Id);
            var brand = _dbContext.Brands.FirstOrDefault(b => b.Id == product.BrandId);
            var category = _dbContext.Categories.FirstOrDefault(c => c.ProductId == productDto.Id);
            var price = _dbContext.ProductPrices.FirstOrDefault(x => x.ProductId == productDto.Id);

            var brandEntity = new BrandViewModel() { Id = product.BrandId, Name = productDto.Brand };
            var categoryEntity = new CategoryViewModel() { Id = category.Id, Name = productDto.Category, ProductId = productDto.Id };
            var priceEntity = new ProductPriceViewModel() { Id = price.Id, Price = productDto.Price, ProductId = productDto.Id };
            var productEntity = new ProductViewModel();

            productEntity.Id = productDto.Id;
            productEntity.Displayname = productDto.Displayname;
            productEntity.Active = productDto.Active;
            productEntity.Price = priceEntity;
            productEntity.CreationDate = product.CreationDate;
            productEntity.LastUpdateDate = DateTime.UtcNow;
            productEntity.Brand = brandEntity;
            productEntity.Category = categoryEntity;
            productEntity.BrandId = brand.Id;

            _dbContext.Entry(brand).CurrentValues.SetValues(brandEntity);
            _dbContext.Entry(category).CurrentValues.SetValues(categoryEntity);
            _dbContext.Entry(price).CurrentValues.SetValues(priceEntity);
            _dbContext.Entry(product).CurrentValues.SetValues(productEntity);

            _dbContext.SaveChanges();

            var products = GetAll();
            var target = products.FirstOrDefault(e => e.Id == productDto.Id);

            if (target != null)
            {
                target.Displayname = productDto.Displayname;
                target.Active = productDto.Active;
                target.Price = productDto.Price;
                target.CreationDate = productDto.CreationDate;
                target.LastUpdateDate = productDto.LastUpdateDate;
                target.Brand = productDto.Brand;
                target.Category = productDto.Category;
            }

            _dbContext.Session.SetObjectAsJson("Products", products);
        }

        public void Destroy(ProductDtoViewModel productDto)
        {
            var productEntity = _dbContext.Products.FirstOrDefault(p => p.Id == productDto.Id);
            var brandEntity = _dbContext.Brands.FirstOrDefault(b => b.Id == productEntity.BrandId);
            var categoryEntity = _dbContext.Categories.FirstOrDefault(c => c.ProductId == productDto.Id);
            var priceEntiy = _dbContext.ProductPrices.FirstOrDefault(p => p.ProductId == productDto.Id);

            _dbContext.Brands.Remove(brandEntity);
            _dbContext.Categories.Remove(categoryEntity);
            _dbContext.ProductPrices.Remove(priceEntiy);
            _dbContext.Products.Remove(productEntity);

            _dbContext.SaveChanges();

            var products = GetAll();
            var target = products.FirstOrDefault(e => e.Id == productDto.Id);

            if (target is not null)
            {
                products.Remove(target);
                _dbContext.Session.SetObjectAsJson("Products", products);
            }
        }
    }
}
