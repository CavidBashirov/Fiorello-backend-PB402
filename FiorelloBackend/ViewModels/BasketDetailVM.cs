namespace FiorelloBackend.ViewModels
{
    public class BasketDetailVM
    {
        public Dictionary<ProductDetailVM,int> Products { get; set; }
        public decimal Total { get; set; }
    }
}
