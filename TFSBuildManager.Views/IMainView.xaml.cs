//-----------------------------------------------------------------------
// <copyright file="IMainView.xaml.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System;
    using System.Collections.Generic;

    using TfsBuildManager.Views.ViewModels;

    public interface IMainView
    {
        IEnumerable<BuildDefinitionViewModel> SelectedItems { get; }

        IEnumerable<BuildViewModel> SelectedBuilds { get; }

        IEnumerable<BuildViewModel> SelectedActiveBuilds { get; }

        IEnumerable<BuildTemplateViewModel> SelectedBuildProcessTemplates { get; }

        IEnumerable<BuildResourceViewModel> SelectedBuildResources { get; }

        string SelectedTeamProject { get; }

        void DisplayError(Exception ex);
    }
}