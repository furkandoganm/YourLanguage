using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Business.Services.Bases;
using Business.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using YourLanguage.Settings;
using AppCore.Business.Models.Results;

namespace YourLanguage.Controllers
{
    public class WordsController : Controller
    {
        private readonly IWordService _wordService;
        private readonly IDomainService _domainService;

        public WordsController(IWordService wordService, IDomainService domainService)
        {
            _wordService = wordService;
            _domainService = domainService;
        }

        public IActionResult Index()
        {
            var words = _wordService.Query().ToList();
            return View(words);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
                return View("NotFound");
            var word = _wordService.Query().SingleOrDefault(w => w.Id == id.Value);
            if (word == null)
                return View("NotFound");
            return View(word);
        }

        public IActionResult Create()
        {
            ViewBag.Domains = new SelectList(_domainService.Query().ToList(), "Id", "Name");
            var word = new WordModel();
            return View(word);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(WordModel word, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                string filePath = null;
                string fileName = null;
                string fileExtension = null;
                bool saveFile = false;
                if (image != null && image.Length > 0)
                {
                    fileName = image.FileName;
                    fileExtension = Path.GetExtension(fileName);
                    string[] appSettingsFileExtensions = AppSettings.AcceptedImageExtensions.Split(",");
                    bool acceptedFileExtension = false;
                    foreach (string item in appSettingsFileExtensions)
                    {
                        if (fileExtension.ToUpper() == item.Trim().ToUpper())
                        {
                            acceptedFileExtension = true;
                            break;
                        }
                    }
                    if (!acceptedFileExtension)
                    {
                        ModelState.AddModelError("", "The acception image extensions are " + AppSettings.AcceptedImageExtensions);
                        ViewBag.Domains = new SelectList(_domainService.Query().ToList(), "Id", "Name", word.DomainId);
                        return View(word);
                    }
                    var acceptedFileLength = AppSettings.AcceptedImageMaximumLength * Math.Pow(1024, 2);
                    if (image.Length > acceptedFileLength)
                    {
                        ModelState.AddModelError("", "The maximum size of an image must be " + AppSettings.AcceptedImageMaximumLength + "MB");
                        ViewBag.Domains = new SelectList(_domainService.Query().ToList(), "Id", "Name", word.DomainId);
                        return View(word);
                    }
                    saveFile = true;
                }
                if (saveFile)
                {
                    fileName = Guid.NewGuid() + fileExtension;
                    filePath = Path.Combine("wwwroot", "files", "words", fileName);
                }
                word.ImagePath = fileName;
                var result = _wordService.Add(word);
                if (result.Status == ResultStatus.Exception)
                    throw new Exception(result.Message);
                if (result.Status == ResultStatus.Success)
                {
                    if (saveFile)
                    {
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            image.CopyTo(fileStream);
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message);
            }
            ViewBag.Domains = new SelectList(_domainService.Query().ToList(), "Id", "Name", word.DomainId);
            return View(word);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
                return View("NotFound");
            var wordResult = _wordService.GetWord(id.Value);
            if (wordResult.Status == ResultStatus.Exception)
                throw new Exception(wordResult.Message);
            if (wordResult.Status == ResultStatus.Error)
                ModelState.AddModelError("", wordResult.Message);
            ViewBag.Domains = new SelectList(_domainService.Query().ToList(), "Id", "Name", wordResult.Data.DomainId);
            return View(wordResult.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(WordModel word, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                string filePath = null;
                string fileName = null;
                string fileExtension = null;
                bool saveFile = false;
                if (image != null && image.Length > 0)
                {
                    fileName = image.FileName;
                    fileExtension = Path.GetExtension(fileName);
                    string[] appSettingsFileExtensions = AppSettings.AcceptedImageExtensions.Split(",");
                    bool acceptedFileExtension = false;
                    foreach (string item in appSettingsFileExtensions)
                    {
                        if (fileExtension.ToUpper() == item.Trim().ToUpper())
                        {
                            acceptedFileExtension = true;
                            break;
                        }
                    }
                    if (!acceptedFileExtension)
                    {
                        ModelState.AddModelError("", "The acception image extensions are " + AppSettings.AcceptedImageExtensions);
                        ViewBag.Domains = new SelectList(_domainService.Query().ToList(), "Id", "Name", word.DomainId);
                        return View(word);
                    }
                    var acceptedFileLength = AppSettings.AcceptedImageMaximumLength * Math.Pow(1024, 2);
                    if (image.Length > acceptedFileLength)
                    {
                        ModelState.AddModelError("", "The maximum size of an image must be " + AppSettings.AcceptedImageMaximumLength + "MB");
                        ViewBag.Domains = new SelectList(_domainService.Query().ToList(), "Id", "Name", word.DomainId);
                        return View(word);
                    }
                    saveFile = true;
                }
                var resultWord = _wordService.GetWord(word.Id).Data;
                if (string.IsNullOrWhiteSpace(resultWord.ImagePath) && saveFile)
                    fileName = Guid.NewGuid() + fileExtension;
                else
                    fileName = resultWord.ImagePath;
                word.ImagePath = fileName;
                var updatedWord = _wordService.Update(word);
                if (updatedWord.Status == ResultStatus.Exception)
                    throw new Exception(updatedWord.Message);
                if (updatedWord.Status == ResultStatus.Success)
                {
                    if (saveFile)
                    {
                        filePath = Path.Combine("wwwroot", "files", "words", fileName);
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            image.CopyTo(fileStream);
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", updatedWord.Message);
            }
            ViewBag.Domains = new SelectList(_domainService.Query().ToList(), "Id", "Name", word.DomainId);
            return View(word);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
                return View("NotFound");
            var wordResult = _wordService.Delete(id.Value);
            if (wordResult.Status == ResultStatus.Exception)
                throw new Exception(wordResult.Message);
            if (wordResult.Status == ResultStatus.Error)
            {
                ModelState.AddModelError("", wordResult.Message);
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult DeleteWordImage(int? id)
        {
            if (id == null)
                return View("NotFound");
            var wordResult = _wordService.GetWord(id.Value);
            if (wordResult.Status == ResultStatus.Exception)
                throw new Exception(wordResult.Message);
            if (wordResult.Status == ResultStatus.Success)
            {
                var word = wordResult.Data;
                if (!string.IsNullOrWhiteSpace(word.ImagePath))
                {
                    string filePath = Path.Combine("wwwroot", "files", "words", word.ImagePath);
                    word.ImagePath = null;
                    var updatedWord = _wordService.Update(word);
                    if (updatedWord.Status == ResultStatus.Exception)
                        throw new Exception(updatedWord.Message);
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }
                return RedirectToAction(nameof(Edit), new { id = wordResult.Data.Id });
            }
            ModelState.AddModelError("", wordResult.Message);
            return View("Edit", wordResult.Data);
        }
    }
}
