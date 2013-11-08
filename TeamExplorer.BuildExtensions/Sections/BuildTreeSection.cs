using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BuildTree.Models;
using BuildTree.Views;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Build.Controls;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.TeamFoundation.Build;

namespace BuildTree.Sections
{
    [TeamExplorerSection(BuildTreeSection.SectionId, TeamExplorerPageIds.Builds, 250)]
    public class BuildTreeSection : TeamExplorerBaseSection
    {
        public const string SectionId = "0C1852A7-0C9B-4D95-8893-02C60BEE271E";

        private VsTeamFoundationBuild _buildService;

        private ObservableCollection<BuildDefinitionViewModel> _builds = new ObservableCollection<BuildDefinitionViewModel>();
        public ObservableCollection<BuildDefinitionViewModel> Builds
        {
            get { return _builds; }
            protected set
            {
                this._builds = value;
                this.RaisePropertyChanged("Builds");
            }
        }

        public BuildDefinitionViewModel SelectedBuildDefinition { get; set; }

        protected BuildTreeView View
        {
            get { return this.SectionContent as BuildTreeView; }
        }

        public BuildTreeSection()
        {
            this.Title = "Build Tree";
            this.IsVisible = true;
            this.IsExpanded = true;
            this.IsBusy = false;
            this.SectionContent = new BuildTreeView();
            this.View.ParentSection = this;
        }

        public async override void Initialize(object sender, SectionInitializeEventArgs e)
        {
            base.Initialize(sender, e);
            _buildService = new VsTeamFoundationBuild();

            var sectionContext = e.Context as BuildsSectionContext;
            if (sectionContext != null)
            {
                this.Builds = sectionContext.Builds;
            }
            else
            {
                await this.RefreshAsync();
            }
        }

        public async override void Refresh()
        {
            base.Refresh();
            await this.RefreshAsync();
        }
        
        public override void SaveContext(object sender, SectionSaveContextEventArgs e)
        {
            base.SaveContext(sender, e);

            var context = new BuildsSectionContext {Builds = this.Builds};
            e.Context = context;
        }

        protected override async void ContextChanged(object sender, ContextChangedEventArgs e)
        {
            base.ContextChanged(sender, e);

            if (e.TeamProjectCollectionChanged || e.TeamProjectChanged)
            {
                await this.RefreshAsync();
            }
        }
        
        private async Task RefreshAsync()
        {
            try
            {
                this.IsBusy = true;
                this.Builds.Clear();

                var buildRefresh = new ObservableCollection<BuildDefinitionViewModel>();

                await Task.Run(() =>
                {
                    ITeamFoundationContext context = this.CurrentContext;
                    if (context != null && context.HasCollection && context.HasTeamProject)
                    {
                        IBuildServer buildServer = context.TeamProjectCollection.GetService<IBuildServer>();
                        if (buildServer != null)
                        {
                            var buildDefinitions = buildServer.QueryBuildDefinitions(context.TeamProjectName);
                            var builDefinitionTree = BuildDefinitionTreeBuilder.Build(buildDefinitions);
                            foreach (var rootNode in builDefinitionTree)
                            {
                                buildRefresh.Add(new BuildDefinitionViewModel(rootNode));
                            }
                        }
                    }
                });

                this.Builds = buildRefresh;
            }
            catch (Exception ex)
            {
                this.ShowNotification(ex.Message, NotificationType.Error);
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        public void EditBuildDefinition()
        {
            if (SelectedBuildDefinition == null || SelectedBuildDefinition.Definition == null) return;

            if (_buildService != null)
            {
                _buildService.OpenDefinition(SelectedBuildDefinition.Definition.Uri);
            }
        }

        public void ViewBuilds()
        {
            if (SelectedBuildDefinition == null || SelectedBuildDefinition.Definition == null) return;

            if (_buildService != null)
            {
                _buildService.BuildExplorer.CompletedView.Show(SelectedBuildDefinition.Definition.TeamProject, SelectedBuildDefinition.Definition.Name, String.Empty, DateFilter.Today);
            }
        }

        public void QueueNewBuild()
        {
            if (SelectedBuildDefinition == null || SelectedBuildDefinition.Definition == null) return;

            if (_buildService != null)
            {
                _buildService.DetailsManager.QueueBuild(SelectedBuildDefinition.Definition.TeamProject, SelectedBuildDefinition.Definition.Uri);
            }
        }
    }
}
