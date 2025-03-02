using FiorelloBackend.Areas.Admin.ViewModels.Category;
using FiorelloBackend.Data;
using FiorelloBackend.Helpers.Constants;
using FiorelloBackend.Models;
using FiorelloBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiorelloBackend.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ISettingService _settingService;
        private readonly AppDbContext _context;

        public CategoryController(ICategoryService categoryService,
                                  AppDbContext context,
                                  ISettingService settingService)
        {
            _categoryService = categoryService;
            _context = context;
            _settingService = settingService;

        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _categoryService.GetAllAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateVM request)
        {
            if (!ModelState.IsValid) return View(request);

            int count = await _context.Categories.CountAsync();
            Dictionary<string,string> settings = await _settingService.GetAllAsync();

            if(count == Convert.ToInt32(settings["CategoryLimit"]))
            {
                ModelState.AddModelError("Name", "Category count is full");
                return View(request);
            }

            var existCategory = await _context.Categories.FirstOrDefaultAsync(m => m.Name == request.Name);

            if (existCategory != null)
            {
                ModelState.AddModelError("Name", ValidationMessages.ExistData);
                return View(request);
            }

            await _context.Categories.AddAsync(new Category { Name = request.Name});
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();
            var category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
            if (category is null) return NotFound();
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if(id is null) return BadRequest();

            var existCategory = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);

            if(existCategory is null) return NotFound();

            return View(new CategoryDetailVM { Name = existCategory.Name});
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return BadRequest();

            var existCategory = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);

            if (existCategory is null) return NotFound();

            return View(new CategoryEditVM { Name = existCategory.Name });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, CategoryEditVM request)
        {
            if (id is null) return BadRequest();

            if (!ModelState.IsValid) return View(request);

            var dbCategory = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);

            if (dbCategory is null) return NotFound();

            bool hasCategory = await _context.Categories.AnyAsync(m => m.Name.Trim() == request.Name.Trim() && m.Id != id);

            if (hasCategory)
            {
                ModelState.AddModelError("Name", ValidationMessages.ExistData);
                return View(request);
            }

            dbCategory.Name = request.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
