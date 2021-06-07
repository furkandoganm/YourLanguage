using AppCore.Business.Models.Results;
using Business.Models;
using Business.Services.Bases;
using DataAccess.Repositories;
using Entities.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Business.Services
{
    public class WordService: IWordService
    {
        private readonly WordRepositoryBase _wordRepository;
        public WordService(WordRepositoryBase wordRepository)
        {
            _wordRepository = wordRepository;
        }

        public IQueryable<WordModel> Query(Expression<Func<WordModel, bool>> predicate = null)
        {
            var query = _wordRepository.Query("Domain").Select(w => new WordModel()
            {
                Id = w.Id,
                GuId = w.GuId,
                Vocable = w.Vocable,
                Mean = w.Mean,
                DomainId = w.DomainId,
                Domain = new DomainModel()
                {
                    Id = w.Domain.Id,
                    GuId = w.Domain.GuId,
                    Name = w.Domain.Name
                },
                ImagePath = w.ImagePath
            });
            if (predicate != null)
                query = query.Where(predicate);
            return query;
        }

        public Result Add(WordModel model)
        {
            try
            {
                if (_wordRepository.Query().Any(w => w.Vocable.ToUpper() == model.Vocable.Trim().ToUpper() && w.Mean.ToUpper() == model.Mean.Trim().ToUpper()))
                    return new ErrorResult("Same word is already exist.");
                var entity = new Word()
                {
                    Vocable = model.Vocable.Trim(),
                    Mean = model.Mean.Trim(),
                    DomainId = model.DomainId,
                    ImagePath = model.ImagePath
                };
                _wordRepository.Add(entity);
                model.Id = entity.Id;//lazım olabilir
                return new SuccessResult();
            }
            catch (Exception exc)
            {
                return new ExceptionResult(exc);
            }
        }

        public Result Update(WordModel model)
        {
            try
            {
                if (_wordRepository.Query().Any(w => w.Vocable.ToUpper() == model.Vocable.Trim().ToUpper() && w.Mean.ToUpper() == model.Mean.Trim().ToUpper() && w.Id != model.Id))
                    return new ErrorResult("Same word is already exist.");
                var entity = new Word()
                {
                    Id = model.Id,
                    GuId = model.GuId,
                    Vocable = model.Vocable.Trim(),
                    Mean = model.Mean.Trim(),
                    DomainId = model.DomainId,
                    ImagePath = model.ImagePath
                };
                _wordRepository.Update(entity);
                return new SuccessResult();
            }
            catch (Exception exc)
            {
                return new ExceptionResult(exc);
            }
        }

        public Result Delete(int id)
        {
            var query = _wordRepository.Query(w => w.Id == id).SingleOrDefault();
            if (query.UserWords != null && query.UserWords.Count > 0)
                return new ErrorResult("This word could not be deleted because of has userwords.");
            _wordRepository.Delete(id);
            return new SuccessResult();
        }

        public void Dispose()
        {
            _wordRepository?.Dispose();
        }
        
        public Result<WordModel> GetWord(int id)
        {
            try
            {
                var word = Query().SingleOrDefault(w => w.Id == id);
                if (word == null)
                    return new ErrorResult<WordModel>("Word is not exist!");
                return new SuccessResult<WordModel>(word);
            }
            catch (Exception exc)
            {
                return new ExceptionResult<WordModel>(exc);
            }
        }
    }
}
