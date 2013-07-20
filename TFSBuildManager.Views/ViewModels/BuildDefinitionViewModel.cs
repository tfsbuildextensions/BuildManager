//-----------------------------------------------------------------------
// <copyright file="BuildDefinitionViewModel.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System;
    using System.IO;
    using Microsoft.TeamFoundation.Build.Client;

    public class BuildDefinitionViewModel : ViewModelBase
    {
        private const string NotAvailable = "n/a";

        public BuildDefinitionViewModel(IBuildDefinition build)
        {
            this.Name = build.Name;
            this.Uri = build.Uri;
            this.TeamProject = build.TeamProject;
            this.ContinuousIntegrationType = build.ContinuousIntegrationType.ToString();    
            this.BuildController = build.BuildController != null ? build.BuildController.Name : NotAvailable;
            this.Process = build.Process != null ? Path.GetFileNameWithoutExtension(build.Process.ServerPath) : NotAvailable;
            this.Description = build.Description;
            this.DefaultDropLocation = build.DefaultDropLocation;
            this.Enabled = build.QueueStatus != DefinitionQueueStatus.Disabled;
        }

        public string Name { get; set; }

        public Uri Uri { get; set; }

        public string TeamProject { get; set; }

        public string ContinuousIntegrationType { get; set; }

        public string BuildController { get; set; }

        public string Process { get; set; }

        public string Description { get; set; }

        public string DefaultDropLocation { get; set; }

        public bool Enabled { get; set; }

        public bool HasProcess
        {
            get
            {
                return string.Compare(this.Process, NotAvailable, StringComparison.Ordinal) == 0;
            }
        }
    }
}