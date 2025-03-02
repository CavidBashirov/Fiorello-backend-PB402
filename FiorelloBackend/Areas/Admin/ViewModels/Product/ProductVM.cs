using FiorelloBackend.ViewModels;

namespace FiorelloBackend.Areas.Admin.ViewModels.Product
{
    public class ProductVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public IEnumerable<ProductImageVM> ProductImages { get; set; }
    }
}
