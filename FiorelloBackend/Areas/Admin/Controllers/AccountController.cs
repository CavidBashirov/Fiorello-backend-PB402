using FiorelloBackend.Areas.Admin.ViewModels.Account;
using FiorelloBackend.Helpers.Enums;
using FiorelloBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiorelloBackend.Areas.Admin.Controllers
{
    [Area("Admin")]
   
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, 
                                 RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;

        }

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();

            List<UserRolesDto> userWithRoles = new();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                userWithRoles.Add(new UserRolesDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Roles = userRoles.ToList()
                });
            }

            return View(userWithRoles);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> AddRoleToUser()
        {
            return View();
        }



        //create roles when need

        //[HttpGet]
        //public async Task<IActionResult> CreateRoles()
        //{
        //    foreach (var role in Enum.GetValues(typeof(Roles)))
        //    {
        //        if(!await _roleManager.RoleExistsAsync(role.ToString()))
        //        {
        //            await _roleManager.CreateAsync(new IdentityRole { Name = role.ToString() });
        //        }
        //    }

        //    return Ok();
        //}

        
    }
}
