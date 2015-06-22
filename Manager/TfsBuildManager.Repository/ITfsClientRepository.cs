//-----------------------------------------------------------------------
// <copyright file="ITfsClientRepository.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Repository
{
    using System;
    using System.Collections.Generic;
    using Microsoft.TeamFoundation.Build.Client;

    public delegate bool EvaluateRemoveBuildProcessTemplate();

    public interface ITfsClientRepository
    {
        IEnumerable<IBuildController> Controllers { get; }

        IEnumerable<IBuildDefinition> AllBuildDefinitions { get; }

        IEnumerable<string> AllTeamProjects { get; }

        IEnumerable<IBuildDefinition> GetBuildDefinitions(IBuildController controller);

        IEnumerable<IBuildDefinition> GetBuildDefinitions(IBuildController controller, string teamProject);

        IEnumerable<IBuildDefinition> GetBuildDefinitionsForTeamProject(string teamProject);

        IBuildController GetController(string selectedController);

        IEnumerable<IProcessTemplate> GetBuildProcessTemplates();

        IEnumerable<IProcessTemplate> GetBuildProcessTemplates(string teamProject);

        IEnumerable<IProcessTemplate> GetBuildProcessTemplates(IEnumerable<string> teamProjects);

        IEnumerable<string> GetBuildQualities(IEnumerable<string> teamProjects);

        void SetBuildQualities(IEnumerable<Uri> builds, string buildQuality);

        IEnumerable<IQueuedBuild> GetQueuedBuilds(BuildResourceFilter filter);

        IEnumerable<IBuildDetail> GetCompletedBuilds(BuildResourceFilter filter, DateTime minFinishTime);

        void CleanDropsFolders(IEnumerable<Uri> buildDefinitions);

        void DeleteBuildDefinitions(IEnumerable<Uri> buildDefinitions, DeleteOptions deleteOptions);

        void GenerateBuildNotes(IEnumerable<IBuildDetail> buildDefinitions, IEnumerable<string> noteOptions);

        void DisableBuildDefinitions(IEnumerable<Uri> buildDefinitions);

        void PauseBuildDefinitions(IEnumerable<Uri> buildDefinitions);

        void EnableBuildDefinitions(IEnumerable<Uri> buildDefinitions);
        
        bool OpenDropFolder(IEnumerable<string> folders);

        void RetryBuilds(IEnumerable<Uri> builds);
        
        string GetBuildLogLocation(IBuildDetail build);

        void DeleteBuilds(IEnumerable<IBuildDetail> builds);

        void StopBuilds(IEnumerable<IQueuedBuild> builds);

        void ResumeBuilds(IEnumerable<IQueuedBuild> builds);

        void RetainIndefinitely(IEnumerable<IBuildDetail> builds);
        
        void SetRetentionPolicies(IEnumerable<Uri> buildDefinitions, BuildRetentionPolicy policies);

        void ChangeProcessParameter(IEnumerable<Uri> buildDefinitions, string[] parameter, bool booleanParameter);

        void AssignBuildProcessTemplate(IEnumerable<Uri> buildDefinitions, string serverPath);

        void ChangeBuildController(IEnumerable<Uri> buildDefinitions, string newController);

        void ChangeDefaultDropLocation(IEnumerable<Uri> buildDefinitions, string replacePath, string searchPath, bool replaceInExistingBuilds);

        void UpdateTrigger(IEnumerable<Uri> buildDefinitions, int minutes, int submissions, DefinitionTriggerType triggerType);

        void UpdateTrigger(IEnumerable<Uri> buildDefinitions, DefinitionTriggerType triggerType, ScheduleDays scheduleDays, DateTime scheduleTime, TimeZoneInfo timeZoneInfo);

        void SetDefaultDropLocation(IEnumerable<Uri> buildDefinitions, string newDropLocation, Dictionary<string, string> macros, bool replaceInExistingBuilds);

        void QueueBuilds(IEnumerable<Uri> buildDefinitions);

        void QueueHighBuilds(IEnumerable<Uri> buildDefinitions);

        string CloneBuildToProject(Uri buildDefinition, string newName, string targetProjectName);

        string CloneBuild(Uri buildDefinition, string newName, Branch source, Branch target);

        string CloneGitBuild(Uri buildDefinition, string newName, bool forceDisabled);

        IEnumerable<Branch> GetChildBranchObjectsForItem(string item);

        IBuildServer GetBuildServer();

        Branch GetBranchObjectForItem(string item);

        IEnumerable<string> GetProjectsToBuild(Uri buildDefinition);

        string GenerateBuildResourcesDependencyGraph();

        IBuildDefinition GetBuildDefinition(string teamProject, string newBuildDefinitionName);

        void SetDefaultBuildProcessTemplate(string serverPath, string teamProject);

        void AddBuildProcessTemplates(IEnumerable<string> templates, IEnumerable<string> teamProjects, bool setAsDefault);

        void RemoveBuildProcessTemplates(IEnumerable<string> templates, IEnumerable<string> teamProjects, EvaluateRemoveBuildProcessTemplate shouldRemove);

        void RemoveBuildAgent(IBuildAgent agent);

        void RemoveBuildController(IBuildController controller);

        void EnableAgent(IBuildAgent agent);

        void DisableAgent(IBuildAgent agent);

        void EnableController(IBuildController controller);

        void DisableController(IBuildController controller);

        VersionControlType GetVersionControlType(IBuildDefinition buildDefinition);

        void UpdateOutputLocation(IEnumerable<Uri> buildDefinitions, string location);

        void SetQueuedBuildPriority(IEnumerable<Uri> buildDefinitionUris, QueuePriority queuePriority);
    }
}
