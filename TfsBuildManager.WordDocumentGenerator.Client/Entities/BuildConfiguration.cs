//-----------------------------------------------------------------------
// <copyright file="BuildConfiguration.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace WordDocumentGenerator.Client.Entities
{
    using System.Collections.Generic;

    public class BuildConfiguration
    {
        public string BuildConfigFlavor { get; set; }

        public string BuildConfigPlatform { get; set; }

        public string BuildConfigTotalErrors { get; set; }

        public string BuildConfigTotalWarnings { get; set; }
    }
}