using AppCore.Business.Models.Results;
using Business.Models;
using Business.Models.Studies;
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
    public class SpaceTestService : ISpaceTestService
    {
        private readonly SpaceTestRepositoryBase _spaceTestRepository;
        public SpaceTestService(SpaceTestRepositoryBase spaceTestRepository)
        {
            _spaceTestRepository = spaceTestRepository;
        }

        public IQueryable<SpaceTestModel> Query(Expression<Func<SpaceTestModel, bool>> predicate = null)
        {
            var query = _spaceTestRepository.Query("Topic").Select(s => new SpaceTestModel()
            {
                Id = s.Id,
                GuId = s.GuId,
                CreatorId = s.CreatorId,
                TopicId = s.TopicId,
                Topic = new TopicModel()
                {
                    Id = s.Topic.Id,
                    GuId = s.Topic.GuId,
                    Name = s.Topic.Name
                },
                QuestionPart1 = s.QuestionPart1,
                QuestionPart2 = s.QuestionPart2,
                AnswerPart1 = s.AnswerPart1,
                AnswerWord = s.AnswerWord
            });
            if (predicate != null)
                query = query.Where(predicate);
            return query;
        }

        public Result Add(SpaceTestModel model)
        {
            try
            {
                if (_spaceTestRepository.Query().Any(s => s.QuestionPart1 == model.QuestionPart1 && s.QuestionPart2 == ((model.QuestionPart2 != null) ? model.QuestionPart2 : "")))
                    return new ErrorResult("Same question is exist.");
                var entity = new SpaceTest()
                {
                    CreatorId = model.CreatorId,
                    TopicId = model.TopicId,
                    QuestionPart1 = model.QuestionPart1,
                    QuestionPart2 = model.QuestionPart2,
                    AnswerPart1 = model.AnswerPart1,
                    AnswerWord = model.AnswerWord
                };
                _spaceTestRepository.Add(entity);
                return new SuccessResult();
            }
            catch (Exception exc)
            {
                return new ExceptionResult(exc);
            }
        }

        public Result Update(SpaceTestModel model)
        {
            if (_spaceTestRepository.Query().Any(s => s.QuestionPart1 == model.QuestionPart1 && s.Id != model.Id && s.QuestionPart2 == ((model.QuestionPart2 != null) ? model.QuestionPart2 : "")))
                return new ErrorResult("Same question is exist.");
            var entity = new SpaceTest()
            {
                Id = model.Id,
                GuId = model.GuId,
                CreatorId = model.CreatorId,
                TopicId = model.TopicId,
                QuestionPart1 = model.QuestionPart1,
                QuestionPart2 = model.QuestionPart2,
                AnswerPart1 = model.AnswerPart1,
                AnswerWord = model.AnswerWord
            };
            _spaceTestRepository.Update(entity);
            return new SuccessResult();
        }

        public Result Delete(int id)
        {
            _spaceTestRepository.Delete(id);
            return new SuccessResult();
        }

        public void Dispose()
        {
            _spaceTestRepository?.Dispose();
        }
        public Result<List<SpaceModel>> GetSpaceTestsGroupByDomain()
        {
            try
            {
                var query = Query().OrderBy(q => q.Topic.Name)
                .GroupBy(q => new { q.TopicId, q.Topic.Name })
                .Select(qg => new SpaceModel()
                {
                    TopicId = qg.Key.TopicId,
                    Topic = qg.Key.Name
                }).ToList();
                if (query == null)
                    return new ErrorResult<List<SpaceModel>>("Record not found.");
                return new SuccessResult<List<SpaceModel>>(query);
            }
            catch (Exception exc)
            {
                return new ExceptionResult<List<SpaceModel>>(exc);
            }
        }
        public Result<SpaceTestModel> GetEntity(int id)
        {
            try
            {
                var entity = Query().SingleOrDefault(st => st.Id == id);
                if (entity == null)
                    return new ErrorResult<SpaceTestModel>("Record not found.");
                return new SuccessResult<SpaceTestModel>(entity);
            }
            catch (Exception exc)
            {
                return new ExceptionResult<SpaceTestModel>(exc);
            }
        }
    }
}
