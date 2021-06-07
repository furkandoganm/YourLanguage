using AppCore.Records.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities.Entities
{
    public class Word: Record
    {
        [Required]
        [StringLength(100)]
        public string Vocable { get; set; }
        [Required]
        [StringLength(100)]
        public string Mean { get; set; }
        public int DomainId { get; set; }
        public Domain Domain { get; set; }
        public List<UserWord> UserWords { get; set; }
        public string ImagePath { get; set; }
    }
}
