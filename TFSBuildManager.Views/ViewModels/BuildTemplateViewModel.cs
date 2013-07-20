//-----------------------------------------------------------------------
// <copyright file="BuildTemplateViewModel.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.TeamFoundation.Build.Client;

    public class BuildTemplateListViewModel : ViewModelBase
    {
        public BuildTemplateListViewModel(IEnumerable<IProcessTemplate> templates)
        {
            this.BuildTemplates = new ObservableCollection<BuildTemplateViewModel>();
            foreach (var t in templates)
            {
                this.BuildTemplates.Add(new BuildTemplateViewModel(t));
            }
        }

        public ObservableCollection<BuildTemplateViewModel> BuildTemplates { get; private set; }
    }

    public class BuildTemplateViewModel : ViewModelBase
    {
        public BuildTemplateViewModel(IProcessTemplate template)
        {
            this.ServerPath = template.ServerPath;
            this.TeamProject = template.TeamProject;
            this.TemplateType = template.TemplateType.ToString();
        }

        public string TemplateType { get; set; }

        public string TeamProject { get; set; }

        public string ServerPath { get; set; }
    }
}
