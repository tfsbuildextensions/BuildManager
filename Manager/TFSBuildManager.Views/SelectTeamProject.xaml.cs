//-----------------------------------------------------------------------
// <copyright file="SelectTeamProject.xaml.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using TfsBuildManager.Views.ViewModels;

    /// <summary>
    /// Interaction logic for SelectTeamProject
    /// </summary>
    public partial class SelectTeamProject
    {
        private readonly TeamProjectListViewModel model;

        public SelectTeamProject(TeamProjectListViewModel model, string selectedTeamProject)
        {
            this.model = model;
            this.InitializeComponent();

            this.Grid1.DataContext = this.model;
            if (selectedTeamProject != null)
            {
                var project = model.TeamProjects.FirstOrDefault(tp => tp.Name == selectedTeamProject);
                if (project != null)
                {
                    this.BuildControllerList.SelectedValue = project;
                }
            }
        }

        public IEnumerable<TeamProjectViewModel> SelectedTeamProjects { get; set; }

        public bool SetAsDefault { get; set; }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            if (this.BuildControllerList.SelectedItems != null && this.BuildControllerList.SelectedItems.Count > 0)
            {
                var selectedProjects = (from object p in this.BuildControllerList.SelectedItems select p as TeamProjectViewModel).ToList();
                this.SelectedTeamProjects = selectedProjects;
                this.SetAsDefault = this.cbSetAsDefault.IsChecked.HasValue && this.cbSetAsDefault.IsChecked.Value;
                this.DialogResult = true;
                this.Close();
            }
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
