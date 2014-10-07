//-----------------------------------------------------------------------
// <copyright file="BuildConfigurationSolution.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace WordDocumentGenerator.Client.Entities
{
    public class BuildConfigurationSolution
    {
        public string BuildRootNodeSolutionServerPath { get; set; }

        public string BuildRootNodeErrorCount { get; set; }

        public string BuildRootNodeWarningCount { get; set; }

        public string BuildRootNodeLogFile { get; set; }
    }
}