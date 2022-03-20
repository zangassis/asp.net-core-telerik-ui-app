using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Models
{
    public class ProductDtoViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Displayname*:")]
        [Required(ErrorMessage = "You must specify the Displayname.")]
        public string Displayname { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool Active { get; set; }

        [Display(Name = "Brand*:")]
        [Required(ErrorMessage = "You must specify the Brand.")]
        public string Brand { get; set; }

        [Display(Name = "Price*:")]
        [Required(ErrorMessage = "You must specify the Price.")]
        public double Price { get; set; }

        [Display(Name = "Category*:")]
        [Required(ErrorMessage = "You must specify the Category.")]
        public string Category { get; set; }
    }
}
