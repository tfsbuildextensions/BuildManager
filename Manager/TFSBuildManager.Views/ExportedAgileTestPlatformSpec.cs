//-----------------------------------------------------------------------
// <copyright file="ExportedAgileTestPlatformSpec.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using Microsoft.TeamFoundation.Build.Workflow.Activities;

    public class ExportedAgileTestPlatformSpec
    {
        public bool FailBuildOnFailure { get; set; }

        public string RunName { get; set; }

        public string AssemblyFileSpec { get; set; }

        public ExecutionPlatformType ExecutionPlatform { get; set; }

        public string RunSettingsFileName { get; set; }

        public RunSettingsType TypeRunSettings { get; set; }

        public string TestCaseFilter { get; set; }
    }
}
