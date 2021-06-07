using AppCore.DataAccess.Repositories.EntityFramework;
using Entities.Entities.Tests;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Repositories
{
    public abstract class TopicRepositoryBase : RepositoryBase<Topic>
    {
        public TopicRepositoryBase(DbContext db) : base(db)
        {

        }
    }
    public class TopicRepository : TopicRepositoryBase
    {
        public TopicRepository(DbContext db) : base(db)
        {

        }
    }
}
