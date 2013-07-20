//-----------------------------------------------------------------------
// <copyright file="TargetBranchViewModel.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using TfsBuildManager.Repository;

    public class TargetBranchViewModel : ViewModelBase
    {
        private readonly string originalName;
        private string newName;
        private TargetBranch selectedBranch;

        public TargetBranchViewModel(string originalName, IEnumerable<Branch> targets)
        {
            this.originalName = originalName;
            this.TargetBranches = new ObservableCollection<TargetBranch>();
            foreach (var t in targets)
            {
                this.TargetBranches.Add(new TargetBranch { Branch = t, Path = t.ServerPath });
            }
        }

        public ObservableCollection<TargetBranch> TargetBranches { get; private set; }
        
        public string NewName
        {
            get
            {
                return this.newName;
            }

            set
            {
                this.newName = value;
                this.NotifyPropertyChanged("NewName");
                this.NotifyPropertyChanged("IsEnabled");
            }
        }

        public TargetBranch SelectedBranch 
        {
            get
            {
                return this.selectedBranch;
            }

            set 
            {
                var old = this.selectedBranch;
                this.selectedBranch = value;
                this.NewName = this.originalName + "." + Path.GetFileName(value.Path);
                this.NotifyPropertyChanged("NewName");
                if (value != old)
                {
                    this.NotifyPropertyChanged("IsEnabled");
                }
            }
        }

        public bool IsEnabled 
        {
            get
            {
                return this.SelectedBranch != null && !string.IsNullOrEmpty(this.NewName);
            }
        }
    }

    public class TargetBranch : ViewModelBase
    {
        public Branch Branch { get; set; }

        public string Path { get; set; }
    }
}
