using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catel.Collections;
using Catel.Data;
using Catel.MVVM;
using Catel.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using SystemForTesting.Models;
using SystemForTesting.Repositories;
using Catel.IoC;

namespace SystemForTesting.ViewModels
{
    public class ImportQuestionsViewModel : ViewModelBase
	{
        #region Поля
        private readonly IUIVisualizerService _uiVisualizer;
        private readonly IMessageService _messageService;
        private readonly IOpenFileService _openFileService;
        private IUnitOfWork _unitOfWork;
        private IQuestionRepository _questionRepository;
        private readonly ISaveFileService _saveFileService;
        private int _idTopic;
        #endregion

        #region Конструкторы
        public ImportQuestionsViewModel(List<QuestionViewModel> questionViewModels, bool isImportMode, IUIVisualizerService uiVisualizer, IMessageService messageService,
            IUnitOfWork unitOfWork, IQuestionRepository questionRepository, int idTopic)
        {
            _messageService = messageService;
            IsImportMode = isImportMode;
            QuestionsCollection = new FastObservableCollection<QuestionViewModel>(questionViewModels);
            Title = IsImportMode ? "Импорт вопросов" : "Экспорт вопросов";
            HeaderTable = IsImportMode ? "Выберите вопросы для добавления к теме" : "Выберите вопросы для экспорта во внешний файл";
            SaveButtonContent = IsImportMode ? "Добавить вопросы к теме" : "Сохранить вопросы во внешний файл";
            IsSave = false;
            _saveFileService = this.GetDependencyResolver().Resolve<ISaveFileService>();
            _uiVisualizer = uiVisualizer;
            _messageService = messageService;
            //_unitOfWork = this.GetDependencyResolver().Resolve<IUnitOfWork>();
            //_answerRepository = _unitOfWork.GetRepository<IAnswerRepository>();
            _openFileService = this.GetDependencyResolver().Resolve<IOpenFileService>();
            _unitOfWork = unitOfWork;
            _questionRepository = questionRepository;
            //_answerRepository = answerRepository;
            _idTopic = idTopic;
        }
		#endregion

		#region Свойства
        public override string Title { get; protected set; }
        public string HeaderTable { get; protected set; }
        public string SaveButtonContent { get; protected set; }
        public bool IsSave { get; set; }

		public FastObservableCollection<QuestionViewModel> QuestionsCollection
        {
            get { return GetValue<FastObservableCollection<QuestionViewModel>>(nameProperty); }
            set { SetValue(nameProperty, value); }
        }
        public static readonly PropertyData nameProperty =
            RegisterProperty(nameof(QuestionsCollection), typeof(FastObservableCollection<QuestionViewModel>), null);

        public bool IsImportMode
		{
			get { return GetValue<bool>(IsImportModeProperty); }
			set { SetValue(IsImportModeProperty, value); }
		}
		public static readonly PropertyData IsImportModeProperty =
			RegisterProperty(nameof(IsImportMode), typeof(bool), true);

        public bool IsNotBusy
        {
            get { return GetValue<bool>(IsNotBusyProperty); }
            set { SetValue(IsNotBusyProperty, value); }
        }
        public static readonly PropertyData IsNotBusyProperty =
            RegisterProperty(nameof(IsNotBusy), typeof(bool), true);
		#endregion

		#region Commands

		private Command _loadQuestionsCommand;
		public Command LoadQuestionsCommand
		{
			get
			{
				return _loadQuestionsCommand ?? (_loadQuestionsCommand = new Command(async () =>
				{
                    _openFileService.Filter = "JSON файл |*.json";
                    if (!(await _openFileService.DetermineFileAsync()))
                        return;
                    if (!await ImportQuestions(_openFileService.FileName))
                        await _messageService.ShowAsync("Произошла ошибка при попытке импортировать данные.",
                            "Ошибка", MessageButton.OK, MessageImage.Error);
                }));
			}
		}

		private Command _saveQuestionsCommand;
		public Command SaveQuestionsCommand
		{
			get
			{
				return _saveQuestionsCommand ?? (_saveQuestionsCommand = new Command(async () =>
                {
                    if (!QuestionsCollection.Any(q => q.FlagSelected))
                    {
                        await _messageService.ShowAsync("Сначала отметьте вопросы для проведения операции импорта/экспорта.",
                            "Отметьте вопросы", MessageButton.OK, MessageImage.Warning);
                    }
                    IsNotBusy = false;
                    bool flagSave = true;
                    if (!IsImportMode)
                    {
                        _saveFileService.Filter = "Json File|*.json";
                        if (await _saveFileService.DetermineFileAsync())
                        {
                            flagSave = await ExportQuestions(_saveFileService.FileName);
                            if (!flagSave)
                            {
                                await _messageService.ShowAsync("Произошла ошибка при попытке экспортировать данные.",
                                    "Ошибка", MessageButton.OK, MessageImage.Error);
                                IsNotBusy = true;
                                return;
                            }
                        }
                        else
                        {
                            IsNotBusy = true;
                            return;
                        }
                    }
                    else
                    {
                        flagSave = await SaveQuestions();
                        if (!flagSave)
                        {
                            await _messageService.ShowAsync("Произошла ошибка при попытке сохранить данные.",
                                "Ошибка", MessageButton.OK, MessageImage.Error);
                            IsNotBusy = true;
                            return;
                        }
                    }
                    await _messageService.ShowAsync("Операция успешно завершена.",
                        "Успех", MessageButton.OK, MessageImage.Information);
                    IsSave = flagSave;
                    Close();
                }, () => QuestionsCollection.Count != 0 && IsNotBusy));
			}
		}

        private Command _closeCommand;
        public Command CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new Command(async () => { Close(); }));
            }
        }
        #endregion

        #region Methods
        protected override async Task InitializeAsync()
        {
            QuestionsCollection.ForEach(q => q.FlagSelected = true);
            await base.InitializeAsync();
        }

        private async void Close()
        {
            await CloseViewModelAsync(true);
        }

        //private void SerializeData()
        //{
        //    //var q = QuestionsCollection[0];
        //    var options = new JsonSerializerOptions
        //    {
        //        WriteIndented = true,
        //        IgnoreNullValues = true
        //    };
        //    var questions = QuestionsCollection.Select(q => q.QuestionObject).ToArray();
        //    var s = JsonSerializer.Serialize<Question[]>(questions, options);
        //    var qs = JsonSerializer.Deserialize<Question[]>(s);
        //}

        private async Task<bool> ExportQuestions(string filePath)
        {
            try
            {
                var arrayQuesions = QuestionsCollection.Where(q => q.FlagSelected).Select(q => q.QuestionObject).ToArray();
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    IgnoreNullValues = true
                };
                var jsonString = JsonSerializer.Serialize<Question[]>(arrayQuesions, options);
                File.WriteAllText(filePath, jsonString);
                return true;
            }
            catch (Exception e)
            { 
                //MainWindowViewModel.WriteLog(e.Message, e.ToString());
                return false;
            }
        }

        private async Task<bool> ImportQuestions(string filePath)
        {
            try
            {
                var jsonString = File.ReadAllText(filePath);
                var arrayQuesions = JsonSerializer.Deserialize<Question[]>(jsonString);
                QuestionsCollection.AddItems(arrayQuesions.Select(q =>
                    new QuestionViewModel(_uiVisualizer, _messageService, _unitOfWork, _questionRepository, _idTopic,
                        q)));
                RaisePropertyChanged(nameof(QuestionsCollection));
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private async Task<bool> SaveQuestions()
        {
            try
            {
                var arrayQuesions = QuestionsCollection.Where(q => q.FlagSelected).Select(q => q.QuestionObject).ToArray();
                foreach (var quesion in arrayQuesions)
                {
                    quesion.IdTopic = _idTopic;
                    _questionRepository.Add(quesion);
                }
                _unitOfWork.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        #endregion
    }
}
