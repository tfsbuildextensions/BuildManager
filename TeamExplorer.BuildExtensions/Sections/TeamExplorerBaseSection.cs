using System;
using Microsoft.TeamFoundation.Controls;

namespace BuildTree.Sections
{
    /// <summary>
    /// Team Explorer base section class.
    /// </summary>
    public class TeamExplorerBaseSection : TeamExplorerBase, ITeamExplorerSection
    {
        private string title;
        private bool isExpanded = true;
        private bool isVisible = true;
        private bool isBusy;
        private object sectionContent;

        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.title = value;
                this.RaisePropertyChanged("Title");
            }
        }

        public object SectionContent
        {
            get
            {
                return this.sectionContent;
            }

            set
            {
                this.sectionContent = value;
                this.RaisePropertyChanged("SectionContent");
            }
        }

        public bool IsVisible
        {
            get
            {
                return this.isVisible;
            }

            set
            {
                this.isVisible = value;
                this.RaisePropertyChanged("IsVisible");
            }
        }

        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }

            set
            {
                this.isExpanded = value;
                this.RaisePropertyChanged("IsExpanded");
            }
        }

        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }

            set
            {
                this.isBusy = value;
                this.RaisePropertyChanged("IsBusy");
            }
        }

        public virtual void Initialize(object sender, SectionInitializeEventArgs e)
        {
            this.ServiceProvider = e.ServiceProvider;
        }

        public virtual void SaveContext(object sender, SectionSaveContextEventArgs e)
        {
        }

        public virtual void Loaded(object sender, SectionLoadedEventArgs e)
        {
        }

        public virtual void Refresh()
        {
        }

        public virtual void Cancel()
        {
        }

        public virtual object GetExtensibilityService(Type serviceType)
        {
            return null;
        }
    }
}
