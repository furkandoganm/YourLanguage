using AppCore.Records.Base;
using Entities.Entities.Enums;

namespace Entities.Entities
{
    public class UserWord: Record
    {
        public string WordListName { get; set; }
        public LearningDegree LearningDegree { get; set; }
        public int WordId { get; set; }
        public Word Word { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public bool Active { get; set; }
    }
}
