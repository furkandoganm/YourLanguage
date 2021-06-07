namespace Business.Models.Reports
{
    public class UserWordReportModel
    {
        public string Vocable { get; set; }
        public string Mean { get; set; }
        public string Domain { get; set; }
        public int DomainId { get; set; }
        public bool Active { get; set; }
        public int Count { get; set; }
        public int WordId { get; set; }
    }
    public class UserWordReportExcelModel
    {
        public string Vocable { get; set; }
        public string Mean { get; set; }
        public string Domain { get; set; }
        public string Active { get; set; }
        public int Count { get; set; }
    }
}
