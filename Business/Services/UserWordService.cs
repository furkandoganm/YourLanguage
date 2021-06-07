using AppCore.Business.Models.Ordering;
using AppCore.Business.Models.Paging;
using AppCore.Business.Models.Results;
using Business.Models;
using Business.Models.Filters;
using Business.Models.Joins;
using Business.Models.Reports;
using Business.Models.Studies;
using Business.Services.Bases;
using DataAccess.Repositories;
using Entities.Entities;
using Entities.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Business.Services
{
    public class UserWordService : IUserWordService
    {
        private readonly UserWordRepositoryBase _userWordRepository;
        private readonly UserRepositoryBase _userRepository;
        private readonly WordRepositoryBase _wordRepository;
        public UserWordService(UserWordRepositoryBase userWordRepository, UserRepositoryBase userRepository, WordRepositoryBase wordRepository)
        {
            _userWordRepository = userWordRepository;
            _userRepository = userRepository;
            _wordRepository = wordRepository;
        }
        public IQueryable<UserWordModel> Query(Expression<Func<UserWordModel, bool>> predicate = null)
        {
            var query = _userWordRepository.Query(q => q.Active == true).Select(uw => new UserWordModel()
            {
                Id = uw.Id,
                GuId = uw.GuId,
                Active = uw.Active,
                LearningDegree = uw.LearningDegree,
                WordListName = uw.WordListName,
                UserId = uw.UserId,
                WordId = uw.WordId,
                User = new UserModel()
                {
                    Id = uw.User.Id,
                    GuId = uw.User.GuId,
                    UserName = uw.User.UserName
                },
                Word = new WordModel()
                {
                    Id = uw.Word.Id,
                    GuId = uw.Word.GuId,
                    Vocable = uw.Word.Vocable,
                    Mean = uw.Word.Mean
                }
            });
            if (predicate != null)
                query = query.Where(predicate);
            return query;
        }

        public Result Add(UserWordModel model)
        {
            try
            {
                if (_userWordRepository.Query().Any(uw => uw.WordId == model.WordId && uw.WordListName == model.WordListName && uw.UserId == model.UserId && uw.Active == true))
                    return new ErrorResult("The same word in this word list is exist.");
                var entity = new UserWord()
                {
                    UserId = model.UserId,
                    WordId = model.WordId,
                    LearningDegree = LearningDegree.Beginner,
                    WordListName = model.WordListName,
                    Active = true
                };
                _userWordRepository.Add(entity);
                return new SuccessResult();
            }
            catch (Exception exc)
            {
                return new ExceptionResult(exc);
            }
        }

        public Result Update(UserWordModel model)
        {
            try
            {
                if (_userWordRepository.Query().Any(uw => uw.WordId == model.WordId && uw.WordListName == model.WordListName && uw.UserId == model.UserId && uw.Id != model.Id && uw.Active == true))
                    return new ErrorResult("The same word in this word list is exist.");
                var entity = _userWordRepository.Query().SingleOrDefault(q => q.Id == model.Id);
                entity.LearningDegree = model.LearningDegree;
                entity.WordId = model.WordId;
                entity.WordListName = model.WordListName;
                entity.UserId = model.UserId;
                entity.Active = model.Active;
                if (model.LearningDegree == LearningDegree.Intermediate)
                {
                    User user = _userRepository.Query().SingleOrDefault(u => u.Id == entity.UserId);
                    user.NumbersofWordLearned += 1;
                    _userRepository.Update(user);
                }
                _userWordRepository.Update(entity);
                //_userWordRepository.Save();
                return new SuccessResult();
            }
            catch (Exception exc)
            {
                return new ExceptionResult(exc);
            }
        }

        public Result Delete(int id)
        {
            _userWordRepository.Delete(id);
            return new SuccessResult();
        }

        public void Dispose()
        {
            _userWordRepository?.Dispose();
        }

        public Result<List<UserWordReportModel>> GetReport(WordFilterModel filter, PageModel page = null, OrderModel order = null)
        {
            try
            {
                //var userQuery = _userRepository.Query();
                var wordQuery = _wordRepository.Query();
                var userWordQuery = Query();

                var query = from w in wordQuery
                            join uw in userWordQuery
                            on w.Id equals uw.WordId
                            //join u in userQuery
                            //on uw.UserId equals u.Id
                            orderby w.Vocable
                            select new UserWordReportModel()
                            {
                                Vocable = w.Vocable,
                                Mean = w.Mean,
                                WordId = w.Id,
                                Domain = w.Domain.Name,
                                DomainId = w.DomainId,
                                Active = uw.Active
                            };
                var queryGroupBy = query.OrderBy(w => w.Vocable).
                    GroupBy(w => new { w.Vocable, w.WordId, w.Mean, w.Active, w.DomainId, w.Domain })
                    .Select(wg => new UserWordReportModel()
                    {
                        Vocable = wg.Key.Vocable,
                        Mean = wg.Key.Mean,
                        Count = wg.Count(),
                        Active = wg.Key.Active,
                        DomainId = wg.Key.DomainId,
                        Domain = wg.Key.Domain
                    });
                queryGroupBy = queryGroupBy.OrderBy(q => q.Vocable);
                if (order != null)
                {
                    if (!string.IsNullOrWhiteSpace(order.Expression))
                    {
                        switch (order.Expression.Trim())
                        {
                            case "Vocable":
                                queryGroupBy = (order.DirectionAscending ? queryGroupBy.OrderBy(q => q.Vocable) : queryGroupBy.OrderByDescending(q => q.Vocable));
                                break;
                            case "Mean":
                                queryGroupBy = (order.DirectionAscending ? queryGroupBy.OrderBy(q => q.Mean) : queryGroupBy.OrderByDescending(q => q.Mean));
                                break;
                            case "Count":
                                queryGroupBy = (order.DirectionAscending ? queryGroupBy.OrderBy(q => q.Count) : queryGroupBy.OrderByDescending(q => q.Count));
                                break;
                            case "Domain":
                                queryGroupBy = (order.DirectionAscending ? queryGroupBy.OrderBy(q => q.Domain) : queryGroupBy.OrderByDescending(q => q.Domain));
                                break;
                        }
                    }
                }
                if (filter.IsActive != null)
                {
                    queryGroupBy = queryGroupBy.Where(q => q.Active == filter.IsActive); page.PageNumber = 1;
                }
                if (filter.Vocable != null)
                {
                    queryGroupBy = queryGroupBy.Where(q => q.Vocable.ToUpper().Contains(filter.Vocable.Trim().ToUpper())); page.PageNumber = 1;
                }
                if (filter.Mean != null)
                {
                    queryGroupBy = queryGroupBy.Where(q => q.Mean.ToUpper().Contains(filter.Mean.Trim().ToUpper())); page.PageNumber = 1;
                }
                if (filter.DomainId != null)
                {
                    queryGroupBy = queryGroupBy.Where(q => q.DomainId == filter.DomainId); page.PageNumber = 1;
                }
                if (page == null)
                    page = new PageModel() { PageNumber = -1 };
                if (page.PageNumber != -1)
                {
                    page.RecordsCount = queryGroupBy.Count();
                    int skip = (page.PageNumber - 1) * page.RecordsCountPerPage;
                    int take = page.RecordsCountPerPage;
                    queryGroupBy = queryGroupBy.Skip(skip).Take(take);
                }
                return new SuccessResult<List<UserWordReportModel>>(queryGroupBy.ToList());
            }
            catch (Exception exc)
            {
                return new ExceptionResult<List<UserWordReportModel>>(exc);
            }
        }
        public Result<List<StudyModel>> GetQuiz(int userId, string listName)
        {
            try
            {
                if (listName == "quiz")
                    listName = null;
                var query = Query(uw => (listName != null) ? uw.WordListName == listName && uw.UserId == userId : uw.UserId == userId && uw.LearningDegree != LearningDegree.Intermediate).Select(uw => new StudyModel()
                {
                    Id = uw.Id,
                    Vocable = uw.Word.Vocable.Trim(),
                    Mean = uw.Word.Mean.Trim(),
                    LearningDegree = (int)uw.LearningDegree,
                    ListName = uw.WordListName
                }).ToList();
                if (query == null && query.Count == 0)
                    return new ErrorResult<List<StudyModel>>("Record not found at this list!");
                //for (int sessionId = 1; sessionId <= query.Count; sessionId++)
                //{
                //    query[sessionId].SessionId = sessionId;
                //}
                return new SuccessResult<List<StudyModel>>(query);
            }
            catch (Exception exc)
            {
                return new ExceptionResult<List<StudyModel>>(exc);
            }
        }
        public Result<IEnumerable<StudyModel>> GetQuizGroupByListName(int? id)
        {
            try
            {
                var tempQuery = Query();
                if (id.HasValue)
                    tempQuery = tempQuery.Where(uw => uw.UserId == id.Value);
                var query = tempQuery.OrderBy(w => w.Word.Vocable)
                    .GroupBy(w => new { w.UserId, w.WordListName })
                    .Select(wg => new StudyModel()
                    {
                        UserId = wg.Key.UserId,
                        ListName = wg.Key.WordListName
                    });
                if (query == null && query.Count() == 0)
                    return new ErrorResult<IEnumerable<StudyModel>>("Record not found!");
                return new SuccessResult<IEnumerable<StudyModel>>(query);
            }
            catch (Exception exc)
            {
                return new ExceptionResult<IEnumerable<StudyModel>>(exc);
            }
        }
        public void Update(int id)
        {
            var entity = _userWordRepository.Query().SingleOrDefault(uw => uw.Id == id);
            entity.LearningDegree += 1;
            _userWordRepository.Update(entity);
        }
        public Result UpdateList(string listName, int id = 0)
        {
            try
            {
                if (id != 0)
                    listName = Query().SingleOrDefault(uw => uw.Id == id).WordListName;
                var query = Query(uw => uw.WordListName == listName.Trim());
                foreach (var item in query)
                {
                    var entity = new UserWord()
                    {
                        Id = item.Id,
                        UserId = item.UserId,
                        WordId = item.WordId,
                        Active = false
                    };
                    _userWordRepository.Update(entity, false);
                }
                _userWordRepository.Save();
                return new SuccessResult();
            }
            catch (Exception exc)
            {
                return new ExceptionResult(exc);
            }
        }
        public async Task<Result<IEnumerable<StudyModel>>> GetListsAsync(string name)
        {
            try
            {
                //var query = await _userWordRepository.Query().ToListAsync();
                var query = await _userWordRepository.Query(w => w.Active == true && w.User.UserName == name.Trim()).OrderBy(w => w.Word.Vocable)
                    .GroupBy(w => new { w.UserId, w.WordListName })
                    .Select(wg => new StudyModel()
                    {
                        UserId = wg.Key.UserId,
                        ListName = wg.Key.WordListName
                    }).ToListAsync();
                if (query == null && query.Count() == 0)
                    return new ErrorResult<IEnumerable<StudyModel>>("Record not found!");
                return new SuccessResult<IEnumerable<StudyModel>>(query);
            }
            catch (Exception exc)
            {
                return new ExceptionResult<IEnumerable<StudyModel>>(exc);
            }
        }
    }
}
