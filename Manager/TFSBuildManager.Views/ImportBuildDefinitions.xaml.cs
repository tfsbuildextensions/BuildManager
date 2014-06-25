//-----------------------------------------------------------------------
// <copyright file="ImportBuildDefinitions.xaml.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using Microsoft.TeamFoundation.Build.Client;
    using Microsoft.TeamFoundation.Build.Workflow;
    using Newtonsoft.Json;

    /// <summary>
    /// Interaction logic for ImportBuildDefinitions
    /// </summary>
    public partial class ImportBuildDefinitions
    {
        private readonly ObservableCollection<BuildImport> buildFiles = new ObservableCollection<BuildImport>();
        private readonly IBuildServer buildServer;

        public ImportBuildDefinitions(string teamProjectName, IBuildServer server)
        {
            this.InitializeComponent();
            this.lableTeamProject.Content = teamProjectName;
            this.buildServer = server;
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
                if (bi == null)
                {
                    return;
                }

                if (!File.Exists(bi.JsonFile))
                {
                    bi.Status = "Failed";
                    bi.Message = "File not found";
                }
                else
                {
                    ExportedBuildDefinition exdef = JsonConvert.DeserializeObject<ExportedBuildDefinition>(File.ReadAllText(bi.JsonFile));
                    var newBuildDefinition = this.buildServer.CreateBuildDefinition(this.lableTeamProject.Content.ToString());
                    newBuildDefinition.Name = exdef.Name;
                    newBuildDefinition.Description = exdef.Description;
                    newBuildDefinition.ContinuousIntegrationType = exdef.ContinuousIntegrationType;
                    newBuildDefinition.ContinuousIntegrationQuietPeriod = exdef.ContinuousIntegrationQuietPeriod;

                    newBuildDefinition.QueueStatus = exdef.QueueStatus;
                    if (exdef.SourceProviders.All(s => s.Name != "TFGIT"))
                    {
                        foreach (var mapping in exdef.Mappings)
                        {
                            newBuildDefinition.Workspace.AddMapping(mapping.ServerItem, mapping.LocalItem, mapping.MappingType);
                        }
                    }

                    foreach (var ret in exdef.RetentionPolicyList)
                    {
                        newBuildDefinition.RetentionPolicyList.Add(ret);
                    }

                    foreach (var sp in exdef.SourceProviders)
                    {
                        newBuildDefinition.CreateInitialSourceProvider(sp.Name);
                    }

                    newBuildDefinition.BuildController = this.buildServer.GetBuildController(exdef.BuildController);
                    newBuildDefinition.Process = this.buildServer.QueryProcessTemplates(this.lableTeamProject.Content.ToString()).First(p => p.ServerPath == exdef.ProcessTemplate);
                    newBuildDefinition.DefaultDropLocation = exdef.DefaultDropLocation;
                    foreach (var sched in exdef.Schedules)
                    {
                        var newSched = newBuildDefinition.AddSchedule();
                        newSched.DaysToBuild = sched.DaysToBuild;
                        newSched.StartTime = sched.StartTime;
                        newSched.TimeZone = sched.TimeZone;
                    }

                    newBuildDefinition.ProcessParameters = WorkflowHelpers.SerializeProcessParameters(exdef.ProcessParameters);
                    newBuildDefinition.Save();
                }
            }

            this.DataGridBuildsToImport.ItemsSource = null;
            this.DataGridBuildsToImport.ItemsSource = this.buildFiles;
        }
    }
}
