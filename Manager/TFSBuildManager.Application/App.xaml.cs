//-----------------------------------------------------------------------
// <copyright file="App.xaml.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;

namespace TfsBuildManager.Application
{
    using System;
    using System.Windows;
    using Microsoft.TeamFoundation.Build.Client;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.Server;
    using TfsBuildManager.Repository;

    /// <summary>
    /// Interaction logic for App
    /// </summary>
    public partial class App
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Handling the exception within the UnhandledExcpeiton handler.
            MessageBox.Show(e.Exception.ToString(), "Community TFS Build Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }

    public class AppTfsContext : ITfsContext
    {
        private readonly TfsTeamProjectCollection collection;
        private readonly ProjectInfo selectedProject;

        public AppTfsContext(TfsTeamProjectCollection collection, ProjectInfo selectedProject)
        {
            this.collection = collection;
            this.selectedProject = selectedProject;
        }

        public event EventHandler ProjectChanged = delegate { };
        
        public string SelectedProject
        {
            get { return this.selectedProject.Name; }
        }

        public string ActiveConnection
        {
            get { return this.collection.Uri.ToString(); }
        }

        public void ShowBuild(Uri buildUri)
        {
            var buildUrl = string.Format("{0}?url={1}", buildUri, collection.Uri);
            Process.Start(buildUrl);
        }

        public void EditBuildDefinition(Uri buildDefinition)
        {
            ShowNotSupportedMessage();
        }

        public void ShowControllerManager()
        {            
           ShowNotSupportedMessage();
        }

        public void RemapWorkspaces(IBuildDefinition buildDefinition)
        {
            ShowNotSupportedMessage();
        }

        private static void ShowNotSupportedMessage()
        {
            MessageBox.Show("This feature is not supported when running the stand alone application.", "Visual Studio Extension Required", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
