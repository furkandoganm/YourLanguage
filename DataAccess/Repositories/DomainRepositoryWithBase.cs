using AppCore.DataAccess.Repositories.EntityFramework;
using Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public abstract class DomainRepositoryBase: RepositoryBase<Domain>
    {
        public DomainRepositoryBase(DbContext db): base(db)
        {

        }
    }
    public class DomainRepository: DomainRepositoryBase
    {
        public DomainRepository(DbContext db): base(db)
        {

        }
    }
}
