using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemForTesting.Models;
using SystemForTesting.Repositories;
using Catel.Collections;
using Catel.Data;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;

namespace SystemForTesting.ViewModels
{
    public class TopicViewModel : ViewModelBase
    {
        #region Поля
        private readonly IUIVisualizerService _uiVisualizer;
        private readonly IMessageService _messageService;
        private IUnitOfWork _unitOfWork;
        private IQuestionRepository _questionRepository;
        private IAnswerRepository _answerRepository;
        private List<Question> _deleteList;
        private List<QuestionViewModel> _newQuestions;
        #endregion

        #region Конструкторы
        public TopicViewModel(IUIVisualizerService uiVisualizer, IMessageService messageService, Topic topic = null)
        {
            TopicObject = topic ?? new Topic();
            _uiVisualizer = uiVisualizer;
            _messageService = messageService;
            _unitOfWork = this.GetDependencyResolver().Resolve<IUnitOfWork>();
            _questionRepository = _unitOfWork.GetRepository<IQuestionRepository>();
            _answerRepository = _unitOfWork.GetRepository<IAnswerRepository>();
        }
        #endregion

        #region Свойства
        public override string Title => "Тема";

        [Model]
        public Topic TopicObject
        {
            get { return GetValue<Topic>(TopicObjectProperty); }
            set { SetValue(TopicObjectProperty, value); }
        }
        public static readonly PropertyData TopicObjectProperty =
            RegisterProperty(nameof(TopicObject), typeof(Topic), null);

        [ViewModelToModel(Mode = ViewModelToModelMode.Explicit)]
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
        public static readonly PropertyData NameProperty =
            RegisterProperty(nameof(Name), typeof(string), null);


        public FastObservableCollection<QuestionViewModel> QuestionsCollection 
        {
            get { return GetValue<FastObservableCollection<QuestionViewModel>>(nameProperty); }
            set { SetValue(nameProperty, value); }
        }
        public static readonly PropertyData nameProperty =
            RegisterProperty(nameof(QuestionsCollection), typeof(FastObservableCollection<QuestionViewModel>), new FastObservableCollection<QuestionViewModel>());

        public QuestionViewModel SelectedQuestion
        {
            get { return GetValue<QuestionViewModel>(SelectedQuestionProperty); }
            set { SetValue(SelectedQuestionProperty, value); }
        }
        public static readonly PropertyData SelectedQuestionProperty =
            RegisterProperty(nameof(SelectedQuestion), typeof(QuestionViewModel), null);
        #endregion

        #region Commands
        private Command _addCommand;
        public Command AddQuestionCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new Command(() =>
                {
                    //SaveNewTopic();
                    var viewModel = new QuestionViewModel(_uiVisualizer, _messageService, _unitOfWork, _questionRepository, TopicObject.Id);
                    //var viewModel = new TopicViewModel();
                    _uiVisualizer.ShowDialogAsync(viewModel, (sender, e) =>
                    {
                        if (e.Result ?? false)
                        {   
                            QuestionsCollection.Add(viewModel);
                            _newQuestions.Add(viewModel);
                            //AddQuestion(viewModel.QuestionObject);
                            //viewModel.QuestionObject.IdTopic = TopicObject.Id;
                            //_questionRepository.Add(viewModel.QuestionObject);
                            //_unitOfWork.SaveChangesAsync();
                        }
                    });
                }));
            }
        }

        private Command _editCommand;
        public Command EditQuestionCommand
        {
            get
            {
                return _editCommand ?? (_editCommand = new Command( () =>
                {
                    var viewModel = new QuestionViewModel(_uiVisualizer, _messageService, _unitOfWork, _questionRepository, TopicObject.Id, SelectedQuestion.QuestionObject);
                    //SelectedQuestion
                    _uiVisualizer.ShowDialogAsync(viewModel, async (sender, args) =>
                    {
                        if (args.Result ?? false)
                        {
                            // СУПЕР КОСТЫЛЬ, обожаю Catel!!!
                            if (_newQuestions.Contains(SelectedQuestion))
                            {
                                int index = QuestionsCollection.IndexOf(SelectedQuestion); ;
                                QuestionsCollection.Remove(SelectedQuestion);
                                QuestionsCollection.Insert(index, viewModel);
                                SelectedQuestion = viewModel;
                                _newQuestions.Add(viewModel);
                            }
                            else
                            {
                                SelectedQuestion.Content = viewModel.Content;
                                SelectedQuestion.Duration = viewModel.Duration;
                                SelectedQuestion.Score = viewModel.Score;
                                SelectedQuestion.Hint = viewModel.Hint;
                                SelectedQuestion.ContentImage = viewModel.ContentImage;
                            }
                        }
                        else
                        {
                            //string s = SelectedQuestion.QuestionObject.Content;
                            //await SelectedQuestion.CancelNoob(s);
                        }
                    });
                },
                () => SelectedQuestion != null));
            }
        }

        private Command _removeCommand;
        public Command RemoveQuestionCommand
        {
            get
            {
                return _removeCommand ?? (_removeCommand = (new Command(async () =>
                {
                    //SelectedFileObjectPropertyItemViewModel.IsActive = false;
                    //BasketCollection.Add(SelectedFileObjectPropertyItemViewModel.FileObjectProperty);
                    //FileObjectProperties.Remove(SelectedFileObjectPropertyItemViewModel);
                    if (await _messageService.ShowAsync("Вы действительно хотите удалить вопрос?", "Внимание",
                                MessageButton.YesNo, MessageImage.Warning) != MessageResult.Yes)
                        return;
                    //_deleteList.Add(SelectedQuestion.QuestionObject);
                    _questionRepository.Delete(SelectedQuestion.QuestionObject);
                    QuestionsCollection.Remove(SelectedQuestion);
                },
                () => SelectedQuestion != null)));
            }
        }

        private Command _openImageCommand;
        public Command OpenImageCommand
        {
            get
            {
                return _openImageCommand ?? (_openImageCommand = new Command(() =>
                {
                    if (SelectedQuestion.ContentImage == null)
                        return;
                    var viewModel = new ImageViewModel(SelectedQuestion.ContentImage);
                    _uiVisualizer.ShowAsync(viewModel);
                }, () => SelectedQuestion != null));
            }
        }


        private Command _importCommand;
        public Command ImportCommand
        {
            get
            {
                return _importCommand ?? (_importCommand = new Command(async () =>
                {
                    var viewModel = new ImportQuestionsViewModel(new List<QuestionViewModel>(), true, _uiVisualizer,
                        _messageService, _unitOfWork, _questionRepository, TopicObject.Id);
                    await _uiVisualizer.ShowDialogAsync(viewModel);
                    if (viewModel.IsSave)
                    {
                        QuestionsCollection.AddItems(viewModel.QuestionsCollection.Where(q => q.FlagSelected));
                        _newQuestions.AddRange(viewModel.QuestionsCollection.Where(q => q.FlagSelected));
                        //RaisePropertyChanged(nameof(QuestionsCollection));
                    }
                }));
            }
        }

        private Command _exportCommand;
        public Command ExportCommand
        {
            get
            {
                return _exportCommand ?? (_exportCommand = new Command(() =>
                {
                    var viewModel = new ImportQuestionsViewModel(QuestionsCollection.ToList(), false, _uiVisualizer,
                        _messageService, _unitOfWork, _questionRepository, TopicObject.Id);
                    _uiVisualizer.ShowDialogAsync(viewModel);
                }));
            }
        }


        #endregion


        #region Methods

        /// <summary>
        /// Добавляет новую тему в базу данных
        /// </summary>
        protected void SaveNewTopic()
        {
            if (TopicObject.Id != 0)
                return;
            _unitOfWork.GetRepository<ITopicRepository>().Add(TopicObject);
            _unitOfWork.SaveChangesAsync();
        }

        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if (string.IsNullOrEmpty(Name?.Trim()))
            {
                validationResults.Add(FieldValidationResult.CreateError(NameProperty, "Название темы должно быть заполнено"));
            }
            //else if (AnswersCollection.All(a => a.AnswerObject.FlagCorrectly != "Y"))
            //{
            //    validationResults.Add(FieldValidationResult.CreateError(AnswersCollectionProperty, "Хотя бы один ответ должен быть правильным"));
            //}
        }

        //protected void AddQuestion(Question q)
        //{
        //    if (TopicObject.Questions == null)
        //        TopicObject.Questions = new List<Question>();
        //    TopicObject.Questions.Add(q);
        //    q.Answers = new List<Answer>();
        //}

        protected override async Task OnClosingAsync()
        {
            //await SaveAsync();
            base.OnClosingAsync();
        }

        protected override async Task InitializeAsync()
        {
            _deleteList = new List<Question>();
            _newQuestions = new List<QuestionViewModel>();
            if (TopicObject.Id != 0)
                QuestionsCollection = new FastObservableCollection<QuestionViewModel>(_questionRepository
                    .GetQuestions(TopicObject.Id).Select(m =>
                        new QuestionViewModel(_uiVisualizer, _messageService, _unitOfWork, _questionRepository, TopicObject.Id, m)));
            //QuestionsCollection = new FastObservableCollection<QuestionViewModel>(TopicObject.Questions.Select(m =>
            //    new QuestionViewModel(_uiVisualizer, _messageService, _unitOfWork, _answerRepository, m)));
            //QuestionsCollection.AddItems(_questionRepository.GetQuestions(TopicObject.Id)
            //    .Select(m => new QuestionViewModel(_uiVisualizer, _messageService, m)));
            await base.InitializeAsync();
        }

        protected override async Task<bool> SaveAsync()
        {
            //foreach (var question in _deleteList)
            //{
            //    _questionRepository.Delete(question);
            //}
            UpdateExplicitViewModelToModelMappings();
            _unitOfWork.GetRepository<ITopicRepository>().Update(TopicObject);
            await _unitOfWork.SaveChangesAsync();
            return await base.SaveAsync();
        }
        #endregion
    }
}
