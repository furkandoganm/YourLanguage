using AppCore.Business.Models.Results;
using AppCore.Business.Services.Bases;
using Business.Models.Tests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Services.Bases
{
    public interface ITopicService: IService<TopicModel>
    {
        Result<TopicModel> GetTopic(int id);
    }
}
