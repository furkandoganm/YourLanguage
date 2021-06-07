using AppCore.Business.Models.Results;
using Business.Models.Tests;
using Business.Services.Bases;
using DataAccess.Repositories;
using Entities.Entities.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Business.Services
{
    public class TopicService: ITopicService
    {
        private readonly TopicRepositoryBase _topicRepository;
        public TopicService(TopicRepositoryBase topicRepository)
        {
            _topicRepository = topicRepository;
        }

        public IQueryable<TopicModel> Query(Expression<Func<TopicModel, bool>> predicate = null)
        {
            var query = _topicRepository.Query().Select(t => new TopicModel()
            {
                Id = t.Id,
                GuId = t.GuId,
                Name = t.Name
            });
            if (predicate != null)
                query = query.Where(predicate);
            return query;
        }

        public Result Add(TopicModel model)
        {
            try
            {
                if (_topicRepository.Query().Any(t => t.Name == model.Name))
                    return new ErrorResult("Same topic is exist.");
                var entity = new Topic()
                {
                    Name = model.Name.Trim()
                };
                _topicRepository.Add(entity);
                return new SuccessResult();
            }
            catch (Exception exc)
            {
                return new ExceptionResult(exc);
            }
        }

        public Result Update(TopicModel model)
        {
            try
            {
                if (_topicRepository.Query().Any(t => t.Name == model.Name && t.Id != model.Id))
                    return new ErrorResult("Same topic is exist.");
                var entity = _topicRepository.Query().SingleOrDefault(t => t.Id == model.Id);
                entity.Name = model.Name;
                _topicRepository.Update(entity);
                return new SuccessResult();
            }
            catch (Exception exc)
            {
                return new ExceptionResult(exc);
            }
        }

        public Result Delete(int id)
        {
            var query = Query(d => d.Id == id).SingleOrDefault();
            if ((query.SpaceTests != null && query.SpaceTests.Count > 0) || (query.QuestionTests != null && query.QuestionTests.Count > 0))
                return new ErrorResult("This topic could not be deleted because of has words.");
            _topicRepository.Delete(id);
            return new SuccessResult();
        }

        public void Dispose()
        {
            _topicRepository?.Dispose();
        }
        public Result<TopicModel> GetTopic(int id)
        {
            try
            {
                var topic = Query().SingleOrDefault(w => w.Id == id);
                if (topic == null)
                    return new ErrorResult<TopicModel>("Topic is not exist!");
                return new SuccessResult<TopicModel>(topic);
            }
            catch (Exception exc)
            {
                return new ExceptionResult<TopicModel>(exc);
            }
        }
    }
}
