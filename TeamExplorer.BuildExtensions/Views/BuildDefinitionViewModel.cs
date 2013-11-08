using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BuildTree.Models;
using BuildTree.Properties;
using Microsoft.TeamFoundation.Build.Client;

namespace BuildTree.Views
{
    public class BuildDefinitionViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<BuildDefinitionViewModel> Children { get; set; }
        public BuildDefinitionViewModel Parent { get; set; }
        public string Name { get; set; }
        public bool IsBuildNode
        {
            get { return Definition != null; }
        }
        public IBuildDefinition Definition { get; set; }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }

                if (_isExpanded && Parent != null)
                    Parent.IsExpanded = true;
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public BuildDefinitionViewModel(BuildDefinitionTreeNode node)
        {
            Name = node.Name;
            Definition = node.BuildDefinition;
            Children = new ObservableCollection<BuildDefinitionViewModel>();

            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    Children.Add(new BuildDefinitionViewModel(child, this));
                }
            }
        }
        
        private BuildDefinitionViewModel(BuildDefinitionTreeNode node, BuildDefinitionViewModel parent) : this(node)
        {
            Parent = parent;
        }
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
