//-----------------------------------------------------------------------
// <copyright file="ExportedBuildDefinition.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System.Collections.Generic;
    using Microsoft.TeamFoundation.Build.Client;
    using Microsoft.TeamFoundation.Build.Common;

    internal class ExportedBuildDefinition
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ContinuousIntegrationType ContinuousIntegrationType { get; set; }

        public string BuildController { get; set; }

        public DefinitionQueueStatus QueueStatus { get; set; }

        public string ProcessTemplate { get; set; }

        public string DefaultDropLocation { get; set; }

        public AgentSettingsBuildParameter TfvcAgentSettings { get; set; }

        public BuildParameter GitAgentSettings { get; set; }
        
        public List<ExportedISchedule> Schedules { get; set; }

        public int ContinuousIntegrationQuietPeriod { get; set; }

        public List<ExportedIBuildDefinitionSourceProvider> SourceProviders { get; set; }

        public List<ExportedIWorkspaceMapping> Mappings { get; set; }

        public Dictionary<string, object> TestParameters { get; set; }

        public List<ExportedIRetentionPolicy> RetentionPolicyList { get; set; }

        public IDictionary<string, object> ProcessParameters { get; set; }
    }
}
