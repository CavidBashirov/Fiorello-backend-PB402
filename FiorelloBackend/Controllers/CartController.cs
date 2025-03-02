using FiorelloBackend.Services.Interfaces;
using FiorelloBackend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FiorelloBackend.Controllers
{
    public class CartController : Controller
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IProductService _productService;

        public CartController(IHttpContextAccessor accessor,
                              IProductService productService)
        {
            _accessor = accessor;
            _productService = productService;
        }
        public async Task<IActionResult> Index()
        {
            List<BasketVM> basketDatas = [];

            if (_accessor.HttpContext.Request.Cookies["basket"] != null)
            {
                basketDatas = JsonConvert.DeserializeObject<List<BasketVM>>(_accessor.HttpContext.Request.Cookies["basket"]);
            }

            Dictionary<ProductDetailVM,int> products = new();

            foreach (var item in basketDatas)
            {
                var product = await _productService.GetByIdAsync(item.ProductId);
                products.Add(product, item.ProductCount);
            }

            decimal total = products.Sum(m => m.Key.Price * m.Value);

            return View(new BasketDetailVM { Products = products, Total = total });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            List<BasketVM> basketDatas = [];

            if (_accessor.HttpContext.Request.Cookies["basket"] != null)
            {
                basketDatas = JsonConvert.DeserializeObject<List<BasketVM>>(_accessor.HttpContext.Request.Cookies["basket"]);
            }

            var existBasketData = basketDatas.FirstOrDefault(m => m.ProductId == id);
            basketDatas.Remove(existBasketData);

            _accessor.HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketDatas));

            int count = basketDatas.Sum(m => m.ProductCount);

            Dictionary<ProductDetailVM, int> products = new();

            foreach (var item in basketDatas)
            {
                var product = await _productService.GetByIdAsync(item.ProductId);
                products.Add(product, item.ProductCount);
            }

            decimal total = products.Sum(m => m.Key.Price * m.Value);

            return Ok(new { total, count});

        }
    }
}
