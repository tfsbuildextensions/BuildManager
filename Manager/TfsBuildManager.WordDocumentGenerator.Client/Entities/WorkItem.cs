//-----------------------------------------------------------------------
// <copyright file="WorkItem.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace WordDocumentGenerator.Client.Entities
{
    using System;

    public class WorkItem
    {
        public string WorkItemId { get; set; }

        public string WorkItemTitle { get; set; }

        public string WorkItemState { get; set; }

        public string WorkItemType { get; set; }

        public string WorkItemAssignedTo { get; set; }

        public Uri WorkItemUri { get; set; }

        public string WorkItemIterationPath { get; set; }

        public string WorkItemAreaPath { get; set; }
    }
}