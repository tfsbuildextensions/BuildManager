//-----------------------------------------------------------------------
// <copyright file="ExportedMSTestSpec.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    public class ExportedMSTestSpec
    {
        public bool FailBuildOnFailure { get; set; }

        public string RunName { get; set; }

        public string CategoryFilter { get; set; }

        public int MaximumPriority { get; set; }

        public int MinimumPriority { get; set; }

        public string MSTestCommandLineArgs { get; set; }
    }
}
