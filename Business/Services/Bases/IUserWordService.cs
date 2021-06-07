using AppCore.Business.Models.Ordering;
using AppCore.Business.Models.Paging;
using AppCore.Business.Models.Results;
using AppCore.Business.Services.Bases;
using Business.Models;
using Business.Models.Filters;
using Business.Models.Reports;
using Business.Models.Studies;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Services.Bases
{
    public interface IUserWordService: IService<UserWordModel>
    {
        Result<List<UserWordReportModel>> GetReport(WordFilterModel filter, PageModel page = null, OrderModel order = null);
        Result<List<StudyModel>> GetQuiz(int userId, string listName = null);
        public void Update(int id);
        Result<IEnumerable<StudyModel>> GetQuizGroupByListName(int? id);
        Result UpdateList(string listName, int id = 0);
        Task<Result<IEnumerable<StudyModel>>> GetListsAsync(string name);
    }
}
