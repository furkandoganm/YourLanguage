using System.ComponentModel;

namespace Business.Models.Filters
{
    public class WordFilterModel
    {
        [DisplayName("Domain")]
        public int? DomainId { get; set; }
        public string Vocable { get; set; }
        public string Mean { get; set; }
        //[DisplayName("Learning Degree")]
        //public double? LearningDegree { get; set; }
        public bool? IsActive { get; set; }
    }
}
