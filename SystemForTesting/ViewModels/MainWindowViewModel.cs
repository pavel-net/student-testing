using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SystemForTesting.Repositories;
using SystemForTesting.Models;
using SystemForTesting.Singletones;
using Catel.Data;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;

namespace SystemForTesting.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Поля

        private IUnitOfWork _unitOfWork;
        private IStudentRepository _studentRepository;
        private ITeacherRepository _teacherRepository;
        private IMessageService _messageService;
        #endregion

        #region Конструкторы

        public MainWindowViewModel()
        {
            //try
            //{
            Options.ConnectionString = File.ReadAllText("СТРОКА ПОДКЛЮЧЕНИЯ.txt");
            //TestConnection();
            InitialCatel();
            //}
            //catch (Exception e)
            //{
            //    WriteLog(e.Message, e.ToString());
            //}
        }

        #endregion

        #region Свойства
        public override string Title => "Система тестирования";


        public string Surname
        {
            get { return GetValue<string>(SurnameProperty); }
            set { SetValue(SurnameProperty, value); }
        }
        public static readonly PropertyData SurnameProperty =
            RegisterProperty(nameof(Surname), typeof(string), "");

        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
        public static readonly PropertyData NameProperty =
            RegisterProperty(nameof(Name), typeof(string), "");

        public string MiddleName
        {
            get { return GetValue<string>(MiddleNameProperty); }
            set { SetValue(MiddleNameProperty, value); }
        }
        public static readonly PropertyData MiddleNameProperty =
            RegisterProperty(nameof(MiddleName), typeof(string), "");

        public string GroupNumber
        {
            get { return GetValue<string>(GroupNumberProperty); }
            set { SetValue(GroupNumberProperty, value); }
        }
        public static readonly PropertyData GroupNumberProperty =
            RegisterProperty(nameof(GroupNumber), typeof(string), "");


        public string Login
        {
            get { return GetValue<string>(LoginProperty); }
            set { SetValue(LoginProperty, value); }
        }
        public static readonly PropertyData LoginProperty =
            RegisterProperty(nameof(Login), typeof(string), "");

        public string Password
        {
            get { return GetValue<string>(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }
        public static readonly PropertyData PasswordProperty =
            RegisterProperty(nameof(Password), typeof(string), "");

        #endregion

        #region Commands

        private Command _loginStudentCommand;
        public Command LoginStudentCommand
        {
            get
            {
                return _loginStudentCommand ?? (_loginStudentCommand = new Command(() =>
                {
                    if (string.IsNullOrWhiteSpace(Surname) || string.IsNullOrWhiteSpace(GroupNumber))
                    {
                        _messageService.ShowAsync("Фамилия и номер группы не могут быть пустыми полями.",
                            "Заполните поля", MessageButton.OK, MessageImage.Information);
                        return;
                    }
                    string surname = Surname.Trim().ToLower();
                    string group = GroupNumber.ToLower().Trim();
                    string name = Name?.ToLower().Trim();
                    string mName = MiddleName?.ToLower().Trim();
                    Student student = _studentRepository.GetStudent(surname, name, mName, group);
                    if (student == null)
                    {
                        student = new Student()
                        {
                            GroupNumber = group,
                            Name = name,
                            Surname = surname,
                            MiddleName = mName
                        };
                        _studentRepository.Add(student);
                        _unitOfWork.SaveChanges();
                    }
                    StartStudentSession(student);
                }));
            }
        }

        private Command _loginTeacherCommand;
        public Command LoginTeacherCommand
        {
            get
            {
                return _loginTeacherCommand ?? (_loginTeacherCommand = new Command(() =>
                {
                    if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
                    {
                        _messageService.ShowAsync("Сначала введите логин и пароль.",
                            "Заполните поля", MessageButton.OK, MessageImage.Information);
                        return;
                    }
                    string login = Login.ToLower().Trim();
                    string password = Password.ToLower().Trim();
                    Teacher teacher = _teacherRepository.GetTeacher(login, password);
                    if (teacher == null)
                    {
                        _messageService.ShowAsync("Введён неверный логин или пароль!",
                            "Ошибка", MessageButton.OK, MessageImage.Exclamation);
                        return;
                    }
                    StartTeacherSession(teacher);
                }));
            }
        }


        private Command _aboutCommand;
        public Command AboutCommand
        {
            get
            {
                return _aboutCommand ?? (_aboutCommand = new Command(() =>
                {
                    string text = File.ReadAllText("о программе.txt");
                    _messageService.ShowAsync(text,
                        "О программе", MessageButton.OK, MessageImage.Information);
                }));
            }
        }

        private Command _helpStudentCommand;
        public Command HelpStudentCommand
        {
            get
            {
                return _helpStudentCommand ?? (_helpStudentCommand = new Command(() =>
                {
                    //Application.Documents.Open(@"C:\Test\NewDocument.docx");
                    System.Diagnostics.Process.Start("Инструкция для студента.docx");
                }));
            }
        }

        private Command _helpTeacherCommand;
        public Command HelpTeacherCommand
        {
            get
            {
                return _helpTeacherCommand ?? (_helpTeacherCommand = new Command(() =>
                {
                    System.Diagnostics.Process.Start("Инструкция для преподавателя.docx");
                }));
            }
        }



        #endregion

        #region Methods
        private void InitialCatel()
        {
            try
            {
                var serviceLocator = ServiceLocator.Default;
                serviceLocator.RegisterType<ITopicRepository, TopicRepository>(RegistrationType.Transient);
                serviceLocator.RegisterType<IQuestionRepository, QuestionRepository>(RegistrationType.Transient);
                serviceLocator.RegisterType<IAnswerRepository, AnswerRepository>(RegistrationType.Transient);
                serviceLocator.RegisterType<IDisciplineRepository, DisciplineRepository>(RegistrationType.Transient);
                serviceLocator.RegisterType<ITestResultAnswerTableRepository, TestResultAnswerTableRepository>(RegistrationType.Transient);
                serviceLocator.RegisterType<ITestResultRepository, TestResultRepository>(RegistrationType.Transient);
                serviceLocator.RegisterType<ITestRepository, TestRepository>(RegistrationType.Transient);
                serviceLocator.RegisterType<IStudentRepository, StudentRepository>(RegistrationType.Transient);
                serviceLocator.RegisterType<ITeacherRepository, TeacherRepository>(RegistrationType.Transient);

                serviceLocator.RegisterType<IUnitOfWork>(x => new UnitOfWork(new TestDbContext()), RegistrationType.Transient);
                _unitOfWork = this.GetDependencyResolver().Resolve<IUnitOfWork>();
                _studentRepository = _unitOfWork.GetRepository<IStudentRepository>();
                _teacherRepository = _unitOfWork.GetRepository<ITeacherRepository>();
                _messageService = this.GetDependencyResolver().Resolve<IMessageService>();
            }
            catch (Exception e)
            {
                WriteError(e.Message, e.ToString());
                Application.Current.Shutdown();
            }
        }

        private bool TestConnection()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Options.ConnectionString))
                {
                    connection.Open();
                }
                return true;
            }
            catch (Exception e)
            {
                WriteError(
                    "Ошибка подключения к базе данных. Использовалась следующая строка подключения: " +
                    Options.ConnectionString + "\r\n" + e.Message, e.ToString());
                return false;
            }
        }

        private void WriteError(string title, string content)
        {
            MessageBox.Show("Произошла ошибка: " + title + "\r\nПодробное содержание:\r\n" + content, "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void StartStudentSession(Student student)
        {
            var uiVisualizerService = this.GetDependencyResolver().Resolve<IUIVisualizerService>();
            var viewModel = new StudentMainViewModel(student, uiVisualizerService, _messageService);
            uiVisualizerService.ShowAsync(viewModel);
        }

        private void StartTeacherSession(Teacher teacher)
        {
            Login = "";
            Password = "";
            var uiVisualizerService = this.GetDependencyResolver().Resolve<IUIVisualizerService>();
            var viewModel = new TeacherMainViewModel(teacher, uiVisualizerService, _messageService);
            uiVisualizerService.ShowAsync(viewModel);
        }

        private Teacher getTeacher()
        {
            return _teacherRepository.GetTeacher(2);
        }

        private Student getStudent()
        {
            return _studentRepository.GetStudent(1002);
        }

        public static void WriteLog(string title, string content = null)
        {
            using (StreamWriter sw = new StreamWriter("ОТЧЁТ О РАБОТЕ.txt", true, Encoding.UTF8))
            {
                sw.WriteLine();
                sw.WriteLine("=====");
                sw.WriteLine(DateTime.Now.ToString() + " " + title);
                if (content != null)
                    sw.WriteLine(content);
            }
        }
        #endregion
    }
}
