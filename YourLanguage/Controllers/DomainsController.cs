using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Business.Services.Bases;
using AppCore.Business.Models.Results;
using Business.Models;

namespace YourLanguage.Controllers
{
    public class DomainsController : Controller
    {
        private readonly IDomainService _domainService;

        public DomainsController(IDomainService domainService)
        {
            _domainService = domainService;
        }

        public IActionResult Index()
        {
            var domains = _domainService.Query().ToList();
            return View(domains);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
                return View("NotFound");
            var domainResult = _domainService.GetDomain(id.Value);
            if (domainResult.Status == ResultStatus.Exception)
                throw new Exception(domainResult.Message);
            if (domainResult.Status == ResultStatus.Error)
            {
                ModelState.AddModelError("", domainResult.Message);
                return View("Index");
            }
            return View(domainResult.Data);
        }

        public IActionResult Create()
        {
            var domain = new DomainModel();
            return View(domain);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DomainModel domain)
        {
            if (ModelState.IsValid)
            {
                var domainResult = _domainService.Add(domain);
                if (domainResult.Status == ResultStatus.Exception)
                    throw new Exception(domainResult.Message);
                if (domainResult.Status == ResultStatus.Success)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", domainResult.Message);
            }
            return View(domain);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
                return View("NotFound");
            var domainResult = _domainService.GetDomain(id.Value);
            if (domainResult.Status == ResultStatus.Exception)
                throw new Exception(domainResult.Message);
            return View(domainResult.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(DomainModel domain)
        {
            if (ModelState.IsValid)
            {
                var domainResult = _domainService.Update(domain);
                if (domainResult.Status == ResultStatus.Exception)
                    throw new Exception(domainResult.Message);
                if (domainResult.Status == ResultStatus.Success)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", domainResult.Message);
            }
            return View(domain);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
                return View("NotFound");
            var domainResult = _domainService.Delete(id.Value);
            if (domainResult.Status == ResultStatus.Exception)
                throw new Exception(domainResult.Message);
            if (domainResult.Status == ResultStatus.Success)
                return RedirectToAction(nameof(Index));
            ModelState.AddModelError("", domainResult.Message);
            return View(id.Value);
        }
    }
}
