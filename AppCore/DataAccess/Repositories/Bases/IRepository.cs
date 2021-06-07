using AppCore.Records.Base;
using System;
using System.Linq;

namespace AppCore.DataAccess.Repositories.Bases
{
    public interface IRepository<TEntity>: IDisposable where TEntity: Record, new()
    {
        IQueryable<TEntity> Query(params string[] entityToInclude);
        void Add(TEntity entity, bool save = true);
        void Update(TEntity entity, bool save = true);
        void Delete(TEntity entity, bool save = true);
        void Save();
    }
}
