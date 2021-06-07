using AppCore.Business.Models.Results;
using Business.Models.Tests;
using Business.Services.Bases;
using DataAccess.Repositories;
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
    public class QuestionTestController : Controller
    {
        private readonly IQuestionTestService _questionTestService;
        private readonly ITopicService _topicService;
        public QuestionTestController(IQuestionTestService questionTestService, ITopicService topicService)
        {
            _questionTestService = questionTestService;
            _topicService = topicService;
        }
        public IActionResult Details(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));
            var query = _questionTestService.Query().SingleOrDefault(q => q.Id == id.Value);
            return View(query);
        }
        public IActionResult Index(int? topicId)
        {
            ViewBag.Topic = false;
            var query = _questionTestService.Query();
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
                return RedirectToAction("Index", "Study");
            int queryCount = _questionTestService.Query(qt => qt.TopicId == topicId.Value).Count();
            int pageCount = (int)Math.Ceiling((decimal)queryCount / AppSettings.MaxQuestionCount);
            int pageNumber = 1;
            IEnumerable<QuestionTestModel> takenEnumerableQuery;
            List<QuestionTestModel> takenQuery = new List<QuestionTestModel>();
            if (HttpContext.Session.GetString("takenqueryjson") == null)
            {
                takenEnumerableQuery = _questionTestService.Query(qt => qt.TopicId == topicId.Value).Take(AppSettings.MaxQuestionCount);
                var takenQueryJson = JsonConvert.SerializeObject(takenEnumerableQuery);
                HttpContext.Session.SetString("takenqueryjson", takenQueryJson);
                takenQuery = takenEnumerableQuery.ToList();
            }
            else
            {
                pageNumber = JsonConvert.DeserializeObject<int>(HttpContext.Session.GetString("questionpage"));
                pageNumber += 1;
                var takenQueryJson = HttpContext.Session.GetString("takenqueryjson");
                takenEnumerableQuery = JsonConvert.DeserializeObject<IEnumerable<QuestionTestModel>>(takenQueryJson);
                takenQuery = _questionTestService.Query(qt => qt.TopicId == topicId.Value).Skip(AppSettings.MaxQuestionCount * (pageNumber - 1)).Take(AppSettings.MaxQuestionCount).ToList();
                //foreach (var item in takenEnumerableQuery.ToList())
                //{
                //    takenQuery.Remove(item);
                //}
                takenEnumerableQuery = (IEnumerable<QuestionTestModel>)takenQuery;
                var takenQueryJsonSet = JsonConvert.SerializeObject(takenEnumerableQuery);
                HttpContext.Session.SetString("takenqueryjson", takenQueryJsonSet);
            }
            var query = takenQuery.Select(qt => new QuestionTestViewModel()
            {
                Id = qt.Id,
                //QuestionNumber = i,
                Question = qt.Question,
                Answers = new List<AnswerViewModel>()
                    {
                        new AnswerViewModel()
                        {
                            Id = 1,
                            QuestionId = qt.Id,
                            Name = qt.WrongAnswer1
                        },
                        new AnswerViewModel()
                        {
                            Id = 2,
                            QuestionId = qt.Id,
                            Name = qt.WrongAnswer2
                        },
                        new AnswerViewModel()
                        {
                            Id = 3,
                            QuestionId = qt.Id,
                            Name = qt.CorrectAnswer
                        },
                        new AnswerViewModel()
                        {
                            Id = 4,
                            QuestionId = qt.Id,
                            Name = qt.WrongAnswer3
                        }
                    },
            });
            var questionsViewModel = new QuestionsViewModel()
            {
                Topic = _topicService.Query(t => t.Id == topicId.Value).FirstOrDefault().Name,
                TestCount = query.Count(),
                Questions = query.ToList(),
                QuestionPageCount = pageCount,
                QuestionPage = pageNumber,
                IsCorrectAnswer = false,
                IsCheck = false,
                TopicId = topicId.Value
            };
            var quizJson = JsonConvert.SerializeObject(questionsViewModel.QuestionPage);
            HttpContext.Session.SetString("questionpage", quizJson);
            return View(questionsViewModel);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Test(QuestionsViewModel questionsViewModel)
        {
            List<QuestionTestModel> takenListQuery = new List<QuestionTestModel>();
            IEnumerable<QuestionTestModel> takenQuery;
            var questionPage = JsonConvert.DeserializeObject<int>(HttpContext.Session.GetString("questionpage"));
            var pageNumber = questionsViewModel.QuestionPage;
            var takenQueryJson = HttpContext.Session.GetString("takenqueryjson");
            takenQuery = JsonConvert.DeserializeObject<IEnumerable<QuestionTestModel>>(takenQueryJson);
            takenListQuery = takenQuery.ToList();
            #region deneme 4
            //if (pageNumber != questionPage)
            //{
            //    takenQuery = _questionTestService.Query(qt => qt.Topic.Name == questionsViewModel.Topic);
            //    if (pageNumber > questionPage)
            //    {
            //        if (pageNumber > questionsViewModel.QuestionPageCount)
            //            ViewBag.LastPage = "There is no any question from this topic. You may try change the topic.";
            //        else if (pageNumber == questionsViewModel.QuestionPageCount)
            //            takenQuery = takenQuery.Skip((pageNumber - 1) * AppSettings.MaxQuestionCount);
            //        else
            //            takenQuery = takenQuery.Skip((pageNumber - 1) * AppSettings.MaxQuestionCount).Take(AppSettings.MaxQuestionCount);
            //    }
            //    if (pageNumber < questionPage)
            //    {
            //        if (pageNumber < 1)
            //            ViewBag.LastPage = "There is no any question from this topic. You may try change the topic.";
            //        else if (pageNumber == 1)
            //            takenQuery = takenQuery.Take(AppSettings.MaxQuestionCount);
            //        else
            //            takenQuery = takenQuery.Skip((pageNumber - 1) * AppSettings.MaxQuestionCount).Take(AppSettings.MaxQuestionCount);
            //    }
            //    takenListQuery = takenQuery.ToList();
            //    var takenQueryJson = JsonConvert.SerializeObject(takenQuery);
            //    HttpContext.Session.SetString("takenqueryjson", takenQueryJson);
            //    return RedirectToAction("Test", new { topicId = takenQuery.FirstOrDefault().TopicId });
            //}
            #endregion
            var query = takenListQuery.Select(qt => new QuestionTestViewModel()
            {
                Id = qt.Id,
                Question = qt.Question,
                Answers = new List<AnswerViewModel>()
                    {
                        new AnswerViewModel()
                        {
                            Id = 1,
                            QuestionId = qt.Id,
                            Name = qt.WrongAnswer1
                        },
                        new AnswerViewModel()
                        {
                            Id = 2,
                            QuestionId = qt.Id,
                            Name = qt.WrongAnswer2
                        },
                        new AnswerViewModel()
                        {
                            Id = 3,
                            QuestionId = qt.Id,
                            Name = qt.CorrectAnswer
                        },
                        new AnswerViewModel()
                        {
                            Id = 4,
                            QuestionId = qt.Id,
                            Name = qt.WrongAnswer3
                        }
                    }
            });
            foreach (var item in questionsViewModel.Questions)
            {
                item.Question = query.SingleOrDefault(q => q.Id == item.Id).Question;
                item.Answers = query.SingleOrDefault(q => q.Id == item.Id).Answers;

                if (item.SelectedAnswer.Id == 3)
                    item.IsCorrect = true;
                else if (item.SelectedAnswer.Id > 4)
                    item.IsCorrect = null;
                else
                    item.IsCorrect = false;
                item.SelectedAnswer = query.SingleOrDefault(q => q.Id == item.Id).SelectedAnswer;
            }
            #region deneme 3
            //if (pageNumber != questionPage)
            //{
            //    questionsViewModel.Questions.Clear();
            //    questionsViewModel.IsCheck = false;
            //    foreach (var item in query)
            //    {
            //        questionsViewModel.Questions.Add(item);
            //    }
            //}
            //else
            //{
            //    foreach (var item in questionsViewModel.Questions)
            //    {
            //        item.Question = query.SingleOrDefault(q => q.Id == item.Id).Question;
            //        item.Answers = query.SingleOrDefault(q => q.Id == item.Id).Answers;

            //        if (item.SelectedAnswer.Id == 3)
            //            item.IsCorrect = true;
            //        else if (item.SelectedAnswer.Id > 4)
            //            item.IsCorrect = null;
            //        else
            //            item.IsCorrect = false;
            //        item.SelectedAnswer = query.SingleOrDefault(q => q.Id == item.Id).SelectedAnswer;
            //    }
            //}
            #endregion
            #region deneme 2 deneme 1
            //foreach (var item in questionsViewModel.Questions)
            //{
            //    item.Question = query.SingleOrDefault(q => q.Id == item.Id).Question;
            //    item.Answers = query.SingleOrDefault(q => q.Id == item.Id).Answers;

            //    if (pageNumber == questionPage)
            //    {
            //        if (item.SelectedAnswer.Id == 3)
            //            item.IsCorrect = true;
            //        else
            //            item.IsCorrect = false;
            //    }
            //}
            #endregion
            #region deneme 1
            //if (questionsViewModel.IsCheck)
            //{
            //    var query = takenListQuery.Select(qt => new QuestionTestViewModel()
            //    {
            //        Id = qt.Id,
            //        Question = qt.Question,
            //        Answers = new List<AnswerViewModel>()
            //        {
            //            new AnswerViewModel()
            //            {
            //                Id = 1,
            //                QuestionId = qt.Id,
            //                Name = qt.WrongAnswer1
            //            },
            //            new AnswerViewModel()
            //            {
            //                Id = 2,
            //                QuestionId = qt.Id,
            //                Name = qt.WrongAnswer2
            //            },
            //            new AnswerViewModel()
            //            {
            //                Id = 3,
            //                QuestionId = qt.Id,
            //                Name = qt.CorrectAnswer
            //            },
            //            new AnswerViewModel()
            //            {
            //                Id = 4,
            //                QuestionId = qt.Id,
            //                Name = qt.WrongAnswer3
            //            }
            //        },
            //    });
            //    foreach (var item in questionsViewModel.Questions)
            //    {
            //        item.Question = query.SingleOrDefault(q => q.Id == item.Id).Question;
            //        item.Answers = query.SingleOrDefault(q => q.Id == item.Id).Answers;

            //        if (pageNumber == questionPage)
            //        {
            //            if (item.SelectedAnswer.Id == 3)
            //                item.IsCorrect = true;
            //            else
            //                item.IsCorrect = false;
            //        }
            //    }
            //}
            #endregion
            HttpContext.Session.SetString("questionpage", JsonConvert.SerializeObject(questionsViewModel.QuestionPage));
            return PartialView("_Test", questionsViewModel);
        }
        public ActionResult Create()
        {
            var model = new QuestionTestModel();
            ViewBag.Topics = new SelectList(_topicService.Query().ToList(), "Id", "Name");
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(QuestionTestModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string userId = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value;
                //var user = _userService.Query(u => u.Id == Convert.ToInt32(userId)).SingleOrDefault();
                viewModel.CreatorId = Convert.ToInt32(userId);
                var result = _questionTestService.Add(viewModel);
                if (result.Status == ResultStatus.Exception)
                    throw new Exception(result.Message);
                if (result.Status == ResultStatus.Success)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", result.Message);
            }
            return View(viewModel);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return View("NotFound");
            var model = _questionTestService.Query().SingleOrDefault(q => q.Id == id.Value);
            string userId = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value;
            if (model.CreatorId != Convert.ToInt32(userId))
            {
                TempData["info"] = "You have not authorization to change it.";
                return RedirectToAction("Index", "Study");
            }
            ViewBag.Topics = new SelectList(_topicService.Query().ToList(), "Id", "Name", model.TopicId);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(QuestionTestModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.CreatorId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value);
                var result = _questionTestService.Update(viewModel);
                if (result.Status == ResultStatus.Exception)
                    throw new Exception(result.Message);
                if (result.Status == ResultStatus.Success)
                    return RedirectToAction(nameof(Index), new { topicId = viewModel.TopicId });
                ModelState.AddModelError("", result.Message);
            }
            ViewBag.Topics = new SelectList(_topicService.Query().ToList(), "Id", "Name", viewModel.TopicId);
            return View(viewModel);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return View("NotFound");
            int topicId = _questionTestService.Query().SingleOrDefault(q => q.Id == id.Value).TopicId;
            var result = _questionTestService.Delete(id.Value);
            if (result.Status == ResultStatus.Exception)
                throw new Exception(result.Message);
            TempData["info"] = "Question is deleted.";
            if (_questionTestService.Query(q => q.TopicId == topicId).Count() <= 0)
                return RedirectToAction("Index", "Study");
            return RedirectToAction(nameof(Index), new { topicId = topicId });

        }
    }
}
