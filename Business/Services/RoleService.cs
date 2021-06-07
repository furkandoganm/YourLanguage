using AppCore.Business.Models.Results;
using Business.Enums;
using Business.Models;
using Business.Services.Bases;
using DataAccess.Repositories;
using Entities.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Business.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleRepositoryBase _roleRepository;
        private readonly UserRepositoryBase _userRepository;
        public RoleService(RoleRepositoryBase roleRepository, UserRepositoryBase userRepository)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
        }

        public IQueryable<RoleModel> Query(Expression<Func<RoleModel, bool>> predicate = null)
        {
            var query = _roleRepository.Query().Select(r => new RoleModel
            {
                Id = r.Id,
                GuId = r.GuId,
                Name = r.Name
            });
            return query;
        }

        public Result Add(RoleModel model)
        {
            try
            {
                if (_roleRepository.Query().Any(r => r.Name.ToUpper() == model.Name.Trim().ToUpper()))
                    return new ErrorResult("Same role is already exsist.");
                var entity = new Role()
                {
                    Name = model.Name
                };
                _roleRepository.Add(entity);
                return new SuccessResult();
            }
            catch (Exception exc)
            {
                return new ExceptionResult(exc);
            }
        }

        public Result Update(RoleModel model)
        {
            try
            {
                if (_roleRepository.Query().Any(r => r.Name.ToUpper() == model.Name.Trim().ToUpper() && r.Id != model.Id))
                    return new ErrorResult("Same role is already exsist.");
                var entity = new Role()
                {
                    Id = model.Id,
                    GuId = model.GuId,
                    Name = model.Name
                };
                _roleRepository.Update(entity);
                return new SuccessResult();
            }
            catch (Exception exc)
            {
                return new ExceptionResult(exc);
            }
        }

        public Result Delete(int id)
        {
            //if (_roleRepository.Query().SingleOrDefault(r => r.Id == id).Users != null)
            //    return new ErrorResult("Users have this role, so you can not delete this role");
            var users = _userRepository.Query(u => u.RoleId == id);
            if (users != null)
            {
                foreach (var item in users)
                {
                    item.Active = false;
                    item.RoleId = (int)Roles.User;
                    _userRepository.Update(item, false);
                }
                _userRepository.Save();
            }
            _roleRepository.Delete(id);
            return new SuccessResult();
        }

        public void Dispose()
        {
            _roleRepository?.Dispose();
        }
    }
}
