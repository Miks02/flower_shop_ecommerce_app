using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using FlowerShop.Models;
using FlowerShop.Services.Interfaces;
using FlowerShop.ViewModels.Components;
using Microsoft.AspNetCore.Authorization;

namespace FlowerShop.Controllers;

 public class AccountController : BaseController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;

        private readonly string _loginComponent = "Components/Login/Default";
        private readonly string _registerComponent = "Components/Register/Default";
        private readonly string _profileSettingsComponent = "~/Views/Shared/Components/Settings/Default.cshtml";

        public AccountController(
            SignInManager<ApplicationUser> signInManager, 
            IUserService userService,
            ILogger<BaseController> logger
            ) : base(logger)
        {
            _signInManager = signInManager;
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

            var result = await _userService.CreateUserAsync(model, "User");

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(nameof(model.ConfirmPassword), error);
                return PartialView(_registerComponent, model);
            }
            
            await _signInManager.SignInAsync(result.Payload!, isPersistent: false);
            
            Response.Headers.Append("HX-Redirect", "/Home/Index");
            return Ok();
        }
        

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(SettingsPageViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView(_profileSettingsComponent, model);

            var result = await _userService.UpdateProfileAsync(model);
            
            if (!result.IsSucceeded)
            {
                string errorMessage = string.Empty;
                foreach (var error in result.Errors!)
                    errorMessage += " " + error.Description;
                
                SetErrorMessage(errorMessage);
                return PartialView(_profileSettingsComponent, model);
            }
            
            string message = (result.Payload!.ProfileUpdated, result.Payload.PasswordChanged) switch
            {
                (false, false) => "Nema izmenjenih podataka za čuvanje.",
                (true,  false) => "Profil je uspešno ažuriran.",
                (false, true ) => "Lozinka je uspešno ažurirana.",
                (true,  true ) => "Profil i lozinka su uspešno ažurirani."
            };
            
            if(result.Payload.ProfileUpdated || result.Payload.PasswordChanged)
                await _signInManager.RefreshSignInAsync(result.Payload.User);
            
            SetSuccessMessage(message);
            Response.Headers.Append("HX-Redirect", "/User/Profile/Settings");
            return Ok();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> RemoveProfilePicture()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
            {
                SetErrorMessage("Došlo je do neočekivane greške. Korisnik nije pronadjen");
                return ViewComponent("Settings");
            }

            var result = await _userService.RemoveProfilePictureAsync(userId);

            if (!result.Succeeded)
            {
                SetErrorMessage("Došlo je do greške prilikom brisanja profilne slike");
                return ViewComponent("Settings");
            }
            
            SetSuccessMessage("Profilna slika je uspešno obrisana!");
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