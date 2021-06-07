using AppCore.DataAccess.Configs;
using Entities.Entities;
using Entities.Entities.Tests;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.EntityFramework.Contexts
{
    public class LanguageContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Word> Words { get; set; }
        public DbSet<Domain> Domains { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserWord> UserWords { get; set; }
        public DbSet<QuestionTest> QuestionTests { get; set; }
        public DbSet<SpaceTest> SpaceTests { get; set; }
        public DbSet<Topic> Topics { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionConfig.ConnectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(user => user.Role)
                .WithMany(role => role.Users)
                .HasForeignKey(user => user.RoleId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Word>()
                .HasOne(word => word.Domain)
                .WithMany(domain => domain.Words)
                .HasForeignKey(word => word.DomainId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<UserWord>()
                .HasOne(userWord => userWord.Word)
                .WithMany(word => word.UserWords)
                .HasForeignKey(UserWord => UserWord.WordId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<UserWord>()
                .HasOne(userWord => userWord.User)
                .WithMany(user => user.UserWords)
                .HasForeignKey(UserWord => UserWord.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<QuestionTest>()
                .HasOne(question => question.Topic)
                .WithMany(topic => topic.QuestionTests)
                .HasForeignKey(question => question.TopicId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<SpaceTest>()
                .HasOne(space => space.Topic)
                .WithMany(topic => topic.SpaceTests)
                .HasForeignKey(space => space.TopicId)
                .OnDelete(DeleteBehavior.NoAction);
            //modelBuilder.Entity<Domain>()
            //    .HasIndex(domain => domain.Name)
            //    .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(user => user.EMail)
                .IsUnique();
        }
    }
}
