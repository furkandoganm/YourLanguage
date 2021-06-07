using System.ComponentModel;

namespace Business.Models.Studies
{
    public class StudyModel
    {
        public int Id { get; set; }
        public int? SessionId { get; set; }
        public string Vocable { get; set; }
        public string Mean { get; set; }
        public int LearningDegree { get; set; }
        [DisplayName("Word List")]
        public string ListName { get; set; }

        public int UserId { get; set; }
    }
}
