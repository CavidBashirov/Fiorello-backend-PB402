using FiorelloBackend.Areas.Admin.ViewModels.Slider;
using FiorelloBackend.Data;
using FiorelloBackend.Helpers.Extensions;
using FiorelloBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiorelloBackend.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SliderController(AppDbContext context,
                                IWebHostEnvironment env)
        {
            _context = context;
            _env = env;

        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<SliderVM> sliders = await _context.Sliders.Select(m => new SliderVM { Id = m.Id, Image = m.Image })
                                                                  .ToListAsync();
            return View(sliders);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            Slider slider = await _context.Sliders.FirstOrDefaultAsync(m=>m.Id == id);
            if (slider == null) return NotFound();
            return View(new SliderDetailVM { Image = slider.Image });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderCreateVM request)
        {
            if(!ModelState.IsValid) return View(request);

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

            foreach (var item in request.UploadImages)
            {
                string fileName = Guid.NewGuid().ToString() + "-" + item.FileName;

                string filePath = _env.GenerateFilePath("img", fileName);

                using (FileStream stream = new(filePath, FileMode.Create))
                {
                    await item.CopyToAsync(stream);
                }

                await _context.Sliders.AddAsync(new Slider { Image = fileName });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();
            Slider slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);
            if (slider == null) return NotFound();

            string filePath = _env.GenerateFilePath("img", slider.Image);

            filePath.DeleteFile();

            _context.Sliders.Remove(slider);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();
            Slider slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);
            if (slider == null) return NotFound();
            return View(new SliderEditVM { ExistImage = slider.Image });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, SliderEditVM request)
        {
            if (id == null) return BadRequest();
            Slider slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);
            if (slider == null) return NotFound();

            if(request.UploadImage is not null)
            {
                string filePath = _env.GenerateFilePath("img", slider.Image);
                filePath.DeleteFile();

                string fileName = request.UploadImage.GenerateFileName();

                string newFilePath = _env.GenerateFilePath("img", fileName);

                using (FileStream stream = new(newFilePath, FileMode.Create))
                {
                    await request.UploadImage.CopyToAsync(stream);
                }
                slider.Image = fileName;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));

        }
        
    }
}
