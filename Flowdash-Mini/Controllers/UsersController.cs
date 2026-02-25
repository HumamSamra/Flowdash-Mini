using AutoMapper;
using Flowdash_Mini.Controllers;
using Flowdash_Mini.Enums;
using Flowdash_Mini.Extensions;
using Flowdash_Mini.Models.Accounts;
using Flowdash_Mini.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Flowdash_Mini.Areas.Admin.Controllers
{
    [Authorize(Roles = nameof(UserType.Admin))]
    public class UsersController : _BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;
        public UsersController(UserManager<AppUser> userManager, IMapper mapper,
                                RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int type = 0)
        {
            var users = _userManager.Users.ToList();
            var map = new List<AppUserVM>();
            foreach (var user in users)
            {
                var mappedUser = _mapper.Map<AppUserVM>(user);
                mappedUser.IsLockedout = await _userManager.IsLockedOutAsync(user);
                map.Add(mappedUser);
            }
            return View(map);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateUserVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _mapper.Map<AppUser>(model);
            user.CreatedBy = User.GetUserName();
            user.ModifiedBy = User.GetUserName();

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _userManager.AddToRolesAsync(user, model.Roles);

            return Redirect("/users");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = _userManager.Users.FirstOrDefault(e => e.Id.ToString() == id);
            if (user == null)
            {
                return Redirect("/users");
            }
            var map = _mapper.Map<EditUserVM>(user);
            map.Roles = await _userManager.GetRolesAsync(user);
            return View(map);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _userManager.Users.FirstOrDefault(e => e.Id == model.Id);
            if (user == null)
            {
                return Redirect("/users");
            }

            var updatedUser = _mapper.Map(model, user);
            updatedUser.ModifiedBy = User.GetUserName();
            updatedUser.ModifiedAt = DateTime.UtcNow;
            updatedUser.CreatedAt = user.CreatedAt; // Keep the original created date
            updatedUser.CreatedBy = user.CreatedBy; // Keep the original created by

            var result = await _userManager.UpdateAsync(updatedUser);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            if (model.Roles.Count > 0 && user.Id != model.Id)
            {
                foreach (var role in model.Roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        ModelState.AddModelError(string.Empty, $"Role '{role}' Doesn't Exist!");
                        return View(model);
                    }
                }

                var oldRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, oldRoles);
                await _userManager.AddToRolesAsync(user, model.Roles);
            }
            return Redirect("/users");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(AppUserVM model, string password, string logoutUsers = "off")
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                ViewBag.PasswordError = "Password is not valid";
                return View(model);
            }

            var user = _userManager.Users.FirstOrDefault(e => e.Id == model.Id);
            if (user == null)
            {
                return Redirect("/users");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, password);
            if (!result.Succeeded)
            {
                string msg = "";
                foreach (var err in result.Errors)
                {
                    msg += err.Description + Environment.NewLine;
                }
                ViewBag.PasswordError = msg;
                return View(model);

            }

            if (logoutUsers == "on")
            {
                // Logout everyone
                await _userManager.UpdateSecurityStampAsync(user);
            }

            return Redirect("/users");
        }

        [HttpPost]
        public async Task<JsonResult> SetUserState(string id, bool active)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null && user.Id != User.GetUserId())
            {
                user.ModifiedAt = DateTime.UtcNow;
                user.ModifiedBy = User.GetUserName();
                if (active)
                {
                    await _userManager.SetLockoutEnabledAsync(user, true);
                    await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow);
                    await _userManager.SetLockoutEnabledAsync(user, false);
                }
                else
                {
                    await _userManager.SetLockoutEnabledAsync(user, true);
                    await _userManager.SetLockoutEndDateAsync(user, DateTime.MaxValue);

                    // Logout everyone
                    await _userManager.UpdateSecurityStampAsync(user);
                }
                await _userManager.UpdateAsync(user);
            }
            return Json(new { statusMessage = 200, msg = "User state updated successfully" });
        }

        [HttpPost]
        public async Task<JsonResult> ForceDelete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null && user.Id != User.GetUserId())
            {
                // Logout everyone
                await _userManager.UpdateSecurityStampAsync(user);
                await _userManager.DeleteAsync(user);
            }
            return Json(new { statusMessage = 200, msg = "User Deleted successfully" });
        }
    }
}
