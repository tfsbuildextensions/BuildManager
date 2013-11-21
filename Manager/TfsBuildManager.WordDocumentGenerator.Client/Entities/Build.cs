//-----------------------------------------------------------------------
// <copyright file="Build.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace WordDocumentGenerator.Client.Entities
{
    using System;

    public class Build
    {
        public string BuildNumber { get; set; }

        public string BuildState { get; set; }

        public string BuildReason { get; set; }

        public string BuildQuality { get; set; }

        public string BuildRequestedBy { get; set; }

        public string BuildDefinition { get; set; }

        public string BuildTeamProject { get; set; }

        public string BuildStartTime { get; set; }

        public string BuildEndTime { get; set; }

        public string BuildTotalExecutionTimeInSeconds { get; set; }

        public string BuildController { get; set; }

        public string BuildCompletedNoOfDaysAgo { get; set; }

        public Uri BuildUri { get; set; }

        public string BuildRetainIndefinitely { get; set; }

        public string BuildDropLocation { get; set; }

        public string BuildIsDeleted { get; set; }

        public string BuildSourceGetVersion { get; set; }

        public string BuildShelvesetName { get; set; }

        public string BuildLabelName { get; set; }

        public string BuildLogLocation { get; set; }

        public string BuildLastChangedOn { get; set; }

        public string BuildLastModifiedNoOfDaysAgo { get; set; }

        public string BuildLastChangedBy { get; set; }

        public string BuildTestStatus { get; set; }
    }
}