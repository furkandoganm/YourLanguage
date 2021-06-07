using AppCore.Records.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class RoleModel: Record
    {
        [Required(ErrorMessage = "{0} is required!")]
        [MaxLength(100, ErrorMessage = "{0} must be maximum {1} character!")]
        [MinLength(2, ErrorMessage = "{0} must be minimum {1} character!")]
        public string Name { get; set; }
        public List<UserModel> Users { get; set; }
    }
}
