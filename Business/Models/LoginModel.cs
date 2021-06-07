using AppCore.Records.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class LoginModel: Record
    {
        [Required(ErrorMessage = "{0} is required!")]
        [MaxLength(100, ErrorMessage = "{0} must be maximum {1} character!")]
        [MinLength(2, ErrorMessage = "{0} must be minimum {1} character!")]
        [DisplayName("User Name")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "{0} is required!")]
        [MaxLength(8, ErrorMessage = "{0} must be maximum {1} character!")]
        [MinLength(3, ErrorMessage = "{0} must be minimum {1} character!")]
        public string Password { get; set; }
        public string RoleName { get; set; }
    }
}
