using AppCore.Records.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Models.Tests
{
    public class TopicModel: Record
    {
        [Required(ErrorMessage = "{0} is required!")]
        [MaxLength(45, ErrorMessage = "{0} must be maximum {1} character!")]
        [MinLength(2, ErrorMessage = "{0} must be minimum {1} character!")]
        public string Name { get; set; }
        public List<QuestionTestModel> QuestionTests { get; set; }
        public List<SpaceTestModel> SpaceTests { get; set; }
    }
}
