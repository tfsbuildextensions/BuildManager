//-----------------------------------------------------------------------
// <copyright file="MainView.xaml.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;
    using Microsoft.TeamFoundation.Build.Client;
    using Microsoft.TeamFoundation.Build.Workflow;
    using TfsBuildManager.Repository;
    using TfsBuildManager.Views.ViewModels;

    /// <summary>
    /// Interaction logic for MainView
    /// </summary>
    public partial class MainView : IMainView
    {
        private ITfsContext context;
        private ITfsClientRepository repository;
        private BuildManagerViewModel viewmodel;
        private DispatcherTimer dispatcherTimer;
        private bool initialized;
        
        public MainView()
        {
            try
            {
                this.InitializeComponent();
                this.dispatcherTimer = new DispatcherTimer();
                this.dispatcherTimer.Tick += this.OnTimerUpdate;
                this.dispatcherTimer.Interval = new TimeSpan(0, 0, 2, 0);
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                this.lblVersion.Content = fvi.ProductVersion;
                this.initialized = false;
            }
            catch (Exception ex)
            {
                this.DisplayError(ex);
            }
        }

        public IEnumerable<BuildDefinitionViewModel> SelectedItems
        {
            get { return this.BuildDefinitionList.SelectedItems.Cast<BuildDefinitionViewModel>(); }
        }

        public IEnumerable<BuildViewModel> SelectedBuilds
        {
            get { return this.CompletedBuilds.BuildList.SelectedItems.Cast<BuildViewModel>(); }
        }

        public IEnumerable<BuildViewModel> SelectedActiveBuilds
        {
            get { return this.RunningBuilds.BuildList.SelectedItems.Cast<BuildViewModel>(); }
        }

        public IEnumerable<BuildTemplateViewModel> SelectedBuildProcessTemplates
        {
            get { return this.ProcessTemplateGrid.ProcessTemplateList.SelectedItems.Cast<BuildTemplateViewModel>(); }
        }

        public IEnumerable<BuildResourceViewModel> SelectedBuildResources
        {
            get { return this.BuildResourcesGrid.BuildResourcesList.SelectedItems.Cast<BuildResourceViewModel>(); }
        }

        public string SelectedController
        {
            get { return this.ControllerCombo.SelectedItem as string; }
            set { this.ControllerCombo.SelectedItem = value; }
        }

        public string SelectedTeamProject
        {
            get { return this.TeamProjectCombo.SelectedItem as string; }
            set { this.TeamProjectCombo.SelectedItem = value; }
        }

        public void InitializeContext(ITfsContext tfsContext)
        {
            this.initialized = false;
            this.context = tfsContext;
            this.context.ProjectChanged += this.OnProjectChanged;
        }

        public void InitializeRepository(ITfsClientRepository rep)
        {
            this.repository = rep;
        }

        public void Reload()
        {
            this.InitializeMainView();
        }

        public void OnRefresh(object sender, EventArgs e)
        {
#if !DEBUG
            try
            {
#endif
                this.UpdateBuildDefinitions();
#if !DEBUG
            }
            catch (Exception ex)
            {
                this.DisplayError(ex);
            }
#endif
        }

        public void DisplayError(Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Community TFS Build Manager", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void OnProjectChanged(object sender, EventArgs e)
        {
            try
            {
                this.Reload();
            }
            catch (Exception ex)
            {
                this.DisplayError(ex);
            }
        }

        private BuildResourceFilter CreateBuildResourceFilter()
        {
            return new BuildResourceFilter { Controller = this.SelectedController == BuildManagerViewModel.AllItem ? null : this.SelectedController, TeamProject = this.SelectedTeamProject == BuildManagerViewModel.AllItem ? null : this.SelectedTeamProject };
        }

        private void OnTeamProjectChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectedTeamProject != null)
            {
                this.UpdateBuildDefinitions();
            }
        }

        private void OnControllerChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectedController != null)
            {
                this.UpdateBuildDefinitions();
            }
        }

        private void UpdateBuildDefinitions()
        {
            if (this.SelectedTeamProject == null)
            {
                return;
            }

#if !DEBUG
            try
            {
#endif
                using (new WaitCursor())
                {
                    if (this.dispatcherTimer.IsEnabled)
                    {
                        this.dispatcherTimer.Stop();
                    }

                    if (this.viewmodel.SelectedBuildView == BuildView.BuildDefinitions)
                    {
                        IEnumerable<IBuildDefinition> builds;
                        if (this.SelectedController == BuildManagerViewModel.AllItem)
                        {
                            builds = this.SelectedTeamProject == BuildManagerViewModel.AllItem ? this.repository.AllBuildDefinitions : this.repository.GetBuildDefinitionsForTeamProject(this.SelectedTeamProject);
                        }
                        else
                        {
                            IBuildController controller = this.repository.GetController(this.SelectedController);
                            builds = this.SelectedTeamProject == BuildManagerViewModel.AllItem ? this.repository.GetBuildDefinitions(controller) : this.repository.GetBuildDefinitions(controller, this.SelectedTeamProject);
                        }

                        builds = this.viewmodel.IncludeDisabledBuildDefinitions ? builds : builds.Where(b => b.QueueStatus != DefinitionQueueStatus.Disabled);
                        if (!string.IsNullOrWhiteSpace(this.viewmodel.BuildDefinitionFilter))
                        {
                            // if it starts with $/ then we seach on workspace mappings
                            if (this.viewmodel.BuildDefinitionFilter.StartsWith(@"$/", StringComparison.OrdinalIgnoreCase))
                            {
                                if (!string.IsNullOrWhiteSpace(this.viewmodel.BuildDefinitionFilter))
                                {
                                    List<IBuildDefinition> builds2 = new List<IBuildDefinition>();
                                    var filter = this.viewmodel.BuildDefinitionFilter;
                                    builds2.AddRange(builds.Where(b => b.Workspace.Mappings.Any(mapping => mapping.ServerItem.StartsWith(filter, StringComparison.OrdinalIgnoreCase))));
                                    var buildDefinitions = builds2.ToArray();
                                    this.viewmodel.AssignBuildDefinitions(buildDefinitions);
                                    this.lblCount.Content = buildDefinitions.Count();
                                }
                            }
                            else if (this.viewmodel.BuildDefinitionFilter.StartsWith(@"*", StringComparison.OrdinalIgnoreCase))
                            {
                                // if it starts with * then we are going to search Process Parameters
                                if (this.viewmodel.BuildDefinitionFilter == "*" || this.viewmodel.BuildDefinitionFilter == "**")
                                {
                                    return;
                                }

                                List<IBuildDefinition> builds2 = new List<IBuildDefinition>();
                                foreach (var b in builds)
                                {
                                    var processParameters = WorkflowHelpers.DeserializeProcessParameters(b.ProcessParameters);

                                    // if it starts with ** we do a contains, otherwise it starts with * and we do a compare.
                                    if (this.viewmodel.BuildDefinitionFilter.StartsWith(@"**", StringComparison.OrdinalIgnoreCase))
                                    {
                                        bool found = false;
                                        foreach (var parameter in processParameters)
                                        {
                                            // some process parameters are arrays so we need to look at each key value pair in the arrays
                                            if (parameter.Value.GetType().GetElementType() == typeof(string))
                                            {
                                                string[] arr = ((IEnumerable)parameter.Value).Cast<object>()
                                                    .Select(x => x.ToString())
                                                    .ToArray();

                                                if (arr.Any(s => s.IndexOf(this.viewmodel.BuildDefinitionFilter.Replace("*", string.Empty), StringComparison.OrdinalIgnoreCase) >= 0))
                                                {
                                                    builds2.Add(b);
                                                    found = true;
                                                }
                                            }
                                            else
                                            {
                                                if (parameter.Value.ToString().IndexOf(this.viewmodel.BuildDefinitionFilter.Replace("*", string.Empty), StringComparison.OrdinalIgnoreCase) >= 0)
                                                {
                                                    builds2.Add(b);
                                                    break;
                                                }
                                            }

                                            if (found)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        bool found = false;
                                        foreach (var parameter in processParameters)
                                        {
                                            if (parameter.Value.GetType().GetElementType() == typeof(string))
                                            {
                                                string[] arr = ((IEnumerable)parameter.Value).Cast<object>()
                                                    .Select(x => x.ToString())
                                                    .ToArray();

                                                if (arr.Any(s => string.Compare(s, this.viewmodel.BuildDefinitionFilter.Replace("*", string.Empty), StringComparison.OrdinalIgnoreCase) == 0))
                                                {
                                                    builds2.Add(b);
                                                    found = true;
                                                }
                                            }
                                            else
                                            {
                                                if (string.Compare(parameter.Value.ToString(), this.viewmodel.BuildDefinitionFilter.Replace("*", string.Empty), StringComparison.OrdinalIgnoreCase) == 0)
                                                {
                                                    builds2.Add(b);
                                                    break;
                                                }
                                            }

                                            if (found)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }

                                var buildDefinitions = builds2.ToArray();
                                this.viewmodel.AssignBuildDefinitions(buildDefinitions);
                                this.lblCount.Content = buildDefinitions.Count();
                            }
                            else 
                            {
                                var filter = this.viewmodel.BuildDefinitionFilter.ToUpperInvariant();
                                builds = builds.Where(b => b.Name.ToUpperInvariant().Contains(filter)).ToArray();
                                var buildDefinitions = builds as IBuildDefinition[];
                                this.viewmodel.AssignBuildDefinitions(buildDefinitions);
                                this.lblCount.Content = buildDefinitions.Count();
                            }
                        }
                        else
                        {
                            var buildDefinitions = builds as IBuildDefinition[] ?? builds.ToArray();
                            this.viewmodel.AssignBuildDefinitions(buildDefinitions);
                            this.lblCount.Content = buildDefinitions.Count();
                        }
                    }
                    else if (this.viewmodel.SelectedBuildView == BuildView.Builds)
                    {
                        this.UpdateBuilds();
                    }
                    else if (this.viewmodel.SelectedBuildView == BuildView.BuildProcessTemplates)
                    {
                        this.UpdateBuildProcessTemplates();
                    }
                    else if (this.viewmodel.SelectedBuildView == BuildView.BuildResources)
                    {
                        this.UpdateBuildResources();
                    }
                }
#if !DEBUG
            }
            catch (Exception ex)
            {
                this.DisplayError(ex);
            }
#endif
        }

        private void UpdateBuildProcessTemplates()
        {
            IEnumerable<IProcessTemplate> templates = this.SelectedTeamProject == BuildManagerViewModel.AllItem ? this.repository.GetBuildProcessTemplates() : this.repository.GetBuildProcessTemplates(this.SelectedTeamProject);
            this.viewmodel.AssignBuildProcessTemplates(templates);
        }

        private void UpdateBuildResources()
        {
            this.viewmodel.AssignBuildResources(this.repository.Controllers);
        }

        private void UpdateBuilds()
        {
            if (!this.dispatcherTimer.IsEnabled && this.CheckBoxAutoRefresh.IsChecked == true)
            {
                this.dispatcherTimer.Start();
            }

            BuildResourceFilter filter = this.CreateBuildResourceFilter();

            if (this.viewmodel.SelectedBuildFilter == BuildFilter.Completed)
            {
                var dateFilter = this.CompletedBuildsDateFilter.SelectedItem as BuildDateFilter;
                if (dateFilter != null)
                {
                    this.viewmodel.AssignBuilds(this.repository.GetCompletedBuilds(filter, dateFilter.TimeSpan));
                }
            }
            else
            {
                this.viewmodel.AssignBuilds(this.repository.GetQueuedBuilds(filter));
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!this.initialized)
            {
                this.InitializeMainView();
                this.initialized = true;
            }
        }

        private void InitializeMainView()
        {
            using (new WaitCursor())
            {
                if (this.repository != null && this.context != null && this.context.SelectedProject != null)
                {
                    var controllers = this.repository.Controllers;
                    var teamProjects = this.repository.AllTeamProjects;
                    this.viewmodel = new BuildManagerViewModel(Window.GetWindow(this), this.repository, this, controllers, teamProjects, this.context);

                    this.DataContext = this.viewmodel;
                    this.RunningBuilds.DataContext = this.viewmodel;
                    this.CompletedBuilds.DataContext = this.viewmodel;
                    this.ProcessTemplateGrid.DataContext = this.viewmodel;
                    this.SelectedTeamProject = this.context.SelectedProject;
                    this.SelectedController = BuildManagerViewModel.AllItem;

                    this.ControllerCombo.SelectionChanged += this.OnControllerChanged;
                    this.TeamProjectCombo.SelectionChanged += this.OnTeamProjectChanged;
                    this.viewmodel.Refresh += this.OnRefresh;
                    this.viewmodel.PropertyChanged += this.OnPropertyChanged;
                    this.viewmodel.SelectedBuildView = BuildView.BuildDefinitions;
                    this.UpdateBuildDefinitions();
                }
            }
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                switch (e.PropertyName)
                {
                    case "SelectedBuildFilter":
                        using (new WaitCursor())
                        {
                            this.UpdateBuilds();
                        }

                        break;
                    case "BuildDefinitionFilter":
                    case "includeDisabledBuildDefinitions":
                    case "SelectedBuildView":
                        using (new WaitCursor())
                        {
                            this.UpdateBuildDefinitions();
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                this.DisplayError(ex);
            }
        }

        private void OnTimerUpdate(object sender, EventArgs e)
        {
            try
            {
                if (this.viewmodel != null)
                {
                    this.dispatcherTimer.Stop();
                    if (this.viewmodel.SelectedBuildView == BuildView.Builds && this.viewmodel.SelectedBuildFilter == BuildFilter.Queued)
                    {
                        using (new WaitCursor())
                        {
                            this.UpdateBuilds();
                        }

                        if (this.CheckBoxAutoRefresh.IsChecked == true)
                        {
                            this.dispatcherTimer.Start();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.DisplayError(ex);
            }
        }

        private void BuildDefinitionList_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Delete)
            {
                this.viewmodel.OnDelete();
            }
        }

        private void CheckBoxAutoRefresh_OnChecked(object sender, RoutedEventArgs e)
        {
            if (this.initialized)
            {
                this.dispatcherTimer.Start();
            }
        }

        private void CheckBoxAutoRefresh_OnUnchecked(object sender, RoutedEventArgs e)
        {
            this.dispatcherTimer.Stop();
        }
    }
}
