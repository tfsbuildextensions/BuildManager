//-----------------------------------------------------------------------
// <copyright file="TfsClientRepository.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.TeamFoundation.Build.Client;
    using Microsoft.TeamFoundation.Build.Workflow;
    using Microsoft.TeamFoundation.Build.Workflow.Activities;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.Framework.Client;
    using Microsoft.TeamFoundation.Server;
    using Microsoft.TeamFoundation.TestManagement.Client;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using WordDocumentGenerator.Client;

    /// <summary>
    /// The TFS client repository.
    /// </summary>
    public class TfsClientRepository : IDisposable, ITfsClientRepository
    {
        private const string ConfigurationFolderPath = "ConfigurationFolderPath";
        private const string BuildSettings = "BuildSettings";
        private const string TestSpecs = "TestSpecs";
        private readonly IBuildServer buildServer;
        private TfsTeamProjectCollection collection;
        private WorkItemStore workItemStore;

        public TfsClientRepository(TfsTeamProjectCollection collection)
        {
            this.collection = collection;
            this.buildServer = (IBuildServer)this.collection.GetService(typeof(IBuildServer));
        }

        public TeamFoundationIdentity AuthenticatedIdentity
        {
            get { return this.collection.AuthorizedIdentity; }
        }

        public IEnumerable<IBuildDefinition> AllBuildDefinitions
        {
            get
            {
                var buildDefinitions = new List<IBuildDefinition>();
                var teamProjects = this.AllTeamProjects;
                foreach (var tp in teamProjects)
                {
                    buildDefinitions.AddRange(this.GetBuildDefinitionsForTeamProject(tp));
                }

                return buildDefinitions;
            }
        }

        public IEnumerable<IBuildController> Controllers
        {
            get { return this.buildServer.QueryBuildControllers(); }
        }

        public IEnumerable<string> AllTeamProjects
        {
            get
            {
                var structService = this.collection.GetService<ICommonStructureService>();

                foreach (var p in structService.ListAllProjects())
                {
                    yield return p.Name;
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEnumerable<IBuildDefinition> GetBuildDefinitionsForTeamProject(string teamProject)
        {
            return this.buildServer.QueryBuildDefinitions(teamProject);
        }

        public IBuildController GetController(string selectedController)
        {
            return this.buildServer.GetBuildController(selectedController);
        }

        public IBuildServer GetBuildServer()
        {
            return (IBuildServer)this.collection.GetService(typeof(IBuildServer));
        }

        public IEnumerable<IBuildDefinition> GetBuildDefinitions(IBuildController controller)
        {
            return this.AllBuildDefinitions.Where(bd => bd.BuildControllerUri == controller.Uri);
        }

        public IEnumerable<IBuildDefinition> GetBuildDefinitions(IBuildController controller, string teamProject)
        {
            return this.GetBuildDefinitionsForTeamProject(teamProject).Where(b => b.BuildController != null && b.BuildController.Uri == controller.Uri);
        }

        public IEnumerable<IProcessTemplate> GetBuildProcessTemplates()
        {
            var allBuildProcessTemplates = new List<IProcessTemplate>();

            foreach (var project in this.AllTeamProjects)
            {
                allBuildProcessTemplates.AddRange(this.buildServer.QueryProcessTemplates(project).AsEnumerable());
            }

            return allBuildProcessTemplates;
        }

        public IEnumerable<IQueuedBuild> GetQueuedBuilds(BuildResourceFilter filter)
        {
            IQueuedBuildSpec spec;
            if (string.IsNullOrEmpty(filter.Controller) && string.IsNullOrEmpty(filter.TeamProject))
            {
                spec = this.buildServer.CreateBuildQueueSpec("*");
            }
            else if (!string.IsNullOrEmpty(filter.Controller) && string.IsNullOrEmpty(filter.TeamProject))
            {
                var definitions = this.GetBuildDefinitions(this.GetController(filter.Controller));
                spec = this.buildServer.CreateBuildQueueSpec(definitions.Select(d => d.Uri));
            }
            else if (string.IsNullOrEmpty(filter.Controller) && !string.IsNullOrEmpty(filter.TeamProject))
            {
                spec = this.buildServer.CreateBuildQueueSpec(filter.TeamProject);
            }
            else
            {
                var definitions = this.GetBuildDefinitions(this.GetController(filter.Controller), filter.TeamProject);
                spec = this.buildServer.CreateBuildQueueSpec(definitions.Select(d => d.Uri));
            }

            if (spec.DefinitionSpec != null)
            {
                spec.DefinitionSpec.PropertyNameFilters = null;
            }

            spec.QueryOptions = QueryOptions.Definitions | QueryOptions.Controllers | QueryOptions.Agents | QueryOptions.BatchedRequests;
            spec.Status = QueueStatus.InProgress | QueueStatus.Queued | QueueStatus.Postponed;
            return this.buildServer.QueryQueuedBuilds(spec).QueuedBuilds.AsEnumerable();
        }

        public IEnumerable<IBuildDetail> GetCompletedBuilds(BuildResourceFilter filter, DateTime minFinishTime)
        {
            var result = new List<IBuildDetail>();
            IBuildDetailSpec spec;
            if (string.IsNullOrEmpty(filter.Controller) && string.IsNullOrEmpty(filter.TeamProject))
            {
                spec = this.buildServer.CreateBuildDetailSpec("*");
            }
            else if (!string.IsNullOrEmpty(filter.Controller) && string.IsNullOrEmpty(filter.TeamProject))
            {
                var buildDefinitionUris = this.GetBuildDefinitions(this.GetController(filter.Controller)).Select(bd => bd.Uri).ToList();
                if (!buildDefinitionUris.Any())
                {
                    return result;
                }

                spec = this.buildServer.CreateBuildDetailSpec(buildDefinitionUris);
            }
            else if (string.IsNullOrEmpty(filter.Controller) && !string.IsNullOrEmpty(filter.TeamProject))
            {
                spec = this.buildServer.CreateBuildDetailSpec(filter.TeamProject);
            }
            else
            {
                var buildDefinitionUris = this.GetBuildDefinitions(this.GetController(filter.Controller), filter.TeamProject).Select(bd => bd.Uri).ToList();
                if (!buildDefinitionUris.Any())
                {
                    return result;
                }

                spec = this.buildServer.CreateBuildDetailSpec(buildDefinitionUris);
            }

            spec.InformationTypes = new[] { Microsoft.TeamFoundation.Build.Common.InformationTypes.AgentScopeActivityTracking };
            spec.MaxBuildsPerDefinition = 100;
            spec.Status = BuildStatus.Succeeded | BuildStatus.Stopped | BuildStatus.PartiallySucceeded | BuildStatus.Failed;
            spec.QueryOrder = BuildQueryOrder.FinishTimeDescending;
            spec.MinFinishTime = minFinishTime;
            spec.QueryOptions = QueryOptions.Definitions | QueryOptions.Controllers | QueryOptions.Agents | QueryOptions.BatchedRequests;
            return this.buildServer.QueryBuilds(spec).Builds;
        }

        public void DisableBuildDefinitions(IEnumerable<Uri> buildDefinitions)
        {
            foreach (var buildUri in buildDefinitions)
            {
                var bd = this.buildServer.GetBuildDefinition(buildUri);
                bd.QueueStatus = DefinitionQueueStatus.Disabled;
                bd.Save();
            }
        }

        public void PauseBuildDefinitions(IEnumerable<Uri> buildDefinitions)
        {
            foreach (var buildUri in buildDefinitions)
            {
                var bd = this.buildServer.GetBuildDefinition(buildUri);
                bd.QueueStatus = DefinitionQueueStatus.Paused;
                bd.Save();
            }
        }

        public void SetQueuedBuildPriority(IEnumerable<Uri> buildDefinitionUris, QueuePriority queuePriority)
        {
            var queuedBuildSpec = this.buildServer.CreateBuildQueueSpec(buildDefinitionUris);
            foreach (var build in this.buildServer.QueryQueuedBuilds(queuedBuildSpec).QueuedBuilds)
            {
                build.Priority = queuePriority;
                build.Save();
            }
        }

        public bool OpenDropFolder(IEnumerable<string> folders)
        {
            bool dropFolderFound = false;
            foreach (var folder in folders.Where(Directory.Exists))
            {
                System.Diagnostics.Process.Start(folder);
                dropFolderFound = true;
            }

            return dropFolderFound;
        }

        public void RetryBuilds(IEnumerable<Uri> builds)
        {
            var definitions = this.buildServer.QueryBuildDefinitionsByUri(builds.ToArray());
            foreach (var bd in definitions.Where(d => d.Process != null))
            {
                this.buildServer.QueueBuild(bd);
            }
        }

        public string GetBuildLogLocation(IBuildDetail build)
        {
            var linkService = (TswaClientHyperlinkService)this.collection.GetService(typeof(TswaClientHyperlinkService));
            var url = linkService.GetViewBuildDetailsUrl(build.Uri);
            return url.ToString();
        }

        public void DeleteBuilds(IEnumerable<IBuildDetail> builds)
        {
            foreach (var build in builds)
            {
                build.Delete(DeleteOptions.All);
            }
        }

        public void StopBuilds(IEnumerable<IQueuedBuild> builds)
        {
            foreach (var build in builds)
            {
                if (build.Status != QueueStatus.InProgress)
                {
                    build.Cancel();
                }
                else if (build.Status == QueueStatus.InProgress)
                {
                    this.buildServer.StopBuilds(new[] { build.Build.Uri });
                }
            }
        }

        public void ResumeBuilds(IEnumerable<IQueuedBuild> builds)
        {
            foreach (var build in builds)
            {
                if (build.Status == QueueStatus.Postponed)
                {
                    build.Resume();
                    build.Save();
                }
            }
        }

        public void RetainIndefinitely(IEnumerable<IBuildDetail> builds)
        {
            foreach (var build in builds)
            {
                build.KeepForever = !build.KeepForever;
                build.Save();
            }
        }

        public void EnableBuildDefinitions(IEnumerable<Uri> buildDefinitions)
        {
            foreach (var buildUri in buildDefinitions)
            {
                var bd = this.buildServer.GetBuildDefinition(buildUri);
                bd.QueueStatus = DefinitionQueueStatus.Enabled;
                bd.Save();
            }
        }

        public void CleanDropsFolders(IEnumerable<Uri> buildDefinitions)
        {
            var buildDefinitionUris = buildDefinitions.ToArray();
            foreach (var buildDefinition in this.buildServer.QueryBuildDefinitionsByUri(buildDefinitionUris))
            {
                IBuildDetailSpec spec = this.buildServer.CreateBuildDetailSpec(buildDefinition);
                spec.QueryDeletedOption = QueryDeletedOption.OnlyDeleted;
                var builds = this.buildServer.QueryBuilds(spec).Builds;
                var taskArray = new Collection<Task>();
                foreach (var build in builds)
                {   
                    var buildLocation = build.DropLocation;
                    if (!string.IsNullOrWhiteSpace(buildLocation))
                    {
                        taskArray.Add(Task.Factory.StartNew(() => 
                                    { 
                                        if (Directory.Exists(buildLocation))
                                        {
                                            Directory.Delete(buildLocation, true);
                                        }
                                    }));
                    }
                }

                Task.WaitAll(taskArray.ToArray());
            }
        }

        public void DeleteBuildDefinitions(IEnumerable<Uri> buildDefinitions, DeleteOptions deleteOptions)
        {
            Uri[] buildDefinitionUris = buildDefinitions.ToArray();
            var bds = this.buildServer.QueryBuildDefinitionsByUri(buildDefinitionUris);
            foreach (var bd in bds)
            {
                var builds = bd.QueryBuilds();
                this.buildServer.DeleteBuilds(builds, deleteOptions);
            }

            this.buildServer.DeleteBuildDefinitions(buildDefinitionUris);
        }

        public void GenerateBuildNotes(IEnumerable<IBuildDetail> buildDefinitions, IEnumerable<string> noteOptions)
        {
            // Get the complete build information for the selected builds
            var b = buildDefinitions.ToArray();
            var buildDetails = new IBuildDetail[b.Count()];
            for (var i = 0; i < b.Count(); i++)
            {
                var build = b[i];
                buildDetails[i] = this.buildServer.GetBuild(build.Uri);
            }

            var exclusions =
                Enum.GetValues(typeof(BuildNoteOptions)).Cast<BuildNoteOptions>().Where(
                    c => !noteOptions.Contains(c.ToString())).Select(e => e.ToString()).ToList();

            this.workItemStore = this.collection.GetService<WorkItemStore>();
            var vcs = this.collection.GetService<VersionControlServer>();
            var tms = this.collection.GetService<TestManagementService>();

            var wordDocumentGenerator = new Program(buildDetails, noteOptions, exclusions, this.workItemStore, vcs, tms);
            wordDocumentGenerator.GenerateBuildNotesUsingBuildTemplate();
        }

        public IEnumerable<IProcessTemplate> GetBuildProcessTemplates(string teamProject)
        {
            return this.buildServer.QueryProcessTemplates(teamProject);
        }

        public IEnumerable<IProcessTemplate> GetBuildProcessTemplates(IEnumerable<string> teamProjects)
        {
            var buildProcessTemplates = new List<IProcessTemplate>();
            foreach (var teamProject in teamProjects)
            {
                buildProcessTemplates.AddRange(this.GetBuildProcessTemplates(teamProject));
            }

            return buildProcessTemplates;
        }

        public void AssignBuildProcessTemplate(IEnumerable<Uri> buildDefinitions, string serverPath)
        {
            foreach (var bd in this.buildServer.QueryBuildDefinitionsByUri(buildDefinitions.ToArray()))
            {
                var newProcessTemplate = this.buildServer.QueryProcessTemplates(bd.TeamProject).FirstOrDefault(pt => pt.ServerPath == serverPath) ??
                    this.buildServer.CreateProcessTemplate(bd.TeamProject, serverPath);

                if (bd.Process == null || string.Compare(bd.Process.ServerPath, newProcessTemplate.ServerPath, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    bd.Process = newProcessTemplate;
                    bd.Save();
                }
            }
        }

        public void ChangeBuildController(IEnumerable<Uri> buildDefinitions, string newController)
        {
            var controller = this.buildServer.GetBuildController(newController);

            foreach (var bd in this.buildServer.QueryBuildDefinitionsByUri(buildDefinitions.ToArray()))
            {
                bd.BuildController = controller;
                bd.Save();
            }
        }

        public void ChangeDefaultDropLocation(IEnumerable<Uri> buildDefinitions, string replacePath, string searchPath, bool replaceInExistingBuilds)
        {
            foreach (var bd in this.buildServer.QueryBuildDefinitionsByUri(buildDefinitions.ToArray()))
            {
                bd.DefaultDropLocation = CaseInsensitiveReplace(bd.DefaultDropLocation, searchPath, replacePath);
                bd.Save();

                if (replaceInExistingBuilds)
                {
                    this.ChangeDropLocationForExistingBuilds(replacePath, searchPath, bd);
                }
            }
        }

        public void UpdateTrigger(IEnumerable<Uri> buildDefinitions, int minutes, int submissions, DefinitionTriggerType triggerType)
        {
            foreach (var bd in this.buildServer.QueryBuildDefinitionsByUri(buildDefinitions.ToArray()))
            {
                bd.TriggerType = triggerType;
                if (bd.TriggerType == DefinitionTriggerType.BatchedContinuousIntegration)
                {
                    bd.ContinuousIntegrationQuietPeriod = minutes;
                }

                if (bd.TriggerType == DefinitionTriggerType.BatchedGatedCheckIn)
                {
                    bd.BatchSize = submissions;
                }
                
                bd.Save();
            }
        }

        public void UpdateTrigger(IEnumerable<Uri> buildDefinitions, DefinitionTriggerType triggerType, ScheduleDays scheduleDays, DateTime scheduleTime, TimeZoneInfo timeZoneInfo)
        {
            foreach (var bd in this.buildServer.QueryBuildDefinitionsByUri(buildDefinitions.ToArray()))
            {
                bd.TriggerType = triggerType;

                if (bd.Schedules.Any())
                {
                    bd.Schedules.Clear();
                }

                var schedule = bd.AddSchedule();
                schedule.DaysToBuild = scheduleDays;
                schedule.StartTime = (int)scheduleTime.TimeOfDay.TotalSeconds;
                schedule.TimeZone = timeZoneInfo;

                bd.Save();
            }
        }

        public void SetDefaultDropLocation(IEnumerable<Uri> buildDefinitions, string newDropLocation, Dictionary<string, string> macros, bool replaceInExistingBuilds)
        {
            foreach (var bd in this.buildServer.QueryBuildDefinitionsByUri(buildDefinitions.ToArray()))
            {
                macros["$(TeamProject)"] = bd.TeamProject;
                macros["$(BuildDefinition)"] = bd.Name;
                macros["$(BuildServer)"] = bd.BuildController != null ? bd.BuildController.Name : "Undefined buildcontroller";
                macros["$(BuildType)"] = bd.ContinuousIntegrationType.ToString();
                string expandedDropLocation = ExpandMacros(newDropLocation, macros);
                string oldDrop = bd.DefaultDropLocation;

                bd.DefaultDropLocation = expandedDropLocation;
                bd.Save();

                if (replaceInExistingBuilds)
                {
                    this.ChangeDropLocationForExistingBuilds(oldDrop, expandedDropLocation, bd);
                }
            }
        }

        public void QueueBuilds(IEnumerable<Uri> buildDefinitions)
        {
            var definitions = this.buildServer.QueryBuildDefinitionsByUri(buildDefinitions.ToArray());
            foreach (var bd in definitions.Where(d => d.Process != null))
            {
                this.buildServer.QueueBuild(bd);
            }
        }

        public void QueueHighBuilds(IEnumerable<Uri> buildDefinitions)
        {
            var definitions = this.buildServer.QueryBuildDefinitionsByUri(buildDefinitions.ToArray());
            foreach (var bd in definitions.Where(d => d.Process != null))
            {
                IBuildRequest buildRequest = bd.CreateBuildRequest();
                buildRequest.Priority = QueuePriority.High;
                this.buildServer.QueueBuild(buildRequest);
            }
        }

        public void SetRetentionPolicies(IEnumerable<Uri> buildDefinitions, BuildRetentionPolicy policies)
        {
            const BuildReason Reason = BuildReason.Triggered | BuildReason.Manual;
            foreach (var bd in this.buildServer.QueryBuildDefinitionsByUri(buildDefinitions.ToArray()))
            {
                UpdateRetentionPolicy(bd, Reason, policies.StoppedKeep, policies.StoppedDeleteOptions, BuildStatus.Stopped);
                UpdateRetentionPolicy(bd, Reason, policies.FailedKeep, policies.FailedDeleteOptions, BuildStatus.Failed);
                UpdateRetentionPolicy(bd, Reason, policies.PartiallySucceededKeep, policies.PartiallySucceededDeleteOptions, BuildStatus.PartiallySucceeded);
                UpdateRetentionPolicy(bd, Reason, policies.SucceededKeep, policies.SucceededDeleteOptions, BuildStatus.Succeeded);
                bd.Save();
            }
        }

        public void ChangeProcessParameter(IEnumerable<Uri> buildDefinitions, string[] parameter, bool booleanParameter)
        {
            foreach (var bd in this.buildServer.QueryBuildDefinitionsByUri(buildDefinitions.ToArray()))
            {
                var parameters = WorkflowHelpers.DeserializeProcessParameters(bd.ProcessParameters);
                if (parameters.ContainsKey(parameter[0]))
                {
                    if (booleanParameter)
                    {
                        parameters[parameter[0]] = Convert.ToBoolean(parameter[1]);
                    }
                    else
                    {
                        parameters[parameter[0]] = parameter[1];
                    }
                }
                else
                {
                    if (booleanParameter)
                    {
                        parameters.Add(parameter[0], Convert.ToBoolean(parameter[1]));
                    }
                    else
                    {
                        parameters.Add(parameter[0], parameter[1]);
                    }
                }

                bd.ProcessParameters = WorkflowHelpers.SerializeProcessParameters(parameters);
                bd.Save();
            }
        }
        
        public IEnumerable<string> GetProjectsToBuild(Uri buildDefinition)
        {
            var bd = this.buildServer.GetBuildDefinition(buildDefinition);
            var parameters = WorkflowHelpers.DeserializeProcessParameters(bd.ProcessParameters);

            if (parameters.ContainsKey(BuildSettings))
            {
                var buildSettings = parameters[BuildSettings] as BuildSettings;
                if (buildSettings == null)
                {
                    return new List<string>();
                }

                if (buildSettings.HasProjectsToBuild)
                {
                    return buildSettings.ProjectsToBuild;
                }
            }
            else if (parameters.ContainsKey("ProjectsToBuild"))
            {
                return parameters["ProjectsToBuild"] as string[];
            }

            return new List<string>();
        }

        public Branch GetBranchObjectForItem(string item)
        {
            var vcs = this.collection.GetService<VersionControlServer>();
            bool foundBranches = false;
            string lastItemWithBranches = item;

            while (vcs.QueryMergeRelationships(item).Length > 0)
            {
                foundBranches = true;
                lastItemWithBranches = item;
                int indexOfLastSlash = item.LastIndexOf('/');
                if (indexOfLastSlash >= 0)
                {
                    item = item.Substring(0, indexOfLastSlash);
                }
            }

            if (!foundBranches)
            {
                return null;
            }

            return new Branch { Name = lastItemWithBranches, ServerPath = lastItemWithBranches };
        }

        public IEnumerable<Branch> GetChildBranchObjectsForItem(string item)
        {
            List<Branch> childBranches = new List<Branch>();
            var branchObject = this.GetBranchObjectForItem(item);
            if (branchObject == null)
            {
                return childBranches;
            }

            var vcs = this.collection.GetService<VersionControlServer>();
            var relations = vcs.QueryMergeRelationships(item);
            childBranches.AddRange(relations.Select(r => new Branch { Name = r.Item, ServerPath = r.Item }));
            return childBranches;
        }

        public string CloneBuild(Uri buildDefinition, string newName, Branch source, Branch target)
        {
            string rootBranch = source.ServerPath;
            string targetBranch = target.ServerPath;

            var sourceBranchName = Path.GetFileName(rootBranch);
            var branchName = Path.GetFileName(targetBranch);
            var bd = this.buildServer.GetBuildDefinition(buildDefinition);
            var newBuildDefinition = this.buildServer.CreateBuildDefinition(bd.TeamProject);
            newBuildDefinition.Name = newName;
            newBuildDefinition.Description = bd.Description;
            newBuildDefinition.ContinuousIntegrationType = bd.ContinuousIntegrationType;
            newBuildDefinition.QueueStatus = bd.QueueStatus;
            CloneWorkspaceMappings(rootBranch, targetBranch, bd, newBuildDefinition);
            newBuildDefinition.BuildController = bd.BuildController;
            CloneDropLocation(sourceBranchName, branchName, bd, newBuildDefinition);
            CloneBuildSchedule(bd, newBuildDefinition);
            newBuildDefinition.ContinuousIntegrationQuietPeriod = bd.ContinuousIntegrationQuietPeriod;
            this.CloneBuildProcessTemplate(rootBranch, targetBranch, bd, newBuildDefinition);
            CloneRetentionPolicies(bd, newBuildDefinition);

            var parameters = WorkflowHelpers.DeserializeProcessParameters(bd.ProcessParameters);
            CloneStringParameters(rootBranch, targetBranch, parameters);
            CloneItemsToBuild(rootBranch, targetBranch, parameters);
            CloneConfigurationFolderPath(rootBranch, targetBranch, bd, parameters);
            CloneTestSpecs(rootBranch, targetBranch, parameters);
            newBuildDefinition.ProcessParameters = WorkflowHelpers.SerializeProcessParameters(parameters);

            newBuildDefinition.Save();
            return newBuildDefinition.ToString();
        }

        public string CloneGitBuild(Uri buildDefinition, string newName, bool forceDisabled)
        {
            var bd = this.buildServer.GetBuildDefinition(buildDefinition);
            var newBuildDefinition = this.buildServer.CreateBuildDefinition(bd.TeamProject);
            newBuildDefinition.Name = newName;
            newBuildDefinition.Description = bd.Description;
            newBuildDefinition.ContinuousIntegrationType = bd.ContinuousIntegrationType;
            newBuildDefinition.QueueStatus = forceDisabled ? DefinitionQueueStatus.Disabled : bd.QueueStatus;
            newBuildDefinition.BuildController = bd.BuildController;
            newBuildDefinition.ProcessParameters = bd.ProcessParameters;
            newBuildDefinition.DefaultDropLocation = bd.DefaultDropLocation;

            var provider = CloneSourceProviders(newBuildDefinition, bd);

            newBuildDefinition.SetSourceProvider(provider);

            CloneBuildSchedule(bd, newBuildDefinition);
            newBuildDefinition.ContinuousIntegrationQuietPeriod = bd.ContinuousIntegrationQuietPeriod;
            newBuildDefinition.Process = bd.Process;
            CloneRetentionPolicies(bd, newBuildDefinition);

            var parameters = WorkflowHelpers.DeserializeProcessParameters(bd.ProcessParameters);
            newBuildDefinition.ProcessParameters = WorkflowHelpers.SerializeProcessParameters(parameters);

            newBuildDefinition.Save();
            return newBuildDefinition.ToString();
        }

        public string CloneBuildToProject(Uri buildDefinition, string newName, string targetProjectName)
        {
            var bd = this.buildServer.GetBuildDefinition(buildDefinition);
            string sourceProjectName = bd.TeamProject;

            var newBuildDefinition = this.buildServer.CreateBuildDefinition(targetProjectName);
            newBuildDefinition.Name = newName;
            newBuildDefinition.Description = bd.Description;
            newBuildDefinition.ContinuousIntegrationType = bd.ContinuousIntegrationType;
            newBuildDefinition.QueueStatus = bd.QueueStatus;
            CloneWorkspaceMappings(string.Concat("$/", sourceProjectName), string.Concat("$/", targetProjectName), bd, newBuildDefinition);
            newBuildDefinition.BuildController = bd.BuildController;
            newBuildDefinition.ProcessParameters = bd.ProcessParameters;
            CloneDropLocation(sourceProjectName, targetProjectName, bd, newBuildDefinition, false);
            CloneBuildSchedule(bd, newBuildDefinition);
            newBuildDefinition.ContinuousIntegrationQuietPeriod = bd.ContinuousIntegrationQuietPeriod;
            newBuildDefinition.Process = this.EnsureProjectHasBuildProcessTemplate(targetProjectName, bd.Process.ServerPath);
            CloneRetentionPolicies(bd, newBuildDefinition);

            var parameters = WorkflowHelpers.DeserializeProcessParameters(bd.ProcessParameters);
            CloneStringParameters(sourceProjectName, targetProjectName, parameters);
            CloneItemsToBuild(sourceProjectName, targetProjectName, parameters);
            CloneConfigurationFolderPath(sourceProjectName, targetProjectName, bd, parameters);
            CloneTestSpecs(sourceProjectName, targetProjectName, parameters);
            newBuildDefinition.ProcessParameters = WorkflowHelpers.SerializeProcessParameters(parameters);

            newBuildDefinition.Save();
            return newBuildDefinition.ToString();
        }

        /// <summary>
        /// Note: Preliminary code for generating a DGML markup string that represents the build resource structure (controllers, agents, hosts).
        /// </summary>
        /// <returns>Generated DGML document</returns>
        public string GenerateBuildResourcesDependencyGraph()
        {
            DgmlGenerator generator = new DgmlGenerator(this.buildServer);
            DirectedGraph dg = generator.GenerateGraph();
            return dg.Serialize();
        }

        public IBuildDefinition GetBuildDefinition(string teamProject, string newBuildDefinitionName)
        {
            return this.GetBuildDefinitionsForTeamProject(teamProject).FirstOrDefault(b => b.Name == newBuildDefinitionName);
        }

        public void SetDefaultBuildProcessTemplate(string serverPath, string teamProject)
        {
            var buildProcessTemplates = this.buildServer.QueryProcessTemplates(teamProject).ToList();
            var template = buildProcessTemplates.FirstOrDefault(tp => tp.ServerPath == serverPath);
            if (template != null)
            {
                // Check for existing default template in team project
                var defaultTemplate = buildProcessTemplates.FirstOrDefault(tp => tp.TemplateType == ProcessTemplateType.Default);
                if (defaultTemplate != null)
                {
                    defaultTemplate.TemplateType = ProcessTemplateType.Custom;
                    defaultTemplate.Save();
                }

                template.TemplateType = ProcessTemplateType.Default;
                template.Save();
            }
        }

        public void AddBuildProcessTemplates(IEnumerable<string> templates, IEnumerable<string> teamProjects, bool setAsDefault)
        {
            foreach (var teamProject in teamProjects)
            {
                foreach (var template in templates.ToList())
                {
                    if (this.buildServer.QueryProcessTemplates(teamProject).All(pt => pt.ServerPath != template))
                    {
                        var t = this.buildServer.CreateProcessTemplate(teamProject, template);
                        t.Save();
                    }

                    if (setAsDefault)
                    {
                        this.SetDefaultBuildProcessTemplate(template, teamProject);
                    }
                }
            }
        }

        public IEnumerable<string> GetBuildQualities(IEnumerable<string> teamProjects)
        {
            var buildQualities = new List<string>();

            foreach (var teamProject in teamProjects)
            {
                buildQualities.AddRange(this.buildServer.GetBuildQualities(teamProject));
            }

            return buildQualities.Distinct();
        }

        public void SetBuildQualities(IEnumerable<Uri> builds, string buildQuality)
        {
            foreach (var build in this.buildServer.QueryBuildsByUri(builds.ToArray(), null, QueryOptions.None))
            {
                build.Quality = buildQuality;
                build.Save();
            }
        }

        public void RemoveBuildProcessTemplates(IEnumerable<string> templates, IEnumerable<string> teamProjects, EvaluateRemoveBuildProcessTemplate shouldRemove)
        {
            foreach (var teamProject in teamProjects)
            {
                foreach (var template in templates.ToList())
                {
                    if (this.buildServer.QueryBuildDefinitions(teamProject).Any(bd => bd.Process != null && bd.Process.ServerPath == template))
                    {
                        if (!shouldRemove())
                        {
                            return;
                        }
                    }

                    var t = this.buildServer.QueryProcessTemplates(teamProject).FirstOrDefault(pt => pt.ServerPath == template);
                    if (t != null)
                    {
                        if (t.TemplateType == ProcessTemplateType.Default)
                        {
                            t.TemplateType = ProcessTemplateType.Custom;
                            t.Save();
                        }

                        t.Delete();
                    }
                }
            }
        }

        public void RemoveBuildAgent(IBuildAgent agent)
        {
            this.buildServer.DeleteBuildAgents(new[] { agent.Uri });
        }

        public void RemoveBuildController(IBuildController controller)
        {
            this.buildServer.DeleteBuildAgents(controller.Agents.ToArray());
            this.buildServer.DeleteBuildControllers(new[] { controller.Uri });
        }

        public void EnableAgent(IBuildAgent agent)
        {
            agent.Enabled = true;
            agent.Save();
        }

        public void DisableAgent(IBuildAgent agent)
        {
            agent.Enabled = false;
            agent.Save();
        }

        public void EnableController(IBuildController controller)
        {
            controller.Enabled = true;
            controller.Save();
        }

        public void DisableController(IBuildController controller)
        {
            controller.Enabled = false;
            controller.Save();
        }

        public VersionControlType GetVersionControlType(IBuildDefinition buildDefinition)
        {
            if (buildDefinition.SourceProviders.Any(s => s.Name.ToUpperInvariant().Contains("GIT")))
            {
                return VersionControlType.Git;
            }

            return VersionControlType.Tfvc;
        }

        public void UpdateOutputLocation(IEnumerable<Uri> buildDefinitions, string location)
        {
            foreach (var bd in this.buildServer.QueryBuildDefinitionsByUri(buildDefinitions.ToArray()))
            {
                var parameters = WorkflowHelpers.DeserializeProcessParameters(bd.ProcessParameters);
                if (parameters.ContainsKey("OutputLocation"))
                {
                    parameters["OutputLocation"] = location;
                }
                else
                {
                    parameters.Add("OutputLocation", location);
                }

                bd.ProcessParameters = WorkflowHelpers.SerializeProcessParameters(parameters);
                bd.Save();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                if (this.collection != null)
                {
                    this.collection.Dispose();
                    this.collection = null;
                }
            }
        }

        private static void CloneRetentionPolicies(IBuildDefinition bd, IBuildDefinition newBuildDefinition)
        {
            newBuildDefinition.RetentionPolicyList.Clear();
            foreach (var retpol in bd.RetentionPolicyList)
            {
                newBuildDefinition.AddRetentionPolicy(retpol.BuildReason, retpol.BuildStatus, retpol.NumberToKeep, retpol.DeleteOptions);
            }
        }

        private static void CloneWorkspaceMappings(string rootBranch, string targetBranch, IBuildDefinition bd, IBuildDefinition newBuildDefinition)
        {
            var existingworkspace = bd.Workspace;
            foreach (var mapping in existingworkspace.Mappings)
            {
                string m = mapping.ServerItem;

                if (m.StartsWith(rootBranch, StringComparison.OrdinalIgnoreCase))
                {
                    m = m.Replace(rootBranch, targetBranch);
                }

                newBuildDefinition.Workspace.AddMapping(m, mapping.LocalItem, mapping.MappingType);
            }
        }

        private static void CloneBuildSchedule(IBuildDefinition bd, IBuildDefinition newBuildDefinition)
        {
            foreach (var sched in bd.Schedules)
            {
                var newSched = newBuildDefinition.AddSchedule();
                newSched.DaysToBuild = sched.DaysToBuild;
                newSched.StartTime = sched.StartTime;
                newSched.TimeZone = sched.TimeZone;
            }
        }

        private static void CloneTestSpecs(string rootBranch, string targetBranch, IDictionary<string, object> parameters)
        {
            if (parameters.ContainsKey(TestSpecs) && parameters[TestSpecs] != null)
            {
                var testSpecList = parameters[TestSpecs] as TestSpecList;
                if (testSpecList == null)
                {
                    return;
                }

                foreach (var spec in testSpecList)
                {
                    var assemblySpec = spec as TestAssemblySpec;
                    if (assemblySpec != null)
                    {
                        if (!string.IsNullOrEmpty(assemblySpec.TestSettingsFileName) && assemblySpec.TestSettingsFileName.StartsWith(rootBranch, StringComparison.OrdinalIgnoreCase))
                        {
                            assemblySpec.TestSettingsFileName = assemblySpec.TestSettingsFileName.Replace(rootBranch, targetBranch);
                        }
                    }

                    var metadataFileSpec = spec as TestMetadataFileSpec;
                    if (metadataFileSpec != null)
                    {
                        var modifiedTestLists = new StringList();
                        foreach (var list in metadataFileSpec.TestLists)
                        {
                            string modifiedTestList = list;
                            if (modifiedTestList.StartsWith(rootBranch, StringComparison.OrdinalIgnoreCase))
                            {
                                modifiedTestList = modifiedTestList.Replace(rootBranch, targetBranch);
                            }

                            modifiedTestLists.Add(modifiedTestList);
                        }

                        if (!string.IsNullOrEmpty(metadataFileSpec.MetadataFileName) && metadataFileSpec.MetadataFileName.StartsWith(rootBranch, StringComparison.OrdinalIgnoreCase))
                        {
                            metadataFileSpec.MetadataFileName = metadataFileSpec.MetadataFileName.Replace(rootBranch, targetBranch);
                        }
                    }
                }
            }
        }

        private static void CloneConfigurationFolderPath(string rootBranch, string targetBranch, IBuildDefinition bd, IDictionary<string, object> parameters)
        {
            if (bd.Process.TemplateType == ProcessTemplateType.Upgrade && parameters.ContainsKey(ConfigurationFolderPath) && parameters[ConfigurationFolderPath] != null)
            {
                var configurationFolderPath = parameters[ConfigurationFolderPath].ToString();
                if (configurationFolderPath.StartsWith(rootBranch, StringComparison.OrdinalIgnoreCase))
                {
                    parameters[ConfigurationFolderPath] = configurationFolderPath.Replace(rootBranch, targetBranch);
                }
            }
        }

        private static void CloneItemsToBuild(string sourceName, string targetName, IDictionary<string, object> parameters)
        {
            if (parameters.ContainsKey(BuildSettings))
            {
                CloneBuildSettings(sourceName, targetName, parameters);
            }
            else if (parameters.ContainsKey("ProjectsToBuild"))
            {
                CloneProjectsToBuild(sourceName, targetName, parameters);
            }
        }

        private static void CloneProjectsToBuild(string sourceName, string targetName, IDictionary<string, object> parameters)
        {
            string chkBranch = sourceName;
            string setBranch = targetName;
            if (!sourceName.StartsWith("$/", StringComparison.OrdinalIgnoreCase))
            {
                chkBranch = string.Concat("$/", sourceName);
            }

            if (!targetName.StartsWith("$/", StringComparison.OrdinalIgnoreCase))
            {
                setBranch = string.Concat("$/", targetName);
            }

            var projects = parameters["ProjectsToBuild"] as string[];
            if (projects != null)
            {
                for (int i = 0; i < projects.Count(); i++)
                {
                    if (projects[i].StartsWith(chkBranch, StringComparison.OrdinalIgnoreCase))
                    {
                        projects[i] = projects[i].Replace(chkBranch, setBranch);
                    }
                }
            }
        }

        private static void CloneBuildSettings(string sourceName, string targetName, IDictionary<string, object> parameters)
        {
            var buildSettings = parameters[BuildSettings] as BuildSettings;
            if (buildSettings == null || !buildSettings.HasProjectsToBuild)
            {
                return;
            }

            string chkBranch = sourceName;
            string setBranch = targetName;
            if (!sourceName.StartsWith("$/", StringComparison.OrdinalIgnoreCase))
            {
                chkBranch = string.Concat("$/", sourceName);
            }

            if (!targetName.StartsWith("$/", StringComparison.OrdinalIgnoreCase))
            {
                setBranch = string.Concat("$/", targetName);
            }

            for (int i = 0; i < buildSettings.ProjectsToBuild.Count(); i++)
            {
                if (buildSettings.ProjectsToBuild[i].StartsWith(chkBranch, StringComparison.OrdinalIgnoreCase))
                {
                    buildSettings.ProjectsToBuild[i] = buildSettings.ProjectsToBuild[i].Replace(chkBranch, setBranch);
                }
            }
        }

        private static void CloneStringParameters(string sourceName, string targetName, IDictionary<string, object> parameters)
        {
            List<string> keys = parameters.Keys.ToList();
            for (int idx = 0; idx < parameters.Count(); idx++)
            {
                if (parameters[keys[idx]] is string)
                {
                    if ((parameters[keys[idx]] as string).Contains(sourceName))
                    {
                        parameters[keys[idx]] = (parameters[keys[idx]] as string).Replace(sourceName, targetName);
                    }
                }
            }
        }

        private static void UpdateRetentionPolicy(IBuildDefinition bd, BuildReason reason, int keep, DeleteOptions options, BuildStatus status)
        {
            var rp = bd.RetentionPolicyList.FirstOrDefault(r => r.BuildReason == reason && r.BuildStatus == status);
            if (rp == null)
            {
                bd.AddRetentionPolicy(reason, BuildStatus.Stopped, keep, options);
            }
            else
            {
                rp.NumberToKeep = keep;
                rp.DeleteOptions = options;
            }
        }

        private static void CloneDropLocation(string rootBranchName, string targetBranchName, IBuildDefinition bd, IBuildDefinition newBuildDefinition, bool autoIncludeBranchName = true)
        {
            if (bd.DefaultDropLocation == null || string.IsNullOrWhiteSpace(bd.DefaultDropLocation))
            {
                return;
            }

            string dropLocation = bd.DefaultDropLocation.ToLower();
            if (dropLocation.Contains(rootBranchName.ToLower()))
            {
                newBuildDefinition.DefaultDropLocation = dropLocation.Replace(
                    rootBranchName.ToLower(), targetBranchName.ToLower());
            }
            else if (autoIncludeBranchName)
            {
                newBuildDefinition.DefaultDropLocation = Path.Combine(dropLocation, targetBranchName.ToLower());
            }
            else
            {
                newBuildDefinition.DefaultDropLocation = dropLocation;
            }
        }

        private static string ExpandMacros(string p, Dictionary<string, string> macros)
        {
            return macros.Keys.Aggregate(p, (current, macroName) => CaseInsensitiveReplace(current, macroName, macros[macroName]));
        }

        private static string CaseInsensitiveReplace(string org, string search, string replace)
        {
            search = search.ToUpper();
            int start = org.ToUpper().IndexOf(search, System.StringComparison.Ordinal);
            do
            {
                if (start >= 0)
                {
                    org = org.Substring(0, start) + replace + org.Substring(start + search.Length);
                }

                start = org.ToUpper().IndexOf(search, System.StringComparison.Ordinal);

                if (start == 0)
                {
                    // we have a recursive loop as we are starting at 0 again and have a match. Let's force an exit.
                    start = -1;
                }
            }
            while (start >= 0);

            return org;
        }

        private static IBuildDefinitionSourceProvider CloneSourceProviders(IBuildDefinition newBuildDefinition, IBuildDefinition bd)
        {
            var provider = newBuildDefinition.CreateInitialSourceProvider(bd.SourceProviders.First().Name);
            var originalProviders = bd.SourceProviders.First();
            foreach (var f in originalProviders.Fields)
            {
                provider.Fields[f.Key] = f.Value;
            }

            return provider;
        }

        private IProcessTemplate EnsureProjectHasBuildProcessTemplate(string teamProject, string templateServerPath)
        {
            IProcessTemplate template = this.buildServer.QueryProcessTemplates(teamProject).FirstOrDefault(pt => pt.ServerPath == templateServerPath);
            if (template == null)
            {
                template = this.buildServer.CreateProcessTemplate(teamProject, templateServerPath);
                template.Save();
            }

            return template;
        }

        private void ChangeDropLocationForExistingBuilds(string replacePath, string searchPath, IBuildDefinition bd)
        {
            foreach (var buildDetail in this.buildServer.QueryBuilds(bd))
            {
                if (buildDetail.DropLocation != null)
                {
                    buildDetail.DropLocation = Regex.Replace(buildDetail.DropLocation, Regex.Escape(searchPath), replacePath, RegexOptions.IgnoreCase);
                    buildDetail.Save();
                }
            }
        }

        private void CloneBuildProcessTemplate(string rootBranch, string targetBranch, IBuildDefinition bd, IBuildDefinition newBuildDefinition)
        {
            if (bd.Process.ServerPath.StartsWith(rootBranch, StringComparison.OrdinalIgnoreCase))
            {
                string targetBuildProcessTemplate = bd.Process.ServerPath.Replace(rootBranch, targetBranch);
                var targetTemplate = this.buildServer.QueryProcessTemplates(bd.TeamProject).Where(pt => pt.ServerPath == targetBuildProcessTemplate).ToList();
                newBuildDefinition.Process = targetTemplate.Count() == 1 ? targetTemplate.First() : bd.Process;
            }
            else
            {
                newBuildDefinition.Process = bd.Process;
            }
        }
    }

    public class Branch
    {
        public string ServerPath { get; set; }

        public string Name { get; set; }
    }
}
