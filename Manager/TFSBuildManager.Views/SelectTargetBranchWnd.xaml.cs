//-----------------------------------------------------------------------
// <copyright file="SelectTargetBranchWnd.xaml.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System.Collections.Generic;
    using System.Windows;
    using TfsBuildManager.Repository;
    using TfsBuildManager.Views.ViewModels;

    /// <summary>
    /// Interaction logic for SelectTargetBranchWnd
    /// </summary>
    public partial class SelectTargetBranchWindow
    {
        private readonly string teamProject;
        private readonly ITfsClientRepository tfs;
        private readonly TargetBranchViewModel viewmodel;

        public SelectTargetBranchWindow(string originalName, IEnumerable<Branch> targets, string teamProject, ITfsClientRepository tfs)
        {
            this.teamProject = teamProject;
            this.tfs = tfs;
            this.InitializeComponent();

            this.viewmodel = new TargetBranchViewModel(originalName, targets);
            this.DataContext = this.viewmodel;
        }

        public Branch SelectedTargetBranch
        {
            get
            {
                return this.viewmodel.SelectedBranch.Branch;
            }
        }

        public string NewBuildDefinitionName
        {
            get
            {
                return this.viewmodel.NewName;
            }
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            if (this.tfs.GetBuildDefinition(this.teamProject, this.NewBuildDefinitionName) != null)
            {
                MessageBox.Show(this, "Build definition " + this.NewBuildDefinitionName + " already exists", "Clone to branch", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            this.DialogResult = true;
            this.Close();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
