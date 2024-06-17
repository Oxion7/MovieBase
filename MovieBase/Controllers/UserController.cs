using MovieBase.Models;
using MovieBase.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GameLibrary.Controllers
{
    public class UserController : Controller
    {
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index()
        {
            var usersList = await _userManager.Users.ToListAsync();

            var usersViewModel = new List<UserViewModel>();
            foreach (var user in usersList)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersViewModel.Add(new UserViewModel
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Role = string.Join(",", roles)
                });
            }
            return View(usersViewModel);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
                return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user);

            var userViewModel = new UserViewModel()
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Role = string.Join(",", roles)
            };
            return View(userViewModel);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(UserViewModel user)
        {
            if (user != null)
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
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
                return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user);

            var userViewModel = new UserViewModel()
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Role = string.Join(",", roles)
            };

            ViewBag.Roles = await _roleManager.Roles.ToListAsync();
            return View(userViewModel);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel user, string newRole)
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

            return RedirectToAction("Index");
        }
    }
}