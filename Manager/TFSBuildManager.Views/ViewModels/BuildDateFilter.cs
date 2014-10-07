//-----------------------------------------------------------------------
// <copyright file="BuildDateFilter.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------

namespace TfsBuildManager.Views
{
    using System;

    public class BuildDateFilter
    {
        public string Name { get; set; }

        public DateFilter Value { get; set; }

        public DateTime TimeSpan
        {
            get
            {
                switch (this.Value)
                {
                    case DateFilter.Today:
                        return DateTime.Now.Date;
                    case DateFilter.OneDay:
                        return DateTime.Now.Date.AddDays(-1).Date;
                    case DateFilter.TwoDays:
                        return DateTime.Now.Date.AddDays(-2).Date;
                    case DateFilter.OneWeek:
                        return DateTime.Now.Date.AddDays(-7).Date;
                    case DateFilter.TwoWeeks:
                        return DateTime.Now.Date.AddDays(-14).Date;
                    case DateFilter.FourWeeks:
                        return DateTime.Now.Date.AddDays(-28).Date;
                    case DateFilter.Anytime:
                        return DateTime.MinValue;
                    default:
                        return DateTime.Now.Date;
                }
            }
        }
    }
}