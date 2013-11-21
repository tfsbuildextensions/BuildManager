//-----------------------------------------------------------------------
// <copyright file="DateFilterCollection.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------

namespace TfsBuildManager.Views
{
    using System.Collections.ObjectModel;

    public class DateFilterCollection : ObservableCollection<BuildDateFilter>
    {
        public DateFilterCollection()
        {
            this.Add(new BuildDateFilter { Name = "Today", Value = DateFilter.Today });
            this.Add(new BuildDateFilter { Name = "Last 24 hours", Value = DateFilter.OneDay });
            this.Add(new BuildDateFilter { Name = "Last 48 hours", Value = DateFilter.TwoDays });
            this.Add(new BuildDateFilter { Name = "Last 7 days", Value = DateFilter.OneWeek });
            this.Add(new BuildDateFilter { Name = "Last 14 days", Value = DateFilter.TwoWeeks });
            this.Add(new BuildDateFilter { Name = "Last 28 days", Value = DateFilter.FourWeeks });
            this.Add(new BuildDateFilter { Name = "<Any Time>", Value = DateFilter.Anytime });
        }
    }
}