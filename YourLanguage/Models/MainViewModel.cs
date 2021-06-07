using Business.Models.Studies;
using Business.Models.Tests;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace YourLanguage.Models
{
    public class MainViewModel
    {
        [DisplayName("Word Quiz")]
        public IEnumerable<StudyModel> StudyModels { get; set; }
        [DisplayName("Question Tests")]
        public List<QuestionModel> QuestionTest { get; set; }
        [DisplayName("Space Tests")]
        public List<SpaceModel> SpaceTests { get; set; }
        public int NumbersofWordLearned { get; set; }
    }
}
