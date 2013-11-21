//-----------------------------------------------------------------------
// <copyright file="RemapWorkspaces.xaml.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System;
    using System.Windows;
    using TfsBuildManager.Views.ViewModels;

    /// <summary>
    /// Interaction logic for RemapWorkspaces.xaml
    /// </summary>
    public partial class RemapWorkspaces : Window
    {
        private RemapWorkspacesViewModel viewModel;

        public RemapWorkspaces(RemapWorkspacesViewModel viewModel)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.DataContext = viewModel;
        }

        private static void DisplayError(Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Community TFS Build Manager", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.viewModel.Save();
                this.Close();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ReplacementText_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.viewModel.ReplacementText = this.ReplacementText.Text;
            this.viewModel.RemapWorkspaces();
            this.WorkspacesList.Items.Refresh();
        }
    }
}
