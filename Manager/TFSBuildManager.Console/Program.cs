//-----------------------------------------------------------------------
// <copyright file="Program.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TFSBuildManager.Console
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.TeamFoundation.Build.Client;
    using Microsoft.TeamFoundation.Build.Common;
    using Microsoft.TeamFoundation.Build.Workflow;
    using Microsoft.TeamFoundation.Build.Workflow.Activities;
    using Newtonsoft.Json;
    using TfsBuildManager.Views;

    public class Program
    {
        static void Main(string[] args)
        {
        }

        private static void ExportDefinition(IBuildDefinition b, string filePath)
        {
            ExportedBuildDefinition buildToExport = new ExportedBuildDefinition { Name = b.Name, Description = b.Description };
            if (b.BuildController != null)
            {
                buildToExport.BuildController = b.BuildController.Name;
            }

            buildToExport.ContinuousIntegrationType = b.ContinuousIntegrationType;
            buildToExport.DefaultDropLocation = b.DefaultDropLocation;
            buildToExport.Schedules = new List<ExportedISchedule>();

            foreach (var schedule in b.Schedules)
            {
                buildToExport.Schedules.Add(new ExportedISchedule { StartTime = schedule.StartTime, DaysToBuild = schedule.DaysToBuild, TimeZone = schedule.TimeZone });
            }

            buildToExport.SourceProviders = new List<ExportedIBuildDefinitionSourceProvider>();
            foreach (var provider in b.SourceProviders)
            {
                buildToExport.SourceProviders.Add(new ExportedIBuildDefinitionSourceProvider { Fields = provider.Fields, Name = provider.Name, SupportedTriggerTypes = provider.SupportedTriggerTypes });
            }

            buildToExport.QueueStatus = b.QueueStatus;
            buildToExport.ContinuousIntegrationQuietPeriod = b.ContinuousIntegrationQuietPeriod;

            if (b.SourceProviders.All(s => s.Name != "TFGIT"))
            {
                buildToExport.Mappings = new List<ExportedIWorkspaceMapping>();
                foreach (var map in b.Workspace.Mappings)
                {
                    buildToExport.Mappings.Add(new ExportedIWorkspaceMapping { Depth = map.Depth, LocalItem = map.LocalItem, MappingType = map.MappingType, ServerItem = map.ServerItem });
                }
            }

            buildToExport.RetentionPolicyList = new List<ExportedIRetentionPolicy>();
            foreach (var rp in b.RetentionPolicyList)
            {
                buildToExport.RetentionPolicyList.Add(new ExportedIRetentionPolicy { BuildDefinition = rp.BuildDefinition, BuildReason = rp.BuildReason, BuildStatus = rp.BuildStatus, NumberToKeep = rp.NumberToKeep, DeleteOptions = rp.DeleteOptions });
            }

            if (b.Process != null)
            {
                buildToExport.ProcessTemplate = b.Process.ServerPath;
            }

            var processParameters = WorkflowHelpers.DeserializeProcessParameters(b.ProcessParameters);
            if (processParameters.ContainsKey("AgentSettings"))
            {
                if (processParameters["AgentSettings"].GetType() == typeof(AgentSettings))
                {
                    AgentSettings ags = (AgentSettings)processParameters["AgentSettings"];
                    AgentSettingsBuildParameter agentSet = new AgentSettingsBuildParameter();
                    agentSet.MaxExecutionTime = ags.MaxExecutionTime;
                    agentSet.MaxWaitTime = ags.MaxWaitTime;
                    agentSet.Name = ags.Name;
                    agentSet.Comparison = ags.TagComparison;
                    agentSet.Tags = ags.Tags;
                    buildToExport.TfvcAgentSettings = agentSet;
                }
                else if (processParameters["AgentSettings"].GetType() == typeof(BuildParameter))
                {
                    BuildParameter ags = (BuildParameter)processParameters["AgentSettings"];
                    {
                        buildToExport.GitAgentSettings = ags;
                    }
                }
            }

            buildToExport.ProcessParameters = WorkflowHelpers.DeserializeProcessParameters(b.ProcessParameters);

            File.WriteAllText(Path.Combine(filePath, b.Name + ".json"), JsonConvert.SerializeObject(buildToExport, Formatting.Indented));
        }
    }
}
