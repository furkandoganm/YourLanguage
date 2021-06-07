using AppCore.DataAccess.Repositories.EntityFramework;
using Entities.Entities.Tests;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Repositories
{
    public abstract class SpaceTestRepositoryBase : RepositoryBase<SpaceTest>
    {
        public SpaceTestRepositoryBase(DbContext db) : base(db)
        {

        }
    }
    public class SpaceTestRepository : SpaceTestRepositoryBase
    {
        public SpaceTestRepository(DbContext db) : base(db)
        {

        }
    }
}
