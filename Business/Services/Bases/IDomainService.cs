using AppCore.Business.Models.Results;
using AppCore.Business.Services.Bases;
using Business.Models;

namespace Business.Services.Bases
{
    public interface IDomainService: IService<DomainModel>
    {
        Result<DomainModel> GetDomain(int id);
    }
}
