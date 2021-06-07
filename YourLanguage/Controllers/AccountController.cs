using AppCore.Business.Models.Results;
using Business.Models;
using Business.Services;
using Business.Services.Bases;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using YourLanguage.Settings;

namespace YourLanguage.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;
        public AccountController(IAccountService accountService, IUserService userService)
        {
            _accountService = accountService;
            _userService = userService;
        }
        public IActionResult Register()
        {
            var model = new RegisterModel();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                model.Active = AppSettings.NewUserActive;
                var accountResult = _accountService.Register(model);
                if (accountResult.Status == ResultStatus.Exception)
                    throw new Exception(accountResult.Message);
                if (accountResult.Status == ResultStatus.Error)
                    ModelState.AddModelError("", accountResult.Message);
                LoginModel tempModel = new LoginModel()
                {
                    UserName = model.UserName,
                    Password = model.Password
                };
                var result = _accountService.Login(tempModel);
                if (result.Status == ResultStatus.Exception)
                    throw new Exception(result.Message);
                if (result.Status == ResultStatus.Success)
                {
                    var user = result.Data;
                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, user.RoleName),
                        new Claim(ClaimTypes.Sid, user.Id.ToString())
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }
        public IActionResult Login()
        {
            var model = new LoginModel();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _accountService.Login(model);
                if (result.Status == ResultStatus.Exception)
                    throw new Exception(result.Message);
                if (result.Status == ResultStatus.Success)
                {
                    var user = result.Data;
                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, user.RoleName),
                        new Claim(ClaimTypes.Sid, user.Id.ToString())
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", result.Message);
            }
            return View(model);
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        public IActionResult Details()
        {
            var userId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value);
            var model = _userService.Query().SingleOrDefault(u => u.Id == userId);
            return View(model);
        }
        public IActionResult Edit()
        {
            var userId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value);
            var model = _userService.Query().SingleOrDefault(u => u.Id == userId);
            RegisterModel registerModel = new RegisterModel()
            {
                Id = model.Id,
                GuId = model.GuId,
                UserName = model.UserName,
                EMail = model.EMail,
                Password = model.Password,
            };
            return View(registerModel);
        }
        [HttpPost]
        public IActionResult Edit(RegisterModel registerModel)
        {
            if (ModelState.IsValid)
            {
                var userId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value);
                registerModel.Id = userId;
                var result = _accountService.Update(registerModel);
                if (result.Status == ResultStatus.Exception)
                    throw new Exception(result.Message);
                if (result.Status == ResultStatus.Success)
                    return RedirectToAction(nameof(Details));
                ModelState.AddModelError("", result.Message);
            }
            return View(registerModel);
        }
    }
}
