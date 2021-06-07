using AppCore.Business.Models.Results;
using Business.Models.Tests;
using Business.Services.Bases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using YourLanguage.Models;
using YourLanguage.Settings;

namespace YourLanguage.Controllers
{
    public class SpaceTestController : Controller
    {
        private readonly ISpaceTestService _spaceTestService;
        private readonly ITopicService _topicService;
        public SpaceTestController(ISpaceTestService spaceTestService, ITopicService topicService)
        {
            _spaceTestService = spaceTestService;
            _topicService = topicService;
        }
        public IActionResult Index(int? topicId)
        {
            ViewBag.Topic = false;
            var query = _spaceTestService.Query();
            if (topicId != null)
            {
                query = query.Where(st => st.TopicId == topicId.Value);
                ViewBag.Topic = true;
            }
            return View(query);
        }
        public IActionResult Test(int? topicId)
        {
            if (topicId == null)
            {
                TempData["IsDeleted"] = "Not found any question test!";
                return RedirectToAction("Index", "Study");
            }
            int pageNumber = 1;
            IEnumerable<SpaceTestViewModel> takenEnumerableQuery;
            List<SpaceTestViewModel> takenQuery = new List<SpaceTestViewModel>();
            var query = _spaceTestService.Query(st => st.TopicId == topicId.Value).Select(st => new SpaceTestViewModel()
            {
                Id = st.Id,
                QuestionPart1 = st.QuestionPart1,
                QuestionPart2 = st.QuestionPart2,
                Answer = st.AnswerPart1,
                AnswerWord = st.AnswerWord,
                IsCorrect = false,
                Topic = st.Topic
            });
            int pageCount = (int)Math.Ceiling((decimal)query.Count() / AppSettings.MaxQuestionCount);
            if (HttpContext.Session.GetString("takenqueryjsonspacetest") == null)
            {
                takenEnumerableQuery = query.Take(AppSettings.MaxQuestionCount);
                HttpContext.Session.SetString("takenqueryjsonspacetest", JsonConvert.SerializeObject(takenEnumerableQuery));
                takenQuery = takenEnumerableQuery.ToList();
            }
            else
            {
                pageNumber = JsonConvert.DeserializeObject<int>(HttpContext.Session.GetString("questionpagespacetest"));
                pageNumber += 1;
                takenEnumerableQuery = JsonConvert.DeserializeObject<IEnumerable<SpaceTestViewModel>>(HttpContext.Session.GetString("takenqueryjsonspacetest"));
                takenQuery = query.Skip(AppSettings.MaxQuestionCount * (pageNumber - 1)).Take(AppSettings.MaxQuestionCount).ToList();
                takenEnumerableQuery = (IEnumerable<SpaceTestViewModel>)takenQuery;
                HttpContext.Session.SetString("takenqueryjsonspacetest", JsonConvert.SerializeObject(takenEnumerableQuery));
            }
            var quizJson = JsonConvert.SerializeObject(query);
            HttpContext.Session.SetString("spacetestpage", quizJson);
            var spaceViewModel = new SpaceViewModel()
            {
                SpaceTest = takenQuery,
                PageCount = pageCount,
                PageNumber = pageNumber,
                Topic = query.FirstOrDefault().Topic.Name,
                IsCheck = false,
                IsCorrectAnswer = false,
                TopicId = topicId.Value
            };
            HttpContext.Session.SetString("questionpagespacetest", JsonConvert.SerializeObject(pageNumber));
            return View(spaceViewModel);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Test(SpaceViewModel viewModel)
        {
            
            var takenEnumerableQuery = JsonConvert.DeserializeObject<IEnumerable<SpaceTestViewModel>>(HttpContext.Session.GetString("takenqueryjsonspacetest"));
            foreach (var item in viewModel.SpaceTest)
            {
                var takenItem = takenEnumerableQuery.SingleOrDefault(st => st.Id == item.Id);
                item.QuestionPart1 = takenItem.QuestionPart1;
                item.QuestionPart2 = takenItem.QuestionPart2;
                item.Answer = takenItem.Answer;
                item.AnswerWord = takenItem.AnswerWord;
                if (viewModel.IsCheck)
                {
                    if (item.Answer.ToLower() == (item.TriedAnswer != null ? item.TriedAnswer.Trim().ToLower(): "") )
                    {
                        item.IsCorrect = true;
                    }
                    else
                    {
                        item.IsCorrect = false;
                    }
                }
                if (viewModel.IsCorrectAnswer)
                {
                    item.TriedAnswer = takenItem.Answer;
                }
            }
            viewModel.IsCheck = true;
            viewModel.TopicId = takenEnumerableQuery.FirstOrDefault().Topic.Id;
            return PartialView( "_Test", viewModel);
        }
        public IActionResult Details(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));
            var result = _spaceTestService.GetEntity(id.Value);
            if (result.Status == ResultStatus.Exception)
                throw new Exception(result.Message);
            if (result.Status == ResultStatus.Success)
                return View(result.Data);
            ModelState.AddModelError("", result.Message);
            return View("NotFound");
        }
        public IActionResult Create()
        {
            var model = new SpaceTestModel();
            ViewBag.Topic = new SelectList(_topicService.Query(), "Id", "Name");
            return View(model);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(SpaceTestModel spaceTestModel)
        {
            if (ModelState.IsValid)
            {
                spaceTestModel.CreatorId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value);
                var result = _spaceTestService.Add(spaceTestModel);
                if (result.Status == ResultStatus.Exception)
                    throw new Exception(result.Message);
                if (result.Status == ResultStatus.Success)
                    return RedirectToAction(nameof(Index), new { topicId = spaceTestModel.TopicId });
                ViewBag.Error = "Same space question is already exist!";
            }
            return View(spaceTestModel);
        }
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));
            var result = _spaceTestService.GetEntity(id.Value);
            if (result.Data.CreatorId != Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value))
            {
                TempData["IsDeleted"] = "You have not authorization to change it!";
                return RedirectToAction("Index", "Study");
            }
            if (result.Status == ResultStatus.Success)
            {
                ViewBag.Topic = new SelectList(_topicService.Query(), "Id", "Name", result.Data.TopicId);
                return View(result.Data);
            }
            if (result.Status == ResultStatus.Exception)
                throw new Exception(result.Message);
            ModelState.AddModelError("", result.Message);
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(SpaceTestModel spaceTestModel)
        {
            if (ModelState.IsValid)
            {
                spaceTestModel.CreatorId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value);
                var result = _spaceTestService.Update(spaceTestModel);
                if (result.Status == ResultStatus.Success)
                    return RedirectToAction(nameof(Index), new { topicId = spaceTestModel.TopicId });
                if (result.Status == ResultStatus.Exception)
                    throw new Exception(result.Message);
                ViewBag.Error = "Same space question is already exist!";
            }
            return View(spaceTestModel);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));
            int topicId = _spaceTestService.Query().SingleOrDefault(st => st.Id == id.Value).TopicId;
            var result = _spaceTestService.Delete(id.Value);
            if (result.Status == ResultStatus.Exception)
                throw new Exception(result.Message);
            return RedirectToAction(nameof(Index), new { topicId = topicId });
        }
    }
}
