//-----------------------------------------------------------------------
// <copyright file="TestResultFailedItem.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace WordDocumentGenerator.Client.Entities
{
    public class TestResultFailedItem
    {
        public string TestResultFailedListTitle { get; set; }

        public string TestResultFailedListStatus { get; set; }

        public string TestResultFailedListReason { get; set; }
    }
}