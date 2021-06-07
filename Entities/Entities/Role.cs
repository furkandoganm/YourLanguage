using AppCore.Records.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities.Entities
{
    public class Role: Record
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public List<User> Users { get; set; }
    }
}
