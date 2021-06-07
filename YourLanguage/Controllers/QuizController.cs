using Business.Services.Bases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YourLanguage.Models;
using YourLanguage.Settings;

namespace YourLanguage.Controllers
{
    public class QuizController : Controller
    {
        private readonly IUserWordService _userWordService;
        private readonly IQuestionTestService _questionTestService;
        private readonly ISpaceTestService _spaceTestService;
        public QuizController(IUserWordService userWordService, IQuestionTestService questionTestService, ISpaceTestService spaceTestService)
        {
            _userWordService = userWordService;
            _questionTestService = questionTestService;
            _spaceTestService = spaceTestService;
        }
        public IActionResult Index()
        {
            var userWords = _userWordService.Query().ToList();
            var questionTests = _questionTestService.Query().ToList();
            var spaceTests = _spaceTestService.Query().ToList();
            TestViewModel viewModel = new TestViewModel();
            List<QuizViewModel> tempQuizViewModel = new List<QuizViewModel>();
            for (int i = 0; i < AppSettings.TestCount / 3; i++)
            {
                Random rndUW = new Random();
                Random rndQT = new Random();
                Random rndST = new Random();
                int rUW = rndUW.Next(userWords.Count);
                int rQT = rndQT.Next(questionTests.Count());
                int rST = rndST.Next(spaceTests.Count());
                tempQuizViewModel.Add(new QuizViewModel()
                {
                    UserWord = new UserWordViewModel()
                    {
                        Id = userWords[rUW].Id,
                        Vocable = userWords[rUW].Word.Vocable,
                        Mean = userWords[rUW].Word.Mean,
                        IsCorrect = false
                    },
                    QuestionTest = new QuestionTestViewModel()
                    {
                        Id = questionTests[rQT].Id,
                        Question = questionTests[rQT].Question,
                        Answers = new List<AnswerViewModel>()
                        {
                            new AnswerViewModel()
                            {
                                Id = 1,
                                QuestionId = questionTests[rQT].Id,
                                Name = questionTests[rQT].WrongAnswer1
                            },
                            new AnswerViewModel()
                            {
                                Id = 2,
                                QuestionId = questionTests[rQT].Id,
                                Name = questionTests[rQT].WrongAnswer2
                            },
                            new AnswerViewModel()
                            {
                                Id = 3,
                                QuestionId = questionTests[rQT].Id,
                                Name = questionTests[rQT].CorrectAnswer
                            },
                            new AnswerViewModel()
                            {
                                Id = 4,
                                QuestionId = questionTests[rQT].Id,
                                Name = questionTests[rQT].WrongAnswer3
                            }
                        },
                        IsCorrect = false
                    },
                    SpaceTest = new SpaceTestViewModel()
                    {
                        Id = spaceTests[rST].Id,
                        QuestionPart1 = spaceTests[rST].QuestionPart1,
                        Answer = spaceTests[rST].AnswerPart1,
                        QuestionPart2 = spaceTests[rST].QuestionPart2,
                        AnswerWord = spaceTests[rST].AnswerWord,
                        Topic = spaceTests[rST].Topic,
                        IsCorrect = false
                    },
                    QuizNumber = i + 1,
                    QuestionNumber = 1,
                    NextPrevious = null,
                    Finish = false,
                    IsCorrect = false
                });
                viewModel.Test = tempQuizViewModel;
                userWords.Remove(userWords[rUW]);
                questionTests.Remove(questionTests[rQT]);
                spaceTests.Remove(spaceTests[rST]);
            }
            viewModel.TestNumber = 1;
            viewModel.WhichQuiz = 1;
            var quizJson = JsonConvert.SerializeObject(viewModel);
            HttpContext.Session.SetString("mixedquiz", quizJson);
            return View(viewModel.Test[0]);
        }
        [HttpPost]
        public IActionResult Index(QuizViewModel viewModel)
        {
            QuizViewModel newViewModel = new QuizViewModel();
            var quizJson = HttpContext.Session.GetString("mixedquiz");
            var exViewModels = JsonConvert.DeserializeObject<TestViewModel>(quizJson);
            if (viewModel.NextPrevious == false)
            {
                if (viewModel.QuestionNumber == 1)
                {
                    if (viewModel.UserWord.Answer == viewModel.UserWord.Mean)
                        exViewModels.Test.SingleOrDefault(q => q.UserWord.Id == viewModel.UserWord.Id).UserWord.IsCorrect = true;
                    else
                        exViewModels.Test.SingleOrDefault(q => q.UserWord.Id == viewModel.UserWord.Id).UserWord.IsCorrect = false;
                    if (exViewModels.TestNumber > 1)
                        exViewModels.TestNumber -= 1;
                    exViewModels.Test.SingleOrDefault(q => q.UserWord.Id == viewModel.UserWord.Id).UserWord.Answer = viewModel.UserWord.Answer;
                    newViewModel = exViewModels.Test.SingleOrDefault(q => q.QuizNumber == exViewModels.TestNumber);
                }
                if (viewModel.QuestionNumber == 2)
                {
                    if (viewModel.QuestionTest.SelectedAnswer.Id == 3)
                        exViewModels.Test.SingleOrDefault(qt => qt.QuestionTest.Id == viewModel.QuestionTest.Id).QuestionTest.IsCorrect = true;
                    else if (viewModel.QuestionTest.SelectedAnswer.Id > 4)
                        viewModel.QuestionTest.SelectedAnswer.Id = 6;
                    else
                        exViewModels.Test.SingleOrDefault(qt => qt.QuestionTest.Id == viewModel.QuestionTest.Id).QuestionTest.IsCorrect = false;
                    exViewModels.Test.SingleOrDefault(q => q.QuizNumber == exViewModels.TestNumber).QuestionNumber -= 1;
                    exViewModels.Test.SingleOrDefault(q => q.QuestionTest.Id == viewModel.QuestionTest.Id).QuestionTest.SelectedAnswer = viewModel.QuestionTest.SelectedAnswer;
                    newViewModel = exViewModels.Test.SingleOrDefault(q => q.QuizNumber == exViewModels.TestNumber);
                }
                if (viewModel.QuestionNumber == 3)
                {
                    if (viewModel.SpaceTest.TriedAnswer != null)
                    {
                        if (viewModel.SpaceTest.TriedAnswer.Trim().ToLower() == viewModel.SpaceTest.Answer.ToLower())
                            exViewModels.Test.SingleOrDefault(uw => uw.SpaceTest.Id == viewModel.SpaceTest.Id).SpaceTest.IsCorrect = true;
                        else
                            exViewModels.Test.SingleOrDefault(uw => uw.SpaceTest.Id == viewModel.SpaceTest.Id).SpaceTest.IsCorrect = false;
                    }
                    exViewModels.Test.SingleOrDefault(q => q.QuizNumber == exViewModels.TestNumber).QuestionNumber -= 1;
                    exViewModels.Test.SingleOrDefault(q => q.SpaceTest.Id == viewModel.SpaceTest.Id).SpaceTest.TriedAnswer = viewModel.SpaceTest.TriedAnswer;
                    newViewModel = exViewModels.Test.SingleOrDefault(q => q.QuizNumber == exViewModels.TestNumber);
                }
            }
            if (viewModel.NextPrevious == true)
            {
                if (viewModel.QuestionNumber == 1)
                {
                    if ((viewModel.UserWord.Answer != null ? viewModel.UserWord.Answer.Trim().ToLower() : "") == viewModel.UserWord.Mean.ToLower())
                        exViewModels.Test.SingleOrDefault(q => q.UserWord.Id == viewModel.UserWord.Id).UserWord.IsCorrect = true;
                    else
                        exViewModels.Test.SingleOrDefault(q => q.UserWord.Id == viewModel.UserWord.Id).UserWord.IsCorrect = false;
                    exViewModels.Test.SingleOrDefault(q => q.QuizNumber == exViewModels.TestNumber).QuestionNumber += 1;
                    exViewModels.Test.SingleOrDefault(q => q.UserWord.Id == viewModel.UserWord.Id).UserWord.Answer = viewModel.UserWord.Answer;
                    newViewModel = exViewModels.Test.SingleOrDefault(q => q.QuizNumber == exViewModels.TestNumber);
                }
                if (viewModel.QuestionNumber == 2)
                {
                    if (viewModel.QuestionTest.SelectedAnswer.Id == 3)
                        exViewModels.Test.SingleOrDefault(q => q.QuestionTest.Id == viewModel.QuestionTest.Id).QuestionTest.IsCorrect = true;
                    else if (viewModel.QuestionTest.SelectedAnswer.Id > 4)
                        viewModel.QuestionTest.SelectedAnswer.Id = 6;
                    else
                        exViewModels.Test.SingleOrDefault(q => q.QuestionTest.Id == viewModel.QuestionTest.Id).QuestionTest.IsCorrect = false;
                    exViewModels.Test.SingleOrDefault(q => q.QuizNumber == exViewModels.TestNumber).QuestionNumber += 1;
                    exViewModels.Test.SingleOrDefault(q => q.QuestionTest.Id == viewModel.QuestionTest.Id).QuestionTest.SelectedAnswer = viewModel.QuestionTest.SelectedAnswer;
                    newViewModel = exViewModels.Test.SingleOrDefault(q => q.QuizNumber == exViewModels.TestNumber);
                }
                if (viewModel.QuestionNumber == 3)
                {
                    if (viewModel.SpaceTest.TriedAnswer != null)
                    {
                        if (viewModel.SpaceTest.TriedAnswer.Trim().ToLower() == viewModel.SpaceTest.Answer.ToLower())
                            exViewModels.Test.SingleOrDefault(uw => uw.SpaceTest.Id == viewModel.SpaceTest.Id).SpaceTest.IsCorrect = true;
                        else
                            exViewModels.Test.SingleOrDefault(uw => uw.SpaceTest.Id == viewModel.SpaceTest.Id).SpaceTest.IsCorrect = false;
                        exViewModels.Test.SingleOrDefault(q => q.SpaceTest.Id == viewModel.SpaceTest.Id).SpaceTest.TriedAnswer = viewModel.SpaceTest.TriedAnswer;
                    }
                    if (exViewModels.TestNumber < AppSettings.TestCount)
                        exViewModels.TestNumber += 1;
                    newViewModel = exViewModels.Test.SingleOrDefault(q => q.QuizNumber == exViewModels.TestNumber);
                }
            }
            string newQuizJson;
            if (viewModel.Finish == true)
            {
                if (viewModel.QuestionNumber == 1)
                {
                    if (viewModel.UserWord.Answer == viewModel.UserWord.Mean)
                        exViewModels.Test.SingleOrDefault(q => q.UserWord.Id == viewModel.UserWord.Id).UserWord.IsCorrect = true;
                    else
                        exViewModels.Test.SingleOrDefault(q => q.UserWord.Id == viewModel.UserWord.Id).UserWord.IsCorrect = false;
                    exViewModels.Test.SingleOrDefault(q => q.UserWord.Id == viewModel.UserWord.Id).UserWord.Answer = viewModel.UserWord.Answer;
                }
                if (viewModel.QuestionNumber == 2)
                {
                    if (viewModel.QuestionTest.SelectedAnswer.Id == 3)
                        exViewModels.Test.SingleOrDefault(qt => qt.QuestionTest.Id == viewModel.QuestionTest.Id).QuestionTest.IsCorrect = true;
                    else
                        exViewModels.Test.SingleOrDefault(qt => qt.QuestionTest.Id == viewModel.QuestionTest.Id).QuestionTest.IsCorrect = false;
                    exViewModels.Test.SingleOrDefault(q => q.QuestionTest.Id == viewModel.QuestionTest.Id).QuestionTest.SelectedAnswer = viewModel.QuestionTest.SelectedAnswer;
                }
                if (viewModel.QuestionNumber == 3)
                {
                    if (viewModel.SpaceTest.TriedAnswer != null)
                    {
                        if (viewModel.SpaceTest.TriedAnswer.Trim().ToLower() == viewModel.SpaceTest.Answer.ToLower())
                            exViewModels.Test.SingleOrDefault(uw => uw.SpaceTest.Id == viewModel.SpaceTest.Id).SpaceTest.IsCorrect = true;
                        else
                            exViewModels.Test.SingleOrDefault(uw => uw.SpaceTest.Id == viewModel.SpaceTest.Id).SpaceTest.IsCorrect = false;
                        exViewModels.Test.SingleOrDefault(q => q.SpaceTest.Id == viewModel.SpaceTest.Id).SpaceTest.TriedAnswer = viewModel.SpaceTest.TriedAnswer;
                    }
                }
                newQuizJson = JsonConvert.SerializeObject(exViewModels);
                HttpContext.Session.SetString("mixedquiz", newQuizJson);
                //TempData["CorrectNumber"] = correctNumber + " / " + AppSettings.TestCount;
                return RedirectToAction(nameof(TestResult));
            }
            newQuizJson = JsonConvert.SerializeObject(exViewModels);
            HttpContext.Session.SetString("mixedquiz", newQuizJson);
            newViewModel.Finish = false;
            return View(newViewModel);
        }
        public IActionResult TestResult()
        {
            var quizJson = HttpContext.Session.GetString("mixedquiz");
            var viewModels = JsonConvert.DeserializeObject<TestViewModel>(quizJson);
            int correctNumber = 0;
            foreach (var item in viewModels.Test)
            {
                if (item.UserWord.IsCorrect == true)
                    correctNumber += 1;
                if (item.QuestionTest.IsCorrect == true)
                    correctNumber += 1;
                if (item.SpaceTest.IsCorrect == true)
                    correctNumber += 1;
            }
            ViewBag.CorrectNumber = correctNumber + " / " + AppSettings.TestCount;
            return View(viewModels);
        }
    }
}
