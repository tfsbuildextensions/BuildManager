//-----------------------------------------------------------------------
// <copyright file="AgentSettingsBuildParameter.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
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
    }
}
