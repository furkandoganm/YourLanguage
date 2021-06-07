using AppCore.DataAccess.Repositories.Bases;
using AppCore.Records.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace AppCore.DataAccess.Repositories.EntityFramework
{
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : Record, new()
    {
        private readonly DbContext _db;
        public RepositoryBase(DbContext db)
        {
            _db = db;
        }

        public virtual IQueryable<TEntity> Query(params string[] entityToInclude)
        {
            var query = _db.Set<TEntity>().AsQueryable();
            foreach (var entity in entityToInclude)
            {
                query = query.Include(entity);
            }
            return query;
        }
        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate, params string[] entityToInclude)
        {
            var entityQuery = Query(entityToInclude);
            return entityQuery.Where(predicate);
        }

        public virtual void Add(TEntity entity, bool save = true)
        {
            entity.GuId = Guid.NewGuid().ToString();
            _db.Set<TEntity>().Add(entity);
            if (save == true)
                Save();
        }

        public virtual void Update(TEntity entity, bool save = true)
        {
            _db.Set<TEntity>().Update(entity);
            if (save == true)
                Save();
        }

        public virtual void Delete(TEntity entity, bool save = true)
        {
            _db.Set<TEntity>().Remove(entity);
            if (save == true)
                Save();
        }
        public virtual void Delete(int id, bool save = true)
        {
            var result = Query(e => e.Id == id).SingleOrDefault();
            Delete(result);
            if (save)
                Save();
        }

        private bool _disposed = false;

        private void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _db?.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
