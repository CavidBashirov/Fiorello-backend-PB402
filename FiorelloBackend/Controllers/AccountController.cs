using FiorelloBackend.Helpers.Enums;
using FiorelloBackend.Models;
using FiorelloBackend.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FiorelloBackend.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, 
                                 SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;

        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM request)
        {
            if(!ModelState.IsValid)
            {
                return View(request);
            }

            AppUser user = new()
            {
                FullName = request.FullName,
                Email = request.Email,
                UserName = request.UserName,
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }
                return View(request);
            }

            await _userManager.AddToRoleAsync(user, Roles.Member.ToString());

            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var existUser = await _userManager.FindByEmailAsync(request.EmailOrUsername);

            if(existUser is null)
            {
                existUser = await _userManager.FindByNameAsync(request.EmailOrUsername);
            }

            if(existUser is null)
            {
                ModelState.AddModelError(string.Empty, "Email or password is wrong");
                return View(request);
            }

            var result = await _userManager.CheckPasswordAsync(existUser, request.Password);

            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Email or password is wrong");
                return View(request);
            }

            var res = await _signInManager.PasswordSignInAsync(existUser, request.Password, false, false);

            if (res.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "Please check your email for comfirmation");
                return View(request);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
