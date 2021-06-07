using AppCore.Business.Models.Results;
using Business.Enums;
using Business.Models;
using Business.Services.Bases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YourLanguage.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        public UserController(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }
        public IActionResult Index()
        {
            var query = _userService.Query();
            List<UserModel> userList = new List<UserModel>();
            foreach (var item in query)
            {
                string stars = item.Password.Take(1).FirstOrDefault().ToString() + item.Password.Skip(1).Take(1).FirstOrDefault().ToString();
                int starNumber = item.Password.Count();
                for (int i = 0; i < starNumber - 1; i++)
                {
                    stars += "*";
                }

                userList.Add(new UserModel()
                {
                    Id = item.Id,
                    GuId = item.GuId,
                    UserName = item.UserName,
                    EMail = item.EMail,
                    Active = item.Active,
                    NumbersofWordLearned = item.NumbersofWordLearned,
                    Role = item.Role,
                    Password = stars
                });
            }
            if (TempData["danger"] != null)
            {
                if (TempData["danger"].ToString() != "")
                    ViewBag.Danger = TempData["danger"].ToString();
                TempData["danger"] = null;
            }
            if (TempData["info"] != null)
            {
                if (TempData["info"].ToString() != "")
                    ViewBag.Info = TempData["info"].ToString();
                TempData["info"] = null;
            }
            return View(userList);
        }
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                TempData["danger"] = "Not found any user!";
                return RedirectToAction(nameof(Index));
            }
            var user = _userService.Query().SingleOrDefault(u => u.Id == id.Value);
            return View(user);
        }
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(_roleService.Query().ToList(), "Id", "Name", (int)Roles.Admin);
            var model = new UserModel();
            return View(model);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(UserModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _userService.Add(model);
                if (result.Status == ResultStatus.Exception)
                    throw new Exception(result.Message);
                if (result.Status == ResultStatus.Success)
                {
                    TempData["info"] = "User created succesfully.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message);
            }
            ViewBag.Roles = new SelectList(_roleService.Query().ToList(), "Id", "Name", model.RoleId);
            return View(model);
        }
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["danger"] = "Not found any user!";
                return RedirectToAction(nameof(Index));
            }
            var model = _userService.Query().SingleOrDefault(u => u.Id == id.Value);
            ViewBag.Roles = new SelectList(_roleService.Query().ToList(), "Id", "Name", model.RoleId);
            return View(model);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(UserModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _userService.Update(model);
                if (result.Status == ResultStatus.Exception)
                    throw new Exception(result.Message);
                if (result.Status == ResultStatus.Success)
                {
                    TempData["info"] = "User edited succesfully.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message);
            }
            ViewBag.Roles = new SelectList(_roleService.Query().ToList(), "Id", "Name", model.RoleId);
            return View(model);
        }
        public IActionResult DeleteDirectly(int? id)
        {
            if (id == null)
            {
                TempData["danger"] = "Not found any user!";
                return RedirectToAction(nameof(Index));
            }
            var result = _userService.Delete(id.Value);
            if (result.Status == ResultStatus.Exception)
                throw new Exception(result.Message);
            TempData["info"] = "User is deleted succesfuly.";
            return RedirectToAction(nameof(Index));
        }
    }
}
