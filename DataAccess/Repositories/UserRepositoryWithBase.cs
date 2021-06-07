using AppCore.DataAccess.Repositories.EntityFramework;
using Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public abstract class UserRepositoryBase: RepositoryBase<User>
    {
        public UserRepositoryBase(DbContext db): base(db)
        {

        }
    }
    public class UserRepository: UserRepositoryBase
    {
        public UserRepository(DbContext db): base(db)
        {

        }
    }
}
