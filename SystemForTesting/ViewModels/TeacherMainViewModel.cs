using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Catel.Collections;
using Catel.MVVM;
using SystemForTesting.Data;
using SystemForTesting.Repositories;
using SystemForTesting.Models;
using Catel.Data;
using Catel.IoC;
using Catel.MVVM.Views;
using Catel.Services;
using DevExpress.Xpf.Core;

namespace SystemForTesting.ViewModels
{
    public class TeacherMainViewModel : ViewModelBase
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
        private Teacher _teacher;
        private IViewManager _viewManager;
        #endregion

        #region Конструкторы
        public TeacherMainViewModel(Teacher teacher, IUIVisualizerService uiVisualizer, IMessageService messageService)
        {
            _teacher = teacher;
            _uiVisualizer = uiVisualizer;
            _messageService = messageService;
            BottomHeader = teacher.Fio;
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


        private Command _addNewTopicCommand;
        public Command AddNewTopicCommand
        {
            get
            {
                return _addNewTopicCommand ?? (_addNewTopicCommand = new Command(async () =>
                {
                    var simpleEditViewModel = new SimpleEditViewModel("Новая тема", "Название темы");
                    await _uiVisualizer.ShowDialogAsync(simpleEditViewModel, async (sender, e) =>
                    {
                        if (e.Result ?? false)
                        {
                            Topic topic = new Topic()
                            {
                                IdDiscipline = selectedDiscipline.Id,
                                Name = simpleEditViewModel.TextValue.Trim()
                            };
                            _topicRepository.Add(topic);
                            Node nodeTopic = new Node(topic, topic.Name) { Id = ++_id, ParentId = node.Id, Tag = "topic" };
                            ListNodes.Add(nodeTopic);
                            //RaisePropertyChanged(nameof(ListNodes));
                            await _unitOfWork.SaveChangesAsync();
                        }
                    });
                }));
            }
        }

        private Command _openTestCommand;
        public Command OpenTestCommand
        {
            get
            {
                return _openTestCommand ?? (_openTestCommand = new Command(() =>
                {
                    Test test = null;
                    if (IsSelectedDiscipline)
                    {
                        test = _testList.FirstOrDefault(t => t.IdDiscipline == selectedDiscipline.Id);
                    }
                    else
                    {
                        test = _testList.FirstOrDefault(t => t.IdTopic == selectedTopic.Id);
                    }
                    bool flagNew = test == null;
                    var viewModel = new TestViewModel(_uiVisualizer, _messageService, selectedTopic, selectedDiscipline, test);
                    _uiVisualizer.ShowDialogAsync(viewModel, (sender, args) =>
                    {
                        if (args.Result ?? false)
                        {
                            if (flagNew)
                            {
                                test = viewModel.TestObject;
                                _testRepository.Add(test);
                                _testList.Add(test);
                                if (IsSelectedDiscipline)
                                    test.IdDiscipline = selectedDiscipline.Id;
                                else
                                    test.IdTopic = selectedTopic.Id;
                            }
                            _unitOfWork.SaveChangesAsync();
                        }
                    });
                }));
            }
        }

        private Command _deleteTopicCommand;
        public Command DeleteTopicCommand
        {
            get
            {
                return _deleteTopicCommand ?? (_deleteTopicCommand = new Command(async () =>
                {
                    var result = await _messageService.ShowAsync(
                        "Вы уверены, что хотите удалить выбранную тему '" + selectedTopic.Name +
                        "'. Вместе с ней будут также удалены все созданные вопросы и тесты по теме.", "Внимание",
                        MessageButton.YesNo, MessageImage.Warning);
                    if (result != MessageResult.Yes)
                        return;
                    var topic = selectedTopic;
                    ListNodes.Remove(node);
                    await DeleteTopic(topic);
                }, () => selectedTopic != null));
            }
        }

        private Command _editTopicCommand;
        public Command EditTopicCommand
        {
            get
            {
                return _editTopicCommand ?? (_editTopicCommand = new Command(async () =>
                {
                    var viewModel = new TopicViewModel(_uiVisualizer, _messageService, selectedTopic);
                    await _uiVisualizer.ShowDialogAsync(viewModel, async (sender, e) =>
                    {
                        if (e.Result ?? false)
                        {
                            node.DisplayValue = selectedTopic.Name;
                            SelectedName = selectedTopic.Name;
                        }
                    });
                }, () => selectedTopic != null));
            }
        }


        #endregion

        #region Methods

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

        private async Task DeleteTopic(Topic topic)
        {
            _topicRepository.Delete(topic);
            await _unitOfWork.SaveChangesAsync();
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
                Node node = new Node(discipline, discipline.Name) {Id = ++_id, ParentId = 0, Tag = "discipline"};
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

        protected override async Task OnClosingAsync()
        {

            var viewParent = _viewManager.ActiveViews.FirstOrDefault(v => v is MainWindow);
            if (viewParent != null)
                viewParent.IsEnabled = true;
            base.OnClosingAsync();
        }

        #endregion
    }
}
