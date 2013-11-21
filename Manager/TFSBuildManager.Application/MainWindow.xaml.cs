//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Application
{
    using System;
    using System.Linq;
    using System.Windows;
    using Microsoft.TeamFoundation.Client;
    using TfsBuildManager.Repository;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private static void DisplayError(Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Community TFS Build Manager", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            this.Close();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposable object passed")]
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (TeamProjectPicker tpp = new TeamProjectPicker(TeamProjectPickerMode.SingleProject, false))
                {
                    System.Windows.Forms.DialogResult result = tpp.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        var context = new AppTfsContext(tpp.SelectedTeamProjectCollection, tpp.SelectedProjects.First());
                        this.MainView.InitializeContext(context);
                        this.MainView.InitializeRepository(new TfsClientRepository(tpp.SelectedTeamProjectCollection));
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.DisplayError(ex);
            }
        }

        private void OnChangeConnection(object sender, RoutedEventArgs e)
        {
            this.OnLoaded(sender, e);
            this.MainView.Reload();
        }

        private void OnExit(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown(0);
        }
    }
}
