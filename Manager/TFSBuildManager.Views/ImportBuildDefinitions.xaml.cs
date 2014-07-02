//-----------------------------------------------------------------------
// <copyright file="ImportBuildDefinitions.xaml.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System;
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
                try
                {
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

                        newBuildDefinition.RetentionPolicyList.Clear();
                        foreach (var ret in exdef.RetentionPolicyList)
                        {
                            newBuildDefinition.AddRetentionPolicy(ret.BuildReason, ret.BuildStatus, ret.NumberToKeep, ret.DeleteOptions);
                        }

                        foreach (var sp in exdef.SourceProviders)
                        {
                            var provider = newBuildDefinition.CreateInitialSourceProvider(sp.Name);
                            if (exdef.SourceProviders.All(s => s.Name == "TFGIT"))
                            {
                                provider.Fields["RepositoryName"] = sp.Fields["RepositoryName"];
                                provider.Fields["DefaultBranch"] = sp.Fields["DefaultBranch"];
                                provider.Fields["CIBranches"] = sp.Fields["CIBranches"];
                                provider.Fields["RepositoryUrl"] = sp.Fields["RepositoryUrl"];
                            }

                            newBuildDefinition.SetSourceProvider(provider);
                        }

                        newBuildDefinition.BuildController = this.buildServer.GetBuildController(exdef.BuildController);
                        var x = this.buildServer.QueryProcessTemplates(this.lableTeamProject.Content.ToString());
                        if (x.All(p => p.ServerPath != exdef.ProcessTemplate))
                        {
                            bi.Status = "Failed";
                            bi.Message = "ProcessTemplate not found";
                            break;
                        }

                        newBuildDefinition.Process = this.buildServer.QueryProcessTemplates(this.lableTeamProject.Content.ToString()).First(p => p.ServerPath == exdef.ProcessTemplate);
                        newBuildDefinition.DefaultDropLocation = exdef.DefaultDropLocation;
                        foreach (var sched in exdef.Schedules)
                        {
                            var newSched = newBuildDefinition.AddSchedule();
                            newSched.DaysToBuild = sched.DaysToBuild;
                            newSched.StartTime = sched.StartTime;
                            newSched.TimeZone = sched.TimeZone;
                        }

                        var process = WorkflowHelpers.DeserializeProcessParameters(newBuildDefinition.ProcessParameters);

                        foreach (var param in exdef.ProcessParameters)
                        {
                            switch (param.Key)
                            {
                                case "ProjectsToBuild":
                                    break;
                                case "ConfigurationsToBuild":
                                    break;
                                case "AdvancedBuildSettings":
                                    break;
                                case "RunSettingsForTestRun":
                                    break;
                                default:
                                    process.Add(param.Key, param.Value);
                                    break;
                            }
                        }

                        // process.Add("ProjectsToBuild", new[] { "Test.sln" });
                        // process.Add("ConfigurationsToBuild", new[] { "Mixed Platforms|Debug" });

                        //////Advanced build settings
                        ////var buildParams = new Dictionary<string, string>();
                        ////buildParams.Add("PreActionScriptPath", "/prebuild.ps1");
                        ////buildParams.Add("PostActionScriptPath", "/postbuild.ps1");
                        ////var param = new BuildParameter(buildParams);
                        ////process.Add("AdvancedBuildSettings", param);

                        //////test settings
                        ////var testParams = new Dictionary<string, object>
                        ////             {
                        ////                 { "AssemblyFileSpec", "*.exe" },
                        ////                 { "HasRunSettingsFile", true },
                        ////                 { "ExecutionPlatform", "X86" },
                        ////                 { "FailBuildOnFailure", true },
                        ////                 { "RunName", "MyTestRunName" },
                        ////                 { "HasTestCaseFilter", false },
                        ////                 { "TestCaseFilter", null }
                        ////             };

                        ////var runSettingsForTestRun = new Dictionary<string, object>
                        ////                        {
                        ////                            { "HasRunSettingsFile", true },
                        ////                            { "ServerRunSettingsFile", "" },
                        ////                            { "TypeRunSettings", "CodeCoverageEnabled" }
                        ////                        };
                        ////testParams.Add("RunSettingsForTestRun", runSettingsForTestRun);
                        ////process.Add("AutomatedTests", new[] { new BuildParameter(testParams) });
                        ////process.Add("SymbolStorePath", @"\\server\symbols\somepath");

                        newBuildDefinition.ProcessParameters = WorkflowHelpers.SerializeProcessParameters(process);
                        newBuildDefinition.Save();
                    }
                }
                catch (Exception ex)
                {
                    bi.Status = "Failed";
                    bi.Message = ex.Message;
                }
                finally
                {
                    this.DataGridBuildsToImport.ItemsSource = null;
                    this.DataGridBuildsToImport.ItemsSource = this.buildFiles;
                }
            }
        }
    }
}
