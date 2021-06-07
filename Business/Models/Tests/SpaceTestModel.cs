using AppCore.Records.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Models.Tests
{
    public class SpaceTestModel: Record
    {
        public int CreatorId { get; set; }
        [Required(ErrorMessage = "{0} is required!")]
        [MaxLength(60, ErrorMessage = "{0} must be maximum {1} character!")]
        [MinLength(2, ErrorMessage = "{0} must be minimum {1} character!")]
        [DisplayName("First Part")]
        public string QuestionPart1 { get; set; }
        [Required(ErrorMessage = "{0} is required!")]
        [MaxLength(60, ErrorMessage = "{0} must be maximum {1} character!")]
        [MinLength(2, ErrorMessage = "{0} must be minimum {1} character!")]
        [DisplayName("Answer")]
        public string AnswerPart1 { get; set; }

        public int TopicId { get; set; }
        public TopicModel Topic { get; set; }

        [MaxLength(60, ErrorMessage = "{0} must be maximum {1} character!")]
        [MinLength(1, ErrorMessage = "{0} must be minimum {1} character!")]
        [DisplayName("Second Part")]
        public string QuestionPart2 { get; set; }
        [MaxLength(60, ErrorMessage = "{0} must be maximum {1} character!")]
        [MinLength(2, ErrorMessage = "{0} must be minimum {1} character!")]
        [DisplayName("Word")]
        public string AnswerWord { get; set; }
    }
}
