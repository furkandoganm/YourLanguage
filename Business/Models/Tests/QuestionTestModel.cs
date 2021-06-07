using AppCore.Records.Base;
using System.ComponentModel.DataAnnotations;

namespace Business.Models.Tests
{
    public class QuestionTestModel: Record
    {
        public int CreatorId { get; set; }

        public int TopicId { get; set; }
        public TopicModel Topic { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        [MaxLength(240, ErrorMessage = "{0} must be maximum {1} character!")]
        [MinLength(10, ErrorMessage = "{0} must be minimum {1} character!")]
        public string Question { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        [MaxLength(45, ErrorMessage = "{0} must be maximum {1} character!")]
        [MinLength(2, ErrorMessage = "{0} must be minimum {1} character!")]
        public string CorrectAnswer { get; set; }
        [Required(ErrorMessage = "{0} is required!")]
        [MaxLength(45, ErrorMessage = "{0} must be maximum {1} character!")]
        [MinLength(2, ErrorMessage = "{0} must be minimum {1} character!")]
        public string WrongAnswer1 { get; set; }
        [Required(ErrorMessage = "{0} is required!")]
        [MaxLength(45, ErrorMessage = "{0} must be maximum {1} character!")]
        [MinLength(2, ErrorMessage = "{0} must be minimum {1} character!")]
        public string WrongAnswer2 { get; set; }
        [Required(ErrorMessage = "{0} is required!")]
        [MaxLength(45, ErrorMessage = "{0} must be maximum {1} character!")]
        [MinLength(2, ErrorMessage = "{0} must be minimum {1} character!")]
        public string WrongAnswer3 { get; set; }
        public string WrongAnswer4 { get; set; }
    }
}
