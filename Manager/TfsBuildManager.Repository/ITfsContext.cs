//-----------------------------------------------------------------------
// <copyright file="ITfsContext.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------

namespace TfsBuildManager.Repository
{
    using System;
    using Microsoft.TeamFoundation.Build.Client;

    public interface ITfsContext
    {
        event EventHandler ProjectChanged;

        string SelectedProject { get; }

        string ActiveConnection { get; }

        void ShowBuild(Uri buildUri);

        void EditBuildDefinition(Uri buildDefinition);

        void ShowControllerManager();

        void RemapWorkspaces(IBuildDefinition buildDefinition);
    }
}