using FlowerShop.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using FlowerShop.Models;
using FlowerShop.ViewModels.Components;
using Microsoft.AspNetCore.Authorization;

namespace FlowerShop.Controllers;

 public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _environment;

        private readonly string _loginComponent = "Components/Login/Default";
        private readonly string _registerComponent = "Components/Register/Default";
        private readonly string _profileSettingsComponent = "~/Views/Shared/Components/Settings/Default.cshtml";

        public AccountController(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment environment,
            ILogger<BaseController> logger
            ) : base(logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (!_signInManager.IsSignedIn(User))
                return PartialView(_loginComponent);
            
            Response.Headers.Append("HX-Redirect", "/Home/Index");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return PartialView(_loginComponent, model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                Response.Headers.Append("HX-Redirect", "/Home/Index");

                return Ok();
            }

            ModelState.AddModelError(nameof(model.Password), "Neispravna lozinka ili korisničko ime.");

            return PartialView(_loginComponent, model);

        }

        [HttpGet]
        public IActionResult Register()
        {
            if (!_signInManager.IsSignedIn(User))
                return PartialView(_registerComponent);

            Response.Headers.Append("HX-Redirect", "/Home/Index");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            
            if (!ModelState.IsValid) 
                return PartialView(_registerComponent, model);

            if (await _userManager.FindByNameAsync(model.Username) is not null)
            {
                ModelState.AddModelError(nameof(model.Username), "Korisničko ime je zauzeto");
                return PartialView(_registerComponent, model);
            }
            
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
            {
                ModelState.AddModelError(nameof(model.Email), "Email adresa je zauzeta");
                return PartialView(_registerComponent, model);
            }

            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Username,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("User"));
                }

                await _userManager.AddToRoleAsync(user, "User");

                await _signInManager.SignInAsync(user, isPersistent: false);
                Response.Headers.Append("HX-Redirect", "/Home/Index");
                return Ok();
            }


            ModelState.AddModelError(nameof(model.ConfirmPassword), "Došlo je do greške prilikom registracije");

            foreach (var error in result.Errors)
            {
                LogHelper.LogModelErrors(ModelState, _logger, error.Description);
                
            }

            return PartialView(_registerComponent, model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(SettingsPageViewModel vm)
        {
            
            if (!ModelState.IsValid)
                return PartialView(_profileSettingsComponent, vm);
            
            var user = await _userManager.FindByNameAsync(User.Identity!.Name!);

            if (user is null)
                return ViewComponent("NotFound");

            if (user.UserName != vm.ProfileVm.UserName)
                user.UserName = vm.ProfileVm.UserName;
            if (user.Email != vm.ProfileVm.Email)
                user.Email = vm.ProfileVm.Email;

            user.FirstName = vm.ProfileVm.FirstName;
            user.LastName = vm.ProfileVm.LastName;
            user.PhoneNumber = vm.ProfileVm.PhoneNumber;

            if (vm.ProfileVm.ProfilePicture is not null && vm.ProfileVm.ProfilePicture.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "user");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid() + "_" + vm.ProfileVm.ProfilePicture.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                await using var fileStream = new FileStream(filePath, FileMode.Create);
                
                await vm.ProfileVm.ProfilePicture.CopyToAsync(fileStream);
                
                if (!string.IsNullOrEmpty(user.ImagePath))
                {
                    var oldImagePath = Path.Combine(_environment.WebRootPath, user.ImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                    
                }

                user.ImagePath = "/uploads/user/" + uniqueFileName;
            }
            
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                SetErrorMessage("Došlo je do greške prilikom ažuriranja podataka");
                return PartialView(_profileSettingsComponent, vm);
            }

            var currentPassword = vm.ChangePasswordVm.CurrentPassword;
            var newPassword = vm.ChangePasswordVm.NewPassword;

            if (!string.IsNullOrEmpty(currentPassword))
            {
                bool isPasswordValid = await _userManager.CheckPasswordAsync(user, currentPassword);

                if (!isPasswordValid)
                {
                    _logger.LogError("Uneta je netačna lozinka");
                    ModelState.AddModelError("ChangePasswordVm.CurrentPassword", "Netačna lozinka.");
                    return PartialView(_profileSettingsComponent, vm);
                }
                
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

                if (!changePasswordResult.Succeeded)
                {
                    TempData["Error"] = "Došlo je do greške prilikom izmene lozinke";
                    return PartialView(_profileSettingsComponent, vm);
                }

            }
            
            await _signInManager.RefreshSignInAsync(user);
            
            Response.Headers.Append("HX-Redirect", "/User/Profile/Settings");
            TempData["Success"] = "Promene su uspešno sačuvane";
            _logger.LogInformation("Podaci su uspešno sačuvani");
           
            return Ok();

        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();
    }