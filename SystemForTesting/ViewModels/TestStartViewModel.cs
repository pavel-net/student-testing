using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemForTesting.Models;
using SystemForTesting.Repositories;
using SystemForTesting.Views;
using Catel.Data;
using Catel.IoC;
using Catel.MVVM;
using Catel.MVVM.Views;
using Catel.Services;

namespace SystemForTesting.ViewModels
{
    public class TestStartViewModel : ViewModelBase
	{
		#region Поля
        private readonly IUIVisualizerService _uiVisualizer;
        private readonly IMessageService _messageService;
        private Test _test;
        private Student _student;
        private IUnitOfWork _unitOfWork;
        private IQuestionRepository _questionRepository;
        private List<Question> _questions;
        private List<int> _listIdTopics;

        private StudentMainViewModel _parentModel;
        private IViewManager _viewManager;
        private bool IsTestStarted = false;
        #endregion

        #region Конструкторы
        public TestStartViewModel(StudentMainViewModel parentModel, Test test, Student student, List<int> listIdTopics, IUnitOfWork unitOfWork, IUIVisualizerService uiVisualizer, IMessageService messageService)
        {
            //try
            //{
                _parentModel = parentModel;
                _uiVisualizer = uiVisualizer;
                _messageService = messageService;
                _student = student;
                _test = test;
                _unitOfWork = unitOfWork;
                NameTest = test.Name;
                CountQuestions = test.CountQuestions;
                _listIdTopics = listIdTopics;
                _questionRepository = _unitOfWork.GetRepository<IQuestionRepository>();
                _viewManager = ServiceLocator.Default.ResolveType<IViewManager>();
            //}
            //catch (Exception e)
            //{
            //    MainWindowViewModel.WriteLog(e.Message, e.ToString());
            //}
        }
		#endregion

		#region Свойства
        public override string Title => "Подготовка к тесту";
        public string NameTest { get; set; }
        public int CountQuestions { get; set; }
		#endregion

		#region Commands

		private Command _startTestCommand;
		public Command StartTestCommand
		{
			get
			{
				return _startTestCommand ?? (_startTestCommand = new Command(async () =>
                {
                    var result = await _messageService.ShowAsync(
                        "При старте выполнения теста будет запущен таймер на выполнение всех заданий. Остановить таймер или выйти из режима тестирования без фиксации результата будет невозможно. Вы готовы приступить к решению теста?",
                        "Внимание",
                        MessageButton.YesNo, MessageImage.Exclamation);
                    if (result != MessageResult.Yes)
                        return;
                    StartTest();
                }));
			}
		}

        #endregion

        #region Methods
        protected override async Task InitializeAsync()
        {
            var viewParent = _viewManager.ActiveViews.FirstOrDefault(v => v is StudentMainView);
            if (viewParent != null)
                viewParent.IsEnabled = false;
            _questions = _questionRepository.GetRandomQuestions(_listIdTopics, CountQuestions);
            await base.InitializeAsync();
        }

        private async void StartTest()
        {
            var testViewModel = new TestProcessViewModel(_test, _student, _questions, _unitOfWork, _uiVisualizer, _messageService);
            IsTestStarted = true;
            await CloseViewModelAsync(true);
            _uiVisualizer.ShowAsync(testViewModel);
        }

        protected override async Task OnClosingAsync()
        {
            if (IsTestStarted)
            {
                _parentModel.CloseFromTest();
            }
            else
            {
                var viewParent = _viewManager.ActiveViews.FirstOrDefault(v => v is StudentMainView);
                if (viewParent != null)
                    viewParent.IsEnabled = true;
            }
            base.OnClosingAsync();
        }
        #endregion
    }
}
