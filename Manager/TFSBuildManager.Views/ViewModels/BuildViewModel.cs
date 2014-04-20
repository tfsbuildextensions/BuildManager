//-----------------------------------------------------------------------
// <copyright file="BuildViewModel.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------

namespace TfsBuildManager.Views
{
    using System;
    using Microsoft.TeamFoundation.Build.Client;
    using Microsoft.TeamFoundation.Build.Common;

    public class BuildViewModel : ViewModelBase
    {
        private readonly bool keep;

        public BuildViewModel(IBuildDetail build)
        {
            this.FullBuildDetail = build;
            this.FullBuildDefinition = build.BuildDefinition;
            this.Name = build.BuildNumber;
            this.BuildDefinition = build.BuildDefinition.Name;
            this.BuildDefinitionUri = build.BuildDefinitionUri;
            this.TeamProject = build.TeamProject;
            this.BuildStatus = build.Status.ToString();
            this.BuildController = build.BuildController != null ? build.BuildController.Name : "n/a";
            this.RequestedBy = build.RequestedFor;
            this.QueuedTime = build.Requests.Count > 0 ? build.Requests[0].QueueTime.ToString("g") : build.StartTime.ToString("g");
            this.StartTime = build.StartTime.ToString("g");
            this.SortableStartTime = build.StartTime.ToString("s");
            this.SortableQueuedTime = build.Requests.Count > 0 ? build.Requests[0].QueueTime.ToString("s") : build.StartTime.ToString("s");
            this.DropLocation = build.DropLocation;
            if (build.BuildFinished)
            {
                this.FinishTime = build.FinishTime.ToString("g");
                this.SortableFinishTime = build.FinishTime.ToString("s");
                this.Duration = string.Format("{0:hh\\:mm\\:ss}", build.FinishTime - build.StartTime);
            }

            this.Uri = build.Uri;
            this.SortableUri = build.Uri.ToString();
            this.keep = build.KeepForever;
            this.Quality = build.Quality;
            this.BuildAgent = GetBuildAgentName(build);
        }

        public BuildViewModel(IQueuedBuild build)
        {
            this.QueuedBuildDetail = build;
            if (build.Build != null)
            {
                string[] refreshAllDetails = { "*" };
                build.Build.Refresh(refreshAllDetails, QueryOptions.Agents | QueryOptions.BatchedRequests);
                this.Name = build.Build.BuildNumber;
            }
            else
            {
                this.Name = build.Id.ToString();
            }
            
            this.FullBuildDefinition = build.BuildDefinition;
            this.BuildDefinition = build.BuildDefinition != null ? build.BuildDefinition.Name : "n/a";
            this.BuildDefinitionUri = build.BuildDefinition != null ? build.BuildDefinitionUri : null;
            this.TeamProject = build.TeamProject;
            this.BuildStatus = build.Status.ToString();
            this.BuildController = build.BuildController != null ? build.BuildController.Name : "n/a";
            this.RequestedBy = build.RequestedFor;
            this.Priority = build.Priority.ToString();

            if (build.Build != null)
            {
                this.StartTime = build.Build.StartTime.ToString("g");
                this.SortableStartTime = build.Build.StartTime.ToString("s");
                if (build.Build.BuildFinished)
                {
                    this.FinishTime = build.Build.FinishTime.ToString("g");
                    this.SortableFinishTime = build.Build.FinishTime.ToString("s");
                    this.Duration = string.Format("{0:hh\\:mm\\:ss}", build.Build.FinishTime - build.Build.StartTime);
                }

                this.QueuedTime = build.Build.Requests.Count > 0 ? build.Build.Requests[0].QueueTime.ToString("g") : build.Build.StartTime.ToString("g");
                this.SortableQueuedTime = build.Build.Requests.Count > 0 ? build.Build.Requests[0].QueueTime.ToString("s") : build.Build.StartTime.ToString("s");
                this.BuildAgent = GetBuildAgentName(build.Build);
                this.Uri = build.Build.Uri;
            }
        }

        public IBuildDefinition FullBuildDefinition { get; set; }

        public IBuildDetail FullBuildDetail { get; set; }

        public IQueuedBuild QueuedBuildDetail { get; set; }

        public string Name { get; set; }

        public string BuildDefinition { get; set; }

        public Uri BuildDefinitionUri { get; set; }

        public string TeamProject { get; set; }

        public string BuildStatus { get; set; }

        public string BuildController { get; set; }

        public string DropLocation { get; set; }

        public string BuildAgent { get; set; }

        public string QueuedTime { get; set; }

        public string SortableQueuedTime { get; set; }

        public string StartTime { get; set; }

        public string SortableStartTime { get; set; }

        public string FinishTime { get; set; }

        public string SortableFinishTime { get; set; }

        public string Duration { get; set; }
        
        public Uri Uri { get; set; }

        public string SortableUri { get; set; }

        public string RequestedBy { get; set; }

        public string Priority { get; set; }

        public string Quality { get; set; }

        public string Status
        {
            get { return "Graphics\\" + this.BuildStatus + ".png"; }
        }

        public string KeepForever
        {
            get { return this.keep ? "Graphics\\lock_16.png" : string.Empty; }
        }

        private static string GetBuildAgentName(IBuildDetail build)
        {
            var buildInformationNodes = build.Information.GetNodesByType("AgentScopeActivityTracking", true);
            if (buildInformationNodes != null)
            {
                var node = buildInformationNodes.Find(s => s.Fields.ContainsKey(InformationFields.ReservedAgentName));
                return node != null ? node.Fields[InformationFields.ReservedAgentName] : string.Empty;
            }

            return string.Empty;
        }
    }
}