//-----------------------------------------------------------------------
// <copyright file="ImportBuildDefinitions.xaml.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------

using System.IO;

namespace TfsBuildManager.Views
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for ImportBuildDefinitions
    /// </summary>
    public partial class ImportBuildDefinitions
    {
        public ImportBuildDefinitions(string teamProjectName)
        {
            this.InitializeComponent();
            this.lableTeamProject.Content = teamProjectName;
        }

        private void ButtonSelectFiles_OnClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".json";
            dlg.Filter = "json Files (*.json)|*.json";
            dlg.Multiselect = true;
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                foreach (string filename in dlg.FileNames)
                {
                    this.ListBoxBuilds.Items.Add(filename);
                }
            }
        }

        private void ButtonImport_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.ListBoxBuilds.Items)
            {
                if (File.Exists(item.ToString()))
                {
                    MessageBox.Show("Processing " + item);
                }
            }
        }
    }
}
