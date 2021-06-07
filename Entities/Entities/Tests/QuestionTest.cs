using AppCore.Records.Base;
using System.ComponentModel.DataAnnotations;

namespace Entities.Entities.Tests
{
    public class QuestionTest: Record
    {
        public int CreatorId { get; set; }

        public int TopicId { get; set; }
        public Topic Topic { get; set; }

        [Required]
        [StringLength(250)]
        public string Question { get; set; }

        [Required]
        [StringLength(50)]
        public string CorrectAnswer { get; set; }
        [Required]
        [StringLength(50)]
        public string WrongAnswer1 { get; set; }
        [Required]
        [StringLength(50)]
        public string WrongAnswer2 { get; set; }
        [Required]
        [StringLength(50)]
        public string WrongAnswer3 { get; set; }
        public string WrongAnswer4 { get; set; }
    }
}
