using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YourLanguage.Models
{
    public class QuestionsViewModel
    {
        public List<QuestionTestViewModel> Questions { get; set; }
        public int TestCount { get; set; }
        public string Topic { get; set; }
        public int TopicId { get; set; }
        public bool IsCorrectAnswer { get; set; }
        public bool IsCheck { get; set; }
        public int QuestionPageCount { get; set; }
        public int QuestionPage { get; set; }
    }
    public class QuestionTestViewModel
    {
        public int Id { get; set; }
        //public int QuestionNumber { get; set; }
        public string Question { get; set; }
        public List<AnswerViewModel> Answers { get; set; }
        public AnswerViewModel SelectedAnswer { get; set; }
        public bool? IsCorrect { get; set; }
    }
    public class AnswerViewModel
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Name { get; set; }
    }
}
