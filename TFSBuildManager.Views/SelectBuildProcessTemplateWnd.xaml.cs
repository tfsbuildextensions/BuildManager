//-----------------------------------------------------------------------
// <copyright file="SelectBuildProcessTemplateWnd.xaml.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System.Windows;
    using TfsBuildManager.Views.ViewModels;

    /// <summary>
    /// Interaction logic for SelectBuildProcessTemplateWnd
    /// </summary>
    public partial class SelectBuildProcessTemplateWindow
    {
        private readonly BuildTemplateListViewModel model;

        public SelectBuildProcessTemplateWindow(BuildTemplateListViewModel model)
        {
            this.model = model;
            this.InitializeComponent();
            this.Grid1.DataContext = this.model;
        }

        public BuildTemplateViewModel SelectedBuildTemplate { get; set; }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            if (this.BuildTemplateList.SelectedItem != null)
            {
                this.SelectedBuildTemplate = this.BuildTemplateList.SelectedItem as BuildTemplateViewModel;
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
