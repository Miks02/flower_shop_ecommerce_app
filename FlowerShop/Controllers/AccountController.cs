using FlowerShop.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using FlowerShop.Models;
using FlowerShop.Services.Interfaces;
using FlowerShop.ViewModels.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace FlowerShop.Controllers;

 public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _environment;
        private readonly IUserService _userService;

        private readonly string _loginComponent = "Components/Login/Default";
        private readonly string _registerComponent = "Components/Register/Default";
        private readonly string _profileSettingsComponent = "~/Views/Shared/Components/Settings/Default.cshtml";

        public AccountController(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment environment,
            IUserService userService,
            ILogger<BaseController> logger
            ) : base(logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _environment = environment;
            _userService = userService;
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
        public async Task<IActionResult> UpdateProfile(SettingsPageViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView(_profileSettingsComponent, model);

            var user = await _userService.GetCurrentUser();
            if (user is null)
            {
                SetErrorMessage("Došlo je do nepredvidjene greške. Korisnik nije pronadjen");
                return PartialView(_profileSettingsComponent, model);
            }

            var result = await _userService.UpdateProfileAsync(model);
            
            string message = (result.ProfileUpdated, result.PasswordChanged) switch
            {
                (false, false) => "Nema izmenjenih podataka za čuvanje.",
                (true,  false) => "Profil je uspešno ažuriran.",
                (false, true ) => "Lozinka je uspešno ažurirana.",
                (true,  true ) => "Profil i lozinka su uspešno ažurirani."
            };

            if (!result.Success)
            {
                string errorMessage = string.Empty;
                foreach (var error in result.Errors)
                    errorMessage += " " + error;
                
                SetErrorMessage(errorMessage);
            }
            
            if(result.ProfileUpdated || result.PasswordChanged)
                await _signInManager.RefreshSignInAsync(user);
            
            SetSuccessMessage(message);
            Response.Headers.Append("HX-Redirect", "/User/Profile/Settings");
            return Ok();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> RemoveProfilePicture()
        {
            var user = await _userManager.FindByNameAsync(User.Identity!.Name!);

            if (user is null)
            {
                SetErrorMessage("Došlo je do greške. Korisnik nije pronadjen");
                return ViewComponent("Settings");
            }

            var imagePath = user.ImagePath;
            user.ImagePath = null;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                _logger.LogError("An error has occurred while removing the profile picture. ");
                foreach (var error in result.Errors)
                {
                    _logger.LogError("Error: " + error);
                }
                SetErrorMessage("Došlo je do greške prilikom brisanja profilne slike");
                return ViewComponent("Settings");
            }

            if (!string.IsNullOrEmpty(imagePath))
            {
                var oldImagePath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                
                if (System.IO.File.Exists(oldImagePath))
                    System.IO.File.Delete(oldImagePath);
            }

            SetSuccessMessage("Slika je uspešno obrisana");
            return ViewComponent("Settings");

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