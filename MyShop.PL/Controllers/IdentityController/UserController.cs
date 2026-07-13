using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShop.DAL.Entities.IdentityEntity;
using MyShop.PL.ViewModels.Identity;

namespace MyShop.PL.Controllers.IdentityController
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? email)
        {
            ViewBag.PageTitle = "Security";
            ViewBag.CardTitle = "Users";

            if (string.IsNullOrWhiteSpace(email))
            {
                var userEntities = await _userManager.Users.OrderBy(u => u.Email).ToListAsync();
                var users = new List<UserViewModel>();

                foreach (var user in userEntities)
                    users.Add(await MapUserAsync(user));

                return View(users);
            }

            var matchedUser = await _userManager.FindByEmailAsync(email.Trim());
            if (matchedUser is null)
                return View(Enumerable.Empty<UserViewModel>());

            return View(new List<UserViewModel> { await MapUserAsync(matchedUser) });
        }

        public async Task<IActionResult> Details(string? id, string viewName = nameof(Details))
        {
            SetPageTitles(viewName);

            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            return View(viewName, await MapUserAsync(user));
        }

        public async Task<IActionResult> Edit(string? id)
            => await Details(id, nameof(Edit));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string? id, UserViewModel model)
        {
            SetPageTitles(nameof(Edit));

            if (string.IsNullOrWhiteSpace(id) || id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            user.FirstName = model.FirstName?.Trim() ?? string.Empty;
            user.LastName = model.LastName?.Trim();

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["success"] = "User updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            model.Roles = await _userManager.GetRolesAsync(user);
            return View(model);
        }

        public async Task<IActionResult> Delete(string? id)
            => await Details(id, nameof(Delete));

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(string? id)
        {
            SetPageTitles(nameof(Delete));

            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            if (user.Id == _userManager.GetUserId(User))
            {
                ModelState.AddModelError(string.Empty, "You cannot delete the account currently signed in.");
                return View(nameof(Delete), await MapUserAsync(user));
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["success"] = "User deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(nameof(Delete), await MapUserAsync(user));
        }

        private async Task<UserViewModel> MapUserAsync(ApplicationUser user)
        {
            return new UserViewModel
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName ?? string.Empty,
                Roles = await _userManager.GetRolesAsync(user)
            };
        }

        private void SetPageTitles(string viewName)
        {
            ViewBag.PageTitle = "Security";
            ViewBag.CardTitle = viewName switch
            {
                nameof(Edit) => "Edit User",
                nameof(Delete) => "Delete User",
                _ => "User Details"
            };
        }
    }
}
