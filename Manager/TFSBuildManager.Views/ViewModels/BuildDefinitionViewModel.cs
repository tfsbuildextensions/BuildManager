//-----------------------------------------------------------------------
// <copyright file="BuildDefinitionViewModel.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------

using System.Linq;

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
            this.BuildDefinition = build;
            this.Uri = build.Uri;
            this.TeamProject = build.TeamProject;
            this.ContinuousIntegrationType = GetFriendlyTriggerName(build.ContinuousIntegrationType);
            this.BuildController = build.BuildController != null ? build.BuildController.Name : NotAvailable;
            this.Process = build.Process != null ? Path.GetFileNameWithoutExtension(build.Process.ServerPath) : NotAvailable;
            this.Description = build.Description;
            this.DefaultDropLocation = build.DefaultDropLocation;
            this.Id = build.Id;
            this.QueueStatus = build.QueueStatus.ToString();
            this.Enabled = build.QueueStatus != DefinitionQueueStatus.Disabled;
            IsGitProject = build.SourceProviders.Any(s => s.Name == "TFGIT");
            IsTfvcProject = !IsGitProject;
        }


        public IBuildDefinition BuildDefinition { get; set; }

        public string Name { get; set; }

        public string Id { get; set; }

        public Uri Uri { get; set; }

        public string TeamProject { get; set; }

        public string ContinuousIntegrationType { get; set; }

        public string BuildController { get; set; }

        public string QueueStatus { get; set; }
        
        public string Process { get; set; }

        public string Description { get; set; }

        public string DefaultDropLocation { get; set; }

        public bool Enabled { get; set; }

        public bool IsTfvcProject { get; set; }

        public bool IsGitProject { get; set; }

        public bool HasProcess
        {
            get
            {
                return string.Compare(this.Process, NotAvailable, StringComparison.Ordinal) == 0;
            }
        }     
        
        private static string GetFriendlyTriggerName(ContinuousIntegrationType trigger)
        {
            string friendlyName = string.Empty;
            switch (trigger)
            {
                case Microsoft.TeamFoundation.Build.Client.ContinuousIntegrationType.Batch:
                    friendlyName = "Rolling";
                    break;
                case Microsoft.TeamFoundation.Build.Client.ContinuousIntegrationType.Gated:
                    friendlyName = "Gated";
                    break;
                case Microsoft.TeamFoundation.Build.Client.ContinuousIntegrationType.Individual:
                    friendlyName = "CI";
                    break;
                case Microsoft.TeamFoundation.Build.Client.ContinuousIntegrationType.None:
                    friendlyName = "Manual (None)";
                    break;
                case Microsoft.TeamFoundation.Build.Client.ContinuousIntegrationType.Schedule:
                    friendlyName = "Schedule";
                    break;
                case Microsoft.TeamFoundation.Build.Client.ContinuousIntegrationType.ScheduleForced:
                    friendlyName = "Forced Schedule";
                    break;
            }

            return friendlyName;
        }
    }
}