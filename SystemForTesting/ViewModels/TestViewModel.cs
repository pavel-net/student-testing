using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemForTesting.Models;
using Catel.Data;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;

namespace SystemForTesting.ViewModels
{
    public class TestViewModel : ViewModelBase
    {
        #region Поля
        private readonly IUIVisualizerService _uiVisualizer;
        private readonly IMessageService _messageService;
        private IUnitOfWork _unitOfWork;
        private Topic _topic;
        private Discipline _discipline;
        #endregion

        #region Конструкторы

        /// <summary>
        /// Редактор теста либо для темы, либо для дисциплины
        /// </summary>
        public TestViewModel(IUIVisualizerService uiVisualizer, IMessageService messageService, Topic topic, Discipline discipline, Test test = null)
        {
            if (test != null)
            {
                IsStaticticsEnable = true;
                //Name = test.Name;
                //CountQuestions = test.CountQuestions;
            }

            TestObject = test ?? new Test();
            _topic = topic;
            _discipline = discipline;
            _uiVisualizer = uiVisualizer;
            _messageService = messageService;
            _unitOfWork = this.GetDependencyResolver().Resolve<IUnitOfWork>();
            if (Name == null)
            {
                Name = topic == null ? ("Тест по дисциплине " + discipline.Name) : ("Тест по теме " + topic.Name);
            }
        }
        #endregion

        #region Свойства
        public override string Title => "Тест";

        [Model]
        public Test TestObject
        {
            get { return GetValue<Test>(TestObjectProperty); }
            set { SetValue(TestObjectProperty, value); }
        }
        public static readonly PropertyData TestObjectProperty =
            RegisterProperty(nameof(TestObject), typeof(Test), null);

        [ViewModelToModel(Mode = ViewModelToModelMode.Explicit)]
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
        public static readonly PropertyData NameProperty =
            RegisterProperty(nameof(Name), typeof(string), null);

        [ViewModelToModel(Mode = ViewModelToModelMode.Explicit)]
        public int CountQuestions
        {
            get { return GetValue<int>(CountQuestionsProperty); }
            set { SetValue(CountQuestionsProperty, value); }
        }
        public static readonly PropertyData CountQuestionsProperty =
            RegisterProperty(nameof(CountQuestions), typeof(int), 0);

        public int CountThemeQuestion
        {
            get { return GetValue<int>(CountThemeQuestionProperty); }
            set { SetValue(CountThemeQuestionProperty, value); }
        }
        public static readonly PropertyData CountThemeQuestionProperty =
            RegisterProperty(nameof(CountThemeQuestion), typeof(int), 0);

        public bool IsStaticticsEnable
        {
            get { return GetValue<bool>(IsStaticticsEnableProperty); }
            set { SetValue(IsStaticticsEnableProperty, value); }
        }
        public static readonly PropertyData IsStaticticsEnableProperty =
            RegisterProperty(nameof(IsStaticticsEnable), typeof(bool), false);

        #endregion

        #region Commands


        private Command _openStatisticsCommand;
        public Command OpenStatisticsCommand
        {
            get
            {
                return _openStatisticsCommand ?? (_openStatisticsCommand = new Command(async () =>
                {
                    var testViewModel = new StatisticsTestViewModel(TestObject, _unitOfWork);
                    await _uiVisualizer.ShowDialogAsync(testViewModel);
                }));
            }
        }

        #endregion

        #region Methods

        protected override async Task InitializeAsync()
        {
            using (TestDbContext db = new TestDbContext())
            {
                IQueryable<int> query;
                if (_topic != null)
                {
                    query = from topic in db.Topics
                        join question in db.Questions on topic.Id equals question.IdTopic into questions
                        where topic.Id == _topic.Id
                        select questions.Count();

                }
                else
                {
                    query = from dis in db.Disciplines
                        join topic in db.Topics on dis.Id equals topic.IdDiscipline
                        join question in db.Questions on topic.Id equals question.IdTopic into questions
                        where dis.Id == _discipline.Id
                        select questions.Count();
                }
                CountThemeQuestion = query.Sum();
                //CountThemeQuestion = query.First();
            }
            await base.InitializeAsync();
        }


        protected override async Task<bool> SaveAsync()
        {
            UpdateExplicitViewModelToModelMappings();
            //await _unitOfWork.SaveChangesAsync();
            return await base.SaveAsync();
        }

        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if (string.IsNullOrEmpty(Name?.Trim()))
            {
                validationResults.Add(FieldValidationResult.CreateError(NameProperty, "Название теста не может быть пустым"));
            }

            if (CountQuestions == 0)
            {
                validationResults.Add(FieldValidationResult.CreateError(CountQuestionsProperty, "Количество вопросов не может быть равно 0"));
            }
        }
        #endregion
    }
}
