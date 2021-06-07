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

namespace Business.Services
{
    public class QuestionTestService : IQuestionTestService
    {
        private readonly QuestionTestRepositoryBase _questionTestRepository;
        public QuestionTestService(QuestionTestRepositoryBase questionTestRepository)
        {
            _questionTestRepository = questionTestRepository;
        }
        public IQueryable<QuestionTestModel> Query(Expression<Func<QuestionTestModel, bool>> predicate = null)
        {
            var query = _questionTestRepository.Query("Topic").Select(q => new QuestionTestModel()
            {
                Id = q.Id,
                GuId = q.GuId,
                CreatorId = q.CreatorId,
                Question = q.Question,
                TopicId = q.TopicId,
                Topic = new TopicModel()
                {
                    Id = q.Topic.Id,
                    GuId = q.Topic.GuId,
                    Name = q.Topic.Name
                },
                CorrectAnswer = q.CorrectAnswer,
                WrongAnswer1 = q.WrongAnswer1,
                WrongAnswer2 = q.WrongAnswer2,
                WrongAnswer3 = q.WrongAnswer3,
                WrongAnswer4 = q.WrongAnswer4 ?? q.WrongAnswer4
            });
            if (predicate != null)
                query = query.Where(predicate);
            return query;
        }

        public Result Add(QuestionTestModel model)
        {
            try
            {
                if (_questionTestRepository.Query().Any(q => q.Question == model.Question))
                    return new ErrorResult("Same question is already exist.");
                var entity = new QuestionTest()
                {
                    CreatorId = model.CreatorId,
                    Question = model.Question,
                    TopicId = model.TopicId,
                    //Topic = new TopicModel()
                    //{
                    //    Id = q.Topic.Id,
                    //    GuId = q.Topic.GuId,
                    //    Name = q.Topic.Name
                    //},
                    CorrectAnswer = model.CorrectAnswer,
                    WrongAnswer1 = model.WrongAnswer1,
                    WrongAnswer2 = model.WrongAnswer2,
                    WrongAnswer3 = model.WrongAnswer3,
                    WrongAnswer4 = model.WrongAnswer4 ?? model.WrongAnswer4
                };
                _questionTestRepository.Add(entity);
                return new SuccessResult();
            }
            catch (Exception exc)
            {
                return new ExceptionResult(exc);
            }
        }

        public Result Update(QuestionTestModel model)
        {
            try
            {
                if (_questionTestRepository.Query().Any(q => q.Question == model.Question && q.Id != model.Id))
                    return new ErrorResult("Same question is already exist.");
                var entity = new QuestionTest()
                {
                    Id = model.Id,
                    GuId = model.GuId,
                    CreatorId = model.CreatorId,
                    Question = model.Question,
                    TopicId = model.TopicId,
                    //Topic = new TopicModel()
                    //{
                    //    Id = q.Topic.Id,
                    //    GuId = q.Topic.GuId,
                    //    Name = q.Topic.Name
                    //},
                    CorrectAnswer = model.CorrectAnswer,
                    WrongAnswer1 = model.WrongAnswer1,
                    WrongAnswer2 = model.WrongAnswer2,
                    WrongAnswer3 = model.WrongAnswer3,
                    WrongAnswer4 = model.WrongAnswer4 ?? model.WrongAnswer4
                };
                _questionTestRepository.Update(entity);
                return new SuccessResult();
            }
            catch (Exception exc)
            {
                return new ExceptionResult(exc);
            }
        }

        public Result Delete(int id)
        {
            _questionTestRepository.Delete(id);
            return new SuccessResult();
        }

        public void Dispose()
        {
            _questionTestRepository?.Dispose();
        }
        public Result<List<QuestionModel>> GetQuestionTestsGroupByDomain()
        {
            try
            {
                var query = Query().OrderBy(q => q.Topic.Name)
                .GroupBy(q => new { q.Topic.Name, q.TopicId})
                .Select(qg => new QuestionModel()
                {
                    TopicId = qg.Key.TopicId,
                    Topic = qg.Key.Name
                }).ToList();
                if (query == null)
                    return new ErrorResult<List<QuestionModel>>("Record not found.");
                return new SuccessResult<List<QuestionModel>>(query);
            }
            catch (Exception exc)
            {
                return new ExceptionResult<List<QuestionModel>>(exc);
            }
        }
    }
}
