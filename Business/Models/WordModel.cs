using AppCore.Records.Base;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class WordModel: Record
    {
        [Required(ErrorMessage = "{0} is required!")]
        [MaxLength(100, ErrorMessage = "{0} must be maximum {1} character!")]
        [MinLength(2, ErrorMessage = "{0} must be minimum {1} character!")]
        public string Vocable { get; set; }
        [Required(ErrorMessage = "{0} is required!")]
        [MaxLength(100, ErrorMessage = "{0} must be maximum {1} character!")]
        [MinLength(2, ErrorMessage = "{0} must be minimum {1} character!")]
        public string Mean { get; set; }
        public int DomainId { get; set; }
        public DomainModel Domain { get; set; }
        public List<UserWordModel> UserWords { get; set; }
        [DisplayName("Image")]
        public string ImagePath { get; set; }
        //public string Description { get; set; }
    }
}
