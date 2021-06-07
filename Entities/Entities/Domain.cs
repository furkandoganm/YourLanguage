using AppCore.Records.Base;
using Entities.Entities.Tests;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities.Entities
{
    public class Domain: Record
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public List<Word> Words { get; set; }
    }
}
