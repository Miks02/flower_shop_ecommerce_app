using FlowerShop.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using FlowerShop.Models;
using FlowerShop.ViewModels.Components;

namespace FlowerShop.Controllers;

 public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly string _loginComponent = "Components/Login/Default";
        private readonly string _registerComponent = "Components/Register/Default";

        public AccountController(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            RoleManager<IdentityRole> roleManager,
            ILogger<BaseController> logger
            ) : base(logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();
    }