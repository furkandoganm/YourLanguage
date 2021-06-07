using AppCore.Business.Models.Results;
using Business.Models;
using Business.Models.Tests;
using Business.Services.Bases;
using DataAccess.Repositories;
using Entities.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Business.Services
{
    public class DomainService: IDomainService
    {
        private readonly DomainRepositoryBase _domainRepository;
        public DomainService(DomainRepositoryBase domainRepository)
        {
            _domainRepository = domainRepository;
        }
        public IQueryable<DomainModel> Query(Expression<Func<DomainModel, bool>> predicate = null)
        {
            var query = _domainRepository.Query("Words").Select(d => new DomainModel()
            {
                Id = d.Id,
                GuId = d.GuId,
                Name = d.Name,
                Words = d.Words.Select(w => new WordModel()
                {
                    Id = w.Id,
                    GuId = w.GuId,
                    Vocable = w.Vocable,
                    Mean = w.Mean,
                    DomainId = w.DomainId
                }).ToList()
            });
            if (predicate != null)
                query = query.Where(predicate);
            return query;
        }

        public Result Add(DomainModel model)
        {
            try
            {
                if (_domainRepository.Query().Any(d => d.Name == model.Name))
                    return new ErrorResult("Same domain is exist.");
                var entity = new Domain()
                {
                    Name = model.Name.Trim()
                };
                _domainRepository.Add(entity);
                return new SuccessResult();
            }
            catch (Exception exc)
            {
                return new ExceptionResult(exc);
            }
        }

        public Result Update(DomainModel model)
        {
            try
            {
                if (_domainRepository.Query().Any(d => d.Name == model.Name && d.Id != model.Id))
                    return new ErrorResult("Same domain is exist.");
                var entity = new Domain()
                {
                    Id = model.Id,
                    Name = model.Name.Trim()
                };
                _domainRepository.Update(entity);
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
            if (query.Words != null && query.Words.Count > 0)
                return new ErrorResult("This domain could not be deleted because of has words.");
            _domainRepository.Delete(id);
            return new SuccessResult();
        }

        public void Dispose()
        {
            _domainRepository?.Dispose();
        }

        public Result<DomainModel> GetDomain(int id)
        {
            try
            {
                var domain = Query().SingleOrDefault(w => w.Id == id);
                if (domain == null)
                    return new ErrorResult<DomainModel>("Domain is not exist!");
                return new SuccessResult<DomainModel>(domain);
            }
            catch (Exception exc)
            {
                return new ExceptionResult<DomainModel>(exc);
            }
        }
    }
}
