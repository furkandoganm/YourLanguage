using Business.Models;
using Business.Models.Studies;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace YourLanguage.Models
{
    public class StudyViewModel
    {
        public List<StudyModel> StudyModel { get; set; }
        public int QuizCount { get; set; }
        public int QuizNumber { get; set; }
        public bool? IsQuizCorrect { get; set; }
        public string ListName { get; set; }
        public bool WasShown { get; set; }

        public SelectList SelectList { get; set; }

        public WordModel Word { get; set; }
    }
}
