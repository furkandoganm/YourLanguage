using AppCore.Records.Base;
using Entities.Entities.Enums;
using System.ComponentModel;

namespace Business.Models
{
    public class UserWordModel: Record
    {
        public string WordListName { get; set; }
        [DisplayName("Learning Degree")]
        public LearningDegree LearningDegree { get; set; }
        public int WordId { get; set; }
        public WordModel Word { get; set; }
        public int UserId { get; set; }
        public UserModel User { get; set; }
        public bool Active { get; set; }
    }
}
