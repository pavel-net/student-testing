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
    public class TestFinishViewModel : ViewModelBase
	{
		#region Поля
        //private Student _student;
        //private TestResult _testResult;
		#endregion

		#region Конструкторы
        public TestFinishViewModel(string nameTest, Student student, TestResult testResult)
        {
            Student = student;
            Result = testResult;
            if (Result.Rating2 == 0.0)
            {
                IsRecommendCommentVisible = true;
                RecommendComment = "Изучить тему повторно и обратиться за помощью к преподавателю";
            }
			else if (Result.Rating2 <= 2.0)
            {
                IsRecommendCommentVisible = true;
                RecommendComment = "Повторить тему и обратиться за помощью к преподавателю";
			}
            Fio = Student.Surname + " " + Student.Name + " " + Student.MiddleName;
            NameTest = nameTest;
            Rating = Result.TotalScore + ", " + Result.Rating1 + ", " + Result.Rating2 + ", " + Result.Rating3;
        }
		#endregion

		#region Свойства

		public override string Title => "Результаты тестирования";
        public string Fio { get; set; }
        public string NameTest { get; set; }
        public string Rating { get; set; }

		public TestResult Result
		{
			get { return GetValue<TestResult>(ResultProperty); }
			set { SetValue(ResultProperty, value); }
		}
		public static readonly PropertyData ResultProperty =
			RegisterProperty(nameof(Result), typeof(TestResult), null);

		public Student Student
		{
			get { return GetValue<Student>(StudentProperty); }
			set { SetValue(StudentProperty, value); }
		}
		public static readonly PropertyData StudentProperty =
			RegisterProperty(nameof(Student), typeof(Student), null);

        public bool IsRecommendCommentVisible
		{
			get { return GetValue<bool>(IsRecommendCommentVisibleProperty); }
			set { SetValue(IsRecommendCommentVisibleProperty, value); }
		}
		public static readonly PropertyData IsRecommendCommentVisibleProperty =
			RegisterProperty(nameof(IsRecommendCommentVisible), typeof(bool), false);

		public string RecommendComment
		{
			get { return GetValue<string>(RecommendCommentProperty); }
			set { SetValue(RecommendCommentProperty, value); }
		}
		public static readonly PropertyData RecommendCommentProperty =
			RegisterProperty(nameof(RecommendComment), typeof(string), null);

		#endregion

		#region Commands

		#endregion

		#region Methods
        protected override async Task OnClosingAsync()
        {
            base.OnClosingAsync();
            var _uiVisualizerService = this.GetDependencyResolver().Resolve<IUIVisualizerService>();
            var viewModel = new StudentMainViewModel(Student, _uiVisualizerService,
                this.GetDependencyResolver().Resolve<IMessageService>());
            _uiVisualizerService.ShowAsync(viewModel);
		}
		#endregion
	}
}
