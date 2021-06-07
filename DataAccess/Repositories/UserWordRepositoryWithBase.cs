using AppCore.DataAccess.Repositories.EntityFramework;
using Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public abstract class UserWordRepositoryBase: RepositoryBase<UserWord>
    {
        public UserWordRepositoryBase(DbContext db): base(db)
        {

        }
    }
    public class UserWordRepository: UserWordRepositoryBase
    {
        public UserWordRepository(DbContext db): base(db)
        {

        }
    }
}
