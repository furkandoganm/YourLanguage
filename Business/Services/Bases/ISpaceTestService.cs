using AppCore.Business.Models.Results;
using AppCore.Business.Services.Bases;
using Business.Models.Studies;
using Business.Models.Tests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Services.Bases
{
    public interface ISpaceTestService: IService<SpaceTestModel>
    {
        Result<List<SpaceModel>> GetSpaceTestsGroupByDomain();
        Result<SpaceTestModel> GetEntity(int id);
    }
}
