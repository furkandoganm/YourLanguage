using Business.Models.Filters;
using Business.Models.Reports;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace YourLanguage.Models
{
    public class WordReportViewModel
    {
        public WordReportViewModel()
        {
            PageNumber = 1;
            OrderByDirectionAscending = true;
        }
        public IEnumerable<UserWordReportModel> Words { get; set; }
        public WordFilterModel Filter { get; set; }
        //public string ValidationMessage { get; set; }
        public int PageNumber { get; set; } = 1;
        public SelectList PageNumbers { get; set; }
        public string OrderByExpression { get; set; }
        public bool OrderByDirectionAscending { get; set; }
    }
}
