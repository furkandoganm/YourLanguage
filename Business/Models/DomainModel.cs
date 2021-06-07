using AppCore.Records.Base;
using Business.Models.Tests;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class DomainModel: Record
    {
        [Required(ErrorMessage = "{0} is required!")]
        [MaxLength(100, ErrorMessage = "{0} must be maximum {1} character!")]
        [MinLength(2, ErrorMessage = "{0} must be minimum {1} character!")]
        [DisplayName("Domain")]
        public string Name { get; set; }
        public List<WordModel> Words { get; set; }
    }
}
