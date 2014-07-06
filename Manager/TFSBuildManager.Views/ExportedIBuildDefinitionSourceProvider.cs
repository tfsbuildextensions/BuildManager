//-----------------------------------------------------------------------
// <copyright file="ExportedIBuildDefinitionSourceProvider.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System.Collections.Generic;
    using Microsoft.TeamFoundation.Build.Client;

    public class ExportedIBuildDefinitionSourceProvider : IBuildDefinitionSourceProvider
    {
        public string Name { get; set; }

        public DefinitionTriggerType SupportedTriggerTypes { get; set; }

        public IDictionary<string, string> Fields { get; set; }
    }
}
