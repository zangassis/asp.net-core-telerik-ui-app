using Microsoft.EntityFrameworkCore;
using ProductCatalog.Models;

namespace ProductCatalog.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ISession _session;

        public ApplicationDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _session = httpContextAccessor.HttpContext.Session;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("DataSource = productCatalog; Cache=Shared");

        public DbSet<ProductViewModel> Products { get; set; }
        public DbSet<ProductPriceViewModel> ProductPrices { get; set; }
        public DbSet<CategoryViewModel> Categories { get; set; }
        public DbSet<BrandViewModel> Brands { get; set; }
        public ISession Session { get { return _session; } }
    }
}
