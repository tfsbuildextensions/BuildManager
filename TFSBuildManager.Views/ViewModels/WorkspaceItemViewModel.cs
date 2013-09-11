//-----------------------------------------------------------------------
// <copyright file="WorkspaceItemViewModel.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------

namespace TfsBuildManager.Views.ViewModels
{
    public class WorkspaceItemViewModel
    {
        public string Status { get; set; }

        public string SourceControlFolder { get; set; }

        public string RemappedSourceControlFolder { get; set; }
    }
}
