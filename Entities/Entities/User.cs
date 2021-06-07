using AppCore.Records.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities.Entities
{
    public class User: Record
    {
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }
        [Required]
        [StringLength(100)]
        public string EMail { get; set; }
        [Required]
        [StringLength(20)]
        public string Password { get; set; }
        public bool Active { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public int NumbersofWordLearned { get; set; }
        public List<UserWord> UserWords { get; set; }
    }
}
