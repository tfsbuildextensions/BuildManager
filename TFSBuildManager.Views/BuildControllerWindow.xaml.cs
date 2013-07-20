//-----------------------------------------------------------------------
// <copyright file="BuildControllerWindow.xaml.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System.Windows;
    using TfsBuildManager.Views.ViewModels;

    /// <summary>
    /// Interaction logic for BuildControllerWindow
    /// </summary>
    public partial class BuildControllerWindow
    {
        private readonly BuildControllerListViewModel model;

        public BuildControllerWindow(BuildControllerListViewModel model)
        {
            this.model = model;
            this.InitializeComponent();

            this.Grid1.DataContext = this.model;
        }

        public BuildControllerViewModel SelectedBuildController { get; set; }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            if (this.BuildControllerList.SelectedItem != null)
            {
                this.SelectedBuildController = this.BuildControllerList.SelectedItem as BuildControllerViewModel;
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
