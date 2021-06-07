using AppCore.DataAccess.Repositories.EntityFramework;
using Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public abstract class RoleRepositoryBase : RepositoryBase<Role>
    {
        public RoleRepositoryBase(DbContext db) : base(db)
        {

        }
    }
    public class RoleRepository : RoleRepositoryBase
    {
        public RoleRepository(DbContext db) : base(db)
        {

        }
    }
}
