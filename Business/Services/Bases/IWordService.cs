using AppCore.Business.Models.Results;
using AppCore.Business.Services.Bases;
using Business.Models;

namespace Business.Services.Bases
{
    public interface IWordService: IService<WordModel>
    {
        Result<WordModel> GetWord(int id);
    }
}
