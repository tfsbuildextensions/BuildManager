//-----------------------------------------------------------------------
// <copyright file="ExportedBuildDefinition.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System.Collections.Generic;
    using Microsoft.TeamFoundation.Build.Client;

    internal class ExportedBuildDefinition
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ContinuousIntegrationType ContinuousIntegrationType { get; set; }

        public string BuildController { get; set; }

        public string ProcessTemplate { get; set; }

        public string DefaultDropLocation { get; set; }
        
        public List<IBuildDefinitionSourceProvider> SourceProviders { get; set; }

        public List<IWorkspaceMapping> Mappings { get; set; }

        public Dictionary<string, object> TestParameters { get; set; }

        public List<IRetentionPolicy> RetentionPolicyList { get; set; }


        public IDictionary<string, object> ProcessParameters { get; set; }
    }
}
