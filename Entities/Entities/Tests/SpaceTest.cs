using AppCore.Records.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.Entities.Tests
{
    public class SpaceTest: Record
    {

        public int CreatorId { get; set; }
        [Required]
        [StringLength(100)]
        public string QuestionPart1 { get; set; }
        [Required]
        [StringLength(100)]
        public string AnswerPart1 { get; set; }

        public int TopicId { get; set; }
        public Topic Topic { get; set; }

        public string QuestionPart2 { get; set; }
        public string AnswerWord { get; set; }
    }
}
