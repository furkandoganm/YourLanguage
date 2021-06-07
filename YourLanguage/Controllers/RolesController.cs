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
    public class RolesController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;
        public RolesController(IRoleService roleService, IUserService userService)
        {
            _roleService = roleService;
            _userService = userService;
        }
        public IActionResult Index()
        {
            var query = _roleService.Query();
            if (TempData["info"] != null)
            {
                if (TempData["info"].ToString() != "")
                    ViewBag.Info = TempData["info"].ToString();
                TempData["info"] = null;
            }
            if (TempData["danger"] != null)
            {
                if (TempData["danger"].ToString() != "")
                    ViewBag.Danger = TempData["danger"].ToString();
                TempData["danger"] = null;
            }
            return View(query);
        }
        public IActionResult Create()
        {
            var model = new RoleModel();
            return View(model);
        }
        [HttpPost]
        public IActionResult Create(RoleModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _roleService.Add(model);
                if (result.Status == ResultStatus.Exception)
                    throw new Exception(result.Message);
                if (result.Status == ResultStatus.Success)
                {
                    TempData["info"] = "Role is created.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message);
            }
            return View(model);
        }
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["danger"] = "Not found any role!";
                return RedirectToAction(nameof(Index));
            }
            var model = _roleService.Query().SingleOrDefault(r => r.Id == id.Value);
            return View(model);
        }
        [HttpPost]
        public IActionResult Edit(RoleModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _roleService.Update(model);
                if (result.Status == ResultStatus.Exception)
                    throw new Exception(result.Message);
                if (result.Status == ResultStatus.Success)
                {
                    TempData["info"] = "Role is deleted.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message);
            }
            return View(model);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["danger"] = "Not found any role!";
                return RedirectToAction(nameof(Index));
            }
            var model = _roleService.Query().SingleOrDefault(r => r.Id == id.Value);
            var users = _userService.Query(u => u.RoleId == id.Value);
            ViewBag.Users = null;
            if (users != null && users.Count() > 0)
            {
                if (users.Count() > 1)
                    ViewBag.Info = "There is an user have this role, are you sure to delete this role?";
                else
                    ViewBag.Info = "There are some user have this role, are you sure to delete this role?";
                ViewBag.Users = new SelectList(users, "Id", "UserName");
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult Delete(RoleModel model)
        {
            if (model.Id == (int)Roles.Admin || model.Id == (int)Roles.User)
            {
                var users = _userService.Query(u => u.RoleId == model.Id);
                ViewBag.Users = new SelectList(users, "Id", "UserName");
                ViewBag.Danger = "This role could not being deleted!";
                return View(model);
            }
            var result = _roleService.Delete(model.Id);
            if (result.Status == ResultStatus.Exception)
                throw new Exception(result.Message);
            TempData["info"] = "User deleted succesfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
