using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MovieBase.Models;
using MovieBase.ViewModels;

namespace MovieBase.Controllers
{
    public class UserController : Controller
    {
        //private ApplicationDbContext _db;
        private UserManager<User> _userManager;

        private RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            //_db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "admin")]
        public IActionResult Index()
        {
            var users = _userManager.Users.Select(c => new UserViewModel()
            {
                Id = c.Id,
                Username = c.UserName,
                Email = c.Email,
                Role = string.Join(",", _userManager.GetRolesAsync(c).Result.ToArray())
            }).ToList();

            return View(users);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Delete(string? Id)
        {
            if (Id == null)
                return NotFound();
            var user = _userManager.Users.Select(c => new UserViewModel()
            {
                Id = c.Id,
                Username = c.UserName,
                Email = c.Email,
                Role = string.Join(",", _userManager.GetRolesAsync(c).Result.ToArray())
            }).ToList().FirstOrDefault(u => u.Id == Id);
            if (user == null)
                return NotFound();
            return View(user);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(UserViewModel user)
        {
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                var userToDelete = await _userManager.FindByIdAsync(user.Id);
                if (userToDelete != null)
                {
                    var result = await _userManager.DeleteAsync(userToDelete);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to delete user.");
                        return View(user);
                    }
                }

                return RedirectToAction("Index", "User");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Edit(string? Id)
        {
            if (Id == null)
                return NotFound();
            var user = _userManager.Users.Select(c => new UserViewModel()
            {
                Id = c.Id,
                Username = c.UserName,
                Email = c.Email,
                Role = string.Join(",", _userManager.GetRolesAsync(c).Result.ToArray())
            }).ToList().FirstOrDefault(u => u.Id == Id);

            if (user == null)
                return NotFound();
            ViewBag.Roles = _roleManager.Roles.ToList();
            return View(user);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> EditRole(UserViewModel user, string newRole)
        {
            if (user == null)
                return NotFound();
            var userToEdit = await _userManager.FindByIdAsync(user.Id);
            if (userToEdit == null)
                return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(userToEdit);
            var removeResult = await _userManager.RemoveFromRolesAsync(userToEdit, currentRoles);
            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to remove current roles.");
                ViewBag.Roles = _roleManager.Roles.ToList();
                return View(user);
            }

            var addResult = await _userManager.AddToRoleAsync(userToEdit, newRole);
            if (!addResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to add new role.");
                ViewBag.Roles = _roleManager.Roles.ToList();
                return View(user);
            }

            return RedirectToAction("Index", "User");
        }
    }
}