//-----------------------------------------------------------------------
// <copyright file="ExportedAgileTestPlatformSpec.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------

using Microsoft.TeamFoundation.Build.Workflow.Activities;

namespace TfsBuildManager.Repository
{
    public class ExportedAgileTestPlatformSpec
    {
        public bool FailBuildOnFailure { get; set; }

        public string RunName { get; set; }

        public string AssemblyFileSpec { get; set; }

        public ExecutionPlatformType ExecutionPlatform { get; set; }

        public string RunSettingsFileName { get; set; }

        public RunSettingsType TypeRunSettings { get; set; }

        public string TestCaseFilter { get; set; }

        public static implicit operator ExportedAgileTestPlatformSpec(AgileTestPlatformSpec agilespec)
        {
            ExportedAgileTestPlatformSpec expAgileSpec = new ExportedAgileTestPlatformSpec();
            expAgileSpec.AssemblyFileSpec = agilespec.AssemblyFileSpec;
            expAgileSpec.ExecutionPlatform = agilespec.ExecutionPlatform;
            expAgileSpec.FailBuildOnFailure = agilespec.FailBuildOnFailure;
            expAgileSpec.RunName = agilespec.RunName;
            expAgileSpec.TestCaseFilter = agilespec.TestCaseFilter;
            expAgileSpec.RunSettingsFileName = agilespec.RunSettingsForTestRun.ServerRunSettingsFile;
            expAgileSpec.TypeRunSettings = agilespec.RunSettingsForTestRun.TypeRunSettings;
            return expAgileSpec;
        }

        public static implicit operator AgileTestPlatformSpec(ExportedAgileTestPlatformSpec exportedAgileTestPlatformSpec)
        {
            AgileTestPlatformSpec agileSpec = new AgileTestPlatformSpec();
            agileSpec.AssemblyFileSpec = exportedAgileTestPlatformSpec.AssemblyFileSpec;
            agileSpec.ExecutionPlatform = exportedAgileTestPlatformSpec.ExecutionPlatform;
            agileSpec.FailBuildOnFailure = exportedAgileTestPlatformSpec.FailBuildOnFailure;
            agileSpec.RunName = exportedAgileTestPlatformSpec.RunName;
            agileSpec.TestCaseFilter = exportedAgileTestPlatformSpec.TestCaseFilter;
            agileSpec.RunSettingsForTestRun = new RunSettings();
            agileSpec.RunSettingsForTestRun.ServerRunSettingsFile = exportedAgileTestPlatformSpec.RunSettingsFileName;
            agileSpec.RunSettingsForTestRun.TypeRunSettings = exportedAgileTestPlatformSpec.TypeRunSettings;
            return agileSpec;
        }
    }
}
