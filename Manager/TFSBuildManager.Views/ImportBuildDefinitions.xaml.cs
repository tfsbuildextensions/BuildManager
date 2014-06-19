//-----------------------------------------------------------------------
// <copyright file="ImportBuildDefinitions.xaml.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System.Collections.ObjectModel;
    using System.Windows;

    /// <summary>
    /// Interaction logic for ImportBuildDefinitions
    /// </summary>
    public partial class ImportBuildDefinitions
    {
        private readonly ObservableCollection<BuildImport> buildFiles = new ObservableCollection<BuildImport>();

        public ImportBuildDefinitions(string teamProjectName)
        {
            this.InitializeComponent();
            this.lableTeamProject.Content = teamProjectName;
        }
        
        public ObservableCollection<BuildImport> BuildFiles
        {
            get { return this.buildFiles; }
        }

        private void ButtonSelectFiles_OnClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog { DefaultExt = ".json", Filter = "json Files (*.json)|*.json", Multiselect = true };
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                foreach (string filename in dlg.FileNames)
                {
                    this.BuildFiles.Add(new BuildImport { JsonFile = filename });
                }

                this.DataGridBuildsToImport.ItemsSource = this.buildFiles;
            }
        }

        private void ButtonImport_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.DataGridBuildsToImport.Items)
            {
                BuildImport bi = item as BuildImport;

                if (bi != null)
                {
                    bi.Status = "Imported";
                }
            }

            this.DataGridBuildsToImport.ItemsSource = null;
            this.DataGridBuildsToImport.ItemsSource = this.buildFiles;
        }
    }
}
