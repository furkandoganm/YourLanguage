using AppCore.Business.Models.Results;
using Business.Models.Tests;
using Business.Services.Bases;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YourLanguage.Controllers
{
    public class TopicsController : Controller
    {
        private readonly ITopicService _topicService;

        public TopicsController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        public IActionResult Index()
        {
            var topics = _topicService.Query();
            return View(topics);
        }

        //public IActionResult Details(int? id)
        //{
        //    if (id == null)
        //        return View("NotFound");
        //    var topicResult = _topicService.GetTopic(id.Value);
        //    if (topicResult.Status == ResultStatus.Exception)
        //        throw new Exception(topicResult.Message);
        //    if (topicResult.Status == ResultStatus.Error)
        //    {
        //        ModelState.AddModelError("", topicResult.Message);
        //        return View("Index");
        //    }
        //    return View(topicResult.Data);
        //}

        public IActionResult Create()
        {
            var topic = new TopicModel();
            return View(topic);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TopicModel topic)
        {
            if (ModelState.IsValid)
            {
                var topicResult = _topicService.Add(topic);
                if (topicResult.Status == ResultStatus.Exception)
                    throw new Exception(topicResult.Message);
                if (topicResult.Status == ResultStatus.Success)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", topicResult.Message);
            }
            return View(topic);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
                return View("NotFound");
            var topicResult = _topicService.GetTopic(id.Value);
            if (topicResult.Status == ResultStatus.Exception)
                throw new Exception(topicResult.Message);
            return View(topicResult.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TopicModel topic)
        {
            if (ModelState.IsValid)
            {
                var topicResult = _topicService.Update(topic);
                if (topicResult.Status == ResultStatus.Exception)
                    throw new Exception(topicResult.Message);
                if (topicResult.Status == ResultStatus.Success)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", topicResult.Message);
            }
            return View(topic);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
                return View("NotFound");
            var topicResult = _topicService.Delete(id.Value);
            if (topicResult.Status == ResultStatus.Exception)
                throw new Exception(topicResult.Message);
            if (topicResult.Status == ResultStatus.Success)
                return RedirectToAction(nameof(Index));
            ModelState.AddModelError("", topicResult.Message);
            return RedirectToAction(nameof(Index));
        }
    }
}
