using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccess.EntityFramework.Contexts;
using Entities.Entities;
using Business.Services.Bases;
using YourLanguage.Models;
using System.Security.Claims;
using AppCore.Business.Models.Results;
using Business.Models.Studies;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Entities.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Business.Models;
using YourLanguage.Settings;

namespace YourLanguage.Controllers
{
    [Authorize]
    public class StudyController : Controller
    {
        private readonly IUserWordService _userWordService;
        private readonly IQuestionTestService _questionService;
        private readonly ISpaceTestService _spaceService;
        private readonly IUserService _userService;
        private readonly IWordService _wordService;

        public StudyController(IUserWordService userWordService, IQuestionTestService questionService, ISpaceTestService spaceService, IUserService userService, IWordService wordService)
        {
            _userWordService = userWordService;
            _questionService = questionService;
            _spaceService = spaceService;
            _userService = userService;
            _wordService = wordService;
        }
        public IActionResult Index()
        {
            HttpContext.Session.Remove("takenqueryjson");
            int userId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value);
            var user = _userService.Query(u => u.Id == userId).SingleOrDefault();
            var quiz = _userWordService.GetQuizGroupByListName(userId).Data;
            var questionTests = _questionService.GetQuestionTestsGroupByDomain().Data;
            var spaceTests = _spaceService.GetSpaceTestsGroupByDomain().Data;
            var viewModel = new MainViewModel()
            {
                StudyModels = quiz,
                SpaceTests = spaceTests,
                QuestionTest = questionTests,
                NumbersofWordLearned = user.NumbersofWordLearned
            };
            if (TempData["IsDeleted"] != null)
            {
                if (TempData["IsDeleted"].ToString() != "")
                    ViewBag.IsDeleted = TempData["IsDeleted"].ToString();
                TempData["IsDeleted"] = null;
            }
            if (TempData["info"] != null)
            {
                if (TempData["info"].ToString() != "")
                    ViewBag.Info = TempData["info"].ToString();
                TempData["info"] = null;
            }
            return View(viewModel);
        }

        // GET: Study/Details/5
        public IActionResult Details(string listName)
        {
            if (listName == "")
            {
                TempData["danger"] = "Not found any word!";
                return RedirectToAction(nameof(Index));
            }
            var userId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value);
            var list = _userWordService.Query(uw => uw.UserId == userId && uw.WordListName == listName).Select(uw => new StudyModel()
            {
                Id = uw.Id,
                Vocable = uw.Word.Vocable.Trim(),
                Mean = uw.Word.Mean.Trim(),
                LearningDegree = (int)uw.LearningDegree,
                ListName = uw.WordListName
            }).ToList();
            return View(list);
        }

        // GET: Study/Create
        public IActionResult Create()
        {
            var viewModel = new StudyViewModel();
            viewModel.QuizCount = AppSettings.QuizListCount;
            var studyModels = new List<StudyModel>();
            var words = _wordService.Query();
            viewModel.SelectList = new SelectList(words, "Id", "Mean");
            for (int i = 0; i < viewModel.QuizCount; i++)
            {
                studyModels.Add(new StudyModel()
                {
                    UserId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value)
                });
            }
            var studyModelJson = JsonConvert.SerializeObject(studyModels);
            HttpContext.Session.SetString("studyModelJson", studyModelJson);
            viewModel.StudyModel = studyModels;
            //viewModel.IsQuizCorrect = false;
            return View(viewModel);
        }

        // POST: Study/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult Create(StudyViewModel viewModel)
        {
            string quizcount;
            var words = _wordService.Query();
            if (HttpContext.Session.GetString("quizcount") == null)
            {
                quizcount = JsonConvert.SerializeObject(AppSettings.QuizListCount);
                HttpContext.Session.SetString("quizcount", quizcount);
            }
            quizcount = HttpContext.Session.GetString("quizcount");
            int exQuizCount = JsonConvert.DeserializeObject<int>(quizcount);
            if (exQuizCount < viewModel.QuizCount)
            {
                //ViewBag.QuizMaxLength = null;
                if (viewModel.QuizCount > AppSettings.MaxWordListCount)
                {
                    ViewBag.QuizMaxLength = ("The list can has maximum " + AppSettings.MaxWordListCount + " words!");
                    viewModel.QuizCount = viewModel.QuizCount - 1;
                }
                else
                {
                    quizcount = JsonConvert.SerializeObject(viewModel.QuizCount);
                    HttpContext.Session.SetString("quizcount", quizcount);
                    viewModel.StudyModel.Add(new StudyModel()
                    {
                        UserId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value)
                    });
                }
                viewModel.SelectList = new SelectList(words, "Id", "Mean");
                return View(viewModel);
            }
            if (viewModel.ListName == null)
            {
                ViewBag.ListNameRequired = "List name is required!";
                viewModel.SelectList = new SelectList(words, "Id", "Mean");
                return View(viewModel);
            }
            foreach (var item in viewModel.StudyModel)
            {
                var userWord = new UserWordModel()
                {
                    Active = true,
                    UserId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value),
                    WordListName = viewModel.ListName.Trim(),
                    WordId = item.Id
                };
                var result = _userWordService.Add(userWord);
                if (result.Status == ResultStatus.Exception)
                    throw new Exception(result.Message);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Study/Edit/5
        public IActionResult Edit(string listName)
        {
            if (listName == "")
            {
                TempData["danger"] = "Not found any word!";
                return RedirectToAction(nameof(Index));
            }
            var userId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value);
            var studyModelList = _userWordService.GetQuiz(userId, listName.Trim());
            var words = _wordService.Query();
            var studyViewModel = new StudyViewModel();
            studyViewModel.ListName = listName.Trim();
            var selectList = new SelectList(words, "Id", "Mean");
            studyViewModel.SelectList = selectList;
            studyViewModel.StudyModel = studyModelList.Data;
            return View(studyViewModel);
        }

        // POST: Study/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(StudyViewModel viewModel)
        {
            if (viewModel.Word.Id > 0)
            {
                var studyViewModel = new StudyViewModel();
                var userId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value);
                var word = _wordService.Query().SingleOrDefault(w => w.Id == viewModel.Word.Id);
                var model = new UserWordModel()
                {
                    UserId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value),
                    Active = true,
                    WordId = word.Id,
                    WordListName = viewModel.ListName.Trim()
                };
                var result = _userWordService.Add(model);
                if (result.Status == ResultStatus.Exception)
                    throw new Exception(result.Message);
                ViewBag.Error = null;
                if (result.Status == ResultStatus.Error)
                {
                    //ModelState.AddModelError("", result.Message);
                    ViewBag.Error = result.Message;
                }
                var words = _wordService.Query();
                studyViewModel.SelectList = new SelectList(words, "Id", "Mean");
                studyViewModel.StudyModel = _userWordService.GetQuiz(userId, viewModel.ListName.Trim()).Data;
                studyViewModel.ListName = viewModel.ListName.Trim();
                return View(studyViewModel);
            }
            return View(viewModel);
        }

        // GET: Study/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["danger"] = "Not found any word!";
                return RedirectToAction(nameof(Index));
            }
            var userWord = _userWordService.Query().SingleOrDefault(uw => uw.Id == id.Value);
            var listName = userWord.WordListName;
            userWord.Active = false;
            var result = _userWordService.Update(userWord);
            if (result.Status == ResultStatus.Exception)
                throw new Exception(result.Message);
            if (result.Status == ResultStatus.Error)
                ModelState.AddModelError("", result.Message);
            var userId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value);
            var words = _wordService.Query();
            var studyViewModel = new StudyViewModel()
            {
                SelectList = new SelectList(words, "Id", "Mean"),
                StudyModel = _userWordService.GetQuiz(userId, listName.Trim()).Data,
                ListName = listName.Trim()
            };
            //studyViewModel.SelectList = new SelectList(words, "Id", "Mean");
            //studyViewModel.StudyModel = _userWordService.GetQuiz(userId, listName.Trim()).Data;
            //studyViewModel.ListName = listName.Trim();
            return RedirectToAction(nameof(Edit), studyViewModel);
        }

        // POST: Study/Delete/5
        //public IActionResult DeleteWordList(int? id)
        public IActionResult DeleteWordList(int? id, string listName = "")
        {
            //string listName = "";
            //var result = _userWordService.UpdateList("", id.Value);
            if (listName == null || !id.HasValue)
                return View("NotFound");
            Result result;
            if (listName != "")
            {
                result = _userWordService.UpdateList(listName.Trim());
                if (result.Status == ResultStatus.Exception)
                    throw new Exception(result.Message);
            }
            if (id.HasValue)
            {
                result = _userWordService.UpdateList(_userWordService.Query(uw => uw.Id == id.Value).FirstOrDefault().WordListName);
                if (result.Status == ResultStatus.Exception)
                    throw new Exception(result.Message);
            }
            TempData["info"] = "All list is deleted.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Quiz(string listName, int wordNumber = 1)
        {
            string userId = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value;
            string quizJson;
            StudyViewModel viewModel;
            if (listName == null)
                listName = "quiz";
            listName = listName.Trim();
            if (HttpContext.Session.GetString(listName) == null)
            {
                var query = _userWordService.GetQuiz(Convert.ToInt32(userId), listName);
                if (query.Data == null || query.Data.Count() < 1)
                {
                    TempData["IsDeleted"] = "You have no any word. To obtain, click the add new one.";
                    return RedirectToAction(nameof(Index));
                }    
                if (query.Status == ResultStatus.Exception)
                    throw new Exception(query.Message);
                if (query.Status == ResultStatus.Error)
                {
                    TempData["Message"] = query.Message;
                    return RedirectToAction(nameof(Index));
                }
                viewModel = new StudyViewModel();
                List<StudyModel> studyModels = new List<StudyModel>();
                //foreach (var item in query.Data)
                for (int i = 1; i <= query.Data.Count; i++)
                {
                    var item = query.Data[(i - 1)];
                    studyModels.Add(new StudyModel()
                    {
                        Id = item.Id,
                        SessionId = i,
                        Vocable = item.Vocable,
                        Mean = item.Mean,
                        LearningDegree = item.LearningDegree
                    });
                    //viewModel.StudyModel.Add(new StudyModel()
                    //{
                    //    Id = item.Id,
                    //    SessionId = i,
                    //    Vocable = item.Vocable,
                    //    Mean = item.Mean
                    //});
                }
                viewModel.StudyModel = studyModels;
                viewModel.QuizNumber = wordNumber;
                viewModel.QuizCount = query.Data.Count;
                //viewModel.IsQuizCorrect = null;
                viewModel.ListName = listName;
                viewModel.WasShown = false;
                quizJson = JsonConvert.SerializeObject(viewModel);
                HttpContext.Session.SetString(listName, quizJson);
            }
            quizJson = HttpContext.Session.GetString(listName);
            viewModel = JsonConvert.DeserializeObject<StudyViewModel>(quizJson);
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Quiz(StudyViewModel viewModel)
        {
            var quizJson = HttpContext.Session.GetString(viewModel.ListName.Trim());
            var exViewModel = JsonConvert.DeserializeObject<StudyViewModel>(quizJson);
            if (viewModel.QuizNumber == exViewModel.QuizNumber)
            {
                var study = exViewModel.StudyModel[viewModel.QuizNumber - 1];
                if (viewModel.IsQuizCorrect == true)
                {
                    if (study.LearningDegree <= 4)
                    {
                        study.LearningDegree += 1;
                        _userWordService.Update(study.Id);
                        if (exViewModel.StudyModel.Count <= 1)
                            return RedirectToAction(nameof(Index));
                    }
                    exViewModel.IsQuizCorrect = true;
                }
                if (viewModel.IsQuizCorrect == false)
                {
                    if (study.LearningDegree > 1)
                        study.LearningDegree -= 1;
                    exViewModel.IsQuizCorrect = false;
                }
                if (viewModel.WasShown)
                {
                    exViewModel.WasShown = true;
                }
                else
                {
                    exViewModel.WasShown = false;
                }
                exViewModel.StudyModel[viewModel.QuizNumber - 1] = study;
                quizJson = JsonConvert.SerializeObject(exViewModel);
                HttpContext.Session.SetString(viewModel.ListName, quizJson);
                //return PartialView("_WordsQuiz", study);
                return PartialView("_WordsQuiz", exViewModel);
            }
            if (exViewModel.StudyModel.Where(st => st.LearningDegree > 4) != null && exViewModel.StudyModel.Where(st => st.LearningDegree > 4).Count() > 0)
            {
                int id = exViewModel.StudyModel.SingleOrDefault(st => st.LearningDegree > 4).Id;
                var model = _userWordService.Query().SingleOrDefault(uw => uw.Id == id);
                model.LearningDegree = LearningDegree.Intermediate;
                _userWordService.Update(model);
            }
            exViewModel.StudyModel = exViewModel.StudyModel.Where(sm => sm.LearningDegree <= 4).ToList();
            if (viewModel.QuizNumber < 1)
                viewModel.QuizNumber = viewModel.QuizCount;
            if (viewModel.QuizNumber > viewModel.QuizCount)
                viewModel.QuizNumber = 1;
            exViewModel.QuizNumber = viewModel.QuizNumber;
            quizJson = JsonConvert.SerializeObject(exViewModel);
            HttpContext.Session.SetString(viewModel.ListName.Trim(), quizJson);
            exViewModel.IsQuizCorrect = null;
            exViewModel.WasShown = false;
            return PartialView("_WordsQuiz", exViewModel);
        }
    }
}
