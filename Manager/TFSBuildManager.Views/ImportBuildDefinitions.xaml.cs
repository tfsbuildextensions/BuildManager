//-----------------------------------------------------------------------
// <copyright file="ImportBuildDefinitions.xaml.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
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
    using Microsoft.TeamFoundation.Build.Workflow.Activities;
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
                        bi.StatusImage = "Graphics/Failed.png";
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
                            bi.StatusImage = "Graphics/Failed.png";
                            bi.Message = "Process Template not found - " + exdef.ProcessTemplate;
                            continue;
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
                            if (param.Key != "AgentSettings" && param.Key != "BuildSettings")
                            {
                                Newtonsoft.Json.Linq.JArray arrayItem = param.Value as Newtonsoft.Json.Linq.JArray;
                                if (arrayItem == null)
                                {
                                    Newtonsoft.Json.Linq.JObject objectItem = param.Value as Newtonsoft.Json.Linq.JObject;
                                    if (objectItem == null)
                                    {
                                        process.Add(param.Key, param.Value);
                                    }
                                    else
                                    {
                                        Microsoft.TeamFoundation.Build.Common.BuildParameter paramItem = new Microsoft.TeamFoundation.Build.Common.BuildParameter(param.Value.ToString());
                                        process.Add(param.Key, paramItem);
                                    }
                                }
                                else
                                {
                                    string[] arrayItemList = new string[arrayItem.Count];
                                    for (int i = 0; i < arrayItem.Count; i++)
                                    {
                                        arrayItemList[i] = arrayItem[i].ToString();
                                    }

                                    process.Add(param.Key, arrayItemList);
                                }
                            }
                        }

                        if (exdef.ProjectsToBuild != null)
                        {
                            process.Add("BuildSettings", new BuildSettings { ProjectsToBuild = exdef.ProjectsToBuild, PlatformConfigurations = exdef.ConfigurationsToBuild });
                        }

                        if (exdef.TfvcAgentSettings != null)
                        {
                            process.Add("AgentSettings", new AgentSettings { MaxExecutionTime = exdef.TfvcAgentSettings.MaxExecutionTime, MaxWaitTime = exdef.TfvcAgentSettings.MaxWaitTime, Name = exdef.TfvcAgentSettings.Name, TagComparison = exdef.TfvcAgentSettings.Comparison, Tags = exdef.TfvcAgentSettings.Tags });   
                        }
                        else if (exdef.GitAgentSettings != null)
                        {
                            process.Add("AgentSettings", exdef.GitAgentSettings);
                        }

                        if (exdef.BuildReasons != null)
                        {
                            foreach (var key in exdef.BuildReasons.Keys)
                            {
                                if (process.ContainsKey(key))
                                {
                                    process[key] = exdef.BuildReasons[key];
                                }
                            }
                        }

                        if (exdef.IntegerParameters != null)
                        {
                            foreach (var key in exdef.IntegerParameters.Keys)
                            {
                                if (process.ContainsKey(key))
                                {
                                    process[key] = exdef.IntegerParameters[key];
                                }
                            }
                        }

                        if (exdef.BuildVerbosities != null)
                        {
                            foreach (var key in exdef.BuildVerbosities.Keys)
                            {
                                if (process.ContainsKey(key))
                                {
                                    process[key] = exdef.BuildVerbosities[key];
                                }
                            }
                        }

                        newBuildDefinition.ProcessParameters = WorkflowHelpers.SerializeProcessParameters(process);
                        newBuildDefinition.Save();
                        bi.Status = "Succeeded";
                        bi.StatusImage = "Graphics/Succeeded.png";
                        bi.Message = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    bi.Status = "Failed";
                    bi.StatusImage = "Graphics/Failed.png"; 
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
