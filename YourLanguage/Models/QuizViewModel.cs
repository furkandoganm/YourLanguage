using Business.Models;
using Business.Models.Tests;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace YourLanguage.Models
{
    public class TestViewModel
    {
        public List<QuizViewModel> Test { get; set; }
        public int TestNumber { get; set; }
        public int WhichQuiz { get; set; }
    }
    public class QuizViewModel
    {
        public QuestionTestViewModel QuestionTest { get; set; }
        public SpaceTestViewModel SpaceTest { get; set; }
        public UserWordViewModel UserWord { get; set; }
        public int QuizNumber { get; set; }
        public bool IsCorrect { get; set; }
        public int QuestionNumber { get; set; }
        public bool? NextPrevious { get; set; }
        public bool Finish { get; set; }
    }
    public class UserWordViewModel
    {
        public int Id { get; set; }
        public string Vocable { get; set; }
        public string Mean { get; set; }
        public string Answer { get; set; }
        public bool IsCorrect { get; set; }
    }
    public class SpaceViewModel
    {
        public List<SpaceTestViewModel> SpaceTest { get; set; }
        public int PageNumber { get; set; }
        public int PageCount { get; set; }
        public bool IsCheck { get; set; }
        public string Topic { get; set; }
        public int TopicId { get; set; }
        public bool IsCorrectAnswer { get; set; }
    }
    public class SpaceTestViewModel
    {
        public int Id { get; set; }
        [DisplayName("Question part 1")]
        public string QuestionPart1 { get; set; }
        public string Answer { get; set; }
        [DisplayName("Question part")]
        public string QuestionPart2 { get; set; }
        [DisplayName("Your answer")]
        public string TriedAnswer { get; set; }
        [DisplayName("Word")]
        public string AnswerWord { get; set; }
        public TopicModel Topic { get; set; }
        public bool IsCorrect { get; set; }
    }
}
