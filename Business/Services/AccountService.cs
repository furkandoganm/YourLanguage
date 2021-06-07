using AppCore.Business.Models.Results;
using Business.Enums;
using Business.Models;
using DataAccess.Repositories;
using Entities.Entities;
using System;
using System.Linq;

namespace Business.Services
{
    public interface IAccountService
    {
        Result Register(RegisterModel model);
        Result<LoginModel> Login(LoginModel model);
        public Result Update(RegisterModel model);
    }
    public class AccountService : IAccountService
    {
        private readonly UserRepositoryBase _userRepository;
        public AccountService(UserRepositoryBase userRepository)
        {
            _userRepository = userRepository;
        }
        public Result<LoginModel> Login(LoginModel model)
        {
            try
            {
                var user = _userRepository.Query("Role").SingleOrDefault(u => u.UserName.ToUpper() == model.UserName.Trim().ToUpper() && u.Password == model.Password.Trim());
                if (user == null)
                    return new ErrorResult<LoginModel>("No user found!");
                var userModel = new LoginModel()
                {
                    Id = user.Id,
                    GuId = user.GuId,
                    UserName = user.UserName,
                    RoleName = user.Role.Name
                };
                return new SuccessResult<LoginModel>(userModel);
            }
            catch (Exception exc)
            {
                return new ExceptionResult<LoginModel>(exc);
            }
        }

        public Result Register(RegisterModel model)
        {
            try
            {
                if (_userRepository.Query().Any(u => u.UserName.ToUpper() == model.UserName.Trim().ToUpper()))
                    return new ErrorResult("User with the same name is exist.");
                if (_userRepository.Query().Any(u => u.EMail == model.EMail))
                    return new ErrorResult("User with the same E-Mail is exist.");
                var entity = new User()
                {
                    Active = true,
                    UserName = model.UserName.Trim(),
                    Password = model.Password.Trim(),
                    RoleId = (int)Roles.User,
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
        public Result Update(RegisterModel model)
        {
            try
            {
                if (_userRepository.Query().Any(u => u.UserName.ToUpper() == model.UserName.Trim().ToUpper() && u.Id != model.Id))
                    return new ErrorResult("User with the same name is exist.");
                if (_userRepository.Query().Any(u => u.EMail == model.EMail && u.Id != model.Id))
                    return new ErrorResult("User with the same E-Mail is exist.");
                var entity = _userRepository.Query().SingleOrDefault(u => u.Id == model.Id);
                entity.UserName = model.UserName.Trim();
                entity.Password = model.Password.Trim();
                entity.EMail = model.EMail.Trim();
                _userRepository.Update(entity);
                return new SuccessResult();
            }
            catch (Exception exc)
            {
                return new ExceptionResult(exc);
            }
        }
    }
}
