using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemForTesting.Models;
using Catel.Data;
using Catel.MVVM;
using Catel.Services;
using System.Timers;
using SystemForTesting.Repositories;
using Catel.Collections;

namespace SystemForTesting.ViewModels
{
    public class TestProcessViewModel : ViewModelBase
    {
        #region Поля
        private readonly IUIVisualizerService _uiVisualizer;
        private readonly IMessageService _messageService;
        private Timer _timer;
        private int _durationTest;
        private int _timeTest;

        private Test _test;
        private Student _student;
        private IUnitOfWork _unitOfWork;
        private bool IsFinished = false;
        #endregion

        #region Конструкторы

        public TestProcessViewModel(Test test, Student student, List<Question> questions, IUnitOfWork unitOfWork, IUIVisualizerService uiVisualizer, IMessageService messageService)
        {
            _uiVisualizer = uiVisualizer;
            _messageService = messageService;
            _student = student;
            _test = test;
            NameTest = test.Name;
            _unitOfWork = unitOfWork;
            Initial(questions);
        }
        #endregion

        #region Свойства

        public override string Title => "Тестирование";
        public string NameTest { get; set; }
        public ObservableCollection<TestQuestionViewModel> QuestionsCollection { get; set; }


        public TestQuestionViewModel SelectedQuestionViewModel
        {
            get { return GetValue<TestQuestionViewModel>(SelectedQuestionViewModelProperty); }
            set
            {
                SetValue(SelectedQuestionViewModelProperty, value);
                UpdateHintStatus(value);
            }
        }
        public static readonly PropertyData SelectedQuestionViewModelProperty =
            RegisterProperty(nameof(SelectedQuestionViewModel), typeof(TestQuestionViewModel), null);

        public bool IsButtonHintEnable
        {
            get { return GetValue<bool>(IsButtonHintEnableProperty); }
            set { SetValue(IsButtonHintEnableProperty, value); }
        }
        public static readonly PropertyData IsButtonHintEnableProperty =
            RegisterProperty(nameof(IsButtonHintEnable), typeof(bool), false);

        public bool IsHintEnable
        {
            get { return GetValue<bool>(IsHintEnableProperty); }
            set { SetValue(IsHintEnableProperty, value); }
        }
        public static readonly PropertyData IsHintEnableProperty =
            RegisterProperty(nameof(IsHintEnable), typeof(bool), false);

        public bool IsHint2Enable
        {
            get { return GetValue<bool>(IsHint2EnableProperty); }
            set { SetValue(IsHint2EnableProperty, value); }
        }
        public static readonly PropertyData IsHint2EnableProperty =
            RegisterProperty(nameof(IsHint2Enable), typeof(bool), false);

        public bool IsHint3Enable
        {
            get { return GetValue<bool>(IsHint3EnableProperty); }
            set { SetValue(IsHint3EnableProperty, value); }
        }
        public static readonly PropertyData IsHint3EnableProperty =
            RegisterProperty(nameof(IsHint3Enable), typeof(bool), false);

        public string TitleHintButton
        {
            get { return GetValue<string>(TitleHintButtonProperty); }
            set { SetValue(TitleHintButtonProperty, value); }
        }
        public static readonly PropertyData TitleHintButtonProperty =
            RegisterProperty(nameof(TitleHintButton), typeof(string), "Использовать подсказку");

        public DateTime TimerValue
        {
            get { return GetValue<DateTime>(TimerValueProperty); }
            set { SetValue(TimerValueProperty, value); }
        }
        public static readonly PropertyData TimerValueProperty =
            RegisterProperty(nameof(TimerValue), typeof(DateTime), DateTime.MaxValue);

        #endregion

        #region Commands


        private Command _hintCommand;
        public Command HintCommand
        {
            get
            {
                return _hintCommand ?? (_hintCommand = new Command(async () =>
                {
                    var result = await _messageService.ShowAsync(
                        "Вы уверены, что хотите использовать подсказку? За правильный ответ на данный вопрос вы получите меньше баллов.", "Внимание",
                        MessageButton.YesNo, MessageImage.Warning);
                    if (result != MessageResult.Yes)
                        return;
                    SelectedQuestionViewModel.ActivateHint();
                    UpdateHintStatus(SelectedQuestionViewModel);
                }, () => IsButtonHintEnable));
            }
        }


        private Command _nextQuestionCommand;
        public Command NextQuestionCommand
        {
            get
            {
                return _nextQuestionCommand ?? (_nextQuestionCommand = new Command(() =>
                {
                    int index = QuestionsCollection.IndexOf(SelectedQuestionViewModel);
                    SelectedQuestionViewModel = QuestionsCollection[index + 1];
                }, () => SelectedQuestionViewModel != null && QuestionsCollection.Last() != SelectedQuestionViewModel));
            }
        }


        private Command _prevQuestionCommand;
        public Command PrevQuestionCommand
        {
            get
            {
                return _prevQuestionCommand ?? (_prevQuestionCommand = new Command(() =>
                {
                    int index = QuestionsCollection.IndexOf(SelectedQuestionViewModel);
                    SelectedQuestionViewModel = QuestionsCollection[index - 1];
                }, () => SelectedQuestionViewModel != null && QuestionsCollection.First() != SelectedQuestionViewModel));
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
                            image = SelectedQuestionViewModel.QuestionObject.HintImage;
                            break;
                        case "hint2":
                            image = SelectedQuestionViewModel.QuestionObject.Hint2Image;
                            break;
                        case "hint3":
                            image = SelectedQuestionViewModel.QuestionObject.Hint3Image;
                            break;
                    }
                    var viewModel = new ImageViewModel(image);
                    _uiVisualizer.ShowAsync(viewModel);
                }, nameHint =>
                {
                    if (SelectedQuestionViewModel == null)
                        return false;
                    switch (nameHint)
                    {
                        case "hint1":
                            return SelectedQuestionViewModel.QuestionObject.HintImage != null;
                        case "hint2":
                            return SelectedQuestionViewModel.QuestionObject.Hint2Image != null;
                        case "hint3":
                            return SelectedQuestionViewModel.QuestionObject.Hint3Image != null;
                        default:
                            return false;
                    }
                }));
            }
        }


        private Command _finishTestCommand;
        public Command FinishTestCommand
        {
            get
            {
                return _finishTestCommand ?? (_finishTestCommand = new Command(async () =>
                {
                    if (!IsAllAnswerFixed())
                    {
                        var result = await _messageService.ShowAsync(
                            "Вы ответили ещё не на все вопросы. Всё равно завершить тестирование?",
                            "Внимание",
                            MessageButton.YesNo, MessageImage.Exclamation);
                        if (result != MessageResult.Yes)
                            return;
                    }
                    Finish();
                }));
            }
        }


        #endregion

        #region Methods

        private void UpdateHintStatus(TestQuestionViewModel value)
        {
            if (value == null || !value.IsHaveHint())
            {
                TitleHintButton = "Использовать подсказку";
                IsButtonHintEnable = false;
                IsHintEnable = false;
                IsHint2Enable = false;
                IsHint3Enable = false;
            }
            else if (value.IsHint3Activate)
            {   // 3я подсказка уже активирована
                TitleHintButton = "Использовать третью подсказку";
                IsButtonHintEnable = false;
                IsHintEnable = true;
                IsHint2Enable = true;
                IsHint3Enable = true;
            }
            else if (value.IsHint2Activate)
            {   // 2я подсказка уже активирована
                TitleHintButton = "Использовать третью подсказку";
                IsButtonHintEnable = value.IsHaveHint3();
                IsHintEnable = true;
                IsHint2Enable = true;
                IsHint3Enable = false;
            }
            else if (value.IsHintActivate)
            {   // 1я подсказка уже активирована
                TitleHintButton = "Использовать вторую подсказку";
                IsButtonHintEnable = value.IsHaveHint2();
                IsHintEnable = true;
                IsHint2Enable = false;
                IsHint3Enable = false;
            }
            else
            {
                TitleHintButton = "Использовать подсказку";
                IsButtonHintEnable = value.IsHaveHint();
                IsHintEnable = false;
                IsHint2Enable = false;
                IsHint3Enable = false;
            }
        }

        private void Initial(List<Question> questions)
        {
            int num = 0;
            QuestionsCollection =
                new ObservableCollection<TestQuestionViewModel>(questions.Select(q =>
                    new TestQuestionViewModel(q, ++num, _uiVisualizer)));
            SelectedQuestionViewModel = QuestionsCollection[0];
            _timeTest = questions.Sum(q => q.Duration);
            TimerValue = new DateTime(2019, 1, 1, 0, 0, 0).AddSeconds(_timeTest);
            SetTimer();
        }
        private void SetTimer()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (TimerValue.Hour == 0 && TimerValue.Minute == 0 && TimerValue.Second <= 1.0)
            {
                _timer.Enabled = false;
                Finish();
            }
            _durationTest++;
            TimerValue = TimerValue.AddSeconds(-1.0);
        }

        private void Finish()
        {
            TestResult result = FixResult();
            IsFinished = true;
            CloseViewModelAsync(true);
            var finishViewModel = new TestFinishViewModel(_test.Name, _student, result);
            _uiVisualizer.ShowAsync(finishViewModel);
        }

        private TestResult FixResult()
        {
            string rating1 = null;
            double rating2 = 0;
            string rating3 = null;
            int score = GetScore();
            GetRating(score, ref rating1, ref rating2, ref rating3);
            ICollection<long> answersId = new List<long>();
            foreach (var q in QuestionsCollection)
            {
                answersId.AddRange(q.GetIdAnwerStudentCollection());
            }
            TestResult result = new TestResult()
            {
                Duration = _durationTest,
                IdStudent = _student.Id,
                IdTest = _test.Id,
                TestDate = DateTime.Now,
                TotalScore = score,
                Rating1 = rating1,
                Rating2 = rating2,
                Rating3 = rating3
            };
            SaveResult(result, answersId);
            return result;
        }

        private int GetScore()
        {
            double summScore = 0;
            double maxScore = 0;
            foreach (var q in QuestionsCollection)
            {
                summScore += q.GetTotalScoreForAnswer();
                maxScore += q.GetMaxScore();
            }
            return (int) (summScore * 100.0 / maxScore);
        }

        private void SaveResult(TestResult result, ICollection<long> answersId)
        {
            foreach (var l in answersId)
            {
                result.TestResultAnswerTables.Add(new TestResultAnswerTable() {IdAnswer = l});
            }
            var testResultRepository = _unitOfWork.GetRepository<ITestResultRepository>();
            testResultRepository.Add(result);
            _unitOfWork.SaveChanges();
        }

        private bool IsAllAnswerFixed()
        {
            return QuestionsCollection.All(q => q.IsAnswerFixed);
        }

        protected override async Task<bool> CancelAsync()
        {
            var result = await _messageService.ShowAsync(
                "Вы ответили ещё не на все вопросы. Всё равно завершить тестирование?",
                "Внимание",
                MessageButton.YesNo, MessageImage.Exclamation);
            if (result != MessageResult.Yes)
                return false;
            Finish();
            return await base.CancelAsync();
        }

        protected override async Task OnClosingAsync()
        {
            if (!IsFinished)
            {
                FixResult();
            }
            base.OnClosingAsync();
        }

        private void GetRating(int score, ref string rating1, ref double rating2, ref string rating3)
        {
            if (score >= 95)
            {
                rating1 = "A";
                rating2 = 4.0;
                rating3 = "Отлично";
            }
            else if (score >= 90)
            {
                rating1 = "A-";
                rating2 = 3.67;
                rating3 = "Отлично";
            }
            else if (score >= 85)
            {
                rating1 = "B+";
                rating2 = 3.33;
                rating3 = "Хорошо";
            }
            else if (score >= 80)
            {
                rating1 = "B";
                rating2 = 3.0;
                rating3 = "Хорошо";
            }
            else if (score >= 75)
            {
                rating1 = "B-";
                rating2 = 2.67;
                rating3 = "Хорошо";
            }
            else if (score >= 70)
            {
                rating1 = "C+";
                rating2 = 2.33;
                rating3 = "Хорошо";
            }
            else if (score >= 65)
            {
                rating1 = "C";
                rating2 = 2.0;
                rating3 = "Удовлетворительно";
            }
            else if (score >= 60)
            {
                rating1 = "C-";
                rating2 = 1.67;
                rating3 = "Удовлетворительно";
            }
            else if (score >= 55)
            {
                rating1 = "D+";
                rating2 = 1.33;
                rating3 = "Удовлетворительно";
            }
            else if (score >= 50)
            {
                rating1 = "D";
                rating2 = 1.0;
                rating3 = "Удовлетворительно";
            }
            else
            {
                rating1 = "F";
                rating2 = 0.0;
                rating3 = "Неудовлетворительно";
            }
        }
        #endregion
    }
}
