//-----------------------------------------------------------------------
// <copyright file="BuildControllerViewModel.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------

namespace TfsBuildManager.Views.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.TeamFoundation.Build.Client;
    using System.Linq;
    using System;

    public class BuildControllerViewModel : ViewModelBase
    {
        public BuildControllerViewModel(IBuildController controller)
        {
            this.Name = controller.Name;
            this.Tags = string.Empty;
            foreach (var tag in controller.Tags)
            {
                this.Tags += tag + "\n";
            }

            this.Tags = this.Tags.TrimEnd('\n');
            if (string.IsNullOrEmpty(this.Tags))
            {
                this.Tags = "<No Tags>";
            }
        }

        public string Name { get; set; }

        public string Tags { get; set; }
    }

    public class DropLocationViewModel : ViewModelBase
    {
        public DropLocationViewModel()
        {
            this.ModeReplace = true;
            this.Macros = new Dictionary<string, string>();
        }

        public string SearchTxt { get; set; }

        public string ReplaceTxt { get; set; }

        public bool UpdateExistingBuilds { get; set; }

        public bool ModeReplace { get; set; }

        public string SetDropLocation { get; set; }

        public Dictionary<string, string> Macros { get; private set; }

        public void AddMacro(string macro, string value)
        {
            this.Macros.Add(macro, value);
        }
    }

    public class TriggerViewModel : ViewModelBase
    {
        public TriggerViewModel()
        {
            this.Manual = true;
            this.TriggerType = DefinitionTriggerType.ScheduleForced;
        }

        public bool Manual { get; set; }

        public int Minutes { get; set; }

        public int Submissions { get; set; }

        public DefinitionTriggerType TriggerType { get; set; }

        public ScheduleDays ScheduleDays { get; set; }

        public DateTime ScheduleTime { get; set; }

        public TimeZoneInfo TimeZoneInfo { get; set; }
    }

    public class BuildControllerListViewModel : ViewModelBase
    {
        public BuildControllerListViewModel(IEnumerable<IBuildController> controllers)
        {
            this.BuildControllers = new ObservableCollection<BuildControllerViewModel>();
            foreach (var c in controllers)
            {
                this.BuildControllers.Add(new BuildControllerViewModel(c));
            }
        }

        public ObservableCollection<BuildControllerViewModel> BuildControllers { get; private set; }
    }

    public class TagListViewModel : ViewModelBase
    {
        public TagListViewModel(IEnumerable<TagViewModel> tags)
        {
            this.Tags = new ObservableCollection<TagViewModel>();
            foreach (var t in tags)
            {
                this.Tags.Add(t);
            }
        }

        public ObservableCollection<TagViewModel> Tags { get; private set; }
    }

    public class TagViewModel : ViewModelBase
    {
        public TagViewModel(string name, bool allAgentsHaveThisTag, bool someAgentsHaveThisTag)
        {
            this.Name = name;
            this.AllAgentsHaveThisTag = allAgentsHaveThisTag;
            this.SomeAgentsHaveThisTag = someAgentsHaveThisTag;
        }

        public string Name { get; set; }

        public bool AllAgentsHaveThisTag { get; set; }

        public bool SomeAgentsHaveThisTag { get; set; }
    }

    public class TeamProjectViewModel : ViewModelBase
    {
        public TeamProjectViewModel(string project)
        {
            this.Name = project;
        }

        public string Name { get; set; }
    }

    public class TeamProjectListViewModel : ViewModelBase
    {
        public TeamProjectListViewModel(IEnumerable<string> projects)
        {
            this.TeamProjects = new ObservableCollection<TeamProjectViewModel>();
            foreach (var p in projects)
            {
                this.TeamProjects.Add(new TeamProjectViewModel(p));
            }
        }

        public ObservableCollection<TeamProjectViewModel> TeamProjects { get; private set; }
    }

    public class BuildQualityViewModel : ViewModelBase
    {
        public BuildQualityViewModel(string project)
        {
            this.Name = project;
        }

        public string Name { get; set; }
    }

    public class BuildQualityListViewModel : ViewModelBase
    {
        public BuildQualityListViewModel(IEnumerable<string> projects)
        {
            this.BuildQualities = new ObservableCollection<BuildQualityViewModel>();
            foreach (var p in projects)
            {
                this.BuildQualities.Add(new BuildQualityViewModel(p));
            }
        }

        public ObservableCollection<BuildQualityViewModel> BuildQualities { get; private set; }
    }
}