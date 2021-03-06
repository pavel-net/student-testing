using SystemForTesting.Singletones;

namespace SystemForTesting.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class TestDbContext : DbContext
    {
        //public TestDbContext()
        //    : base("name=TestDbContext")
        //{
        //}
        public TestDbContext()
            : base(Options.ConnectionString)
        {
        }

        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<Discipline> Disciplines { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }
        public virtual DbSet<TestResultAnswerTable> TestResultAnswerTables { get; set; }
        public virtual DbSet<TestResult> TestResults { get; set; }
        public virtual DbSet<Test> Tests { get; set; }
        public virtual DbSet<Topic> Topics { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Answer>()
                .Property(e => e.FlagCorrectly)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Answer>()
                .HasMany(e => e.TestResultAnswerTables)
                .WithOptional(e => e.Answer)
                .HasForeignKey(e => e.IdAnswer)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Discipline>()
                .HasMany(e => e.Tests)
                .WithOptional(e => e.Discipline)
                .HasForeignKey(e => e.IdDiscipline)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Discipline>()
                .HasMany(e => e.Topics)
                .WithOptional(e => e.Discipline)
                .HasForeignKey(e => e.IdDiscipline);

            modelBuilder.Entity<Question>()
                .HasMany(e => e.Answers)
                .WithOptional(e => e.Question)
                .HasForeignKey(e => e.IdQuestion)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Student>()
                .HasMany(e => e.TestResults)
                .WithOptional(e => e.Student)
                .HasForeignKey(e => e.IdStudent)
                .WillCascadeOnDelete();

            modelBuilder.Entity<TestResult>()
                .HasMany(e => e.TestResultAnswerTables)
                .WithOptional(e => e.TestResult)
                .HasForeignKey(e => e.IdTestResult);

            modelBuilder.Entity<Test>()
                .HasMany(e => e.TestResults)
                .WithOptional(e => e.Test)
                .HasForeignKey(e => e.IdTest)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Topic>()
                .HasMany(e => e.Questions)
                .WithOptional(e => e.Topic)
                .HasForeignKey(e => e.IdTopic)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Topic>()
                .HasMany(e => e.Tests)
                .WithOptional(e => e.Topic)
                .HasForeignKey(e => e.IdTopic)
                .WillCascadeOnDelete();
        }
    }
}
