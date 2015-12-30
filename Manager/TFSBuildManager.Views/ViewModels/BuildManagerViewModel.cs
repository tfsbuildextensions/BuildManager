//-----------------------------------------------------------------------
// <copyright file="BuildManagerViewModel.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Microsoft.TeamFoundation.Build.Client;
    using Microsoft.TeamFoundation.Build.Common;
    using Microsoft.TeamFoundation.Build.Workflow;
    using Microsoft.TeamFoundation.Build.Workflow.Activities;
    using Newtonsoft.Json;
    using TfsBuildManager.Repository;
    using TfsBuildManager.Views.ViewModels;

    public class BuildManagerViewModel : ViewModelBase
    {
        public const string AllItem = "All";
        private readonly Window owner;
        private readonly ITfsContext context;
        private readonly ITfsClientRepository repository;
        private readonly IMainView view;
        private bool includeDisabledBuildDefinitions;
        private string buildDefinitionFilter;
        private DateFilter selectedBuildDateFilter;
        private BuildFilter selectedBuildFilter;
        private BuildView selectedBuildView;

        public BuildManagerViewModel(Window owner, ITfsClientRepository repository, IMainView view, IEnumerable<IBuildController> controllers, IEnumerable<string> teamProjects, ITfsContext context)
        {
            this.owner = owner;
            this.repository = repository;
            this.view = view;
            this.context = context;
            this.BuildDefinitions = new ObservableCollection<BuildDefinitionViewModel>();
            this.Builds = new ObservableCollection<BuildViewModel>();
            this.BuildResources = new ObservableCollection<BuildResourceViewModel>();
            this.CleanDropsCommands = new DelegateCommand(this.OnCleanDrops);
            this.DeleteCommand = new DelegateCommand(this.OnDelete);
            this.OpenDropFolderCommand = new DelegateCommand(this.OnOpenDropfolder);
            this.RetainIndefinitelyCommand = new DelegateCommand(this.OnRetainIndefinitely);
            this.SetBuildQualityCommand = new DelegateCommand(this.OnSetBuildQuality);
            this.BuildNotesCommand = new DelegateCommand(this.OnBuildNotes);
            this.DeleteBuildCommand = new DelegateCommand(this.OnDeleteBuild);
            this.ShowDetailsCommand = new DelegateCommand(this.OnShowDetails);
            this.RetryCommand = new DelegateCommand(this.OnRetry);
            this.ViewBuildLogsCommand = new DelegateCommand(this.OnViewBuildLogs);
            this.ShowQueuedDetailsCommand = new DelegateCommand(this.OnShowQueuedDetails);
            this.StopBuildCommand = new DelegateCommand(this.OnStopBuild);
            this.DisabledQueuedDefinitionCommand = new DelegateCommand(this.OnDisabledQueuedDefinition);
            this.PauseQueuedDefinitionCommand = new DelegateCommand(this.OnPauseQueuedDefinition);
            this.SetHighPriorityCommand = new DelegateCommand(() => this.SetQueuedBuildPriority(QueuePriority.High));
            this.SetAboveNormalPriorityCommand = new DelegateCommand(() => this.SetQueuedBuildPriority(QueuePriority.AboveNormal));
            this.SetNormalPriorityCommand = new DelegateCommand(() => this.SetQueuedBuildPriority(QueuePriority.Normal));
            this.SetBelowNormalPriorityCommand = new DelegateCommand(() => this.SetQueuedBuildPriority(QueuePriority.BelowNormal));
            this.SetLowPriorityCommand = new DelegateCommand(() => this.SetQueuedBuildPriority(QueuePriority.Low));
            this.ResumeBuildCommand = new DelegateCommand(this.OnResumeBuild);
            this.ChangeBuildTemplateCommand = new DelegateCommand(this.OnChangeBuildTemplate);
            this.SetDefaultBuildTemplateCommand = new DelegateCommand(this.OnSetDefaultBuildTemplate, this.OnCanSetDefaultBuildTemplate);
            this.AddBuildProcessTemplateCommand = new DelegateCommand(this.OnAddBuildProcessTemplate);
            this.RemoveBuildProcessTemplateCommand = new DelegateCommand(this.OnRemoveBuildProcessTemplate);
            this.EditControllerCommand = new DelegateCommand(this.OnEditController);
            this.EnableBuildResourceCommand = new DelegateCommand(this.OnEnableBuildResource);
            this.DisableBuildResourceCommand = new DelegateCommand(this.OnDisableBuildResource);
            this.RemoveCommand = new DelegateCommand(this.OnRemove);
            this.EnableCommand = new DelegateCommand(this.OnEnable);
            this.DisableCommand = new DelegateCommand(this.OnDisable);
            this.PauseCommand = new DelegateCommand(this.OnPause);
            this.SetRetentionPoliciesCommand = new DelegateCommand(this.OnSetRetentionsPolicies);
            this.ChangeProcessParameterCommand = new DelegateCommand(this.OnChangeProcessParameterCommand);
            this.ChangeBuildControllerCommand = new DelegateCommand(this.OnChangeBuildController);
            this.ChangeDefaultDropLocationCommand = new DelegateCommand(this.OnChangeDefaultDropLocation);
            this.ChangeOutputLocationAsConfiguredCommand = new DelegateCommand(this.OnChangeOutputLocationAsConfiguredCommand);
            this.ChangeOutputLocationPerProjectCommand = new DelegateCommand(this.OnChangeOutputLocationPerProjectCommand);
            this.ChangeOutputLocationSingleFolderCommand = new DelegateCommand(this.OnChangeOutputLocationSingleFolderCommand);
            this.ChangeTriggerCommand = new DelegateCommand(this.OnChangeTrigger);
            this.ExportDefinitionCommand = new DelegateCommand(this.OnExportBuildDefinition);
            this.CloneBuildsCommand = new DelegateCommand(this.OnCloneBuilds, this.OnCanCloneBuilds);
            this.CloneGitBuildsCommand = new DelegateCommand(this.OnCloneGitBuilds, this.OnCanCloneBuilds);
            this.CloneGitBuildsDisabledCommand = new DelegateCommand(this.CloneGitBuildsDisabled, this.OnCanCloneBuilds);
            this.CloneBuildToProjectCommand = new DelegateCommand(this.OnCloneBuildToProject, this.OnCanCloneBuilds);
            this.RemapWorkspacesCommand = new DelegateCommand(this.OnRemapWorkspaces, this.OnCanRemapWorkspaces);
            this.QueueBuildsCommand = new DelegateCommand(this.OnQueueBuilds, this.OnCanQueueBuilds);
            this.QueueHighBuildsCommand = new DelegateCommand(this.OnQueueHighBuilds, this.OnCanQueueBuilds);
            this.EditBuildDefinitionCommand = new DelegateCommand(this.OnEditBuildDefinition, this.OnCanEditBuildDefinition);
            this.GenerateBuildResourcesCommand = new DelegateCommand(this.OnGenerateBuildResources);
            this.Controllers = new ObservableCollection<string>(controllers.Select(c => c.Name));
            this.Controllers.Sort();
            this.RefreshCurrentView = new DelegateCommand(this.OnRefreshCurrentView);
            this.ImportBuildDefinition = new DelegateCommand(this.OnImportBuildDefinition);
            this.Controllers.Insert(0, AllItem);
            this.TeamProjects = new ObservableCollection<string>(teamProjects.Select(tp => tp));
            this.TeamProjects.Sort();
            this.TeamProjects.Insert(0, AllItem);
            this.SelectedBuildFilter = BuildFilter.Queued;
            this.includeDisabledBuildDefinitions = false;
            this.BuildViews = new ObservableCollection<BuildViewItem> { new BuildViewItem { Name = "Build Definitions", Value = BuildView.BuildDefinitions }, new BuildViewItem { Name = "Builds", Value = BuildView.Builds }, new BuildViewItem { Name = "Build Process Templates", Value = BuildView.BuildProcessTemplates }, new BuildViewItem { Name = "Build Resources", Value = BuildView.BuildResources } };
            this.DateFilters = new DateFilterCollection();
            this.SelectedBuildView = BuildView.BuildDefinitions;
            this.selectedBuildDateFilter = DateFilter.Today;
            this.BuildProcessTemplatess = new ObservableCollection<BuildTemplateViewModel>();
        }

        public event EventHandler Refresh;

        public ICommand CleanDropsCommands { get; private set; }

        public ICommand DeleteCommand { get; private set; }

        public ICommand DisableCommand { get; private set; }

        public ICommand PauseCommand { get; private set; }

        public ICommand EnableCommand { get; private set; }

        public ICommand ChangeBuildTemplateCommand { get; private set; }

        public ICommand SetDefaultBuildTemplateCommand { get; private set; }

        public ICommand AddBuildProcessTemplateCommand { get; private set; }

        public ICommand RemoveBuildProcessTemplateCommand { get; private set; }

        public ICommand EditControllerCommand { get; private set; }

        public ICommand EnableBuildResourceCommand { get; private set; }

        public ICommand DisableBuildResourceCommand { get; private set; }

        public ICommand RemoveCommand { get; private set; }

        public ICommand OpenDropFolderCommand { get; private set; }

        public ICommand DeleteBuildCommand { get; private set; }

        public ICommand ShowDetailsCommand { get; private set; }

        public ICommand RetryCommand { get; private set; }

        public ICommand ViewBuildLogsCommand { get; private set; }

        public ICommand ShowQueuedDetailsCommand { get; private set; }

        public ICommand ResumeBuildCommand { get; private set; }

        public ICommand StopBuildCommand { get; private set; }

        public ICommand DisabledQueuedDefinitionCommand { get; private set; }

        public ICommand PauseQueuedDefinitionCommand { get; private set; }

        public ICommand SetHighPriorityCommand { get; private set; }

        public ICommand SetAboveNormalPriorityCommand { get; private set; }

        public ICommand SetNormalPriorityCommand { get; private set; }

        public ICommand SetBelowNormalPriorityCommand { get; private set; }

        public ICommand SetLowPriorityCommand { get; private set; }

        public ICommand RetainIndefinitelyCommand { get; private set; }

        public ICommand SetBuildQualityCommand { get; private set; }

        public ICommand BuildNotesCommand { get; private set; }

        public ICommand ChangeBuildControllerCommand { get; private set; }

        public ICommand ChangeOutputLocationAsConfiguredCommand { get; private set; }

        public ICommand ChangeOutputLocationPerProjectCommand { get; private set; }

        public ICommand ChangeOutputLocationSingleFolderCommand { get; private set; }

        public ICommand ChangeDefaultDropLocationCommand { get; private set; }

        public ICommand ChangeTriggerCommand { get; private set; }

        public ICommand ExportDefinitionCommand { get; private set; }

        public ICommand SetRetentionPoliciesCommand { get; private set; }

        public ICommand ChangeProcessParameterCommand { get; private set; }

        public ICommand QueueBuildsCommand { get; private set; }

        public ICommand QueueHighBuildsCommand { get; private set; }

        public ICommand EditBuildDefinitionCommand { get; private set; }

        public ICommand CloneBuildsCommand { get; private set; }

        public ICommand CloneGitBuildsCommand { get; private set; }

        public ICommand CloneGitBuildsDisabledCommand { get; private set; }
        
        public ICommand CloneBuildToProjectCommand { get; private set; }

        public ICommand GenerateBuildResourcesCommand { get; private set; }

        public ICommand RemapWorkspacesCommand { get; private set; }

        public ICommand RefreshCurrentView { get; private set; }

        public ICommand ImportBuildDefinition { get; private set; }

        public ObservableCollection<BuildDefinitionViewModel> BuildDefinitions { get; private set; }

        public ObservableCollection<BuildViewModel> Builds { get; private set; }

        public ObservableCollection<BuildResourceViewModel> BuildResources { get; private set; }

        public ObservableCollection<BuildTemplateViewModel> BuildProcessTemplatess { get; private set; }

        public ObservableCollection<string> Controllers { get; private set; }

        public ObservableCollection<string> TeamProjects { get; private set; }

        public ObservableCollection<BuildViewItem> BuildViews { get; private set; }

        public DateFilterCollection DateFilters { get; private set; }

        public BuildFilter SelectedBuildFilter
        {
            get
            {
                return this.selectedBuildFilter;
            }

            set
            {
                var old = this.selectedBuildFilter;
                this.selectedBuildFilter = value;
                if (value != old)
                {
                    this.NotifyPropertyChanged("SelectedBuildFilter");
                }
            }
        }

        public string BuildDefinitionFilter
        {
            get
            {
                return this.buildDefinitionFilter;
            }

            set
            {
                var old = this.buildDefinitionFilter;
                this.buildDefinitionFilter = value;
                if (value != old)
                {
                    this.NotifyPropertyChanged("BuildDefinitionFilter");
                }
            }
        }

        public bool IncludeDisabledBuildDefinitions
        {
            get
            {
                return this.includeDisabledBuildDefinitions;
            }

            set
            {
                var old = this.includeDisabledBuildDefinitions;
                this.includeDisabledBuildDefinitions = value;
                if (value != old)
                {
                    this.NotifyPropertyChanged("includeDisabledBuildDefinitions");
                }
            }
        }

        public BuildView SelectedBuildView
        {
            get
            {
                return this.selectedBuildView;
            }

            set
            {
                var old = this.selectedBuildView;
                this.selectedBuildView = value;
                if (value != old)
                {
                    this.NotifyPropertyChanged("SelectedBuildView");
                    this.NotifyPropertyChanged("BuildDefinitionViewVisible");
                    this.NotifyPropertyChanged("BuildsViewVisible");
                    this.NotifyPropertyChanged("BuildProcessTemplateViewVisible");
                    this.NotifyPropertyChanged("BuildResourcesViewVisible");
                }
            }
        }

        public DateFilter SelectedDateFilter
        {
            get
            {
                return this.selectedBuildDateFilter;
            }

            set
            {
                var old = this.selectedBuildDateFilter;
                this.selectedBuildDateFilter = value;
                if (value != old)
                {
                    this.OnRefresh(new EventArgs());
                }
            }
        }

        public Visibility BuildDefinitionViewVisible
        {
            get { return this.SelectedBuildView == BuildView.BuildDefinitions ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility BuildsViewVisible
        {
            get { return this.SelectedBuildView == BuildView.Builds ? Visibility.Visible : Visibility.Collapsed; }
        }

        public bool IsTfvcProject
        {
            get
            {
                if (this.view.SelectedItems.Count() != 1)
                {
                    return false;
                }

                return this.view.SelectedItems.First().IsTfvcProject;
            }
        }

        public bool IsGitProject
        {
            get
            {
                return !this.IsTfvcProject;
            }
        }

        public Visibility BuildProcessTemplateViewVisible
        {
            get { return this.SelectedBuildView == BuildView.BuildProcessTemplates ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility BuildResourcesViewVisible
        {
            get { return this.SelectedBuildView == BuildView.BuildResources ? Visibility.Visible : Visibility.Collapsed; }
        }

        public static void ExportDefinition(BuildDefinitionViewModel b, string filePath)
        {
            ExportedBuildDefinition buildToExport = new ExportedBuildDefinition { Name = b.BuildDefinition.Name, Description = b.BuildDefinition.Description };
            if (b.BuildDefinition.BuildController != null)
            {
                buildToExport.BuildController = b.BuildDefinition.BuildController.Name;
            }

            buildToExport.ContinuousIntegrationType = b.BuildDefinition.ContinuousIntegrationType;
            buildToExport.DefaultDropLocation = b.BuildDefinition.DefaultDropLocation;
            buildToExport.Schedules = new List<ExportedISchedule>();

            foreach (var schedule in b.BuildDefinition.Schedules)
            {
                buildToExport.Schedules.Add(new ExportedISchedule { StartTime = schedule.StartTime, DaysToBuild = schedule.DaysToBuild, TimeZone = schedule.TimeZone });
            }

            buildToExport.SourceProviders = new List<ExportedIBuildDefinitionSourceProvider>();
            foreach (var provider in b.BuildDefinition.SourceProviders)
            {
                buildToExport.SourceProviders.Add(new ExportedIBuildDefinitionSourceProvider { Fields = provider.Fields, Name = provider.Name, SupportedTriggerTypes = provider.SupportedTriggerTypes });
            }

            buildToExport.QueueStatus = b.BuildDefinition.QueueStatus;
            buildToExport.ContinuousIntegrationQuietPeriod = b.BuildDefinition.ContinuousIntegrationQuietPeriod;

            if (b.BuildDefinition.SourceProviders.All(s => s.Name != "TFGIT"))
            {
                buildToExport.Mappings = new List<ExportedIWorkspaceMapping>();
                foreach (var map in b.BuildDefinition.Workspace.Mappings)
                {
                    buildToExport.Mappings.Add(new ExportedIWorkspaceMapping { Depth = map.Depth, LocalItem = map.LocalItem, MappingType = map.MappingType, ServerItem = map.ServerItem });
                }
            }

            buildToExport.RetentionPolicyList = new List<ExportedIRetentionPolicy>();
            foreach (var rp in b.BuildDefinition.RetentionPolicyList)
            {
                buildToExport.RetentionPolicyList.Add(new ExportedIRetentionPolicy { BuildDefinition = rp.BuildDefinition, BuildReason = rp.BuildReason, BuildStatus = rp.BuildStatus, NumberToKeep = rp.NumberToKeep, DeleteOptions = rp.DeleteOptions });
            }

            if (b.BuildDefinition.Process != null)
            {
                buildToExport.ProcessTemplate = b.BuildDefinition.Process.ServerPath;
            }

            var processParameters = WorkflowHelpers.DeserializeProcessParameters(b.BuildDefinition.ProcessParameters);
            if (processParameters.ContainsKey("AgentSettings"))
            {
                if (processParameters["AgentSettings"].GetType() == typeof(AgentSettings))
                {
                    buildToExport.TfvcAgentSettings = (AgentSettings)processParameters["AgentSettings"];
                }
                else if (processParameters["AgentSettings"].GetType() == typeof(BuildParameter))
                {
                    buildToExport.GitAgentSettings = (BuildParameter)processParameters["AgentSettings"];
                }
            }

            if (processParameters.ContainsKey("BuildSettings"))
            {
                var buildSettings = processParameters["BuildSettings"] as BuildSettings;
                if (buildSettings != null && buildSettings.HasProjectsToBuild)
                {
                    buildToExport.ProjectsToBuild = buildSettings.ProjectsToBuild;
                    if (buildSettings.HasPlatformConfigurations)
                    {
                        buildToExport.ConfigurationsToBuild = buildSettings.PlatformConfigurations;
                    }
                }
            }

            if (processParameters.ContainsKey("TestSpecs"))
            {
                var testSpecs = processParameters["TestSpecs"] as TestSpecList;
                if (testSpecs != null)
                {
                    buildToExport.AgileTestSpecs = new List<ExportedAgileTestPlatformSpec>();
                    foreach (var spec in testSpecs)
                    {
                        var agilespec = spec as AgileTestPlatformSpec;
                        if (agilespec != null)
                        {
                            buildToExport.AgileTestSpecs.Add(agilespec);
                        }                        
                    }
                }
            }

            buildToExport.ProcessParameters = WorkflowHelpers.DeserializeProcessParameters(b.BuildDefinition.ProcessParameters);
            foreach (KeyValuePair<string, object> item in processParameters)
            {
                if (item.Value.GetType() == typeof(Microsoft.TeamFoundation.Build.Common.BuildParameter))
                {
                    buildToExport.ProcessParameters[item.Key] = JsonConvert.DeserializeObject(item.Value.ToString());
                }
                else if (item.Value.GetType() == typeof(BuildReason))
                {
                    buildToExport.BuildReasons.Add(item.Key, (BuildReason)item.Value);
                }
                else if (item.Value is int)
                {
                    buildToExport.IntegerParameters.Add(item.Key, (int)item.Value);
                }
                else if (item.Value.GetType() == typeof(BuildVerbosity))
                {
                    buildToExport.BuildVerbosities.Add(item.Key, (BuildVerbosity)item.Value);
                }
            }

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            };

            File.WriteAllText(Path.Combine(filePath, b.Name + ".json"), JsonConvert.SerializeObject(buildToExport, Formatting.Indented, jsonSerializerSettings));
        }

        public void OnCleanDrops()
        {
            try
            {
                var items = this.view.SelectedItems;
                if (MessageBox.Show("Are you sure you want to delete drop folder of all deleted builds?", "Clean Drop Folders", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using (new WaitCursor())
                    {
                        this.repository.CleanDropsFolders(items.Select(b => b.Uri));
                        this.OnRefresh(new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void OnDelete()
        {
            try
            {
                var items = this.view.SelectedItems;
                DeleteOptions? options = this.PromptDeleteOptions();
                if (options.HasValue)
                {
                    using (new WaitCursor())
                    {
                        this.repository.DeleteBuildDefinitions(items.Select(b => b.Uri), options.Value);
                        this.OnRefresh(new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void OnSetDefaultBuildTemplate()
        {
            try
            {
                var items = this.view.SelectedBuildProcessTemplates;
                foreach (var item in items)
                {
                    this.repository.SetDefaultBuildProcessTemplate(item.ServerPath, item.TeamProject);
                }

                this.OnRefresh(new EventArgs());
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public bool OnCanSetDefaultBuildTemplate()
        {
            var items = this.view.SelectedBuildProcessTemplates.ToList();
            return items.Count() == 1 && items.First().TemplateType != "Default";
        }

        public void OnAddBuildProcessTemplate()
        {
            try
            {
                var items = this.view.SelectedBuildProcessTemplates;
                var projects = this.repository.AllTeamProjects.Select(tp => tp).ToList();
                var viewModel = new TeamProjectListViewModel(projects);

                var wnd = new SelectTeamProject(viewModel, null);
                bool? res = wnd.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    using (new WaitCursor())
                    {
                        this.repository.AddBuildProcessTemplates(items.Select(pt => pt.ServerPath), wnd.SelectedTeamProjects.Select(tp => tp.Name), wnd.SetAsDefault);
                        this.OnRefresh(new EventArgs());
                    }

                    this.OnRefresh(new EventArgs());
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void OnRemoveBuildProcessTemplate()
        {
            try
            {
                var items = this.view.SelectedBuildProcessTemplates.ToList();
                var projects = this.repository.AllTeamProjects.Select(tp => tp).ToList();
                var viewModel = new TeamProjectListViewModel(projects);

                var wnd = new SelectTeamProject(viewModel, this.view.SelectedTeamProject) { cbSetAsDefault = { Visibility = Visibility.Collapsed } };
                bool? res = wnd.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    using (new WaitCursor())
                    {
                        this.repository.RemoveBuildProcessTemplates(items.Select(pt => pt.ServerPath), wnd.SelectedTeamProjects.Select(tp => tp.Name), this.ShouldRemove);
                        this.OnRefresh(new EventArgs());
                    }

                    this.OnRefresh(new EventArgs());
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void OnEditController()
        {
            try
            {
                this.context.ShowControllerManager();
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void OnEnableBuildResource()
        {
            try
            {
                var selectedBuildResources = this.view.SelectedBuildResources.ToList();
                if (!selectedBuildResources.Any())
                {
                    return;
                }

                foreach (var res in selectedBuildResources)
                {
                    res.OnEnable(this.repository);
                }

                this.OnRefresh(new EventArgs());
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void OnDisableBuildResource()
        {
            try
            {
                var selectedBuildResources = this.view.SelectedBuildResources.ToList();
                if (!selectedBuildResources.Any())
                {
                    return;
                }

                foreach (var res in selectedBuildResources)
                {
                    res.OnDisable(this.repository);
                }

                this.OnRefresh(new EventArgs());
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void OnRemove()
        {
            try
            {
                var selectedBuildResources = this.view.SelectedBuildResources.ToList();
                if (!selectedBuildResources.Any())
                {
                    return;
                }

                if (MessageBox.Show("Are you sure you want to delete the selected item(s)?", "Community TFS Build Manager", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    foreach (var res in selectedBuildResources)
                    {
                        res.OnRemove(this.repository);
                    }

                    this.OnRefresh(new EventArgs());
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void OnDisable()
        {
            try
            {
                var items = this.view.SelectedItems;
                using (new WaitCursor())
                {
                    this.repository.DisableBuildDefinitions(items.Select(b => b.Uri));
                    this.OnRefresh(new EventArgs());
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void OnPause()
        {
            try
            {
                var items = this.view.SelectedItems;
                using (new WaitCursor())
                {
                    this.repository.PauseBuildDefinitions(items.Select(b => b.Uri));
                    this.OnRefresh(new EventArgs());
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void OnOpenDropfolder()
        {
            try
            {
                var items = this.view.SelectedBuilds;
                using (new WaitCursor())
                {
                    if (!this.repository.OpenDropFolder(items.Select(b => b.DropLocation)))
                    {
                        MessageBox.Show("No valid drop folders were found for the selected builds", "Open Drop Folder", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void OnRetry()
        {
            try
            {
                var items = this.view.SelectedBuilds;
                using (new WaitCursor())
                {
                    this.repository.RetryBuilds(items.Select(b => b.BuildDefinitionUri));
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void OnShowDetails()
        {
            foreach (var selectedBuild in this.view.SelectedBuilds)
            {
                this.context.ShowBuild(selectedBuild.Uri);
            }
        }

        public void OnViewBuildLogs()
        {
            var selectedBuild = this.view.SelectedBuilds.First();
            string logUrl = this.repository.GetBuildLogLocation(selectedBuild.FullBuildDetail);
            Process.Start(logUrl);
        }

        public void OnShowQueuedDetails()
        {
            foreach (var selectedBuild in this.view.SelectedActiveBuilds)
            {
                this.context.ShowBuild(selectedBuild.Uri);
            }
        }

        public void OnDisabledQueuedDefinition()
        {
            try
            {
                var items = this.view.SelectedActiveBuilds;
                using (new WaitCursor())
                {
                    this.repository.DisableBuildDefinitions(items.Select(b => b.BuildDefinitionUri));
                    this.OnRefresh(new EventArgs());
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void OnPauseQueuedDefinition()
        {
            try
            {
                var items = this.view.SelectedActiveBuilds;
                using (new WaitCursor())
                {
                    this.repository.PauseBuildDefinitions(items.Select(b => b.BuildDefinitionUri));
                    this.OnRefresh(new EventArgs());
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void SetQueuedBuildPriority(QueuePriority priority)
        {
            try
            {
                var items = this.view.SelectedActiveBuilds.ToList();

                if (!items.Any())
                {
                    return;
                }

                using (new WaitCursor())
                {
                    this.repository.SetQueuedBuildPriority(items.Select(b => b.BuildDefinitionUri), priority);
                    this.OnRefresh(new EventArgs());
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void OnDeleteBuild()
        {
            try
            {
                if (MessageBox.Show(this.owner, "Confirm Delete. ALL artefacts will be deleted.", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var items = this.view.SelectedBuilds;
                    using (new WaitCursor())
                    {
                        this.repository.DeleteBuilds(items.Select(b => b.FullBuildDetail));
                        this.OnRefresh(new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void OnResumeBuild()
        {
            try
            {
                var items = this.view.SelectedActiveBuilds;
                using (new WaitCursor())
                {
                    this.repository.ResumeBuilds(items.Select(b => b.QueuedBuildDetail));
                    this.OnRefresh(new EventArgs());
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void OnStopBuild()
        {
            try
            {
                if (MessageBox.Show(this.owner, "Confirm stop.", "Stop", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var items = this.view.SelectedActiveBuilds;
                    using (new WaitCursor())
                    {
                        this.repository.StopBuilds(items.Select(b => b.QueuedBuildDetail));
                        this.OnRefresh(new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void OnBuildNotes()
        {
            try
            {
                var items = this.view.SelectedBuilds;
                var options = this.PromptNotesOptions();
                if (options != null)
                {
                    using (new WaitCursor())
                    {
                        this.repository.GenerateBuildNotes(items.Select(b => b.FullBuildDetail), options);
                        this.OnRefresh(new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void OnRetainIndefinitely()
        {
            try
            {
                var items = this.view.SelectedBuilds;
                using (new WaitCursor())
                {
                    this.repository.RetainIndefinitely(items.Select(b => b.FullBuildDetail));
                    this.OnRefresh(new EventArgs());
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void OnSetBuildQuality()
        {
            try
            {
                var items = this.view.SelectedBuilds.ToList();
                var buildQualities = this.repository.GetBuildQualities(items.Select(bd => bd.TeamProject).ToList().Distinct());
                var wnd = new SelectBuildQuality(new BuildQualityListViewModel(buildQualities));
                bool? res = wnd.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    using (new WaitCursor())
                    {
                        this.repository.SetBuildQualities(items.Select(bd => bd.Uri), wnd.SelectedBuildQuality.Name);
                        this.OnRefresh(new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void OnEnable()
        {
            try
            {
                var items = this.view.SelectedItems;
                using (new WaitCursor())
                {
                    this.repository.EnableBuildDefinitions(items.Select(b => b.Uri));
                    this.OnRefresh(new EventArgs());
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void AssignBuildDefinitions(IEnumerable<IBuildDefinition> builds)
        {
            try
            {
                this.BuildDefinitions.Clear();
                foreach (var b in builds.Select(b => new BuildDefinitionViewModel(b)))
                {
                    this.BuildDefinitions.Add(b);
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void AssignBuilds(IEnumerable<IBuildDetail> builds)
        {
            try
            {
                this.Builds.Clear();
                foreach (var b in builds.Where(b => b != null).Select(b => new BuildViewModel(b)))
                {
                    this.Builds.Add(b);
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void AssignBuildProcessTemplates(IEnumerable<IProcessTemplate> buildProcessTemplates)
        {
            try
            {
                this.BuildProcessTemplatess.Clear();
                foreach (var b in buildProcessTemplates.Where(b => b != null).Select(b => new BuildTemplateViewModel(b)))
                {
                    this.BuildProcessTemplatess.Add(b);
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void AssignBuildResources(IEnumerable<IBuildController> buildResources)
        {
            try
            {
                this.BuildResources.Clear();
                foreach (var c in buildResources)
                {
                    this.BuildResources.Add(new BuildControllerResourceViewModel(c));
                    foreach (var a in c.Agents)
                    {
                        this.BuildResources.Add(new BuildAgentViewModel(a));
                    }
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        public void AssignBuilds(IEnumerable<IQueuedBuild> queuedBuilds)
        {
            try
            {
                this.Builds.Clear();
                foreach (var b in queuedBuilds.Where(b => b != null).Select(b => new BuildViewModel(b)))
                {
                    this.Builds.Add(b);
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        private static string GetCommonString(List<string> lst, bool caseSensitive = false)
        {
            string shortest = lst[0];
            for (int i = 0; i < lst.Count; i++)
            {
                string s = lst[i];
                if (s.Length < shortest.Length)
                {
                    shortest = s;
                }

                if (!caseSensitive)
                {
                    lst[i] = lst[i].ToUpper();
                }
            }

            for (int i = 0; i < shortest.Length; i++)
            {
                char c = lst[0].ElementAt(i);

                if (lst.Any(s => c != s.ElementAt(i)))
                {
                    return shortest.Substring(0, i);
                }
            }

            return shortest;
        }

        private void ShowNoBranchMessage(string project)
        {
            MessageBox.Show(this.owner, "Could not locate branch object for " + project, "Clone Build To Branch", MessageBoxButton.OK, MessageBoxImage.Stop);
        }

        private void ShowInvalidActionMessage(string action, string message)
        {
            MessageBox.Show(this.owner, message, action, MessageBoxButton.OK, MessageBoxImage.Stop);
        }

        private bool ShouldRemove()
        {
            return MessageBox.Show(this.owner, "One or more of the selected build process templates are used by build definitions. Do you want to proceed?", "Remove Build Process Template", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
        }

        private IEnumerable<string> PromptNotesOptions()
        {
            try
            {
                var wnd = new BuildNotesOptionWnd();
                bool? action = wnd.ShowDialog();
                if (action.HasValue && action.Value)
                {
                    return wnd.Option;
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }

            return null;
        }

        private DeleteOptions? PromptDeleteOptions()
        {
            try
            {
                var wnd = new DeleteOptionsWindow();
                var action = wnd.ShowDialog();
                if (action.HasValue && action.Value)
                {
                    return wnd.Option;
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }

            return null;
        }

        private bool OnCanCloneBuilds()
        {
            try
            {
                return this.view.SelectedItems.Any();
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }

            return false;
        }

        private void OnRefreshCurrentView()
        {
            this.OnRefresh(new EventArgs());
        }

        private void OnImportBuildDefinition()
        {
            if (this.view.SelectedTeamProject == "All")
            {
                MessageBox.Show("Please select an individual Team Project.", "Error: Unable to import into All projects", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var wnd = new ImportBuildDefinitions(this.view.SelectedTeamProject, this.repository.GetBuildServer());
            wnd.ShowDialog();
            this.OnRefresh(new EventArgs());
        }

        private void OnGenerateBuildResources()
        {
            try
            {
                using (new WaitCursor())
                {
                    var dgml = this.repository.GenerateBuildResourcesDependencyGraph();
                    string tempFile = Path.GetTempFileName();
                    tempFile = Path.ChangeExtension(tempFile, ".dgml");
                    using (var sw = new StreamWriter(tempFile))
                    {
                        sw.Write(dgml);
                    }

                    Process.Start(tempFile);
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        private void OnCloneBuilds()
        {
            try
            {
                var items = this.view.SelectedItems.ToList();
                if (!items.Any())
                {
                    return;
                }

                foreach (var item in items)
                {
                    if (item.BuildDefinition.SourceProviders.Any(s => s.Name.ToUpperInvariant().Contains("GIT")))
                    {
                        return;
                    }

                    using (new WaitCursor())
                    {
                        var projects = this.repository.GetProjectsToBuild(item.Uri).ToList();
                        if (!projects.Any())
                        {
                            this.ShowInvalidActionMessage("Clone Build to Branch", "Could not locate any projects in the selected build(s)");
                            return;
                        }

                        var project = projects.First();
                        var branchObject = this.repository.GetBranchObjectForItem(project);
                        if (branchObject == null)
                        {
                            this.ShowNoBranchMessage(project);
                            return;
                        }

                        var childBranches = this.repository.GetChildBranchObjectsForItem(branchObject.ServerPath).ToList();
                        if (!childBranches.Any())
                        {
                            MessageBox.Show(this.owner, "No branch exist for " + branchObject.ServerPath, "Clone Build To Branch", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }

                        var dlg = new SelectTargetBranchWindow(item.Name, childBranches, item.TeamProject, this.repository);
                        bool? res = dlg.ShowDialog();
                        if (res.HasValue && res.Value)
                        {
                            this.repository.CloneBuild(item.Uri, dlg.NewBuildDefinitionName, branchObject, dlg.SelectedTargetBranch);
                        }
                        else if (res.HasValue)
                        {
                            break;
                        }

                        this.OnRefresh(new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        private void OnCloneGitBuilds()
        {
            try
            {
                var items = this.view.SelectedItems.ToList();
                if (!items.Any())
                {
                    return;
                }

                foreach (var item in items)
                {
                    // check for corrupted builds caused by the TFS Power Tools Clone feature.
                    if (item.BuildDefinition.SourceProviders.Any(s => s.Name.ToUpperInvariant().Contains("TFVC")))
                    {
                        MessageBox.Show(string.Format("{0} appears to be bound to TFVC rather than Git.\n\nIf you cloned this build using the TFS Power Tools Clone menu, it will have corrupted your definition. You should create a new definition and delete this one.", item.Name), "Error Exporting " + item.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Note that if a Git build pulls from a repo then its sourceprovider is TFGIT. If it does not, then its SourceProvider is GIT
                    if (!item.BuildDefinition.SourceProviders.All(s => s.Name.ToUpperInvariant().Contains("GIT")))
                    {
                        return;
                    }

                    using (new WaitCursor())
                    {
                        this.repository.CloneGitBuild(item.Uri, item.Name + "_" + DateTime.Now.ToString("F").Replace(":", "-"), false);
                        this.OnRefresh(new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        private void CloneGitBuildsDisabled()
        {
            try
            {
                var items = this.view.SelectedItems.ToList();
                if (!items.Any())
                {
                    return;
                }

                foreach (var item in items)
                {
                    // check for corrupted builds caused by the TFS Power Tools Clone feature.
                    if (item.BuildDefinition.SourceProviders.Any(s => s.Name.ToUpperInvariant().Contains("TFVC")))
                    {
                        MessageBox.Show(string.Format("{0} appears to be bound to TFVC rather than Git.\n\nIf you cloned this build using the TFS Power Tools Clone menu, it will have corrupted your definition. You should create a new definition and delete this one.", item.Name), "Error Exporting " + item.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Note that if a Git build pulls from a repo then its sourceprovider is TFGIT. If it does not, then its SourceProvider is GIT
                    if (!item.BuildDefinition.SourceProviders.All(s => s.Name.ToUpperInvariant().Contains("GIT")))
                    {
                        return;
                    }

                    using (new WaitCursor())
                    {
                        this.repository.CloneGitBuild(item.Uri, item.Name + "_" + DateTime.Now.ToString("F").Replace(":", "-"), true);
                        this.OnRefresh(new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }      

        private void OnCloneBuildToProject()
        {
            try
            {
                var items = this.view.SelectedItems.ToList();
                if (!items.Any())
                {
                    return;
                }

                foreach (var item in items)
                {
                    using (new WaitCursor())
                    {
                        var projects = this.repository.AllTeamProjects.Select(tp => tp).ToList();
                        var viewModel = new TeamProjectListViewModel(projects);

                        var wnd = new SelectTeamProject(viewModel, this.view.SelectedTeamProject) { cbSetAsDefault = { Visibility = Visibility.Collapsed } };
                        bool? res = wnd.ShowDialog();
                        if (res.HasValue && res.Value)
                        {
                            using (new WaitCursor())
                            {
                                string targetProject = wnd.SelectedTeamProjects.Select(tp => tp.Name).First();
                                if (item.BuildDefinition.TeamProject == targetProject)
                                {
                                    item.Name = item.Name + "_" + DateTime.Now.ToString("F").Replace(":", "-");
                                }

                                this.repository.CloneBuildToProject(item.Uri, item.Name, targetProject);
                            }

                            this.OnRefresh(new EventArgs());
                        }
                        else if (res.HasValue)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        private void OnRemapWorkspaces()
        {
            try
            {
                if (this.selectedBuildView == BuildView.BuildDefinitions)
                {
                    var buildDefinition = this.view.SelectedItems.First().BuildDefinition;
                    this.context.RemapWorkspaces(buildDefinition);
                }
                else if (this.selectedBuildFilter == BuildFilter.Completed)
                {
                    var buildDefinition = this.view.SelectedBuilds.First().FullBuildDefinition;
                    this.context.RemapWorkspaces(buildDefinition);
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        private bool OnCanRemapWorkspaces()
        {
            try
            {
                return this.view.SelectedItems.Count() == 1 || this.view.SelectedBuilds.Count() == 1;
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }

            return false;
        }

        private void OnQueueHighBuilds()
        {
            try
            {
                var items = this.view.SelectedItems;
                using (new WaitCursor())
                {
                    this.repository.QueueHighBuilds(items.Select(b => b.Uri));
                    this.SelectedBuildView = BuildView.Builds;
                    this.SelectedBuildFilter = BuildFilter.Queued;
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        private void OnQueueBuilds()
        {
            try
            {
                var items = this.view.SelectedItems;
                using (new WaitCursor())
                {
                    this.repository.QueueBuilds(items.Select(b => b.Uri));
                    this.SelectedBuildView = BuildView.Builds;
                    this.SelectedBuildFilter = BuildFilter.Queued;
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        private void OnEditBuildDefinition()
        {
            try
            {
                if (this.selectedBuildView == BuildView.BuildDefinitions)
                {
                    var buildDefinitionUri = this.view.SelectedItems.First().Uri;
                    this.context.EditBuildDefinition(buildDefinitionUri);
                }
                else if (this.selectedBuildFilter == BuildFilter.Completed)
                {
                    var buildDefinitionUri = this.view.SelectedBuilds.First().BuildDefinitionUri;
                    this.context.EditBuildDefinition(buildDefinitionUri);
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        private bool OnCanEditBuildDefinition()
        {
            try
            {
                return this.view.SelectedItems.Count() == 1 || this.view.SelectedBuilds.Count() == 1;
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }

            return false;
        }

        private bool OnCanQueueBuilds()
        {
            try
            {
                return this.view.SelectedItems.Any(s => !s.HasProcess);
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }

            return false;
        }

        private void OnSetRetentionsPolicies()
        {
            try
            {
                var items = this.view.SelectedItems;
                var wnd = new RetentionPolicyWindow();
                bool? res = wnd.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    using (new WaitCursor())
                    {
                        this.repository.SetRetentionPolicies(items.Select(bd => bd.Uri), wnd.BuildRetentionPolicy);
                        this.OnRefresh(new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        private void OnChangeProcessParameterCommand()
        {
            try
            {
                var items = this.view.SelectedItems;
                var wnd = new ProcessParameterWindow();
                bool? res = wnd.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    using (new WaitCursor())
                    {
                        this.repository.ChangeProcessParameter(items.Select(bd => bd.Uri), wnd.ProcessParameter, wnd.BooleanParameter);
                        this.OnRefresh(new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        private void OnRefresh(EventArgs e)
        {
            try
            {
                EventHandler handler = this.Refresh;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        private void OnChangeBuildTemplate()
        {
            try
            {
                var items = this.view.SelectedItems.ToList();
                var teamProjects = items.Select(i => i.TeamProject).Distinct();
                IEnumerable<IProcessTemplate> buildTemplates;
                using (new WaitCursor())
                {
                    buildTemplates = this.repository.GetBuildProcessTemplates(teamProjects);
                }

                var viewModel = new BuildTemplateListViewModel(buildTemplates);
                var wnd = new SelectBuildProcessTemplateWindow(viewModel);
                bool? res = wnd.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    using (new WaitCursor())
                    {
                        this.repository.AssignBuildProcessTemplate(items.Select(bd => bd.Uri), wnd.SelectedBuildTemplate.ServerPath);
                        this.OnRefresh(new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        private void OnExportBuildDefinition()
        {
            var items = this.view.SelectedItems;
            using (new WaitCursor())
            {
                System.Windows.Forms.FolderBrowserDialog saveFolder = new System.Windows.Forms.FolderBrowserDialog { Description = "Select a folder to export to..." };
                System.Windows.Forms.DialogResult result2 = saveFolder.ShowDialog();
                if (result2 == System.Windows.Forms.DialogResult.OK)
                {
                    foreach (var b in items)
                    {
                        try
                        {
                            ExportDefinition(b, saveFolder.SelectedPath);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), "Error Exporting " + b.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private void OnChangeTrigger()
        {
            try
            {
                var items = this.view.SelectedItems;
                var viewModel = new TriggerViewModel();

                var wnd = new TriggerWindow(viewModel);
                bool? res = wnd.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    using (new WaitCursor())
                    {
                        if (wnd.Trigger.TriggerType == DefinitionTriggerType.Schedule || wnd.Trigger.TriggerType == DefinitionTriggerType.ScheduleForced)
                        {
                            this.repository.UpdateTrigger(items.Select(bd => bd.Uri), wnd.Trigger.TriggerType, wnd.Trigger.ScheduleDays, wnd.Trigger.ScheduleTime, wnd.Trigger.TimeZoneInfo);
                        }
                        else
                        {
                            this.repository.UpdateTrigger(items.Select(bd => bd.Uri), wnd.Trigger.Minutes, wnd.Trigger.Submissions, wnd.Trigger.TriggerType);
                        }

                        this.OnRefresh(new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        private void OnChangeOutputLocationAsConfiguredCommand()
        {
            var items = this.view.SelectedItems;
            using (new WaitCursor())
            {
                this.repository.UpdateOutputLocation(items.Select(bd => bd.Uri), "AsConfigured");
                this.OnRefresh(new EventArgs());
            }
        }

        private void OnChangeOutputLocationPerProjectCommand()
        {
            var items = this.view.SelectedItems;
            using (new WaitCursor())
            {
                this.repository.UpdateOutputLocation(items.Select(bd => bd.Uri), "PerProject");
                this.OnRefresh(new EventArgs());
            }
        }

        private void OnChangeOutputLocationSingleFolderCommand()
        {
            var items = this.view.SelectedItems;
            using (new WaitCursor())
            {
                this.repository.UpdateOutputLocation(items.Select(bd => bd.Uri), "SingleFolder");
                this.OnRefresh(new EventArgs());
            }
        }

        private void OnChangeDefaultDropLocation()
        {
            try
            {
                var items = this.view.SelectedItems.ToList();
                var viewModel = new DropLocationViewModel();
                var lstDropLocations = from b in items orderby b.DefaultDropLocation.Length select b.DefaultDropLocation;
                viewModel.SearchTxt = GetCommonString(lstDropLocations.ToList());
                viewModel.ReplaceTxt = viewModel.SearchTxt;
                viewModel.SetDropLocation = string.Empty;
                viewModel.AddMacro("$(TeamProject)", "TO BE SET");
                viewModel.AddMacro("$(BuildDefinition)", "TO BE SET");
                viewModel.AddMacro("$(BuildServer)", "TO BE SET");
                viewModel.AddMacro("$(BuildType)", "TO BE SET");

                var wnd = new DropLocationWindow(viewModel);
                bool? res = wnd.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    using (new WaitCursor())
                    {
                        if (viewModel.ModeReplace)
                        {
                            this.repository.ChangeDefaultDropLocation(items.Select(bd => bd.Uri), viewModel.ReplaceTxt, viewModel.SearchTxt, viewModel.UpdateExistingBuilds);
                            this.OnRefresh(new EventArgs());
                        }
                        else
                        {
                            this.repository.SetDefaultDropLocation(items.Select(bd => bd.Uri), viewModel.SetDropLocation, viewModel.Macros, viewModel.UpdateExistingBuilds);
                            this.OnRefresh(new EventArgs());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }

        private void OnChangeBuildController()
        {
            try
            {
                var items = this.view.SelectedItems;
                var controllers = this.repository.Controllers;
                var viewModel = new BuildControllerListViewModel(controllers);

                var wnd = new BuildControllerWindow(viewModel);
                bool? res = wnd.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    using (new WaitCursor())
                    {
                        this.repository.ChangeBuildController(items.Select(bd => bd.Uri), wnd.SelectedBuildController.Name);
                        this.OnRefresh(new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ex);
            }
        }
    }
}
