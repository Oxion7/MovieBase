using MovieBase.Models;
using MovieBase.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameLibrary.Controllers
{
    [Authorize(Roles = "admin")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

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

        [HttpGet]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null) return NotFound();

            var user = await GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var userViewModel = await GetUserViewModelAsync(user);

            return View(userViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var userToDelete = await GetUserByIdAsync(id);
            if (userToDelete == null) return RedirectToAction("Index");

            var result = await _userManager.DeleteAsync(userToDelete);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Failed to delete user.");
            var userViewModel = await GetUserViewModelAsync(userToDelete);
            return View("Delete", userViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null) return NotFound();

            var user = await GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var userViewModel = await GetUserViewModelAsync(user);
            ViewBag.Roles = await _roleManager.Roles.ToListAsync();

            return View(userViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel user, string newRole)
        {
            if (user == null) return NotFound();

            var userToEdit = await GetUserByIdAsync(user.Id);
            if (userToEdit == null) return NotFound();

            var result = await UpdateUserRoleAsync(userToEdit, newRole);
            if (!result.Succeeded)
            {
                ViewBag.Roles = await _roleManager.Roles.ToListAsync();
                return View(user);
            }

            return RedirectToAction("Index");
        }

        private async Task<User> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        private async Task<UserViewModel> GetUserViewModelAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return new UserViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Role = string.Join(",", roles)
            };
        }

        private async Task<IdentityResult> UpdateUserRoleAsync(User user, string newRole)
        {
            var currentRoles = await _userManager.GetRolesAsync(user);

            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to remove current roles.");
                return removeResult;
            }

            var addResult = await _userManager.AddToRoleAsync(user, newRole);
            if (!addResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to add new role.");
            }

            return addResult;
        }
    }
}