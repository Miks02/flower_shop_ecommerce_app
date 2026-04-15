using System.Security.Claims;
using Azure;
using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Application.Features.Auth.Commands.Login;
using FlowerShop.Application.Features.Auth.Commands.Logout;
using FlowerShop.Application.Features.Auth.Commands.RegisterUser;
using FlowerShop.Application.Features.Users.Commands.RemoveProfilePicture;
using FlowerShop.Application.Features.Users.Commands.UpdatePassword;
using FlowerShop.Application.Features.Users.Commands.UpdateProfile;
using Microsoft.AspNetCore.Mvc;
using FlowerShop.Web.ViewModels.Components;
using Microsoft.AspNetCore.Authorization;

namespace FlowerShop.Web.Controllers;

 public class AccountController : BaseController
    {
        private readonly RegisterUserHandler _registerUserHandler;
        private readonly LoginHandler _loginHandler;
        private readonly LogoutHandler _logoutHandler;
        private readonly UpdateProfileHandler _updateProfileHandler;
        private readonly UpdatePasswordHandler _updatePasswordHandler;
        private readonly RemoveProfilePictureHandler _removeProfilePictureHandler;
        private readonly IUserProvider _userProvider;
        

        private readonly string _loginComponent = "Components/Login/Default";
        private readonly string _registerComponent = "Components/Register/Default";
        private readonly string _profileSettingsComponent = "~/Views/Shared/Components/Settings/Default.cshtml";

        public AccountController(
            ILogger<BaseController> logger,
            LoginHandler loginHandler,
            RegisterUserHandler registerUserHandler,
            LogoutHandler logoutHandler,
            UpdateProfileHandler updateProfileHandler,
            UpdatePasswordHandler updatePasswordHandler,
            IUserProvider userProvider,
            RemoveProfilePictureHandler removeProfilePictureHandler
            ) : base(logger)
        {
            _registerUserHandler = registerUserHandler;
            _loginHandler = loginHandler;
            _logoutHandler = logoutHandler;
            _updateProfileHandler = updateProfileHandler;
            _updatePasswordHandler = updatePasswordHandler;
            _userProvider = userProvider;
            _removeProfilePictureHandler = removeProfilePictureHandler;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (!User.Identity!.IsAuthenticated)
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

            var loginCommand = new LoginCommand
            {
                Username = model.Username,
                Password = model.Password,
                RememberMe = model.RememberMe
            };

            var result = await _loginHandler.Handle(loginCommand);
            if (result.IsSuccess)
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
            if (!User.Identity!.IsAuthenticated)
                return PartialView(_registerComponent);

            Response.Headers.Append("HX-Redirect", "/Home/Index");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            
            if (!ModelState.IsValid) 
                return PartialView(_registerComponent, model);

            var registerCommand = new RegisterUserCommand
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = model.Password,
                Username = model.Username,
                ConfirmPassword = model.ConfirmPassword,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email
            };

            var result = await _registerUserHandler.Handle(registerCommand);

            if (!result.IsSuccess)
            {
                foreach (var error in result.Errors!)
                    ModelState.AddModelError(nameof(model.ConfirmPassword), error.Description);
                return PartialView(_registerComponent, model);
            }

            await _loginHandler.Handle(new LoginCommand { Username = model.Username, Password = model.Password });
            
            Response.Headers.Append("HX-Redirect", "/Home/Index");
            return Ok();
        }
        

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(SettingsPageViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView(_profileSettingsComponent, model);

            var command = new UpdateProfileCommand
            {
                UserId = _userProvider.GetCurrentUserId(),
                UserName = model.ProfileVm.UserName,
                FirstName = model.ProfileVm.FirstName,
                LastName = model.ProfileVm.LastName,
                Email = model.ProfileVm.Email,
                ProfilePicture = model.ProfileVm.ProfilePicture,
                PhoneNumber = model.ProfileVm.PhoneNumber
            };
        
            var result = await _updateProfileHandler.Handle(command);
            
            if (!result.IsSuccess)
            {
                string errorMessage = string.Empty;
                foreach (var error in result.Errors!)
                    errorMessage += " " + error.Description;
                
                SetErrorMessage(errorMessage);
                return PartialView(_profileSettingsComponent, model);
            }

            if (!string.IsNullOrEmpty(model.ChangePasswordVm.CurrentPassword))
            {
                var passwordCommand = new UpdatePasswordCommand
                {
                    UserId = _userProvider.GetCurrentUserId(),
                    CurrentPassword = model.ChangePasswordVm.CurrentPassword,
                    NewPassword = model.ChangePasswordVm.NewPassword,
                    ConfirmPassword = model.ChangePasswordVm.ConfirmPassword
                };

                var passwordResult = await _updatePasswordHandler.Handle(passwordCommand);

                if (!passwordResult.IsSuccess)
                {
                    string errorMessage = string.Empty;
                    foreach (var error in passwordResult.Errors!)
                        errorMessage += " " + error.Description;
                    
                    SetErrorMessage(errorMessage);
                    return PartialView(_profileSettingsComponent, model);
                }
            }
            
            SetSuccessMessage("Profil je uspesno ažuriran");
            Response.Headers.Append("HX-Redirect", "/User/Profile/Settings");
            return Ok();
        }
        
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> RemoveProfilePicture()
        {
            var result = await _removeProfilePictureHandler.Handle(new RemoveProfilePictureCommand{ UserId = _userProvider.GetCurrentUserId() });
        
            if (!result.IsSuccess)
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
            await _logoutHandler.Handle();
            return RedirectToAction("Index", "Home");
        }
        
    }