//-----------------------------------------------------------------------
// <copyright file="BuildDefinitionViewModel.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Microsoft.TeamFoundation.Build.Client;
    using Microsoft.TeamFoundation.Build.Common;
    using Microsoft.TeamFoundation.Build.Workflow;
    using Microsoft.TeamFoundation.Build.Workflow.Activities;
    using Newtonsoft.Json;

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
            if (build.ContinuousIntegrationType == Microsoft.TeamFoundation.Build.Client.ContinuousIntegrationType.Schedule || build.ContinuousIntegrationType == Microsoft.TeamFoundation.Build.Client.ContinuousIntegrationType.ScheduleForced)
            {
                this.ContinuousIntegrationType = string.Format("{0} - {1}", this.ContinuousIntegrationType, ConvertTime(build.Schedules[0].StartTime.ToString(CultureInfo.CurrentCulture)));
            }
            else if (build.ContinuousIntegrationType == Microsoft.TeamFoundation.Build.Client.ContinuousIntegrationType.Gated)
            {
                this.ContinuousIntegrationType = string.Format("{0} - {1}", this.ContinuousIntegrationType, build.BatchSize);
            }
            else if (build.ContinuousIntegrationType == Microsoft.TeamFoundation.Build.Client.ContinuousIntegrationType.Batch)
            {
                this.ContinuousIntegrationType = string.Format("{0} - {1}", this.ContinuousIntegrationType, build.ContinuousIntegrationQuietPeriod);
            }

            this.BuildController = build.BuildController != null ? build.BuildController.Name : NotAvailable;
            this.Process = build.Process != null ? Path.GetFileNameWithoutExtension(build.Process.ServerPath) : NotAvailable;
            this.Description = build.Description;
            this.DefaultDropLocation = build.DefaultDropLocation;
            this.Id = Convert.ToInt32(build.Id);
            this.QueueStatus = build.QueueStatus.ToString();
            this.Enabled = build.QueueStatus != DefinitionQueueStatus.Disabled;
            this.IsGitProject = build.SourceProviders.Any(s => s.Name == "TFGIT");
            this.IsTfvcProject = !this.IsGitProject;
            this.LastModifiedBy = build.Workspace.LastModifiedBy;
            this.LastModifiedDate = build.Workspace.LastModifiedDate;

            try
            {
                var parameters = WorkflowHelpers.DeserializeProcessParameters(build.ProcessParameters);
                this.OutputLocation = parameters.ContainsKey("OutputLocation") ? parameters["OutputLocation"].ToString() : "SingleFolder";

                if (parameters.ContainsKey("AgentSettings"))
                {
                    try
                    {
                        if (parameters["AgentSettings"].GetType() == typeof(AgentSettings))
                        {
                            AgentSettings ags = (AgentSettings)parameters["AgentSettings"];
                            if (ags.HasTags)
                            {
                                this.AgentTags = ags.Tags.ToString();
                            }
                        }
                        else if (parameters["AgentSettings"].GetType() == typeof(BuildParameter))
                        {
                            BuildParameter ags = (BuildParameter)parameters["AgentSettings"];
                            {
                                var jstt = JsonConvert.DeserializeObject<AgentSettingsBuildParameter>(ags.Json);
                                if (jstt.Tags != null && jstt.Tags.Count > 0)
                                {
                                    this.AgentTags = string.Join(", ", jstt.Tags.ToArray());
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        this.AgentTags = "Failed to determine";
                    }
                }
            }
            catch (Exception)
            {
                this.OutputLocation = "Failed to determine";
            }
        }

        public IBuildDefinition BuildDefinition { get; set; }

        public string Name { get; set; }

        public int Id { get; set; }

        public Uri Uri { get; set; }

        public string TeamProject { get; set; }

        public string ContinuousIntegrationType { get; set; }

        public string BuildController { get; set; }

        public string AgentTags { get; set; }

        public string QueueStatus { get; set; }
        
        public string Process { get; set; }

        public string Description { get; set; }

        public string DefaultDropLocation { get; set; }

        public string OutputLocation { get; set; }

        public bool Enabled { get; set; }

        public bool IsTfvcProject { get; set; }

        public bool IsGitProject { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }
        
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
                    friendlyName = "Manual";
                    break;
                case Microsoft.TeamFoundation.Build.Client.ContinuousIntegrationType.Schedule:
                    friendlyName = "Schedule";
                    break;
                case Microsoft.TeamFoundation.Build.Client.ContinuousIntegrationType.ScheduleForced:
                    friendlyName = "Schedule Forced";
                    break;
            }

            return friendlyName;
        }

        private static string ConvertTime(string secondsSinceMidnight)
        {
            int value = Convert.ToInt32(secondsSinceMidnight);
            int hours = value / 3600;
            int minutes = (value / 60) - (hours * 60);
            int seconds = value - ((hours * 3600) + (minutes * 60));
            string time = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);

            return time;
        }
    }
}