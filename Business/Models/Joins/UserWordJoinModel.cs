using Entities.Entities.Enums;

namespace Business.Models.Joins
{
    public class UserWordJoinModel
    {
        public string Vocable { get; set; }
        public string Mean { get; set; }
        //public LearningDegree LearningDegree { get; set; }
        public string Domain { get; set; }
        public int DomainId { get; set; }
        public bool Active { get; set; }
        public int WordId { get; set; }
    }
}
