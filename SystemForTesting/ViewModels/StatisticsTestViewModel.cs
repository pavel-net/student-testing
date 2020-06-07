using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemForTesting.Models;
using SystemForTesting.Repositories;
using SystemForTesting.Views;
using Catel.Collections;
using Catel.Data;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;

namespace SystemForTesting.ViewModels
{
    public class StatisticsTestViewModel : ViewModelBase
	{
        #region Поля
        private IUnitOfWork _unitOfWork;
        private Test _test;
        //private IUIVisualizerService _uiVisualizer;
        #endregion

        #region Конструкторы
        public StatisticsTestViewModel(Test test, IUnitOfWork unitOfWork)
        {
            _test = test;
            _unitOfWork = unitOfWork;
            //_uiVisualizer = uiVisualizer;
            NameTest = test.Name;
        }
		#endregion

		#region Свойства
        public override string Title => "Результаты теста";
        public string NameTest { get; set; }

        public FastObservableCollection<TestResult> TestResults
        {
            get { return GetValue<FastObservableCollection<TestResult>>(TestResultsProperty); }
            set { SetValue(TestResultsProperty, value); }
        }
        public static readonly PropertyData TestResultsProperty =
            RegisterProperty(nameof(TestResults), typeof(FastObservableCollection<TestResult>), new FastObservableCollection<TestResult>());

        #endregion

        #region Commands

        #endregion

        #region Methods
        protected override async Task InitializeAsync()
        {
            var resultsRepository = _unitOfWork.GetRepository<ITestResultRepository>();
            TestResults = new FastObservableCollection<TestResult>(resultsRepository.GetResults(_test.Id));
            await base.InitializeAsync();
        }

        //protected override async Task OnClosingAsync()
        //{
        //    base.OnClosingAsync();
        //}
		#endregion
	}
}
