//-----------------------------------------------------------------------
// <copyright file="AgentSettingsBuildParameter.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Repository
{
    using System;
    using Microsoft.TeamFoundation.Build.Workflow.Activities;

    public class AgentSettingsBuildParameter
    {
        public TagComparison Comparison { get; set; }

        public TimeSpan MaxWaitTime { get; set; }

        public TimeSpan MaxExecutionTime { get; set; }

        public string Name { get; set; } 

        public StringList Tags { get; set; }

        public static implicit operator AgentSettingsBuildParameter(AgentSettings ags)
        {
            AgentSettingsBuildParameter agentSet = new AgentSettingsBuildParameter();
            agentSet.MaxExecutionTime = ags.MaxExecutionTime;
            agentSet.MaxWaitTime = ags.MaxWaitTime;
            agentSet.Name = ags.Name;
            agentSet.Comparison = ags.TagComparison;
            agentSet.Tags = ags.Tags;
            return agentSet;
        }

        public static implicit operator AgentSettings(AgentSettingsBuildParameter agentSet)
        {
            return new AgentSettings { MaxExecutionTime = agentSet.MaxExecutionTime, MaxWaitTime = agentSet.MaxWaitTime, Name = agentSet.Name, TagComparison = agentSet.Comparison, Tags = agentSet.Tags };
        }
    }
}
