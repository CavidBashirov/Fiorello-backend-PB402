using FiorelloBackend.Areas.Admin.ViewModels.Product;
using FiorelloBackend.Data;
using FiorelloBackend.Helpers.Extensions;
using FiorelloBackend.Models;
using FiorelloBackend.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProductImageVM = FiorelloBackend.Areas.Admin.ViewModels.Product.ProductImageVM;
using ProductVM = FiorelloBackend.Areas.Admin.ViewModels.Product.ProductVM;

namespace FiorelloBackend.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public ProductController(AppDbContext context, 
                                 IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<ProductVM> datas = await _context.Products.Include(m=>m.Category).Include(m => m.ProductImages).Select(m => new ProductVM
            {
                Id = m.Id,
                Name = m.Name,
                Price = m.Price,
                CategoryName = m.Category.Name,
                ProductImages = m.ProductImages.Select(m => new ProductImageVM
                {
                    Name = m.Name,
                    IsMain = m.IsMain,
                }).ToList()

            }).ToListAsync();

            return View(datas);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories.Select(a => new SelectListItem()
            {
                Value = a.Id.ToString(),
                Text = a.Name
            }).ToListAsync();

            ViewBag.categories = categories;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateVM request)
        {
            var categories = await _context.Categories.Select(a => new SelectListItem()
            {
                Value = a.Id.ToString(),
                Text = a.Name
            }).ToListAsync();

            ViewBag.categories = categories;

            if (!ModelState.IsValid)
            {
                return View(request);
            }

            foreach (var item in request.ProductImages)
            {
                if (!item.CheckFileType("image/"))
                {
                    ModelState.AddModelError("ProductImages", "Input type mus be only image");
                    return View(request);
                }

                if (!item.CheckFileSize(500))
                {
                    ModelState.AddModelError("ProductImages", "Image size must be smaller than 500KB");
                    return View(request);
                }
            }

            List<ProductImage> productImages = new();

            foreach (var item in request.ProductImages)
            {
                string fileName = Guid.NewGuid().ToString() + "-" + item.FileName;

                string filePath = _env.GenerateFilePath("img", fileName);

                using (FileStream stream = new(filePath, FileMode.Create))
                {
                    await item.CopyToAsync(stream);
                }

                productImages.Add(new ProductImage { Name = fileName, IsMain = false });
            }

            productImages.FirstOrDefault().IsMain = true;


            await _context.Products.AddAsync(new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                CategoryId = (int)request.CategoryId,
                ProductImages = productImages
            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var categories = await _context.Categories.Select(a => new SelectListItem()
            {
                Value = a.Id.ToString(),
                Text = a.Name
            }).ToListAsync();

            ViewBag.categories = categories;

            if (id is null) return BadRequest();

            var existProduct = await _context.Products.Include(m=>m.ProductImages)
                                                      .Include(m=>m.Category)
                                                      .FirstOrDefaultAsync(m => m.Id == id);

            if(existProduct is null) return NotFound();

            var model = new ProductEditVM
            {
                Name = existProduct.Name,
                Description = existProduct.Description,
                Price = existProduct.Price,
                CategoryId = existProduct.CategoryId,
                ProductImages = existProduct.ProductImages.Select(m=>new ProductImageVM {Id = m.Id, Name = m.Name, IsMain = m.IsMain})
            };

            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, ProductEditVM request)
        {
            var categories = await _context.Categories.Select(a => new SelectListItem()
            {
                Value = a.Id.ToString(),
                Text = a.Name
            }).ToListAsync();

            ViewBag.categories = categories;

            if (id is null) return BadRequest();

         

            var existProduct = await _context.Products.Include(m => m.ProductImages)
                                                      .Include(m => m.Category)
                                                      .FirstOrDefaultAsync(m => m.Id == id);

            if (existProduct is null) return NotFound();

            if (!ModelState.IsValid)
            {
                request.ProductImages = existProduct.ProductImages.Select(m => new ProductImageVM { Id = m.Id, Name = m.Name, IsMain = m.IsMain });
                return View(request);
            }

            if (request.UploadImages is not null)
            {
                foreach (var item in request.UploadImages)
                {
                    if (!item.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("UploadImages", "Input type mus be only image");
                        return View(request);
                    }

                    if (!item.CheckFileSize(500))
                    {
                        ModelState.AddModelError("UploadImages", "Image size must be smaller than 500KB");
                        return View(request);
                    }
                }

                List<ProductImage> productImages = new();

                foreach (var item in request.UploadImages)
                {
                    string fileName = Guid.NewGuid().ToString() + "-" + item.FileName;

                    string filePath = _env.GenerateFilePath("img", fileName);

                    using (FileStream stream = new(filePath, FileMode.Create))
                    {
                        await item.CopyToAsync(stream);
                    }

                    productImages.Add(new ProductImage { Name = fileName, IsMain = false, ProductId = existProduct.Id });
                }

                await _context.ProductImages.AddRangeAsync(productImages);

               
            }

            existProduct.Name = request.Name;
            existProduct.Description = request.Description;
            existProduct.CategoryId = request.CategoryId;
            existProduct.Price = request.Price;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }

        [HttpPost]
        public async Task<IActionResult> DeleteProductImage(int? id)
        {
            if (id == null) return BadRequest();
            ProductImage productImage = await _context.ProductImages.FirstOrDefaultAsync(m => m.Id == id);
            if (productImage == null) return NotFound();

            string filePath = _env.GenerateFilePath("img", productImage.Name);
            filePath.DeleteFile();

            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();

            return Ok();

        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();

            var existProduct = await _context.Products.Include(m=>m.ProductImages).FirstOrDefaultAsync(m => m.Id == id);

            if (existProduct is null) return NotFound();

            foreach (var item in existProduct.ProductImages)
            {
                string filePath = _env.GenerateFilePath("img", item.Name);

                filePath.DeleteFile();
            }

            _context.Products.Remove(existProduct);
            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}
