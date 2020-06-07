using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SystemForTesting.Annotations;

namespace SystemForTesting.Data
{
    public class TreeNode<T>
    {
        public T Value { get; set; }
        public TreeNode<T> Parent { get; private set; }
        public object Tag { get; set; }

        private readonly List<TreeNode<T>> _children = new List<TreeNode<T>>();
        public IReadOnlyCollection<TreeNode<T>> ChildNodes
        {
            get { return _children.AsReadOnly(); }
        }

        public TreeNode(T value)
        {
            Value = value;
        }

        public void AppendChild(TreeNode<T> child)
        {
            child.Parent = this;
            _children.Add(child);
        }

        public TreeNode<T> AppendChild(T value)
        {
            var child = new TreeNode<T>(value) { Parent = this };
            _children.Add(child);
            return child;
        }

        public TreeNode<T>[] AppendChild(params T[] values)
        {
            return values.Select(AppendChild).ToArray();
        }

        public void Execute(Action<TreeNode<T>> action)
        {
            action(this);
            foreach (var child in _children)
            {
                child.Execute(action);
            }
        }
    }

    /// <summary>
    /// Используется для хранения данных, привязанных к DevExpress TreeListView
    /// </summary>
    public class Node : INotifyPropertyChanged
    {
        public object Value { get; set; }

        private string _displayValue;
        public string DisplayValue
        {
            get { return _displayValue; }
            set
            {
                _displayValue = value;
                OnPropertyChanged(nameof(DisplayValue));
            }
        }

        public int Id { get; set; }
        public int ParentId { get; set; }
        public bool Checked { get; set; } = true;
        public bool Visible { get; set; } = true;
        public object Tag { get; set; }

        public Node(object value, string displayValue)
        {
            Value = value;
            DisplayValue = displayValue;
        }

        public Node() { }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
