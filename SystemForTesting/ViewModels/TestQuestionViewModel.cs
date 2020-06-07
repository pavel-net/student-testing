using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemForTesting.Models;
using Catel.Collections;
using Catel.Data;
using Catel.MVVM;
using Catel.Services;

namespace SystemForTesting.ViewModels
{
    public class TestQuestionViewModel : ViewModelBase
	{
        #region Поля
        private readonly IUIVisualizerService _uiVisualizer;
        #endregion

        #region Конструкторы
        public TestQuestionViewModel(Question question, int numberQuestion, IUIVisualizerService uiVisualizer)
        {
            _uiVisualizer = uiVisualizer;
            QuestionObject = question;
            TitleQuestion = "Вопрос номер " + numberQuestion;
            NameQuestion = "Вопрос " + numberQuestion;
            if (QuestionObject.ContentImage == null)
            {
                MaxHeightQuestion = 300.0;
                //MaxHeightAnswers = 700.0;
            }
            else
            {
                MaxHeightQuestion = 150.0;
                //MaxHeightAnswers = 600.0;
            }
            InitialAnswers();
        }
        #endregion

        #region Свойства


        public Question QuestionObject
        {
            get { return GetValue<Question>(QuestionObjectProperty); }
            set { SetValue(QuestionObjectProperty, value); }
        }
        public static readonly PropertyData QuestionObjectProperty =
            RegisterProperty(nameof(QuestionObject), typeof(Question), null);
        public string TitleQuestion { get; set; }
        public string NameQuestion { get; set; }
        public FastObservableCollection<AnswerStudent> AnswersCollection { get; set; }

        public bool IsAnswerFixed
        {
            get { return GetValue<bool>(IsAnswerFixedProperty); }
            set { SetValue(IsAnswerFixedProperty, value); }
        }
        public static readonly PropertyData IsAnswerFixedProperty =
            RegisterProperty(nameof(IsAnswerFixed), typeof(bool), false);

        public AnswerStudent SelectedAnswer
        {
            get { return GetValue<AnswerStudent>(SelectedAnswerProperty); }
            set { SetValue(SelectedAnswerProperty, value); }
        }
        public static readonly PropertyData SelectedAnswerProperty =
            RegisterProperty(nameof(SelectedAnswer), typeof(AnswerStudent), null);

        public bool IsHintActivate
        {
            get { return GetValue<bool>(IsHintActivateProperty); }
            set { SetValue(IsHintActivateProperty, value); }
        }
        public static readonly PropertyData IsHintActivateProperty =
            RegisterProperty(nameof(IsHintActivate), typeof(bool), false);

        public bool IsHint2Activate
        {
            get { return GetValue<bool>(IsHint2ActivateProperty); }
            set { SetValue(IsHint2ActivateProperty, value); }
        }
        public static readonly PropertyData IsHint2ActivateProperty =
            RegisterProperty(nameof(IsHint2Activate), typeof(bool), false);

        public bool IsHint3Activate
        {
            get { return GetValue<bool>(IsHint3ActivateProperty); }
            set { SetValue(IsHint3ActivateProperty, value); }
        }
        public static readonly PropertyData IsHint3ActivateProperty =
            RegisterProperty(nameof(IsHint3Activate), typeof(bool), false);

        public double MaxHeightQuestion
        {
            get { return GetValue<double>(MaxHeightQuestionProperty); }
            set { SetValue(MaxHeightQuestionProperty, value); }
        }
        public static readonly PropertyData MaxHeightQuestionProperty =
            RegisterProperty(nameof(MaxHeightQuestion), typeof(double), 150.0);

        public double MaxHeightAnswers
        {
            get { return GetValue<double>(MaxHeightAnswersProperty); }
            set { SetValue(MaxHeightAnswersProperty, value); }
        }
        public static readonly PropertyData MaxHeightAnswersProperty =
            RegisterProperty(nameof(MaxHeightAnswers), typeof(double), 700.0);

        #endregion


        #region Commands

        private Command _openImageCommand;
        public Command OpenImageCommand
        {
            get
            {
                return _openImageCommand ?? (_openImageCommand = new Command(() =>
                {
                    var viewModel = new ImageViewModel(QuestionObject.ContentImage);
                    _uiVisualizer.ShowAsync(viewModel);
                }, () => QuestionObject.ContentImage != null));
            }
        }

        private Command _openImageAnswerCommand;
        public Command OpenImageAnswerCommand
        {
            get
            {
                return _openImageAnswerCommand ?? (_openImageAnswerCommand = new Command(() =>
                {
                    var viewModel = new ImageViewModel(SelectedAnswer.Answer.ContentImage);
                    _uiVisualizer.ShowAsync(viewModel);
                }, () => SelectedAnswer?.Answer.ContentImage != null));
            }
        }


        //private Command _cellValueChangedCommand;   
        //public Command CellValueChangedCommand
        //{
        //    get
        //    {
        //        return _cellValueChangedCommand ?? (_cellValueChangedCommand = new Command(() =>
        //        {
                    
        //        }));
        //    }
        //}


        #endregion

        #region Methods

        private void InitialAnswers()
        {
            var tempList = new List<AnswerStudent>(QuestionObject.Answers.Select(a => new AnswerStudent(a, ChangeFixedAnswer)));
            Random rnd = new Random();
            var answerStudentCollection = tempList.OrderBy(x => rnd.Next());
            int index = -1;
            //foreach (var answerStudent in answerStudentCollection)
            //{
            //    answerStudent.OrderIndex = ++index;
            //}
            AnswersCollection = new FastObservableCollection<AnswerStudent>(answerStudentCollection);
        }

        public void ChangeFixedAnswer()
        {
            IsAnswerFixed = AnswersCollection.Any(a => a.FlagSelected);
        }

        public void ActivateHint()
        {
            if (IsHaveHint() && !IsHintActivate)
            {
                IsHintActivate = true;
                return;
            }
            if (IsHaveHint2() && !IsHint2Activate)
            {
                IsHint2Activate = true;
                return;
            }
            if (IsHaveHint3() && !IsHint3Activate)
            {
                IsHint3Activate = true;
                return;
            }
        }

        public bool IsHaveHint()
        {
            return !string.IsNullOrEmpty(QuestionObject.Hint);
        }

        public bool IsHaveHint2()
        {
            return !string.IsNullOrEmpty(QuestionObject.Hint2);
        }

        public bool IsHaveHint3()
        {
            return !string.IsNullOrEmpty(QuestionObject.Hint3);
        }

        public double GetTotalScoreForAnswer()
        {
            // количество правильных ответов
            int countCorrectlyAnswer = AnswersCollection.Count(a => a.FlagSelected && a.Answer.FlagCorrectly == "Y");
            // количество неправильных ответов
            int countNotCorrectlyAnswer = AnswersCollection.Count(a => a.FlagSelected && a.Answer.FlagCorrectly == "N");
            // всего правильный ответов
            int countAnswers = QuestionObject.Answers.Count(a => a.FlagCorrectly == "Y");
            if (countAnswers == 0 || countNotCorrectlyAnswer >= countCorrectlyAnswer)
                return 0;
            // количество штрафных баллов
            int countMinusHint = 0;
            if (IsHint3Activate)
                countMinusHint = 3;
            else if (IsHint2Activate)
                countMinusHint = 2;
            else if (IsHintActivate)
                countMinusHint = 1;
            // балл за один правильный ответ
            double scoreAnswer = QuestionObject.Score / (double)countAnswers;
            return Math.Max((scoreAnswer * countCorrectlyAnswer) - countMinusHint, 0);
        }

        public double GetMaxScore()
        {
            return QuestionObject.Score;
        }

        public IEnumerable<long> GetIdAnwerStudentCollection()
        {
            return AnswersCollection.Where(a => a.FlagSelected).Select(a => a.Answer.Id);
        }
        #endregion
    }

    public class AnswerStudent : ModelBase
    {
        public AnswerStudent(Answer answer, Action flagSelectedChange)
        {
            Answer = answer;
            _flagSelectedChange = flagSelectedChange;
        }

        private readonly Action _flagSelectedChange;
        public Answer Answer { get; set; }
        //public int OrderIndex { get; set; }

        public bool FlagSelected
        {
            get { return GetValue<bool>(FlagSelectedProperty); }
            set
            {
                SetValue(FlagSelectedProperty, value);
                _flagSelectedChange();
            }
        }
        public static readonly PropertyData FlagSelectedProperty =
            RegisterProperty(nameof(FlagSelected), typeof(bool), false);
    }
}
