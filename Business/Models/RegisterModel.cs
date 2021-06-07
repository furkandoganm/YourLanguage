using AppCore.Records.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Models
{
    public class RegisterModel: Record
    {
        [Required(ErrorMessage = "{0} is required!")]
        [MaxLength(100, ErrorMessage = "{0} must be maximum {1} character!")]
        [MinLength(2, ErrorMessage = "{0} must be minimum {1} character!")]
        [DisplayName("User Name")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "{0} is required!")]
        [MaxLength(100, ErrorMessage = "{0} must be maximum {1} character!")]
        [MinLength(2, ErrorMessage = "{0} must be minimum {1} character!")]
        [DisplayName("E-Mail")]
        public string EMail { get; set; }
        [Required(ErrorMessage = "{0} is required!")]
        [MaxLength(8, ErrorMessage = "{0} must be maximum {1} character!")]
        [MinLength(3, ErrorMessage = "{0} must be minimum {1} character!")]
        public string Password { get; set; }
        [Compare("Password")]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
        public bool Active { get; set; }
        public int RoleId { get; set; }
        public RoleModel Role { get; set; }
        public int NumbersofWordLearned { get; set; }
    }
}
