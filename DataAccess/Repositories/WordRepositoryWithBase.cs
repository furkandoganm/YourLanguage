using AppCore.DataAccess.Repositories.EntityFramework;
using Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public abstract class WordRepositoryBase: RepositoryBase<Word>
    {
        protected WordRepositoryBase(DbContext db): base(db)
        {

        }
    }
    public class WordRepository: WordRepositoryBase
    {
        public WordRepository(DbContext db): base(db)
        {

        }
    }
}
