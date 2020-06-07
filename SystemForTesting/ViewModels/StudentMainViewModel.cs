using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemForTesting.Data;
using SystemForTesting.Models;
using SystemForTesting.Repositories;
using SystemForTesting.Views;
using Catel.Collections;
using Catel.Data;
using Catel.IoC;
using Catel.MVVM;
using Catel.MVVM.Views;
using Catel.Services;

namespace SystemForTesting.ViewModels
{
    public class StudentMainViewModel : ViewModelBase
	{
		#region Поля
        protected TreeNode<Node> tree;
        private readonly IUIVisualizerService _uiVisualizer;
        private readonly IMessageService _messageService;
        private IUnitOfWork _unitOfWork;
        private ITopicRepository _topicRepository;
        private ITestRepository _testRepository;
        private List<Discipline> disciplinesList;
        private List<Topic> topicsList;
        private int _id;
        private Node node;
        private Discipline selectedDiscipline;
        private Topic selectedTopic;
        private List<Test> _testList;
        private Student _student;
        private IViewManager _viewManager;
        private bool IsTestStartFlag = false;
        #endregion

        #region Конструкторы

        public StudentMainViewModel(Student student, IUIVisualizerService uiVisualizer, IMessageService messageService)
        {
            _student = student;
            _uiVisualizer = uiVisualizer;
            _messageService = messageService;
            BottomHeader = _student.Surname + " " + student.Name + " " + student.MiddleName;
            _unitOfWork = this.GetDependencyResolver().Resolve<IUnitOfWork>();
            _topicRepository = _unitOfWork.GetRepository<ITopicRepository>();
            _testRepository = _unitOfWork.GetRepository<ITestRepository>();
            _viewManager = ServiceLocator.Default.ResolveType<IViewManager>();
        }
		#endregion

		#region Свойства
        public override string Title => "Главное окно";
        public string BottomHeader { get; set; }
        public FastObservableCollection<Node> ListNodes { get; set; } = new FastObservableCollection<Node>();

        public object SelectedNode
        {
            get { return GetValue<object>(SelectedNodeProperty); }
            set
            {
                SetValue(SelectedNodeProperty, value);
                SelectedNodeChange();
            }
        }
        public static readonly PropertyData SelectedNodeProperty =
            RegisterProperty(nameof(SelectedNode), typeof(object), null);

        public string SelectedName
        {
            get { return GetValue<string>(SelectedNameProperty); }
            set { SetValue(SelectedNameProperty, value); }
        }
        public static readonly PropertyData SelectedNameProperty =
            RegisterProperty(nameof(SelectedName), typeof(string), null);

        public string SelectedTitle
        {
            get { return GetValue<string>(SelectedTitleProperty); }
            set { SetValue(SelectedTitleProperty, value); }
        }
        public static readonly PropertyData SelectedTitleProperty =
            RegisterProperty(nameof(SelectedTitle), typeof(string), "Дисциплина");

        public bool IsSelectedDiscipline
        {
            get { return GetValue<bool>(IsSelectedDisciplineProperty); }
            set { SetValue(IsSelectedDisciplineProperty, value); }
        }
        public static readonly PropertyData IsSelectedDisciplineProperty =
            RegisterProperty(nameof(IsSelectedDiscipline), typeof(bool), false);
        #endregion


        #region Commands
        private Command _closeCommand;
        public Command CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new Command(async () =>
                {
                    bool result = await SaveViewModelAsync();
                    if (result)
                        await CloseViewModelAsync(true);
                }));
            }
        }

        private Command _startTestCommand;
        public Command StartTestCommand
        {
            get
            {
                return _startTestCommand ?? (_startTestCommand = new Command(() =>
                {
                    Test test = getTest();
                    if (test == null)
                    {
                        _messageService.ShowAsync("Указанного теста в системе не найдено.", "Ошибка", MessageButton.OK,
                            MessageImage.Information);
                        return;
                    }
                    List<int> idTopics = test.IdDiscipline.HasValue
                        ? _topicRepository.GetTopicListId(test.IdDiscipline.Value)
                        : new List<int>() { test.IdTopic.Value };
                    var testStartViewModel = new TestStartViewModel(this, test, _student,
                        idTopics, _unitOfWork, _uiVisualizer, _messageService);
                    _uiVisualizer.ShowAsync(testStartViewModel);
                }));
            }
        }
        #endregion


        #region Methods

        private Test getTestForDiscipline(int id)
        {
            return _testList.FirstOrDefault(t => t.IdDiscipline == id);
        }

        private Test getTestForTopic(int id)
        {
            return _testList.FirstOrDefault(t => t.IdTopic == id);
        }

        private Test getTest()
        {
            if (SelectedNode == null || (selectedDiscipline == null && selectedTopic == null))
                return null;
            Test test = null;
            test = IsSelectedDiscipline
                ? getTestForDiscipline(selectedDiscipline.Id)
                : getTestForTopic(selectedTopic.Id);
            return test;
        }

        private void SelectedNodeChange()
        {
            if (SelectedNode == null)
                return;
            node = SelectedNode as Node;
            IsSelectedDiscipline = node.Tag.ToString() == "discipline";
            if (IsSelectedDiscipline)
            {
                selectedDiscipline = node.Value as Discipline;
                selectedTopic = null;
            }
            else
            {
                selectedTopic = node.Value as Topic;
                selectedDiscipline = null;
            }
            SelectedName = IsSelectedDiscipline ? selectedDiscipline.Name : selectedTopic.Name;
            SelectedTitle = IsSelectedDiscipline ? "Дисциплина" : "Тема";
        }

        protected override async Task InitializeAsync()
        {
            var viewParent = _viewManager.ActiveViews.FirstOrDefault(v => v is MainWindow);
            if (viewParent != null)
                viewParent.IsEnabled = false;
            _id = 0;
            InitializeData();
            InitializeTree();
            await base.InitializeAsync();
        }

        private void InitializeData()
        {
            IDisciplineRepository disciplineRepository = _unitOfWork.GetRepository<IDisciplineRepository>();
            disciplinesList = new List<Discipline>(disciplineRepository.GetDisciplines().OrderBy(d => d.Name));
            topicsList = new List<Topic>(_topicRepository.GetTopics());
            _testList = new List<Test>(_testRepository.GetTests());
        }

        protected void InitializeTree()
        {
            tree = new TreeNode<Node>(null) { Tag = "root" };

            foreach (var discipline in disciplinesList)
            {
                Node node = new Node(discipline, discipline.Name) { Id = ++_id, ParentId = 0, Tag = "discipline" };
                ListNodes.Add(node);
                int currentId = _id;
                foreach (var topic in topicsList.Where(t => t.IdDiscipline == discipline.Id).OrderBy(t => t.Name))
                {
                    Node nodeTopic = new Node(topic, topic.Name) { Id = ++_id, ParentId = currentId, Tag = "topic" };
                    ListNodes.Add(nodeTopic);
                }
            }
            ConvertListNodesInTreeStruct();
        }

        private void ConvertListNodesInTreeStruct()
        {
            var dictTreeNodes = new Dictionary<int, TreeNode<Node>> { { 0, tree } };
            foreach (var node in ListNodes.OrderBy(n => n.ParentId))
            {
                var treeNode = dictTreeNodes[node.ParentId].AppendChild(node);
                treeNode.Tag = node.Value;
                dictTreeNodes.Add(node.Id, treeNode);
            }
        }

        public async void CloseFromTest()
        {
            IsTestStartFlag = true;
            await this.CloseViewModelAsync(true);
        }

        protected override async Task OnClosingAsync()
        {
            if (!IsTestStartFlag)
            {
                var viewParent = _viewManager.ActiveViews.FirstOrDefault(v => v is MainWindow);
                if (viewParent != null)
                    viewParent.IsEnabled = true;
            }
            base.OnClosingAsync();
        }

        #endregion
    }
}
