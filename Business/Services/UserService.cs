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
    public class UserService : IUserService
    {
        private readonly UserRepositoryBase _userRepository;
        private readonly UserWordRepositoryBase _userWordRepository;
        public UserService(UserRepositoryBase userRepository, UserWordRepositoryBase userWordRepository)
        {
            _userRepository = userRepository;
            _userWordRepository = userWordRepository;
        }

        public IQueryable<UserModel> Query(Expression<Func<UserModel, bool>> predicate = null)
        {
            var query = _userRepository.Query().Select(u => new UserModel()
            {
                Id = u.Id,
                GuId = u.GuId,
                Active = u.Active,
                UserName = u.UserName,
                Password = u.Password,
                EMail = u.EMail,
                NumbersofWordLearned = u.NumbersofWordLearned,
                RoleId = u.RoleId,
                Role = new RoleModel()
                {
                    Id = u.Role.Id,
                    Name = u.Role.Name
                }
            });
            if (predicate != null)
                query = query.Where(predicate);
            return query;
        }

        public Result Add(UserModel model)
        {
            try
            {
                if (_userRepository.Query().Any(u => u.UserName.ToUpper() == model.UserName.Trim().ToUpper()))
                    return new ErrorResult("User with the same name is exist.");
                if (_userRepository.Query().Any(u => u.EMail == model.EMail))
                    return new ErrorResult("User with the same E-Mail is exist.");
                var entity = new User()
                {
                    Active = model.Active,
                    UserName = model.UserName.Trim(),
                    Password = model.Password.Trim(),
                    RoleId = model.RoleId,
                    EMail = model.EMail.Trim(),
                    NumbersofWordLearned = 0
                };
                _userRepository.Add(entity);
                return new SuccessResult();
            }
            catch (Exception exc)
            {
                return new ExceptionResult(exc);
            }
        }

        public Result Update(UserModel model)
        {
            try
            {
                if (_userRepository.Query().Any(u => u.UserName.ToUpper() == model.UserName.Trim().ToUpper() && u.Id != model.Id))
                    return new ErrorResult("User with the same name is exist.");
                if (_userRepository.Query().Any(u => u.EMail == model.EMail && u.Id != model.Id))
                    return new ErrorResult("User with the same E-Mail is exist.");
                var entity = new User()
                {
                    Id = model.Id,
                    GuId = model.GuId,
                    Active = model.Active,
                    UserName = model.UserName.Trim(),
                    Password = model.Password.Trim(),
                    RoleId = model.RoleId,
                    EMail = model.EMail.Trim()
                };
                _userRepository.Update(entity);
                return new SuccessResult();
            }
            catch (Exception exc)
            {
                return new ExceptionResult(exc);
            }
        }

        public Result Delete(int id)
        {
            try
            {
                var query = _userWordRepository.Query(uw => uw.UserId == id);
                if (query != null && query.Count() > 0)
                {
                    foreach (var item in query)
                    {
                        _userWordRepository.Delete(item);
                    }
                }
                _userRepository.Delete(id);
                return new SuccessResult();
            }
            catch (Exception exc)
            {
                return new ExceptionResult(exc);
            }
        }

        public void Dispose()
        {
            _userRepository?.Dispose();
        }
    }
}
