using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SystemForTesting.Repositories;
using SystemForTesting.Models;
using Catel.Collections;
using Catel.Data;
using Catel.IO;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using DevExpress.Xpf.Core.DataSources;
using DevExpress.Xpf.Core.ServerMode;

namespace SystemForTesting.ViewModels
{
    public class QuestionViewModel : ViewModelBase
    {
        #region Поля
        private readonly IUIVisualizerService _uiVisualizer;
        private readonly IMessageService _messageService;
        private readonly IOpenFileService _openFileService;
        private IUnitOfWork _unitOfWork;
        private IQuestionRepository _questionRepository;
        //private IAnswerRepository _answerRepository;
        private List<Answer> _deleteList = new List<Answer>();
        private List<Answer> _addList = new List<Answer>();
        private TestDbContext _db;
        private int _idTopic;
        #endregion

        #region Конструкторы

        public QuestionViewModel(IUIVisualizerService uiVisualizer, IMessageService messageService,
            IUnitOfWork unitOfWork, IQuestionRepository questionRepository, int idTopic, Question question = null)
        {
            QuestionObject = question ?? new Question();
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
        public override string Title => "Вопрос";

        [Model]
        public Question QuestionObject 
        {
            get { return GetValue<Question>(nameProperty); }
            set { SetValue(nameProperty, value); }
        }
        public static readonly PropertyData nameProperty =
            RegisterProperty(nameof(QuestionObject), typeof(Question), null);

        [ViewModelToModel(Mode = ViewModelToModelMode.Explicit)]
        public string Content
        {
            get { return GetValue<string>(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        public static readonly PropertyData ContentProperty =
            RegisterProperty(nameof(Content), typeof(string), null);

        [ViewModelToModel(Mode = ViewModelToModelMode.Explicit)]
        public int Duration
        {
            get { return GetValue<int>(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }
        public static readonly PropertyData DurationProperty =
            RegisterProperty(nameof(Duration), typeof(int), 0);

        [ViewModelToModel(Mode = ViewModelToModelMode.Explicit)]
        public int Score
        {
            get { return GetValue<int>(ScoreProperty); }
            set { SetValue(ScoreProperty, value); }
        }
        public static readonly PropertyData ScoreProperty =
            RegisterProperty(nameof(Score), typeof(int), 0);

        [ViewModelToModel(Mode = ViewModelToModelMode.Explicit)]
        public string Hint
        {
            get { return GetValue<string>(HintProperty); }
            set { SetValue(HintProperty, value); }
        }
        public static readonly PropertyData HintProperty =
            RegisterProperty(nameof(Hint), typeof(string), null);
        
        [ViewModelToModel(Mode = ViewModelToModelMode.Explicit)]
        public string Hint2
        {
            get { return GetValue<string>(Hint2Property); }
            set { SetValue(Hint2Property, value); }
        }
        public static readonly PropertyData Hint2Property =
            RegisterProperty(nameof(Hint2), typeof(string), null);

        [ViewModelToModel(Mode = ViewModelToModelMode.Explicit)]
        public string Hint3
        {
            get { return GetValue<string>(Hint3Property); }
            set { SetValue(Hint3Property, value); }
        }
        public static readonly PropertyData Hint3Property =
            RegisterProperty(nameof(Hint3), typeof(string), null);

        [ViewModelToModel(Mode = ViewModelToModelMode.Explicit)]
        public byte[] HintImage
        {
            get { return GetValue<byte[]>(HintImageProperty); }
            set { SetValue(HintImageProperty, value); }
        }
        public static readonly PropertyData HintImageProperty =
            RegisterProperty(nameof(HintImage), typeof(byte[]), null);

        [ViewModelToModel(Mode = ViewModelToModelMode.Explicit)]
        public byte[] Hint2Image
        {
            get { return GetValue<byte[]>(Hint2ImageProperty); }
            set { SetValue(Hint2ImageProperty, value); }
        }
        public static readonly PropertyData Hint2ImageProperty =
            RegisterProperty(nameof(Hint2Image), typeof(byte[]), null);

        [ViewModelToModel(Mode = ViewModelToModelMode.Explicit)]
        public byte[] Hint3Image
        {
            get { return GetValue<byte[]>(Hint3ImageProperty); }
            set { SetValue(Hint3ImageProperty, value); }
        }
        public static readonly PropertyData Hint3ImageProperty =
            RegisterProperty(nameof(Hint3Image), typeof(byte[]), null);

        [ViewModelToModel(Mode = ViewModelToModelMode.Explicit)]
        public byte[] ContentImage
        {
            get { return GetValue<byte[]>(ContentImageProperty); }
            set { SetValue(ContentImageProperty, value); }
        }
        public static readonly PropertyData ContentImageProperty =
            RegisterProperty(nameof(ContentImage), typeof(byte[]), null);

        public bool FlagSelected
        {
            get { return GetValue<bool>(FlagSelectedProperty); }
            set { SetValue(FlagSelectedProperty, value); }
        }
        public static readonly PropertyData FlagSelectedProperty =
            RegisterProperty(nameof(FlagSelected), typeof(bool), true);


        public FastObservableCollection<AnswerViewModel> AnswersCollection
        {
            get { return GetValue<FastObservableCollection<AnswerViewModel>>(AnswersCollectionProperty); }
            set { SetValue(AnswersCollectionProperty, value); }
        }
        public static readonly PropertyData AnswersCollectionProperty =
            RegisterProperty(nameof(AnswersCollection), typeof(FastObservableCollection<AnswerViewModel>), new FastObservableCollection<AnswerViewModel>());

        public AnswerViewModel SelectedAnswer
        {
            get { return GetValue<AnswerViewModel>(SelectedAnswerProperty); }
            set { SetValue(SelectedAnswerProperty, value); }
        }
        public static readonly PropertyData SelectedAnswerProperty =
            RegisterProperty(nameof(SelectedAnswer), typeof(AnswerViewModel), null);


        //public IQueryable<Answer> EntityAnswers
        //{
        //    get { return GetValue<IQueryable<Answer>>(EntityAnswersProperty); }
        //    set { SetValue(EntityAnswersProperty, value); }
        //}
        //public static readonly PropertyData EntityAnswersProperty =
        //    RegisterProperty(nameof(EntityAnswers), typeof(IQueryable<Answer>), null);

        //public EntitySimpleDataSource EntityAnswers
        //{
        //    get { return GetValue<EntitySimpleDataSource>(EntityAnswersProperty); }
        //    set { SetValue(EntityAnswersProperty, value); }
        //}
        //public static readonly PropertyData EntityAnswersProperty =
        //    RegisterProperty(nameof(EntityAnswers), typeof(EntitySimpleDataSource), null);

        //public ObservableCollection<Answer> EntityAnswers { get; set; }

        public ObservableCollection<Answer> EntityAnswers
        {
            get { return GetValue<ObservableCollection<Answer>>(EntityAnswersProperty); }
            set { SetValue(EntityAnswersProperty, value); }
        }
        public static readonly PropertyData EntityAnswersProperty =
            RegisterProperty(nameof(EntityAnswers), typeof(ObservableCollection<Answer>), null);

        #endregion

        #region Commands
        private Command _addCommand;
        public Command AddAnswerCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new Command(() =>
                {
                    var viewModel = new AnswerViewModel();
                    viewModel.Content = "Текст ответа";
                    AnswersCollection.Add(viewModel);
                    _addList.Add(viewModel.AnswerObject);
                    //_uiVisualizer.ShowDialogAsync(viewModel, (sender, e) =>
                    //{
                    //    if (e.Result ?? false)
                    //    {
                    //        AnswersCollection.Add(viewModel);
                    //        _addList.Add(viewModel.AnswerObject);
                    //        //viewModel.AnswerObject.IdQuestion = QuestionObject.Id;
                    //        //AddAnswer(viewModel.AnswerObject);
                    //        //_answerRepository.Add(viewModel.AnswerObject);
                    //    }
                    //});
                }));
            }
        }

        //private Command _editCommand;
        //public Command EditAnswerCommand
        //{
        //    get
        //    {
        //        return _editCommand ?? (_editCommand = new Command(() =>
        //        {
        //            var viewModel = new AnswerViewModel(SelectedAnswer.AnswerObject);
        //            _uiVisualizer.ShowDialogAsync(viewModel, (sender, args) =>
        //            {
        //                if (args.Result ?? false)
        //                {
        //                    SelectedAnswer.Content = viewModel.Content;
        //                    SelectedAnswer.FlagCorrectly = viewModel.FlagCorrectly;
        //                    SelectedAnswer.Content = viewModel.Content;
        //                }
        //            });
        //        },
        //        () => SelectedAnswer != null));
        //    }
        //}

        private Command _removeCommand;
        public Command RemoveAnswerCommand
        {
            get
            {
                return _removeCommand ?? (_removeCommand = (new Command(async () =>
                {
                    //if (await _messageService.ShowAsync("Вы действительно хотите удалить ответ?", "Внимание",
                    //            MessageButton.YesNo, MessageImage.Warning) != MessageResult.Yes)
                    //    return;
                    //if (SelectedAnswer.AnswerObject.Id == 0)
                    //    QuestionObject.Answers.Remove(SelectedAnswer.AnswerObject);
                    //else
                    //    _deleteList.Add(SelectedAnswer.AnswerObject);
                    //_answerRepository.Delete(SelectedAnswer.AnswerObject);
                    if (_addList.Contains(SelectedAnswer.AnswerObject))
                        _addList.Remove(SelectedAnswer.AnswerObject);
                    _deleteList.Add(SelectedAnswer.AnswerObject);
                    AnswersCollection.Remove(SelectedAnswer);
                },
                () => SelectedAnswer != null)));
            }
        }

        /// <summary>
        /// Загрузка нового изображения
        /// </summary>
        private Command _loadImageCommand;
        public Command LoadImageCommand
        {
            get
            {
                return _loadImageCommand ?? (_loadImageCommand = new Command(async () =>
                {
                    byte[] image = await LoadImage();
                    if (image != null)
                        ContentImage = image;
                }));
            }
        }

        /// <summary>
        /// Удаление изображения
        /// </summary>
        private Command _deleteImageCommand;
        public Command DeleteImageCommand
        {
            get
            {
                return _deleteImageCommand ?? (_deleteImageCommand = new Command(async () =>
                {
                    if (await _messageService.ShowAsync("Вы действительно хотите удалить изображение?", "Внимание",
                            MessageButton.YesNo, MessageImage.Warning) != MessageResult.Yes)
                        return;
                    ContentImage = null;
                    RaisePropertyChanged(nameof(ContentImage));
                }, () => ContentImage != null));
            }
        }


        private Command _openImageCommand;
        public Command OpenImageCommand
        {
            get
            {
                return _openImageCommand ?? (_openImageCommand = new Command(() =>
                {
                    var viewModel = new ImageViewModel(ContentImage);
                    _uiVisualizer.ShowAsync(viewModel);
                }, () => ContentImage != null));
            }
        }


        private Command _openImageAnswerCommand;
        public Command OpenImageAnswerCommand
        {
            get
            {
                return _openImageAnswerCommand ?? (_openImageAnswerCommand = new Command(() =>
                {
                    var viewModel = new ImageViewModel(SelectedAnswer.ContentImage);
                    _uiVisualizer.ShowAsync(viewModel);
                }, () => SelectedAnswer?.ContentImage != null));
            }
        }



        private Command<string> _loadHintImageCommand;
        public Command<string> LoadHintImageCommand
        {
            get
            {
                return _loadHintImageCommand ?? (_loadHintImageCommand = new Command<string>(async nameHint =>
                {
                    byte[] image = await LoadImage();
                    if (image == null)
                        return;
                    switch (nameHint)
                    {
                        case "hint1":
                            HintImage = image;
                            break;
                        case "hint2":
                            Hint2Image = image;
                            break;
                        case "hint3":
                            Hint3Image = image;
                            break;
                    }
                }));
            }
        }

        private Command<string> _deleteHintImageCommand;
        public Command<string> DeleteHintImageCommand
        {
            get
            {
                return _deleteHintImageCommand ?? (_deleteHintImageCommand = new Command<string>(async nameHint =>
                {
                    if (await _messageService.ShowAsync("Вы действительно хотите удалить изображение?", "Внимание",
                            MessageButton.YesNo, MessageImage.Warning) != MessageResult.Yes)
                        return;
                    switch (nameHint)
                    {
                        case "hint1":
                            HintImage = null;
                            RaisePropertyChanged(nameof(HintImage));
                            break;
                        case "hint2":
                            Hint2Image = null;
                            RaisePropertyChanged(nameof(Hint2Image));
                            break;
                        case "hint3":
                            Hint3Image = null;
                            RaisePropertyChanged(nameof(Hint3Image));
                            break;
                    }
                }, nameHint =>
                {
                    switch (nameHint)
                    {
                        case "hint1":
                            return HintImage != null;
                        case "hint2":
                            return Hint2Image != null;
                        case "hint3":
                            return Hint3Image != null;
                        default:
                            return false;
                    }
                }));
            }
        }

        private Command<string> _openHintImageCommand;
        public Command<string> OpenHintImageCommand
        {
            get
            {
                return _openHintImageCommand ?? (_openHintImageCommand = new Command<string>(nameHint =>
                {
                    byte[] image = null;
                    switch (nameHint)
                    {
                        case "hint1":
                            image = HintImage;
                            break;
                        case "hint2":
                            image = Hint2Image;
                            break;
                        case "hint3":
                            image = Hint3Image;
                            break;
                    }
                    var viewModel = new ImageViewModel(image);
                    _uiVisualizer.ShowAsync(viewModel);
                }, nameHint =>
                {
                    switch (nameHint)
                    {
                        case "hint1":
                            return HintImage != null;
                        case "hint2":
                            return Hint2Image != null;
                        case "hint3":
                            return Hint3Image != null;
                        default:
                            return false;
                    }
                }));
            }
        }


        #endregion

        #region Methods

        private async Task<byte[]> LoadImage()
        {
            _openFileService.Filter = "Изображения (*.bmp;*.jpg;*.png;*.tif)|*.bmp;*.jpg;*.png;*.tif";
            if (!(await _openFileService.DetermineFileAsync()))
                return null;
            try
            {

                var img = Image.FromFile(_openFileService.FileName);
                using (Stream stream = new MemoryStream())
                {
                    img.Save(stream, ImageFormat.Jpeg);
                    return stream.ToByteArray();
                }
            }
            catch (Exception exception)
            {
                await _messageService.ShowAsync("Ошибка загрузки изображения", "Ошибка", MessageButton.OK,
                    MessageImage.Error);
                return null;
            }
        }

        /// <summary>
        ///  метод обновления привязки
        /// </summary>
        //public void UpdateBinding(string сontent, int duration, int score, )
        //{
        //    RaisePropertyChanged(nameof(QuestionObject));
        //}

        protected override async Task InitializeAsync()
        {
            _deleteList = new List<Answer>();
            _db = new TestDbContext();
            _db.Answers.Where(a => a.IdQuestion == QuestionObject.Id).Load();
            AnswersCollection =
                new FastObservableCollection<AnswerViewModel>(_db.Answers.Local.Select(a => new AnswerViewModel(a)));
            await base.InitializeAsync();
        }

        protected void SaveNewQuestion()
        {
            if (QuestionObject.Id != 0)
                return;
            //var _questionRepository = _unitOfWork.GetRepository<IQuestionRepository>();
            QuestionObject.IdTopic = _idTopic;
            _questionRepository.Add(QuestionObject);
        }

        protected override async Task<bool> SaveAsync()
        {
            if (AnswersCollection == null || AnswersCollection.Count == 0)
            {
                await _messageService.ShowAsync("Список ответов не может быть пустым. Добавьте варианты ответов.", "Сохранение невозможно",
                    MessageButton.OK, MessageImage.Error);
                return false;
            } 
            else if (AnswersCollection.All(a => a.AnswerObject.FlagCorrectly != "Y"))
            {
                await _messageService.ShowAsync("Выберите хотя бы один правильный вариант ответа.", "Сохранение невозможно",
                    MessageButton.OK, MessageImage.Error);
                return false;
            }
            UpdateExplicitViewModelToModelMappings();
            SaveNewQuestion();
            await _unitOfWork.SaveChangesAsync();

            foreach (var answer in _addList)
            {
                answer.IdQuestion = QuestionObject.Id;
                if (answer.FlagCorrectly == null)
                    answer.FlagCorrectly = "N";
                _db.Answers.Add(answer);
            }
            foreach (var answer in _deleteList.Where(a => a.Id != 0))
            {
                _db.Answers.Remove(answer);
            }
            _db.SaveChanges();

            if (_db != null)
                _db.Dispose();
            return await base.SaveAsync();
            //if (QuestionObject.Id != 0)
            //{
            //    foreach (var answer in _deleteList)
            //    {
            //        _answerRepository.Delete(answer);
            //    }

            //    foreach (var answerViewModel in AnswersCollection)
            //    {
            //        if (answerViewModel.AnswerObject.Id == 0)
            //        {
            //            answerViewModel.AnswerObject.IdQuestion = QuestionObject.Id;
            //            _answerRepository.Add(answerViewModel.AnswerObject);
            //        }
            //    }
            //    //QuestionObject.IdTopic = _idTopic;
            //    //_unitOfWork.GetRepository<IQuestionRepository>().Add(QuestionObject);
            //    _unitOfWork.SaveChanges();
            //    //_unitOfWork.GetRepository<IQuestionRepository>().Add(QuestionObject);
            //}
            //await _unitOfWork.SaveChangesAsync();
        }

        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if (string.IsNullOrEmpty(Content?.Trim()))
            {
                validationResults.Add(FieldValidationResult.CreateError(ContentProperty, "Текст вопроса не может быть пустым"));
            }
            if (Duration == 0)
            {
                validationResults.Add(FieldValidationResult.CreateError(ContentProperty, "Время на ответ не может быть равным 0"));
            }
            if (Score == 0)
            {
                validationResults.Add(FieldValidationResult.CreateError(ContentProperty, "Количество баллов не может быть равно 0"));
            }
            //else if (AnswersCollection.All(a => a.AnswerObject.FlagCorrectly != "Y"))
            //{
            //    validationResults.Add(FieldValidationResult.CreateError(AnswersCollectionProperty, "Хотя бы один ответ должен быть правильным"));
            //}
        }


        protected override async Task OnClosingAsync()
        {
            if (_db != null)
                _db.Dispose();
            //await CancelAsync();
            base.OnClosingAsync();
        }
        #endregion
    }
}
