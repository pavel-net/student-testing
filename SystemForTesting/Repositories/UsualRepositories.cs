using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemForTesting.Models;
using Catel.Data.Repositories;

namespace SystemForTesting.Repositories
{
    public interface IDisciplineRepository : IEntityRepository<Discipline, int>
    {
        List<Discipline> GetDisciplines();
        //int GetCountQuestions();
    }

    public class DisciplineRepository : EntityRepositoryBase<Discipline, int>, IDisciplineRepository
    {
        private DbContext myDbContext;
        public DisciplineRepository(DbContext dbContext) : base(dbContext)
        {
            myDbContext = dbContext;
        }

        public List<Discipline> GetDisciplines()
        {
            return GetQuery().ToList();
        }
    }


    public interface ITopicRepository : IEntityRepository<Topic, int>
    {
        List<Topic> GetTopics();
        Topic GetTopicById(int id);
        List<int> GetTopicListId(int idDisp);
    }

    public class TopicRepository : EntityRepositoryBase<Topic, int>, ITopicRepository
    {
        public TopicRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public List<Topic> GetTopics()
        {
            return GetQuery().ToList();
        }

        public Topic GetTopicById(int id)
        {
            return GetQuery().First(t => t.Id == id);
        }

        public List<int> GetTopicListId(int idDisp)
        {
            return GetQuery().Where(t => t.IdDiscipline == idDisp).Select(t => t.Id).ToList();
        }
    }

    public interface IQuestionRepository : IEntityRepository<Question, long>
    {
        List<Question> GetQuestions(int idTopic);
        List<Question> GetRandomQuestions(List<int> idTopics, int count);
    }

    public class QuestionRepository : EntityRepositoryBase<Question, long>, IQuestionRepository
    {
        public QuestionRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public List<Question> GetQuestions(int idTopic)
        {
            return GetQuery().Where(q => q.IdTopic == idTopic).ToList();
        }

        public List<Question> GetRandomQuestions(List<int> idTopics, int count)
        {
            return GetQuery().Where(q => idTopics.Contains(q.IdTopic.Value)).OrderBy(r => Guid.NewGuid()).Take(count)
                .Include(q => q.Answers).ToList();
        }
    }


    public interface IAnswerRepository : IEntityRepository<Answer, long>
    {
        List<Answer> GetAnswers(long idQuestion);
    }

    public class AnswerRepository : EntityRepositoryBase<Answer, long>, IAnswerRepository
    {
        public AnswerRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public List<Answer> GetAnswers(long idQuestion)
        {
            return GetQuery().Where(a => a.IdQuestion == idQuestion).ToList();
        }
    }


    public interface ITestResultRepository : IEntityRepository<TestResult, int>
    {
        List<TestResult> GetResults(int idTest);
    }

    public class TestResultRepository : EntityRepositoryBase<TestResult, int>, ITestResultRepository
    {
        public TestResultRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public List<TestResult> GetResults(int idTest)
        {
            return GetQuery().Where(r => r.IdTest == idTest).Include(t => t.Student).ToList();
        }
    }


    public interface ITestResultAnswerTableRepository : IEntityRepository<TestResultAnswerTable, long>
    {
    }

    public class TestResultAnswerTableRepository : EntityRepositoryBase<TestResultAnswerTable, long>, ITestResultAnswerTableRepository
    {
        public TestResultAnswerTableRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }


    public interface IStudentRepository : IEntityRepository<Student, int>
    {
        Student GetStudent(int id);
        Student GetStudent(string surname, string name, string middleName, string groupNumber);
    }

    public class StudentRepository : EntityRepositoryBase<Student, int>, IStudentRepository
    {
        public StudentRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public Student GetStudent(int id)
        {
            return GetQuery(s => s.Id == id).First();
        }

        public Student GetStudent(string surname, string name, string middleName, string groupNumber)
        {
            return GetQuery()
                .FirstOrDefault(s => s.Surname == surname && s.Name == name && s.MiddleName == middleName &&
                                     s.GroupNumber == groupNumber);
        }
    }


    public interface ITeacherRepository : IEntityRepository<Teacher, int>
    {
        Teacher GetTeacher(int id);
        Teacher GetTeacher(string login, string password);
    }

    public class TeacherRepository : EntityRepositoryBase<Teacher, int>, ITeacherRepository
    {
        public TeacherRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public Teacher GetTeacher(int id)
        {
            return GetQuery(s => s.Id == id).First();
        }

        public Teacher GetTeacher(string login, string password)
        {
            return GetQuery().FirstOrDefault(t => t.Login == login && t.Password == password);
        }
    }

    public interface ITestRepository : IEntityRepository<Test, int>
    {
        List<Test> GetTests();
    }

    public class TestRepository : EntityRepositoryBase<Test, int>, ITestRepository
    {
        public TestRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public List<Test> GetTests()
        {
            return GetQuery().ToList();
        }
    }
}
