using AppCore.DataAccess.Repositories.EntityFramework;
using Entities.Entities.Tests;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Repositories
{
    public abstract class QuestionTestRepositoryBase: RepositoryBase<QuestionTest>
    {
        public QuestionTestRepositoryBase(DbContext db): base(db)
        {

        }
    }
    public class QuestionTestRepository: QuestionTestRepositoryBase
    {
        public QuestionTestRepository(DbContext db): base(db)
        {

        }
    }
}
