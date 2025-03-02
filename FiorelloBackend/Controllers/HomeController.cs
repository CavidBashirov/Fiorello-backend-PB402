using FiorelloBackend.Data;
using FiorelloBackend.Models;
using FiorelloBackend.Services.Interfaces;
using FiorelloBackend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FiorelloBackend.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBlogService _blogService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IHttpContextAccessor _accessor;

        public HomeController(AppDbContext context,
                              IBlogService blogService,
                              ICategoryService categoryService,
                              IProductService productService,
                              IHttpContextAccessor accessor)
        {
            _context = context;
            _blogService = blogService;
            _categoryService = categoryService;
            _productService = productService;
            _accessor = accessor;
        }
        public async Task<IActionResult>  Index()
        {
            IEnumerable<BlogVM> blogs = await _blogService.GetAllAsync(3);

            IEnumerable<CategoryVM> categories = await _categoryService.GetAllAsync();

            IEnumerable<ProductVM> products = await _productService.GetAllAsync();

            return View(new HomeVM
            {
                Blogs = blogs,
                Categories = categories,
                Products = products
            });
        }

        [HttpPost]
        public IActionResult AddProductToBasket(int id)
        {
            List<BasketVM> basketDatas = [];

            if(_accessor.HttpContext.Request.Cookies["basket"] != null)
            {
                basketDatas = JsonConvert.DeserializeObject<List<BasketVM>>(_accessor.HttpContext.Request.Cookies["basket"]);
            }

            var existBasketData = basketDatas.FirstOrDefault(m => m.ProductId == id);

            if (existBasketData is null)
            {
                basketDatas.Add(new BasketVM { ProductId = id, ProductCount = 1 });
            }
            else
            {
                existBasketData.ProductCount++;
            }

            _accessor.HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketDatas));

            int basketCount = basketDatas.Sum(m => m.ProductCount); 
            return Ok(basketCount);
        }
    }
}
