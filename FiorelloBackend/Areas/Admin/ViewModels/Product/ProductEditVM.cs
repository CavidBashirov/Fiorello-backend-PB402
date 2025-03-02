using FiorelloBackend.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace FiorelloBackend.Areas.Admin.ViewModels.Product
{
    public class ProductEditVM
    {
        [Required]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public IEnumerable<ProductImageVM> ProductImages { get; set; }
        public IEnumerable<IFormFile> UploadImages { get; set; }
    }
}
