//-----------------------------------------------------------------------
// <copyright file="RemapWorkspacesViewModel.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------

namespace TfsBuildManager.Views.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.TeamFoundation.Build.Client;

    public class RemapWorkspacesViewModel : ViewModelBase
    {
        private ObservableCollection<WorkspaceItemViewModel> workspaceItems;
        
        private IBuildDefinition buildDefinition;

        public RemapWorkspacesViewModel(IBuildDefinition buildDefinition)
        {
            buildDefinition.Refresh();
            this.buildDefinition = buildDefinition;
            this.workspaceItems = InitializeWorkSpaceItems(buildDefinition);
        }

        public string TextToSearch { get; set; }

        public string ReplacementText { get; set; }

        public ObservableCollection<WorkspaceItemViewModel> WorkspaceItems 
        {
            get
            {
                return this.workspaceItems;
            }
        }

        public void RemapWorkspaces()
        {
            if (!string.IsNullOrWhiteSpace(this.TextToSearch) && !string.IsNullOrWhiteSpace(this.ReplacementText))
            {
                foreach (var workSpaceItem in this.workspaceItems)
                {
                    workSpaceItem.RemappedSourceControlFolder = Regex.Replace(workSpaceItem.SourceControlFolder, Regex.Escape(this.TextToSearch), this.ReplacementText, RegexOptions.IgnoreCase);                    
                }
            }            
        }

        public void Save()
        {
            foreach (var mapping in this.buildDefinition.Workspace.Mappings)
            {
                var workspaceItem = this.WorkspaceItems.FirstOrDefault(mp => string.Compare(mp.SourceControlFolder, mapping.ServerItem, StringComparison.OrdinalIgnoreCase) == 0);
                mapping.ServerItem = (workspaceItem != null) ? workspaceItem.RemappedSourceControlFolder : mapping.ServerItem;
            }            

            this.buildDefinition.Save();
        }

        private static ObservableCollection<WorkspaceItemViewModel> InitializeWorkSpaceItems(IBuildDefinition buildDefinition)
        {   
            var workspaceMappings = new ObservableCollection<WorkspaceItemViewModel>();
            foreach (var mapping in buildDefinition.Workspace.Mappings)
            {
                workspaceMappings.Add(new WorkspaceItemViewModel()
                {
                    Status = (mapping.MappingType == WorkspaceMappingType.Map) ? "Active" : "Cloaked",
                    SourceControlFolder = mapping.ServerItem,
                    RemappedSourceControlFolder = mapping.ServerItem
                });
            }

            return workspaceMappings;
        }
    }
}
